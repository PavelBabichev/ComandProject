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
    /// Логика взаимодействия для viewRusult.xaml
    /// </summary>
    public partial class viewRusult : Window
    {
        int studentId;
        int testId;
        int totalAnswer;
        int correntlyAnswer;
        public viewRusult(int student,int test,int totalAns,int correntlyAns)
        {
            InitializeComponent();
            studentId = student;
            testId = test;
            totalAnswer = totalAns;
            correntlyAnswer = correntlyAns;
            Loaded += ViewRusult_Loaded;
        }

        private void ViewRusult_Loaded(object sender, RoutedEventArgs e)
        {
            User name;
            Test testName;
            //заполняем результаты теста
            using (TestingEntities db = new TestingEntities())
            {
                name = db.Users.FirstOrDefault(u => u.id == studentId);
                testName = db.Tests.FirstOrDefault(t => t.id == testId);
                txtResult.Text = "Студент: " + name.first_name + " " + name.last_name + "\nТест: " + testName.name_test + "\nВсего вопросов: " + totalAnswer.ToString();
                txtResult.Text += "\nПравильных ответов: " + correntlyAnswer.ToString() + "\nРезультат: " + ((int)((double)correntlyAnswer / totalAnswer * 100)).ToString() + "%";
                db.UserRatings.Add(new UserRating { userId = studentId, testId = this.testId, rating =((int)((double)correntlyAnswer / totalAnswer * 100)).ToString() });
                db.SaveChanges();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MainWindowProgram window = new MainWindowProgram(false, studentId);
            window.Show();
            this.Close();
        }
    }
}
