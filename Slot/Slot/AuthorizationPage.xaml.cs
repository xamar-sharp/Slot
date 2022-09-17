using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace Slot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthorizationPage : ContentPage
    {
        public bool IsSignIn { get; set; }
        public bool IsSignUp { get => !IsSignIn; set { } }
        public UserInfo Model { get; set; }
        public ICommand SignInCommand { get; set; }
        public ICommand IconCommand { get; set; }
        public ICommand SignUpCommand { get; set; }
        private readonly ControlTemplate _login;
        private readonly ControlTemplate _register;
        public AuthorizationPage()
        {
            InitializeComponent();
            Model = new UserInfo();
            _login = new ControlTemplate(typeof(LoginView));
            _register = new ControlTemplate(typeof(RegisterView));
            IconCommand = new Command((obj) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = await Xamarin.Essentials.MediaPicker.PickPhotoAsync();
                    Model.Icon = await System.IO.File.ReadAllBytesAsync(result.FullPath);
                    (obj as ImageButton).Source = ImageSource.FromStream(() => new System.IO.MemoryStream(Model.Icon));
                });
            });
            SignInCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (await SlotAPI.Authorize(Model))
                    {
                        App.Current.MainPage = new SlotPage();
                    }
                    else
                    {
                        await DisplayAlert(Resource.Message, Resource.APIError, Resource.Accept);
                    }
                });
            });
            SignUpCommand = new Command(async () =>
            {
                if (await SlotAPI.Authorize(Model))
                {
                    App.Current.MainPage = new SlotPage();
                }
                else
                {
                    await DisplayAlert(Resource.Message, Resource.APIError, Resource.Accept);
                }
            });
            ControlTemplate = _login;
            IsSignIn = true;
            this.BindingContext = this;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (IsSignIn)
            {
                IsSignIn = false;
                ControlTemplate = _register;
                OnPropertyChanged("IsSignUp");
            }
            else
            {
                IsSignIn = true;
                ControlTemplate = _login;
                OnPropertyChanged("IsSignUp");
            }
        }
    }
}