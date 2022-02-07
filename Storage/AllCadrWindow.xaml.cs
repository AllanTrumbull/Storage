using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;


namespace Storage
{
    public partial class AllCadrWindow
    {
        List<AllCadrListView> items = new List<AllCadrListView>();
        private string conn;
        private int count = 0;
        string path = "log.txt";

        public AllCadrWindow()
        {
            InitializeComponent();
            LoadData();

        }

        private void LoadData()
        {
            var sqlCon = new Connect();
            conn = sqlCon.conn;
            using (var myConnection = new MySqlConnection(conn))
            {
                myConnection.Open();

                //Добавляем в таблицу
                string sqlMoving = "SELECT name,type,room,sost, count(*) FROM storagebase WHERE room=311 OR room=337/1 GROUP BY name,type,room,sost";

                // Создание объекта, выполняющего запрос к БД:
                MySqlCommand command = new MySqlCommand(sqlMoving, myConnection);

                // Получение объекта для чтения данных из БД, содержащих несколько строк и столбцов
                MySqlDataReader reader = command.ExecuteReader();

                //Строка таблицы
                while (reader.Read())
                {
                    var item = new AllCadrListView
                    {
                        storageName = reader[0].ToString(),
                        storageType = reader[1].ToString(),
                        storageRoom = reader[2].ToString(),
                        storageSost = reader[3].ToString(),
                        storageCount = reader[4].ToString(),
                    };
                    items.Add(item);

                }
                reader.Close();
                myConnection.Close();
            }
            AllCadrListView.ItemsSource = items;

            var view = (CollectionView)CollectionViewSource.GetDefaultView(AllCadrListView.ItemsSource);
            view.Filter = FindViewFilter;
        }
        private bool FindViewFilter(object item)
        {
            if (string.IsNullOrEmpty(findFirstBox.Text))
                return true;
            return ((AllCadrListView)item).storageName.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        private void buttonFind_Click(object sender, RoutedEventArgs e)
        {
            //Log("Combo " + comboBox.SelectedIndex);
            CollectionViewSource.GetDefaultView(AllCadrListView.ItemsSource).Refresh();
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            buttonFind_Click(null, null);
        }

        private void buttonExcel_Click(object sender, RoutedEventArgs e)
        {
            buttonExcel.IsEnabled = false;
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            worker.RunWorkerAsync();
            
        }

        //Экспорт в Excel
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string filePath = null;
            Microsoft.Office.Interop.Excel.Application exApp = new Microsoft.Office.Interop.Excel.Application
            {
                Visible = false
            };
            exApp.Workbooks.Add();
            var workSheet = (Worksheet)exApp.ActiveSheet;
            workSheet.Cells[1, 1] = "Наименование";
            workSheet.Cells[1, 2] = "Тип";
            workSheet.Cells[1, 3] = "Помещение";
            workSheet.Cells[1, 4] = "Количество";
            workSheet.Cells[1, 5] = "Состояние";
            var rowExcel = 2;

            foreach (AllCadrListView item in items)
            {
                workSheet.Cells[rowExcel, "A"] = item.storageName;
                workSheet.Cells[rowExcel, "B"] = item.storageType;
                workSheet.Cells[rowExcel, "C"] = item.storageRoom;
                workSheet.Cells[rowExcel, "D"] = item.storageCount;
                workSheet.Cells[rowExcel, "E"] = item.storageSost;
                var percentage = ((rowExcel - 2) + 1) * 100 / items.Count;
                (sender as BackgroundWorker)?.ReportProgress(percentage);

                ++rowExcel;
            }

            /*
                for (int i = 0; i < 3; i++)
            {
                workSheet.Cells[rowExcel, "A"] = items."БПР";
                workSheet.Cells[rowExcel, "B"] = "ЗХЗ";
                workSheet.Cells[rowExcel, "C"] = "ффывфвыфывф";
                ++rowExcel;
            }
                */
            
            var saveFileDialog = new SaveFileDialog { Filter = "Excel Workbook|*.xlsx", FileName = "ЗИП123.xlsx" };

            if (saveFileDialog.ShowDialog() == true)
            {
                filePath = saveFileDialog.FileName;
            }
            filePath = "ЗИП123.xlsx";
            workSheet.SaveAs(filePath);
            exApp.Quit();
            buttonExcel.IsEnabled = true;
        }
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

    }
}
