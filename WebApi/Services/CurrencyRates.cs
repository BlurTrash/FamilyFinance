using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml;
using System;

namespace WebApi.Services
{
    public class ValCurs
    {
        [XmlElementAttribute("Valute")]
        public ValCursValute[] ValuteList;
    }

    public class ValCursValute
    {

        [XmlElementAttribute("CharCode")]
        public string ValuteStringCode;

        [XmlElementAttribute("Name")]
        public string ValuteName;

        [XmlElementAttribute("Value")]
        public string ExchangeRate;
    }

    public static class CurrencyRates
    {
        /// <summary>
        /// Получить список котировок ЦБ ФР на данный момент
        /// </summary>
        /// <returns>список котировок ЦБ РФ</returns>
        public static List<ValCursValute> GetExchangeRates()
        {
            List<ValCursValute> result = new List<ValCursValute>();
            XmlSerializer xs = new XmlSerializer(typeof(ValCurs));
            XmlReader xr = new XmlTextReader(@"http://www.cbr.ru/scripts/XML_daily.asp");
            foreach (ValCursValute valute in ((ValCurs)xs.Deserialize(xr)).ValuteList)
            {
                result.Add(valute);
            }
            return result;
        }
    }
}
