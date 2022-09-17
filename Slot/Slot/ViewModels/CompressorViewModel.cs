using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.IO;
using System.IO.Compression;
using Xamarin.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace Slot.ViewModels
{
    public sealed class CompressorViewModel
    {
        public bool IsDecompress { get; set; }
        public string CompressType { get; set; }
        private string _dest;
        public string Destination { get => _dest; set { _dest = value;OnPropertyChanged(); } }
        public ICommand DoneCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly string _srcPath;
        public void OnPropertyChanged([CallerMemberName] string name = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public CompressorViewModel(string sourcePath)
        {
            _srcPath = sourcePath;
            DoneCommand = new Command((obj) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    (obj as ActivityIndicator).IsRunning = true;
                });
                if (IsDecompress)
                {
                    Decompress(CompressType);
                }
                else
                {
                    Compress(CompressType);
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    (obj as ActivityIndicator).IsRunning = false;
                });
            });
        }
        public void Compress(string compressType)
        {
            using (var stream = File.OpenRead(_srcPath))
            {
                using (var dest = File.Create(Destination))
                {
                    if (compressType == Resource.Brotli)
                    {
                        using (BrotliStream brotli = new BrotliStream(dest, CompressionLevel.Optimal))
                        {
                            stream.CopyTo(brotli);
                        }
                    }
                    else if (compressType == Resource.GZip)
                    {
                        using (GZipStream gzip = new GZipStream(dest, CompressionLevel.Optimal))
                        {
                            stream.CopyTo(gzip);
                        }
                    }
                    else
                    {
                        using (DeflateStream deflate = new DeflateStream(dest, CompressionLevel.Optimal))
                        {
                            stream.CopyTo(deflate);
                        }
                    }
                }
            }
        }
        public void Decompress(string compressType)
        {
            using (var stream = File.OpenWrite(Destination))
            {
                using (var dest = File.OpenRead(_srcPath))
                {
                    if (compressType == Resource.Brotli)
                    {
                        using (BrotliStream brotli = new BrotliStream(dest, CompressionMode.Decompress))
                        {
                            brotli.CopyTo(stream);
                        }
                    }
                    else if (compressType == Resource.GZip)
                    {
                        using (GZipStream gzip = new GZipStream(dest, CompressionMode.Decompress))
                        {
                            gzip.CopyTo(stream);
                        }
                    }
                    else
                    {
                        using (DeflateStream deflate = new DeflateStream(dest, CompressionMode.Decompress))
                        {
                            deflate.CopyTo(stream);
                        }
                    }
                }
            }
        }

    }
}
