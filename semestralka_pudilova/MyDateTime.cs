using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace semestralka_pudilova
{
    // Zdroj: https://stackoverflow.com/questions/798121/date-vs-datetime#answer-798471
    public class MyDateTime : IEquatable<MyDateTime>, IEquatable<DateTime>
    {
        public MyDateTime(DateTime date)
        {
            value = date.Date;
        }
        public bool Equals(MyDateTime other)
        {
            return other != null && value.Equals(other.value);
        }
        public bool Equals(DateTime other)
        {
            return value.Equals(other);
        }
        public override string ToString()
        {
            return value.ToString("d", CultureInfo.CreateSpecificCulture("de-DE"));
        }
        public static implicit operator DateTime(MyDateTime date)
        {
            return date.value;
        }
        public static explicit operator MyDateTime(DateTime dateTime)
        {
            return new MyDateTime(dateTime);
        }
        private DateTime value;
    }
}
