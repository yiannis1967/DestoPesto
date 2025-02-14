using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DestoPesto
{
    public interface IUploadData
    {
        void PostSubmissionWithImage(string url, string submissionjson, Stream image);
    }
}
