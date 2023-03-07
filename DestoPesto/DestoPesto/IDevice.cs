using System;
using System.Collections.Generic;
using System.Text;

namespace DestoPesto
{
    /// <MetaDataID>{48d05848-b872-46b6-9b65-5a423dc01fab}</MetaDataID>
    public interface IDevice
    {
        //  Task<string> SendMessage(string xmlMessage);




        bool IsBackgroundServiceStarted { get; }
        

        bool RunInBackground(Action action, BackgroundServiceState serviceState);

        void StopBackgroundService();


    }
}
