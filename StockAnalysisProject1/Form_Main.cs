using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    public partial class Form_Basic : Form
    {
        private List<aCandlestick> listOfCandlesticks = new List<aCandlestick>();

        public Form_Basic()
        {
            InitializeComponent();
        }

        private void button_fireOpenFileDialog_Click(object sender, EventArgs e)
        {
            openFileDialog_fileSelector.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog_fileSelector.ShowDialog();
        }

        private void openFileDialog_fileSelector_FileOk(object sender, CancelEventArgs e)
        {
            string filename = openFileDialog_fileSelector.FileName;
            Text = "Loading: " + filename;

            listOfCandlesticks = readCandlestickFile(filename);

            if (listOfCandlesticks == null || listOfCandlesticks.Count == 0)
            {
                MessageBox.Show("No candlestick data found.");
                return;
            }

            // Bind all data to grid initially
            BindToDataGridView(listOfCandlesticks);

            // Set date range pickers
            dateTimePicker_Start.Value = listOfCandlesticks.First().date;
            dateTimePicker_End.Value = listOfCandlesticks.Last().date;

            // Draw chart
            RenderCandlestickChart(listOfCandlesticks);
        }

        private List<aCandlestick> readCandlestickFile(string tickerFile)
        {
            List<aCandlestick> candlesticks = new List<aCandlestick>();

            try
            {
                var lines = File.ReadAllLines(tickerFile)
                                .Where(l => !string.IsNullOrWhiteSpace(l))
                                .ToArray();

                for (int i = 1; i < lines.Length; i++) // Skip header
                {
                    try
                    {
                        candlesticks.Add(new aCandlestick(lines[i]));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Skipping bad line {i}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading file: " + ex.Message);
            }

            return candlesticks.OrderBy(c => c.date).ToList();
        }

        // Filters and rebinds the chart and grid
        private void button_Update_Click(object sender, EventArgs e)
        {
            if (listOfCandlesticks == null || listOfCandlesticks.Count == 0)
                return;

            var filtered = filterCandlesticks(
                listOfCandlesticks,
                dateTimePicker_Start.Value,
                dateTimePicker_End.Value
            );

            BindToDataGridView(filtered);
            RenderCandlestickChart(filtered);
        }

        public List<aCandlestick> filterCandlesticks(List<aCandlestick> unfilteredList, DateTime startDate, DateTime endDate)
        {
            return unfilteredList
                .Where(c => c.date >= startDate && c.date <= endDate)
                .OrderBy(c => c.date)
                .ToList();
        }

        // ---------------- Binding & Chart Rendering ----------------

        private void BindToDataGridView(List<aCandlestick> data)
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = new BindingList<aCandlestick>(data);
            dataGridView1.AutoResizeColumns();
        }

        private void RenderCandlestickChart(List<aCandlestick> data)
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            var area = new ChartArea("MainArea");
            area.AxisX.LabelStyle.Format = "MM/dd";
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            chart1.ChartAreas.Add(area);

            // ----- Candlestick Series -----
            Series candleSeries = new Series("Price");
            candleSeries.ChartType = SeriesChartType.Candlestick;
            candleSeries.XValueType = ChartValueType.Date;
            candleSeries.YValuesPerPoint = 4;
            candleSeries["OpenCloseStyle"] = "Triangle";
            candleSeries["ShowOpenClose"] = "Both";
            candleSeries["PriceUpColor"] = "Green";
            candleSeries["PriceDownColor"] = "Red";

            foreach (var c in data)
            {
                int pointIndex = candleSeries.Points.AddXY(c.date, c.high);
                var pt = candleSeries.Points[pointIndex];
                pt.YValues = new double[] { (double)c.high, (double)c.low, (double)c.open, (double)c.close };
            }

            chart1.Series.Add(candleSeries);

            // ----- Volume Series -----
            Series volSeries = new Series("Volume");
            volSeries.ChartType = SeriesChartType.Column;
            volSeries.YAxisType = AxisType.Secondary;
            volSeries.XValueType = ChartValueType.Date;

            foreach (var c in data)
            {
                volSeries.Points.AddXY(c.date, (double)c.volume);
            }

            chart1.Series.Add(volSeries);

            // Configure Y2 axis for volume
            area.AxisY2.Enabled = AxisEnabled.True;
            area.AxisY2.MajorGrid.Enabled = false;
            area.AxisY2.Title = "Volume";
        }
    }
}
