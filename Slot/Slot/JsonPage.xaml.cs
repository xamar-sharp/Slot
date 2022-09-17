using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Slot.ViewModels;
namespace Slot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JsonPage : ContentPage,INotifyable
    {
        private readonly Stopwatch _watch;
        public JsonViewModel Model { get; set; }
        public JsonPage(string fileName = null, bool writeable = true, string fromAPI = null)
        {
            _watch = Stopwatch.StartNew();
            InitializeComponent();
            Title = Resource.JsonPageTitle;
            Model = new JsonViewModel(fileName ?? fromAPI, writeable, this);
            var type = writeable ? typeof(JsonWrite) : typeof(JsonRead);
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