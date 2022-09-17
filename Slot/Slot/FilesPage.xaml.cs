using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slot.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
namespace Slot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilesPage : ContentPage
    {
        public ObservableCollection<FileInfoModel> Source { get; set; }
        public FilesPage(IList<FileInfoModel> models)
        {
            Source = new ObservableCollection<FileInfoModel>(models);
            InitializeComponent();
            this.BindingContext = this;
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            GC.Collect(2, GCCollectionMode.Forced);
            await new StreamerViewModel(this).Match((e.Item as FileInfoModel).Extension, (e.Item as FileInfoModel).FullName, writeable: false);
        }
    }
}