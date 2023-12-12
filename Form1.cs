using System;
using System.Windows.Forms;
using Npgsql;

namespace HotelManagementSystem
{
    public partial class Form1 : Form
    {
        private const string ConnectionString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=HotelManagementDB";

        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;

        public Form1()
        {
            InitializeComponent();
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            btnLogin = new Button();

            txtUsername.Location = new System.Drawing.Point(120, 50);
            txtUsername.Size = new System.Drawing.Size(150, 20);

            txtPassword.Location = new System.Drawing.Point(120, 80);
            txtPassword.Size = new System.Drawing.Size(150, 20);
            txtPassword.PasswordChar = '*';

            btnLogin.Location = new System.Drawing.Point(120, 110);
            btnLogin.Size = new System.Drawing.Size(150, 30);
            btnLogin.Text = "Войти";
            btnLogin.Click += new EventHandler(btnLogin_Click);
            LinkLabel linkRegister = new LinkLabel();

            linkRegister.Text = "Зарегистрироваться";
            linkRegister.Location = new System.Drawing.Point(120, 150);
            linkRegister.Size = new System.Drawing.Size(150, 20);
            linkRegister.LinkClicked += new LinkLabelLinkClickedEventHandler(linkRegister_LinkClicked);

            this.Controls.Add(txtUsername);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(linkRegister);
        }
        private void OpenRegistrationForm()
        {
            RegistrationForm registrationForm = new RegistrationForm();
            registrationForm.ShowDialog();
        }

        private void linkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenRegistrationForm();
        }
        private void OpenManagerForm(string role)
        {
            if (role == "Admin")
            {
                ManagerForm managerForm = new ManagerForm();
                managerForm.ShowDialog();
            }
            else
            {
                // Здесь вы можете добавить логику для открытия других форм в зависимости от роли
                MessageBox.Show("Доступ только для менеджера.");
            }
        }


        private void btnLogin_Click(object sender, EventArgs e)
{
    string username = txtUsername.Text;
    string password = txtPassword.Text;

    using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
    {
        connection.Open();

        using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM users WHERE username = '{username}' AND password = '{password}'", connection))
        {
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    string role = reader["role"].ToString();
                    if (role == "Admin")
                    {OpenManagerForm(role);}
                    
                    if (role == "Guest")
                    {
                        // Проверяем наличие гостя в таблице guests
                        if (CheckGuestExistence(username))
                        {
                            // Гость найден, открываем форму с номером комнаты
                            OpenGuestRoomForm(username);
                        }
                        else
                        {
                            // Гостя нет, открываем форму с сообщением о отсутствии номера
                            OpenNoGuestRoomForm();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Добро пожаловать, {username}! Роль: {role}");
                        // Здесь можно добавить логику для перехода к различным формам в зависимости от роли пользователя
                        // Например: OpenAdminForm(), OpenWorkerForm()
                    }
                }
                else
                {
                    MessageBox.Show("Неверные учетные данные. Пожалуйста, повторите попытку.");
                }
            }
        }
    }
}

private bool CheckGuestExistence(string guestName)
{
    using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
    {
        connection.Open();

        using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM guests WHERE guest_name = '{guestName}'", connection))
        {
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                return reader.Read();
            }
        }
    }
}

private void OpenGuestRoomForm(string guestName)
{
    // Открываем форму с номером комнаты гостя
    GuestRoomForm guestRoomForm = new GuestRoomForm(guestName);
    guestRoomForm.Show();

    // Закрываем текущую форму входа
    this.Hide();
}

private void OpenNoGuestRoomForm()
{
    // Открываем форму с сообщением о отсутствии номера комнаты
    NoGuestRoomForm noGuestRoomForm = new NoGuestRoomForm();
    noGuestRoomForm.Show();

    // Закрываем текущую форму входа
    this.Hide();
}

    }
}
