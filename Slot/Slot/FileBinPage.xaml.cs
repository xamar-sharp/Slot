using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Slot.ViewModels;
using System.Diagnostics;
namespace Slot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FileBinPage : ContentPage, INotifyable
    {
        private readonly Stopwatch _watch;
        public BinViewModel Model { get; set; }
        public FileBinPage(string fileName = null, bool writeable = true, byte[] fromAPI = null)
        {
            _watch = Stopwatch.StartNew();
            InitializeComponent();
            Title = Resource.FileBinPageTitle;
            Model = new BinViewModel(fileName, writeable, this,fromAPI);
            var type = writeable ? typeof(FileWrite) : typeof(FileRead);
            ControlTemplate = new ControlTemplate(type);
            this.BindingContext = Model;
        }
        public double Notify()
        {
            var ms = _watch.ElapsedMilliseconds;
            _watch.Reset();
            return ms;
        }
    }
}