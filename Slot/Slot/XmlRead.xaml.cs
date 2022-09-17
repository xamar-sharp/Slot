﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Slot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class XmlRead : ContentView
    {
        public XmlRead()
        {
            InitializeComponent();
        }

        private void Editor_Completed(object sender, EventArgs e)
        {
            textChanged.IsChecked = true;
        }
    }
}