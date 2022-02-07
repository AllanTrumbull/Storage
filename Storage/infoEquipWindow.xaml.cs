using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class infoEquipWindow : Window
    {
        string conn;
        string conn_dbdef;
        int itera = 0;
        string path = "log.txt";
        string[] infoEquipment = new string[11];
        private string carantin;
        //private string isp = "USER";
        List<equipmentMovingListView> items = new List<equipmentMovingListView>();

        List<Room> rooms = new List<Room>();
        List<Rack> Racks = new List<Rack>();

        public infoEquipWindow()
        {
            var sqlCon = new Connect();
            conn = sqlCon.conn;
            conn_dbdef = sqlCon.conn_debdef;
            InitializeComponent();
            loadDate();
            roomEquipmentBoxAdd();
            ComboBoxAddStellag();

            roomEquipmentBox.DisplayMemberPath = "Name";
            stellagEquipmentBox.DisplayMemberPath = "Name";
            checkCarantin();
            this.Closed += MainWindow_Closed;

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
        //Проверяется, если в карантине
        private void checkCarantin()
        {
            Log("");
            Log("Окно: infoEquipWindow");
            Log("Метод: checkCarantin");
            var myConnection = new MySqlConnection(conn);

            myConnection.Open();

            var sql = "SELECT sost FROM storagebase WHERE id=" + IDClass.id;
            Log("SQL запрос: " + sql);
            // Создание объекта, выполняющего запрос к БД:
            var command = new MySqlCommand(sql, myConnection);

            // Получение объекта для чтения данных из БД, содержащих несколько строк и столбцов
            var reader = command.ExecuteReader();


            deleteCarantinButton.Visibility = Visibility.Visible;
            while (reader.Read())
            {
                carantin = reader[0].ToString();

            }

            deleteCarantinButton.Visibility = !carantin.Equals("карантин") ? Visibility.Hidden : Visibility.Visible;
            addCarantinButton.Visibility = !carantin.Equals("карантин") ? Visibility.Visible : Visibility.Hidden;
            reader.Close();
            myConnection.Close();
            Log("");
        }

        private void loadDate()
        {
            //dateInfo.IsTodayHighlighted = true;

            var myConnection = new MySqlConnection(conn);

            //Устанавливаем соединение с БД
            myConnection.Open();
            var sql =
                "SELECT id,name,type,room,stellag,polka,oborydovanie,sn,datemade,sost,category FROM storagebase WHERE id=" +
                IDClass.id;

            // Создание объекта, выполняющего запрос к БД:
            var command = new MySqlCommand(sql, myConnection);


            // Получение объекта для чтения данных из БД, содержащих несколько строк и столбцов
            var reader = command.ExecuteReader();

            //Строка таблицы

            while (reader.Read())
            {
                infoEquipment[0] = reader[0].ToString(); //id
                infoEquipment[1] = reader[1].ToString(); //name
                infoEquipment[2] = reader[2].ToString(); //Type
                infoEquipment[3] = reader[3].ToString(); //room
                infoEquipment[4] = reader[4].ToString(); //stellag
                infoEquipment[5] = reader[5].ToString(); //polka
                infoEquipment[6] = reader[6].ToString(); //oborud
                infoEquipment[7] = reader[7].ToString(); //SN
                infoEquipment[8] = reader[8].ToString(); //data
                infoEquipment[9] = reader[9].ToString(); //sost
                infoEquipment[10] = reader[10].ToString(); //category
            }

            reader.Close();
            myConnection.Close();
            labelId.Content = infoEquipment[0];
            nameEquipmentBox.Text = infoEquipment[1];
            typeEquipmentBox.Text = infoEquipment[2];
            roomEquipmentBox.Text = infoEquipment[3];
            stellagEquipmentBox.Text = infoEquipment[4];
            polkaEquipmentBox.Text = infoEquipment[5];
            OborudEquipmentBox.Text = infoEquipment[6];
            SNEquipmentBox.Text = infoEquipment[7];
            dataEquipmentBox.Text = infoEquipment[8];
            sostEquipmentComBox.Text = infoEquipment[9];
            switch (infoEquipment[10])
            {
                case "Модули":
                    categoryComboBox.SelectedIndex = 0;
                    break;
                case "Материнские платы":
                    categoryComboBox.SelectedIndex = 1;
                    break;
                case "БП":
                    categoryComboBox.SelectedIndex = 2;
                    break;
                case "Контроллеры":
                    categoryComboBox.SelectedIndex = 3;
                    break;
                case "Комплектующие ПК":
                    categoryComboBox.SelectedIndex = 4;
                    break;
                case "Сетевые платы":
                    categoryComboBox.SelectedIndex = 5;
                    break;
                case "Платы":
                    categoryComboBox.SelectedIndex = 6;
                    break;
                case "Конверторы":
                    categoryComboBox.SelectedIndex = 7;
                    break;
                case "Переферия":
                    categoryComboBox.SelectedIndex = 8;
                    break;
                case "Оборудование":
                    categoryComboBox.SelectedIndex = 9;
                    break;
                case "Реле":
                    categoryComboBox.SelectedIndex = 10;
                    break;
            }


            addTableMoving();
        }

        //обновление таблицы перемещения
        private void addTableMoving()
        {
            var myConnection = new MySqlConnection(conn);
            myConnection.Open();

            //Добавляем в таблицу перемещения данные

            Log("");
            Log("Окно: infoEquipWindow");
            Log("Метод: addTableMoving");
            var sqlMoving = "SELECT id, date, moving FROM peremechenie WHERE name=" + IDClass.id;
            Log("SQL запрос: " + sqlMoving);

            // Создание объекта, выполняющего запрос к БД:
            var command = new MySqlCommand(sqlMoving, myConnection);

            // Получение объекта для чтения данных из БД, содержащих несколько строк и столбцов
            var reader = command.ExecuteReader();

            //Строка таблицы
            while (reader.Read())
            {
                //id.Add(new idList() { storageID = reader[0].ToString() });
                var index1 = reader[2].ToString().IndexOf("шк./стел.: ", StringComparison.Ordinal);
                var index2 = reader[2].ToString().IndexOf("пол./кр.: ", StringComparison.Ordinal);
                var index3 = reader[2].ToString().IndexOf("сост.: ", StringComparison.Ordinal);
                var index4 = reader[2].ToString().IndexOf("В пом.: ", StringComparison.Ordinal);
                var index5 = reader[2].ToString().Substring(index4).
                    IndexOf("шк./стел.: ", StringComparison.Ordinal);
                var index6 = reader[2].ToString().Substring(index4).
                    IndexOf("пол./кр.: ", StringComparison.Ordinal);
                var index7 = reader[2].ToString().Substring(index4).IndexOf("сост.: ", StringComparison.Ordinal);
                items.Add(new equipmentMovingListView()
                {
                    equipmentID = reader[0].ToString(),
                    equipmentName = infoEquipment[1],
                    equipmentDate = reader[1].ToString().Remove(10),

                    equipmentRoomIZ = reader[2].ToString().Substring(9, (index1 - 10)),

                    equipmentStellagIZ = reader[2].ToString().Substring(
                        index1 + 11,
                        ((index2 -
                          (index1 + 12)))),

                    equipmentPolkaIZ = reader[2].ToString().Substring(
                        index2 + 10,
                        ((index3 -
                          (index2 + 11)))),

                    equipmentSostIZ = reader[2].ToString().Substring(
                        index3 + 7,
                        ((index4 -
                          (index3 + 8)))),

                    equipmentRoomV = reader[2].ToString().Substring(index4).Substring(8,
                        (index5 - 9)),

                    equipmentStellagV = reader[2].ToString().Substring(index4).Substring(
                        index5 + 11,
                        ((index6 -
                          (index5 + 12)))),

                    equipmentPolkaV = reader[2].ToString().Substring(index4).Substring(
                        index6 + 10,
                        ((index7 -
                          (index6 + 11)))),

                    equipmentSostV = reader[2].ToString().Substring(index4).Substring(
                        index7 + 7,
                        ((reader[2].ToString().Substring(index4).IndexOf("исп.: ", StringComparison.Ordinal) -
                          (index7 + 8)))),

                    equipmentIsp = reader[2].ToString().Substring(index4).Substring(
    (reader[2].ToString().Substring(index4).IndexOf("исп.: ", StringComparison.Ordinal)) + 6,
    ((reader[2].ToString().Substring(index4).Length -
      (reader[2].ToString().Substring(index4).IndexOf("исп.: ", StringComparison.Ordinal) + 6))))

                });
            }

            reader.Close();
            myConnection.Close();
            equipmentMovingListView.ItemsSource = items;
            Log("");

        }

        //Обработчик нажатия на строку в таблице
        private void changeMouse(object sender, MouseButtonEventArgs e)
        {
            //проверяем, если нажали на пустое место
            if (equipmentMovingListView.SelectedItem != null &&
                !string.IsNullOrEmpty(equipmentMovingListView.SelectedItem.ToString()))
            {
                //Обработка выбора таблицы
                var item = equipmentMovingListView.SelectedItem as equipmentMovingListView;
                IDClass.idMovingEquip = item.equipmentID;
                var editMovingWindow = new EditMovingWindow();

                editMovingWindow.Show();
                this.Close();
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            // MainWindow main = new MainWindow();
            //  main.Wi
        }


        //Кнопка "Удалить"
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Удалить?",
                    "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Log("infoEquipWindow deleteButton удален " + IDClass.id);
                var connection = new MySqlConnection(conn);
                // открываем соединение
                connection.Open();
                // запрос удаления данных
                var query = "DELETE FROM storagebase WHERE id = " + IDClass.id;
                // объект для выполнения SQL-запроса
                var command = new MySqlCommand(query, connection);
                // выполняем запрос
                command.ExecuteNonQuery();

                // закрываем подключение к БД
                connection.Close();
                (this.Owner as MainWindow)?.UpdateList();
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

        //Обноление данных в БД "storagebase"m
        private void updateStorage()
        {
            // объект для установления соединения с БД
            var connection = new MySqlConnection(conn);
            // открываем соединение
            connection.Open();
            // запрос обновления данных
            var query = "UPDATE storagebase SET name = '" + nameEquipmentBox.Text + "', type='" +
                        typeEquipmentBox.Text + "', room='" + roomEquipmentBox.Text +
                        "', stellag='" + stellagEquipmentBox.Text + "', polka='" + polkaEquipmentBox.Text +
                        "', oborydovanie='" + OborudEquipmentBox.Text +
                        "', sn='" + SNEquipmentBox.Text + "', datemade='" + dataEquipmentBox.Text + "', sost='" +
                        sostEquipmentComBox.Text + "', category='" + categoryComboBox.Text +
                        "' WHERE id ='" + IDClass.id + "'";
            Log("InfoEquipWindow roomEquipmentBox.Text= " + roomEquipmentBox.Text);
            Log("InfoEquipWindow polkaEquipmentBox.Text= " + polkaEquipmentBox.Text);
            Log("InfoEquipWindow Query: " + query);
            // объект для выполнения SQL-запроса
            var command = new MySqlCommand(query, connection);
            // выполняем запрос
            command.ExecuteNonQuery();
            // закрываем подключение к БД
            connection.Close();
            (Owner as MainWindow)?.UpdateList();
        }

        //Кнопка "Обновить"
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            updateStorage();
        }

        private void addCarantinButton_Click(object sender, RoutedEventArgs e)
        {
            var connection = new MySqlConnection(conn);
            // открываем соединение
            connection.Open();
            // запрос обновления данных
            Log("");
            Log("Окно: infoEquipWindow");
            Log("Метод: addCarantinButton_Click");

            var query = "UPDATE storagebase SET sost='карантин' WHERE id ='" + IDClass.id + "';";
            Log("SQL запрос: " + query);

            // объект для выполнения SQL-запроса
            var command = new MySqlCommand(query, connection);
            // выполняем запрос
            command.ExecuteNonQuery();
            Log("");
            // закрываем подключение к БД
            connection.Close();
            sostEquipmentComBox.Text = "карантин";

            Log("ID:" + infoEquipment[0] + " ИЗ пом.:" + infoEquipment[3] + " шк./стел.: " + infoEquipment[4] + " пол./кр.: " + infoEquipment[5] +
                " сост.: " + infoEquipment[9] + " В пом.: " + roomEquipmentBox.Text + " шк./стел.: " +
                stellagEquipmentBox.Text + " пол./кр.: " + polkaEquipmentBox.Text + " сост.: " + sostEquipmentComBox.Text + " исп.: " + ispComboBox.Text);

            // объект для установления соединения с БД

            // открываем соединение
            connection.Open();

            // запросы
            // запрос вставки данных
            if (ispComboBox.Text != null)
            {
                var queryInsert = "INSERT INTO peremechenie (name, date, moving) VALUES ('" + IDClass.id + "', '" +
                                  DateTime.Now.ToString("yyyy-MM-dd")
                                  + "', '" + "ИЗ пом.: " + infoEquipment[3] + " шк./стел.: " + infoEquipment[4] + " пол./кр.: " + infoEquipment[5] +
                                  " сост.: " + infoEquipment[9] + " В пом.: " + roomEquipmentBox.Text + " шк./стел.: " +
                                  stellagEquipmentBox.Text + " пол./кр.: " + polkaEquipmentBox.Text + " сост.: " + sostEquipmentComBox.Text + " исп.: " + ispComboBox.Text + "')";
                Log("SQL запрос: " + queryInsert);

                // объект для выполнения SQL-запроса
                var commandInsert = new MySqlCommand(queryInsert, connection);

                // выполняем запрос
                commandInsert.ExecuteNonQuery();


                // закрываем подключение к БД
                connection.Close();

                //обновляем таблицу
                items.Clear();
                updateStorage();
                addTableMoving();

                CollectionViewSource.GetDefaultView(equipmentMovingListView.ItemsSource).Refresh();

                //addMoving();
                (Owner as MainWindow)?.UpdateList();
                Close();
            }
        }

        private void deleteCarantinButton_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Удалить?",
                    "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            Log("");
            Log("Окно: infoEquipWindow");
            Log("Метод: deleteCarantinButton_Click");

            var connection = new MySqlConnection(conn);
            // открываем соединение
            connection.Open();
            // запрос удаления данных
            var query = "UPDATE storagebase SET sost='исправен' WHERE id ='" + IDClass.id + "';";
            Log("SQL запрос: " + query);
            // объект для выполнения SQL-запроса
            var command = new MySqlCommand(query, connection);
            // выполняем запрос
            command.ExecuteNonQuery();

            Log("");
            // закрываем подключение к БД
            connection.Close();
            sostEquipmentComBox.Text = "исправен";
            connection.Open();
            // запросы
            // запрос вставки данных
            if (ispComboBox != null)
            {
                var queryInsert = "INSERT INTO peremechenie (name, date, moving) VALUES ('" + IDClass.id + "', '" +
                                  DateTime.Now.ToString("yyyy-MM-dd")
                                  + "', '" + "ИЗ пом.: " + infoEquipment[3] + " шк./стел.: " + infoEquipment[4] + " пол./кр.: " + infoEquipment[5] +
                                  " сост.: " + infoEquipment[9] + " В пом.: " + roomEquipmentBox.Text + " шк./стел.: " +
                                  stellagEquipmentBox.Text + " пол./кр.: " + polkaEquipmentBox.Text + " сост.: " + sostEquipmentComBox.Text + " исп.: " + ispComboBox.Text + "')";
                Log("SQL запрос: " + queryInsert);

                // объект для выполнения SQL-запроса
                var commandInsert = new MySqlCommand(queryInsert, connection);

                // выполняем запрос
                commandInsert.ExecuteNonQuery();


                // закрываем подключение к БД
                connection.Close();

                //обновляем таблицу
                items.Clear();
                updateStorage();
                addTableMoving();

                CollectionViewSource.GetDefaultView(equipmentMovingListView.ItemsSource).Refresh();

                Log("");
                (Owner as MainWindow)?.UpdateList();
                ((MainWindow)Owner).Topmost = true;
                Close();
            }
        }

        private void roomEquipmentBoxAdd()
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
                    Log("Метод: roomEquipmentBoxAdd");
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
            roomEquipmentBox.ItemsSource = rooms;
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
                        Racks.Add(new Rack() {Room = reader[0].ToString(), Name = reader[1].ToString()});
                    }

                    reader.Close();
                    myConnection.Close();

                }
            }
        }

        //"Живой" поиск в comboBox
        private void roomEquipmentBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            stellagEquipmentBox.ItemsSource = Racks.Where(x => x.Room == (roomEquipmentBox.SelectedValue as Room)?.Name).ToList();

        }
        private void stellagEquipmentBox_TextChangedStellag(object sender, TextChangedEventArgs e)
        {

        }
        //Кнопка "Перемещение"
        private void buttonMove_Click(object sender, RoutedEventArgs e)
        {
            Log("");
            Log("Окно: infoEquipWindow");
            Log("Метод: buttonMove_Click");
            Log("Начальные данные");
            Log("RoomList: " + infoEquipment[3] + ", Stellag: " + infoEquipment[4] + ", Polka: " + infoEquipment[5] + ", Sost: " + infoEquipment[9]);
            Log("Измененные данные");
            Log("RoomList: " + roomEquipmentBox.Text + ", Stellag: " + stellagEquipmentBox.Text + ", Polka: " +
                polkaEquipmentBox.Text + ", Sost: " + sostEquipmentComBox.Text);

            if (!roomEquipmentBox.Text.Equals(infoEquipment[3]) || !stellagEquipmentBox.Text.Equals(infoEquipment[4]) ||
                !polkaEquipmentBox.Text.Equals(infoEquipment[5]) || !sostEquipmentComBox.Text.Equals(infoEquipment[9]))

            {
                Log("Есть изменение!");
                Log("");
                Log("ID:" + infoEquipment[0] + " ИЗ пом.:" + infoEquipment[3] + " шк./стел.: " + infoEquipment[4] + " пол./кр.: " + infoEquipment[5] +
                    " сост.: " + infoEquipment[9] + " В пом.: " + roomEquipmentBox.Text + " шк./стел.: " +
                    stellagEquipmentBox.Text + " пол./кр.: " + polkaEquipmentBox.Text + " сост.: " + sostEquipmentComBox.Text + " исп.: " + ispComboBox.Text);

                // объект для установления соединения с БД
                var connection = new MySqlConnection(conn);

                // открываем соединение
                connection.Open();

                // запросы
                // запрос вставки данных
                var query = "INSERT INTO peremechenie (name, date, moving) VALUES ('" + IDClass.id + "', '" +
                            DateTime.Now.ToString("yyyy-MM-dd")
                            + "', '" + "ИЗ пом.: " + infoEquipment[3] + " шк./стел.: " + infoEquipment[4] + " пол./кр.: " + infoEquipment[5] +
                            " сост.: " + infoEquipment[9] + " В пом.: " + roomEquipmentBox.Text + " шк./стел.: " +
                            stellagEquipmentBox.Text + " пол./кр.: " + polkaEquipmentBox.Text + " сост.: " + sostEquipmentComBox.Text + " исп.: " + ispComboBox.Text + "')";
                Log("SQL запрос" + query);

                // объект для выполнения SQL-запроса
                var command = new MySqlCommand(query, connection);

                // выполняем запрос
                command.ExecuteNonQuery();

                // закрываем подключение к БД
                connection.Close();

                //обновляем таблицу
                items.Clear();
                updateStorage();
                addTableMoving();

                CollectionViewSource.GetDefaultView(equipmentMovingListView.ItemsSource).Refresh();
                //equipmentMovingListView.Items.Refresh();
                infoEquipment[3] = roomEquipmentBox.Text;
                infoEquipment[4] = stellagEquipmentBox.Text;
                infoEquipment[5] = polkaEquipmentBox.Text;
                infoEquipment[9] = sostEquipmentComBox.Text;
            }

            else
            {
                Log("Нет изменений!");
                updateStorage();
            }
            Log("");
        }

        //обратка нажатия ENTER
        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
        }

    }
}
