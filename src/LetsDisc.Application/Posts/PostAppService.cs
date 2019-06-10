using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using LetsDisc.PostDetails;
using LetsDisc.Posts.Dto;
using LetsDisc.Votes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsDisc.Posts
{
    enum VoteTypes
    {
        Accepted = 1,
        Upvote,
        Downvote,
        Favorite,
        Spam,
        ModeratorReview
    }
    public class PostAppService : AsyncCrudAppService<Post, PostDto, int, PagedResultRequestDto, CreatePostDto, PostDto>, IPostAppService
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<Vote> _voteRepository;

        public PostAppService(IRepository<Post> postRepository, IRepository<Vote> voteRepository): base(postRepository)
        {
            _postRepository = postRepository;
            _voteRepository = voteRepository;
        }

        public override async Task<PostDto> Create(CreatePostDto input)
        {
            CheckCreatePermission();

            var post = ObjectMapper.Map<Post>(input);
            await _postRepository.InsertAsync(post);
            CurrentUnitOfWork.SaveChanges();

            return MapToEntityDto(post);
        }

        public override async Task<PostDto> Update(PostDto input)
        {
            CheckUpdatePermission();

            var post = await _postRepository.GetAsync(input.Id);
            MapToEntity(input, post);
            await _postRepository.UpdateAsync(post);

            return await Get(input);
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

        protected override IQueryable<Post> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return base.CreateFilteredQuery(input);
        }

        public async Task<PostWithVoteInfo> GetPost(int id)
        {
            var post = await _postRepository.GetAllIncluding(p => p.CreatorUser).FirstOrDefaultAsync(x => x.Id == id);
            var userVoted = await _voteRepository.FirstOrDefaultAsync(x => x.PostId == post.Id && x.CreatorUserId == AbpSession.UserId);

            if (post == null)
            {
                throw new EntityNotFoundException(typeof(Post), id);
            }

            return new PostWithVoteInfo
            {
                Post = post.MapTo<PostDto>(),
                Upvote = userVoted != null && userVoted.VoteTypeId == (int)VoteTypes.Upvote ? true : false,
                Downvote = userVoted != null && userVoted.VoteTypeId == (int)VoteTypes.Downvote ? true : false
            };
        }

        // Getting all questions on the Home Page
        public async Task<PagedResultDto<PostDto>> GetQuestions(PagedResultRequestDto input)
        {
            var questionCount = await _postRepository.CountAsync(p => p.PostTypeId == 1);
            var questions = await _postRepository
                                .GetAll()
                                .Include(a => a.CreatorUser)
                                .Where(a => a.PostTypeId == 1)
                                .OrderByDescending(a => a.CreationTime)
                                .ToListAsync();

            return new PagedResultDto<PostDto>
            {
                TotalCount = questionCount,
                Items = questions.MapTo<List<PostDto>>()
            };
        }

        //Voting Up the Post both Question and Answer, where there will be two things either Upvoting or Downvoting
        public async Task<VoteChangeOutput> PostVoteUp(int id)
        {
            var question = await _postRepository.GetAsync(id);
            var isUserVoted = _voteRepository.FirstOrDefault(userVote => userVote.CreatorUserId == AbpSession.UserId && userVote.PostId == id);
            if (isUserVoted != null)
            {
                if(isUserVoted.VoteTypeId == (int)VoteTypes.Upvote)
                {
                    question.Score--;
                    await _voteRepository.DeleteAsync(isUserVoted.Id);
                    return new VoteChangeOutput(question.Score, false, false);
                }
                else if( isUserVoted.VoteTypeId == (int)VoteTypes.Downvote)
                {
                    question.Score += 2;
                    isUserVoted.VoteTypeId = (int)VoteTypes.Upvote;
                    return new VoteChangeOutput(question.Score, true, false);
                }
            }
            else
            {
                question.Score++;
                await _voteRepository.InsertAsync(new Vote(id, (int)VoteTypes.Upvote));
                return new VoteChangeOutput(question.Score, true, false);
            }
            return new VoteChangeOutput(question.Score, false, false);
        }

        //Voting Down the Post both Question and Answer, where there will be two things either Upvoting or Downvoting
        public async Task<VoteChangeOutput> PostVoteDown(int id)
        {
            var question = await _postRepository.GetAsync(id);
            var isUserVoted = _voteRepository.FirstOrDefault(userVote => userVote.CreatorUserId == AbpSession.UserId && userVote.PostId == id);
            if (isUserVoted != null)
            {
                if (isUserVoted.VoteTypeId == (int)VoteTypes.Downvote)
                {
                    question.Score++;
                    await _voteRepository.DeleteAsync(isUserVoted.Id);
                    return new VoteChangeOutput(question.Score, false, false);
                }
                else if (isUserVoted.VoteTypeId == (int)VoteTypes.Upvote)
                {
                    question.Score -= 2;
                    isUserVoted.VoteTypeId = (int)VoteTypes.Downvote;
                    return new VoteChangeOutput(question.Score, false, true);
                }
            }
            else
            {
                question.Score--;
                await _voteRepository.InsertAsync(new Vote(id, (int)VoteTypes.Downvote));
                return new VoteChangeOutput(question.Score, false, true);
            }
            return new VoteChangeOutput(question.Score, false, false);
        }
    }
}
