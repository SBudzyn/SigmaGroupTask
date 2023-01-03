﻿using ShopApp.Entities.OrderEntity;
using ShopApp.Entities.OrderItemEntity;
using ShopApp.Entities.ProductEntity;
using ShopApp.Interface;
using ShopApp.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.UI
{
    internal class UserConsole
    {
        private IReadStorage storage;
        private IUserOrder orderService;
        private IProxyPay payment;

        public Order CurrentOrder { get; set; }

        public UserConsole(IReadStorage storage, IUserOrder orderService, IProxyPay payment, Order currentOrder)
        {
            this.storage = storage;
            this.orderService = orderService;
            this.payment = payment;
            CurrentOrder = currentOrder;
        }
        public void ShowMenu()
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine(">>> Вiтаємо у нашому магазинi та бажаємо приємних покупок! <<<");
            Console.ResetColor();
            while (true)
            {
                Console.WriteLine("Оберiть номер пункта меню:".Replace('і', 'i'));
                Console.WriteLine("1. Вивести весь список продуктів.".Replace('і', 'i'));
                Console.WriteLine("2. Знайти продукт за номером.");
                Console.WriteLine("3. Показати мої замовлення.");
                Console.WriteLine("4. Оновити поточне замовлення.");
                Console.WriteLine("5. Підтвердити поточне замовлення.".Replace('і', 'i'));
                Console.WriteLine("6. Сплатити поточне замовлення.");
                Console.WriteLine("7. Вихiд.");
                int choose=Convert.ToInt32(Console.ReadLine());
                switch (choose)
                { 
                    case 1:
                        Console.Clear();
                        ShowListOfProducts();
                        break;
                    case 2:
                        Console.Clear();
                        Console.WriteLine("Введiть номер продукта для пошуку:");
                        int productNumber=Convert.ToInt32(Console.ReadLine());
                        FindProductsById(productNumber);
                        break;
                    case 3:
                        Console.Clear();
                        GetMyOrders();
                        break;
                    case 4:
                        Console.Clear();
                        Console.WriteLine("Введiть номер продукту, який бажаєте додати до замовлення:");
                        productNumber=Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Введiть кiлькiсть продуктiв, який бажаєте додати до замовлення:");
                        int amount=Convert.ToInt32(Console.ReadLine());
                        UpdateCurrentOrder(FindProductsById(productNumber),amount);
                        break;
                    case 5:
                        Console.Clear();
                        ApplyOrder();
                        break;
                    case 6:
                        Console.Clear();
                        ApplyPayment();
                        break;
                    case 7:
                        Console.Clear();
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.WriteLine(">>> Дякуємо, що завiтали до нас! <<<");
                        Console.ResetColor();
                        return;
                    default:
                        Console.WriteLine("Введіть число зі списку.");
                        break;
                }

            }

        }
        public void GetMyOrders()
        {
            try
            {
                //TODO change user ID
                var myOrders = orderService.GetAll().Result.Where(x => x.UserId == 0);
                foreach (var item in myOrders)
                {
                    decimal totalPrice = 0;
                    Console.WriteLine($"Дата замовлення:{item.OrderedAt}");
                    foreach (var orderItem in item.OrderItems)
                    {
                        Console.WriteLine($"{orderItem.Product.Name}............{orderItem.Amount} шт. {orderItem.PriceWithSale}");
                        totalPrice += orderItem.PriceWithSale;
                    }
                    Console.WriteLine($"Вартість: {totalPrice}");
                    Console.WriteLine(new String('=', 50));
                }
            }
            catch(Exception)
            {
                Console.WriteLine("У Вас не має створених замовлень.");
            }
        }

        public void ShowListOfProducts()
        {
            try
            {
                var products = storage.ReadProducts().Result;
                foreach (var product in products)
                {
                    ShowProduct(product);
                    Console.WriteLine(new string('-', 30));
                }
            }
            catch(NullReferenceException)
            {
                Console.WriteLine("Продуктiв не знайдено.");
            }
        }

        public Product? FindProductsById(int id)
        {
            try
            {
                var product = storage.FindProductsById(id).Result;
                if (product is null)
                {
                    Console.WriteLine("Продуктiв з таким номером не знайдено.");
                }
                else
                {
                    ShowProduct(product);
                    Console.WriteLine(new string('-', 30));
                }
                return product;
            }
            catch(Exception) 
            {
                Console.WriteLine("Продукт з таким номером не знайдено.");
            }
            return null;
        }
       
        private void ShowProduct(Product product)
        {
            Console.WriteLine($"{product.Name}:\nОпис: {product.Description}");
            /*Console.WriteLine($"Виробник: {product.Manufacter.Title}");*/
        }

        public void UpdateCurrentOrder(Product product, int amount)
        {
            if(product is null || amount<=0)
            {
                Console.WriteLine("Iнформацiя про товар або кiлькi сть введено неправильно.");
                return;
            }
            if(CurrentOrder is null)
            {
                CurrentOrder = new Order();
            }
            var newOrderItem = new OrderItem()
            {
                Id = CurrentOrder.OrderItems.Count == 0 ? 0 : CurrentOrder.OrderItems.Count,
                Amount = amount,
                Product = product,
                ProductVendorCode = product.VendorCode
            };
            CurrentOrder.OrderItems.Add(newOrderItem);

        }

        public void ApplyOrder()
        {
            CurrentOrder.OrderedAt = DateTime.Now;
            if (orderService.CreateOrder(CurrentOrder).Result is not null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Замовлення вдало створено.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Виникли помилки при створеннi замовлення.");
                Console.ResetColor();
            }
        }

        public void ApplyPayment()
        {
            payment.ChoosePaymentSystem();
        }
    }
}
