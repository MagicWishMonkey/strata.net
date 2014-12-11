using System;
using System.Collections;
namespace Strata.Util {
    /// <summary>
    /// This class provides various date related functionality and utility methods
    /// </summary>
    public static class Dates {
        #region -------- VARIABLES AND CONSTRUCTOR(S) --------
        private static ArrayList months;
        static Dates() {
            months = new ArrayList(12);
            months.Add(new Month("January", 1));
            months.Add(new Month("February", 2));
            months.Add(new Month("March", 3));
            months.Add(new Month("April", 4));
            months.Add(new Month("May", 5));
            months.Add(new Month("June", 6));
            months.Add(new Month("July", 7));
            months.Add(new Month("August", 8));
            months.Add(new Month("September", 9));
            months.Add(new Month("October", 10));
            months.Add(new Month("November", 11));
            months.Add(new Month("December", 12));
        }
        #endregion

        #region -------- GET TIME DELTA --------
        /// <summary>
        /// Return a TimeSpan object representing the time differential
        /// between the specified start and end times.				
        /// </summary>
        /// <param name="begin">The DateTime object representing the start</param>
        /// <param name="end">The DateTime object represending the end</param>
        /// <returns>A TimeSpan object representing the total differential</returns>
        public static TimeSpan Delta(DateTime begin, DateTime end) {
            TimeSpan start = new TimeSpan(begin.Ticks);
            TimeSpan stop = new TimeSpan(end.Ticks);
            TimeSpan diff = stop.Subtract(start);
            return diff;
        }
        /// <summary>
        /// Return a TimeSpan object representing the time differential
        /// between the specified start and end TimeSpan objects.				
        /// </summary>
        /// <param name="begin">The TimeSpan object representing the start</param>
        /// <param name="end">The TimeSpan object represending the end</param>
        /// <returns>A TimeSpan object representing the total differential</returns>
        public static TimeSpan Delta(TimeSpan begin, TimeSpan end) {
            TimeSpan diff = end.Subtract(begin);
            return diff;
        }
        /// <summary>
        /// The time delta (in years) between the given start and end dates
        /// </summary>
        /// <param name="begin">The DateTime object representing the start</param>
        /// <param name="end">The DateTime object represending the end</param>
        /// <returns>A double representing the total number of years between the two dates</returns>
        public static double DeltaYears(DateTime begin, DateTime end) {
            TimeSpan start = new TimeSpan(begin.Ticks);
            TimeSpan stop = new TimeSpan(end.Ticks);
            TimeSpan diff = stop.Subtract(start);
            return diff.TotalDays / 365;
        }
        /// <summary>
        /// The time delta (in months) between the given start and end dates
        /// </summary>
        /// <param name="begin">The DateTime object representing the start</param>
        /// <param name="end">The DateTime object represending the end</param>
        /// <returns>A double representing the total number of months between the two dates</returns>
        public static double DeltaMonths(DateTime begin, DateTime end) {
            TimeSpan start = new TimeSpan(begin.Ticks);
            TimeSpan stop = new TimeSpan(end.Ticks);
            TimeSpan diff = stop.Subtract(start);
            return diff.TotalDays / 30;
        }
        /// <summary>
        /// The time delta (in weeks) between the given start and end dates
        /// </summary>
        /// <param name="begin">The DateTime object representing the start</param>
        /// <param name="end">The DateTime object represending the end</param>
        /// <returns>A double representing the total number of weeks between the two dates</returns>
        public static double DeltaWeeks(DateTime begin, DateTime end) {
            TimeSpan start = new TimeSpan(begin.Ticks);
            TimeSpan stop = new TimeSpan(end.Ticks);
            TimeSpan diff = stop.Subtract(start);
            return diff.TotalDays / 7;
        }

        /// <summary>
        /// The time delta (in days) between the given start and end dates
        /// </summary>
        /// <param name="begin">The DateTime object representing the start</param>
        /// <param name="end">The DateTime object represending the end</param>
        /// <returns>A double representing the total number of days between the two dates</returns>
        public static double DeltaDays(DateTime begin, DateTime end) {
            TimeSpan start = new TimeSpan(begin.Ticks);
            TimeSpan stop = new TimeSpan(end.Ticks);
            TimeSpan diff = stop.Subtract(start);
            return diff.TotalDays;
        }

