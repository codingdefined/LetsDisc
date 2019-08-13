﻿using Abp.Application.Services.Dto;
using LetsDisc.Questions;
using LetsDisc.Questions.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace LetsDisc.Tests.Questions
{
    public class QuestionAppService_Tests : LetsDiscTestBase
    {
        private readonly IQuestionAppService _questionAppService;

        public QuestionAppService_Tests()
        {
            _questionAppService = LocalIocManager.Resolve<IQuestionAppService>();
        }

        [Fact]
        public void Should_Get_All_Tasks()
        {
            //Act
            var output = _questionAppService.GetQuestions(new GetQuestionsInput());

            //Assert
            output.Items.Count.ShouldBe(0);
        }

        [Fact]
        public void Should_Add_Question_With_Authorized_User()
        {
            AbpSession.UserId = UsingDbContext(context => context.Users.Single(u => u.UserName == "admin" && u.TenantId == AbpSession.TenantId.Value).Id);

            const string questionTitle = "Test question title 1";

            //Question does not exists now
            UsingDbContext(context => context.Questions.FirstOrDefault(q => q.Title == questionTitle).ShouldBe(null));

            //Create the question
            _questionAppService.CreateQuestion(
                new CreateQuestionInput
                {
                    Title = questionTitle,
                    Body = "A dummy question text..."
                });

            //Question is added now
            var question = UsingDbContext(context => context.Questions.FirstOrDefault(q => q.Title == questionTitle));
            question.ShouldNotBe(null);

            //Vote up the question
            //var voteUpOutput = _questionAppService.QuestionVoteUp(new EntityDto(question.Id));
            //voteUpOutput.VoteCount.ShouldBe(1);

            //Vote down the question
            //var voteDownOutput = _questionAppService.QuestionVoteDown(new EntityDto(question.Id));
            //voteDownOutput.VoteCount.ShouldBe(0);
        }

        [Fact]
        public void Should_Not_Add_Question_Without_Login()
        {
            AbpSession.UserId = null; //not logged in

            _questionAppService.CreateQuestion(
                    new CreateQuestionInput
                    {
                        Title = "A dummy title",
                        Body = "A dummy question text..."
                    });
            var question = UsingDbContext(context => context.Questions.FirstOrDefault(q => q.Title == "A dummy title"));
            question.ShouldBe(null);
        }
    }
}
