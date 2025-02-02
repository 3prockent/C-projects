﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LabExcel
{
    public partial class Form1 : Form
    {
        Table table = new Table();
        public Form1()
        {
            InitializeComponent();
            InitializeDataGridView(10, 35);
        }

        private void InitializeDataGridView(int rows, int columns)
        {
            dataGridView1.ColumnHeadersVisible = true;
            dataGridView1.RowHeadersVisible = true;
            dataGridView1.ColumnCount = columns;
            for (int i = 0; i < columns; i++)
            {
                string columnName = _26BasedSystem.To26System(i);
                dataGridView1.Columns[i].Name = columnName;
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            for (int i = 0; i < rows; i++)
            {
                dataGridView1.Rows.Add("");
                dataGridView1.Rows[i].HeaderCell.Value = i.ToString();
            }

            dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);

            table.setTable(columns, rows);
        }


        private void calculateButton_Click(object sender, EventArgs e)
        {
            int col = dataGridView1.SelectedCells[0].ColumnIndex;
            int row = dataGridView1.SelectedCells[0].RowIndex;
            string expression = textBox1.Text;
            if (expression == "") return;
            table.changeCellWithAllPointers(row, col, expression, dataGridView1);
            dataGridView1[col, row].Value = Table.grid[row][col].value;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            int col = dataGridView1.SelectedCells[0].ColumnIndex;
            int row = dataGridView1.SelectedCells[0].RowIndex;
            string expression = "";
            try
            {
                expression = Table.grid[row][col].expression;
            }
            catch 
            {
                MessageBox.Show("Selected incorrect cell");    
            }
            textBox1.Text = expression;
            textBox1.Focus();
        }

        private void addRowButton_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = new System.Windows.Forms.DataGridViewRow();
            if (dataGridView1.Columns.Count == 0)
            {
                MessageBox.Show("There are no colums");  //???
                return;
            }
            dataGridView1.Rows.Add(row);
            dataGridView1.Rows[table.rowCount].HeaderCell.Value = table.rowCount.ToString();
            table.AddRow(dataGridView1);

        }
        private void addColButton_Click(object sender, EventArgs e)
        {
            string name = _26BasedSystem.To26System(table.colCount);
            dataGridView1.Columns.Add(name, name);
            table.AddCol(dataGridView1);
        }

        private void delRowButton_Click(object sender, EventArgs e)
        {
            if (!table.DeleteRow(dataGridView1))
                return;
            dataGridView1.Rows.RemoveAt(table.rowCount);
        }

        private void delColButton_Click(object sender, EventArgs e)
        {
            if (!table.DeleteColumn(dataGridView1))
                return;
            dataGridView1.Columns.RemoveAt(table.colCount);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "TableFile|*.txt";
            saveFileDialog1.Title = "Save table file";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                FileStream fs = (FileStream)saveFileDialog1.OpenFile();
                StreamWriter sw = new StreamWriter(fs);
                table.Save(sw);
                sw.Close();
                fs.Close();
            }
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "TableFile|*.txt";
            openFileDialog1.Title = "Open Table File";
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            StreamReader sr = new StreamReader(openFileDialog1.FileName);
            table.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            int row, column;
            Int32.TryParse(sr.ReadLine(), out row);
            Int32.TryParse(sr.ReadLine(), out column);
            InitializeDataGridView(row, column);
            table.Open(row, column, sr, dataGridView1);
            sr.Close();
        }
    }
}
