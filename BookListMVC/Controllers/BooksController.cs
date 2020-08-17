using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookListMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BookListMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        [BindProperty]
        public Book Book { get; set; }
        public BooksController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            Book book = new Book();
            if (id == null)
            {
                return View(book);
            }
            book=await _dbContext.Books.FirstOrDefaultAsync(b=>b.Id==id);
            if (book == null)
            {
                return NotFound();
            }
                return View(book);
        }
        [HttpPost]
        public async Task<IActionResult> Upsert()
        {
            if (ModelState.IsValid)
            {

                if (Book.Id == 0)
                {
                    _dbContext.Books.Add(Book);
                }
                else
                {
                    _dbContext.Books.Update(Book);
                }
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(Book);
        }
        #region API call
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _dbContext.Books.ToListAsync() });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return Json(new { success = false, message = "can't delete book" });
            }
            _dbContext.Remove(book);
            await _dbContext.SaveChangesAsync();
            return Json(new { success = true, message = "delete successful" });

        }
        #endregion
    }
}
