using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WindowsFormsApp1
{
    /// <summary>
    /// Represents a single candlestick with OHLCV (Open, High, Low, Close, Volume) data
    /// This class stores stock price data for a specific time period
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
        /// Default constructor - creates an empty candlestick
        /// </summary>
        public aCandlestick()
        {
            // Initialize with default values
            date = DateTime.MinValue;
            open = 0;
            high = 0;
            low = 0;
            close = 0;
            volume = 0;
        }

        /// <summary>
        /// Constructor that creates a candlestick from a CSV line.
        /// Expected CSV layout (example):
        /// "AAPL","D","2023-03-24","158.86","160.34","157.85","160.25","59256343",...
        /// This constructor is robust to quoted fields and doubled quotes inside fields.
        /// </summary>
        /// <param name="csvLine">A line from the CSV file containing candlestick data</param>
        public aCandlestick(string csvLine)
        {
            if (string.IsNullOrWhiteSpace(csvLine))
                throw new ArgumentException("CSV line is null or empty.", nameof(csvLine));

            string[] values = SplitCsvLine(csvLine);

            // We expect at least 8 columns: ticker, period, date, open, high, low, close, volume
            if (values.Length < 8)
                throw new FormatException("CSV line does not contain the expected number of fields.");

            // Date is at index 2
            string dateStr = values[2];
            if (!TryParseDate(dateStr, out DateTime parsedDate))
                throw new FormatException($"Unable to parse date '{dateStr}'.");

            // Open/High/Low/Close at indices 3..6
            if (!TryParseDecimal(values[3], out decimal parsedOpen))
                throw new FormatException($"Unable to parse open price '{values[3]}'.");

            if (!TryParseDecimal(values[4], out decimal parsedHigh))
                throw new FormatException($"Unable to parse high price '{values[4]}'.");

            if (!TryParseDecimal(values[5], out decimal parsedLow))
                throw new FormatException($"Unable to parse low price '{values[5]}'.");

            if (!TryParseDecimal(values[6], out decimal parsedClose))
                throw new FormatException($"Unable to parse close price '{values[6]}'.");

            // Volume at index 7
            if (!TryParseULong(values[7], out ulong parsedVolume))
                throw new FormatException($"Unable to parse volume '{values[7]}'.");

            date = parsedDate;
            open = parsedOpen;
            high = parsedHigh;
            low = parsedLow;
            close = parsedClose;
            volume = parsedVolume;
        }

        /// <summary>
        /// Parameterized constructor for creating a candlestick with specific values           
        /// </summary>
        /// <param name="date">Date and time of the candlestick</param>
        /// <param name="open">Opening price</param>
        /// <param name="high">Highest price</param>
        /// <param name="low">Lowest price</param>
        /// <param name="close">Closing price</param>
        /// <param name="volume">Trading volume</param>
        public aCandlestick(DateTime date, decimal open, decimal high, decimal low, decimal close, ulong volume)
        {
            this.date = date;
            this.open = open;
            this.high = high;
            this.low = low;
            this.close = close;
            this.volume = volume;
        }

        // --- Helper parsing methods ---

        private static string[] SplitCsvLine(string line)
        {
            var fields = new List<string>();
            if (line == null) return fields.ToArray();

            var sb = new StringBuilder();
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    // If this is a doubled quote inside a quoted field, append one quote and skip next
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++; // skip the escaped quote
                    }
                    else
                    {
                        // Toggle inQuotes state
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }

            // add last field
            fields.Add(sb.ToString());

            // Trim whitespace and surrounding quotes from each field
            for (int i = 0; i < fields.Count; i++)
            {
                fields[i] = UnquoteAndTrim(fields[i]);
            }

            return fields.ToArray();
        }

        private static string UnquoteAndTrim(string s)
        {
            if (s == null) return string.Empty;
            s = s.Trim();
            if (s.Length >= 2 && s[0] == '"' && s[s.Length - 1] == '"')
            {
                // remove surrounding quotes and replace doubled quotes with single quote
                string inner = s.Substring(1, s.Length - 2);
                return inner.Replace("\"\"", "\"").Trim();
            }
            return s;
        }

        private static bool TryParseDate(string s, out DateTime dt)
        {
            // try exact known formats first, then fallback to TryParse with InvariantCulture
            string[] formats = new[] { "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss", "M/d/yyyy", "MM/dd/yyyy" };
            if (DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return true;

            return DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt);
        }

        private static bool TryParseDecimal(string s, out decimal d)
        {
            // Accept decimal separator '.' using InvariantCulture
            s = s?.Trim();
            return decimal.TryParse(s, NumberStyles.Number | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out d);
        }

        private static bool TryParseULong(string s, out ulong value)
        {
            s = s?.Trim();
            // Some CSV exports may use decimals for volume (rare) — try to parse integer portion
            if (ulong.TryParse(s, NumberStyles.Integer | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out value))
                return true;

            // Try parse as decimal then cast
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal dec))
            {
                if (dec >= 0 && dec <= ulong.MaxValue)
                {
                    value = (ulong)dec;
                    return true;
                }
            }

            value = 0;
            return false;
        }
    }
}
