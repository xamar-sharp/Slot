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
    public partial class XmlPage : ContentPage,INotifyable
    {
        private readonly Stopwatch _watch;
        public XmlViewModel Model { get; set; }
        public XmlPage(string fileName=null,bool writeable=true,string fromAPI=null)
        {
            _watch = Stopwatch.StartNew();
            InitializeComponent();
            Title = Resource.XmlPageTitle;
            Model = new XmlViewModel(fileName ?? fromAPI, writeable, this);
            var type = writeable ? typeof(XmlWrite) : typeof(XmlRead);
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