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
using WebApi.Models;

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
                .Include(e => e.Check)
                .ThenInclude(c => c.CurrencyRate)
                .ToList();

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
                    .Include(e => e.Check)
                    .ThenInclude(c => c.CurrencyRate)
                    .ToList();

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
                    .Include(e => e.Check)
                    .ThenInclude(c => c.CurrencyRate)
                    .ToList();

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
                    .Include(e => e.Check)
                    .ThenInclude(c => c.CurrencyRate)
                    .ToList();

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

                try
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        var userCheck = db.Checks.FirstOrDefault(c => c.Id == expense.CheckId);
                        userCheck.Amount -= expense.SpentMoney;

                        db.Checks.Update(userCheck);
                        db.SaveChanges();

                        db.Expenses.Add(expense);
                        db.SaveChanges();

                        transaction.Commit();

                        return Get(expense.Id);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.InnerException?.Message);
                }
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
                using (var transaction = db.Database.BeginTransaction())
                {
                    var expense = db.Expenses.Include(e => e.Check)
                        .ThenInclude(c => c.CurrencyRate)
                        .FirstOrDefault(e => e.Id == id);
                  
                    if (expense == null)
                    {
                        return NotFound($"Расход не найден.");
                    }

                    var userCheck = db.Checks.FirstOrDefault(c => c.Id == expense.CheckId);

                    userCheck.Amount += expense.SpentMoney;

                    db.Checks.Update(userCheck);
                    db.SaveChanges();

                    db.Expenses.Remove(expense);
                    db.SaveChanges();

                    transaction.Commit();

                    return Ok(expense);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        // GET api/<ExpenseController>/5
        /// <summary>
        /// Возвращает: Топ-5 расходов по дню
        /// </summary>
        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<ExpenseDetails>> GetTopFiveExpenseByDay(DateTime date)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                var expenseCollection = (IEnumerable<Expense>)db.Expenses
                   .Where(e => e.UserId == user.Id && (e.Date.Day == date.Day && e.Date.Month == date.Month && e.Date.Year == date.Year))
                   .Include(e => e.CategoryExpense)
                   .Include(e => e.SubCategoryExpense)
                   .Include(e => e.Check)
                   .ThenInclude(c => c.CurrencyRate)
                   .ToList();

                var totalAmount = expenseCollection.Sum(e => e.SpentMoney * e.Check.CurrencyRate.ExchangeRate);

                var groupCollection = expenseCollection.GroupBy(e => e.CategoryExpense.Name);

                var expenseDetailsCollection = groupCollection.Select(c => new ExpenseDetails { ExpenseCategoryName = c.Key, Summ = Decimal.Round(c.Sum(ex => ex.SpentMoney * ex.Check.CurrencyRate.ExchangeRate)), Persent = (double)Decimal.Round(c.Sum(ex => ex.SpentMoney * ex.Check.CurrencyRate.ExchangeRate) / totalAmount * 100) }).ToList();

                var topFiveExpenseDetails = expenseDetailsCollection.OrderByDescending(e => e.Summ).Take(5).OrderBy(e => e.ExpenseCategoryName).ToList();

                return Ok(topFiveExpenseDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        // GET api/<ExpenseController>/5
        /// <summary>
        /// Возвращает: Топ-5 расходов по месяцу
        /// </summary>
        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<ExpenseDetails>> GetTopFiveExpenseByMonth(DateTime date)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                var expenseCollection = (IEnumerable<Expense>)db.Expenses
                    .Where(e => e.UserId == user.Id && (e.Date.Month == date.Month && e.Date.Year == date.Year))
                    .Include(e => e.CategoryExpense)
                    .Include(e => e.SubCategoryExpense)
                    .Include(e => e.Check)
                    .ThenInclude(c => c.CurrencyRate)
                    .ToList();

                var totalAmount = expenseCollection.Sum(e => e.SpentMoney * e.Check.CurrencyRate.ExchangeRate);

                var groupCollection = expenseCollection.GroupBy(e => e.CategoryExpense.Name);

                var expenseDetailsCollection = groupCollection.Select(c => new ExpenseDetails { ExpenseCategoryName = c.Key, Summ = Decimal.Round(c.Sum(ex => ex.SpentMoney * ex.Check.CurrencyRate.ExchangeRate)), Persent = (double)Decimal.Round(c.Sum(ex => ex.SpentMoney * ex.Check.CurrencyRate.ExchangeRate) / totalAmount * 100) }).ToList();

                var topFiveExpenseDetails = expenseDetailsCollection.OrderByDescending(e => e.Summ).Take(5).OrderBy(e => e.ExpenseCategoryName).ToList();

                return Ok(topFiveExpenseDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        // GET api/<ExpenseController>/5
        /// <summary>
        /// Возвращает: Топ-5 расходов по году
        /// </summary>
        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<ExpenseDetails>> GetTopFiveExpenseByYear(DateTime date)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                var expenseCollection = (IEnumerable<Expense>)db.Expenses
                      .Where(e => e.UserId == user.Id && e.Date.Year == date.Year)
                      .Include(e => e.CategoryExpense)
                      .Include(e => e.SubCategoryExpense)
                      .Include(e => e.Check)
                      .ThenInclude(c => c.CurrencyRate)
                      .ToList();

                var totalAmount = expenseCollection.Sum(e => e.SpentMoney * e.Check.CurrencyRate.ExchangeRate);

                var groupCollection = expenseCollection.GroupBy(e => e.CategoryExpense.Name);

                var expenseDetailsCollection = groupCollection.Select(c => new ExpenseDetails { ExpenseCategoryName = c.Key, Summ = Decimal.Round(c.Sum(ex => ex.SpentMoney * ex.Check.CurrencyRate.ExchangeRate)), Persent = (double)Decimal.Round(c.Sum(ex => ex.SpentMoney * ex.Check.CurrencyRate.ExchangeRate) / totalAmount * 100) }).ToList();

                var topFiveExpenseDetails = expenseDetailsCollection.OrderByDescending(e => e.Summ).Take(5).OrderBy(e => e.ExpenseCategoryName).ToList();

                return Ok(topFiveExpenseDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }
    }
}
