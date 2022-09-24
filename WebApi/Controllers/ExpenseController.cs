using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class ExpenseController : ControllerBase
    {
        ApplicationContext db;

        public ExpenseController(ApplicationContext context)
        {
            db = context;
        }

        // GET: api/<ExpenseController>
        /// <summary>
        /// Возвращает: Cписок всех расходов
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin,moderator")]
        public ActionResult<IEnumerable<Expense>> GetAll()
        {
            var expenseCollection = (IEnumerable<Expense>)db.Expenses.Select(e => e);
            return Ok(expenseCollection);
        }

        // GET api/<ExpenseController>/5
        /// <summary>
        /// Возвращает: Расход по id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<Expense> Get(int id)
        {
            var expense = db.Expenses.FirstOrDefault(e => e.Id == id);
            if (expense == null)
            {
                return NotFound($"Объект c id={id} не найден!");
            }
            return Ok(expense);
        }

        // GET api/<ExpenseController>/5
        /// <summary>
        /// Возвращает: Все расходы по id пользователя
        /// </summary>
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<Expense>> GetAllByUserId(int userId)
        {
            var expenseCollection = (IEnumerable<Expense>)db.Expenses
                .Where(e => e.UserId == userId)
                .Include(e => e.CategoryExpense)
                .Include(e => e.SubCategoryExpense)
                .Include(e => e.Check);

            if (expenseCollection == null)
            {
                return NotFound($"Расходы по пользователю не найдены!");
            }
            return Ok(expenseCollection);
        }

        // GET api/<ExpenseController>/5
        /// <summary>
        /// Возвращает: Все расходы по месяцу
        /// </summary>
        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<Expense>> GetAllByMonth(DateTime date)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                var expenseCollection = (IEnumerable<Expense>)db.Expenses
                    .Where(e => e.UserId == user.Id && (e.Date.Month == date.Month && e.Date.Year == date.Year))
                    .Include(e => e.CategoryExpense)
                    .Include(e => e.SubCategoryExpense)
                    .Include(e => e.Check);

                if (expenseCollection == null)
                {
                    return NotFound($"Расходы не найдены!");
                }
                return Ok(expenseCollection);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        // GET api/<ExpenseController>/5
        /// <summary>
        /// Возвращает: Все расходы по году
        /// </summary>
        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<Expense>> GetAllByYear(DateTime date)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                var expenseCollection = (IEnumerable<Expense>)db.Expenses
                    .Where(e => e.UserId == user.Id && e.Date.Year == date.Year)
                    .Include(e => e.CategoryExpense)
                    .Include(e => e.SubCategoryExpense)
                    .Include(e => e.Check);

                if (expenseCollection == null)
                {
                    return NotFound($"Расходы не найдены!");
                }
                return Ok(expenseCollection);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        // GET api/<ExpenseController>/5
        /// <summary>
        /// Возвращает: Все расходы по дню
        /// </summary>
        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<Expense>> GetAllByDay(DateTime date)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                var expenseCollection = (IEnumerable<Expense>)db.Expenses
                    .Where(e => e.UserId == user.Id && (e.Date.Day == date.Day && e.Date.Month == date.Month && e.Date.Year == date.Year))
                    .Include(e => e.CategoryExpense)
                    .Include(e => e.SubCategoryExpense)
                    .Include(e => e.Check);

                if (expenseCollection == null)
                {
                    return NotFound($"Расходы не найдены!");
                }
                return Ok(expenseCollection);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        // POST api/<ExpenseController>
        /// <summary>
        /// Создает: Новый расход
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<Expense> Post([FromBody] Expense expense)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                expense.UserId = user.Id;

                db.Expenses.Add(expense);
                db.SaveChanges();

                return Get(expense.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        //// PUT api/<ExpenseController>/5
        /// <summary>
        /// Обновляет: Существующий расход
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<Expense> Put([FromBody] Expense expense)
        {
            try
            {
                db.Expenses.Update(expense);
                db.SaveChanges();

                return Get(expense.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        //// DELETE api/<ExpenseController>/5
        ////// <summary>
        /// Удаляет: Существующий расход
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<Expense> Delete(int id)
        {
            try
            {
                var expense = db.Expenses.FirstOrDefault(e => e.Id == id);
                if (expense == null)
                {
                    return NotFound($"Счет не найден.");
                }

                db.Expenses.Remove(expense);
                db.SaveChanges();

                return Ok(expense);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }
    }
}
