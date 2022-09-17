using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Slot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SlotAPIPage : ContentPage
    {
        public ObservableCollection<Data> Source { get; set; }
        public SlotAPIPage()
        {
            InitializeComponent();
            Source = new ObservableCollection<Data>(new List<Data>(4));
            this.BindingContext = this;
        }
        protected override async void OnAppearing()
        {
            Source.Clear();
            foreach (var value in await SlotAPI.GetDatas())
            {
                Source.Add(value);
            }
        }
        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await new Slot.ViewModels.StreamerViewModel((App.Current.MainPage as Shell).CurrentPage).Match(System.IO.Path.GetExtension((e.Item as Data).FileURI), null, (e.Item as Data).FileURI, false);
        }

    }
}