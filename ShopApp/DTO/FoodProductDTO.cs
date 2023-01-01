﻿using GroupProject.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProject.DTO
{
    public class FoodProductDTO : ProductDTO
    {
        public FoodProductDTO(string name, ProdType prodType, decimal price, Weight weightUnit, double weight, Currency currency, DateOnly expiryDate) 
                                    : base(name, prodType, price, weightUnit, weight, currency)
        {
            ExpiryDate = expiryDate;
        }

        public DateOnly ExpiryDate { get; }
    }
}
