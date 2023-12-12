using System;
using System.Windows.Forms;
using Npgsql;

namespace HotelManagementSystem
{
    public partial class RegistrationForm : Form
    {
        private const string ConnectionString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=HotelManagementDB";

        private TextBox txtNewUsername;
        private TextBox txtNewPassword;
        private Button btnRegister;

        public RegistrationForm()
        {
            InitializeComponent();
            InitializeComponents();
        }
         private void InitializeComponent()
        {
            // Этот метод может быть пустым, если вы не используете дизайнер формы
        }

        private void InitializeComponents()
        {
            txtNewUsername = new TextBox();
            txtNewPassword = new TextBox();
            btnRegister = new Button();

            txtNewUsername.Location = new System.Drawing.Point(120, 50);
            txtNewUsername.Size = new System.Drawing.Size(150, 20);

            txtNewPassword.Location = new System.Drawing.Point(120, 80);
            txtNewPassword.Size = new System.Drawing.Size(150, 20);
            txtNewPassword.PasswordChar = '*';

            btnRegister.Location = new System.Drawing.Point(120, 110);
            btnRegister.Size = new System.Drawing.Size(150, 30);
            btnRegister.Text = "Зарегистрироваться";
            btnRegister.Click += new EventHandler(btnRegister_Click);

            this.Controls.Add(txtNewUsername);
            this.Controls.Add(txtNewPassword);
            this.Controls.Add(btnRegister);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string newUsername = txtNewUsername.Text;
            string newPassword = txtNewPassword.Text;

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                // Проверяем, существует ли пользователь с таким именем
                using (NpgsqlCommand checkCmd = new NpgsqlCommand($"SELECT COUNT(*) FROM users WHERE username = '{newUsername}'", connection))
                {
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        MessageBox.Show("Пользователь с таким именем уже существует. Выберите другое имя пользователя.");
                        return;
                    }
                }

                // Регистрируем нового пользователя
                using (NpgsqlCommand registerCmd = new NpgsqlCommand($"INSERT INTO users (username, password, role) VALUES ('{newUsername}', '{newPassword}', 'Guest')", connection))
                {
                    int rowsAffected = registerCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Регистрация успешна, {newUsername}! Теперь вы можете войти в систему.");
                        this.Close(); // Закрываем форму регистрации после успешной регистрации
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка при регистрации. Пожалуйста, повторите попытку.");
                    }
                }
            }
        }
    }
}
