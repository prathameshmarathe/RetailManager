using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using DesktopUI.EventModels;
using DesktopUI.Library.Models;

namespace DesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<Object>,IHandle<LogOnEvent>
    {
        //private LoginViewModel _loginVM;
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private ILoggedInUserModel _user;
        public ShellViewModel(IEventAggregator events, SalesViewModel salesVM, SimpleContainer container, ILoggedInUserModel user)
        {
            _events = events;
           // _loginVM = loginVM;
            _salesVM = salesVM;
            _user = user;

            _events.Subscribe(this);

            //ActivateItem(_container.GetInstance<LoginViewModel>()) ;
            ActivateItem(IoC.Get<LoginViewModel>());
        }

        public void ExitApplication()
        {
            TryClose();
        }

        public void LogOut()
        {
            _user.LogOffUser();
            ActivateItem(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);

        }

        public bool IsLoggedIn
        {
            get
            {
                bool output = false;
                if (string.IsNullOrWhiteSpace(_user.Token)==false)
                {
                    output = true;
                }
                return output;
            }
        }

 

        //public bool IsErrorVisible
        //{
        //    get
        //    {
        //        bool output = false;
        //        if (ErrorMessage?.Length > 0)
        //        {
        //            output = true;
        //        }
        //        return output;
        //    }
        //}

        public void Handle(LogOnEvent message)
        {
            ActivateItem(_salesVM);
            //_loginVM = _container.GetInstance<LoginViewModel>();
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
