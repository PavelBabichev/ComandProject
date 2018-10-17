using System;
using System.Collections.Generic;
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

namespace Testing
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
        }

        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
            using (TestingEntities db = new TestingEntities())
                //Валидация формы
            {
                if (txtFirstName.Text == "" || txtLastName.Text == "" || txtLogin.Text == "" || txtPass.Password == "" || txtRepeatPass.Password == "")
                {
                    MessageBox.Show("Не все поля заполнены");
                    clearForm();
                    return;
                }
                if (txtPass.Password != txtRepeatPass.Password)
                {
                    lblErLogin.Visibility = Visibility.Hidden;
                    lblErPass.Visibility = Visibility.Visible;
                    clearForm();
                    return;
                }
                User user = db.Users.FirstOrDefault(u => u.user_login == txtLogin.Text);
                //Проверка на уникальность логина
                if (user != null)
                {
                    lblErLogin.Visibility = Visibility.Visible;
                    lblErPass.Visibility = Visibility.Hidden;
                    clearForm();
                    return;
                }
                db.Users.Add(new User { first_name = txtFirstName.Text, last_name = txtLastName.Text, user_login = txtLogin.Text, user_password = txtPass.Password, roleId = 2 });
                db.SaveChanges();
            }
            MessageBox.Show("Регистрация прошла успешно");
            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
        }
        void clearForm()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtLogin.Clear();
            txtPass.Clear();
            txtRepeatPass.Clear();
        }
    }
}
