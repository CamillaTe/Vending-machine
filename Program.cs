using System;
using System.Collections.Generic;
using System.Linq;

namespace Vending_machine
{
    class Program
    {
        static readonly int[] moneyDen = { 1000, 500, 200, 100, 50, 20, 10, 5, 2, 1 };
        static Dictionary<int, Product> productList = new();
        static List<Product> purchaseList = new();

        static void Main(string[] args)
        {
            CreateProductList();

            double balance = 0;
            bool swEnd = false;
            bool swMoreMoney;
            bool swSelect;
            bool swInfo;
            int iSelect;

            balance = InputOfMoney((int)balance);
            Console.WriteLine($"Totalt inmatat {balance} kr.");
            ListMenu();
            while (!swEnd)
            {
                (swEnd, swMoreMoney, swInfo, swSelect, iSelect) = InputAction(); // Välj vad som ska hända

                if (swSelect) // Man har valt en produkt
                {
                    productList.TryGetValue(iSelect, out Product p1);
                    if (swInfo) // Vill ha mera info
                    {
                        Console.WriteLine(p1.ExamineLong());
                    }
                    else // Har valt varan
                    {
                        if (p1.Price > balance)
                        {
                            Console.WriteLine("Du har inte råd med det");
                            Console.WriteLine($"Återstår {balance} kronor.");
                            Console.WriteLine("Vill du ha något annat?");
                            Console.WriteLine("Eller ange 'M' för att mata in mer pengar.");
                        }
                        else
                        {
                            p1.Deliver();
                            purchaseList.Add(p1);
                            balance -= p1.Price;
                            Console.WriteLine($"Återstår {balance} kr.");
                            Console.WriteLine("Vill du ha något mer? ('N' eller varans nummer)");
                        }
                    }
                }
                else if (swMoreMoney)
                {
                    balance = InputOfMoney((int)balance);
                    Console.WriteLine($"Totalt inmatat {balance} kr.");
                    ListMenu();
                }
            }
            Console.WriteLine(MoneyBack(balance));
            Receipt();
            Console.WriteLine("Tack för idag!");
        }

        private static void CreateProductList()
        {
            productList.Add(1, new Food("Kycklingbaugette", 60.00, 355));
            productList.Add(2, new Food("Tonfisksandwich", 35.00, 250));
            productList.Add(3, new Food("Ostfralla", 26.00, 200));
            productList.Add(4, new Sweet("Kexchoklad", 15.00, 100));
            productList.Add(5, new Sweet("Snickers", 8.00, 50));
            productList.Add(6, new Sweet("Sesamkakor", 7.00, 40));
            productList.Add(7, new Sweet("Rostade kikärtor", 23.00, 110));
            productList.Add(8, new Drink("Lingondricka", 14.00, 0.25));
            productList.Add(9, new Drink("Citrondricka", 14.00, 0.25));
            productList.Add(10, new Drink("Coca Cola liten", 14.00, 0.25));
            productList.Add(11, new Drink("Coca Cola stor", 22.00, 0.50));
        }
        private static int InputOfMoney(int balance)
        {
            int inputCoin;
            bool swEnd = false;

            while (!swEnd)
            {
                inputCoin = InputCoin();
                if (inputCoin == 0)
                {
                    swEnd = true;
                }
                else
                {
                    balance += inputCoin;
                }
            }
            return balance;
        }
        private static int InputCoin()
        {
            string input;
            int number;

            Console.Write($"Stoppa in sedel eller mynt ('0' när du är klar): ");
            input = Console.ReadLine().ToUpper();
            if (input == "" | input == "0")
                number = 0;
            else
                while (!int.TryParse(input, out number) || (!moneyDen.Contains(number)))
                {
                    int i = moneyDen.Length - 1;
                    string text = "";
                    while (i > 0)
                    {
                        text += $"{moneyDen[i]}";
                        if (i != 1) text += ", ";
                        i--;
                    }
                    text += $" eller {moneyDen[0]}";
                    Console.Write($"Felaktigt värde. Ange {text}: ");
                    input = Console.ReadLine();
                }
            return number;
        }
        private static void ListMenu()
        {
            foreach (var p in productList)
            {
                Console.WriteLine($" {p.Key,3} - {p.Value.ExamineShort()}");
            }
            Console.WriteLine("Välj önskad vara genom att ange dess nummer. ");
            Console.WriteLine("För närmare info om en vara ange I följt av varans nummer.");
            Console.WriteLine("När du inte önskar fler varor, ange 'N'.");
        }
        private static (bool, bool, bool, bool, int) InputAction()
        {
            string input1;
            bool swEnd = false;
            bool swMoreMoney = false;
            bool swSelect = false;
            bool swInfo = false;
            int iSelect = 0;
            input1 = Console.ReadLine().ToUpper();
            if (input1 == "" | input1 == "J") // Om man svarar med bara Enter så visas menyn igen
            {
                ListMenu();
            }
            else if (input1 == "M") // Vill mata in mer pengar
            {
                swMoreMoney = true;
            }
            else if (input1 == "N") // Klart
            {
                swEnd = true;
            }
            else // siffra eller I+siffra (förhoppningsvis)
            {
                if (input1.Substring(0, 1) == "I")
                {
                    swInfo = true;
                    input1 = input1[1..];
                }

                if (int.TryParse(input1, out iSelect) & (productList.ContainsKey(iSelect)))
                {
                    swSelect = true;
                }
                else
                {
                    Console.WriteLine("Felaktigt val");
                    ListMenu();
                }
            }
            return (swEnd, swMoreMoney, swInfo, swSelect, iSelect);
        }

