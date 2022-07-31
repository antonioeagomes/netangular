using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Extensions;

public static class DateTimeExtensions
{
    public static int CalculateAge(this DateTime date)
    {
        var today = DateTime.Now;
        int age = today.Year - date.Year;

        if(date.Date > today.AddYears(age)) --age;

        return age;

    }
}
