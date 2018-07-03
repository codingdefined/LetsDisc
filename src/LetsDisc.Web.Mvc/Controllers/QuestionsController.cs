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

namespace LetsDisc.Web.Mvc.Controllers
{
    public class QuestionsController : LetsDiscControllerBase
    {
        /*private readonly LetsDiscDbContext _context;

        public QuestionsController(LetsDiscDbContext context)
        {
            _context = context;
        }*/

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
        [AbpMvcAuthorize]
        public IActionResult Create()
        {
            return View();
        }
        /*
        // POST: Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Body,UpvoteCount,ViewCount,IsDeleted,DeleterUserId,DeletionTime,LastModificationTime,LastModifierUserId,CreationTime,CreatorUserId,Id")] CreateQuestionInput question)
        {
            if (ModelState.IsValid)
            {
                _questionAppService.CreateQuestion(question);
                await _questionAppService.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatorUserId"] = new SelectList(_context.Users, "Id", "EmailAddress", question.CreatorUserId);
            ViewData["DeleterUserId"] = new SelectList(_context.Users, "Id", "EmailAddress", question.DeleterUserId);
            ViewData["LastModifierUserId"] = new SelectList(_context.Users, "Id", "EmailAddress", question.LastModifierUserId);
            return View(question);
        }

        // GET: Questions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions.SingleOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }
            ViewData["CreatorUserId"] = new SelectList(_context.Users, "Id", "EmailAddress", question.CreatorUserId);
            ViewData["DeleterUserId"] = new SelectList(_context.Users, "Id", "EmailAddress", question.DeleterUserId);
            ViewData["LastModifierUserId"] = new SelectList(_context.Users, "Id", "EmailAddress", question.LastModifierUserId);
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Body,UpvoteCount,ViewCount,IsDeleted,DeleterUserId,DeletionTime,LastModificationTime,LastModifierUserId,CreationTime,CreatorUserId,Id")] Question question)
        {
            if (id != question.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(question);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatorUserId"] = new SelectList(_context.Users, "Id", "EmailAddress", question.CreatorUserId);
            ViewData["DeleterUserId"] = new SelectList(_context.Users, "Id", "EmailAddress", question.DeleterUserId);
            ViewData["LastModifierUserId"] = new SelectList(_context.Users, "Id", "EmailAddress", question.LastModifierUserId);
            return View(question);
        }

        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.CreatorUser)
                .Include(q => q.DeleterUser)
                .Include(q => q.LastModifierUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.Questions.SingleOrDefaultAsync(m => m.Id == id);
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }*/
    }
}
