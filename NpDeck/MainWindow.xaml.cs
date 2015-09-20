using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using NpDeck.Detectors;

namespace NpDeck
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private readonly Config _config = new Config();

		public Config Config
		{
			get { return _config; }
		}

		private readonly Timer _timer = new Timer {AutoReset = true, Interval = 5000};
		private string _formattedResult;
		private Result _lastResult;
		private string _statusText = "Hello!";
		public bool EnableWrite { get; set; }

		public Result LastResult
		{
			get { return _lastResult; }
			private set
			{
				_lastResult = value;
				OnPropertyChanged();
			}
		}

		public string FormattedResult
		{
			get { return _formattedResult; }
			private set
			{
				_formattedResult = value;
				OnPropertyChanged();
			}
		}

		public String StatusText
		{
			get { return _statusText; }
			set
			{
				_statusText = value;
				OnPropertyChanged();
			}
		}

		public MainWindow()
		{
			InitializeComponent();
			Config.Load();
			DataContext = this;
			_timer.Elapsed += TimerOnElapsed;
			_timer.Enabled = true;
			EnableWrite = true;
			DoDetect();
		}

		private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			DoDetect();
		}

		private void DoDetect()
		{
			Result result = null;
			foreach (var detector in _config.Detectors)
			{
				result = detector.Detect();
				if (result != null) break;
			}
			if (result == null)
			{
				if (LastResult == null)
				{
					StatusText = "Couldn't detect any track :(";
				}
				return;
			}
			if (LastResult != null && LastResult.Title == result.Title) return;
			LastResult = result;
			Reformat();
		}

		private void FormatChanged(object sender, TextChangedEventArgs e)
		{
			Reformat();
		}

		private void Reformat()
		{
			var formatted = Config.Format.Replace("{np}", LastResult != null ? LastResult.Title : String.Empty);
			if (formatted == FormattedResult) return;
			FormattedResult = formatted;
			if (EnableWrite)
			{
				SaveOutputFile();
			}
		}

		private void SaveOutputFile()
		{
			try
			{
				File.WriteAllText(Config.DestinationFilename, FormattedResult, new UTF8Encoding(false, false));
			}
			catch (Exception exc)
			{
				StatusText = string.Format("Couldn't save file: {0}", exc);
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		private void DestFileDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var sfd = new SaveFileDialog {FileName = Config.DestinationFilename};
			var res = sfd.ShowDialog(this);
			if (res.HasValue && res.Value)
			{
				Config.DestinationFilename = sfd.FileName;
			}
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			Config.Save();
		}
	}
}