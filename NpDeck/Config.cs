using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using NpDeck.Detectors;

namespace NpDeck
{
	public class Config : INotifyPropertyChanged
	{
		private string _format = "{np}";
		private string _destinationFilename;
		private string _fileEncoding = "UTF-8";

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

		public string DestinationFilename
		{
			get { return _destinationFilename; }
			set
			{
				_destinationFilename = value;
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


		[MiniSerialize.Ignore]
		public List<IDetector> Detectors { get; private set; }

		public Config()
		{
			DestinationFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
				"npdeck.txt");
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