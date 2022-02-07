using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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
using Storage.listView;

namespace Storage
{
    /// <summary>
    /// Логика взаимодействия для AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        string conn;
        string conn_dbdef;
        string path = "log.txt";
        private int extractId;
        private string isp = "USER";
        List<Room> rooms = new List<Room>();
        List<Rack> Racks = new List<Rack>();

        public AddWindow()
        {
            var sqlConn = new Connect();
            conn = sqlConn.conn;
            conn_dbdef = sqlConn.conn_debdef;
            InitializeComponent();
            ComboBoxAddRoom();
            ComboBoxAddStellag();
            ComboBoxRoom.DisplayMemberPath = "Name";
            ComboBoxStellag.DisplayMemberPath = "Name";

        }
        private void ComboBoxAddRoom()
        {
            // строка подключения к БД

            using (var myConnection = new MySqlConnection(conn_dbdef))
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
                    Log("Окно: infoEquipWindow");
                    Log("Метод: ComboBoxAddRoom");
                    var sql = "SELECT DISTINCT `room` FROM `pom`";

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

            }
            ComboBoxRoom.ItemsSource = rooms;

        }
        private void ComboBoxAddStellag()
        {
            using (var myConnection = new MySqlConnection(conn_dbdef))
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
                    Log("Окно: infoEquipWindow");
                    Log("Метод: ComboBoxAddStellag");
                    var sql = "SELECT room, oborud FROM `pom`";

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


        private void button_Click(object sender, RoutedEventArgs e)
        {
            int betta = 0;
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
                    Log("Окно: AddWindow");
                    Log("Метод: button_Click");
                    //Добавляем в таблицу 
                    string sqlYchet = "SELECT `sn` FROM `storagebase` WHERE `sn`='" + textBoxSN.Text + "'";

                    Log("SQL запрос: " + sqlYchet);
                    // Создание объекта, выполняющего запрос к БД:
                    MySqlCommand commandN = new MySqlCommand(sqlYchet, myConnection);

                    // Получение объекта для чтения данных из БД, содержащих несколько строк и столбцов
                    MySqlDataReader reader = commandN.ExecuteReader();

                    //Строка таблицы
                    while (reader.Read())
                    {
                        betta++;
                    }
                    reader.Close();
                    myConnection.Close();

                }

            }
            
            if (betta > 0)
            {
                MessageBox.Show("Позиция с таким S/N уже существует!","Внимание!",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
            else
            {

                using (var connection = new MySqlConnection(conn))
                {
                    //Устанавливаем соединение с БД
                    try
                    {
                        connection.Open();
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
                        Log("Окно: AddWindow");
                        Log("Метод: button_Click");

                        var query = "INSERT INTO storagebase (name, type, room, stellag, polka, oborydovanie, sn, datemade, sost, category, ychet) VALUES ('" +
                textBoxName.Text + "', '" + textBoxType.Text + "', '" + ComboBoxRoom.Text + "', '" + ComboBoxStellag.Text + "', '" +
                ComboBoxKreit.Text + "', '" + ComboBoxOborydovanie.Text + "', '" + textBoxSN.Text + "', '" + comboBoxDataMade.Text + "', '" +
                comboBoxSost.Text + "', '" + comboBoxCategory.Text + "', ''); ";
                        Log("SQL запрос: " + query);

                        // объект для выполнения SQL-запроса
                        var command = new MySqlCommand(query, connection);
                        // выполняем запрос
                        command.ExecuteNonQuery();
                        // закрываем подключение к БД
                        connection.Close();
                    }
                }
                ExcractId();
                //AddMoving();
                //addMoving();
                (this.Owner as MainWindow)?.UpdateList();
                ((MainWindow)Owner).Topmost = true;
                this.Close();
            }
        }
        private void Log(string eventName)
        {
            using (var logger = new StreamWriter(path, true))
            {
                logger.WriteLine(DateTime.Now.ToLongTimeString() + " - " + eventName);
            }
        }

        private void ExcractId()
        {
            // объект для установления соединения с БД
            var connection = new MySqlConnection(conn);
            // открываем соединение
            connection.Open();
            // запрос обновления данных
            Log("");
            Log("Окно: AddWindow");
            Log("Метод: ExcractId");
            var sqlExcractID = "SELECT * FROM storagebase ORDER BY id DESC LIMIT 1";

            Log("SQL запрос: " + sqlExcractID);
            // Создание объекта, выполняющего запрос к БД:
            var command = new MySqlCommand(sqlExcractID, connection);

            // Получение объекта для чтения данных из БД, содержащих несколько строк и столбцов
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                extractId = int.Parse(reader[0].ToString());
            }
            Log("extractId=" + extractId);
            Log("");
            // закрываем подключение к БД
            connection.Close();
        }



        private void AddMoving()
        {
            // объект для установления соединения с БД
            var connection = new MySqlConnection(conn);
            // открываем соединение
            connection.Open();
            // запрос обновления данных
            Log("");
            Log("Окно: AddWindow");
            Log("Метод: AddMoving");
            var query = "INSERT INTO peremechenie (name, date, moving) VALUES ('" + extractId + "', '" +
                        DateTime.Now.ToString("yyyy-MM-dd")
                        + "', '" + "ИЗ пом.: " + "-" + " шк./стел.: " + "-" + " пол./кр.: " + "-" +
                        " сост.: " + "-" + " В пом.: " + ComboBoxRoom.Text + " шк./стел.: " +
                        ComboBoxStellag.Text + " пол./кр.: " + ComboBoxKreit.Text + " сост.: " + comboBoxSost.Text + " исп.: " + isp + "')";
            Log("SQL запрос: " + query);

            // объект для выполнения SQL-запроса
            var command = new MySqlCommand(query, connection);
            // выполняем запрос
            command.ExecuteNonQuery();
            // закрываем подключение к БД
            connection.Close();
            Log("");
        }

        //"Живой" поиск в comboBox
        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ComboBoxStellag.ItemsSource = Racks.Where(x => x.Room == (ComboBoxRoom.SelectedValue as Room)?.Name).ToList();

        }


        private void ComboBox_TextChangedStellag(object sender, TextChangedEventArgs e)
        {

        }
        /* private void ComboBox_TextChangedKreit(object sender, TextChangedEventArgs e)
         {
             var tb = (TextBox)e.OriginalSource;
             if (tb.SelectionStart != 0)
             {
                 ComboBoxKreit.SelectedItem = null; // Если набирается текст сбросить выбраный элемент
             }
             if (tb.SelectionStart == 0 && ComboBoxKreit.SelectedItem == null)
             {
                 ComboBoxKreit.IsDropDownOpen = false; // Если сбросили текст и элемент не выбран, сбросить фокус выпадающего списка
             }

             ComboBoxKreit.IsDropDownOpen = true;
             if (ComboBoxKreit.SelectedItem == null)
             {
                 // Если элемент не выбран менять фильтр
                 CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(ComboBoxKreit.ItemsSource);
                 cv.Filter = s => ((string)s).IndexOf(ComboBoxKreit.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
             }
         }*/
    }
}
