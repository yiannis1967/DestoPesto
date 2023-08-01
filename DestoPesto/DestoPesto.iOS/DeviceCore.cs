
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;


[assembly: Xamarin.Forms.Dependency(typeof(DestoPesto.IOS.DeviceCore))]

namespace DestoPesto.IOS
{
    /// <MetaDataID>{9197f2b7-0392-426b-b818-566c2f0fa1b9}</MetaDataID>

    public class DeviceCore : IDevice
    {

        public bool IsBackgroundServiceStarted
        {
            get
            {
                if (Background!=null&&Background.Status==TaskStatus.Running)
                    return true;

                return false;

            }
        }


        Task Background;

        public bool RunInBackground(Action action, BackgroundServiceState serviceState)
        {

         

            if (Background==null||Background.Status!=TaskStatus.Running)
            {
                Background=null;
                Background=Task.Run(() =>
                {
                    action.Invoke();
                });
                return true;
            }
            return false;
        }

        public void StopBackgroundService()
        {
          
        }

        static internal string m_androidId;

        static internal string m_OldandroidId="";

        

     
    }


}
