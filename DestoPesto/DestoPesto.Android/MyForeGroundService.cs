using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DestoPesto.Droid
{
    /// <MetaDataID>{52453757-2621-49c4-a293-43e18ae14e6a}</MetaDataID>

    [Service(Name = "com.arionsoftware.destopesto.MyForeGroundService", Exported = true)]
    public class MyForeGroundService : DestoPesto.Droid.ForegroundService, IBackgroundService
    {
        public bool IsServiceStarted
        {
            get
            {
                return isStarted;
            }
        }

        public bool Run(Action action, BackgroundServiceState backgroundServiceState)
        {
            ForegroundService.ServiceState serviceState = new ForegroundService.ServiceState()
            {
                NotificationTitle = Properties.Resources.ApplicationName,
                NotificationContentText = Properties.Resources.NotificationContentText,
                NotificationSmallIcon = Resource.Drawable.icon,
                StopServiceCommandTitle = Properties.Resources.StopServiceCommandTitle,
                StopServiceCommandIcon = Resource.Drawable.icon,
                StopActionID = "destopesto.action.STOP_SERVICE",
                DelayBetweenLogMessage = 5000, // milliseconds,
                ServiceRunningNotificationID = 10000,
                ActionsMainActivity = "destopesto.action.MAIN_ACTIVITY",
                ServiceStartedKey = "has_service_been_started",
                BackgroundServiceState = backgroundServiceState,
                Terminate = false

            };
            serviceState.Runnable = action;

            StartForegroundService(this, "destopesto.action.START_SERVICE", serviceState);
            return true;
        }

        public void Stop()
        {

        }
    }
}