        /// <summary>
        /// The time delta (in hours) between the given start and end dates
        /// </summary>
        /// <param name="begin">The DateTime object representing the start</param>
        /// <param name="end">The DateTime object represending the end</param>
        /// <returns>A double representing the total number of hours between the two dates</returns>
        public static double DeltaHours(DateTime begin, DateTime end) {
            TimeSpan start = new TimeSpan(begin.Ticks);
            TimeSpan stop = new TimeSpan(end.Ticks);
            TimeSpan diff = stop.Subtract(start);
            return diff.TotalHours;
        }

        /// <summary>
        /// The time delta (in minutes) between the given start and end dates
        /// </summary>
        /// <param name="begin">The DateTime object representing the start</param>
        /// <param name="end">The DateTime object represending the end</param>
        /// <returns>A double representing the total number of minutes between the two dates</returns>
        public static double DeltaMinutes(DateTime begin, DateTime end) {
            TimeSpan start = new TimeSpan(begin.Ticks);
            TimeSpan stop = new TimeSpan(end.Ticks);
            TimeSpan diff = stop.Subtract(start);
            return diff.TotalMinutes;
        }

        /// <summary>
        /// The time delta (in seconds) between the given start and end dates
        /// </summary>
        /// <param name="begin">The DateTime object representing the start</param>
        /// <param name="end">The DateTime object represending the end</param>
        /// <returns>A double representing the total number of seconds between the two dates</returns>
        public static double DeltaSeconds(DateTime begin, DateTime end) {
            TimeSpan start = new TimeSpan(begin.Ticks);
            TimeSpan stop = new TimeSpan(end.Ticks);
            TimeSpan diff = stop.Subtract(start);
            return diff.TotalSeconds;
        }
        /// <summary>
        /// The time delta (in milliseconds) between the given start and end dates
        /// </summary>
        /// <param name="begin">The DateTime object representing the start</param>
        /// <param name="end">The DateTime object represending the end</param>
        /// <returns>A double representing the total number of milliseconds between the two dates</returns>
        public static double DeltaMilliseconds(DateTime begin, DateTime end) {
            TimeSpan start = new TimeSpan(begin.Ticks);
            TimeSpan stop = new TimeSpan(end.Ticks);
            TimeSpan diff = stop.Subtract(start);
            return diff.TotalMilliseconds;
        }
        /// <summary>
        /// The time delta (in ticks) between the given start and end dates
        /// </summary>
        /// <param name="begin">The DateTime object representing the start</param>
        /// <param name="end">The DateTime object represending the end</param>
        /// <returns>A long representing the total number of ticks between the two dates</returns>
        public static long DeltaTicks(DateTime begin, DateTime end) {
            //			long amount = end.Ticks - begin.Ticks;
            //			if(amount < 0)
            //				amount *= -1;
            //			return amount;

            TimeSpan start = new TimeSpan(begin.Ticks);
            TimeSpan stop = new TimeSpan(end.Ticks);
            TimeSpan diff = stop.Subtract(start);
            return diff.Ticks;
        }

        /// <summary>
        /// The time delta (in ticks) between the given start and end dates
        /// </summary>
        /// <param name="begin">The DateTime object representing the start</param>
        /// <param name="end">The DateTime object represending the end</param>
        /// <returns>A long representing the total number of ticks between the two dates</returns>
        public static string GetSpanLength(DateTime begin, DateTime end) {
            TimeSpan start = new TimeSpan(begin.Ticks);
            TimeSpan stop = new TimeSpan(end.Ticks);
            TimeSpan diff = stop.Subtract(start);
            return Convert.ToString(Convert.ToInt32(diff.TotalHours)) + " hours, " + Convert.ToString(Convert.ToInt32(diff.TotalMinutes)) + " minutes, " + Convert.ToString(diff.TotalSeconds) + " seconds";
        }

