using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Diagnostics;

using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Windows.Threading;

namespace Bank
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Random random;
        BankDBNewEntities context;
        BankRepository Sber;

        public MainWindow()
        {
            InitializeComponent();
            Sber = new BankRepository();
            random = new Random();
            context = new BankDBNewEntities();
            context.Client.Load();
            DataGridClients.ItemsSource = context.Client.Local.ToBindingList<Client>();
            ComboBoxClients.ItemsSource = context.Client.Local.ToBindingList<Client>();

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            Thread thread = new Thread(new ParameterizedThreadStart(Filling));
            thread.Start(dispatcher);

            ListBoxLog.ItemsSource = BankRepository.Logs;


        }
        /// <summary>
        /// Метод наполнения клиентами
        /// </summary>
        /// <param name="dispatcher"></param>
        private void Filling(object dispatcher)
        {
            Dispatcher dispatcher2 = dispatcher as Dispatcher;


            for (int i = 1; i <= 5; i++)
            {
                int Id = random.Next(1000, 9999);
                while (BankRepository.IdDb.Contains(Id))
                {
                    Id = random.Next(1000, 9999);
                }
                switch (random.Next(1, 4))
                {

                    case 1:
                        dispatcher2.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            Sber.FillingClient(random.Next(1, 9) * 1000, $"{Id}_Иванов", $"+7_927_{random.Next(100, 999)}_{random.Next(1000, 9999)}", 1, context);

                        }));

                        break;

                    case 2:
                        dispatcher2.Invoke(new Action(() =>
                        {
                            Sber.FillingClient(random.Next(1, 9) * 1000, $"{Id}_СМИРНОВ_VIP", $"+7_927_{random.Next(100, 999)}_{random.Next(1000, 9999)}", 2, context);


                        }));

                        break;
                    case 3:
                        dispatcher2.Invoke(new Action(() =>
                        {
                            Sber.FillingClient(random.Next(1, 9) * 1000, $"{Id}_ШАРАГА", $"8_{random.Next(10, 99) * 10}_{random.Next(100000, 999999)}", 3, context);


                        }));

                        break;

                }

                Thread.Sleep(1000);
            }



        }

        /// <summary>
        /// Добавление клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAddClient_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (string.IsNullOrEmpty(TexBoxAccValue.Text) || string.IsNullOrEmpty(TextBoxName.Text) || string.IsNullOrEmpty(TextBoxPhone.Text) ||
                    string.IsNullOrEmpty(ComboBoxStatus.Text)) throw new MyException();

                if (TextBoxName.Text.IsNumberContains()) throw new MyException(TextBoxName.Text, 1);
                if (!TextBoxPhone.Text.IsNumberContains()) throw new MyException(TextBoxPhone.Text, 2);

                int AccValue = Convert.ToInt32(TexBoxAccValue.Text);

                if (AccValue < 0) throw new FormatException();
                int IdType = 0;
                switch (ComboBoxStatus.Text)
                {
                    case "Физическое лицо": IdType = 1; break;
                    case "Физическое лицо(VIP)": IdType = 2; break;
                    case "Юридическое лицо": IdType = 3; break;
                }
                Sber.FillingClient(AccValue, TextBoxName.Text, TextBoxPhone.Text, IdType, context);
            }

            catch (MyException error) when (error.Code == 2)
            {
                MessageBox.Show($"Ошибка в телефоне клиента ");
                Debug.WriteLine(error);
            }
            catch (MyException error) when (error.Code == 1)
            {
                MessageBox.Show($"Ошибка в имени клиента {error.Message}");
                Debug.WriteLine(error);
            }
            catch (FormatException error)
            {
                MessageBox.Show($"Ошибка ввода счёта клиента");
                Debug.WriteLine(error);
            }
            catch (MyException error)
            {
                MessageBox.Show($"Заполните все строки");
                Debug.WriteLine(error);
            }
            catch (Exception error)
            {
                MessageBox.Show($"Ошибка ввода");
                Debug.WriteLine(error);
            }



        }

        /// <summary>
        /// Вклад
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOpenDeposit_Click(object sender, RoutedEventArgs e)
        {

                try
                {
                     Client client = (Client)DataGridClients.SelectedItem;
                //Sber.row = (DataRowView)DataGridClients.SelectedItem;
                    if ((string.IsNullOrEmpty(TexBoxMoney.Text)) || !TexBoxMoney.Text.IsNumberContains()) throw new MyException();
                    int money = Convert.ToInt32(TexBoxMoney.Text);
                    int AccValue=client.AccValue;
                // int AccValue=Convert.ToInt32(Sber.row.Row["AccValue"]);
                if (money > AccValue || money <= 0)
                    {
                        MessageBox.Show("Введите сумму вклада в пределах счёта клиента!");

                    }
                    else
                    {
                        double percent;

                        switch (Convert.ToInt32(client.IdType))
                        {
                            case 1: percent = 0.1; break;
                            case 2: percent = 0.05; break;
                            default: percent = 0.12; break;

                        }

                        client.Deposit =(int) ((money * percent) + money);
                        AccValue -= money;
                        client.AccValue =AccValue;
                   // context.Entry(client).State=EntityState.Modified;
                    context.SaveChanges();
                    MessageBox.Show($"Вклад открыт под {percent * 100}% годовых");

                       BankRepository.Logs.Add($"Открытие вклада: ID: {client.Id} ");
                    }

                }
                catch (MyException error)
                {
                    MessageBox.Show($"Неверный формат денежного вклада");
                    Debug.WriteLine(error);
                }
                catch (NullReferenceException error)
                {
                    MessageBox.Show($"Выберите клиента");
                    Debug.WriteLine(error);
                }

        }
        /// <summary>
        /// Перевод
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Client c1 = (Client)DataGridClients.SelectedItem;
                Client c2 = (Client)ComboBoxClients.SelectedItem;


                if ((string.IsNullOrEmpty(TexBoxSend.Text)) || !TexBoxSend.Text.IsNumberContains()) throw new MyException();
                int money = Convert.ToInt32(TexBoxSend.Text);

                c1.AccValue  -= money;
                c2.AccValue += money;
                context.SaveChanges();

                BankRepository.Logs.Add($"Перевод от: ID: {c1.Id} ");
                BankRepository.Logs.Add($"Перевод кому: ID: {c2.Id} ");

            }
            catch (NullReferenceException error)
            {
                MessageBox.Show($"Выберите кому и куда переводить");
                Debug.WriteLine(error);
            }
            catch (MyException error)
            {
                MessageBox.Show($"Неверный формат денежного перевода");
                Debug.WriteLine(error);
            }

        }

        /// <summary>
        /// Начало редактирование записи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void GVCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var client =(Client) DataGridClients.SelectedItem;
            DataGridClients.BeginEdit();
                      
        }

        /// <summary>
        /// Редактирование записи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GVCurrentCellChanged(object sender, EventArgs e)
        {
             if (DataGridClients == null) return;               
             context.SaveChanges();
        }

        /// <summary>
        /// Удаление
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var client = (Client)DataGridClients.SelectedItem;
            context.Client.Remove(client);
            context.SaveChanges();
            BankRepository.Logs.Add($"Удаление клиента: ID: {client.Id} ");
        }

    }
}