using System;
using System.Linq;
using System.Globalization;

namespace KycApi.Service.Implementation
{
    public class SwedishSsnValidator
    {
        public static bool IsValidSsn(string ssn)
        {
            if (string.IsNullOrWhiteSpace(ssn))
            {
                return false;
            }

            // 1. Regularize the input
            ssn = ssn.Trim();
            bool hasPlusSign = false;
            if (ssn.Contains("+"))
            {
                hasPlusSign = true;
                ssn = ssn.Replace("+", "");
            }
            ssn = ssn.Replace("-", "");

            // 2. Validate the length & ensure only digits now
            if ((ssn.Length != 10 && ssn.Length != 12) || !ssn.All(char.IsDigit))
            {
                return false;
            }

            // 3. Extract date parts and normalize
            string yearStr, monthStr, dayStr;
            string lastFourDigits;
            
            int currentYear = DateTime.Now.Year;
            int currentYearLastTwoDigits = currentYear % 100;

            if (ssn.Length == 10)
            {
                string yyStr = ssn.Substring(0, 2);
                monthStr = ssn.Substring(2, 2);
                dayStr = ssn.Substring(4, 2);
                lastFourDigits = ssn.Substring(6, 4);

                if (!int.TryParse(yyStr, out int yy)) return false; // Should be caught by IsDigit earlier

                int determinedCentury;
                // If yy (e.g. 70) > current year's last two digits (e.g. 24 for 2024), assume 19xx.
                // Else (e.g. yy is 10, or 24), assume 20xx.
                if (yy > currentYearLastTwoDigits)
                {
                    determinedCentury = 1900;
                }
                else
                {
                    determinedCentury = 2000;
                }
                
                int fullYear = determinedCentury + yy;

                // If the calculated year (e.g. 2070 for SSN "70...") is in the future,
                // it must be the previous century (e.g. 1970).
                // This handles cases like current year 2005, SSN "04..." is 2004, SSN "99..." is 1999.
                // SSN "06..." (for 2006) would be 2006.
                // If current year is 2024, SSN "23" -> 2023. SSN "25" -> 2025 (future). SSN "98" -> 1998.
                // If 20YY is in the future by more than a small margin (e.g. people are not yet born), then it's 19YY.
                // This is a common source of ambiguity.
                // A simple rule: if (century_year + yy) > current_year, then century_year -= 100.
                // Example: current year 2024. yy = 70. currentYearLastTwoDigits = 24. yy > currentYearLastTwoDigits -> century = 1900. fullYear = 1970.
                // Example: current year 2024. yy = 10. currentYearLastTwoDigits = 24. yy <= currentYearLastTwoDigits -> century = 2000. fullYear = 2010.
                // Example: current year 2024. yy = 25. currentYearLastTwoDigits = 24. yy > currentYearLastTwoDigits -> century = 1900. fullYear = 1925.
                // This seems more robust. The key is what "yy > currentYearLastTwoDigits" implies.
                // Let's use: if the YY would result in a year far in the past if 20YY, then 19YY or vice-versa.
                // Standard rule: Numbers 00–(current year last two digits) are 20xx. Numbers (current year last two digits)+1–99 are 19xx.
                // This is what my simplified if/else does.

                if (hasPlusSign)
                {
                    fullYear -= 100;
                }
                yearStr = fullYear.ToString();
            }
            else // 12 digits
            {
                yearStr = ssn.Substring(0, 4);
                monthStr = ssn.Substring(4, 2);
                dayStr = ssn.Substring(6, 2);
                lastFourDigits = ssn.Substring(8, 4);
            }

            // 4. Validate the date
            if (!int.TryParse(yearStr, out int yyyy) ||
                !int.TryParse(monthStr, out int mm) ||
                !int.TryParse(dayStr, out int dd))
            {
                return false; // Should not happen due to earlier digit check
            }

            bool isCoordinationNumber = false;
            if (dd >= 61 && dd <= 91)
            {
                isCoordinationNumber = true;
                dd -= 60;
            }

            if (mm < 1 || mm > 12) return false;
            // Day check needs to be against days in month for yyyy, mm
            if (dd < 1 || dd > DateTime.DaysInMonth(yyyy, mm)) return false;


            try
            {
                DateTime date = new DateTime(yyyy, mm, dd); // This already validates day for month/year
                // For non-coordination numbers, check if date is too far in the future.
                // Coordination numbers can be for individuals not yet born or recently deceased.
                if (!isCoordinationNumber && date.Date > DateTime.Now.Date.AddDays(2)) // Allow a 2-day buffer for registration
                {
                     return false;
                }
                // Also, check if the person is unreasonably old (e.g. > 130 years) for non-plus SSNs
                if (!hasPlusSign && (currentYear - yyyy > 130))
                {
                    // This might be too restrictive or not perfectly aligned with official rules,
                    // but serves as a sanity check. The plus sign is the formal way for >100.
                    // For now, let's rely on the century logic primarily.
                }

            }
            catch (ArgumentOutOfRangeException)
            {
                return false; // Invalid date (e.g., Feb 30, though DaysInMonth should catch it)
            }
            
            // 5. Perform Luhn algorithm
            string yyForLuhn = yearStr.Substring(yearStr.Length - 2);
            string luhnNumberStr = yyForLuhn + monthStr + dayStr + lastFourDigits;
            
            // Ensure luhnNumberStr is exactly 10 digits and all are numeric
            // (already guaranteed by ssn parsing and substring logic if inputs were correct)
            if (luhnNumberStr.Length != 10 || !luhnNumberStr.All(char.IsDigit)) {
                return false; // Should not be reached if logic above is sound
            }

            int sum = 0;
            for (int i = 0; i < luhnNumberStr.Length; i++)
            {
                // char.GetNumericValue is safer than int.Parse(char.ToString())
                int digit = (int)char.GetNumericValue(luhnNumberStr[i]);
                
                // Swedish Luhn: digits are processed from left to right.
                // Odd positions (1st, 3rd, 5th... which are index 0, 2, 4...) are multiplied by 2.
                // Even positions (2nd, 4th, 6th... which are index 1, 3, 5...) are multiplied by 1.
                if (i % 2 == 0) // Index 0, 2, 4... (1st, 3rd, 5th digit)
                {
                    digit *= 2;
                    if (digit > 9)
                    {
                        digit = (digit / 10) + (digit % 10);
                    }
                }
                sum += digit;
            }
            return (sum % 10 == 0);
        }
    }
}