        /// <summary>
        /// Return a string containing a the timestame formatted to 
        /// include the exact time of day (to the millisecond).		
        /// </summary>
        /// <returns>The formatted timestamp string</returns>
        public static string PreciseTime() {
            return PreciseTime(DateTime.Now);
        }
        /// <summary>
        /// Return a string containing a the timestame formatted to 
        /// include the exact time of day (to the millisecond).		
        /// </summary>
        /// <param name="date">The DateTime object to generate the precise time for</param>
        /// <returns>The formatted timestamp string</returns>
        public static string PreciseTime(DateTime date) {
            int hh = date.Hour;
            int mm = date.Minute;
            int ss = date.Second;
            string tod = "AM";
            if (hh > 12) {
                tod = "PM";
                hh -= 12;
            }
            string hour = Convert.ToString(hh);
            if (hh < 10)
                hour = "0" + hour;

            string min = Convert.ToString(mm);

            string sec = Convert.ToString(ss);

            string msec;
            if (mm < 10)
                msec = "000" + Convert.ToString(mm);
            else if (mm < 100)
                msec = "00" + Convert.ToString(mm);
            else if (mm < 1000)
                msec = "0" + Convert.ToString(mm);
            else
                msec = Convert.ToString(mm);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(date.ToShortDateString())
                .Append(" ")
                .Append(hour)
                .Append(":")
                .Append(min)
                .Append(":")
                .Append(sec)
                .Append(msec)
                .Append(" ")
                .Append(tod);
            return sb.ToString();
        }

        /// <summary>
        /// A formatted string displaying the time elapsed between two dates
        /// </summary>
        /// <param name="begin">The DateTime object representing the start</param>
        /// <param name="end">The DateTime object represending the end</param>
        /// <returns>A formatted string displaying the time elapsed between the two dates</returns>
        public static System.Text.StringBuilder GetPreciseDelta(DateTime begin, DateTime end) {
            TimeSpan span = Delta(begin, end);
            string hh = Convert.ToString(span.Hours);
            string mm = Convert.ToString(span.Minutes);
            string ss = Convert.ToString(span.Seconds);
            string ms = Convert.ToString(span.Milliseconds);
            if (hh.Length == 1)
                hh = "0" + hh;
            if (mm.Length == 1)
                mm = "0" + mm;
            if (ss.Length == 1)
                ss = "0" + ss;

            int len = 4 - ms.Length;
            string buffer = "";
            for (int i = 0; i < len; i++) {
                buffer += "0";
            }
            ms = buffer + ms;


            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(hh);
            sb.Append(":");
            sb.Append(mm);
            sb.Append(":");
            sb.Append(ss);
            sb.Append(" ");
            sb.Append(ms);
            sb.Append("ms");
            return sb;
        }
        #endregion

        #region -------- GET PRETTY TIME DELTA --------
        /// <summary>
        /// Return a formatted string representing the time differential
        /// between the specified start and end times.				
        /// </summary>
        /// <param name="begin">The DateTime object representing the start</param>
        /// <param name="end">The DateTime object represending the end</param>
        /// <returns>The number of days, hours, minutes, and seconds returned as a formatted string</returns>
        public static string GetPrettyDelta(DateTime begin, DateTime end) {
            TimeSpan start = new TimeSpan(begin.Ticks);
            TimeSpan stop = new TimeSpan(end.Ticks);
            TimeSpan diff = stop.Subtract(start);
            return GetPrettyDelta(diff);
            //string rez = "";
            //if(diff.Days > 0){
            //    rez = Convert.ToString(diff.Days)+" days, ";
            //}
            //if(diff.Hours > 0){
            //    rez += Convert.ToString(diff.Hours)+" hours, ";
            //}
            //if(diff.Minutes > 0){
            //    rez += Convert.ToString(diff.Minutes)+" minutes, ";
            //}
            //rez += Convert.ToString(diff.Seconds)+" seconds";
            //return rez;
        }
        public static string GetPrettyDelta(TimeSpan ts) {
            string rez = "";
            if (ts.Days > 0) {
                rez = Convert.ToString(ts.Days) + " days, ";
            }
            if (ts.Hours > 0) {
                rez += Convert.ToString(ts.Hours) + " hours, ";
            }
            if (ts.Minutes > 0) {
                rez += Convert.ToString(ts.Minutes) + " minutes, ";
            }
            rez += Convert.ToString(ts.Seconds) + " seconds";
            return rez;
        }
        #endregion

