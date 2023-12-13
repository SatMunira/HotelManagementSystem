using System;
using System.Windows.Forms;
using Npgsql;

namespace HotelManagementSystem
{
    public partial class GuestRoomForm : Form
    {
        private const string ConnectionString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=HotelManagementDB";

        private Label lblRoomInfo;
        private Button btnWantsServices;

        private string guestName;

        public GuestRoomForm(string guestName)
        {
            this.guestName = guestName;
            InitializeComponent();
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            lblRoomInfo = new Label();
            btnWantsServices = new Button();

            lblRoomInfo.Location = new System.Drawing.Point(50, 50);
            lblRoomInfo.Size = new System.Drawing.Size(300, 50);

            btnWantsServices.Location = new System.Drawing.Point(50, 120);
            btnWantsServices.Size = new System.Drawing.Size(150, 30);
            btnWantsServices.Text = "Хочу сервис";
            btnWantsServices.Click += new EventHandler(btnWantsServices_Click);

            this.Controls.Add(lblRoomInfo);
            this.Controls.Add(btnWantsServices);

            UpdateRoomInfo();
        }

        private void UpdateRoomInfo()
        {
            lblRoomInfo.Text = GetGuestRoomInfo(guestName);
        }

        private string GetGuestRoomInfo(string guestName)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT room_number, wants_services FROM guests WHERE guest_name = '{guestName}'", connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int roomNumber = reader.GetInt32(0);
                            bool wantsServices = reader.GetBoolean(1);

                            string servicesInfo = wantsServices ? "Хочет сервис" : "Не хочет сервис";

                            return $"Добро пожаловать, {guestName}! Ваш номер комнаты: {roomNumber}. {servicesInfo}";
                        }
                        else
                        {
                            return $"Ошибка: Гость {guestName} не найден в базе данных.";
                        }
                    }
                }
            }
        }
        private void InitializeComponent()
        {
            // Этот метод может быть пустым, если вы не используете дизайнер формы
        }

        private void btnWantsServices_Click(object sender, EventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"UPDATE guests SET wants_services = true, worker_name = NULL WHERE guest_name = '{guestName}'", connection))
                {
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Ваш запрос на получение сервиса принят.");
                        // Обновляем информацию о номере комнаты
                        UpdateRoomInfo();
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка при обновлении запроса на сервис. Пожалуйста, повторите попытку.");
                    }
                }
            }
        }
    }
}
