using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Newtonsoft.Json;
using System.IO;
using Xamarin.Forms;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;
namespace Slot.ViewModels
{
    public sealed class JsonViewModel : INotifyPropertyChanged, IDisposable
    {
        private bool _writeable;
        private bool _textWasChanged;
        private bool _isFromAPI;
        private bool _import;
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
        public bool Import
        {
            get => _import; set
            {
                _import = value;
                OnPropertyChanged();
            }
        }
        public bool IsFromAPI { get => _isFromAPI; set { _isFromAPI = value; OnPropertyChanged(); } }
        public string Text { get => _text; set { _text = value; OnPropertyChanged(); } }
        public Command InfoCommand { get; set; }
        public Command ClipboardCommand { get; set; }
        public Command DoneCommand { get; set; }
        public Command PropertyCommand { get; set; }
        public Command ValueCommand { get; set; }
        public Command StartObjectCommand { get; set; }
        public Command EndObjectCommand { get; set; }
        public Command StartArrayCommand { get; set; }
        public Command EndArrayCommand { get; set; }
        public Command CommentCommand { get; set; }
        public Command NullCommand { get; set; }
        public Command UndefinedCommand { get; set; }
        public Command ImportCommand { get; set; }
        public Command ExportCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private TextWriter _stream;
        private JsonTextWriter _writer;
        private JsonTextReader _reader;
        private string _fileName;
        public JsonViewModel(string apiOrFileName, bool writeable, Page page)
        {
            Writeable = writeable;
            if (writeable)
            {
                if (apiOrFileName.EndsWith(".json"))
                {
                    IsFromAPI = false;
                    Text = String.Empty;
                    _fileName = apiOrFileName;
                    _stream = File.CreateText(_fileName);
                    _writer = new JsonTextWriter(_stream);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                if (apiOrFileName.EndsWith(".json"))
                {
                    IsFromAPI = false;
                    Text = ReadJsonFile(apiOrFileName);
                    _fileName = apiOrFileName;
                }
                else
                {
                    IsFromAPI = true;
                    Text = apiOrFileName;
                }
            }
            ImportCommand = new Command(async () =>
            {
                var uri = await page.DisplayPromptAsync(Resource.Message, Resource.ImportUri, placeholder: Resource.ImportUri, maxLength: 512, keyboard: Keyboard.Url, cancel: Resource.Cancel, accept: Resource.Accept);
                if (uri != Resource.Cancel&&Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute))
                {
                    Text = await NetCentre.ReadAsJsonAsync(uri);
                    Import = true;
                }
            }, () => Writeable);
            ExportCommand = new Command(async () =>
            {
                var uri = await page.DisplayPromptAsync(Resource.Message, Resource.ExportUri, placeholder: Resource.ExportUri, maxLength: 512, keyboard: Keyboard.Url, cancel: Resource.Cancel, accept: Resource.Accept);
                if (uri!=Resource.Cancel &&Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute))
                {
                    await NetCentre.PostAsJsonAsync(uri, Text);
                }
            }, () => !Writeable);
            DoneCommand = new Command(async () =>
            {
                if (Import || (TextWasChanged && !Writeable))
                {
                    await File.WriteAllTextAsync(_fileName, Text);
                }
                Dispose();
                await page.Navigation.PopAsync(true);
            });
            StartObjectCommand = new Command(async () =>
            {
                await _writer.WriteStartObjectAsync();
                Text += "{";
            }, () => Writeable);
            EndObjectCommand = new Command(async () =>
            {
                await _writer.WriteEndObjectAsync();
                Text += "},";
            }, () => Writeable);
            StartArrayCommand = new Command(async () =>
            {
                await _writer.WriteStartArrayAsync();
                Text += "[";
            }, () => Writeable);
            NullCommand = new Command(async () =>
            {
                await _writer.WriteNullAsync();
                Text += "null,";
            }, () => Writeable);
            UndefinedCommand = new Command(async () =>
            {
                await _writer.WriteUndefinedAsync();
                Text += "undefined,";
            }, () => Writeable);
            EndArrayCommand = new Command(async () =>
            {
                await _writer.WriteEndArrayAsync();
                Text += "],";
            }, () => Writeable);
            ValueCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var format = await page.DisplayPromptAsync(Resource.Message, Resource.EnterJsonValue, placeholder: Resource.EnterJsonValue, maxLength: 512, keyboard: Keyboard.Email, cancel: Resource.Cancel, accept: Resource.Accept);
                    if (format != Resource.Cancel)
                    {
                        var type = await page.DisplayActionSheet(Resource.EnterJsonType, Resource.Cancel, Resource.Destructive, Resource.Int, Resource.Long, Resource.Double, Resource.String, Resource.Char, Resource.Byte, Resource.Bool);
                        await MatchJsonValue(format, type, _writer);
                    }
                });
            }, () => Writeable);
            CommentCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    string comment = await page.DisplayPromptAsync(Resource.Message, Resource.EnterJsonComment, placeholder: Resource.EnterJsonComment, maxLength: 512, keyboard: Keyboard.Email, cancel: Resource.Cancel, accept: Resource.Accept);
                    if (comment != Resource.Cancel)
                    {
                        await _writer.WriteCommentAsync(comment);
                        Text += $"/* {comment} */";
                    }
                });
            }, () => Writeable);
            PropertyCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    string prop = await page.DisplayPromptAsync(Resource.Message, Resource.EnterJsonProperty, placeholder: Resource.EnterJsonProperty, maxLength: 512, keyboard: Keyboard.Email, cancel: Resource.Cancel, accept: Resource.Accept);
                    if (prop != Resource.Cancel)
                    {
                        await _writer.WritePropertyNameAsync(prop);
                        Text += "\n" + $"\"{prop}\":";
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
        public async Task MatchJsonValue(string value, string type, JsonTextWriter writer)
        {
            unchecked
            {
                if (type == Resource.Int)
                {
                    await writer.WriteValueAsync(Convert.ToInt32(value));
                    Text += value+",";
                }
                else if (type == Resource.Long)
                {
                    await writer.WriteValueAsync(Convert.ToInt64(value));
                    Text += value+",";
                }
                else if (type == Resource.Byte)
                {
                    await writer.WriteValueAsync(Convert.ToByte(value));
                    Text += value + ",";
                }
                else if (type == Resource.Bool)
                {
                    await writer.WriteValueAsync(Convert.ToBoolean(value));
                    Text += value + ",";
                }
                else if (type == Resource.Char)
                {
                    await writer.WriteValueAsync(value[0]);
                    Text += $"\"{value}\"" + ",";
                }
                else if (type == Resource.Double)
                {
                    await writer.WriteValueAsync(Convert.ToDouble(value));
                    Text += value + ",";
                }
                else
                {
                    await writer.WriteValueAsync(value);
                    Text += $"\"{value}\",";
                }
            }
        }
        public void Dispose()
        {
            _reader?.Close();
            _writer?.Close();
            _stream?.Close();
        }
        public string ReadJsonFile(string fileName)
        {
            StringBuilder builder = new StringBuilder(1024);
            try
            {
                using (TextReader reader = File.OpenText(fileName))
                {
                    _reader = new JsonTextReader(reader);
                    while (_reader.Read())
                    {
                        switch (_reader.TokenType)
                        {
                            case JsonToken.PropertyName:
                                builder.Append($"\"{_reader.Value}\" : ");
                                break;
                            case JsonToken.String:
                                builder.Append($"\"{_reader.Value}\", ");
                                break;
                            case JsonToken.StartObject:
                                builder.Append("{\n");
                                break;
                            case JsonToken.EndObject:
                                builder.Append($"}},\n");
                                break;
                            case JsonToken.StartArray:
                                builder.Append($"[\n");
                                break;
                            case JsonToken.EndArray:
                                builder.Append($"],\n");
                                break;
                            case JsonToken.Comment:
                                builder.Append($"/* {_reader.Value} */");
                                break;
                            case JsonToken.Null:
                                builder.Append($"null, ");
                                break;
                            case JsonToken.Undefined:
                                builder.Append($"undefined, ");
                                break;
                            case JsonToken.Date:
                                builder.Append($"\"{_reader.Value}\", ");
                                break;
                            default:
                                builder.Append($"{_reader.Value},");
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Resource.InvalidJsonFile;
            }
            return builder.ToString();
        }
        public void OnPropertyChanged([CallerMemberName] string name = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
