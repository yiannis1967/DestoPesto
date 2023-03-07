
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
[assembly: Xamarin.Forms.Dependency(typeof(DestoPesto.Services.android.ImageResizer))]
namespace DestoPesto.Services.android
{
    /// <MetaDataID>{3355f2b5-9720-48e2-adf0-d9910dd87c6e}</MetaDataID>
    public  class ImageResizer: IImageResizer
    {
       
        public  async Task<byte[]> ResizeImage(byte[] imageData, float width, float height)
        {
            return ResizeImageAndroid(imageData, width, height);
        }
        public static byte[] ResizeImageAndroid(byte[] imageData, float width, float height)
        {
            // Load the bitmap
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);
            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                return ms.ToArray();
            }
        }
    }
}