        #region -------- GET MONTH VALUES --------
        /// <summary>
        /// Return the number for the specified month from either the name or abbreviation.
        /// </summary>
        /// <param name="month">The month name</param>
        /// <returns>The number of a given month</returns>
        public static string GetMonthNumber(string month) {
            if (month == null)
                return "0";
            month = month.ToUpper();
            int count = months.Count;
            for (int i = 0; i < count; i++) {
                if (((Month)months[i]).Name.ToUpper() == month)
                    return Convert.ToString(((Month)months[i]).Number);
                if (((Month)months[i]).Abbreviation.ToUpper() == month)
                    return Convert.ToString(((Month)months[i]).Number);

            }
            return null;
        }
        /// <summary>
        /// Return the number for the specified month by the specified number.
        /// </summary>
        /// <param name="number">The month number</param>
        /// <returns>The name of a given month</returns>
        public static string GetMonthName(int number) {
            switch (number) {
                case (0):
                case (1):
                    return "January";
                case (2):
                    return "February";
                case (3):
                    return "March";
                case (4):
                    return "April";
                case (5):
                    return "May";
                case (6):
                    return "June";
                case (7):
                    return "July";
                case (8):
                    return "August";
                case (9):
                    return "September";
                case (10):
                    return "October";
                case (11):
                    return "November";
                case (12):
                    return "December";
                default:
                    return "January";
            }
        }

        /// <summary>
        /// Returns a DateTime object for the number of ticks
        /// </summary>
        public static DateTime FromTicks(long ticks) {
            return new DateTime(ticks);
        }

        /// <summary>
        /// Returns a specially formatted date stamp
        /// </summary>
        /// <param name="date">The date for which the stamp is generated</param>
        /// <returns></returns>
        public static string GetDateStamp(DateTime date) {
            string dayName = Convert.ToString(date.DayOfWeek).Substring(0, 3);
            string dayNum = Convert.ToString(date.Day);
            if (date.Day < 10)
                dayNum = "0" + dayNum;
            string month = GetMonthName(date.Month);
            if (month.Length > 3)
                month = month.Substring(0, 3);
            string year = Convert.ToString(date.Year);
            string hh, mm, ss;
            if (date.Hour <= 9)
                hh = "0" + Convert.ToString(date.Hour);
            else
                hh = Convert.ToString(date.Hour);

            if (date.Minute <= 9)
                mm = "0" + Convert.ToString(date.Minute);
            else
                mm = Convert.ToString(date.Minute);

            if (date.Second <= 9)
                ss = "0" + Convert.ToString(date.Second);
            else
                ss = Convert.ToString(date.Second);

            string time = hh + ":" + mm + ":" + ss;
            return dayName + ", " + dayNum + " " + month + " " + year + " " + time;
        }
        #endregion

        #region -------- PUBLIC - GetRFCDateStamp --------
        /// <summary>
        /// Returns a specially formatted date stamp
        /// </summary>
        /// <param name="date">The date for which the stamp is generated</param>
        /// <returns></returns>
        public static string GetRFCDateStamp(DateTime date) {
            string dayName = Convert.ToString(date.DayOfWeek).Substring(0, 3);
            string dayNum = Convert.ToString(date.Day);
            if (date.Day < 10)
                dayNum = "0" + dayNum;
            string month = GetMonthName(date.Month);
            if (month.Length > 3)
                month = month.Substring(0, 3);
            string year = Convert.ToString(date.Year);
            string hh, mm, ss;
            if (date.Hour <= 9)
                hh = "0" + Convert.ToString(date.Hour);
            else
                hh = Convert.ToString(date.Hour);

            if (date.Minute <= 9)
                mm = "0" + Convert.ToString(date.Minute);
            else
                mm = Convert.ToString(date.Minute);

            if (date.Second <= 9)
                ss = "0" + Convert.ToString(date.Second);
            else
                ss = Convert.ToString(date.Second);

            string offset = "";

            int off = TimeZone.CurrentTimeZone.GetUtcOffset(date).Hours;
            string sign = "-";
            if (off > 0)
                sign = "+";
            else
                off *= -1;

            if (off < 9)
                offset = sign + "0" + Convert.ToString(off) + "00";
            else
                offset = sign + Convert.ToString(off) + "00";

            string time = hh + ":" + mm + ":" + ss;
            return dayName + ", " + dayNum + " " + month + " " + year + " " + time + " " + offset;
        }
        #endregion

        #region -------- PUBLIC - GetDateKey --------
        public static string GetDateKey(DateTime date) {
            string key = Dates.PrintLegibleDate(date);
            key = key.Substring(0, key.IndexOf("@"));
            key = key.Trim().Replace(" ", "_");
            key = key.Replace("/", "-");
            return key;
        }
        #endregion

