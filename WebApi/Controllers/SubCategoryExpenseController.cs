using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Database.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SubCategoryExpenseController : ControllerBase
    {
        ApplicationContext db;

        public SubCategoryExpenseController(ApplicationContext context)
        {
            db = context;
        }

        // GET: api/<SubCategoryExpenseController>
        /// <summary>
        /// Возвращает: Cписок всех подкатегорий расхода
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin,moderator")]
        public ActionResult<IEnumerable<SubCategoryExpense>> GetAll()
        {
            var subCategoriesCollection = (IEnumerable<SubCategoryExpense>)db.SubCategoriesExpense.Select(s => s);
            return Ok(subCategoriesCollection);
        }

        // GET api/<SubCategoryExpenseController>/5
        /// <summary>
        /// Возвращает: ПодКатегорию расхода по id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<SubCategoryExpense> Get(int id)
        {
            var subCategory = db.SubCategoriesExpense.FirstOrDefault(s => s.Id == id);
            if (subCategory == null)
            {
                return NotFound($"Объект c id={id} не найден!");
            }
            return Ok(subCategory);
        }

        // GET api/<SubCategoryExpenseController>/5
        /// <summary>
        /// Возвращает: ПодКатегории расхода по id родительской категории
        /// </summary>
        [HttpGet("{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<SubCategoryExpense>> GetAllByCategoryExpenseId(int categoryId)
        {
            var subCategoriesCollection = (IEnumerable<SubCategoryExpense>)db.SubCategoriesExpense.Where(s => s.CategoryExpenseId == categoryId);
            if (subCategoriesCollection == null)
            {
                return NotFound($"Подкатегории по родительской категории не найдены!");
            }
            return Ok(subCategoriesCollection);
        }

        // POST api/<SubCategoryExpenseController>
        /// <summary>
        /// Создает: Новую подкатегорию расхода
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<SubCategoryExpense> Post([FromBody] SubCategoryExpense subCategory)
        {
            try
            {
                db.SubCategoriesExpense.Add(subCategory);
                db.SaveChanges();

                return Get(subCategory.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        //// PUT api/<SubCategoryExpenseController>/5
        /// <summary>
        /// Обновляет: Существующую подкатегорию расхода
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<SubCategoryExpense> Put([FromBody] SubCategoryExpense subCategory)
        {
            try
            {
                db.SubCategoriesExpense.Update(subCategory);
                db.SaveChanges();

                return Get(subCategory.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        //// DELETE api/<SubCategoryExpenseController>/5
        ////// <summary>
        /// Удаляет: Существующую подкатегорию расхода
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<SubCategoryExpense> Delete(int id)
        {
            try
            {
                var subCategory = db.SubCategoriesExpense.FirstOrDefault(s => s.Id == id);
                if (subCategory == null)
                {
                    return NotFound($"Категория не найдена.");
                }

                db.SubCategoriesExpense.Remove(subCategory);
                db.SaveChanges();

                return Ok(subCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }
    }
}
