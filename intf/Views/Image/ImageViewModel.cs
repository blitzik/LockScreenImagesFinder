using Common.Commands;
using Common.ViewModels;
using intf.Services.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace intf.Views
{
    public class ImageViewModel : BaseScreen
    {
        private string _imagePath;
        public string ImagePath
        {
            get { return _imagePath; }
            private set { Set(ref _imagePath, value); }
        }


        private int _width;
        public int Width
        {
            get { return _width; }
            private set { Set(ref _width, value); }
        }


        private int _height;
        public int Height
        {
            get { return _height; }
            private set { Set(ref _height, value); }
        }


        private DelegateCommand<object> _selectCommand;
        public DelegateCommand<object> SelectCommand
        {
            get
            {
                if (_selectCommand == null) {
                    _selectCommand = new DelegateCommand<object>(p => Process());
                }
                return _selectCommand;
            }
        }


        private IIODialogService _dialogService;        

        public ImageViewModel(string imagePath, IIODialogService dialogService)
        {
            _imagePath = imagePath;
            _dialogService = dialogService;

            using (var imageStream = File.OpenRead(_imagePath)) {
                var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.IgnoreColorProfile,
                                                                BitmapCacheOption.Default);

                Width = decoder.Frames[0].PixelWidth;
                Height = decoder.Frames[0].PixelHeight;
            }
        }


        private void Process()
        {
            string filePath = _dialogService.GetFilePath<SaveFileDialog>(Path.GetFileName(ImagePath), (d) => {
                d.Filter = "JPG Image (*.jpg)|*.jpg";
            });

            if (string.IsNullOrEmpty(filePath)) {
                return;
            }

            File.Copy(ImagePath, filePath, true);

            FlashMessagesManager.DisplayFlashMessage("Image's been successfully saved!", Common.FlashMessages.Type.SUCCESS);
        }
    }
}
