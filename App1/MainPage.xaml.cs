using Callisto.TestApp.SamplePages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
		public List<LibraryItem> Libraries { get; set; } = new List<LibraryItem>()
		{
			new LibraryItem("Some Super Ultra Long library display name"),
			new LibraryItem("Lib0"),
			new LibraryItem("Lib0"),

		};
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			//if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		private bool _isOpen = false;
		public bool IsOpen
		{
			get { return _isOpen; }
			set
			{
				SetField(ref _isOpen, value);
			}
		}

		public System.Windows.Input.ICommand OpenCommand { get; set; }
		public System.Windows.Input.ICommand CloseCommand { get; set; }

		public MainPage()
        {
            this.InitializeComponent();
			OpenCommand = new RelayCommand(obj => true, (obj) =>
			{
				this.IsOpen = true;
				Callisto.Controls.SettingsFlyout settings = new Callisto.Controls.SettingsFlyout();
				settings.FlyoutWidth = Callisto.Controls.SettingsFlyout.SettingsFlyoutWidth.Narrow;
				settings.HeaderBrush = new SolidColorBrush(Colors.Orange);
				settings.HeaderText = string.Format("{0} Custom Settings", "Test App");
				settings.SmallLogoImageSource = new BitmapImage(new Uri("http://vignette2.wikia.nocookie.net/silkroad-online/images/a/a9/Example.jpg", UriKind.Absolute));
				settings.Content = new SettingsContent();
				settings.IsOpen = true;

			}
			);
			CloseCommand = new RelayCommand(obj => true, (obj) => this.IsOpen = false);
		}

		private void button_Tapped(object sender, TappedRoutedEventArgs e)
		{
			FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
		}
	}

	public class LibraryItem
	{
		public string LibraryName { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			//if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		private bool _isOpen = false;
		public bool IsOpen
		{
			get { return _isOpen; }
			set
			{
				SetField(ref _isOpen, value);
			}
		}

		public System.Windows.Input.ICommand OpenCommand { get; set; }
		public System.Windows.Input.ICommand CloseCommand { get; set; }

		public LibraryItem(string name)
		{
			LibraryName = name;
			OpenCommand = new RelayCommand(obj => true, (obj) => this.IsOpen = true);
			CloseCommand = new RelayCommand(obj => true, (obj) => this.IsOpen = false);
		}
	}

	public class RelayCommand : System.Windows.Input.ICommand
	{
		private Predicate<object> _canExecute;
		private Action<object> _execute;

		public RelayCommand(Predicate<object> canExecute, Action<object> execute)
		{
			this._canExecute = canExecute;
			this._execute = execute;
		}

		public event EventHandler CanExecuteChanged;

		public void RaiseCanExecuteChanged()
		{
			if (CanExecuteChanged != null)
				CanExecuteChanged(this, EventArgs.Empty);
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			_execute(parameter);
		}
	}
}
