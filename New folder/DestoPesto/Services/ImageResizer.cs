
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DestoPesto.Services
{
    public static class ImageResizer
    {
        static ImageResizer()
        {
        }
        public static  Task<byte[]> ResizeImage(byte[] imageData, float width, float height)
        {
            IImageResizer imageResizer = DependencyService.Get<IImageResizer>();
            return imageResizer.ResizeImage(imageData, width, height);
        }
         
        
    }
    /// <MetaDataID>{efacac76-5c67-4cd5-828f-9619e6d85c5e}</MetaDataID>
    public interface IImageResizer
    {
        Task<byte[]> ResizeImage(byte[] imageData, float width, float height);
    }
}
