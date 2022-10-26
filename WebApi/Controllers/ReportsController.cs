using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Core.Authorization;
using WebApi.Database.Models;
using WebApi.Models;


namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        ApplicationContext db;

        public ReportsController(ApplicationContext context)
        {
            db = context;
        }

        // GET api/<CheckController>/5
        /// <summary>
        /// Возвращает: все остатки на счетах по пользователю
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<ReportDetails>> GetAllCheckBalances()
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                var checks = db.Checks.Where(c => c.UserId == user.Id).Include(c => c.CurrencyRate).ToList();

                var totalAmount = checks.Sum(c => c.Amount * c.CurrencyRate.ExchangeRate);

                var sortedChecks = checks.OrderByDescending(c => c.Amount * c.CurrencyRate.ExchangeRate).ToList();

                var reportDetailsCollection = sortedChecks.Select(c => new ReportDetails { Name = c.Name, ExchageRate = c.CurrencyRate.ExchangeRate, CurrencyStringCode = c.CurrencyRate.CurrencyStringCode, Summ = Decimal.Round(c.Amount), Persent = (c.Amount == 0) ? 0 : (double)Decimal.Round(c.Amount * c.CurrencyRate.ExchangeRate / totalAmount * 100) }).ToList();

                return Ok(reportDetailsCollection);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        // GET api/<ExpenseController>/5
        /// <summary>
        /// Возвращает: все расходы по месяцу
        /// </summary>
        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<ReportDetails>> GetAllExpenseByMonth(DateTime date)
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

                var reportDetailsCollection = groupCollection.Select(c => new ReportDetails { Name = c.Key, Summ = Decimal.Round(c.Sum(ex => ex.SpentMoney * ex.Check.CurrencyRate.ExchangeRate)), Persent = (double)Decimal.Round(c.Sum(ex => ex.SpentMoney * ex.Check.CurrencyRate.ExchangeRate) / totalAmount * 100) }).ToList();

                var sortedReportDetails = reportDetailsCollection.OrderByDescending(e => e.Summ).ToList();

                return Ok(sortedReportDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        // GET api/<ExpenseController>/5
        /// <summary>
        /// Возвращает: все расходы за интервал времени
        /// </summary>
        [HttpGet("{startDate}/{endDate}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<ReportDetails>> GetAllExpenseByInterval(DateTime startDate, DateTime endDate)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                var expenseCollection = (IEnumerable<Expense>)db.Expenses
                    .Where(e => e.UserId == user.Id && (e.Date >= startDate && e.Date <= endDate))
                    .Include(e => e.CategoryExpense)
                    .Include(e => e.SubCategoryExpense)
                    .Include(e => e.Check)
                    .ThenInclude(c => c.CurrencyRate)
                    .ToList();

                var totalAmount = expenseCollection.Sum(e => e.SpentMoney * e.Check.CurrencyRate.ExchangeRate);

                var groupCollection = expenseCollection.GroupBy(e => e.CategoryExpense.Name);

                var reportDetailsCollection = groupCollection.Select(c => new ReportDetails { Name = c.Key, Summ = Decimal.Round(c.Sum(ex => ex.SpentMoney * ex.Check.CurrencyRate.ExchangeRate)), Persent = (double)Decimal.Round(c.Sum(ex => ex.SpentMoney * ex.Check.CurrencyRate.ExchangeRate) / totalAmount * 100) }).ToList();

                var sortedReportDetails = reportDetailsCollection.OrderByDescending(e => e.Summ).ToList();

                return Ok(sortedReportDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        // GET api/<IncomeController>/5
        /// <summary>
        /// Возвращает: все доходы по месяцу
        /// </summary>
        [HttpGet("{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<ReportDetails>> GetAllIncomeByMonth(DateTime date)
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

                var reportDetailsCollection = groupCollection.Select(c => new ReportDetails { Name = c.Key, Summ = Decimal.Round(c.Sum(inc => inc.ReplenishmentMoney * inc.Check.CurrencyRate.ExchangeRate)), Persent = (double)Decimal.Round(c.Sum(inc => inc.ReplenishmentMoney * inc.Check.CurrencyRate.ExchangeRate) / totalAmount * 100) }).ToList();

                var sortedReportDetails = reportDetailsCollection.OrderByDescending(i => i.Summ).ToList();

                return Ok(sortedReportDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        // GET api/<IncomeController>/5
        /// <summary>
        /// Возвращает: все доходы за интервал времени
        /// </summary>
        [HttpGet("{startDate}/{endDate}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<ReportDetails>> GetAllIncomeByInterval(DateTime startDate, DateTime endDate)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                var incomeCollection = (IEnumerable<Income>)db.Incomes
                    .Where(i => i.UserId == user.Id && (i.Date >= startDate && i.Date <= endDate))
                    .Include(i => i.Category)
                    .Include(i => i.SubCategory)
                    .Include(i => i.Check)
                    .ThenInclude(c => c.CurrencyRate)
                    .ToList();

                var totalAmount = incomeCollection.Sum(i => i.ReplenishmentMoney * i.Check.CurrencyRate.ExchangeRate);

                var groupCollection = incomeCollection.GroupBy(i => i.Category.Name);

                var reportDetailsCollection = groupCollection.Select(c => new ReportDetails { Name = c.Key, Summ = Decimal.Round(c.Sum(inc => inc.ReplenishmentMoney * inc.Check.CurrencyRate.ExchangeRate)), Persent = (double)Decimal.Round(c.Sum(inc => inc.ReplenishmentMoney * inc.Check.CurrencyRate.ExchangeRate) / totalAmount * 100) }).ToList();

                var sortedReportDetails = reportDetailsCollection.OrderByDescending(i => i.Summ).ToList();

                return Ok(sortedReportDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }
    }
}
