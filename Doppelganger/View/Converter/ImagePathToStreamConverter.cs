using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.IO;

namespace Doppelganger.View.Converter
{
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    class ImagePathToStreamConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var __path = value as string;
            if (__path != null && File.Exists(__path))
            {
                try
                {
                    var __image = new BitmapImage();
                    __image.BeginInit();
                    __image.StreamSource = new FileStream(__path, FileMode.Open, FileAccess.Read);
                    __image.CacheOption = BitmapCacheOption.OnLoad;
                    __image.EndInit();

                    __image.StreamSource.Dispose();

                    return __image;
                }
                catch(IOException)
                {
                    return null;
                }
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
