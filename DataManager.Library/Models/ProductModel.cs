using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.Models
{
    public class ProductModel
    {
        //id, ProductName, [Description],RetailPrice,QuantityInStock
        public int MyProperty { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public decimal RetailPrice { get; set; }

        public int QuantityInStock { get; set; }
    }
}
