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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Testing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (TestingEntities db = new TestingEntities())
            {
                User user = db.Users.FirstOrDefault(u => u.user_login == txtLogin.Text);
                //Проверяем правильность ввода данных
                if (user == null|| txtPaasword.Password != user.user_password)
                {
                    lblErr.Visibility = Visibility.Visible;
                    lblErr.Visibility = Visibility.Visible;
                    txtLogin.Clear();
                    txtPaasword.Clear();
                    txtLogin.Focus();
                    return;
                }
                bool role;
                //Выбор интерфейса преподаватель/студент
                role = (user.Role.role_name == "Преподаватель") ? true : false;
                MainWindowProgram window = new MainWindowProgram(role,user.id);
                window.Show();
                this.Close();
            }
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Registration window = new Registration();
            window.Show();
            this.Close();
        }
    }
}
