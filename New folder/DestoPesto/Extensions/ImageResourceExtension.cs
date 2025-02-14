using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DestoPesto.Extensions
{
    [ContentProperty(nameof(Source))]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }

            // Do your translation lookup here, using whatever method you require
            ImageSource imageSource;

            if (Device.RuntimePlatform == Device.UWP)
            {
                // fix for UWP resource loading issue
                imageSource = ImageSource.FromResource(Source, Assembly.GetExecutingAssembly());
            }
            else
            {

                var names = Assembly.GetCallingAssembly().GetManifestResourceNames();
                imageSource = ImageSource.FromResource(Source);
            }

            return imageSource;
        }
    }

    public class AppModeExtension : IMarkupExtension
    {
        public string Color { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {

            

            if (Xamarin.Essentials.AppInfo.RequestedTheme == Xamarin.Essentials.AppTheme.Dark)
            {
                switch (Color)
                {
                    case "Background":
                        return Xamarin.Forms.Color.FromHex("#0D6EFD");
                    case "Text":
                        return Xamarin.Forms.Color.FromHex("#0D6EFD");
                    case "TextRevert":
                        return Xamarin.Forms.Color.White;
                    default:
                        return Xamarin.Forms.Color.Black;


                }



                if (Color == "Background")
                    return Xamarin.Forms.Color.FromHex("#0D6EFD");

                else
                    return Xamarin.Forms.Color.Red;
            }
            else if (Xamarin.Essentials.AppInfo.RequestedTheme == Xamarin.Essentials.AppTheme.Light)
            {

                switch (Color)
                {
                    case "Background":
                        return Xamarin.Forms.Color.White;
                    case "Text":
                        return Xamarin.Forms.Color.Black;
                    default:
                        return Xamarin.Forms.Color.Black;


                }
                if (Color == "Background")
                    return Xamarin.Forms.Color.White;

                else
                    return Xamarin.Forms.Color.Red;
            }
            else
            {
                if (Color == "Background")
                    return Xamarin.Forms.Color.FromHex("#0D6EFD");

                else
                    return Xamarin.Forms.Color.Red;

            }

            

        }
    }
}
