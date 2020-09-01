﻿using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.ViewModels
{
    public class SalesViewModel:Screen
    {
        IProductEndPoint _productEndPoint;
        public SalesViewModel(IProductEndPoint productEndPoint)
        {
            _productEndPoint = productEndPoint;
           

        }
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }
        private async Task LoadProducts()
        {
            var productList = await _productEndPoint.GetAll();
            Products = new BindingList<ProductModel>(productList);
        }

        private BindingList<ProductModel> _products;

        public BindingList<ProductModel> Products
        {
            get { return _products; }
            set {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private ProductModel _selectedProduct;

        public ProductModel SelectedProduct
        {
            get { return _selectedProduct; }
            set {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        private BindingList<CartItemModel> _cart=new BindingList<CartItemModel>();

        public BindingList<CartItemModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }


        public string SubTotal
        {
            get
            {
                decimal subTotal = 0;
                foreach (var item in Cart)
                {
                    subTotal +=(item.QuantityInCart)*(item.Product.RetailPrice);
                }
                return subTotal.ToString("C");
            }
            
            //get { return "$0.00"; }
        }

        public string Tax
        {
           
            get { return "$0.00"; }
        }
        public string Total
        {
            //TODO - Relace by Calculation
            get { return "$0.00"; }
        }


        private int _itemQuantity=1;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set { 
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        public void AddToCart()
        {
            CartItemModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);
            if(existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantity;
                //HACK
                Cart.Remove(existingItem);
                Cart.Add(existingItem);
            }
            else
            {
                CartItemModel item = new CartItemModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);

            }
            SelectedProduct.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(()=> SubTotal);
      
        }

        public bool CanAddToCart
        {
            get
            {
                bool output = false;
                //make sure something is selected
                //make sure there is an item quantity
                if(ItemQuantity>0 && SelectedProduct?.QuantityInStock>=ItemQuantity)
                {
                    output = true;
                }
                return output;
            }

        }

        public void RemoveFromCart()
        {
            NotifyOfPropertyChange(() => SubTotal); 
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;
                //make sure something is selected
                return output;
            }

        }

        public void CheckOut()
        {

        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;
                //make sure there is something in the cart
                return output;
            }

        }

    }
}