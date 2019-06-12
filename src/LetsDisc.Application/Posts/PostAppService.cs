﻿using Abp.Application.Services;
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

        public async Task<PostWithAnswers> UpdateQuestion(PostDto input)
        {
            CheckUpdatePermission();

            var post = await _postRepository.GetAsync(input.Id);
            input.CreationTime = post.CreationTime;
            input.CreatorUserId = post.CreatorUserId;
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

        protected override IQueryable<Post> CreateFilteredQuery(PagedResultRequestDto input)
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
                    Body = input.Body,
                    ParentId = input.QuestionId,
                    CreatorUserId = AbpSession.UserId,
                    PostTypeId = 2
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
    }
}
