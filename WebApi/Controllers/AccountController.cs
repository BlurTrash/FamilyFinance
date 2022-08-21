using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Core;
using WebApi.Core.Authorization;
using WebApi.Database.Models;
using WebApi.SpecialTypes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    //[Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        ApplicationContext db;

        public AccountController(ApplicationContext context)
        {
            db = context;
        }

        // GET: api/<UsersController>
        /// <summary>
        /// Возвращает: Cписок всех Пользователей
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAll()
        {
            var usersCollection = (IEnumerable<User>)db.Users.Select(u => u);
            return Ok(usersCollection);
        }

        // GET api/<UsersController>/5
        /// <summary>
        /// Возвращает: Пользователя по id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<User> Get(int id)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound($"Объект c id={id} не найден!");
            }
            return Ok(user);
        }

        // POST api/<UsersController>
        /// <summary>
        /// Создает: Нового пользователя
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public ActionResult<User> Post([FromBody] User user, string password)
        {
            try
            {
                user.Password = AuthUtils.GetHash(password);

                var userRole = db.Roles.FirstOrDefault(r => r.Name == "user");

                if (userRole != null)
                    user.Role = userRole;

                db.Users.Add(user);
                db.SaveChanges();

                return Get(user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        //// PUT api/<UsersController>/5
        /// <summary>
        /// Обновляет: Существующего пользователя
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public ActionResult<User> Put([FromBody] User user)
        {
            try
            {
                db.Users.Update(user);
                // Эта строчка нужна чтобы не затереть пароль пользователя
                db.Entry(user).Property(p => p.Password).IsModified = false;
                db.SaveChanges();

                return Get(user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        //// DELETE api/<UsersController>/5
        ////// <summary>
        /// Удаляет: Существующего пользователя
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<User> Delete(int id)
        {
            try
            {
                var user = db.Users.FirstOrDefault(u=> u.Id == id);
                if (user == null)
                {
                    return NotFound($"Пользователь не найден.");
                }

                db.Users.Remove(user);
                db.SaveChanges();

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

        /// <summary>
        /// Войти в систему
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        [HttpGet]
        [AllowAnonymous] //публичный доступ к методу контроллера чтобы пользователь мог авторизироваться
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<SimpleType<string>>> GetToken(string login, string password)
        {
            var token = await AuthUtils.GetClaimsIdentity(db, HttpContext, login, password);
            if (token != null)
            {
                return this.OkSimpleType(token);
            }
            else
            {
                return NotFound("Пользователь с данным именем и паролем не найден.");
            }
        }

        /// <summary>
        /// Возвращает: Текущего авторизованного пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public ActionResult<User> GetCurrentUser()
        {
            User user = HttpContext.User.CurrentUser();

            if (user == null)
            {
                return NotFound($"Пользователь не найден.");
            }
            return Get(user.Id);
        }

        //Тест методов api для авторизации и ролей
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [Authorize]
        public ActionResult<SimpleType<string>> GetText()
        {
           
            return this.OkSimpleType("Для авторизированного пользователя!");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult<SimpleType<string>> GetAdminRole()
        {
            return this.OkSimpleType("Ваша роль: администратор!");
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [Authorize(Roles = "user")]
        public ActionResult<SimpleType<string>> GetUserRole()
        {
            return this.OkSimpleType("Ваша роль: всего лишь пользователь!");
        }
    }
}
