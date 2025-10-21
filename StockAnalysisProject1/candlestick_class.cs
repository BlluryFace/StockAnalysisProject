using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WindowsFormsApp1
{
    /// <summary>
    /// Represents a single candlestick with OHLCV (Open, High, Low, Close, Volume) data.
    /// Stores stock price data for a specific time period.
    /// </summary>
    public class aCandlestick
    {
        // Date and time of the candlestick
        public DateTime date { get; set; }

        // Opening price for the period
        public decimal open { get; set; }

        // Highest price during the period
        public decimal high { get; set; }

        // Lowest price during the period
        public decimal low { get; set; }

        // Closing price for the period
        public decimal close { get; set; }

        // Trading volume for the period
        public ulong volume { get; set; }

        /// <summary>
        /// Default constructor - creates an empty candlestick with default values.
        /// </summary>
        public aCandlestick()
        {
            date = DateTime.MinValue; // Initialize date with minimal value
            open = 0;                  // Initialize open price to 0
            high = 0;                  // Initialize high price to 0
            low = 0;                   // Initialize low price to 0
            close = 0;                 // Initialize close price to 0
            volume = 0;                // Initialize volume to 0
        }

        /// <summary>
        /// Parameterized constructor for creating a candlestick with specific values.
        /// </summary>
        /// <param name="date">Date and time of the candlestick</param>
        /// <param name="open">Opening price</param>
        /// <param name="high">Highest price</param>
        /// <param name="low">Lowest price</param>
        /// <param name="close">Closing price</param>
        /// <param name="volume">Trading volume</param>
        public aCandlestick(DateTime date, decimal open, decimal high, decimal low, decimal close, ulong volume)
        {
            this.date = date;   // Set date
            this.open = open;   // Set open price
            this.high = high;   // Set high price
            this.low = low;     // Set low price
            this.close = close; // Set close price
            this.volume = volume; // Set volume
        }

        /// <summary>
        /// Constructor that creates a candlestick from a CSV line.
        /// Expected CSV layout:
        /// "AAPL","D","2023-03-24","158.86","160.34","157.85","160.25","59256343",...
        /// Handles quoted fields and doubled quotes.
        /// </summary>
        /// <param name="csvLine">A line from the CSV file containing candlestick data</param>
        public aCandlestick(string csvLine)
        {
            // Validate that the CSV line is not null or empty
            if (string.IsNullOrWhiteSpace(csvLine))
                throw new ArgumentException("CSV line is null or empty.", nameof(csvLine));

            string[] values = SplitCsvLine(csvLine); // Split CSV line into fields

            // Ensure there are at least 8 fields (ticker, period, date, open, high, low, close, volume)
            if (values.Length < 8)
                throw new FormatException("CSV line does not contain the expected number of fields.");

            // Parse date from index 2
            string dateStr = values[2];
            if (!TryParseDate(dateStr, out DateTime parsedDate))
                throw new FormatException($"Unable to parse date '{dateStr}'.");

            // Parse Open, High, Low, Close prices from indices 3..6
            if (!TryParseDecimal(values[3], out decimal parsedOpen))
                throw new FormatException($"Unable to parse open price '{values[3]}'.");

            if (!TryParseDecimal(values[4], out decimal parsedHigh))
                throw new FormatException($"Unable to parse high price '{values[4]}'.");

            if (!TryParseDecimal(values[5], out decimal parsedLow))
                throw new FormatException($"Unable to parse low price '{values[5]}'.");

            if (!TryParseDecimal(values[6], out decimal parsedClose))
                throw new FormatException($"Unable to parse close price '{values[6]}'.");

            // Parse volume from index 7
            if (!TryParseULong(values[7], out ulong parsedVolume))
                throw new FormatException($"Unable to parse volume '{values[7]}'.");

            // Assign parsed values to class properties
            date = parsedDate;
            open = parsedOpen;
            high = parsedHigh;
            low = parsedLow;
            close = parsedClose;
            volume = parsedVolume;
        }

        // --- Helper parsing methods ---

        /// <summary>
        /// Splits a single CSV line into individual fields, handling quotes and escaped quotes.
        /// </summary>
        /// <param name="line">CSV line to split</param>
        /// <returns>Array of field strings</returns>
        private static string[] SplitCsvLine(string line)
        {
            var fields = new List<string>(); // Store split fields
            if (line == null) return fields.ToArray(); // Return empty array if null

            var sb = new StringBuilder(); // Temporary buffer for current field
            bool inQuotes = false;        // Track if inside quotes

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i]; // Current character

                if (c == '"') // Handle quotes
                {
                    // Doubled quote inside quoted field
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"'); // Add one quote
                        i++;             // Skip next quote
                    }
                    else
                    {
                        inQuotes = !inQuotes; // Toggle inQuotes state
                    }
                }
                else if (c == ',' && !inQuotes) // Field separator
                {
                    fields.Add(sb.ToString()); // Add current field
                    sb.Clear();                // Clear buffer
                }
                else
                {
                    sb.Append(c); // Append character to field
                }
            }

            fields.Add(sb.ToString()); // Add last field

            // Clean up each field: remove quotes and trim
            for (int i = 0; i < fields.Count; i++)
            {
                fields[i] = UnquoteAndTrim(fields[i]);
            }

            return fields.ToArray(); // Return all fields
        }

        /// <summary>
        /// Removes surrounding quotes, replaces escaped quotes, and trims whitespace.
        /// </summary>
        /// <param name="s">Input string</param>
        /// <returns>Cleaned string</returns>
        private static string UnquoteAndTrim(string s)
        {
            if (s == null) return string.Empty; // Null input -> empty string

            s = s.Trim(); // Remove leading/trailing whitespace

            // If string is quoted, remove quotes and replace doubled quotes
            if (s.Length >= 2 && s[0] == '"' && s[s.Length - 1] == '"')
            {
                string inner = s.Substring(1, s.Length - 2); // Remove surrounding quotes
                return inner.Replace("\"\"", "\"").Trim();    // Replace doubled quotes and trim
            }

            return s; // Return unmodified if no surrounding quotes
        }

        /// <summary>
        /// Attempts to parse a string into a DateTime using multiple formats.
        /// </summary>
        /// <param name="s">Input string</param>
        /// <param name="dt">Parsed DateTime output</param>
        /// <returns>True if successful</returns>
        private static bool TryParseDate(string s, out DateTime dt)
        {
            string[] formats = new[] { "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss", "M/d/yyyy", "MM/dd/yyyy" };

            // Try parsing using exact known formats
            if (DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return true;

            // Fallback to general parsing
            return DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt);
        }

        /// <summary>
        /// Attempts to parse a string into a decimal using InvariantCulture.
        /// </summary>
        /// <param name="s">Input string</param>
        /// <param name="d">Parsed decimal output</param>
        /// <returns>True if successful</returns>
        private static bool TryParseDecimal(string s, out decimal d)
        {
            s = s?.Trim(); // Remove whitespace

            // Parse decimal, allow leading sign and decimal point
            return decimal.TryParse(s, NumberStyles.Number | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                                    CultureInfo.InvariantCulture, out d);
        }

        /// <summary>
        /// Attempts to parse a string into an unsigned long (volume).
        /// Handles numbers possibly formatted with decimal points.
        /// </summary>
        /// <param name="s">Input string</param>
        /// <param name="value">Parsed ulong output</param>
        /// <returns>True if successful</returns>
        private static bool TryParseULong(string s, out ulong value)
        {
            s = s?.Trim(); // Remove whitespace

            // Try direct ulong parse
            if (ulong.TryParse(s, NumberStyles.Integer | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out value))
                return true;

            // If fails, parse as decimal then cast to ulong
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal dec))
            {
                if (dec >= 0 && dec <= ulong.MaxValue)
                {
                    value = (ulong)dec; // Convert to ulong
                    return true;
                }
            }

            value = 0; // Default to 0 if parsing fails
            return false;
        }
    }
}