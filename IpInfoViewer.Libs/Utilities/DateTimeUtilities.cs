using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpInfoViewer.Libs.Utilities
{
    public static class DateTimeUtilities
    {
        public static IEnumerable<Week> GetWeeksFromTo(DateTime from, DateTime to)
        {
            var current = new Week(from);
            var final = new Week(to);
            while (current < final)
            {
                yield return current;
                current = current.Next();
            }
            yield return final;
        }
    }

    public class Week
    {
        public Week(DateTime day)
        {
            Monday = day.AddDays(-((7 + ((int)day.DayOfWeek - 1)) % 7)).Date;
        }

        public Week(string week)
        {
            var splitWeek = week.Split("-W");
            if (splitWeek.Length != 2)
                throw new ArgumentException($"Bad format: {week}");
            bool isNumericYear = int.TryParse(splitWeek[0], out int year);
            bool isNumericWeek = int.TryParse(splitWeek[1], out int weekNumber);
            if (!isNumericWeek || !isNumericYear || !(weekNumber is > 0 and <= 53))
                throw new ArgumentException($"Bad format: {week}");
            DateTime fourthJanuaryThatYear = new(year, 1, 4);
            DateTime day = fourthJanuaryThatYear.AddDays(7 * (weekNumber-1));
            /*
             * Definition of ISO_8601 first week of year according to Wikipedia:
             *
             * There are several mutually equivalent and compatible descriptions of week 01:
             * - the week with the first business day in the starting year (considering that Saturdays, Sundays and 1st January are non-working days),
             * - the week with the starting year's first Thursday in it (the formal ISO definition),
             * - the week with 4 January in it,
             * - the first week with the majority (four or more) of its days in the starting year, and
             * - the week starting with the Monday in the period 29 December - 4 January.
             */
            Monday = day.AddDays(-((7 + ((int)day.DayOfWeek - 1)) % 7)).Date;
        }

        public DateTime Monday { get; }

        public DateTime Tuesday => Monday.AddDays(1);
        public DateTime Wednesday => Monday.AddDays(2);
        public DateTime Thursday => Monday.AddDays(3);
        public DateTime Friday => Monday.AddDays(4);
        public DateTime Saturday => Monday.AddDays(5);
        public DateTime Sunday => Monday.AddDays(6);

        public Week Next()
        {
            return new Week(Monday.AddDays(7));
        }

        public Week Previous()
        {
            return new Week(Monday.AddDays(-7));
        }

        public override bool Equals(object obj)
        {
            if (obj is not Week objAsWeek)
                return false;
            return objAsWeek.Monday == this.Monday;
        }

        public static bool operator ==(Week left, Week right)
        {
            if (left == null && right == null)
                return true;
            return !(left == null) && left.Equals(right);
        }
        public static bool operator !=(Week left, Week right)
        {
            return !(left == right);
        }

        public static bool operator <(Week left, Week right)
        {
            if (left is null || right is null)
                throw new ArgumentNullException();
            return left.Monday < right.Monday;
        }

        public static bool operator >(Week left, Week right)
        {
            if (left is null || right is null)
                throw new ArgumentNullException();
            return left.Monday > right.Monday;
        }

        public override int GetHashCode()
        {
            return Monday.GetHashCode();
        }
    }
}
