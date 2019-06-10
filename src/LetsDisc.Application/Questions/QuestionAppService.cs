using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using LetsDisc.Authorization.Users;
using LetsDisc.Questions.Dto;
using LetsDisc.Tags;
using Abp.AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Application.Services;
using Abp.Authorization;
using LetsDisc.Authorization;
using Abp.UI;
using Abp.Domain.Uow;

namespace LetsDisc.Questions
{
    public class QuestionAppService : ApplicationService, IQuestionAppService
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Answer> _answerRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<UserVoteForQuestion> _userVoteForQuestionRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly QuestionDomainService _questionDomainService;

        // Getting all the Repositories
        public QuestionAppService(IRepository<Question> questionRepository, IRepository<Answer> answerRepository, IRepository<Tag> tagRepository, IRepository<User, long> userRepository, QuestionDomainService questionDomainService, IUnitOfWorkManager unitOfWorkManager, IRepository<UserVoteForQuestion> userVoteForQuestionRepository)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _tagRepository = tagRepository;
            _userRepository = userRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _questionDomainService = questionDomainService;
            _userVoteForQuestionRepository = userVoteForQuestionRepository;
        }

        // Getting all questions on the Home Page
        public PagedResultDto<QuestionDto> GetQuestions(GetQuestionsInput input)
        {
            var questionCount = _questionRepository.Count();
            var questions = _questionRepository
                                .GetAll()
                                .Include(q => q.CreatorUser)
                                .OrderBy(q => q.CreationTime)
                                .ToList();

            return new PagedResultDto<QuestionDto>
            {
                TotalCount = questionCount,
                Items = questions.MapTo<List<QuestionDto>>()
            };
        }

        // Creating the question
        [AbpAuthorize(PermissionNames.Pages_Questions_Create)]
        public void CreateQuestion(CreateQuestionInput input)
        {
           _questionRepository.Insert(new Question(input.Title, input.Body));
        }

        //Getting a single question based on the id Provided
        public GetQuestionOutput GetQuestion(GetQuestionInput input)
        {
            // We will be including the answers and the users who have voted for this question
            var question =
                _questionRepository
                    .GetAll()
                    .Include(q => q.CreatorUser)
                    .Include(q => q.Answers)
                    .Include(q => q.UsersVoted)
                    .Include("Answers.CreatorUser")
                    .FirstOrDefault(q => q.Id == input.Id);

            if (question == null)
            {
                throw new UserFriendlyException("There is no such a question. Maybe it's deleted.");
            }

            // Incrementing the View if the question function is called
            if (input.IncrementViewCount)
            {
                question.ViewCount++;
            }

            // Mapping the question to another class so that we will have more defined classes which includes Answers and UsersVoted
            return new GetQuestionOutput
            {
                Question = question.MapTo<QuestionWithAnswersDto>()
            };
        }

        //Getting a single question based on the id Provided and editing it
        public GetQuestionOutput EditQuestion(QuestionDto input)
        {
            var question = _questionRepository.Get(input.Id);

            if (question == null)
            {
                throw new UserFriendlyException("There is no such a question. Maybe it's deleted.");
            }

            question.Title = input.Title;
            question.Body = input.Body;

            return new GetQuestionOutput
            {
                Question = question.MapTo<QuestionWithAnswersDto>()
            };
        }

        public void DeleteQuestion(EntityDto<int> input)
        {
            _questionRepository.Delete(input.Id);
        }

        // ToDo
        [AbpAuthorize(PermissionNames.Pages_Answers_Create)]
        public SubmitAnswerOutput SubmitAnswer(SubmitAnswerInput input)
        {
            var question = _questionRepository.Get(input.QuestionId);
            var currentUser = _userRepository.Get(AbpSession.UserId.Value);

            var answer = _answerRepository.Insert(
                new Answer(input.Info)
                {
                    Question = question,
                    CreatorUser = currentUser
                });

            _unitOfWorkManager.Current.SaveChanges();

            return new SubmitAnswerOutput
            {
                Answer = answer.MapTo<AnswerDto>()
            };
        }

        // ToDo
        public void AcceptAnswer(EntityDto input)
        {
            var answer = _answerRepository.Get(input.Id);
            _questionDomainService.AcceptAnswer(answer);
        }
    }
}
