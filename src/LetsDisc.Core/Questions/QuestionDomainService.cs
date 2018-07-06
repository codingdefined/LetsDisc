using Abp.Domain.Repositories;
using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetsDisc.Questions
{
    public class QuestionDomainService : DomainService
    {
        private readonly IRepository<Answer> _answerRepository;

        public QuestionDomainService(IRepository<Answer> answerRepository)
        {
            _answerRepository = answerRepository;
        }

        //To Do
        public void AcceptAnswer(Answer answer)
        {
            var previousAcceptedAnswer = _answerRepository.FirstOrDefault(a => a.QuestionId == answer.QuestionId && a.IsAccepted);

            if (previousAcceptedAnswer != null)
            {
                previousAcceptedAnswer.IsAccepted = false;
            }

            answer.IsAccepted = true;
        }
    }
}
