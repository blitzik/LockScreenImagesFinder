using Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.ExtensionMethods;
using intf.Services.IO;
using Common.Commands;

namespace intf.Views
{
    public class DashboardViewModel : BaseScreen
    {
        private ObservableCollection<ImageViewModel> _imagesViewModels;
        private ReadOnlyObservableCollection<ImageViewModel> _images;
        public ReadOnlyObservableCollection<ImageViewModel> Images
        {
            get { return _images; }
            private set { Set(ref _images, value); }
        }


        private string _lockScreenImagesPath;
        public string LockScreenImagesPath
        {
            get { return _lockScreenImagesPath; }
            private set { Set(ref _lockScreenImagesPath, value); }
        }


        private DelegateCommand<object> _refreshCommand;
        public DelegateCommand<object> RefreshCommand
        {
            get
            {
                if (_refreshCommand == null) {
                    _refreshCommand = new DelegateCommand<object>(p => RefreshList());
                }
                return _refreshCommand;
            }
        }


        private IIODialogService _dialogService;

        public DashboardViewModel(IIODialogService dialogService)
        {
            _dialogService = dialogService;
        }


        protected override void OnInitialize()
        {
            base.OnInitialize();

            LockScreenImagesPath = string.Format(
                "{0}Users\\{1}\\AppData\\Local\\Packages\\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\\LocalState\\Assets",
                Path.GetPathRoot(Environment.SystemDirectory),
                Environment.UserName
            );

            _imagesViewModels = new ObservableCollection<ImageViewModel>(PrepareImageViewModels(Directory.GetFiles(LockScreenImagesPath)));
            Images = new ReadOnlyObservableCollection<ImageViewModel>(_imagesViewModels);
        }


        private IList<ImageViewModel> PrepareImageViewModels(string[] imagesPath)
        {
            List<ImageViewModel> result = new List<ImageViewModel>();
            foreach (string iPath in imagesPath) {
                result.Add(PrepareViewModel(() => { return new ImageViewModel(iPath, _dialogService); }));
            }

            return result;
        }


        private void RefreshList()
        {
            _imagesViewModels.Refill(PrepareImageViewModels(Directory.GetFiles(LockScreenImagesPath)));
        }

    }
}
