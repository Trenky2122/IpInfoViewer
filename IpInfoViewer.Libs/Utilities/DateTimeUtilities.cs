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
