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
    public class NewsController : ControllerBase
    {
        ApplicationContext db;

        public NewsController(ApplicationContext context)
        {
            db = context;
        }

        // GET: api/<CategoryController>
        /// <summary>
        /// Возвращает: Cписок всех новостей
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<News>> GetAll()
        {
            var newsCollection = (IEnumerable<News>)db.News.Select(c => c);

            var sortedNewsCollection = newsCollection.OrderByDescending(n => n.Date).ToList();

            if (newsCollection == null)
            {
                return NotFound($"Новости не найдены!");
            }
            return Ok(sortedNewsCollection);
        }

        // GET api/<CategoryController>/5
        /// <summary>
        /// Возвращает: Новость по id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<News> Get(int id)
        {
            var news = db.News.FirstOrDefault(n => n.Id == id);
            if (news == null)
            {
                return NotFound($"Объект c id={id} не найден!");
            }
            return Ok(news);
        }

        // POST api/<CategoryController>
        /// <summary>
        /// Создает: Новую новость
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = "admin,moderator")]
        public ActionResult<News> Post([FromBody] News news)
        {
            try
            {
                db.News.Add(news);
                db.SaveChanges();

                return Get(news.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        //// PUT api/<CategoryController>/5
        /// <summary>
        /// Обновляет: Существующую новость
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = "admin,moderator")]
        public ActionResult<News> Put([FromBody] News news)
        {
            try
            {
                db.News.Update(news);
                db.SaveChanges();

                return Get(news.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        //// DELETE api/<CategoryController>/5
        ////// <summary>
        /// Удаляет: Существующую новость
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = "admin,moderator")]
        public ActionResult<News> Delete(int id)
        {
            try
            {
                var news = db.News.FirstOrDefault(n => n.Id == id);

                db.News.Remove(news);
                db.SaveChanges();

                return Ok(news);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }
    }
}
