using System;
using System.Collections.Generic;
using System.Text;

namespace DestoPesto.Services
{
    /// <MetaDataID>{50b02a3c-e5c9-4cd5-8a2d-84ecc9877aad}</MetaDataID>
    public interface INotification
    {
        void ShowNotification(string title, string message);
    }
}
