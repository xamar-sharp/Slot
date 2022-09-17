using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Windows.Input;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
namespace Slot.ViewModels
{
    public sealed class XmlViewModel : INotifyPropertyChanged, IAsyncDisposable
    {
        private bool _writeable;
        private bool _textWasChanged;
        private bool _isFromAPI;
        private string _text;
        public bool Writeable
        {
            get => _writeable; set
            {
                _writeable = value;
                OnPropertyChanged();
            }
        }
        public bool TextWasChanged
        {
            get => _textWasChanged; set
            {
                _textWasChanged = value;
                OnPropertyChanged();
            }
        }
        public bool IsFromAPI { get => _isFromAPI; set { _isFromAPI = value; OnPropertyChanged(); } }
        public string Text { get => _text; set { _text = value; OnPropertyChanged(); } }
        public Command InfoCommand { get; set; }
        public Command ClipboardCommand { get; set; }
        public Command DoneCommand { get; set; }
        public Command AttributeCommand { get; set; }
        public Command ValueCommand { get; set; }
        public Command StartDocumentCommand { get; set; }
        public Command EndDocumentCommand { get; set; }
        public Command StartElementCommand { get; set; }
        public Command EndElementCommand { get; set; }
        public Command CommentCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Stack<string> _lastTags;
        private bool _inScopeWasValue;
        private bool _extTagOpened;
        private FileStream _stream;
        private XmlWriter _writer;
        private XmlReader reader;
        private string _fileName;
        public XmlViewModel(string apiOrFileName, bool writeable, Page page)
        {
            _lastTags = new Stack<string>(4);
            Writeable = writeable;
            if (writeable)
            {
                if (apiOrFileName.EndsWith(".xml"))
                {
                    IsFromAPI = false;
                    Text = String.Empty;
                    _fileName = apiOrFileName;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                if (apiOrFileName.EndsWith(".xml"))
                {
                    IsFromAPI = false;
                    Text = ReadXmlFile(apiOrFileName);
                    _fileName = apiOrFileName;
                }
                else
                {
                    IsFromAPI = true;
                    Text = apiOrFileName;
                }
            }
            DoneCommand = new Command(async () =>
            {
                if (TextWasChanged && !Writeable)
                {
                    await File.WriteAllTextAsync(_fileName, Text);
                }
                await DisposeAsync();
                await page.Navigation.PopAsync(true);
            });
            StartDocumentCommand = new Command(async () =>
            {
                _stream = new FileStream(_fileName, FileMode.Create, FileAccess.Write, FileShare.None);
                _writer = XmlWriter.Create(_stream, App.XmlWriterSettings);
                await _writer.WriteStartDocumentAsync();
                Text += "<?xml encoding=\"utf-8\" version=\"1.0\"?>\n";
            }, () => Writeable);
            EndDocumentCommand = new Command(async () =>
            {
                await _writer.WriteEndDocumentAsync();
            }, () => Writeable);
            StartElementCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    string tag = await page.DisplayPromptAsync(Resource.Message, Resource.EnterXmlElement, placeholder: Resource.EnterXmlName, maxLength: 64, keyboard: Keyboard.Text, cancel: Resource.Cancel, accept: Resource.Accept);
                    if (tag != Resource.Cancel)
                    {
                        _writer.WriteStartElement(tag);
                        if (_extTagOpened)
                        {
                            Text += ">\n";
                            _extTagOpened = false;
                        }
                        Text += $"<{tag} ";
                        _lastTags.Push(tag);
                        _extTagOpened = true;
                        _inScopeWasValue = true;
                    }
                });
            }, () => Writeable);
            EndElementCommand = new Command(async () =>
            {
                await _writer.WriteEndElementAsync();
                Text += _inScopeWasValue ? $"</{_lastTags.Pop()}>" : "/>";
            }, () => Writeable);
            AttributeCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var format = await page.DisplayPromptAsync(Resource.Message, Resource.EnterXmlAttribute, placeholder: Resource.XmlAttributePattern, maxLength: 512, keyboard: Keyboard.Email, cancel: Resource.Cancel, accept: Resource.Accept);
                    if (format != Resource.Cancel)
                    {
                        var values = format.Split('=');
                        _writer.WriteAttributeString(values[0], values[1]);
                        Text += $" {values[0]}=\"{values[1]}\" ";
                    }
                });
            }, () => Writeable);
            CommentCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    string comment = await page.DisplayPromptAsync(Resource.Message, Resource.EnterXmlComment, placeholder: Resource.EnterXmlComment, maxLength: 512, keyboard: Keyboard.Email, cancel: Resource.Cancel, accept: Resource.Accept);
                    if (comment != Resource.Cancel)
                    {
                        await _writer.WriteCommentAsync(comment);
                        Text += "\n" + $"<!-- {comment} -->";
                    }
                });
            }, () => Writeable);
            ValueCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    string value = await page.DisplayPromptAsync(Resource.Message, Resource.EnterXmlValue, placeholder: Resource.EnterXmlValue, keyboard: Keyboard.Email, cancel: Resource.Cancel, accept: Resource.Accept);
                    if (value != Resource.Cancel)
                    {
                        _writer.WriteValue(value);
                        if (_extTagOpened)
                        {
                            Text += ">\n";
                        }
                        _inScopeWasValue = true;
                        Text += value;
                    }
                });
            }, () => Writeable);
            ClipboardCommand = new Command(async () =>
            {
                await Clipboard.SetTextAsync(Text);
            }, () => !Writeable);
            InfoCommand = new Command(async () =>
            {
                await page.Navigation.PushAsync(new FileInfoPage(_fileName));
            }, () => !Writeable && !IsFromAPI);
        }
        public async ValueTask DisposeAsync()
        {
            await Task.Run(async () =>
            {
                reader?.Dispose();
                _writer?.Dispose();
                await _stream.DisposeAsync();
            });
        }
        public string ReadXmlFile(string fileName)
        {
            StringBuilder builder = new StringBuilder(1024);
            try
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    reader = XmlReader.Create(stream);
                    reader.MoveToContent();
                    string root = reader.Name;
                    if (reader.IsEmptyElement)
                    {
                        builder.Append($"<{reader.Name}");
                        reader.ReadAttributes(builder);
                        builder.AppendLine("/>");
                        return builder.ToString();
                    }
                    builder.Append($"<{reader.Name}");
                    reader.ReadAttributes(builder);
                    builder.AppendLine(">");
                    while (!reader.EOF)
                    {
                        if (!reader.Read())
                        {
                            break;
                        }
                        if (reader.NodeType == XmlNodeType.EndElement && _lastTags.Count > 0 && reader.Name == _lastTags.Peek())
                        {
                            var val = _lastTags.Pop();
                            builder.AppendLine($"</{reader.Name}>");
                            continue;
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement && _lastTags.Count > 0 && reader.Name == root)
                        {
                            break;
                        }
                        else if (reader.NodeType == XmlNodeType.Whitespace)
                        {
                            continue;
                        }
                        else if (reader.NodeType == XmlNodeType.Comment)
                        {
                            builder.AppendLine("<!--" + reader.Value + "-->");
                        }
                        else if (reader.NodeType == XmlNodeType.Text)
                        {
                            builder.Append(reader.Value);
                        }
                        else if (reader.NodeType == XmlNodeType.Element)
                        {
                            builder.Append($"<{reader.Name}");
                            reader.ReadAttributes(builder);
                            if (reader.IsEmptyElement)
                            {
                                builder.AppendLine($"/>");
                                continue;
                            }
                            else
                            {
                                builder.AppendLine(">");
                                _lastTags.Push(reader.Name);
                            }
                        }
                    }
                    builder.AppendLine($"</{root}>");
                }
            }
            catch (Exception ex)
            {
                return Resource.InvalidXmlFile;
            }
            _lastTags?.Clear();
            return builder.ToString();
        }
        public void OnPropertyChanged([CallerMemberName] string name = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
    public static class XmlReaderExtensions
    {
        public static void ReadAttributes(this XmlReader reader, StringBuilder builder)
        {
            for (int x = 0; x < reader.AttributeCount; x++)
            {
                reader.MoveToAttribute(x);
                builder.AppendFormat(" {0}=\"{1}\"", reader.Name, reader.Value);
                reader.MoveToElement();
            }
        }
    }
}
