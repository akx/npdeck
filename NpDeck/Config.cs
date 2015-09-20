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
		private string _format;
		private string _destinationFilename;


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

		[MiniSerialize.Ignore]
		public List<IDetector> Detectors { get; private set; }

		public Config()
		{
			Format = "{np}";
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
			using (XmlWriter xw = new XmlTextWriter(fileLocation, new UTF8Encoding(false, false)))
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
	}
}