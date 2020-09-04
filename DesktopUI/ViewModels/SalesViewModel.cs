﻿using AutoMapper;
using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.Helpers;
using DesktopUI.Library.Models;
using DesktopUI.Models;
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
        IConfigHelper _configHelper;
        ISaleEndpoint _saleEndPoint;
        IMapper _mapper;
        public SalesViewModel(IProductEndPoint productEndPoint,ISaleEndpoint saleEndpoint,IConfigHelper configHelper,IMapper mapper)
        {
            _productEndPoint = productEndPoint;
            _configHelper = configHelper;
            _saleEndPoint = saleEndpoint;
            _mapper = mapper;

        }
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }
        private async Task LoadProducts()
        {
            var productList = await _productEndPoint.GetAll();
            var products=_mapper.Map<List<ProductDisplayModel>>(productList);
            Products = new BindingList<ProductDisplayModel>(products);
        }

        private BindingList<ProductDisplayModel> _products;

        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            set {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private ProductDisplayModel _selectedProduct;

        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        private CartItemDisplayModel _selectedCartItem;

        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
            }
        }

        private BindingList<CartItemDisplayModel> _cart=new BindingList<CartItemDisplayModel>();

        public BindingList<CartItemDisplayModel> Cart
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
                 
                return CalculateSubTotal().ToString("C");
            }
            
            //get { return "$0.00"; }
        }
        private decimal CalculateSubTotal()
        {
            decimal subTotal = 0;
            foreach (var item in Cart)
            {
                subTotal += (item.QuantityInCart) * (item.Product.RetailPrice);
            }
            return subTotal;
        }

        private decimal CalculateTax()
        {
            decimal taxAmount = 0;
            decimal taxRate = _configHelper.GetTaxRate()/100;
            taxAmount = Cart.Where(x => x.Product.IsTaxable)
                .Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);
            //foreach (var item in Cart)
            //{
            //    if (item.Product.IsTaxable)
            //    {
            //        taxAmount += (item.QuantityInCart * item.Product.RetailPrice * taxRate);
            //    }
            //}
            return taxAmount;
        }

        public string Tax
        {
            get
            {
                return CalculateTax().ToString("C");
            }

            
        }
        public string Total
        {
            //TODO - Relace by Calculation
            get
            {
                decimal total = CalculateSubTotal() + CalculateTax();
                return total.ToString("C");
            }
            
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
            CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);
            if(existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantity;
                ////HACK
                //Cart.Remove(existingItem);
                //Cart.Add(existingItem);
            }
            else
            {
                CartItemDisplayModel item = new CartItemDisplayModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);

            }
            SelectedProduct.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(()=> SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);

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

            SelectedCartItem.Product.QuantityInStock += 1;
            if (SelectedCartItem.QuantityInCart>1)
            {
                SelectedCartItem.QuantityInCart -= 1;
            }
            else
            {
                Cart.Remove(SelectedCartItem);
            }
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanRemoveFromCart);
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;
                if(SelectedCartItem!=null && SelectedCartItem?.QuantityInCart>0)
                {
                    output = true;
                }
                return output;
            }

        }

        public async Task CheckOut()
        {
            //Create a Sale Model and post to API
            SaleModel sale = new SaleModel();
            foreach (var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                   Quantity=item.QuantityInCart
                }) ;
            }
            await _saleEndPoint.PostSale(sale);

        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;
                //make sure there is something in the cart
                if (Cart.Count>0)
                {
                    output = true;
                }
                return output;
            }

        }

    }
}
