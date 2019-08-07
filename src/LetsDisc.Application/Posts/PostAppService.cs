using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using LetsDisc.PostDetails;
using LetsDisc.Posts.Dto;
using LetsDisc.Tags;
using LetsDisc.Votes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsDisc.Posts
{
    public enum VoteTypes
    {
        Accepted = 1,
        Upvote,
        Downvote,
        Favorite,
        Spam,
        ModeratorReview
    }
    public enum PostTypes
    {
        Question = 1,
        Answer
    }
    public class PostAppService : AsyncCrudAppService<Post, PostDto, int, PagedAndSortedResultRequestDto, CreatePostDto, PostDto>, IPostAppService
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<Vote> _voteRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<PostTag> _postTagRepository;

        public PostAppService(IRepository<Post> postRepository, 
                              IRepository<Vote> voteRepository, 
                              IRepository<Tag> tagRepository, 
                              IRepository<PostTag> postTagRepository) : base(postRepository)
        {
            _postRepository = postRepository;
            _voteRepository = voteRepository;
            _tagRepository = tagRepository;
            _postTagRepository = postTagRepository;
        }

        public override async Task<PostDto> Create(CreatePostDto input)
        {
            CheckCreatePermission();

            var post = ObjectMapper.Map<Post>(input);

            var newPostId = await _postRepository.InsertAndGetIdAsync(post);

            await insertTagData(input.Tags, newPostId);
            CurrentUnitOfWork.SaveChanges();

            return MapToEntityDto(post);
        }

        private async Task insertTagData(string tags, int postId)
        {
            var tagsArray = tags.Split(',');
            foreach (var tag in tagsArray)
            {
                var tagInDb = await _tagRepository.FirstOrDefaultAsync(t => t.TagName == tag);
                var tagId = tagInDb == null ? await _tagRepository.InsertAndGetIdAsync(new Tag
                                              {
                                                TagName = tag,
                                                Count = 1
                                              }) 
                                            : tagInDb.Id;
                var postTagInDb = await _postTagRepository.FirstOrDefaultAsync(pt => pt.PostId == postId && pt.TagId == tagId);
                if (postTagInDb == null)
                {
                    await _postTagRepository.InsertAsync(new PostTag
                    {
                        PostId = postId,
                        TagId = tagId
                    });
                    if(tagInDb != null)
                    {
                        tagInDb.Count++;
                    }
                }
            }
        }

        private async Task updateTagData(string tags, string oldTags, int postId)
        {
            var tagsArray = tags.Split(',');
            var oldTagsArray = oldTags.Split(',');
            var addedTags = String.Join(",", tagsArray.Except(oldTagsArray).ToArray());
            var deletedTags = oldTagsArray.Except(tagsArray);
            if(addedTags.Length > 0)
            {
                await insertTagData(addedTags, postId);
            }
            foreach(var tag in deletedTags)
            {
                var tagInDb = await _tagRepository.FirstOrDefaultAsync(t => t.TagName == tag);
                if(tagInDb != null)
                {
                    var postTagInDb = await _postTagRepository.FirstOrDefaultAsync(pt => pt.PostId == postId && pt.TagId == tagInDb.Id);
                    if(postTagInDb != null)
                    {
                        await _postTagRepository.DeleteAsync(a => a.Id == postTagInDb.Id);
                        tagInDb.Count--;
                    }
                }
            }
        }

        public async Task<PostWithAnswers> UpdateQuestion(PostDto input)
        {
            CheckUpdatePermission();

            var post = await _postRepository.GetAsync(input.Id);
            input.CreationTime = post.CreationTime;
            input.CreatorUserId = post.CreatorUserId;
            await updateTagData(input.Tags, post.Tags, input.Id);

            MapToEntity(input, post);
            await _postRepository.UpdateAsync(post);

            return await GetPost(input.Id);
        }

        public override async Task Delete(EntityDto<int> input)
        {
            CheckDeletePermission();
            var post = await _postRepository.GetAsync(input.Id);
            await _postRepository.DeleteAsync(post);
        }

        protected override void MapToEntity(PostDto input, Post post)
        {
            ObjectMapper.Map(input, post);
        }

        protected override PostDto MapToEntityDto(Post post)
        {
            var postDto = base.MapToEntityDto(post);
            return postDto;
        }

        protected override IQueryable<Post> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            return base.CreateFilteredQuery(input);
        }

        public async Task<PostWithAnswers> GetPost(int id)
        {
            var post = await _postRepository.GetAllIncluding(p => p.CreatorUser).FirstOrDefaultAsync(x => x.Id == id);
            var userVoted = await _voteRepository.FirstOrDefaultAsync(x => x.PostId == post.Id && x.CreatorUserId == AbpSession.UserId);
            var answers = await _postRepository.GetAllIncluding(p => p.CreatorUser).Where(x => x.ParentId == id).ToArrayAsync();
            List<PostWithVoteInfo> answersWithVoteInfo = new List<PostWithVoteInfo>();
            foreach(var answer in answers)
            {
                var userVotedAnswer = await _voteRepository.FirstOrDefaultAsync(x => x.PostId == answer.Id && x.CreatorUserId == AbpSession.UserId);
                var answerWithVoteInfo = new PostWithVoteInfo
                {
                    Post = answer.MapTo<PostDto>(),
                    Upvote = userVotedAnswer != null && userVotedAnswer.VoteTypeId == (int)VoteTypes.Upvote ? true : false,
                    Downvote = userVotedAnswer != null && userVotedAnswer.VoteTypeId == (int)VoteTypes.Downvote ? true : false
                };
                answersWithVoteInfo.Add(answerWithVoteInfo);
            }

            answersWithVoteInfo = answersWithVoteInfo.OrderByDescending(a => a.Post.Score).ThenBy(a => a.Post.CreationTime).ToList();

            var postWithVoteInfo = new PostWithVoteInfo
            {
                Post = post.MapTo<PostDto>(),
                Upvote = userVoted != null && userVoted.VoteTypeId == (int)VoteTypes.Upvote ? true : false,
                Downvote = userVoted != null && userVoted.VoteTypeId == (int)VoteTypes.Downvote ? true : false
            };

            if (post == null)
            {
                throw new EntityNotFoundException(typeof(Post), id);
            }

            post.ViewCount++;

            return new PostWithAnswers
            {
                Post = postWithVoteInfo,
                Answers = answersWithVoteInfo
            };
        }

        // Getting all questions on the Home Page
        public async Task<PagedResultDto<PostDto>> GetQuestions(PagedAndSortedResultRequestDto input, string tag)
        {
            int questionCount;
            IQueryable<Post> questions;

            if (!(tag == null || tag == ""))
            {
                questionCount = await _postRepository.CountAsync(p => p.PostTypeId == (int)PostTypes.Question && p.Tags.Contains(tag));
                questions = _postRepository
                                    .GetAll()
                                    .Include(a => a.CreatorUser)
                                    .Where(a => a.PostTypeId == (int)PostTypes.Question && a.Tags.Contains(tag));
            }
            else
            {
                questionCount = await _postRepository.CountAsync(p => p.PostTypeId == (int)PostTypes.Question);
                questions = _postRepository
                                    .GetAll()
                                    .Include(a => a.CreatorUser)
                                    .Where(a => a.PostTypeId == (int)PostTypes.Question);
                                    
            }

            switch (input.Sorting)
            {
                case "votes":
                    questions = questions.OrderByDescending(q => q.Score);
                    break;
                case "newest":
                    questions = questions.OrderByDescending(q => q.CreationTime);
                    break;
                case "viewed":
                    questions = questions.OrderByDescending(q => q.ViewCount);
                    break;
                default:
                    questions = questions.OrderByDescending(q => q.CreationTime);
                    break;
            }

            var questionList = await questions
                                    .Skip(input.SkipCount)
                                    .Take(input.MaxResultCount)
                                    .ToListAsync();

            return new PagedResultDto<PostDto>
            {
                TotalCount = questionCount,
                Items = questionList.MapTo<List<PostDto>>()
            };
        }

        // Getting all questions on the Home Page
        public async Task<PagedResultDto<PostDto>> GetSearchPosts(PagedAndSortedResultRequestDto input, string searchString)
        {
            var searchArray = searchString.Split(" ");
            IQueryable<Post> posts = _postRepository
                                    .GetAll()
                                    .Include(a => a.CreatorUser);

            foreach (var term in searchArray)
            {
                posts = posts.Where(a => a.Body.Contains(term) || a.Title.Contains(term));
            }

            switch (input.Sorting)
            {
                case "votes":
                    posts = posts.OrderByDescending(q => q.Score);
                    break;
                case "newest":
                    posts = posts.OrderByDescending(q => q.CreationTime);
                    break;
                case "viewed":
                    posts = posts.OrderByDescending(q => q.ViewCount);
                    break;
                default:
                    posts = posts.OrderByDescending(q => q.CreationTime);
                    break;
            }

            var postCount = await posts.CountAsync();
            var postList = await posts.Skip(input.SkipCount)
                                    .Take(input.MaxResultCount)
                                    .ToListAsync();

            return new PagedResultDto<PostDto>
            {
                TotalCount = postCount,
                Items = postList.MapTo<List<PostDto>>()
            };
        }

        //Voting Up the Post both Question and Answer, where there will be two things either Upvoting or Downvoting
        public async Task<VoteChangeOutput> PostVoteUp(int id)
        {
            var post = await _postRepository.GetAsync(id);
            var isUserVoted = _voteRepository.FirstOrDefault(userVote => userVote.CreatorUserId == AbpSession.UserId && userVote.PostId == id);
            if (isUserVoted != null)
            {
                if(isUserVoted.VoteTypeId == (int)VoteTypes.Upvote)
                {
                    post.Score--;
                    await _voteRepository.DeleteAsync(isUserVoted.Id);
                }
                else if( isUserVoted.VoteTypeId == (int)VoteTypes.Downvote)
                {
                    post.Score += 2;
                    isUserVoted.VoteTypeId = (int)VoteTypes.Upvote;
                    return ReturnVoteChangeOutput(post, true, false);
                }
            }
            else
            {
                post.Score++;
                await _voteRepository.InsertAsync(new Vote(id, (int)VoteTypes.Upvote));
                return ReturnVoteChangeOutput(post, true, false);
            }
            return ReturnVoteChangeOutput(post, false, false);
        }

        private static VoteChangeOutput ReturnVoteChangeOutput(Post post, bool upvote, bool downvote)
        {
            return new VoteChangeOutput()
            {
                VoteCount = post.Score,
                UpVote = upvote,
                DownVote = downvote,
                PostTypeId = post.PostTypeId,
                PostId = post.Id
            };
        }

        //Voting Down the Post both Question and Answer, where there will be two things either Upvoting or Downvoting
        public async Task<VoteChangeOutput> PostVoteDown(int id)
        {
            var post = await _postRepository.GetAsync(id);
            var isUserVoted = _voteRepository.FirstOrDefault(userVote => userVote.CreatorUserId == AbpSession.UserId && userVote.PostId == id);
            if (isUserVoted != null)
            {
                if (isUserVoted.VoteTypeId == (int)VoteTypes.Downvote)
                {
                    post.Score++;
                    await _voteRepository.DeleteAsync(isUserVoted.Id);
                }
                else if (isUserVoted.VoteTypeId == (int)VoteTypes.Upvote)
                {
                    post.Score -= 2;
                    isUserVoted.VoteTypeId = (int)VoteTypes.Downvote;
                    return ReturnVoteChangeOutput(post, false, true);
                }
            }
            else
            {
                post.Score--;
                await _voteRepository.InsertAsync(new Vote(id, (int)VoteTypes.Downvote));
                return ReturnVoteChangeOutput(post, false, true);
            }
            return ReturnVoteChangeOutput(post, false, false);
        }


        public async Task<PostWithVoteInfo> SubmitAnswer(SubmitAnswerInput input)
        {

            var answer = await _postRepository.InsertAsync(
                new Post()
                {
                    Title = input.Title,
                    Body = input.Body,
                    ParentId = input.QuestionId,
                    CreatorUserId = AbpSession.UserId,
                    PostTypeId = (int)PostTypes.Answer
                });
            var question = await _postRepository.FirstOrDefaultAsync(x => x.Id == input.QuestionId);
            question.AnswerCount++;
            return new PostWithVoteInfo
            {
                Post = answer.MapTo<PostDto>(),
                Upvote = false,
                Downvote = false
            };
        }

        public async Task<PostWithVoteInfo> UpdateAnswer(PostDto input)
        {
            CheckUpdatePermission();

            var post = await _postRepository.GetAsync(input.Id);
            post.Body = input.Body;
            post.LastModificationTime = DateTime.Now;

            await _postRepository.UpdateAsync(post);

            return new PostWithVoteInfo
            {
                Post = post.MapTo<PostDto>(),
                Upvote = false,
                Downvote = false
            };
        }

        protected override IQueryable<Post> ApplySorting(IQueryable<Post> query, PagedAndSortedResultRequestDto input)
        {
            return base.ApplySorting(query, input);
        }
    }
}
