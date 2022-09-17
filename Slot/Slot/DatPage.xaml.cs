using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Slot.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Slot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DatPage : ContentPage, INotifyable
    {
        private readonly Stopwatch _watch;
        public DatViewModel Model { get; set; }
        public DatPage(string fileName = null, bool writeable = true, byte[] fromAPI = null)
        {
            _watch = Stopwatch.StartNew();
            InitializeComponent();
            Title = Resource.FileBinPageTitle;
            Model = new DatViewModel(fileName, writeable, this, fromAPI);
            var type = writeable ? typeof(DatWrite) : typeof(DatRead);
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