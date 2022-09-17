using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Slot.ViewModels;
namespace Slot
{
    public partial class Streamer : ContentPage
    {
        public StreamerViewModel Model { get; set; }
        public Streamer()
        {
            InitializeComponent();
            Title = Resource.StreamerTitle;
            Model = new StreamerViewModel(this);
            this.BindingContext = Model;
        }
    }
}
