using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slot.ViewModels;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Slot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TxtPage : ContentPage, INotifyable
    {
        private readonly Stopwatch _watch;
        public TextViewModel Model { get; set; }
        public TxtPage(string fileName = null, bool writeable = true, byte[] fromAPI = null)
        {
            _watch = Stopwatch.StartNew();
            InitializeComponent();
            Title = Resource.TextPageTitle;
            Model = new TextViewModel(fileName, writeable, this, fromAPI);
            this.BindingContext = Model;
        }
        public double Notify()
        {
            var ms = _watch.ElapsedMilliseconds;
            _watch.Reset();
            return ms;
        }

        private void Editor_Completed(object sender, EventArgs e)
        {
            textChanged.IsChecked = true;
        }
    }
}