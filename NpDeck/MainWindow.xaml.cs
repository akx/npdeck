using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using NpDeck.Detectors;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using Timer = System.Timers.Timer;

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
		private Throttle _debouncedReformat;

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
			DoDetect();
			foreach (var key in Config.Encodings.Keys)
			{
				EncodingSelect.Items.Add(key);
			}
			_debouncedReformat = new Throttle(Reformat, TimeSpan.FromMilliseconds(500));
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
			StatusText = String.Format("New track: {0}", result.Title);
			Reformat();
		}

		private void OutputSettingChanged(object sender, EventArgs e)
		{
			FormattedResult = String.Empty;  // Force reformat
			_debouncedReformat.Call();
		}

		private void Reformat()
		{
			var formatted = Config.Format.Replace("{np}", LastResult != null ? LastResult.Title : String.Empty);
			if (formatted == FormattedResult) return;
			FormattedResult = formatted;
			if (_config.EnableText)
			{
				SaveText();
			}
			if (_config.EnableImage)
			{
				SaveImage();
			}
		}

		private bool SaveImage()
		{
			try
			{
				ImageRenderer.Render(FormattedResult, _config);
				return true;
			}
			catch (Exception exc)
			{
				StatusText = string.Format("Couldn't save image: {0}", exc);
				if (Debugger.IsAttached) throw;
				return false;
			}
		}

		private bool SaveText()
		{
			try
			{
				File.WriteAllText(Config.TextFilename, FormattedResult, Config.GetCurrentEncoding());
				return true;
			}
			catch (Exception exc)
			{
				StatusText = string.Format("Couldn't save text: {0}", exc);
				if (Debugger.IsAttached) throw;
				return false;
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
			var sfd = new SaveFileDialog {FileName = Config.TextFilename};
			var res = sfd.ShowDialog(this);
			if (res.Value)
			{
				Config.TextFilename = sfd.FileName;
			}
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			Config.Save();
		}

		private void PickTextColorClick(object sender, RoutedEventArgs e)
		{
			Color color = Color.FromRgb(255, 255, 255);
			try
			{
				color = (Color) ColorConverter.ConvertFromString(_config.ImageTextColor);
			}
			catch (NullReferenceException nre)
			{
				
			}

			var cd = new ColorDialog {AnyColor = true, Color = System.Drawing.Color.FromArgb(color.R, color.G, color.B), FullOpen = true};
			if (cd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
			_config.ImageTextColor = Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B).ToString();
			Reformat();
		}

		private void Redraw_Click(object sender, RoutedEventArgs e)
		{
			if (SaveImage())
			{
				StatusText = "Image saved.";
			}
		}

		private void PickFont_Click(object sender, RoutedEventArgs e)
		{
			var fp = new FontPicker(_config.ImageFontName, _config.ImageFontSize);
			var result = fp.ShowDialog();
			if (result != null && result.Value == true)
			{
				_config.ImageFontName = fp.TargetFontName;
				_config.ImageFontSize = (int)Math.Round(fp.TargetFontSize);	
			}
			Reformat();
		}
	}
}