        #region -------- PUBLIC - PrintLegibleDate --------
        /// <summary>
        /// Generate an easy-to-read timestamp from the date.
        /// 
        /// Returns the date as a string in the format: MON 01/01/2008 @  3:15:55PM
        /// </summary>
        /// <param name="date">The date for which the stamp is generated</param>
        /// <returns></returns>
        public static string PrintLegibleDate(DateTime date) {
            string dd = date.Day.ToString();
            string mm = (date.Month).ToString();
            string yy = date.Year.ToString();

            int h = date.Hour;
            if (h > 12)
                h -= 12;
            string hour = h.ToString();
            string min = date.Minute.ToString();
            string sec = date.Second.ToString();

            if (dd.Length == 1)
                dd = "0" + dd;
            if (mm.Length == 1)
                mm = "0" + mm;

            bool pad = (hour.Length == 1) ? true : false;
            //if(hour.Length == 1)
            //    hour = " " + hour;
            if (hour.Length == 1)
                hour = "0" + hour;
            if (min.Length == 1)
                min = "0" + min;
            if (sec.Length == 1)
                sec = "0" + sec;

            string day = date.DayOfWeek.ToString().ToUpper().Substring(0, 3);
            string period = (date.Hour < 12) ? "AM" : "PM";

            string result = day + " " + mm + "/" + dd + "/" + yy + " @ " + hour + ":" + min + ":" + sec + " " + period;

            if (pad)
                result += " ";
            return result;
        }
        /// <summary>
        /// Generate an easy-to-read timestamp from the date.
        /// 
        /// Returns the date as a string in the format: MON 01/01/2008 @  3:15:55PM
        /// </summary>
        /// <param name="date">The date for which the stamp is generated</param>
        /// <returns></returns>
        public static string PrintLegibleDateWithMilliseconds(DateTime date) {
            string dd = date.Day.ToString();
            string mm = (date.Month).ToString();
            string yy = date.Year.ToString();

            int h = date.Hour;
            if (h > 12)
                h -= 12;
            string hour = h.ToString();
            string min = date.Minute.ToString();
            string sec = date.Second.ToString();
            string ms = date.Millisecond.ToString();

            if (dd.Length == 1)
                dd = "0" + dd;
            if (mm.Length == 1)
                mm = "0" + mm;

            bool pad = (hour.Length == 1) ? true : false;

            if (hour.Length == 1)
                hour = "0" + hour;
            if (min.Length == 1)
                min = "0" + min;
            if (sec.Length == 1)
                sec = "0" + sec;
            if (ms.Length == 1)
                ms = " " + ms;
            else if (ms.Length > 2)
                ms = ms.Substring(0, 2);

            string day = date.DayOfWeek.ToString().ToUpper().Substring(0, 3);
            string period = (date.Hour < 12) ? "AM" : "PM";

            string result = day + " " + mm + "/" + dd + "/" + yy + " @ " + hour + ":" + min + ":" + sec + ms + period;

            if (pad)
                result += " ";
            return result;
        }
        #endregion

        #region -------- PUBLIC - IsValidDate --------
        public static bool IsValidDate(DateTime date) {
            if (date.ToString().StartsWith("1/1/0001"))
                return false;
            return true;
        }
        public static bool IsValidDate(string date) {
            DateTime rez;
            if (DateTime.TryParse(date, out rez) && !rez.ToString().StartsWith("1/1/0001")) {
                return true;
            }
            return false;
        }
        #endregion


        #region -------- PROPERTIES --------
        /// <summary>
        /// An ArrayList containing month structs
        /// </summary>
        public static ArrayList Months { get { return months; } }
        #endregion
    }

    /// <summary>
    /// A simple struct representing a month value. Quite simply, the best "Month" struct ever
    /// created. 
    /// </summary>
    [Serializable]
    public struct Month : IComparable {
        #region -------- VARIABLES AND CONSTRUCTOR(S) --------
        private string name;
        private int number;
        public Month(string name, int number) {
            this.name = name;
            this.number = number;
        }
        #endregion

        #region -------- PUBLIC PROPERTIES --------
        /// <summary>
        /// Get the abbreviation of the current month
        /// </summary>
        public string Abbreviation {
            get { return this.name.Substring(0, 2).ToUpper(); }
        }
        public string Name { get { return this.name; } }
        public int Number { get { return this.number; } }
        #endregion

        #region -------- CompareTo --------
        public int CompareTo(object month) {
            return ((Month)month).number.CompareTo(this.number);
        }
        #endregion
    }
}
