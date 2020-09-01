using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using DesktopUI.EventModels;

namespace DesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<Object>,IHandle<LogOnEvent>
    {
        //private LoginViewModel _loginVM;
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private SimpleContainer _container;
        public ShellViewModel(IEventAggregator events, SalesViewModel salesVM, SimpleContainer container)
        {
            _events = events;
           // _loginVM = loginVM;
            _salesVM = salesVM;
            _container = container;

            _events.Subscribe(this);

            ActivateItem(_container.GetInstance<LoginViewModel>()) ;
        }

        public void Handle(LogOnEvent message)
        {
            ActivateItem(_salesVM);
            //_loginVM = _container.GetInstance<LoginViewModel>();
        }
    }
}
