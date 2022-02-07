using MySql.Data.MySqlClient;
using Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Storage
{
    /// <summary>
    /// Логика взаимодействия для YchetListWindow.xaml
    /// </summary>
    public partial class YchetWindow : Window
    {

        ObservableCollection<YchetListViewClass> items = new ObservableCollection<YchetListViewClass>();
        List<Room> rooms = new List<Room>();
        List<Rack> Racks = new List<Rack>();
        List<Polka> Polkas = new List<Polka>();
        string[] infoEquipment = new string[11];
        private string conn;
        string path = "log.txt";
        public YchetWindow()
        {
            InitializeComponent();
            addTable();
            roomEquipmentBoxAdd();
            stellagEquipmentBoxAdd();
            polkaEquipmentBoxAdd();
            Closed += new EventHandler(MainWindow_Closed);
            roomEquipmentBox.DisplayMemberPath = "Name";
            stellagEquipmentBox.DisplayMemberPath = "Name";
            polkaEquipmentBox.DisplayMemberPath = "Name";
        }
        public class Room
        {
            public string Name { get; set; }
        }
        //Шкафы
        public class Rack
        {
            public string Room { get; set; }
            public string Name { get; set; }
        }
        public class Polka
        {
            public string Room { get; set; }
            public string Stellag { get; set; }
            public string Name { get; set; }
        }

        //добавить в таблицу
        public void addTable()
        {

            var sqlCon = new Connect();
            conn = sqlCon.conn;

            MySqlConnection myConnection = new MySqlConnection(conn);
            myConnection.Open();

            //Добавляем в таблицу 
            string sqlMoving = "SELECT `id`,`name`,`sn`,`sost` FROM `storagebase` WHERE `room` LIKE '337/1' AND `stellag` LIKE '3' AND `polka` LIKE '6(22)'";

            // Создание объекта, выполняющего запрос к БД:
            MySqlCommand command = new MySqlCommand(sqlMoving, myConnection);

            // Получение объекта для чтения данных из БД, содержащих несколько строк и столбцов
            MySqlDataReader reader = command.ExecuteReader();

            //Строка таблицы
            while (reader.Read())
            {
                items.Add(new YchetListViewClass()
                {
                    storageId = reader[0].ToString(),
                    storageName = reader[1].ToString(),
                    storageSN = reader[2].ToString(),
                    storageSost = reader[3].ToString()

                });
            }
            reader.Close();
            myConnection.Close();
            ychetListView.ItemsSource = items;
        }
        //добавить в таблицу
        public void addTableExecute()
        {
            using (var myConnection = new MySqlConnection(conn))
            {
                //Устанавливаем соединение с БД
                try
                {
                    myConnection.Open();
                }
                catch (MySqlException ex)
                {
                    // Протоколировать исключение
                    Log("");
                    Log("MysqlConnection: " + ex.Message);
                }
                finally
                {
                    Log("");
                    Log("Окно: YchetWindow");
                    Log("Метод: addTableExecute");
                    //Добавляем в таблицу 
                    string sqlYchet = "SELECT `id`,`name`,`sn`,`sost` FROM `storagebase` WHERE `room` LIKE '" + roomEquipmentBox.Text + "' AND `stellag` LIKE '" +
                stellagEquipmentBox.Text + "' AND `polka` LIKE '" + polkaEquipmentBox.Text + "' AND NOT `sost`='карантин' AND NOT `ychet`='Подтвержден'";

                    Log("SQL запрос: " + sqlYchet);
                    // Создание объекта, выполняющего запрос к БД:
                    MySqlCommand command = new MySqlCommand(sqlYchet, myConnection);

                    // Получение объекта для чтения данных из БД, содержащих несколько строк и столбцов
                    MySqlDataReader reader = command.ExecuteReader();

                    //Строка таблицы
                    while (reader.Read())
                    {
                        items.Add(new YchetListViewClass()
                        {
                            storageId = reader[0].ToString(),
                            storageName = reader[1].ToString(),
                            storageSN = reader[2].ToString(),
                            storageSost = reader[3].ToString()

                        });
                    }
                    reader.Close();
                    myConnection.Close();
                }
                _ = Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    ychetListView.ItemsSource = items;
                    //observableCollection.Add(instanceOfYourClass);
                });

            }
        }
        public void UpdateList()
        {
            items.Clear();
            addTableExecute();
            //для поиска в realtime
            var view = (CollectionView)CollectionViewSource.GetDefaultView(ychetListView.ItemsSource);
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
                            return ((YchetListViewClass)item).storageName.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                        case 1:
                            return ((YchetListViewClass)item).storageSN.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                        case 2:
                            return ((YchetListViewClass)item).storageSost.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                        default:
                            return ((YchetListViewClass)item).storageName.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
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
                                    return ((YchetListViewClass)item).storageName.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                        ((YchetListViewClass)item).storageSN.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                                case 2:
                                    return ((YchetListViewClass)item).storageName.IndexOf(findFirstBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                        ((YchetListViewClass)item).storageSost.IndexOf(findSecondBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                            }
                            break;
                    }
                    return true;
                }
            }
        }
        //Узнаем какие есть помещения (Кастыль ввиде двух комнат-кладовок)
        private void roomEquipmentBoxAdd()
        {

            /*
               using (var myConnection = new MySqlConnection(conn))
               {
                   //Устанавливаем соединение с БД
                   try
                   {
                       myConnection.Open();
                   }
                   catch (MySqlException ex)
                   {
                       // Протоколировать исключение
                       Log("");
                       Log("MysqlConnection: " + ex.Message);
                   }
                   finally
                   {
                       Log("");
                       Log("Окно: YchetWindow");
                       Log("Метод: roomEquipmentBoxAdd");
                       var sql = "SELECT DISTINCT `room` FROM `storagebase` WHERE `room` LIKE '337/1' OR `room` LIKE '311'";

                       Log("SQL запрос: " + sql);

                       // Создание объекта, выполняющего запрос к БД:
                       var command = new MySqlCommand(sql, myConnection);

                       // Получение объекта для чтения данных из БД, содержащих несколько строк и столбцов
                       var reader = command.ExecuteReader();

                       //Строка таблицы
                       while (reader.Read())
                       {
                           rooms.Add(new Room() { Name = reader[0].ToString() });
                       }

                       reader.Close();
                       myConnection.Close();
                   }

               }*/
            rooms.Add(new Room() { Name = "311" });
            rooms.Add(new Room() { Name = "337/1" });
            roomEquipmentBox.ItemsSource = rooms;
        }

        //Узнаем какие в помещения стеллажи. Запрос идет вместе с помещениями для поиска (Жеско завязано на 311 и 337/1).
        private void stellagEquipmentBoxAdd()
        {
            // строка подключения к БД

            using (var myConnection = new MySqlConnection(conn))
            {
                //Устанавливаем соединение с БД
                try
                {
                    myConnection.Open();
                }
                catch (MySqlException ex)
                {
                    // Протоколировать исключение
                    Log("");
                    Log("MysqlConnection: " + ex.Message);
                }
                finally
                {
                    Log("");
                    Log("Окно: YchetWindow");
                    Log("Метод: roomEquipmentBoxAdd");
                    var sql = "SELECT DISTINCT `room`,`stellag` FROM `storagebase` WHERE `room` LIKE '337/1' OR `room` LIKE '311' ORDER BY `room` ASC, `stellag`ASC";

                    Log("SQL запрос: " + sql);

                    // Создание объекта, выполняющего запрос к БД:
                    var command = new MySqlCommand(sql, myConnection);

                    // Получение объекта для чтения данных из БД, содержащих несколько строк и столбцов
                    var reader = command.ExecuteReader();

                    //Строка таблицы
                    while (reader.Read())
                    {
                        Racks.Add(new Rack() { Room = reader[0].ToString(), Name = reader[1].ToString() });
                    }
                    reader.Close();
                    myConnection.Close();
                }

            }
        }

        //Узнаем какие в помещения полки. Запрос идет вместе с помещения, стеллажами для поиска.
        private void polkaEquipmentBoxAdd()
        {
            // строка подключения к БД

            using (var myConnection = new MySqlConnection(conn))
            {
                //Устанавливаем соединение с БД
                try
                {
                    myConnection.Open();
                }
                catch (MySqlException ex)
                {
                    // Протоколировать исключение
                    Log("");
                    Log("MysqlConnection: " + ex.Message);
                }
                finally
                {
                    Log("");
                    Log("Окно: YchetWindow");
                    Log("Метод: roomEquipmentBoxAdd");
                    var sql = "SELECT DISTINCT `room`,`stellag`,`polka` FROM `storagebase` WHERE `room` LIKE '337/1' OR `room` LIKE '311' ORDER BY `polka` ASC";

                    Log("SQL запрос: " + sql);

                    // Создание объекта, выполняющего запрос к БД:
                    var command = new MySqlCommand(sql, myConnection);

                    // Получение объекта для чтения данных из БД, содержащих несколько строк и столбцов
                    var reader = command.ExecuteReader();

                    //Строка таблицы
                    while (reader.Read())
                    {
                        Polkas.Add(new Polka() { Room = reader[0].ToString(), Stellag = reader[1].ToString(), Name = reader[2].ToString() });
                    }

                    reader.Close();
                    myConnection.Close();
                }

            }
        }
        //Сортировка по стеллажам, взависимости от выбранного помещения
        private void roomEquipmentBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            stellagEquipmentBox.ItemsSource = Racks.Where(x => x.Room == (roomEquipmentBox.SelectedValue as Room)?.Name).ToList();
        }
        //Сортировка по полками, взависимости от выбранного помещения и стеллажа
        private void stellagEquipmentBox_TextChangedStellag(object sender, TextChangedEventArgs e)
        {
            polkaEquipmentBox.ItemsSource = Polkas.Where(x => x.Room == (roomEquipmentBox.SelectedValue as Room)?.Name && x.Stellag == (stellagEquipmentBox.SelectedValue as Rack)?.Name).ToList();
        }
        private void ChangeMouse(object sender, MouseButtonEventArgs e)
        {
            //проверяем, если нажали не на пустое место
            if (ychetListView.SelectedItem != null && !string.IsNullOrEmpty(ychetListView.SelectedItem.ToString()))
            {
                //передаем данные в класс IDClass
                var item = ychetListView.SelectedItem as YchetListViewClass;
                if (item != null)
                {
                    IDClass.id = item.storageId;
                }

                var infoEquipWindow = new infoEquipWindow();
                infoEquipWindow.ShowDialog();

                UpdateList();
                //this.Close();

            }
        }
        //обратка нажатия ENTER
        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Log("Combo " + comboBox.SelectedIndex);
                Log("ComboSecond " + comboBoxSecond.SelectedIndex);
                CollectionViewSource.GetDefaultView(ychetListView.ItemsSource).Refresh();
            }
        }
        //Ведение логов
        private void Log(string eventName)
        {
            using (var logger = new StreamWriter(path, true))
            {
                logger.WriteLine(DateTime.Now.ToLongTimeString() + " - " + eventName);
            }
        }
        //Кнопка "Выполнить"
        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            label3.Visibility = Visibility.Hidden;
            label4.Visibility = Visibility.Hidden;
            roomEquipmentBox.Visibility = Visibility.Hidden;
            stellagEquipmentBox.Visibility = Visibility.Hidden;
            label5.Visibility = Visibility.Hidden;
            polkaEquipmentBox.Visibility = Visibility.Hidden;
            execute.Visibility = Visibility.Hidden;
            UpdateList();
        }
        //Кнопка "Подвердить" в таблице
        private void Button_Confirm(object sender, RoutedEventArgs e)
        {
            // Button button = sender as Button;
            // YchetListViewClass ychet = button.DataContext as YchetListViewClass;
            YchetListViewClass ychet = (sender as Button)?.DataContext as YchetListViewClass;
            //MessageBox.Show(ychet.storageId);
            string message = "Подтвердить?";
            string caption = "Подтверждение";
            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;
            if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
            {
                using (var myConnection = new MySqlConnection(conn))
                {
                    //Устанавливаем соединение с БД
                    try
                    {
                        myConnection.Open();
                    }
                    catch (MySqlException ex)
                    {
                        // Протоколировать исключение
                        Log("");
                        Log("MysqlConnection: " + ex.Message);
                    }
                    finally
                    {
                        Log("");
                        Log("Окно: YchetWindow");
                        Log("Метод: Button_Confirm");
                        //Добавляем в таблицу 
                        string sqlYchet = "UPDATE `storagebase` SET `ychet`='Подтвержден' WHERE `id`='" + ychet.storageId + "'";

                        Log("SQL запрос: " + sqlYchet);
                        // Создание объекта, выполняющего запрос к БД:
                        MySqlCommand command = new MySqlCommand(sqlYchet, myConnection);

                        // выполняем запрос
                        command.ExecuteNonQuery();
                        myConnection.Close();
                    }
                }
                UpdateList();
            }
            else
            {
            }

        }
        //Обработка событий при закрытии
        void MainWindow_Closed(object sender, EventArgs e)
        {
            using (var myConnection = new MySqlConnection(conn))
            {
                //Устанавливаем соединение с БД
                try
                {
                    myConnection.Open();
                }
                catch (MySqlException ex)
                {
                    // Протоколировать исключение
                    Log("");
                    Log("MysqlConnection: " + ex.Message);
                }
                finally
                {
                    Log("");
                    Log("Окно: YchetWindow");
                    Log("Метод: MainWindow_Closed");
                    //Добавляем в таблицу 
                    string sqlYchet = "UPDATE `storagebase` SET `ychet`='' WHERE `ychet`='Подтвержден'";

                    Log("SQL запрос: " + sqlYchet);
                    // Создание объекта, выполняющего запрос к БД:
                    MySqlCommand command = new MySqlCommand(sqlYchet, myConnection);

                    // выполняем запрос
                    command.ExecuteNonQuery();
                    myConnection.Close();
                }
            }
        }
    }
    class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo culture)
        {
            ListViewItem item = (ListViewItem)value;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            int index = listView.ItemContainerGenerator.IndexFromContainer(item) + 1;
            return index.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
