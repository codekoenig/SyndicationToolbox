using System;
using System.Collections.Generic;
using System.Globalization;

namespace CodeKoenig.SyndicationToolbox
{
    internal static class RFCDateParser
    {
        private static Dictionary<string, int> timezoneHoursDictionary = null;

        static RFCDateParser()
        {
            if (timezoneHoursDictionary == null)
            {
                timezoneHoursDictionary = new Dictionary<string, int>();

                timezoneHoursDictionary.Add("A", 1);
                timezoneHoursDictionary.Add("B", 2);
                timezoneHoursDictionary.Add("C", 3);
                timezoneHoursDictionary.Add("D", 4);
                timezoneHoursDictionary.Add("E", 5);
                timezoneHoursDictionary.Add("F", 6);
                timezoneHoursDictionary.Add("G", 7);
                timezoneHoursDictionary.Add("H", 8);
                timezoneHoursDictionary.Add("I", 9);
                timezoneHoursDictionary.Add("K", 10);
                timezoneHoursDictionary.Add("L", 11);
                timezoneHoursDictionary.Add("M", 12);
                timezoneHoursDictionary.Add("N", -1);
                timezoneHoursDictionary.Add("O", -2);
                timezoneHoursDictionary.Add("P", -3);
                timezoneHoursDictionary.Add("Q", -4);
                timezoneHoursDictionary.Add("R", -5);
                timezoneHoursDictionary.Add("S", -6);
                timezoneHoursDictionary.Add("T", -7);
                timezoneHoursDictionary.Add("U", -8);
                timezoneHoursDictionary.Add("V", -9);
                timezoneHoursDictionary.Add("W", -10);
                timezoneHoursDictionary.Add("X", -11);
                timezoneHoursDictionary.Add("Y", -12);
                timezoneHoursDictionary.Add("EST", 5);
                timezoneHoursDictionary.Add("EDT", 4);
                timezoneHoursDictionary.Add("CST", 6);
                timezoneHoursDictionary.Add("CDT", 5);
                timezoneHoursDictionary.Add("MST", 7);
                timezoneHoursDictionary.Add("MDT", 6);
                timezoneHoursDictionary.Add("PST", 8);
                timezoneHoursDictionary.Add("PDT", 7);
            }
        }
        
        private static int GetHoursByCode(string timezoneCode)
        {
            return timezoneHoursDictionary[timezoneCode.ToUpper()];
        }

        /// <summary>
        /// Returns a <see cref="DateTime">date</see> variable by parsing the given RFC822-compliant date string
        /// </summary>
        /// <param name="dateString">The RFC822-compliant date string</param>
        /// <returns>The parsed date</returns>
        public static DateTime ParseRFC822Date(string dateString)
        {
            return ParseRFC822Date(dateString, DateTime.MinValue);
        }

        /// <summary>
        /// Returns a <see cref="DateTime">date</see> variable by parsing the given RFC822-compliant date string
        /// </summary>
        /// <param name="dateString">The RFC822-compliant date string</param>
        /// <param name="defaultDate">The default date that should be returned when parsing fails</param>
        /// <returns>The parsed date</returns>
        public static DateTime ParseRFC822Date(string dateString, DateTime defaultDate)
        {
            DateTime result;

            try
            {
                result = ParseRFC822DateNow(dateString, defaultDate);
            }
            catch (Exception)
            {
                result = defaultDate;
            }

            return result;
        }

        private static DateTime ParseRFC822DateNow(string dateString, DateTime defaultDate)
        {
            bool bolSuccess = false;
            System.DateTime dteParsedDate = default(System.DateTime);
            int intLastSpaceIndex = dateString.LastIndexOf(" ");

            // First, try to parse the date with .NET's engine
            try
            {
                // Parse date
                dteParsedDate = System.DateTime.Parse(dateString, DateTimeFormatInfo.InvariantInfo);

                // Set to UTC if GMT or Z timezone info is given
                if (dateString.Substring(intLastSpaceIndex + 1) == "GMT" | dateString.Substring(intLastSpaceIndex + 1) == "Z")
                {
                    dteParsedDate.ToUniversalTime();
                }

                bolSuccess = true;
            }
            catch (Exception)
            {
                // Parsing failed, mark to try it "by hand"
                bolSuccess = false;
            }

            if (!bolSuccess)
            {

                // Try a manual parse now without timezone information
                string strTimezone = dateString.Substring(intLastSpaceIndex + 1);
                string strReducedDate = dateString.Substring(0, intLastSpaceIndex);

                dteParsedDate = System.DateTime.Parse(strReducedDate, DateTimeFormatInfo.InvariantInfo);

                // Now, calculate UTC based on the given timezone in the date string
                if (strTimezone.StartsWith("+"))
                {
                    // The Timezone is given as a +hhmm string
                    dteParsedDate.AddHours(-int.Parse(strReducedDate.Substring(1, 2)));
                    dteParsedDate.AddMinutes(-int.Parse(strReducedDate.Substring(3)));
                }
                else if (strTimezone.StartsWith("-"))
                {
                    // The Timezone is given as a -hhmm string
                    // The Timezone is given as a +hhmm string
                    dteParsedDate.AddHours(int.Parse(strReducedDate.Substring(1, 2)));
                    dteParsedDate.AddMinutes(int.Parse(strReducedDate.Substring(3)));
                }
                else
                {
                    // The Timezone is given as a named string
                    dteParsedDate = dteParsedDate.AddHours(GetHoursByCode(strTimezone));
                }
            }

            return dteParsedDate;
        }
    }
}
