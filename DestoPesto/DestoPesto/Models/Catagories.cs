using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace DestoPesto.Models
{
    public class Catagories
    {
        public int id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string description_en { get; set; }
        public string comments { get; set; }
        public string descriptionCognity { get; set; }
        public string color { get; set; }
        public string textColor { get; set; }
        public string VectorImageUrl { get; set; }
        public string IconUrl { get; set; }
        public string markIconUrl { get; set; }
        public string InfoText { get; set; }
        public string InfoText_en { get; set; }

        ImageSource _ImageSource;
        public ImageSource ImageSource
        {
            get
            {

                if (_ImageSource == null)
                {
                    Func<Stream> getStream = () =>
                    {
                        return GetIconStream();
                    };

                    _ImageSource = StreamImageSource.FromStream(getStream);
                }
                return _ImageSource;

            }
        }


        public Stream GetIconStream()
        {


            string iconFileName = $"icon_{id}.png";
            var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // iOS: Environment.SpecialFolder.Resources
            var filePath = Path.Combine(libraryPath, iconFileName);
            if (File.Exists(filePath))
            {
                var buffer = System.IO.File.ReadAllBytes(filePath);
                var stream = new MemoryStream(buffer);
                var le = stream.Length;

                return stream;

            }
            else { return null; }

        }
    }
}