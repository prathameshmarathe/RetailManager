using Caliburn.Micro;
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

        private BindingList<ProductModel> _cart;

        public BindingList<ProductModel> Cart
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
            //TODO - Relace by Calculation
            get { return "$0.00"; }
        }

        public string Tax
        {
            //TODO - Relace by Calculation
            get { return "$0.00"; }
        }
        public string Total
        {
            //TODO - Relace by Calculation
            get { return "$0.00"; }
        }


        private int _itemQuantity;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set { 
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
            }
        }

        public void AddToCart()
        {

        }

        public bool CanAddToCart
        {
            get
            {
                bool output = false;
                //make sure something is selected
                //make sure there is an item quantity
                return output;
            }

        }

        public void RemoveFromCart()
        {

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
