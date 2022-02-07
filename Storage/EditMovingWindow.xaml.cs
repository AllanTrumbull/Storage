using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для EditMovingWindow.xaml
    /// </summary>
    public partial class EditMovingWindow : Window
    {

        string conn;
        string path = "log.txt";
        public EditMovingWindow()
        {
            var sqlCon = new Connect();
            conn = sqlCon.conn;
            InitializeComponent();
            loadData();
            this.Closed += MainWindow_Closed;
        }
        private void loadData()
        {

            var myConnection = new MySqlConnection(conn);
            myConnection.Open();
            //Добавляем в таблицу перемещения данные


            var sqlMoving = "SELECT moving, date FROM peremechenie WHERE id=" + IDClass.idMovingEquip;

            // Создание объекта, выполняющего запрос к БД:
            var command = new MySqlCommand(sqlMoving, myConnection);

            // Получение объекта для чтения данных из БД, содержащих несколько строк и столбцов
            var reader = command.ExecuteReader();

            //Строка таблицы
            while (reader.Read())
            {
                movingBox.Text = reader[0].ToString();
                date.Text = reader[1].ToString();
            }
            reader.Close();
            myConnection.Close();
        }

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            // объект для установления соединения с БД
            var connection = new MySqlConnection(conn);
            // открываем соединение
            connection.Open();
            // запрос обновления данных
            if (date.SelectedDate != null)
            {
                var query = "UPDATE peremechenie SET name = '" + IDClass.id + "', date = '" + date.SelectedDate.Value.ToString("yyyy-MM-dd") + "', moving = '" +
                            movingBox.Text + "' WHERE id ='" + IDClass.idMovingEquip + "'";
                // объект для выполнения SQL-запроса
                var command = new MySqlCommand(query, connection);
                // выполняем запрос
                command.ExecuteNonQuery();
            }

            // закрываем подключение к БД
            connection.Close();
            var infoEquipWindow = new infoEquipWindow();

            infoEquipWindow.Show();
            this.Close();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            var connection = new MySqlConnection(conn);
            // открываем соединение
            connection.Open();
            // запрос удаления данных
            var query = "DELETE FROM peremechenie WHERE id = " + IDClass.idMovingEquip;
            // объект для выполнения SQL-запроса
            var command = new MySqlCommand(query, connection);
            // выполняем запрос
            command.ExecuteNonQuery();

            // закрываем подключение к БД

            connection.Close();
            var infoEquipWindow = new infoEquipWindow();

            infoEquipWindow.Show();
            this.Close();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Log("Closed");
        }


        private void Log(string eventName)
        {
            using (var logger = new StreamWriter(path, true))
            {
                logger.WriteLine(DateTime.Now.ToLongTimeString() + " - " + eventName);
            }
        }
    }
}
