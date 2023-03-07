using System;
using System.Collections.Generic;
using System.Text;

namespace DestoPesto
{
    /// <MetaDataID>{5dc8ca99-833a-4dd4-9092-5f36be3c0939}</MetaDataID>
    public interface IBackgroundService
    {
        bool IsServiceStarted { get; }
        bool Run(Action action, BackgroundServiceState serviceState);

        void Stop();

    }
    /// <MetaDataID>{939ad76d-c162-4674-b7d8-fe79e0d2bcf4}</MetaDataID>
    public class BackgroundServiceState
    {
        public bool Terminate;

    }

    ///// <MetaDataID>{c0b407ba-c962-4565-a377-b3da94e5d1d0}</MetaDataID>
    //public interface IRingtoneService
    //{
    //    void Play();
    //    void Stop();
    //}
}
