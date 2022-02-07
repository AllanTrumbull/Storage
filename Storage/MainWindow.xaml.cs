using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Storage
{
    public partial class MainWindow
    {
        List<StorageList> items = new List<StorageList>();
        string path = "log.txt";
        int idNull;
        readonly string conn;


        public MainWindow()
        {
            var sqlCon = new Connect();
            conn = sqlCon.conn;
            InitializeComponent();
            var statusServer = true;
            while (statusServer)
            {
                using (var myConnection = new MySqlConnection(conn))
                {
                    try
                    {
                        //Устанавливаем соединение с БД
                        statusServer = false;
                    }
                    catch (MySqlException e)
                    {

                        MessageBox.Show("Ошибка сетевого соединения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        myConnection.Close();
                    }
                }
            }
            LoadDate();

        }

        public void UpdateList()
        {
            items.Clear();
            LoadDate();
        }

        private void LoadDate()
        {

            // строка подключения к БД
            using (var myConnection = new MySqlConnection(conn))
            {
                try
                {
                    //Устанавливаем соединение с БД
                    myConnection.Open();
                }
                catch (MySqlException e)
                {
                    var result = MessageBox.Show(e.ToString(), "Ошибка сетевого соединения");
                }
                finally
                {
                    var sql = "SELECT id,name,type,room,stellag,polka,oborydovanie,sn,datemade,sost,category FROM storagebase ORDER BY id";

                    // Создание объекта, выполняющего запрос к БД:
                    var command = new MySqlCommand(sql, myConnection);

                    // Получение объекта для чтения данных из БД, содержащих несколько строк и столбцов
                    var reader = command.ExecuteReader();

                    //Строка таблицы
                    while (reader.Read())
                    {
                        //id.Add(new idList() { storageID = reader[0].ToString() });
                        items.Add(new StorageList()
                        {
                            storageID = reader[0].ToString(),
                            storageName = reader[1].ToString(),
                            storageType = reader[2].ToString(),
                            storageRoom = reader[3].ToString(),
                            storageStellag = reader[4].ToString(),
                            storagePolka = reader[5].ToString(),
                            storageOborydovanie = reader[6].ToString(),
                            storageSn = reader[7].ToString(),
                            storageDatemade = reader[8].ToString(),
                            storageSost = reader[9].ToString(),
                            storageСategory = reader[10].ToString()
                        });
                    }

                    reader.Close();
                    myConnection.Close();
                }
            }

            //задаем содержимое
            storageListView.ItemsSource = items;


            //для поиска в realtime
            var view = (CollectionView)CollectionViewSource.GetDefaultView(storageListView.ItemsSource);
            //view.SortDescriptions.Add(new SortDescription("Наименование", ListSortDirection.Ascending));
            view.Filter = FindViewFilter;
        }

        //Метод для поиска
        private bool FindViewFilter(object item)
        {
            if (string.IsNullOrEmpty(findFirstBox.Text) && string.IsNullOrEmpty(findSecondBox.Text))
                return true;
            //else
            {
                if (comboBox.SelectedIndex != -1 && comboBoxSecond.SelectedIndex == -1 || comboBoxSecond.SelectedIndex == 0)
                {
                    switch (comboBox.SelectedIndex)
                    {
                        case 0:
                            return ((StorageList)item).storageName.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                        case 1:
                            return ((StorageList)item).storageСategory.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                        case 2:
                            return ((StorageList)item).storageType.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                        case 3:
                            return ((StorageList)item).storageRoom.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                        case 4:
                            return ((StorageList)item).storageStellag.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                        case 5:
                            return ((StorageList)item).storagePolka.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                        case 6:
                            return ((StorageList)item).storageOborydovanie.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                        case 7:
                            return ((StorageList)item).storageSn.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                        case 8:
                            return ((StorageList)item).storageDatemade.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                        case 9:
                            return ((StorageList)item).storageSost.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                        default:
                            return ((StorageList)item).storageName.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                    }

                }
                //else
                {
                    switch (comboBox.SelectedIndex)
                    {
                        case 0:
                            switch (comboBoxSecond.SelectedIndex)
                            {
                                case 1:
                                    return ((StorageList)item).storageName.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                        ((StorageList)item).storageСategory.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                                case 2:
                                    return ((StorageList)item).storageName.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                        ((StorageList)item).storageType.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                                case 3:
                                    return ((StorageList)item).storageName.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                        ((StorageList)item).storageRoom.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                                case 4:
                                    return ((StorageList)item).storageName.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                        ((StorageList)item).storageStellag.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                                case 5:
                                    return ((StorageList)item).storageName.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                        ((StorageList)item).storagePolka.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                                case 6:
                                    return ((StorageList)item).storageName.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                        ((StorageList)item).storageOborydovanie.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                                case 7:
                                    return ((StorageList)item).storageName.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                        ((StorageList)item).storageSn.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                                case 8:
                                    return ((StorageList)item).storageName.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                        ((StorageList)item).storageDatemade.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                                case 9:
                                    return ((StorageList)item).storageName.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                        ((StorageList)item).storageSost.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                            }
                            break;
                        case 3:
                            switch (comboBoxSecond.SelectedIndex)
                            {
                                case 1:
                                    return ((StorageList)item).storageRoom.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                           ((StorageList)item).storageСategory.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                                case 2:
                                    return ((StorageList)item).storageRoom.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                           ((StorageList)item).storageType.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                                case 4:
                                    return ((StorageList)item).storageRoom.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                        ((StorageList)item).storageStellag.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                                case 5:
                                    return ((StorageList)item).storageRoom.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                           ((StorageList)item).storagePolka.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                                case 6:
                                    return ((StorageList)item).storageRoom.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                           ((StorageList)item).storageOborydovanie.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                                case 7:
                                    return ((StorageList)item).storageRoom.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                           ((StorageList)item).storageSn.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                                case 8:
                                    return ((StorageList)item).storageRoom.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                           ((StorageList)item).storageDatemade.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                                case 9:
                                    return ((StorageList)item).storageRoom.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                           ((StorageList)item).storageSost.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                            }
                            break;
                        case 4:
                            switch (comboBoxSecond.SelectedIndex)
                            {
                                case 5:
                                    return ((StorageList)item).storageStellag.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                           ((StorageList)item).storagePolka.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                            }
                            break;

                    }
                    return true;
                }
            }
        }

        //обратка нажатия ENTER
        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Log("Combo " + comboBox.SelectedIndex);
                Log("ComboSecond " + comboBoxSecond.SelectedIndex);
                CollectionViewSource.GetDefaultView(storageListView.ItemsSource).Refresh();
            }
        }

        //Обработчик нажатия на строку в таблице
        private void ChangeMouse(object sender, MouseButtonEventArgs e)
        {
            //проверяем, если нажали не на пустое место
            if (storageListView.SelectedItem != null && !string.IsNullOrEmpty(storageListView.SelectedItem.ToString()))
            {

                //передаем данные в класс IDClass
                var item = storageListView.SelectedItem as StorageList;
                if (item != null)
                {
                    IDClass.id = item.storageID;
                    IDClass.name = item.storageName;
                    IDClass.type = item.storageType;
                    IDClass.room = item.storageRoom;
                    IDClass.stellag = item.storageStellag;
                    IDClass.polka = item.storagePolka;
                    IDClass.oborydovanie = item.storageOborydovanie;
                    IDClass.sn = item.storageSn;
                    IDClass.datemade = item.storageDatemade;
                    IDClass.sost = item.storageSost;
                }

                Log("MainWindow id= " + IDClass.id);
                var infoEquipWindow = new infoEquipWindow();
                infoEquipWindow.Owner = this;
                infoEquipWindow.Show();
            }
        }

        //Обработка нажатия на кнопку "Обновить"
        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            UpdateList();
        }

        //Ведение логов
        private void Log(string eventName)
        {
            using (var logger = new StreamWriter(path, true))
            {
                logger.WriteLine(DateTime.Now.ToLongTimeString() + " - " + eventName);
            }
        }

        //Обработка нажатия на кнопку "Добавить"
        private void button_openWindowAdd(object sender, RoutedEventArgs e)
        {
            var add = new AddWindow();
            add.Owner = this;
            add.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            add.Show();
        }

        //Обработка нажатия на кнопку "Поиск"
        private void buttonFind_Click(object sender, RoutedEventArgs e)
        {
            //Log("Combo " + comboBox.SelectedIndex);
            Log("ComboSecond " + comboBoxSecond.SelectedIndex);
            CollectionViewSource.GetDefaultView(storageListView.ItemsSource).Refresh();
        }

        //Обработка нажатия на кнопку "Общий Кадр"
        private void buttonAllCadr_Click(object sender, RoutedEventArgs e)
        {
            var allCadr = new AllCadrWindow();
            allCadr.Show();
        }

        //Сканер штрих-кода
        internal void ButtonScanner_Click(object sender, RoutedEventArgs e)
        {

            var scannerWindow = new ScannerWindow();

            if (scannerWindow.ShowDialog() == true)
            {
                Log("Сканер сработал!");
                Log("scannerWindow.SN= " + scannerWindow.SN);
                if (scannerWindow.SN.Equals("") != true)
                {

                    // строка подключения к БД
                    using (var myConnection = new MySqlConnection(conn))
                    {
                        myConnection.Open();
                        Log("");
                        Log("Окно: MainWindow");
                        Log("Метод: ButtonScanner_Click");

                        var sql =
                            "SELECT id,name,type,room,stellag,polka,oborydovanie,sn,datemade,sost FROM storagebase WHERE sn='" +
                            scannerWindow.SN + "'";

                        Log("SQL запрос: " + sql);
                        // Создание объекта, выполняющего запрос к БД:
                        var command = new MySqlCommand(sql, myConnection);

                        // Получение объекта для чтения данных из БД, содержащих несколько строк и столбцов
                        var reader = command.ExecuteReader();

                        while (reader != null && reader.Read())
                        {
                            IDClass.id = reader[0].ToString();
                            IDClass.name = reader[1].ToString();
                            IDClass.type = reader[2].ToString();
                            IDClass.room = reader[3].ToString();
                            IDClass.stellag = reader[4].ToString();
                            IDClass.polka = reader[5].ToString();
                            IDClass.oborydovanie = reader[6].ToString();
                            IDClass.sn = reader[7].ToString();
                            IDClass.datemade = reader[8].ToString();
                            IDClass.sost = reader[9].ToString();

                        }

                        if (reader != null && reader.HasRows == false)
                        {
                            idNull = 0;
                        }
                        else
                        {
                            idNull = 1;
                        }
                        reader?.Close();
                        myConnection.Close();
                    }
                }

                Log("IDClass.id= " + IDClass.id);
                if (idNull != 0)
                {
                    Log("MainWindow id= " + IDClass.id);
                    var infoEquipScannerWindow = new infoEquipScannerWindow { Owner = this };
                    infoEquipScannerWindow.Show();

                }
                else
                {
                    MessageBox.Show("Серийный номер не найден");
                    ButtonScanner_Click(null, null);
                }
            }



        }

        //Вызов окна учета
        private void ButtonYchet_Click(object sender, RoutedEventArgs e)
        {
            var ychetCadr = new YchetWindow();
            ychetCadr.ShowDialog();
        }
    }
}

