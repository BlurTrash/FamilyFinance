using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.SpecialTypes;

namespace WebApi.Core
{
    /// <summary>
    /// Утилитраный класс для различных функций и методов расширений
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Возвращает SimpleType, чтобы не возвращать из контроллеров простые типы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controllerBase"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ActionResult<SimpleType<T>> OkSimpleType<T>(this ControllerBase controllerBase, T value)
        {
            return controllerBase.Ok(new SimpleType<T>() { Value = value });
        }
    }
}
