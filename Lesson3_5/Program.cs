using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson3_5
{
    class Program
    {
        static void Main(string[] args)
        {
            var banks = GenerateBanks();
            var users = GenerateUsers(banks);



            var usersname12 = users.Where(u => u.LastName.ToCharArray().Length + u.FirstName.ToCharArray().Length >= 12);//1)

            var usersname12linq = from u in users
                                  where u.LastName.ToCharArray().Length + u.FirstName.ToCharArray().Length >= 12//1) LINQ
                                  select u;

            var cheek12 = usersname12.SequenceEqual(usersname12linq); //Cheek for 1)



            var usersTransactions = users.Where(u => (u.Transactions?.Any()).GetValueOrDefault()).
                SelectMany(u => u.Transactions).ToList();//2)

            var usersTransactionslinq = from u in users
                                        from x in u.Transactions
                                        select x;//2) LINQ

            var cheekTransactions = usersTransactionslinq.ToList().SequenceEqual(usersTransactions);//Cheek for 2)



            var maxTrans = users.Max(u => u.Bank.Transactions.Count);
            var mostTransInBank = users.Where(u => u.Bank.Transactions.Count == maxTrans);
            var adminInMostTransBank = mostTransInBank.Where(u => u.Type == UserType.Admin); //4)
            //or
            var adminInMostTransBankEquals = users.Where(u => u.Bank.Transactions.Count == maxTrans && u.Type == UserType.Admin);

            var mostTransInBanklinq = from b in users
                                      where b.Bank.Transactions.Count == users.Max(u => u.Bank.Transactions.Count)
                                      select b;

            var adminInMostTransBanklinq = from u in mostTransInBanklinq
                                           where u.Type == UserType.Admin
                                           select u;//4) LINQ

            var cheekMostBankTrans = adminInMostTransBank.SequenceEqual(adminInMostTransBanklinq);//Cheek for 4)



            var usersByDescend = mostTransInBank.OrderByDescending(u => u.LastName);//3) Bank with most transactions

            var usersByDescendlinq = from u in mostTransInBanklinq
                                     orderby u.LastName descending
                                     select u;//3)LINQ

            var cheekUsersbyDescending = usersByDescendlinq.SequenceEqual(usersByDescend);//Cheek for 3)



            var usersWithoutAdmin = users.Where(u => u.Type != UserType.Admin);

            var uahTransactions = usersWithoutAdmin.Select(x => x.Transactions.Where(y => y.Currency == Currency.UAH));
            var usdTransactions = usersWithoutAdmin.Select(x => x.Transactions.Where(y => y.Currency == Currency.USD));
            var eurTransactions = usersWithoutAdmin.Select(x => x.Transactions.Where(y => y.Currency == Currency.EUR));
            var maxUAHCount = uahTransactions.OrderByDescending(x => x.Count()).First(); // UAH
            var maxUSDCount = usdTransactions.OrderByDescending(x => x.Count()).First(); // USD
            var maxEURCount = eurTransactions.OrderByDescending(x => x.Count()).First(); // EUR

            var usersAll = usersWithoutAdmin.Where(u => u.Transactions.Contains(maxUAHCount.First()) || u.Transactions.Contains(maxUSDCount.First())
            || u.Transactions.Contains(maxEURCount.First())).Select(i => i);//5)

            var uahTransactionslinq = from u in usersWithoutAdmin
                                      select u.Transactions.Where(y => y.Currency == Currency.UAH);
            var usdTransactionslinq = from u in usersWithoutAdmin
                                      select u.Transactions.Where(y => y.Currency == Currency.USD);
            var eurTransactionslinq = from u in usersWithoutAdmin
                                      select u.Transactions.Where(y => y.Currency == Currency.EUR);

            var maxUAHCountlinq = from u in uahTransactionslinq
                                  orderby u.Count() descending
                                  select u.First();
            var maxUSDCountlinq = from u in usdTransactionslinq
                                  orderby u.Count() descending
                                  select u.First();
            var maxEURCountlinq = from u in eurTransactionslinq
                                  orderby u.Count() descending
                                  select u.First();
            var usersAlllinq = from u in usersWithoutAdmin
                               where u.Transactions.Contains(maxUAHCountlinq.First()) || u.Transactions.Contains(maxUSDCountlinq.First()) || u.Transactions.Contains(maxEURCountlinq.First())
                               select u;//5)LINQ

            var cheekforAll = usersAlllinq.SequenceEqual(usersAll);//Cheek for 5)



            var usersPremium = users.Where(u => u.Type == UserType.Premium);
            var bankUserPrem = usersPremium.Max(u => u.Bank.Id);
            var bankPremUsers = users.Where(u => u.Bank.Id == bankUserPrem);
            var bankPremTrans = bankPremUsers.SelectMany(u => u.Transactions).ToList();//6)

            var usersPremiumlinq = from u in users
                                    where u.Type == UserType.Premium
                                    select u;
            var bankUserPremlinq = from u in usersPremiumlinq
                                   select u.Bank.Id;
            var bankUsermax = bankUserPremlinq.Max();
            var bankPremTranslinq = from u in users
                                    where u.Bank.Id == bankUsermax
                                    from t in u.Transactions
                                    select t;

            var cheekprem = bankPremTranslinq.SequenceEqual(bankPremTrans);

            //+1) Сделать выборку всех Пользователей, имя + фамилия которых длиннее чем 12 символов.

            //+2) Сделать выборку всех транзакций (в результате должен получится список из 1000 транзакций)

            //+3) Вывести Банк: и всех его пользователей (Имя + фамилия + количество транзакий в гривне) отсортированных по Фамилии по убиванию. в таком виде :
            //   Имя банка 
            //   ***************
            //   Игорь Сердюк 
            //   Николай Басков

            //+4) Сделать выборку всех Пользователей типа Admin, у которых счет в банке, в котором больше всего транзакций

            //+5) Найти Пользователей(НЕ АДМИНОВ), которые произвели больше всего транзакций в определенной из валют (UAH,USD,EUR) 
            //то есть найти трёх пользователей: 1й который произвел больше всего транзакций в гривне, второй пользователь, который произвел больше всего транзакций в USD 
            //и третьего в EUR

            //+6) Сделать выборку транзакций банка, у которого больше всего Premium пользователей

        }

        public class User
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public List<Transaction> Transactions { get; set; }
            public UserType Type { get; set; }
            public Bank Bank { get; set; }
        }

        public class Transaction
        {
            public decimal Value { get; set; }
            public Currency Currency { get; set; }
        }

        public static List<Transaction> GetTenTransactions()
        {
            var result = new List<Transaction>();
            var sign = random.Next(0, 1);
            var signValue = sign == 0 ? -1 : 1;
            for (var i = 0; i < 10; i++)
            {
                result.Add(new Transaction
                {
                    Value = (decimal)random.NextDouble() * signValue * 100m,
                    Currency = GetRandomCurrency(),
                });
            }
            return result;
        }

        public class Bank
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<Transaction> Transactions { get; set; }
        }

        public enum UserType
        {
            Default = 1,
            Premium = 2,
            Admin = 3
        }

        public static UserType GetRandomUserType()
        {
            int userTypeInt = random.Next(1, 4);
            return (UserType)userTypeInt;
        }

        public enum Currency
        {
            USD = 1,
            UAH = 2,
            EUR = 3
        }

        public static Currency GetRandomCurrency()
        {
            int userTypeInt = random.Next(1, 4);
            return (Currency)userTypeInt;
        }

        public static List<Bank> GenerateBanks()
        {
            var banksCount = random.Next(BANKS_MIN, BANKS_MAX);
            var result = new List<Bank>();

            for (int i = 0; i < banksCount; i++)
            {
                result.Add(new Bank
                {
                    Id = i + 1,
                    Name = RandomString(random.Next(NAME_MIN_LENGTH, NAME_MAX_LENGTH)),
                    Transactions = new List<Transaction>()
                });
            }
            return result;
        }

        public static List<User> GenerateUsers(List<Bank> banks)
        {
            var result = new List<User>();
            int bankId = 0;
            Bank bank = null;
            List<Transaction> transactions = null;
            for (int i = 0; i < 100; i++)
            {
                bankId = random.Next(0, banks.Count);
                bank = banks[bankId];
                transactions = GetTenTransactions();
                result.Add(new User
                {
                    Bank = bank,
                    FirstName = RandomString(random.Next(NAME_MIN_LENGTH, NAME_MAX_LENGTH)),
                    Id = i + 1,
                    LastName = RandomString(random.Next(NAME_MIN_LENGTH, NAME_MAX_LENGTH)),
                    Type = GetRandomUserType(),
                    Transactions = transactions
                });
                bank.Transactions.AddRange(transactions);
            }
            return result;
        }

        private const int BANKS_MIN = 2;
        private const int BANKS_MAX = 5;

        private const int NAME_MAX_LENGTH = 10;
        private const int NAME_MIN_LENGTH = 4;

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    //users.Transactions.Where(x => x.Currency == Currency.UAH).Count descending
}
