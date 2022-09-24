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
    public class CheckController : ControllerBase
    {
        ApplicationContext db;

        public CheckController(ApplicationContext context)
        {
            db = context;
        }

        // GET: api/<CheckController>
        /// <summary>
        /// Возвращает: Cписок всех счетов
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin,moderator")]
        public ActionResult<IEnumerable<Check>> GetAll()
        {
            var checksCollection = (IEnumerable<Check>)db.Checks.Select(c => c);
            return Ok(checksCollection);
        }

        // GET api/<CheckController>/5
        /// <summary>
        /// Возвращает: Счет по id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<Check> Get(int id)
        {
            var сheck = db.Checks.FirstOrDefault(c => c.Id == id);
            if (сheck == null)
            {
                return NotFound($"Объект c id={id} не найден!");
            }
            return Ok(сheck);
        }

        // GET api/<CheckController>/5
        /// <summary>
        /// Возвращает: Все Счета по id пользователя
        /// </summary>
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<Check>> GetAllByUserId(int userId)
        {
            //var checksCollection = (IEnumerable<Check>)db.Checks.Where(c => c.UserId == userId);
            var checksCollection = (IEnumerable<Check>)db.Checks.Where(c => c.UserId == userId).Include(c => c.CurrencyRate);


            if (checksCollection == null)
            {
                return NotFound($"Счета по пользователю не найдены!");
            }
            return Ok(checksCollection);
        }

        // POST api/<CheckController>
        /// <summary>
        /// Создает: Новый счет
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<Check> Post([FromBody] Check check)
        {
            try
            {
                User user = HttpContext.User.CurrentUser();

                check.UserId = user.Id;

                if (check.IsMasterCheck)
                {
                    var masterCheck = db.Checks.FirstOrDefault(c => c.IsMasterCheck == true);
                    if (masterCheck != null)
                    {
                        masterCheck.IsMasterCheck = false;
                        db.Checks.Update(masterCheck);
                    }
                }

                db.Checks.Add(check);
                db.SaveChanges();

                return Get(check.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        //// PUT api/<CheckController>/5
        /// <summary>
        /// Обновляет: Существующий счет
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<Check> Put([FromBody] Check check)
        {
            try
            {
                if (check.IsMasterCheck)
                {
                    var masterCheck = db.Checks.FirstOrDefault(c => c.IsMasterCheck == true);
                    if (masterCheck != null && masterCheck.Id != check.Id)
                    {
                        masterCheck.IsMasterCheck = false;
                        db.Checks.Update(masterCheck);
                    }
                }

                db.Checks.Update(check);
                db.SaveChanges();

                return Get(check.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        //// DELETE api/<CheckController>/5
        ////// <summary>
        /// Удаляет: Существующий счет
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<Check> Delete(int id)
        {
            try
            {
                var check = db.Checks.FirstOrDefault(c => c.Id == id);
                if (check == null)
                {
                    return NotFound($"Счет не найден.");
                }

                db.Checks.Remove(check);
                db.SaveChanges();

                return Ok(check);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        //// TransferToCheck api/<CheckController>/
        ////// <summary>
        /// Удаляет: Существующий счет
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult TransferToCheck(int outgoingCheckId, int incomingCheckId, decimal amount)
        {
            try
            {
                var outgoingCheck = db.Checks.Include(c => c.CurrencyRate).FirstOrDefault(c => c.Id == outgoingCheckId);
                var incomingCheck = db.Checks.Include(c => c.CurrencyRate).FirstOrDefault(c => c.Id == incomingCheckId);
                if (outgoingCheck != null && incomingCheck !=null)
                {
                    decimal outgoingSummBaseCurrency = outgoingCheck.CurrencyRate.ExchangeRate * amount;
                    decimal incomingSummBaseCurrency = outgoingSummBaseCurrency / incomingCheck.CurrencyRate.ExchangeRate;

                    incomingCheck.Amount = incomingCheck.Amount + incomingSummBaseCurrency;
                    outgoingCheck.Amount = outgoingCheck.Amount - amount;

                    db.Checks.Update(incomingCheck);
                    db.Checks.Update(outgoingCheck);

                    db.SaveChanges();

                    return Ok();
                }
                else
                {
                    return NotFound($"Счет не найден.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }
    }
}
