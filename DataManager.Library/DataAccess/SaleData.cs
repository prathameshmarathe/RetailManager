﻿using DataManager.Library.Internal.DataAccess;
using DataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataManager.Library.DataAccess
{
    public class SaleData
    {
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            ProductData products = new ProductData();
            var taxRate = ConfigHelper.GetTaxRate()/ 100;
            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                //Get the information about Product
                var productInfo = products.GetProductById(detail.ProductId);

                if (productInfo == null)
                {
                    throw new Exception($"The product Id of {detail.ProductId} could not be found in the database");
                }
                detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;
                if (productInfo.IsTaxable)
                {
                    detail.Tax = detail.PurchasePrice * taxRate;
                }
                details.Add(detail);
            }
                //Create the Sale DB model
                SaleDBModel sale = new SaleDBModel
                {
                    SubTotal = details.Sum(x => x.PurchasePrice),
                    Tax= details.Sum(x => x.Tax),
                    CashierId=cashierId
                };
                sale.Total = sale.SubTotal + sale.Tax;

                //Save the Sale Model
                SqlDataAccess sql = new SqlDataAccess();
                sql.SaveData("dbo.spSale_Insert", sale, "Data");

            //Get the id from sale model
            sale.Id=sql.LoadData<int, dynamic>("spSale_Lookup", new { sale.CashierId,sale.SaleDate }, "Data").FirstOrDefault();

            // Finish filling in the sale detail model
            foreach (var item in details)
            {
                item.SaleId = sale.Id;
                sql.SaveData("dbo.spSaleDetail_Insert", item, "Data");
            }
            







        }

        //public List<ProductModel> GetProducts()
        //{
        //    SqlDataAccess sql = new SqlDataAccess();
        //    var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", new { }, "Data");
        //    return output;
        //}
    }
}
