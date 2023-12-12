using System;
using System.Windows.Forms;
using Npgsql;

namespace HotelManagementSystem
{
    public partial class DeleteGuestConfirmationForm : Form
    {
        private const string ConnectionString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=HotelManagementDB";

        private Button btnConfirmDelete;

        private int guestId; // Идентификатор гостя

        public DeleteGuestConfirmationForm(int guestId)
        {
            this.guestId = guestId;
            InitializeComponent();
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            btnConfirmDelete = new Button();
            btnConfirmDelete.Location = new System.Drawing.Point(10, 10);
            btnConfirmDelete.Size = new System.Drawing.Size(150, 30);
            btnConfirmDelete.Text = "Подтвердить удаление";
            btnConfirmDelete.Click += new EventHandler(btnConfirmDelete_Click);

            this.Controls.Add(btnConfirmDelete);
        }

        private void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                // Получаем номер комнаты гостя перед удалением
                int roomNumber;
                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT room_number FROM guests WHERE id = {guestId}", connection))
                {
                    roomNumber = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (NpgsqlCommand cmd = new NpgsqlCommand($"DELETE FROM guests WHERE id = {guestId}", connection))
                {
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Гость успешно удален.");

                        // Обновляем статус комнаты на "свободен" после удаления гостя
                        UpdateRoomStatus(roomNumber, "Свободен");

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка при удалении гостя. Пожалуйста, повторите попытку.");
                    }
                }
            }
        }
        private void InitializeComponent()
        {
            // Этот метод может быть пустым, если вы не используете дизайнер формы
        }

        private void UpdateRoomStatus(int roomNumber, string status)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"UPDATE rooms SET status = '{status}' WHERE room_number = {roomNumber}", connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
