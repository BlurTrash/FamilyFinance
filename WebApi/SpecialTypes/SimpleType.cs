using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.SpecialTypes
{
    /// <summary>
    /// Модель простого типа, для того чтобы возвращать значение
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //[NotMapped]
    public class SimpleType<T>
    {
        public T Value { get; set; }
    }
}
