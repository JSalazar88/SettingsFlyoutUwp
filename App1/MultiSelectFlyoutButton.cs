using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace App1
{
	[TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
	[TemplateVisualState(GroupName = "CommonStates", Name = "PointerOver")]
	[TemplatePart(Name = "PART_FlyoutListView", Type = typeof(ListView))]
	public sealed class MultiSelectFlyoutButton : Button, INotifyPropertyChanged
	{
		// -- public properties --

		public ListView FlyoutListView { get; set; }

		public Flyout ListViewFlyout { get; set; }

		#region Dependency Properties and Callbacks

		// ////////////////////////////////////////////////////////////////////////////////////////
		// Dependency Prop: FlyoutSource
		// ////////////////////////////////////////////////////////////////////////////////////////

		public static DependencyProperty FlyoutSourceProperty = DependencyProperty.Register(
		"FlyoutSource", typeof(List<string>), typeof(MultiSelectFlyoutButton),
		new PropertyMetadata(null, new PropertyChangedCallback(OnFlyoutSourceChanged)));

		public List<string> FlyoutSource
		{
			get { return (List<string>)GetValue(FlyoutSourceProperty); }
			set { SetValue(FlyoutSourceProperty, value); }
		}

		public static void OnFlyoutSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var btn = obj as MultiSelectFlyoutButton;
			if ( btn == null ) return;

			// set new flyoutsource as ItemsSource of a lv
			var lv = new ListView();
			lv.ItemsSource = e.NewValue as List<string>; // todo: replace w/object type;

			//  set the ListViewFlyout content to the lv
			btn.ListViewFlyout = new Flyout() { Content = lv };
		}

		// -- public methods --

		public MultiSelectFlyoutButton()
		{
			this.DefaultStyleKey = typeof(Button);
		}

		public void ShowFlyout()
		{
			ListViewFlyout?.ShowAt(this);
		}

		// -- protected methods --

		protected override void OnTapped(TappedRoutedEventArgs e)
		{
			base.OnTapped(e);
			ShowFlyout();
		}

		#endregion

		#region INotifyPropertyChanged Helpers

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		#endregion
	}
}

public sealed class AccessibleButton : Control
{
	public AccessibleButton()
	{
		this.DefaultStyleKey = typeof(AccessibleButton);
	}

	public static DependencyProperty LabelProperty = DependencyProperty.Register(
		"Label", typeof(string), typeof(AccessibleButton),
		PropertyMetadata.Create(string.Empty));

	public string Label
	{
		set { SetValue(LabelProperty, value); }
		get { return (string)GetValue(LabelProperty); }
	}

	protected override void OnPointerPressed(PointerRoutedEventArgs e)
	{
		Click?.Invoke(this, EventArgs.Empty);
	}

	public event EventHandler Click;
}