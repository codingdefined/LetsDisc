using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LetsDisc.EntityFrameworkCore;
using LetsDisc.Questions;
using LetsDisc.Controllers;
using Abp.Application.Services.Dto;
using LetsDisc.Questions.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Http;
using Abp.Runtime.Validation;

namespace LetsDisc.Web.Mvc.Controllers
{
    public class QuestionsController : LetsDiscControllerBase
    {
        private readonly IQuestionAppService _questionAppService;

        public QuestionsController(IQuestionAppService questionAppService)
        {
            _questionAppService = questionAppService;
        }

        // GET: Questions
        public IActionResult Index()
        {
            var letsDiscDbContext = _questionAppService.GetQuestions(new GetQuestionsInput { MaxResultCount = 100 }).Items;
            return View(letsDiscDbContext.ToList());
        }

        // GET: Questions/Details/5
        public IActionResult Details(int? id)
        {
            var cookie = this.HttpContext.Request.Cookies["RefreshFilter"];
            this.RouteData.Values["IsRefreshed"] = cookie != null && cookie.ToString() == this.HttpContext.Request.Path.ToString();
            
            if (id == null)
            {
                return NotFound();
            }
            QuestionWithAnswersDto question;
            if ((bool)this.RouteData.Values["IsRefreshed"] == false)
            {
                question = _questionAppService.GetQuestion(new GetQuestionInput { Id = id.Value, IncrementViewCount = true }).Question;
            }
            else
            {
                question = _questionAppService.GetQuestion(new GetQuestionInput { Id = id.Value, IncrementViewCount = false }).Question;
            }
            
            if (question == null)
            {
                return NotFound();
            }
            this.HttpContext.Response.Cookies.Append("RefreshFilter", this.HttpContext.Request.Path.ToString(), new CookieOptions { Expires=DateTime.Now.AddDays(5) });
            return View(question);
        }

        // GET: Questions/Create
        //[AbpMvcAuthorize]
        public IActionResult Create()
        {
            return View();
        }
    }
}
