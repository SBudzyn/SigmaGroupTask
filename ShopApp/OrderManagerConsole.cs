﻿using ShopApp.DTO.Enums;
using ShopApp.Entities.OrderEntity;
using ShopApp.Entities.OrderItemEntity;
using ShopApp.Entities.ProductEntity;
using ShopApp.Entities.UserEntity;
using ShopApp.Interface;
using ShopApp.Repositories;
using ShopApp.Repositories.Interfaces;

namespace ShopApp
{
    public class OrderManagerConsole : IOrderManager
    {
        protected readonly ICRUDOrders crudOrder;
        protected readonly IReadStorage readStorage;
        protected readonly IUserRepository userRepository;

        public OrderManagerConsole(ICRUDOrders crudOrder, IReadStorage readStorage, IUserRepository userRepository)
        {
            this.crudOrder = crudOrder;
            this.readStorage = readStorage;
            this.userRepository = userRepository;
        }

        public async Task CreateOrder()
        {
            var orderToCreate = new Order();
            User? user = null;
            Console.Write("Введiть id користувача: ");
            if (Int32.TryParse(Console.ReadLine(), out var userId))
            {
                user = await userRepository.GetById(userId);
                if (user is null)
                {
                    Console.WriteLine("Користувача не знайдено");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Користувача не знайдено");
                return;
            }

            Console.WriteLine("Створення замовлення...");

            var allProducts = await readStorage.ReadProducts();
            Console.WriteLine("Список всiх продуктiв: ");
            foreach (var item in allProducts)
            {
                Console.WriteLine(ToStringProduct(item));
            }

            Console.WriteLine("Виберiть необхiднi продукти (Vendor code)");
            var userProductsVendorCode = Console.ReadLine();

            if (userProductsVendorCode == null || userProductsVendorCode.Length == 0 || userProductsVendorCode == "")
            {
                Console.WriteLine("Жоден продукт не вибрано");
                return;
            }

            var productsVendorCode = userProductsVendorCode?.Split(',', ' ');
            int[] vendorCodeArr = new int[] { };
            if (productsVendorCode == null || productsVendorCode.Length == 0)
            {
                Console.WriteLine("Жоден продукт не вибрано");
                return;
            }
            else
            {
                vendorCodeArr = Array.ConvertAll(productsVendorCode, s => int.TryParse(s, out var x) ? x : -1);
            }

            foreach (var item in vendorCodeArr)
            {
                var productById = await readStorage.FindProductsById(item);
                if (productById != null)
                {
                    var orderItem = new OrderItem()
                    {
                        ProductVendorCode = productById.VendorCode,
                        PriceWithSale = productById.Price
                    };

                    Console.Write($"Введiть к-ть {productById.Name}: ");
                    var isSuccess = int.TryParse(Console.ReadLine(), out var productCount);
                    orderItem.Amount = isSuccess ? productCount : 1;

                    orderToCreate.OrderItems.Add(orderItem);
                    Console.WriteLine($"Продукт: {item} успiшно додано");
                }
                else
                {
                    Console.WriteLine($"Продукт: {item} не знайдено");
                }
            }

            Console.Write("Введiть опис замовлення: ");
            orderToCreate.Description = Console.ReadLine();
            orderToCreate.UserId = user.Id;
            orderToCreate.AddressId = user.AddressId;
            orderToCreate.StatusId = (int)OrderStatuses.Pending;
            orderToCreate.OrderedAt = DateTime.Now;

            try
            {
                var createdOrder = await crudOrder.CreateOrder(orderToCreate);
            }
            catch(NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            Console.WriteLine("Замовлення успiшно додано");
        }
        public async Task ReadOrders()
        {
            var orders = await crudOrder.GetAll();

            foreach (var item in orders)
            {
                Console.WriteLine(ToStringOrder(item));
            }
        }
        public async Task UpdateOrder()
        {
            Order? order = null;

            Console.WriteLine("Введiть код замовлення");
            if (int.TryParse(Console.ReadLine(), out var indexOrder))
            {
                order = await crudOrder.GetById(indexOrder);
            }

            if (order == null)
            {
                Console.WriteLine("Замовлення не знайдено");
                return;
            }

            Console.Write("Введiть новий опис: ");
            order.Description = Console.ReadLine();

            foreach (var item in order.OrderItems)
            {
                Console.WriteLine("Введiть нову к-ть товару");
                Console.WriteLine($"{item.ProductVendorCode} {item.Product.Name} - {item.Amount}");

                if (int.TryParse(Console.ReadLine(), out var newProductAmount))
                {
                    item.Amount = newProductAmount;
                }
                else
                {
                    Console.WriteLine("Некоректне значення к-тi товару");
                    Console.WriteLine("Замовлення не оновлено");
                    return;
                }
            }

            try
            {
                await crudOrder.UpdateOrder(order);
            }
            catch(ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            catch(NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            Console.WriteLine("Замовлення успiшно оновлено");
        }

        public async Task DeleteOrder()
        {
            Console.WriteLine("Введiть код замовлення");

            if (int.TryParse(Console.ReadLine(), out var indexOrder))
            {
                try
                {
                    var deletedOrder = await crudOrder.DeleteOrder(indexOrder);
                }
                catch(ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
            else
            {
                Console.WriteLine("Замовлення не знайдено");
                return;
            }
            Console.WriteLine("Замовлення успiшно видалено");
        }


        public async Task ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("1 - Додати замовлення");
                Console.WriteLine("2 - Показати всi замовлення");
                Console.WriteLine("3 - Оновити замовлення");
                Console.WriteLine("4 - Видалити замовлення");
                Console.WriteLine("5 - Вихiд");
                int choose;
                
                try
                {
                    choose = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                    choose = 0;
                }

                switch (choose)
                {
                    case 1:
                    {
                        Console.Clear();
                        await CreateOrder();
                        break;
                    }
                    case 2:
                    {
                        Console.Clear();
                        await ReadOrders();
                        break;
                    }
                    case 3:
                    {
                        Console.Clear();
                        await UpdateOrder();
                        break;
                    }
                    case 4:
                    {
                        Console.Clear();
                        await DeleteOrder();
                        break;
                    }
                    case 5:
                    {
                        Console.Clear();
                        Console.WriteLine("Вихiд.");
                        return;
                    }
                    default:
                    {
                        Console.Clear();
                        Console.WriteLine("Введіть число зі списку.");
                        break;
                    }
                }
            }
        }

        private string ToStringProduct(Product product)
        {
            string result = "";
            result += "VendorCode:" + product.VendorCode + " | ";
            result += product.ProdType + " | ";
            result += product.Name + " | ";
            result += product.Description != null ? product.Description + " | " : "";
            result += product.Amount + " | ";
            result += product.WeightUnit + " | ";
            result += product.Weight.ToString() + " | ";
            result += product.Weight + " | ";
            result += product.MeatSort != null ? product.MeatSort.ToString() + " | " : "";
            result += product.MeatType != null ? product.MeatType.ToString() + " | " : "";
            result += product.ExpiryDate != null ? product.ExpiryDate.ToString() + " | " : "";
            result += product.Currency + " | ";
            return result;
        }
        private string ToStringOrder(Order order)
        {
            string result = "";
            result += order.Id + " | ";
            result += order.Description != null ? order.Description + " | " : "";
            result += order.AddressId + " | ";
            result += order.StatusId + " | ";
            result += order.UserId + " | ";

            return result;
        }
    }
}