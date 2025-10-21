namespace WindowsFormsApp1
{
    partial class Form_Basic
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
            
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.button_fireOpenFileDialog = new System.Windows.Forms.Button();
            this.openFileDialog_fileSelector = new System.Windows.Forms.OpenFileDialog();
            this.dateTimePicker_Start = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_End = new System.Windows.Forms.DateTimePicker();
            this.label_StartDate = new System.Windows.Forms.Label();
            this.label_EndDate = new System.Windows.Forms.Label();
            this.button_Update = new System.Windows.Forms.Button();
            this.comboBox_Period = new System.Windows.Forms.ComboBox();
            this.label_Period = new System.Windows.Forms.Label();
            this.dataGridView_Candles = new System.Windows.Forms.DataGridView();
            this.chart_Candles = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label_Status = new System.Windows.Forms.Label();
            this.label_StockSymbol = new System.Windows.Forms.Label();
            this.aCandlestickBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.aCandlestickBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.aCandlestickBindingSource2 = new System.Windows.Forms.BindingSource(this.components);
            this.programBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Candles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Candles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aCandlestickBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aCandlestickBindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aCandlestickBindingSource2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.programBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // button_fireOpenFileDialog
            // 
            this.button_fireOpenFileDialog.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_fireOpenFileDialog.Location = new System.Drawing.Point(18, 18);
            this.button_fireOpenFileDialog.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_fireOpenFileDialog.Name = "button_fireOpenFileDialog";
            this.button_fireOpenFileDialog.Size = new System.Drawing.Size(225, 54);
            this.button_fireOpenFileDialog.TabIndex = 0;
            this.button_fireOpenFileDialog.Text = "Load Stock Data";
            this.button_fireOpenFileDialog.UseVisualStyleBackColor = true;
            this.button_fireOpenFileDialog.Click += new System.EventHandler(this.button_fireOpenFileDialog_Click);
            // 
            // openFileDialog_fileSelector
            // 
            this.openFileDialog_fileSelector.Filter = "CSV Files|*.csv|All Files|*.*";
            this.openFileDialog_fileSelector.InitialDirectory = "Stock Data";
            this.openFileDialog_fileSelector.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_fileSelector_FileOk);
            // 
            // dateTimePicker_Start
            // 
            this.dateTimePicker_Start.Location = new System.Drawing.Point(122, 82);
            this.dateTimePicker_Start.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dateTimePicker_Start.Name = "dateTimePicker_Start";
            this.dateTimePicker_Start.Size = new System.Drawing.Size(373, 26);
            this.dateTimePicker_Start.TabIndex = 1;
            this.dateTimePicker_Start.Value = new System.DateTime(2020, 1, 1, 0, 0, 0, 0);
            // 
            // dateTimePicker_End
            // 
            this.dateTimePicker_End.Location = new System.Drawing.Point(122, 115);
            this.dateTimePicker_End.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dateTimePicker_End.Name = "dateTimePicker_End";
            this.dateTimePicker_End.Size = new System.Drawing.Size(373, 26);
            this.dateTimePicker_End.TabIndex = 2;
            // 
            // label_StartDate
            // 
            this.label_StartDate.AutoSize = true;
            this.label_StartDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_StartDate.Location = new System.Drawing.Point(18, 85);
            this.label_StartDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_StartDate.Name = "label_StartDate";
            this.label_StartDate.Size = new System.Drawing.Size(96, 22);
            this.label_StartDate.TabIndex = 3;
            this.label_StartDate.Text = "Start Date:";
            // 
            // label_EndDate
            // 
            this.label_EndDate.AutoSize = true;
            this.label_EndDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_EndDate.Location = new System.Drawing.Point(24, 115);
            this.label_EndDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_EndDate.Name = "label_EndDate";
            this.label_EndDate.Size = new System.Drawing.Size(90, 22);
            this.label_EndDate.TabIndex = 4;
            this.label_EndDate.Text = "End Date:";
            // 
            // button_Update
            // 
            this.button_Update.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Update.Location = new System.Drawing.Point(519, 82);
            this.button_Update.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_Update.Name = "button_Update";
            this.button_Update.Size = new System.Drawing.Size(180, 46);
            this.button_Update.TabIndex = 5;
            this.button_Update.Text = "Update Chart";
            this.button_Update.UseVisualStyleBackColor = true;
            this.button_Update.Click += new System.EventHandler(this.button_Update_Click);
            // 
            // comboBox_Period
            // 
            this.comboBox_Period.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Period.FormattingEnabled = true;
            this.comboBox_Period.Location = new System.Drawing.Point(473, 32);
            this.comboBox_Period.Name = "comboBox_Period";
            this.comboBox_Period.Size = new System.Drawing.Size(160, 28);
            this.comboBox_Period.TabIndex = 10;
            this.comboBox_Period.SelectedIndexChanged += new System.EventHandler(this.comboBox_Period_SelectedIndexChanged);
            // 
            // label_Period
            // 
            this.label_Period.AutoSize = true;
            this.label_Period.Location = new System.Drawing.Point(409, 35);
            this.label_Period.Name = "label_Period";
            this.label_Period.Size = new System.Drawing.Size(58, 20);
            this.label_Period.TabIndex = 11;
            this.label_Period.Text = "Period:";
            // 
            // dataGridView_Candles
            // 
            this.dataGridView_Candles.AllowUserToAddRows = false;
            this.dataGridView_Candles.AllowUserToDeleteRows = false;
            this.dataGridView_Candles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView_Candles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Candles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_Candles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_Candles.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView_Candles.Location = new System.Drawing.Point(7, 163);
            this.dataGridView_Candles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataGridView_Candles.Name = "dataGridView_Candles";
            this.dataGridView_Candles.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Candles.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView_Candles.RowHeadersWidth = 62;
            this.dataGridView_Candles.Size = new System.Drawing.Size(712, 1110);
            this.dataGridView_Candles.TabIndex = 6;
            // 
            // chart_Candles
            // 
            this.chart_Candles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea_OHLC";
            chartArea2.AlignWithChartArea = "ChartArea_OHLC";
            chartArea2.Name = "ChartArea_Volume";
            this.chart_Candles.ChartAreas.Add(chartArea1);
            this.chart_Candles.ChartAreas.Add(chartArea2);
            legend1.Name = "Legend1";
            this.chart_Candles.Legends.Add(legend1);
            this.chart_Candles.Location = new System.Drawing.Point(727, 11);
            this.chart_Candles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chart_Candles.Name = "chart_Candles";
            series1.ChartArea = "ChartArea_OHLC";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick;
            series1.CustomProperties = "PriceDownColor=Red, PriceUpColor=Lime";
            series1.Legend = "Legend1";
            series1.Name = "Series_OHLC";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
            series1.YValuesPerPoint = 4;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series2.ChartArea = "ChartArea_Volume";
            series2.Legend = "Legend1";
            series2.Name = "Series_Volume";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
            this.chart_Candles.Series.Add(series1);
            this.chart_Candles.Series.Add(series2);
            this.chart_Candles.Size = new System.Drawing.Size(1279, 1262);
            this.chart_Candles.TabIndex = 7;
            this.chart_Candles.Text = "chartStocks";
            // 
            // label_Status
            // 
            this.label_Status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_Status.AutoSize = true;
            this.label_Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Status.Location = new System.Drawing.Point(18, 1377);
            this.label_Status.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Status.Name = "label_Status";
            this.label_Status.Size = new System.Drawing.Size(62, 22);
            this.label_Status.TabIndex = 8;
            this.label_Status.Text = "Ready";
            // 
            // label_StockSymbol
            // 
            this.label_StockSymbol.AutoSize = true;
            this.label_StockSymbol.Location = new System.Drawing.Point(261, 35);
            this.label_StockSymbol.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_StockSymbol.Name = "label_StockSymbol";
            this.label_StockSymbol.Size = new System.Drawing.Size(54, 20);
            this.label_StockSymbol.TabIndex = 9;
            this.label_StockSymbol.Text = "Stock:";
            // 
            // aCandlestickBindingSource
            // 
            this.aCandlestickBindingSource.DataSource = typeof(WindowsFormsApp1.aCandlestick);
            // 
            // aCandlestickBindingSource1
            // 
            this.aCandlestickBindingSource1.DataSource = typeof(WindowsFormsApp1.aCandlestick);
            // 
            // aCandlestickBindingSource2
            // 
            this.aCandlestickBindingSource2.DataSource = typeof(WindowsFormsApp1.aCandlestick);
            // 
            // programBindingSource
            // 
            this.programBindingSource.DataSource = typeof(StockAnalysisProject1.Program);
            // 
            // Form_Basic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2009, 1277);
            this.Controls.Add(this.label_StockSymbol);
            this.Controls.Add(this.label_Status);
            this.Controls.Add(this.chart_Candles);
            this.Controls.Add(this.dataGridView_Candles);
            this.Controls.Add(this.button_Update);
            this.Controls.Add(this.label_EndDate);
            this.Controls.Add(this.label_StartDate);
            this.Controls.Add(this.dateTimePicker_End);
            this.Controls.Add(this.dateTimePicker_Start);
            this.Controls.Add(this.comboBox_Period);
            this.Controls.Add(this.label_Period);
            this.Controls.Add(this.button_fireOpenFileDialog);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(1339, 893);
            this.Name = "Form_Basic";
            this.Text = "Stock Analysis Application";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Candles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Candles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aCandlestickBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aCandlestickBindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aCandlestickBindingSource2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.programBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_fireOpenFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog_fileSelector;
        private System.Windows.Forms.DateTimePicker dateTimePicker_Start;
        private System.Windows.Forms.DateTimePicker dateTimePicker_End;
        private System.Windows.Forms.Label label_StartDate;
        private System.Windows.Forms.Label label_EndDate;
        private System.Windows.Forms.Button button_Update;
        private System.Windows.Forms.ComboBox comboBox_Period;
        private System.Windows.Forms.Label label_Period;
        private System.Windows.Forms.DataGridView dataGridView_Candles;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_Candles;
        private System.Windows.Forms.Label label_Status;
        private System.Windows.Forms.BindingSource aCandlestickBindingSource;
        private System.Windows.Forms.BindingSource aCandlestickBindingSource2;
        private System.Windows.Forms.BindingSource aCandlestickBindingSource1;
        private System.Windows.Forms.BindingSource programBindingSource;
        private System.Windows.Forms.Label label_StockSymbol;
    }
}