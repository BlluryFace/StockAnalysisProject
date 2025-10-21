using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    public partial class Form_Basic : Form
    {
        // master storage: all periods read for current symbol (keys: "Day","Week","Month","File")
        private Dictionary<string, List<aCandlestick>> candlesticksByPeriod = new Dictionary<string, List<aCandlestick>>(StringComparer.OrdinalIgnoreCase);

        // currently active list (bound after filtering)
        private List<aCandlestick> listOfCandlesticks = new List<aCandlestick>();

        // BindingList used to bind filtered candlesticks to DataGridView
        private BindingList<aCandlestick> filteredCandlesticks = new BindingList<aCandlestick>();

        // currently selected internal period key (e.g. "Day","Week","Month","File")
        private string selectedPeriodKey = null;

        /// <summary>
        /// Initializes the form, sets default date range, and prepares chart structure.
        /// </summary>
        public Form_Basic()
        {
            InitializeComponent(); // initialize designer controls

            dateTimePicker_Start.Value = DateTime.Now.AddYears(-1); // default start = 1 year ago
            dateTimePicker_End.Value = DateTime.Now; // default end = today

            InitializeChart(); // prepare chart areas and series

            // initially set period combobox to empty
            comboBox_Period.Items.Clear();
            selectedPeriodKey = null;
        }

        /// <summary>
        /// Opens the file dialog to allow the user to select a stock CSV file.
        /// </summary>
        private void button_fireOpenFileDialog_Click(object sender, EventArgs e)
        {
            openFileDialog_fileSelector.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"; // ensure filter
            openFileDialog_fileSelector.ShowDialog(); // show dialog to pick file
        }

        /// <summary>
        /// FileDialog OK handler. Loads available period files for the selected symbol into memory,
        /// and displays the default dataset immediately. Further changes require the Update button.
        /// </summary>
        private void openFileDialog_fileSelector_FileOk(object sender, CancelEventArgs e)
        {
            string filename = openFileDialog_fileSelector.FileName; // get selected file path
            string baseName = Path.GetFileNameWithoutExtension(filename);
            string stockSymbol = baseName.Split('-')[0]; // derive symbol from filename
            label_StockSymbol.Text = $"Stock: {stockSymbol}"; // show stock symbol on form
            this.Text = "Loading: " + Path.GetFileName(filename); // update form title during load

            // Attempt to find and read Day/Week/Month files for this symbol (populate dictionary)
            LoadPeriodsForSymbol(filename);

            // Populate combobox with available periods but do not refresh when user changes them later
            // Detach handler while we populate and set initial selection to avoid triggering selected change behavior
            comboBox_Period.SelectedIndexChanged -= comboBox_Period_SelectedIndexChanged;
            comboBox_Period.Items.Clear();

            if (candlesticksByPeriod.ContainsKey("Day")) comboBox_Period.Items.Add("Daily");
            if (candlesticksByPeriod.ContainsKey("Week")) comboBox_Period.Items.Add("Weekly");
            if (candlesticksByPeriod.ContainsKey("Month")) comboBox_Period.Items.Add("Monthly");

            // If none of the standard period files were found, load the selected file as a single "File" dataset.
            if (comboBox_Period.Items.Count == 0)
            {
                // add single loaded file dataset under internal key "File"
                try
                {
                    var rows = readCandlestickFile(filename).OrderBy(c => c.date).ToList();
                    if (rows.Count > 0)
                    {
                        candlesticksByPeriod["File"] = rows;
                        comboBox_Period.Items.Add("Loaded File");
                        selectedPeriodKey = "File";
                        // set combobox to show the loaded-file display
                        comboBox_Period.SelectedItem = "Loaded File";
                    }
                    else
                    {
                        // no rows parsed
                        selectedPeriodKey = null;
                    }
                }
                catch
                {
                    selectedPeriodKey = null;
                }
            }
            else
            {
                // choose a default period (Week preferred)
                string defaultKey = candlesticksByPeriod.ContainsKey("Week") ? "Week" :
                                    candlesticksByPeriod.ContainsKey("Day") ? "Day" :
                                    candlesticksByPeriod.ContainsKey("Month") ? "Month" : null;

                // compute display string for defaultKey if present
                string displayForDefault = null;
                if (defaultKey != null)
                {
                    displayForDefault = defaultKey.Equals("Week", StringComparison.OrdinalIgnoreCase) ? "Weekly" :
                                        defaultKey.Equals("Day", StringComparison.OrdinalIgnoreCase) ? "Daily" :
                                        defaultKey.Equals("Month", StringComparison.OrdinalIgnoreCase) ? "Monthly" : null;
                }

                // If we have a preferred default and it's available in items, select it; otherwise select first item
                if (!string.IsNullOrEmpty(displayForDefault) && comboBox_Period.Items.Contains(displayForDefault))
                {
                    comboBox_Period.SelectedItem = displayForDefault;
                    // set internal key to defaultKey
                    selectedPeriodKey = defaultKey;
                }
                else
                {
                    // select the first combobox item for user convenience and set selectedPeriodKey accordingly
                    comboBox_Period.SelectedIndex = 0;
                    string display = comboBox_Period.SelectedItem as string;
                    selectedPeriodKey = display.Equals("Daily", StringComparison.OrdinalIgnoreCase) ? "Day" :
                                        display.Equals("Weekly", StringComparison.OrdinalIgnoreCase) ? "Week" :
                                        display.Equals("Monthly", StringComparison.OrdinalIgnoreCase) ? "Month" : null;
                }
            }

            // Reattach handler
            comboBox_Period.SelectedIndexChanged += comboBox_Period_SelectedIndexChanged;

            // Display the chosen dataset immediately (initial display)
            if (!string.IsNullOrEmpty(selectedPeriodKey) && candlesticksByPeriod.ContainsKey(selectedPeriodKey))
            {
                listOfCandlesticks = candlesticksByPeriod[selectedPeriodKey];
            }
            else if (candlesticksByPeriod.ContainsKey("File"))
            {
                listOfCandlesticks = candlesticksByPeriod["File"];
            }
            else
            {
                // fallback: try reading the selected file directly
                listOfCandlesticks = readCandlestickFile(filename).OrderBy(c => c.date).ToList();
            }

            // show initial data immediately
            filterCandlesticks();
            normalizeChart();
            displayCandlesticks();

            // Update status label to indicate datasets available — UI already bound to initial dataset
            label_Status.Text = $"Displayed {filteredCandlesticks.Count} rows. Available: {string.Join(", ", comboBox_Period.Items.Cast<string>())}";
            this.Text = $"Loaded {stockSymbol} (initial view)";
        }

        /// <summary>
        /// Loads Day/Week/Month files for the same symbol as the passed file path.
        /// Fills candlesticksByPeriod dictionary for available periods.
        /// </summary>
        /// <param name="anyFilePathForSymbol">A file path in the same folder for the desired symbol</param>
        private void LoadPeriodsForSymbol(string anyFilePathForSymbol)
        {
            candlesticksByPeriod.Clear(); // clear previous

            string dir = Path.GetDirectoryName(anyFilePathForSymbol) ?? ".";
            string baseName = Path.GetFileNameWithoutExtension(anyFilePathForSymbol);
            string symbol = baseName.Split('-')[0];

            // expected file name patterns
            var candidates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Day", Path.Combine(dir, $"{symbol}-Day.csv") },
                { "Week", Path.Combine(dir, $"{symbol}-Week.csv") },
                { "Month", Path.Combine(dir, $"{symbol}-Month.csv") }
            };

            foreach (var kv in candidates)
            {
                string key = kv.Key;
                string path = kv.Value;
                if (File.Exists(path))
                {
                    try
                    {
                        var rows = readCandlestickFile(path).OrderBy(c => c.date).ToList();
                        if (rows.Count > 0)
                            candlesticksByPeriod[key] = rows;
                    }
                    catch
                    {
                        // ignore individual read errors, continue with others
                    }
                }
            }
        }

        /// <summary>
        /// Handles period selection change from the ComboBox.
        /// Only records the selected internal period key; does NOT refresh UI.
        /// </summary>
        private void comboBox_Period_SelectedIndexChanged(object sender, EventArgs e)
        {
            // map display text to internal key but do not call display methods here
            string display = comboBox_Period.SelectedItem as string;
            if (string.IsNullOrEmpty(display))
            {
                selectedPeriodKey = null;
                label_Status.Text = "No period selected (click Update to apply).";
                return;
            }

            selectedPeriodKey = display.Equals("Daily", StringComparison.OrdinalIgnoreCase) ? "Day" :
                                display.Equals("Weekly", StringComparison.OrdinalIgnoreCase) ? "Week" :
                                display.Equals("Monthly", StringComparison.OrdinalIgnoreCase) ? "Month" :
                                display.Equals("Loaded File", StringComparison.OrdinalIgnoreCase) ? "File" : null;

            // update status to show what will be applied on Update click
            if (selectedPeriodKey != null)
                label_Status.Text = $"Selected period: {display} (click Update to apply)";
        }

        /// <summary>
        /// Button Update click — applies the chosen period (if any), then re-filters and redisplays.
        /// </summary>
        private void button_Update_Click(object sender, EventArgs e)
        {
            // If a period is selected in the combo box, switch the active dataset now
            if (!string.IsNullOrEmpty(selectedPeriodKey) && candlesticksByPeriod.ContainsKey(selectedPeriodKey))
            {
                listOfCandlesticks = candlesticksByPeriod[selectedPeriodKey];
            }

            // Ensure we have data to display
            if (listOfCandlesticks == null || listOfCandlesticks.Count == 0)
            {
                MessageBox.Show("Please load a stock file first and choose a period, then click Update.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Apply filter / normalize / display now (only on Update click)
            filterCandlesticks();
            normalizeChart();
            displayCandlesticks();

            // update status
            label_Status.Text = $"Displayed {filteredCandlesticks.Count} rows for {comboBox_Period.SelectedItem}";
        }

        /// <summary>
        /// Reads a stock CSV file and returns a list of parsed candlesticks.
        /// Parameterized version.
        /// </summary>
        /// <param name="tickerFile">CSV file path</param>
        /// <returns>List of candlestick objects</returns>
        private List<aCandlestick> readCandlestickFile(string tickerFile)
        {
            var candlesticks = new List<aCandlestick>(); // temporary container for parsed rows

            try
            {
                // read all non-empty lines from the file
                var lines = File.ReadAllLines(tickerFile).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

                // parse each data line skipping the header at index 0
                for (int i = 1; i < lines.Length; i++)
                {
                    try
                    {
                        aCandlestick c = new aCandlestick(lines[i]); // construct candlestick from CSV line
                        candlesticks.Add(c); // append parsed object
                    }
                    catch
                    {
                        // ignore malformed lines but continue parsing remaining rows
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // show read error
            }

            return candlesticks; // return parsed list (may be empty)
        }

        /// <summary>
        /// Reads candlesticks from the currently selected file and updates the form’s list.
        /// Void version.
        /// </summary>
        private void readCandlesticksFromFile()
        {
            string filename = openFileDialog_fileSelector.FileName; // get chosen file path
            listOfCandlesticks = readCandlestickFile(filename); // read and parse file into list
            listOfCandlesticks = listOfCandlesticks.OrderBy(c => c.date).ToList(); // ensure chronological order
        }

        /// <summary>
        /// Filters a given list of candlesticks within a date range.
        /// Parameterized version.
        /// </summary>
        /// <param name="unfilteredList">List to filter</param>
        /// <param name="startDate">Start of date range</param>
        /// <param name="endDate">End of date range</param>
        /// <returns>Filtered and sorted list</returns>
        private List<aCandlestick> filterCandlesticks(List<aCandlestick> unfilteredList, DateTime startDate, DateTime endDate)
        {
            // return only rows whose date is between start and end inclusive, sorted chronologically
            return unfilteredList
                .Where(c => c.date.Date >= startDate.Date && c.date.Date <= endDate.Date)
                .OrderBy(c => c.date)
                .ToList();
        }

        /// <summary>
        /// Filters the form’s candlestick data and updates the DataGridView.
        /// Void version.
        /// </summary>
        private void filterCandlesticks()
        {
            DateTime start = dateTimePicker_Start.Value.Date; // get start date from UI
            DateTime end = dateTimePicker_End.Value.Date; // get end date from UI

            var filteredList = filterCandlesticks(listOfCandlesticks, start, end); // call parameterized filter
            filteredCandlesticks = new BindingList<aCandlestick>(filteredList); // create BindingList for UI binding
            dataGridView_Candles.DataSource = filteredCandlesticks; // bind grid to filtered data
        }

        /// <summary>
        /// Normalizes the Y-axis of the OHLC chart using filtered data ±2%.
        /// </summary>
        private void normalizeChart()
        {
            if (filteredCandlesticks == null || filteredCandlesticks.Count == 0) // nothing to normalize
                return; // exit early

            decimal minLow = filteredCandlesticks.Min(c => c.low); // find minimum low
            decimal maxHigh = filteredCandlesticks.Max(c => c.high); // find maximum high

            double minAxis = (double)(minLow * 0.98m); // apply -2% padding
            double maxAxis = (double)(maxHigh * 1.02m); // apply +2% padding

            var area = chart_Candles.ChartAreas["ChartArea_OHLC"]; // obtain OHLC chart area
            area.AxisY.Minimum = minAxis; // set Y-axis minimum
            area.AxisY.Maximum = maxAxis; // set Y-axis maximum
        }

        /// <summary>
        /// Initializes chart areas and series for OHLC and volume.
        /// </summary>
        private void InitializeChart()
        {
            chart_Candles.Series.Clear(); // remove any existing series
            chart_Candles.ChartAreas.Clear(); // remove any existing chart areas

            ChartArea areaOHLC = new ChartArea("ChartArea_OHLC"); // create OHLC chart area
            areaOHLC.AxisX.LabelStyle.Format = "MM/dd"; // format X axis labels
            areaOHLC.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray; // set grid color
            areaOHLC.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray; // set Y grid color
            areaOHLC.AxisY.Title = "Price"; // Y axis title
            chart_Candles.ChartAreas.Add(areaOHLC); // add OHLC area

            ChartArea areaVol = new ChartArea("ChartArea_Volume"); // create volume area
            areaVol.AlignWithChartArea = "ChartArea_OHLC"; // align X axis with OHLC area
            areaVol.AlignmentOrientation = AreaAlignmentOrientations.Vertical; // vertical alignment
            areaVol.AxisX.LabelStyle.Format = "MM/dd"; // format X labels
            areaVol.AxisY.MajorGrid.Enabled = false; // hide Y grid for volume
            areaVol.AxisY.Title = "Volume"; // volume axis title
            chart_Candles.ChartAreas.Add(areaVol); // add volume area

            Series seriesOHLC = new Series("Series_OHLC"); // define OHLC series
            seriesOHLC.ChartType = SeriesChartType.Candlestick; // set chart type
            seriesOHLC.ChartArea = "ChartArea_OHLC"; // attach to OHLC area
            seriesOHLC.XValueType = ChartValueType.Date; // X values are dates
            seriesOHLC.YValuesPerPoint = 4; // open, high, low, close
            seriesOHLC.CustomProperties = "PriceDownColor=Red,PriceUpColor=Lime"; // colors
            seriesOHLC["OpenCloseStyle"] = "Triangle"; // open/close marker style
            seriesOHLC["ShowOpenClose"] = "Both"; // show both markers
            chart_Candles.Series.Add(seriesOHLC); // add series to chart

            Series seriesVol = new Series("Series_Volume"); // define volume series
            seriesVol.ChartType = SeriesChartType.Column; // column chart for volume
            seriesVol.ChartArea = "ChartArea_Volume"; // attach to volume area
            seriesVol.XValueType = ChartValueType.Date; // X values are dates
            seriesVol.YAxisType = AxisType.Primary; // use primary Y axis
            chart_Candles.Series.Add(seriesVol); // add series to chart
        }

        /// <summary>
        /// Binds filtered candlestick data to the chart and refreshes display.
        /// </summary>
        private void displayCandlesticks()
        {
            if (filteredCandlesticks == null || filteredCandlesticks.Count == 0) // no data -> clear and exit
            {
                if (chart_Candles.Series.IndexOf("Series_OHLC") >= 0) chart_Candles.Series["Series_OHLC"].Points.Clear(); // clear points
                if (chart_Candles.Series.IndexOf("Series_Volume") >= 0) chart_Candles.Series["Series_Volume"].Points.Clear(); // clear points
                return; // nothing further
            }

            chart_Candles.DataSource = filteredCandlesticks; // set chart data source to binding list

            // configure OHLC series mapping
            var ohlc = chart_Candles.Series["Series_OHLC"]; // get OHLC series
            ohlc.XValueMember = "date"; // bind X to date property
            ohlc.YValueMembers = "high,low,open,close"; // set the expected Y members order
            ohlc.XValueType = ChartValueType.Date; // ensure X is date

            // configure Volume series mapping
            var vol = chart_Candles.Series["Series_Volume"]; // get volume series
            vol.XValueMember = "date"; // bind X to date property
            vol.YValueMembers = "volume"; // bind Y to volume
            vol.XValueType = ChartValueType.Date; // ensure X is date

            chart_Candles.DataBind(); // push data to chart control
            label_Status.Text = $"Loaded {filteredCandlesticks.Count} candlesticks"; // update status label
        }
    }
}
