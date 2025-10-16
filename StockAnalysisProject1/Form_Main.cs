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

        public Form_Basic()
        {
            InitializeComponent();

            // Optional default date range
            dateTimePicker_Start.Value = DateTime.Now.AddYears(-1);
            dateTimePicker_End.Value = DateTime.Now;

            // Initialize chart structure dynamically
            InitializeChart();
        }

        // ------------------ UI Event Handlers ------------------

        private void button_fireOpenFileDialog_Click(object sender, EventArgs e)
        {
            openFileDialog_fileSelector.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog_fileSelector.ShowDialog();
        }

        private void openFileDialog_fileSelector_FileOk(object sender, CancelEventArgs e)
        {
            string filename = openFileDialog_fileSelector.FileName;
            this.Text = "Loading: " + filename;

            // Load candlestick data
            listOfCandlesticks = readCandlestickFile(filename);

            if (listOfCandlesticks == null || listOfCandlesticks.Count == 0)
            {
                MessageBox.Show("No valid candlestick data found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Sort by date
            listOfCandlesticks = listOfCandlesticks.OrderBy(c => c.date).ToList();

            // Bind full data
            filterCandlesticks();

            this.Text = $"Loaded {listOfCandlesticks.Count} candlesticks from {Path.GetFileName(filename)}";
        }

        private void button_Update_Click(object sender, EventArgs e)
        {
            if (listOfCandlesticks == null || listOfCandlesticks.Count == 0)
            {
                MessageBox.Show("Please load a stock file first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            filterCandlesticks();
        }

        // ------------------ Core Methods ------------------

        private List<aCandlestick> readCandlestickFile(string tickerFile)
        {
            List<aCandlestick> candlesticks = new List<aCandlestick>();

            try
            {
                var lines = File.ReadAllLines(tickerFile)
                                .Where(l => !string.IsNullOrWhiteSpace(l))
                                .ToArray();

                // Skip header (index 0)
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];
                    try
                    {
                        var c = new aCandlestick(line);
                        candlesticks.Add(c);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Skipping bad line {i}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return candlesticks;
        }

        private void filterCandlesticks()
        {
            var start = dateTimePicker_Start.Value.Date;
            var end = dateTimePicker_End.Value.Date;

            var filtered = listOfCandlesticks
                .Where(c => c.date >= start && c.date <= end)
                .OrderBy(c => c.date)
                .ToList();

            boundCandlesticks = new BindingList<aCandlestick>(filtered);
            dataGridView1.DataSource = boundCandlesticks;

            RenderCandlestickChart(filtered);
        }

        // ------------------ Chart Configuration ------------------

        private void InitializeChart()
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            // Main area for OHLC
            ChartArea areaPrice = new ChartArea("ChartArea_OHLC");
            areaPrice.AxisX.LabelStyle.Format = "MM/dd";
            areaPrice.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            areaPrice.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            areaPrice.AxisY.Title = "Price";
            chart1.ChartAreas.Add(areaPrice);

            // Volume area
            ChartArea areaVolume = new ChartArea("ChartArea_Volume");
            areaVolume.AlignWithChartArea = "ChartArea_OHLC";
            areaVolume.AlignmentOrientation = AreaAlignmentOrientations.Vertical;
            areaVolume.AxisX.LabelStyle.Format = "MM/dd";
            areaVolume.AxisY.MajorGrid.Enabled = false;
            areaVolume.AxisY.Title = "Volume";
            chart1.ChartAreas.Add(areaVolume);

            // Price series
            Series sPrice = new Series("Series_OHLC");
            sPrice.ChartType = SeriesChartType.Candlestick;
            sPrice.ChartArea = "ChartArea_OHLC";
            sPrice.XValueType = ChartValueType.Date;
            sPrice.YValuesPerPoint = 4;
            sPrice.CustomProperties = "PriceDownColor=Red,PriceUpColor=Lime";
            sPrice["OpenCloseStyle"] = "Triangle";
            sPrice["ShowOpenClose"] = "Both";
            chart1.Series.Add(sPrice);

            // Volume series
            Series sVolume = new Series("Series_Volume");
            sVolume.ChartType = SeriesChartType.Column;
            sVolume.ChartArea = "ChartArea_Volume";
            sVolume.XValueType = ChartValueType.Date;
            sVolume.YAxisType = AxisType.Primary;
            chart1.Series.Add(sVolume);
        }

        private void RenderCandlestickChart(List<aCandlestick> data)
        {
            if (data == null || data.Count == 0)
            {
                chart1.Series["Series_OHLC"].Points.Clear();
                chart1.Series["Series_Volume"].Points.Clear();
                return;
            }

            // Clear old data
            chart1.Series["Series_OHLC"].Points.Clear();
            chart1.Series["Series_Volume"].Points.Clear();

            foreach (var c in data)
            {
                int idx = chart1.Series["Series_OHLC"].Points.AddXY(c.date, c.high);
                var pt = chart1.Series["Series_OHLC"].Points[idx];
                pt.YValues = new double[] { (double)c.high, (double)c.low, (double)c.open, (double)c.close };

                chart1.Series["Series_Volume"].Points.AddXY(c.date, (double)c.volume);
            }

            // Normalize Y range
            double min = (double)data.Min(c => c.low) * 0.98;
            double max = (double)data.Max(c => c.high) * 1.02;
            chart1.ChartAreas["ChartArea_OHLC"].AxisY.Minimum = min;
            chart1.ChartAreas["ChartArea_OHLC"].AxisY.Maximum = max;
        }

        // ------------------ Optional Event Handlers ------------------

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void chart1_Click(object sender, EventArgs e) { }
    }
}
