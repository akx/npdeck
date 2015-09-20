using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NpDeck
{
	/// <summary>
	/// Interaction logic for FontPicker.xaml
	/// </summary>
	public partial class FontPicker : Window, INotifyPropertyChanged
	{
		private string _targetFontName = "Segoe UI";
		private float _targetFontSize = 16;

		public string TargetFontName
		{
			get { return _targetFontName; }
			set
			{
				_targetFontName = value;
				OnPropertyChanged();
			}
		}

		public float TargetFontSize
		{
			get { return _targetFontSize; }
			set
			{
				_targetFontSize = value;
				OnPropertyChanged();
			}
		}

		public FontPicker()
		{
			InitializeComponent();
			DataContext = this;
			foreach (var fontFamily in Fonts.SystemFontFamilies)
			{
				var familyName = fontFamily.FamilyNames.Values.First();
				foreach (var typeface in fontFamily.GetTypefaces())
				{
					FontBox.Items.Add(String.Format("{0} {1}", familyName, typeface.FaceNames.Values.First()));
				}
			}
		}

		public FontPicker(string fontName, int fontSize): this()
		{
			_targetFontName = fontName;
			_targetFontSize = fontSize;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
	}
}
