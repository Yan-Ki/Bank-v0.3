using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;
using System.Data;
using System.Windows.Controls.Primitives;
using System.Data.SqlClient;
using System.Security.Policy;
using System.Security.Principal;



using System.Data.Entity;
using System.Runtime.Remoting.Contexts;


namespace Bank
{
    public class BankRepository
    {


        public static Random random;
        public static List<int> IdDb;// Коллекция для хранения ID
        public static ObservableCollection<string> Logs { get; set; } //Коллекция для хранения логов

        


        /// <summary>
        /// Статический конструктор
        /// </summary>
        static BankRepository()
        {
            BankRepository.random = new Random();
            BankRepository.IdDb = new List<int>();
        }


        /// <summary>
        /// Конструктор
        /// </summary>
        public BankRepository()
        {
            Logs = new ObservableCollection<string>();

        }


        /// <summary>
        /// Метод добавление нового клиента юзером
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Name"></param>
        /// <param name="Phone"></param>
        public void FillingClient(int AccValue, string Name, string Phone, int IdType, BankDBNewEntities context)

        {
            int Id = random.Next(1000, 9999);
            while (IdDb.Contains(Id))
            {
                Id = random.Next(1000, 9999);
            }

            Client client = new Client();
            client.ClientID = Id;
            client.AccValue = AccValue;
            client.Name = Name;
            client.Phone = Phone;
            client.IdType = IdType;

            context.Client.Add(client);
            context.SaveChanges();

            BankRepository.Logs.Add($"Добавление клиента: ID: {client.Id} {client.Name} ");

        }


    }
}

