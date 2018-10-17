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
    /// Interaction logic for MainWindowProgram.xaml
    /// </summary>
    public partial class MainWindowProgram : Window
    {
        bool isTeacher;
        bool isStudent;
        int id;
        public MainWindowProgram(bool flag, int id)
        {
            isTeacher = flag;
            this.id = id;
            InitializeComponent();
            this.Loaded += MainWindowProgram_Loaded;
            isStudent = false;
        }

        private void MainWindowProgram_Loaded(object sender, RoutedEventArgs e)
        {
            //интерфейс преподавателя
            if (isTeacher)
            {
                Title = "Режим преподавателя";
                btnPassTest.Visibility = Visibility.Hidden;
            }
            //интерфейс студента
            else
            {
                Title = "Режим студента";
                btnListStudents.Visibility = Visibility.Hidden;
                btnRedaktor.Visibility = Visibility.Hidden;
            }
            initList(true);
        }
        //заполнение списка
        void initList(bool isTest)
        {
            listBox.Items.Clear();
            using (TestingEntities db = new TestingEntities())
            {
                //заполнение списка тестов
                if (isTest)
                {
                    var tests = db.Tests;
                    foreach (var item in tests)
                    {
                        ListBoxItem listBoxItem = new ListBoxItem();
                        listBoxItem.Margin = new Thickness(5);
                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = item.name_test;
                        listBoxItem.Content = textBlock;
                        listBox.Items.Add(listBoxItem);
                    }
                }
                else
                {
                    //заполнение списка студентов
                    var students = db.Users.Where(u => u.Role.role_name == "Студент");
                    foreach (var item in students)
                    {
                        ListBoxItem listBoxItem = new ListBoxItem();
                        listBoxItem.Margin = new Thickness(5);
                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = item.first_name + " " + item.last_name;
                        listBoxItem.Content = textBlock;
                        listBox.Items.Add(listBoxItem);
                    }
                }
            }
        }

        private void btnListTest_Click(object sender, RoutedEventArgs e)
        {
            initList(true);
            isStudent = false;
        }

        private void btnListStudents_Click(object sender, RoutedEventArgs e)
        {
            initList(false);
            isStudent = true;
        }

        private void btnResults_Click(object sender, RoutedEventArgs e)
        {
            
            if (listBox.SelectedItem == null)
                return;
            //если интерфейс преподавателя
            if (isTeacher)
            {
                using (TestingEntities db = new TestingEntities())
                {
                    //исли активен список студентов
                    if (isStudent)
                    {
                        //получаем имя и фамилию студента, выбранного в списке
                        string studentName = ((listBox.SelectedItem as ListBoxItem).Content as TextBlock).Text;
                        var tests = db.Tests;
                        //выбираем результаты по выбранному студенту
                        var results = db.UserRatings.Where(r => (r.User.first_name + " " + r.User.last_name) == studentName).ToList();
                        listBox.Items.Clear();
                        //заполняем листбокс
                        foreach (var item in tests)
                        {
                            UserRating rating = results.FirstOrDefault(r => r.testId == item.id);
                            ListBoxItem listBoxItem = new ListBoxItem();
                            listBoxItem.Margin = new Thickness(5);
                            TextBlock textBlock = new TextBlock();
                            //если студент не здавал еще тест
                            if (rating == null)
                                textBlock.Text = item.name_test + "\t не здавал";
                            //оценка
                            else
                                textBlock.Text = item.name_test + " \t" + rating.rating + "%";
                            listBoxItem.Content = textBlock;
                            listBox.Items.Add(listBoxItem);
                        }
                    }
                    else
                    {
                        //если активен список тестов
                        int testId = listBox.SelectedIndex + 1;

                        listBox.Items.Clear();
                        foreach (var item in db.Users.Where(u => u.Role.role_name == "Студент"))
                        {
                            //выбераем результаты по каждому тесту
                            UserRating result = db.UserRatings.FirstOrDefault(r => (r.testId == testId) && (r.userId == item.id));
                            ListBoxItem listBoxItem = new ListBoxItem();
                            listBoxItem.Margin = new Thickness(5);
                            TextBlock textBlock = new TextBlock();
                            //если результатов нет
                            if (result == null)
                            {
                                textBlock.Text = item.first_name + " " + item.last_name + "\t не здавал";
                            }
                            else
                                textBlock.Text = item.first_name + " " + item.last_name + "\t" + result.rating + "%";
                            listBoxItem.Content = textBlock;
                            listBox.Items.Add(listBoxItem);
                        }
                    }

                }
            }
            else
            {
                //интерфейс студента
                using (TestingEntities db = new TestingEntities())
                {
                    //выбираем все тесты по данному студенту
                    int testId = listBox.SelectedIndex + 1;
                    Test tests = db.Tests.FirstOrDefault(t => t.id == testId);
                    listBox.Items.Clear();
                    UserRating results = db.UserRatings.FirstOrDefault(r => (r.userId == id) && (r.testId == tests.id));
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Margin = new Thickness(5);
                    TextBlock textBlock = new TextBlock();
                    //если результатов нет
                    if (results == null)
                        textBlock.Text = tests.name_test + " \tне здавал";
                    //оценка
                    else
                        textBlock.Text = tests.name_test + "\t " + results.rating + "%";
                    listBoxItem.Content = textBlock;
                    listBox.Items.Add(listBoxItem);
                }
            }
        }

        private void btnPassTest_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem == null)
                return;
            PassingTest window = new PassingTest((listBox.SelectedIndex + 1), id);
            window.Show();
            this.Close();
        }

        private void btnRedaktor_Click(object sender, RoutedEventArgs e)
        {
            redaktorTestov window = new redaktorTestov();
            window.Show();
            this.Close();
        }
    }
}