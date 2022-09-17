using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using System.IO;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
namespace Slot.ViewModels
{
    public sealed class TextViewModel : INotifyPropertyChanged, IAsyncDisposable
    {
        public ICommand ClipboardCommand { get; set; }
        public ICommand WriteCommand { get; set; }
        public ICommand DoneCommand { get; set; }
        public ICommand InfoCommand { get; set; }
        private string _txt;
        public string Text { get => _txt; set { _txt = value; OnPropertyChanged(); } }
        public bool TextWasChanged { get; set; }
        private readonly StreamWriter _writer;
        private readonly StreamReader _reader;
        private readonly Stream _stream;
        private readonly FileInfoModel _model;
        public event PropertyChangedEventHandler PropertyChanged;
        public TextViewModel(string apiOrFileName,bool writeable,Page page,byte[] apiData = null)
        {
            if (writeable)
            {
                _stream = File.Create(apiOrFileName);
                _writer = new StreamWriter(_stream);
            }
            else
            {
                if (apiData is null)
                {
                    _model = new FileInfoModel(apiOrFileName, page);
                    _stream = File.OpenRead(apiOrFileName);
                }
                else
                {
                    _model = new FileInfoModel("", page);
                    _stream = new MemoryStream(apiData, true);
                }
                _reader = new StreamReader(_stream);
                Text = _reader.ReadToEnd();
            }
            InfoCommand = new Command(() =>
            {
                _model.InfoCommand.Execute(0);
            });
            ClipboardCommand = new Command(async () =>
            {
                await Clipboard.SetTextAsync(Text);
            });
            DoneCommand = new Command(async () =>
            {
                if (TextWasChanged && _writer==null)
                {
                    await File.WriteAllTextAsync(apiOrFileName,Text);
                }
                await DisposeAsync();
                await page.Navigation.PopAsync(true);
            });
            WriteCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var format = await page.DisplayPromptAsync(Resource.Message, Resource.EnterTextValue, placeholder: Resource.EnterTextValue, keyboard: Keyboard.Chat, cancel: Resource.Cancel, accept: Resource.Accept);
                    if (format != Resource.Cancel)
                    {
                        Text += format + Environment.NewLine;
                        await _writer.WriteLineAsync(Text);
                    }
                });
            });
        }
        public async ValueTask DisposeAsync()
        {
            _writer?.Dispose();
            _reader?.Dispose();
            await _stream.DisposeAsync();
        }
        public void OnPropertyChanged([CallerMemberName] string name = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
