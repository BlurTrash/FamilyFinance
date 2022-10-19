using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Core.Authorization;
using WebApi.Database.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        ApplicationContext db;

        public CategoryController(ApplicationContext context)
        {
            db = context;
        }

        // GET: api/<CategoryController>
        /// <summary>
        /// Возвращает: Cписок всех категорий
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin,moderator")]
        public ActionResult<IEnumerable<Category>> GetAll()
        {
            var categoriesCollection = (IEnumerable<Category>)db.Categories.Select(c => c);
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
        public ActionResult<Category> Get(int id)
        {
            var category = db.Categories.FirstOrDefault(c => c.Id == id);
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
        public ActionResult<IEnumerable<Category>> GetAllByUserId(int userId)
        {
            var categoriesCollection = (IEnumerable<Category>)db.Categories.Where(c => c.UserId == userId);
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
        public ActionResult<Category> Post([FromBody] Category category)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();
                //category.User = user;
                category.UserId = user.Id;
                db.Categories.Add(category);
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
        public ActionResult<Category> Put([FromBody] Category category)
        {
            try
            {
                db.Categories.Update(category);
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
        public ActionResult<Category> Delete(int id)
        {
            try
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    var category = db.Categories.FirstOrDefault(c => c.Id == id);
                    if (category == null)
                    {
                        return NotFound($"Категория не найдена.");
                    }

                    var incomes = db.Incomes.Where(i => i.CategoryId == category.Id);
                    foreach (var item in incomes)
                    {
                        db.Incomes.Remove(item);
                    }
                    db.SaveChanges();

                    var subCategories = db.SubCategories.Where(s => s.CategoryId == category.Id);
                    foreach (var item in subCategories)
                    {
                        db.SubCategories.Remove(item);
                    }
                    db.SaveChanges();

                    db.Categories.Remove(category);
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
