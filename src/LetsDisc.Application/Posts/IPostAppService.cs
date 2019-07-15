using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LetsDisc.Posts.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetsDisc.Posts
{ 
    public interface IPostAppService : IAsyncCrudAppService<PostDto, int, PagedResultRequestDto, CreatePostDto, PostDto>
    {
        Task<PagedResultDto<PostDto>> GetQuestions(PagedResultRequestDto input, string tag);
        Task<PostWithAnswers> GetPost(int id);
        Task<VoteChangeOutput> PostVoteUp(int id);
        Task<VoteChangeOutput> PostVoteDown(int id);
        Task<PostWithVoteInfo> SubmitAnswer(SubmitAnswerInput input);
        Task<PostWithAnswers> UpdateQuestion(PostDto input);
    }
}
