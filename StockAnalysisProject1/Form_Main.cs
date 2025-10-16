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
        private List<aCandlestick> listOfCandlesticks = new List<aCandlestick>();
        private BindingList<aCandlestick> boundCandlesticks = new BindingList<aCandlestick>();

        /// <summary>
        /// Initializes the form, sets default date range, and prepares chart structure.
        /// </summary>
        public Form_Basic()
        {
            InitializeComponent();
            dateTimePicker_Start.Value = DateTime.Now.AddYears(-1);
            dateTimePicker_End.Value = DateTime.Now;
            InitializeChart();
        }

        /// <summary>
        /// Opens the file dialog to allow the user to select a stock CSV file.
        /// </summary>
        private void button_fireOpenFileDialog_Click(object sender, EventArgs e)
        {
            openFileDialog_fileSelector.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog_fileSelector.ShowDialog();
        }

        /// <summary>
        /// Handles the FileOk event for the OpenFileDialog.
        /// Executes the main pipeline:
        /// readCandlesticksFromFile() → filterCandlesticks() → normalizeChart() → displayCandlesticks().
        /// </summary>
        private void openFileDialog_fileSelector_FileOk(object sender, CancelEventArgs e)
        {
            string filename = openFileDialog_fileSelector.FileName;
            this.Text = "Loading: " + Path.GetFileName(filename);

            readCandlesticksFromFile();
            filterCandlesticks();
            normalizeChart();
            displayCandlesticks();

            this.Text = $"Loaded {listOfCandlesticks.Count} candlesticks from {Path.GetFileName(filename)}";
        }

        /// <summary>
        /// Handles the Update button click event.
        /// Re-filters and refreshes the chart based on selected date range.
        /// </summary>
        private void button_Update_Click(object sender, EventArgs e)
        {
            if (listOfCandlesticks == null || listOfCandlesticks.Count == 0)
            {
                MessageBox.Show("Please load a stock file first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            filterCandlesticks();
            normalizeChart();
            displayCandlesticks();
        }

        /// <summary>
        /// Reads a stock CSV file and returns a list of parsed candlesticks.
        /// This is the parameterized version required by the assignment.
        /// </summary>
        private List<aCandlestick> readCandlestickFile(string tickerFile)
        {
            var candlesticks = new List<aCandlestick>();

            try
            {
                var lines = File.ReadAllLines(tickerFile)
                                .Where(l => !string.IsNullOrWhiteSpace(l))
                                .ToArray();

                for (int i = 1; i < lines.Length; i++)
                {
                    try
                    {
                        aCandlestick c = new aCandlestick(lines[i]);
                        candlesticks.Add(c);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return candlesticks;
        }

        /// <summary>
        /// Reads candlesticks from the file currently selected in the OpenFileDialog
        /// and updates the form’s listOfCandlesticks.
        /// </summary>
        private void readCandlesticksFromFile()
        {
            string filename = openFileDialog_fileSelector.FileName;
            listOfCandlesticks = readCandlestickFile(filename);
            listOfCandlesticks = listOfCandlesticks.OrderBy(c => c.date).ToList();
        }

        /// <summary>
        /// Filters the given list of candlesticks within the specified date range.
        /// Returns a new filtered list.
        /// </summary>
        private List<aCandlestick> filterCandlesticks(List<aCandlestick> unfilteredList, DateTime startDate, DateTime endDate)
        {
            return unfilteredList
                .Where(c => c.date.Date >= startDate.Date && c.date.Date <= endDate.Date)
                .OrderBy(c => c.date)
                .ToList();
        }

        /// <summary>
        /// Filters the form’s candlestick data based on the start and end DateTimePickers
        /// and updates the bound BindingList and DataGridView.
        /// </summary>
        private void filterCandlesticks()
        {
            DateTime start = dateTimePicker_Start.Value.Date;
            DateTime end = dateTimePicker_End.Value.Date;

            var filteredList = filterCandlesticks(listOfCandlesticks, start, end);
            boundCandlesticks = new BindingList<aCandlestick>(filteredList);
            dataGridView1.DataSource = boundCandlesticks;
        }

        /// <summary>
        /// Normalizes the Y-axis of the OHLC chart by setting its min and max
        /// based on the filtered candlesticks’ high and low values ±2%.
        /// </summary>
        private void normalizeChart()
        {
            if (boundCandlesticks == null || boundCandlesticks.Count == 0)
                return;

            decimal minLow = boundCandlesticks.Min(c => c.low);
            decimal maxHigh = boundCandlesticks.Max(c => c.high);

            double minAxis = (double)(minLow * 0.98m);
            double maxAxis = (double)(maxHigh * 1.02m);

            var area = chart1.ChartAreas["ChartArea_OHLC"];
            area.AxisY.Minimum = minAxis;
            area.AxisY.Maximum = maxAxis;
        }

        /// <summary>
        /// Initializes the chart areas and series for candlestick and volume plots.
        /// </summary>
        private void InitializeChart()
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            ChartArea areaOHLC = new ChartArea("ChartArea_OHLC");
            areaOHLC.AxisX.LabelStyle.Format = "MM/dd";
            areaOHLC.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            areaOHLC.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            areaOHLC.AxisY.Title = "Price";
            chart1.ChartAreas.Add(areaOHLC);

            ChartArea areaVol = new ChartArea("ChartArea_Volume");
            areaVol.AlignWithChartArea = "ChartArea_OHLC";
            areaVol.AlignmentOrientation = AreaAlignmentOrientations.Vertical;
            areaVol.AxisX.LabelStyle.Format = "MM/dd";
            areaVol.AxisY.MajorGrid.Enabled = false;
            areaVol.AxisY.Title = "Volume";
            chart1.ChartAreas.Add(areaVol);

            Series seriesOHLC = new Series("Series_OHLC");
            seriesOHLC.ChartType = SeriesChartType.Candlestick;
            seriesOHLC.ChartArea = "ChartArea_OHLC";
            seriesOHLC.XValueType = ChartValueType.Date;
            seriesOHLC.YValuesPerPoint = 4;
            seriesOHLC.CustomProperties = "PriceDownColor=Red,PriceUpColor=Lime";
            seriesOHLC["OpenCloseStyle"] = "Triangle";
            seriesOHLC["ShowOpenClose"] = "Both";
            chart1.Series.Add(seriesOHLC);

            Series seriesVol = new Series("Series_Volume");
            seriesVol.ChartType = SeriesChartType.Column;
            seriesVol.ChartArea = "ChartArea_Volume";
            seriesVol.XValueType = ChartValueType.Date;
            seriesVol.YAxisType = AxisType.Primary;
            chart1.Series.Add(seriesVol);
        }

        /// <summary>
        /// Binds the filtered candlestick data to the chart
        /// and refreshes the OHLC and Volume visualizations.
        /// </summary>
        private void displayCandlesticks()
        {
            if (boundCandlesticks == null || boundCandlesticks.Count == 0)
            {
                chart1.Series["Series_OHLC"].Points.Clear();
                chart1.Series["Series_Volume"].Points.Clear();
                return;
            }

            chart1.DataSource = boundCandlesticks;
            chart1.Series["Series_OHLC"].XValueMember = "date";
            chart1.Series["Series_OHLC"].YValueMembers = "high,low,open,close";
            chart1.Series["Series_OHLC"].XValueType = ChartValueType.Date;
            chart1.Series["Series_Volume"].XValueMember = "date";
            chart1.Series["Series_Volume"].YValueMembers = "volume";
            chart1.Series["Series_Volume"].XValueType = ChartValueType.Date;

            chart1.DataBind();
        }

        /// <summary>
        /// Placeholder handler for DataGridView cell clicks.
        /// </summary>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        /// <summary>
        /// Placeholder handler for chart clicks.
        /// </summary>
        private void chart1_Click(object sender, EventArgs e) { }
    }
}
