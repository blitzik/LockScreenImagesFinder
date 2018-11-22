using Caliburn.Micro;
using System.Reflection;
using Common.Commands;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Common.Overlay;
using Common.ViewModels;
using Common.EventAggregator.Messages;
using intf.Messages;

namespace intf.Views
{
    public class MainWindowViewModel :
        BaseConductorOneActive<IViewModel>,
        IHandle<IChangeViewMessage<IViewModel>>
    {
        private string _version;
        public string AppVersion
        {
            get { return _version; }
        }


        private DelegateCommand<object> _hideOverlayCommand;
        public DelegateCommand<object> HideOverlayCommand
        {
            get
            {
                if (_hideOverlayCommand == null) {
                    _hideOverlayCommand = new DelegateCommand<object>(p => {
                        if (!Overlay.Token.IsMandatory) {
                            Overlay.HideOverlay();
                        }
                    });
                }
                return _hideOverlayCommand;
            }
        }


        public MainWindowViewModel()
        {
            _version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }


        // -----


        protected override void OnInitialize()
        {
            base.OnInitialize();

            EventAggregator.Subscribe(this);

            EventAggregator.PublishOnUIThread(new ChangeViewMessage<DashboardViewModel>());
        }


        // -----

        
        public void Handle(IChangeViewMessage<IViewModel> message)
        {
            IViewModel vm;
            if (message.ViewModel != null) {
                vm = message.ViewModel;
            } else {
                vm = GetViewModel(message.Type);
            }

            if (vm == ActiveItem) {
                return;
            }
            message.Apply(vm);
            ActivateItem(vm);
        }
    }
}