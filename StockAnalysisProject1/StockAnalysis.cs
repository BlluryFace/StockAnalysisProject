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
        // List containing all candlestick data loaded from the file
        private List<aCandlestick> listOfCandlesticks = new List<aCandlestick>();

        // BindingList used to bind filtered candlesticks to DataGridView
        private BindingList<aCandlestick> filteredCandlesticks = new BindingList<aCandlestick>();

        /// <summary>
        /// Initializes the form, sets default date range, and prepares chart structure.
        /// </summary>
        public Form_Basic()
        {
                    InitializeComponent(); // initialize designer controls

            dateTimePicker_Start.Value = DateTime.Now.AddYears(-1); // default start = 1 year ago
            dateTimePicker_End.Value = DateTime.Now; // default end = today

            InitializeChart(); // prepare chart areas and series
        }

        /// <summary>
        /// Opens the file dialog to allow the user to select a stock CSV file.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event args</param>
        private void button_fireOpenFileDialog_Click(object sender, EventArgs e)
        {
            openFileDialog_fileSelector.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"; // ensure filter
            openFileDialog_fileSelector.ShowDialog(); // show dialog to pick file
        }

        /// <summary>
        /// Handles the FileOk event for the OpenFileDialog.
        /// Loads the selected stock CSV, updates the chart and DataGridView,
        /// and displays the stock symbol extracted from the file name.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">CancelEventArgs</param>
        private void openFileDialog_fileSelector_FileOk(object sender, CancelEventArgs e)
        {
            string filename = openFileDialog_fileSelector.FileName; // get selected file path
            string stockSymbol = Path.GetFileNameWithoutExtension(filename).Split('-')[0]; // derive symbol from filename
            label_StockSymbol.Text = $"Stock: {stockSymbol}"; // show stock symbol on form
            this.Text = "Loading: " + Path.GetFileName(filename); // update form title during load

            readCandlesticksFromFile(); // read data into listOfCandlesticks
            filterCandlesticks(); // filter using date pickers and set binding list
            normalizeChart(); // adjust chart axis to data range
            displayCandlesticks(); // bind filtered data to chart and grid

            this.Text = $"Loaded {listOfCandlesticks.Count} candlesticks for {stockSymbol} from {Path.GetFileName(filename)}"; // final title
        }

        /// <summary>
        /// Handles the Update button click event.
        /// Re-filters and refreshes the chart based on selected date range.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event args</param>
        private void button_Update_Click(object sender, EventArgs e)
        {
            if (listOfCandlesticks == null || listOfCandlesticks.Count == 0) // ensure data exists
            {
                MessageBox.Show("Please load a stock file first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning); // warn user
                return; // do nothing further
            }

            filterCandlesticks(); // update filtered list
            normalizeChart(); // update axis
            displayCandlesticks(); // refresh UI
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
