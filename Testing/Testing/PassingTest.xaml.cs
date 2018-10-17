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
    /// Interaction logic for PassingTest.xaml
    /// </summary>
    public partial class PassingTest : Window
    {
        int testId;
        int studentId;
        int idQuestion;
        bool beAnswer;
        int correctAnswer;
        int totalAnswer;
        string Ans="";
        List<Question> listQuestion;
        public PassingTest(int test,int student)
        {
            testId = test;
            studentId = student;
            this.Loaded += PassingTest_Loaded;
            idQuestion = 0;
            beAnswer = false;
            InitializeComponent();
        }

        void PassingTest_Loaded(object sender, RoutedEventArgs e)
        {
            using (TestingEntities db = new TestingEntities())
            {
                //получаем выбранный тест
                Test test = db.Tests.First(t => t.id == testId);
                txtNameTest.Text = test.name_test;
                totalAnswer = db.Questions.Where(q => q.testId == testId).Count();
                listQuestion = db.Questions.Where(q => q.testId == testId).ToList();
                initList();
            }
        }

        private void initList()
        {
            listAnswer.Items.Clear();
            txtNameQuestion.Text = "";
            using (TestingEntities db = new TestingEntities())
            {
                txtNameQuestion.Text = listQuestion[idQuestion].name_question;
                int i = listQuestion[idQuestion].id;
                //получаем вопрос
                var answers = db.Answers.Where(a => a.questionId == i);
                //заполняем список ответов
                foreach (Answer item in answers)
                {
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Margin = new Thickness(5);
                    StackPanel stackItem = new StackPanel();
                    stackItem.Orientation = Orientation.Horizontal;
                    TextBlock textBlock = new TextBlock();
                    textBlock.MaxWidth = 750;
                    textBlock.TextWrapping = TextWrapping.Wrap;
                    textBlock.Text = item.answer_name;
                    RadioButton radio = new RadioButton { IsChecked = false, GroupName = "Answer" };
                    radio.Checked += radio_Checked;
                    stackItem.Children.Add(radio);
                    stackItem.Children.Add(textBlock);
                    listBoxItem.Content = stackItem;
                    listAnswer.Items.Add(listBoxItem);
                }
            }
        }

        void radio_Checked(object sender, RoutedEventArgs e)
        {
            //получаем ответ студента
            beAnswer = true;
            foreach (var item in ((sender as RadioButton).Parent as StackPanel).Children)
            {
                if (item is TextBlock)
                {
                    Ans = (item as TextBlock).Text;
                }
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            //проверяем был ли ответ
            if (!beAnswer)
                return;
            //проверяем правильность ответа
            using (TestingEntities db = new TestingEntities())
            {
                int i = listQuestion[idQuestion].id;
                Answer answer = db.Answers.FirstOrDefault(a => a.questionId ==i && a.answer_name == Ans);
                if (answer.correctly != 0)
                    correctAnswer++;
            }
            idQuestion++;
            //если вопросов больше нет
            if (idQuestion == totalAnswer)
            {
                viewRusult window = new viewRusult(studentId, testId, totalAnswer, correctAnswer);
                window.Show();
                this.Close();
            }
            else 
            initList();
            beAnswer = false;
        }
    }
}
