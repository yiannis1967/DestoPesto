using Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UIKit;
[assembly: Xamarin.Forms.Dependency(typeof(DestoPesto.iOS.UploadData))]

namespace DestoPesto.iOS
{
    public class UploadData : IUploadData
    {
        public UploadData()
        {

        }
        public void PostSubmissionWithImage(string url, string submissionjson, Stream image)
        {
            NSUrl nsurl = NSUrl.FromString(url);//"Your url adress");//http://www.apple.com/

            NSMutableUrlRequest request = new NSMutableUrlRequest(nsurl);
            request.HttpMethod = "POST";
            NSMutableDictionary dic = new NSMutableDictionary();
            dic.Add(new NSString("Content-Type"), new NSString("application/json"));
            request.Headers = dic; // add Headers
            image.Position = 0;
            request.Body =NSData.FromStream(image);

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filename = Path.Combine(documents, "test.jpg");

            if (File.Exists(filename))
                File.Delete(filename);

            using (var fileStream = File.Create(filename))
            {
                image.Seek(0, SeekOrigin.Begin);
                image.CopyTo(fileStream);
            }

            //NSData.FromString("{\"version\":\"v1\", \"cityid\": \"101010100\"}"); //add body

          

            NSUrlSession session = NSUrlSession.SharedSession;
            NSUrlSessionTask task = session.CreateUploadTask(request, NSUrl.FromFilename(filename),(data, response, error) =>
            {
                Console.WriteLine("---"+response);
                Console.WriteLine("---"+ data);
                Console.WriteLine("---"+ error);
            });
            task.Resume();

        }
    }
}