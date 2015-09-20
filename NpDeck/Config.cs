using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using NpDeck.Detectors;

namespace NpDeck
{
	public class Config : INotifyPropertyChanged
	{
		private string _format = "{np}";
		private string _textFilename;
		private string _fileEncoding = "UTF-8";
		private bool _enableText = false;
		private bool _enableImage = false;
		private string _imageFilename;
		private int _imageWidth = 800;
		private int _imageHeight = 100;
		private string _imageFontName = "Segoe UI";
		private int _imageFontSize = 16;
		private string _imageTextColor = "#ffffff";

		public static Dictionary<string, Encoding> Encodings = new Dictionary<string, Encoding>
		{
			{"UTF-8", new UTF8Encoding(false, false)},
			{"UTF-8 with BOM", new UTF8Encoding(true, false)},
			{"UTF-16 LE with BOM", new UnicodeEncoding(false, true, false)},
			{"UTF-16 LE without BOM", new UnicodeEncoding(false, false, false)},
			{"UTF-16 BE with BOM", new UnicodeEncoding(true, true, false)},
			{"UTF-16 BE without BOM", new UnicodeEncoding(true, false, false)},
			{"UTF-32 LE with BOM", new UTF32Encoding(false, true, false)},
			{"UTF-32 LE without BOM", new UTF32Encoding(false, false, false)},
			{"UTF-32 BE with BOM", new UTF32Encoding(true, true, false)},
			{"UTF-32 BE without BOM", new UTF32Encoding(true, false, false)},
			{"ASCII", new ASCIIEncoding()}
		};

		public string Format
		{
			get { return _format; }
			set
			{
				_format = value;
				OnPropertyChanged();
			}
		}

		public string TextFilename
		{
			get { return _textFilename; }
			set
			{
				_textFilename = value;
				OnPropertyChanged();
			}
		}

		public string FileEncoding
		{
			get { return _fileEncoding; }
			set {
				if (Encodings.ContainsKey(value))
				{
					_fileEncoding = value;
				}
				OnPropertyChanged();
			}
		}

		public bool EnableText
		{
			get { return _enableText; }
			set
			{
				_enableText = value;
				OnPropertyChanged();
			}
		}

		public string ImageFilename
		{
			get { return _imageFilename; }
			set
			{
				_imageFilename = value;
				OnPropertyChanged();
			}
		}

		public int ImageWidth
		{
			get { return _imageWidth; }
			set
			{
				_imageWidth = Math.Max(value, 1);
				OnPropertyChanged();
			}
		}

		public int ImageHeight
		{
			get { return _imageHeight; }
			set
			{
				_imageHeight = Math.Max(value, 1);
				OnPropertyChanged();
			}
		}

		public string ImageFontName
		{
			get { return _imageFontName; }
			set
			{
				_imageFontName = value;
				OnPropertyChanged();
			}
		}

		public int ImageFontSize
		{
			get { return _imageFontSize; }
			set
			{
				_imageFontSize = Math.Max(value, 1);
				OnPropertyChanged();
			}
		}

		public string ImageTextColor
		{
			get { return _imageTextColor; }
			set
			{
				_imageTextColor = value;
				OnPropertyChanged();
			}
		}
		public bool EnableImage
		{
			get { return _enableImage; }
			set
			{
				_enableImage = value;
				OnPropertyChanged();
			}
		}


		[MiniSerialize.Ignore]
		public List<IDetector> Detectors { get; private set; }

		public Config()
		{
			TextFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "npdeck.txt");
			ImageFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "npdeck.png");
			Detectors = new List<IDetector>
			{
				new Winamp(),
				new Foobar2000()
			};
			
		}

		public void Load()
		{
			var fileLocation = GetFileLocation();
			if (!File.Exists(fileLocation)) return;
			try
			{
				var xel = XElement.Parse(File.ReadAllText(fileLocation, Encoding.UTF8));
				MiniSerialize.Unserialize(xel, this);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
			}
		}

		public void Save()
		{
			var fileLocation = GetFileLocation();
			var xel = MiniSerialize.Serialize(this);
			

			using (XmlWriter xw = XmlWriter.Create(fileLocation, new XmlWriterSettings
			{
				Indent = true,
				Encoding = new UTF8Encoding(false, false),
				OmitXmlDeclaration = false
			}))
			{
				xel.WriteTo(xw);
			}
		}

		private static string GetFileLocation()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NpDeck.xml");
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		public Encoding GetCurrentEncoding()
		{
			Encoding enc;
			if (!Encodings.TryGetValue(FileEncoding, out enc))
			{
				enc = new UTF8Encoding(false, false); // Fallback to UTF-8
			}
			return enc;
		}
	}
}