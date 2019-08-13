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
        GetQuestionOutput EditQuestion(QuestionDto input);
        GetQuestionOutput GetQuestion(GetQuestionInput input);
        void AcceptAnswer(EntityDto input);
    }
}
