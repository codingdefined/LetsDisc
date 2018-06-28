using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LetsDisc.Questions.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetsDisc.Questions
{
    public interface IQuestionAppService : IApplicationService
    {
        PagedResultDto<QuestionDto> GetQuestions(GetQuestionsInput input);
        void CreateQuestion(CreateQuestionInput input);
        GetQuestionOutput GetQuestion(GetQuestionInput input);
        VoteChangeOutput QuestionVoteUp(EntityDto input);
        VoteChangeOutput QuestionVoteDown(EntityDto input);
        VoteChangeOutput AnswerVoteUp(EntityDto input);
        VoteChangeOutput AnswerVoteDown(EntityDto input);
        SubmitAnswerOutput SubmitAnswer(SubmitAnswerInput input);
        void AcceptAnswer(EntityDto input);
    }
}
