
using DestoPesto.Models;
using DestoPesto.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DestoPesto
{
    public class UserSubmissions : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        object lodingLock = new object();

        ObservableCollection<DamageData> _AllUserDamages;
        public async Task<ObservableCollection<DamageData>> GetAllUserDamages()
        {

            if (_AllUserDamages == null)
            {
                Task getDamagesTask = null;
                lock (lodingLock)
                {
                    getDamagesTask = GetDamagesTask;
                }
                if (getDamagesTask != null)
                {
                    await getDamagesTask;
                    return _AllUserDamages;
                }

                lock (lodingLock)
                {
                    getDamagesTask = Task.Run(async () =>
                    {
                        _AllUserDamages = await JsonHandler.GetUserDamages();
                    });
                    GetDamagesTask = getDamagesTask;
                }
                await GetDamagesTask;
            }

            return _AllUserDamages;
        }

        public async Task Refresh()
        {
            _AllUserDamages = null;
            await GetAllUserDamages();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AllUserDamages)));
        }

        internal void RemoveUserSubmittedDamage(DamageData damageData)
        {
            if (AllUserDamages != null && AllUserDamages.Contains(damageData))
                _AllUserDamages.Remove(damageData);
        }

        public ObservableCollection<DamageData> AllUserDamages
        {
            get
            {
                return _AllUserDamages;
            }
        }

        public Task GetDamagesTask { get; private set; }
    }
}