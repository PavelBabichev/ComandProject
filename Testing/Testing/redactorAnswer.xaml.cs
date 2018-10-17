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
    /// Логика взаимодействия для redactorAnswer.xaml
    /// </summary>
    public partial class redactorAnswer : Window
    {
        int idTest;
        int idQuestion;
        string correctlyAnswer;
        bool beAnswer;
        List<string> deleteAnswer = new List<string>();
        public redactorAnswer()
        {
            InitializeComponent();
            this.Loaded += RedactorAnswer_Loaded;
        }

        private void RedactorAnswer_Loaded(object sender, RoutedEventArgs e)
        {
            using (TestingEntities db = new TestingEntities())
            {
                var tests = db.Tests;
                foreach (Test item in tests)
                {
                    ComboBoxItem comboBoxItem = new ComboBoxItem();
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = item.name_test;
                    comboBoxItem.Content = textBlock;
                    comboBoxItem.Selected += ComboBoxItem_Selected;
                    cmbTest.Items.Add(comboBoxItem);
                }
            }
        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            //выбор теста
            cmbQuestion.Items.Clear();
            listBox.Items.Clear();
            string nameTest = ((sender as ComboBoxItem).Content as TextBlock).Text;
            using (TestingEntities db = new TestingEntities())
            {
                Test test = db.Tests.FirstOrDefault(t => t.name_test == nameTest);
                idTest = test.id;
                var questions = db.Questions.Where(q => q.testId == idTest);
                //заполнение комбобокса вапросами
                foreach (Question item in questions)
                {
                    ComboBoxItem comboBoxItemQueation = new ComboBoxItem();
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = item.name_question;
                    comboBoxItemQueation.Content = textBlock;
                    comboBoxItemQueation.Selected += ComboBoxItemQueation_Selected; ;
                    cmbQuestion.Items.Add(comboBoxItemQueation);
                }
            }
        }

        private void ComboBoxItemQueation_Selected(object sender, RoutedEventArgs e)
        {
            //выбор вопроса
            listBox.Items.Clear();
            string nameQuestion = ((sender as ComboBoxItem).Content as TextBlock).Text;
            using (TestingEntities db = new TestingEntities())
            {
                //заполнение листа ответами
                Question question = db.Questions.FirstOrDefault(q => q.name_question == nameQuestion);
                idQuestion = question.id;
                var answers = db.Answers.Where(a => a.questionId == idQuestion);
                if (answers == null)
                {
                    addListBoxItem("");
                    addListBoxItem("");
                }
                else
                {
                    foreach (Answer item in answers)
                    {
                        addListBoxItem(item.answer_name);
                    }
                }
            }
        }

        private void Check_Checked(object sender, RoutedEventArgs e)
        {
            //установка флажка на удаление ответа
            foreach (var item in (((sender as CheckBox).Parent as StackPanel).Parent as StackPanel).Children)
            {
                if (item is TextBox)
                {
                    deleteAnswer.Add((item as TextBox).Text);
                }
            }
        }

        private void Radio_Checked(object sender, RoutedEventArgs e)
        {
            //установка правильного ответа
            beAnswer = true;
            foreach (var item in (((sender as RadioButton).Parent as StackPanel).Parent as StackPanel).Children)
            {
                if (item is TextBox)
                {
                    correctlyAnswer = (item as TextBox).Text;
                }
            }
        }

        private void btnRedaktorTestov_Click(object sender, RoutedEventArgs e)
        {
            redaktorTestov window = new redaktorTestov();
            window.Show();
            this.Close();
        }

        private void btnAddAnswer_Click(object sender, RoutedEventArgs e)
        {
            addListBoxItem("");
        }
        void addListBoxItem(string text)
        {
            //добавление в список ответа
            ListBoxItem listBoxItem = new ListBoxItem();
            listBoxItem.Margin = new Thickness(5);
            StackPanel stackItem = new StackPanel();
            stackItem.Orientation = Orientation.Horizontal;
            TextBox textBox = new TextBox();
            textBox.MinWidth = 100;
            textBox.MaxWidth = 180;
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.Text = text;
            RadioButton radio = new RadioButton { IsChecked = false, GroupName = "Answer" };
            CheckBox check = new CheckBox();
            check.Checked += Check_Checked;
            check.Unchecked += Check_Unchecked;
            radio.Checked += Radio_Checked;
            StackPanel smallStack = new StackPanel();
            smallStack.Orientation = Orientation.Vertical;
            smallStack.Margin = new Thickness(10);
            smallStack.Children.Add(radio);
            smallStack.Children.Add(check);
            stackItem.Children.Add(textBox);
            stackItem.Children.Add(smallStack);
            listBoxItem.Content = stackItem;
            listBox.Items.Add(listBoxItem);
        }

        private void Check_Unchecked(object sender, RoutedEventArgs e)
        {
            //снятие флажка на удаление ответа
            foreach (var item in (((sender as CheckBox).Parent as StackPanel).Parent as StackPanel).Children)
            {
                if (item is TextBox)
                {
                    deleteAnswer.Remove((item as TextBox).Text);
                }
            }
        }

        private void btnDeleteAnswer_Click(object sender, RoutedEventArgs e)
        {

            if (deleteAnswer.Count == 0)
                return;
            string answer = deleteAnswer.FirstOrDefault(a => a == correctlyAnswer);
            //проверка на удаление правильного ответа
            if (answer != null)
            {
                MessageBox.Show("Вы удаляете ответ, помеченный как правильный.");
                return;
            }
            int countListBoxItem=0;
            foreach (var item in listBox.Items)
            {
                 countListBoxItem++;
                
            }
            //удаление вопросов из листа
            for (int i = 0; i < countListBoxItem; i++)
            {
                foreach (var item in ((listBox.Items[i] as ListBoxItem).Content as StackPanel).Children)
                {
                    if(item is TextBox)
                    {
                        string answerItem = (item as TextBox).Text;
                        answer = deleteAnswer.FirstOrDefault(a => a == answerItem);
                        if (answer != null)
                            listBox.Items.Remove(listBox.Items[i]);
                    }
                }
            }
            //удаление вопросов из базы
            using (TestingEntities db = new TestingEntities())
            {
                foreach (string item in deleteAnswer)
                {
                    Answer ans = db.Answers.FirstOrDefault(a => a.answer_name == item);
                    if (ans != null)
                        db.Answers.Remove(ans);
                }
                db.SaveChanges();
            }
        }

        private void btnSaveAnswers_Click(object sender, RoutedEventArgs e)
        {
            int countListBoxItem = 0;
            foreach (var item in listBox.Items)
            {
                countListBoxItem++;

            }
            //сохранение ответов
            using (TestingEntities db=new TestingEntities())
            {
                string answerItem="";
                bool? isCorrectly=false;
                var answers = db.Answers.Where(a => a.questionId == idQuestion);
                db.Answers.RemoveRange(answers);
                for (int i = 0; i < countListBoxItem; i++)
                {
                    foreach (var item in ((listBox.Items[i] as ListBoxItem).Content as StackPanel).Children)
                    {
                        if (item is TextBox)
                        {
                           answerItem = (item as TextBox).Text;
                        }
                        else if(item is StackPanel)
                        {
                            foreach (var item1 in (item as StackPanel).Children)
                            {
                                if(item1 is RadioButton)
                                {
                                    isCorrectly = (item1 as RadioButton).IsChecked;
                                }
                            }
                        }
                    }
                    Answer newAnswer = new Answer();
                    newAnswer.answer_name = answerItem;
                    newAnswer.questionId = idQuestion;
                    newAnswer.correctly = (isCorrectly == false) ? 0 : 1;
                    db.Answers.Add(newAnswer);
                    db.SaveChanges();
                }
            }
            
        }
    }
}
