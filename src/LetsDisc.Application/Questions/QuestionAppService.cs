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

        [AbpAuthorize(PermissionNames.Pages_Questions_Create)]
        public void CreateQuestion(CreateQuestionInput input)
        {
           _questionRepository.Insert(new Question(input.Title, input.Body));
        }

        public GetQuestionOutput GetQuestion(GetQuestionInput input)
        {
            var question =
                _questionRepository
                    .GetAll()
                    .Include(q => q.CreatorUser)
                    .Include(q => q.Answers)
                    .Include("Answers.CreatorUser")
                    .FirstOrDefault(q => q.Id == input.Id);

            if (question == null)
            {
                throw new UserFriendlyException("There is no such a question. Maybe it's deleted.");
            }

            if (input.IncrementViewCount)
            {
                question.ViewCount++;
            }

            return new GetQuestionOutput
            {
                Question = question.MapTo<QuestionWithAnswersDto>()
            };
        }

        [AbpAuthorize(PermissionNames.Pages_Questions_Create)]
        public VoteChangeOutput QuestionVoteUp(EntityDto input)
        {
            var question = _questionRepository.Get(input.Id);
            question.UpvoteCount++;
            _userVoteForQuestionRepository.Insert(new UserVoteForQuestion(input.Id, AbpSession.UserId.GetValueOrDefault(), true));
            return new VoteChangeOutput(question.UpvoteCount);
        }

        [AbpAuthorize(PermissionNames.Pages_Questions_Create)]
        public VoteChangeOutput QuestionVoteDown(EntityDto input)
        {
            var question = _questionRepository.Get(input.Id);
            question.UpvoteCount--;
            _userVoteForQuestionRepository.Insert(new UserVoteForQuestion(input.Id, AbpSession.UserId.GetValueOrDefault(), true));
            return new VoteChangeOutput(question.UpvoteCount);
        }

        [AbpAuthorize(PermissionNames.Pages_Questions_Create)]
        public VoteChangeOutput AnswerVoteUp(EntityDto input)
        {
            var answer = _answerRepository.Get(input.Id);
            answer.UpvoteCount++;
            return new VoteChangeOutput(answer.UpvoteCount);
        }

        [AbpAuthorize(PermissionNames.Pages_Questions_Create)]
        public VoteChangeOutput AnswerVoteDown(EntityDto input)
        {
            var answer = _answerRepository.Get(input.Id);
            answer.UpvoteCount--;
            return new VoteChangeOutput(answer.UpvoteCount);
        }

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

        public void AcceptAnswer(EntityDto input)
        {
            var answer = _answerRepository.Get(input.Id);
            _questionDomainService.AcceptAnswer(answer);
        }
    }
}