        private static string MoneyBack(double balance)
        {
            Console.WriteLine($"Tillbaka: {balance} kr");
            int remaining = (int)balance;
            int rest;
            int quota;
            int i = 0;
            string text = "";
            while (remaining > 0 & i < moneyDen.Length)
            {
                quota = remaining / moneyDen[i];
                rest = remaining % moneyDen[i];
                if (quota > 0)
                {
                    text += $"{quota} * {moneyDen[i]} kr";
                    if (rest != 0) text += " + ";
                }
                remaining = rest;
                i++;
            }
            return text;
        }
        private static void Receipt()
        {
            double sum = 0; ;
            Console.WriteLine("\nKvitto");
            Console.WriteLine(DateTime.Now);
            foreach (var p1 in purchaseList)
            {
                Console.WriteLine($"{p1.Name,-16} {p1.Price,4} kr");
                sum += p1.Price;
            }
            Console.WriteLine($"Totalt           {sum,4} kr");
        }
    }

    abstract class Product
    {
        string name;
        double price;
        int productType;

        public string Name { get => name; set => name = value; }
        public double Price { get => price; set => price = value; }
        public int ProductType { get => productType; set => productType = value; }

        public string ExamineShort()
        {
            return $"{name,-16} {price,4} kr";
        }
        public abstract string ExamineLong();
        public virtual void Deliver()
        {
            Console.WriteLine("Varsågod, här är din vara.");
        }

        public override string ToString()
        {
            return $"Typ {productType}, Beteckning {name}, Pris {price} kr";
        }
    }
    class Drink : Product
    {
        double volume;
        public Drink(string name, double price, double volume)
        {
            Name = name;
            ProductType = 3;
            Price = price;
            Volume = volume;
        }

        public double Volume { get => volume; set => volume = value; }

        public override string ExamineLong()
        {
            return $"{Name} {Volume} l {Price,4} kr.";
        }
        public override void Deliver()
        {
            Console.WriteLine("Varsågod och drick!");
        }
        public override string ToString()
        {
            return $"Dryck , Beteckning {Name}, Volym {volume}, Pris {Price} kr";
        }
    }
    class Food : Product
    {
        double weight;
        public Food(string name, double price, double weight)
        {
            Name = name;
            ProductType = 1;
            Price = price;
            Weight = weight;
        }

        public double Weight { get => weight; set => weight = value; }

        public override string ExamineLong()
        {
            return $"{Name} {Weight} g {Price,4} kr.";
        }
        public override string ToString()
        {
            return $"Mat , Beteckning {Name}, Vikt {weight}, Pris {Price} kr";
        }
    }
    class Sweet : Product
    {
        double weight;
        public Sweet(string name, double price, double weight)
        {
            Name = name;
            ProductType = 1;
            Price = price;
            Weight = weight;
        }

        public double Weight { get => weight; set => weight = value; }

        public override string ExamineLong()
        {
            return $"{Name} {weight} g {Price,4} kr.";
        }
        public override void Deliver()
        {
            Console.WriteLine("Varsågod och snaska!");
        }
        public override string ToString()
        {
            return $"Godis , Beteckning {Name}, Vikt {weight}, Pris {Price} kr";
        }
    }
}
