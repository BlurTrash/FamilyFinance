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
    public class IncomeController : ControllerBase
    {
        ApplicationContext db;

        public IncomeController(ApplicationContext context)
        {
            db = context;
        }

        // GET: api/<IncomeController>
        /// <summary>
        /// Возвращает: Cписок всех доходов
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin,moderator")]
        public ActionResult<IEnumerable<Income>> GetAll()
        {
            var incomeCollection = (IEnumerable<Income>)db.Incomes.Select(e => e);
            return Ok(incomeCollection);
        }

        // GET api/<IncomeController>/5
        /// <summary>
        /// Возвращает: Доход по id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<Income> Get(int id)
        {
            var income = db.Incomes.FirstOrDefault(i => i.Id == id);
            if (income == null)
            {
                return NotFound($"Объект c id={id} не найден!");
            }
            return Ok(income);
        }

        // GET api/<IncomeController>/5
        /// <summary>
        /// Возвращает: Все доходы по id пользователя
        /// </summary>
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<Income>> GetAllByUserId(int userId)
        {
            var incomeCollection = (IEnumerable<Income>)db.Incomes
                .Where(i => i.UserId == userId)
                .Include(i => i.Category)
                .Include(i => i.SubCategory)
                .Include(i => i.Check)
                .ThenInclude(c => c.CurrencyRate)
                .ToList();

            if (incomeCollection == null)
            {
                return NotFound($"Доходы по пользователю не найдены!");
            }
            return Ok(incomeCollection);
        }

        // GET api/<IncomeController>/5
        /// <summary>
        /// Возвращает: Все доходы по месяцу
        /// </summary>
        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<Income>> GetAllByMonth(DateTime date)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                var incomeCollection = (IEnumerable<Income>)db.Incomes
                    .Where(i => i.UserId == user.Id && (i.Date.Month == date.Month && i.Date.Year == date.Year))
                    .Include(i => i.Category)
                    .Include(i => i.SubCategory)
                    .Include(i => i.Check)
                    .ThenInclude(c => c.CurrencyRate)
                    .ToList();

                if (incomeCollection == null)
                {
                    return NotFound($"Доходы не найдены!");
                }
                return Ok(incomeCollection);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        // GET api/<IncomeController>/5
        /// <summary>
        /// Возвращает: Все доходы по году
        /// </summary>
        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<Income>> GetAllByYear(DateTime date)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                var incomeCollection = (IEnumerable<Income>)db.Incomes
                    .Where(i => i.UserId == user.Id && i.Date.Year == date.Year)
                    .Include(i => i.Category)
                    .Include(i => i.SubCategory)
                    .Include(i => i.Check)
                    .ThenInclude(i => i.CurrencyRate)
                    .ToList();

                if (incomeCollection == null)
                {
                    return NotFound($"Доходы не найдены!");
                }
                return Ok(incomeCollection);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        // GET api/<IncomeController>/5
        /// <summary>
        /// Возвращает: Все доходы по дню
        /// </summary>
        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<Income>> GetAllByDay(DateTime date)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                var incomeCollection = (IEnumerable<Income>)db.Incomes
                    .Where(i => i.UserId == user.Id && (i.Date.Day == date.Day && i.Date.Month == date.Month && i.Date.Year == date.Year))
                    .Include(i => i.Category)
                    .Include(i => i.SubCategory)
                    .Include(i => i.Check)
                    .ThenInclude(c => c.CurrencyRate)
                    .ToList();

                if (incomeCollection == null)
                {
                    return NotFound($"Доходы не найдены!");
                }
                return Ok(incomeCollection);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        // POST api/<IncomeController>
        /// <summary>
        /// Создает: Новый доход
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<Income> Post([FromBody] Income income)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                income.UserId = user.Id;

                try
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        var userCheck = db.Checks.FirstOrDefault(c => c.Id == income.CheckId);
                        userCheck.Amount += income.ReplenishmentMoney;

                        db.Checks.Update(userCheck);
                        db.SaveChanges();

                        db.Incomes.Add(income);
                        db.SaveChanges();

                        transaction.Commit();

                        return Get(income.Id);
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

        //// PUT api/<IncomeController>/5
        /// <summary>
        /// Обновляет: Существующий доход
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<Income> Put([FromBody] Income income)
        {
            try
            {
                db.Incomes.Update(income);
                db.SaveChanges();

                return Get(income.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        //// DELETE api/<IncomeController>/5
        ////// <summary>
        /// Удаляет: Существующий доход
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<Income> Delete(int id)
        {
            try
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    var income = db.Incomes.Include(i => i.Check)
                        .ThenInclude(c => c.CurrencyRate)
                        .FirstOrDefault(i => i.Id == id);

                    if (income == null)
                    {
                        return NotFound($"Доход не найден.");
                    }

                    var userCheck = db.Checks.FirstOrDefault(c => c.Id == income.CheckId);

                    userCheck.Amount -= income.ReplenishmentMoney;

                    db.Checks.Update(userCheck);
                    db.SaveChanges();

                    db.Incomes.Remove(income);
                    db.SaveChanges();

                    transaction.Commit();

                    return Ok(income);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        // GET api/<IncomeController>/5
        /// <summary>
        /// Возвращает: Топ-5 доходов по дню
        /// </summary>
        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<IncomeDetails>> GetTopFiveIncomeByDay(DateTime date)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                var incomeCollection = (IEnumerable<Income>)db.Incomes
                   .Where(i => i.UserId == user.Id && (i.Date.Day == date.Day && i.Date.Month == date.Month && i.Date.Year == date.Year))
                   .Include(i => i.Category)
                   .Include(i => i.SubCategory)
                   .Include(i => i.Check)
                   .ThenInclude(c => c.CurrencyRate)
                   .ToList();

                var totalAmount = incomeCollection.Sum(i => i.ReplenishmentMoney * i.Check.CurrencyRate.ExchangeRate);

                var groupCollection = incomeCollection.GroupBy(i => i.Category.Name);

                var incomeDetailsCollection = groupCollection.Select(c => new IncomeDetails { IncomeCategoryName = c.Key, Summ = Decimal.Round(c.Sum(inc => inc.ReplenishmentMoney * inc.Check.CurrencyRate.ExchangeRate)), Persent = (double)Decimal.Round(c.Sum(inc => inc.ReplenishmentMoney * inc.Check.CurrencyRate.ExchangeRate) / totalAmount * 100) }).ToList();

                var topFiveIncomeDetails = incomeDetailsCollection.OrderByDescending(i => i.Summ).Take(5).OrderBy(i => i.IncomeCategoryName).ToList();

                return Ok(topFiveIncomeDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        // GET api/<IncomeController>/5
        /// <summary>
        /// Возвращает: Топ-5 доходов по месяцу
        /// </summary>
        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<IncomeDetails>> GetTopFiveIncomeByMonth(DateTime date)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                var incomeCollection = (IEnumerable<Income>)db.Incomes
                    .Where(i => i.UserId == user.Id && (i.Date.Month == date.Month && i.Date.Year == date.Year))
                    .Include(i => i.Category)
                    .Include(i => i.SubCategory)
                    .Include(i => i.Check)
                    .ThenInclude(c => c.CurrencyRate)
                    .ToList();

                var totalAmount = incomeCollection.Sum(i => i.ReplenishmentMoney * i.Check.CurrencyRate.ExchangeRate);

                var groupCollection = incomeCollection.GroupBy(i => i.Category.Name);

                var incomeDetailsCollection = groupCollection.Select(c => new IncomeDetails { IncomeCategoryName = c.Key, Summ = Decimal.Round(c.Sum(inc => inc.ReplenishmentMoney * inc.Check.CurrencyRate.ExchangeRate)), Persent = (double)Decimal.Round(c.Sum(inc => inc.ReplenishmentMoney * inc.Check.CurrencyRate.ExchangeRate) / totalAmount * 100) }).ToList();

                var topFiveIncomeDetails = incomeDetailsCollection.OrderByDescending(i => i.Summ).Take(5).OrderBy(i => i.IncomeCategoryName).ToList();

                return Ok(topFiveIncomeDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        // GET api/<IncomeController>/5
        /// <summary>
        /// Возвращает: Топ-5 доходов по году
        /// </summary>
        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<IncomeDetails>> GetTopFiveIncomeByYear(DateTime date)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                var incomeCollection = (IEnumerable<Income>)db.Incomes
                      .Where(i => i.UserId == user.Id && i.Date.Year == date.Year)
                      .Include(i => i.Category)
                      .Include(i => i.SubCategory)
                      .Include(i => i.Check)
                      .ThenInclude(c => c.CurrencyRate)
                      .ToList();

                var totalAmount = incomeCollection.Sum(i => i.ReplenishmentMoney * i.Check.CurrencyRate.ExchangeRate);

                var groupCollection = incomeCollection.GroupBy(i => i.Category.Name);

                var incomeDetailsCollection = groupCollection.Select(c => new IncomeDetails { IncomeCategoryName = c.Key, Summ = Decimal.Round(c.Sum(inc => inc.ReplenishmentMoney * inc.Check.CurrencyRate.ExchangeRate)), Persent = (double)Decimal.Round(c.Sum(inc => inc.ReplenishmentMoney * inc.Check.CurrencyRate.ExchangeRate) / totalAmount * 100) }).ToList();

                var topFiveIncomeDetails = incomeDetailsCollection.OrderByDescending(i => i.Summ).Take(5).OrderBy(i => i.IncomeCategoryName).ToList();

                return Ok(topFiveIncomeDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }
    }
}
