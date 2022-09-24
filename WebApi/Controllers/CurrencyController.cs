using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebApi.Database.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private IMemoryCache memoryCache;
        private List<CurrencyRate> currencyList;
        private ApplicationContext db;

        public CurrencyController(IMemoryCache memoryCache, ApplicationContext context)
        {
            this.memoryCache = memoryCache;
            db = context;
        }
        // GET: api/<CurrencyController>
        /// <summary>
        /// Возвращает: список со всеми валютами и курсами
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<IEnumerable<CurrencyRate>> GetAll()
        {
            try
            {
                //if (!memoryCache.TryGetValue("key_currencyList", out currencyList))
                //{
                //    throw new Exception("Ошибка получения данных");
                //}
                var collection = (IEnumerable<CurrencyRate>)db.CurrencyRates.Select(c => c);

                return Ok(collection);
                //return Ok(currencyList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }
    }
}
