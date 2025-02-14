using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DestoPesto.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubmisionsListPageFlyout : ContentPage
    {
        public ListView ListView;

        public SubmisionsListPageFlyout()
        {
            InitializeComponent();

            BindingContext = new SubmisionsListPageFlyoutViewModel();
            ListView = MenuItemsListView;
        }

        private class SubmisionsListPageFlyoutViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<SubmisionsListPageFlyoutMenuItem> MenuItems { get; set; }

            public SubmisionsListPageFlyoutViewModel()
            {
                MenuItems = new ObservableCollection<SubmisionsListPageFlyoutMenuItem>(new[]
                {
                    new SubmisionsListPageFlyoutMenuItem { Id = 0, Title = "Page 1" },
                    new SubmisionsListPageFlyoutMenuItem { Id = 1, Title = "Page 2" },
                    new SubmisionsListPageFlyoutMenuItem { Id = 2, Title = "Page 3" },
                    new SubmisionsListPageFlyoutMenuItem { Id = 3, Title = "Page 4" },
                    new SubmisionsListPageFlyoutMenuItem { Id = 4, Title = "Page 5" },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}