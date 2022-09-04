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
    public class SubCategoryController : ControllerBase
    {
        ApplicationContext db;

        public SubCategoryController(ApplicationContext context)
        {
            db = context;
        }

        // GET: api/<SubCategoryController>
        /// <summary>
        /// Возвращает: Cписок всех подкатегорий
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin,moderator")]
        public ActionResult<IEnumerable<SubCategory>> GetAll()
        {
            var subCategoriesCollection = (IEnumerable<SubCategory>)db.SubCategories.Select(s => s);
            return Ok(subCategoriesCollection);
        }

        // GET api/<SubCategoryController>/5
        /// <summary>
        /// Возвращает: ПодКатегорию по id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<SubCategory> Get(int id)
        {
            var subCategory = db.SubCategories.FirstOrDefault(s => s.Id == id);
            if (subCategory == null)
            {
                return NotFound($"Объект c id={id} не найден!");
            }
            return Ok(subCategory);
        }

        // GET api/<SubCategoryController>/5
        /// <summary>
        /// Возвращает: ПодКатегории по id родительской категории
        /// </summary>
        [HttpGet("{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<SubCategory>> GetAllByCategoryId(int categoryId)
        {
            var subCategoriesCollection = (IEnumerable<SubCategory>)db.SubCategories.Where(s => s.CategoryId == categoryId);
            if (subCategoriesCollection == null)
            {
                return NotFound($"Подкатегории по родительской категории не найдены!");
            }
            return Ok(subCategoriesCollection);
        }

        // POST api/<SubCategoryController>
        /// <summary>
        /// Создает: Новую подкатегорию
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<SubCategory> Post([FromBody] SubCategory subCategory)
        {
            try
            {
                db.SubCategories.Add(subCategory);
                db.SaveChanges();

                return Get(subCategory.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        //// PUT api/<SubCategoryController>/5
        /// <summary>
        /// Обновляет: Существующую подкатегорию
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<SubCategory> Put([FromBody] SubCategory subCategory)
        {
            try
            {
                db.SubCategories.Update(subCategory);
                db.SaveChanges();

                return Get(subCategory.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        //// DELETE api/<SubCategoryController>/5
        ////// <summary>
        /// Удаляет: Существующую подкатегорию
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<SubCategory> Delete(int id)
        {
            try
            {
                var subCategory = db.SubCategories.FirstOrDefault(s => s.Id == id);
                if (subCategory == null)
                {
                    return NotFound($"Категория не найдена.");
                }

                db.SubCategories.Remove(subCategory);
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
