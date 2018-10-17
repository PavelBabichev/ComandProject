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
    /// Логика взаимодействия для redaktorTestov.xaml
    /// </summary>
    public partial class redaktorTestov : Window
    {
        int testId;
        int questionId;
        public redaktorTestov()
        {
            InitializeComponent();
            Loaded += RedaktorTestov_Loaded;
        }

        private void RedaktorTestov_Loaded(object sender, RoutedEventArgs e)
        {
           using(TestingEntities db=new TestingEntities())
            {
                var tests = db.Tests;
                //заполняем комбобокс тестами
                foreach (Test item in tests)
                {
                    ComboBoxItem comboBoxItem = new ComboBoxItem();
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = item.name_test;
                    comboBoxItem.Content = textBlock;
                    comboBoxItem.Selected += ComboBoxItem_Selected;
                    cmbTests.Items.Add(comboBoxItem);
                }
            }
        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            //заполняем ответы по выбраному тесту
            txtTest.Text = "";
            txtTest.Text = ((sender as ComboBoxItem).Content as TextBlock).Text;
            cmbQuestion.Items.Clear();
            txtAddQuestion.Clear();
            txtQuestion.Clear();
            using(TestingEntities db=new TestingEntities())
            {
                Test test = db.Tests.FirstOrDefault(t => t.name_test == txtTest.Text);
                testId = test.id;
                var questions = db.Questions.Where(q => q.testId == testId);
                foreach (Question item in questions)
                {
                    ComboBoxItem comboBoxItemQueation = new ComboBoxItem();
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = item.name_question;
                    comboBoxItemQueation.Content = textBlock;
                    comboBoxItemQueation.Selected += ComboBoxItemQueation_Selected;
                    cmbQuestion.Items.Add(comboBoxItemQueation);
                }
            }
        }

        private void ComboBoxItemQueation_Selected(object sender, RoutedEventArgs e)
        {
            txtQuestion.Text = "";
            txtQuestion.Text = ((sender as ComboBoxItem).Content as TextBlock).Text;
            using (TestingEntities db = new TestingEntities()) {
                Question question = db.Questions.FirstOrDefault(q => q.name_question == txtQuestion.Text);
                questionId = question.id;
                    }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //удаление теста
            if (cmbTests.SelectedItem == null)
                return;
            using (TestingEntities db = new TestingEntities())
            {
                db.Tests.Remove(db.Tests.First(t => t.id == testId));
                var deleteQestion = db.Questions.Where(q => q.testId == testId);
                //удаление вопросов и ответов удаленного теста
                foreach (Question item in deleteQestion)
                {
                    var deleteAnswer = db.Answers.Where(a => a.questionId == item.id);
                    db.Answers.RemoveRange(deleteAnswer);
                }
                db.Questions.RemoveRange(deleteQestion);                
                db.SaveChanges();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //редактирование теста
            if (txtTest.Text == "")
                return;
            using (TestingEntities db = new TestingEntities())
            {
                db.Tests.First(t => t.id == testId).name_test = txtTest.Text;
                db.SaveChanges();
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            //очистить поле ввода
            txtAddTest.Text = "";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //добавление нового теста
            if (txtAddTest.Text == "")
                return;
            using (TestingEntities db = new TestingEntities())
            {
                Test test = db.Tests.FirstOrDefault(t => t.name_test == txtAddTest.Text);
                //проверка на уникальность нового названия
                if (test == null)
                {
                    db.Tests.Add(new Test { name_test = txtAddTest.Text });
                    db.SaveChanges();
                }
                else
                {
                    MessageBox.Show("Тест с таким названием уже существует.");
                    btnClear_Click(null, null);
                }
            }
        }

        private void btnDelQuestion_Click(object sender, RoutedEventArgs e)
        {
            //удаление выбраного вопроса
            using (TestingEntities db = new TestingEntities())
            {
                db.Questions.Remove(db.Questions.First(q => q.id == questionId));
                var deleteAnswer = db.Answers.Where(a => a.questionId == questionId);
                //удаление ответов удаленного вопроса
                db.Answers.RemoveRange(deleteAnswer);
                db.SaveChanges();
            }
        }

        private void btnChangeQuestion_Click(object sender, RoutedEventArgs e)
        {
            //редактирование вопроса
            if (txtQuestion.Text == "")
                return;
            using (TestingEntities db = new TestingEntities())
            {
                db.Questions.First(q => q.id == questionId).name_question = txtTest.Text;
                db.SaveChanges();
            }
        }

        private void btnClearQuestion_Click(object sender, RoutedEventArgs e)
        {
            //очистка поля ввода
            txtAddQuestion.Text = "";
        }

        private void btnSaveQuestion_Click(object sender, RoutedEventArgs e)
        {
            //добавление нового вопроса
            if (txtAddQuestion.Text == "")
                return;
            using (TestingEntities db = new TestingEntities())
            {
                Question question = db.Questions.FirstOrDefault(q => q.name_question == txtAddQuestion.Text);
                //проверка на уникальность вопроса
                if (question == null)
                {
                    int id = cmbTests.SelectedIndex + 1;
                    db.Questions.Add(new Question { name_question = txtAddTest.Text,testId=id });
                    db.SaveChanges();
                }
                else
                {
                    MessageBox.Show("Вопрос уже существует.");
                    btnClearQuestion_Click(null, null);
                }
            }
        }

        private void btnRedaktorQuestion_Click(object sender, RoutedEventArgs e)
        {
            redactorAnswer window = new redactorAnswer();
            window.Show();
            this.Close();
        }
    }
}
