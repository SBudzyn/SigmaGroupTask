﻿using GroupProject.DTO;
using Microsoft.EntityFrameworkCore;
using ShopApp.Entities.ProductEntity;
using ShopApp.Interface;
using ShopApp.PaymentService;
using ShopApp.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp
{
    public class StorageManagerConsole : IStorageManager
    {
        private ICRUDStorage storageCRUD;
        private IProxyProdFabric prodFabric;

        public StorageManagerConsole(ICRUDStorage storageCRUD, IProxyProdFabric prodFabric)
        {
            this.storageCRUD = storageCRUD;
            this.prodFabric = prodFabric;
        }
        public void ShowMenu()
        {
            Console.WriteLine("1 - Добавити продукт");
            Console.WriteLine("2 - Видалити продукт");
            Console.WriteLine("3 - Оновити продукт");
            Console.WriteLine("4 - Показати продукти");
            Console.WriteLine("5 - Вихід");

            int menuCount;
            do
            {
                Console.WriteLine("Введіть ваш вибір: ");
                if (!int.TryParse(Console.ReadLine(), out menuCount))
                {
                    Console.WriteLine("Потрібно ввести хоть якесь число!");
                    continue;
                }
                else
                if (menuCount < 1 || menuCount > 5)
                {
                    Console.WriteLine("Введіть дані числа меню!");
                }
            }
            while (menuCount < 1 || menuCount > 5);

            switch (menuCount)
            {
                case 1:
                    {
                        CreateProduct();
                        break;
                    }
                case 2:
                    {
                        DeleteProduct();
                        break;
                    }
                case 3:
                    {
                        ReadProduct();
                        break;
                    }
                case 4:
                    {
                        UpdateProduct();
                        break;
                    }
                case 5:
                    {
                        Console.WriteLine("Вихід.");
                        break;
                    }
                default:
                    throw
                        new Exception("Помилка меню.");
            }
        }
 
        public void CreateProduct()
        {
            throw new NotImplementedException();
        }

        public void DeleteProduct()
        {
            throw new NotImplementedException();
        }

        public void ReadProduct()
        {
            throw new NotImplementedException();
        }
        public void UpdateProduct()
        {
            throw new NotImplementedException();
        }
    }
}
