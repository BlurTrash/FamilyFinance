using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Core.Authorization;
using WebApi.Database.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryExpenseController : ControllerBase
    {
        ApplicationContext db;

        public CategoryExpenseController(ApplicationContext context)
        {
            db = context;
        }

        // GET: api/<CategoryController>
        /// <summary>
        /// Возвращает: Cписок всех категорий
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin,moderator")]
        public ActionResult<IEnumerable<CategoryExpense>> GetAll()
        {
            var categoriesCollection = (IEnumerable<CategoryExpense>)db.CategoriesExpense.Select(c => c);
            return Ok(categoriesCollection);
        }

        // GET api/<CategoryController>/5
        /// <summary>
        /// Возвращает: Категорию по id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<CategoryExpense> Get(int id)
        {
            var category = db.CategoriesExpense.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound($"Объект c id={id} не найден!");
            }
            return Ok(category);
        }

        // GET api/<CategoryController>/5
        /// <summary>
        /// Возвращает: Категорию по id пользователя
        /// </summary>
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<CategoryExpense>> GetAllByUserId(int userId)
        {
            var categoriesCollection = (IEnumerable<CategoryExpense>)db.CategoriesExpense.Where(c => c.UserId == userId);
            if (categoriesCollection == null)
            {
                return NotFound($"Категории по пользователю не найдены!");
            }
            return Ok(categoriesCollection);
        }

        // POST api/<CategoryController>
        /// <summary>
        /// Создает: Новую категорию
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<CategoryExpense> Post([FromBody] CategoryExpense category)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();
                //category.User = user;
                category.UserId = user.Id;
                db.CategoriesExpense.Add(category);
                db.SaveChanges();

                return Get(category.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        //// PUT api/<CategoryController>/5
        /// <summary>
        /// Обновляет: Существующую категорию
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<CategoryExpense> Put([FromBody] CategoryExpense category)
        {
            try
            {
                db.CategoriesExpense.Update(category);
                db.SaveChanges();

                return Get(category.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        //// DELETE api/<CategoryController>/5
        ////// <summary>
        /// Удаляет: Существующую категорию
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<CategoryExpense> Delete(int id)
        {
            try
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    var category = db.CategoriesExpense.FirstOrDefault(c => c.Id == id);
                    if (category == null)
                    {
                        return NotFound($"Категория не найдена.");
                    }

                    var expenses = db.Expenses.Where(e => e.CategoryExpenseId == category.Id);
                    foreach (var item in expenses)
                    {
                        db.Expenses.Remove(item);
                    }
                    db.SaveChanges();

                    var subCategories = db.SubCategoriesExpense.Where(s => s.CategoryExpenseId == category.Id);
                    foreach (var item in subCategories)
                    {
                        db.SubCategoriesExpense.Remove(item);
                    }
                    db.SaveChanges();

                    db.CategoriesExpense.Remove(category);
                    db.SaveChanges();

                    transaction.Commit();

                    return Ok(category);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }
    }
}
