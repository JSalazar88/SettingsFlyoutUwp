using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace App1
{
	/// <summary>
	/// Helper class providing attached properties allowing the user to add a <see cref="FlyoutBase.AttachedFlyoutProperty"/> to
	/// ANY item of type <see cref="FrameworkElement"/>, by binding the <see cref="ParentProperty"/> to it, and controlling its
	/// behavior by simply binding the <see cref="IsVisibleProperty"/> to a ViewModel property.
	/// </summary>
	public static class FlyoutDialogHelper
	{
		// ////////////////////////////////////////////////////////////////////////////////////////
		// IsOpen
		// ////////////////////////////////////////////////////////////////////////////////////////

		public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached(
			"IsOpen", typeof(bool), typeof(FlyoutDialogHelper),
			new PropertyMetadata(true, IsOpenChangedCallback));

		public static void SetIsOpen(DependencyObject element, bool value)
		{
			element.SetValue(IsVisibleProperty, value);
		}

		public static bool GetIsOpen(DependencyObject element)
		{
			return (bool)element.GetValue(IsVisibleProperty);
		}

		private static void IsOpenChangedCallback(DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			var fb = d as FlyoutBase;
			if (fb == null)
				return;

			if ((bool)e.NewValue)
			{
				fb.Closed += flyout_Closed;
				fb.ShowAt(GetParent(d));
			}
			else
			{
				fb.Closed -= flyout_Closed;
				fb.Hide();
			}
		}

		// ////////////////////////////////////////////////////////////////////////////////////////
		// Parent
		// ////////////////////////////////////////////////////////////////////////////////////////

		public static readonly DependencyProperty ParentProperty = DependencyProperty.RegisterAttached(
			"Parent", typeof(FrameworkElement), typeof(FlyoutDialogHelper), null);

		public static void SetParent(DependencyObject element, FrameworkElement value)
		{
			element.SetValue(ParentProperty, value);
		}

		public static FrameworkElement GetParent(DependencyObject element)
		{
			return (FrameworkElement)element.GetValue(ParentProperty);
		}

		// ////////////////////////////////////////////////////////////////////////////////////////
		// FlyouSourceList
		// ////////////////////////////////////////////////////////////////////////////////////////

		public static readonly DependencyProperty FlyoutSourceListProperty = DependencyProperty.RegisterAttached(
			"FlyoutSourceList", typeof(List<string>), typeof(FlyoutDialogHelper),
			new PropertyMetadata(null, FlyoutSourceListChangedCallback));

		public static void SetFlyoutSourceList(DependencyObject element, List<string> value)
		{
			element.SetValue(FlyoutSourceListProperty, value);
		}

		public static List<string> GetFlyoutSourceList(DependencyObject element)
		{
			return (List<string>)element.GetValue(FlyoutSourceListProperty);
		}

		private static void FlyoutSourceListChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Debug.WriteLine("");
		}

		// ////////////////////////////////////////////////////////////////////////////////////////
		// Helper methods
		// ////////////////////////////////////////////////////////////////////////////////////////

		private static void flyout_Closed(object sender, object e)
		{
			// When flyout is closed, sets IsOpen
			SetIsOpen(sender as DependencyObject, false);
		}
	}

	/// <summary>/// Attached Property for sharing a set of VisualStateGroups between elements/// </summary>
	public class VisualStateExtensions : DependencyObject
	{
		public static void SetVisualStatefromTemplate(UIElement element, DataTemplate value)
		{
			element.SetValue(VisualStatefromTemplateProperty, value);
		}
		public static DataTemplate GetVisualStatefromTemplate(UIElement element)
		{
			return (DataTemplate)element.GetValue(VisualStatefromTemplateProperty);
		}
		public static readonly DependencyProperty VisualStatefromTemplateProperty =
		  DependencyProperty.RegisterAttached("VisualStatefromTemplate",
			  typeof(DataTemplate), typeof(VisualStateExtensions), new PropertyMetadata(null, VisualStatefromTemplateChanged));

		private static void VisualStatefromTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var visualstategroups = VisualStateManager.GetVisualStateGroups(d as Grid);
			var template = (DataTemplate)e.NewValue;
			var content = ((FrameworkElement)template.LoadContent()) as Grid;
			var source = VisualStateManager.GetVisualStateGroups(content);
			var original = source.First();
			source.RemoveAt(0);

			visualstategroups.Add(original);
		}
	}

	/// <summary>/// Helper class to set an attached property, from within a style property setter, to some local property/// </summary>
	public class BindingHelper
	{
		public static readonly DependencyProperty GridColumnBindingPathProperty =
			DependencyProperty.RegisterAttached(
				"GridColumnBindingPath", typeof(string), typeof(BindingHelper),
				new PropertyMetadata(null, GridBindingPathPropertyChanged));

		public static readonly DependencyProperty GridRowBindingPathProperty =
			DependencyProperty.RegisterAttached(
				"GridRowBindingPath", typeof(string), typeof(BindingHelper),
				new PropertyMetadata(null, GridBindingPathPropertyChanged));

		public static string GetGridColumnBindingPath(DependencyObject obj)
		{
			return (string)obj.GetValue(GridColumnBindingPathProperty);
		}

		public static void SetGridColumnBindingPath(DependencyObject obj, string value)
		{
			obj.SetValue(GridColumnBindingPathProperty, value);
		}

		public static string GetGridRowBindingPath(DependencyObject obj)
		{
			return (string)obj.GetValue(GridRowBindingPathProperty);
		}

		public static void SetGridRowBindingPath(DependencyObject obj, string value)
		{
			obj.SetValue(GridRowBindingPathProperty, value);
		}

		// ////////////////////////////////////////////////////////////////////////////////////////
		// FlyouSourceList
		// ////////////////////////////////////////////////////////////////////////////////////////

		public static readonly DependencyProperty FlyoutSourceBindingPathProperty =
			DependencyProperty.RegisterAttached(
			"FlyoutSourceBindingPath", typeof(string), typeof(BindingHelper),
			new PropertyMetadata(null, FlyoutSourceBindingPathChangedCallback));

		public static void SetFlyoutSourceBindingPath(DependencyObject element, string value)
		{
			element.SetValue(FlyoutSourceBindingPathProperty, value);
		}

		public static string GetFlyoutSourceBindingPathList(DependencyObject element)
		{
			return (string)element.GetValue(FlyoutSourceBindingPathProperty);
		}

		private static void FlyoutSourceBindingPathChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var propertyPath = e.NewValue as string;

			if (propertyPath != null)
			{
				var property =
					e.Property == FlyoutSourceBindingPathProperty
					? ListView.ItemsSourceProperty
					: null;

				BindingOperations.SetBinding(
					d,
					property,
					new Binding { Path = new PropertyPath(propertyPath) });
			}
		}

		// ////////////////////////////////////////////////////////////////////////////////////////
		// Helper methods
		// ////////////////////////////////////////////////////////////////////////////////////////

		private static void GridBindingPathPropertyChanged(
			DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var propertyPath = e.NewValue as string;

			if (propertyPath != null)
			{
				var gridProperty =
					e.Property == GridColumnBindingPathProperty
					? Grid.ColumnProperty
					: Grid.RowProperty;

				BindingOperations.SetBinding(
					obj,
					gridProperty,
					new Binding { Path = new PropertyPath(propertyPath) });
			}
		}
	}
}

//
// Copyright (c) 2012 Tim Heuer
//
// Licensed under the Microsoft Public License (Ms-PL) (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://opensource.org/licenses/Ms-PL.html
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

namespace Callisto.Controls
{
	/// <summary>
	/// OBSOLETE. This control is a helper control to provide the Settings charm experience for custom settings within your app.
	/// It follows the specific Windows UI guidelines for layout and entrance/exit/positioning animations. It is a
	/// content control for your settings that are app-wide or contextual to your current view.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This control is deprecated in favor of using the <see cref="Windows.UI.Xaml.Controls.SettingsFlyout"/> control in Windows 8.1.
	/// </para>
	/// <para>
	/// This control is a helper control to provide the Settings charm experience for custom settings within your app. 
	/// It follows the specific Windows UI guidelines for layout and entrance/exit/positioning animations. It is a 
	/// content control for your settings that are app-wide or contextual to your current view.
	/// </para>
	/// </remarks>
	[Obsolete("Windows 8.1 now provides this functionality in the XAML framework itself as SettingsFlyout.")]
	public sealed class SettingsFlyout : ContentControl
	{
		#region Member Variables
		private Popup _hostPopup;
		private Rect _windowBounds;
		private double _settingsWidth;
		private Button _backButton;
		private Grid _contentGrid;
		private Border _rootBorder;
		private ScrollViewer _contentScrollViewer;
		const int CONTENT_HORIZONTAL_OFFSET = 100;
		private bool _ihmFocusMoved = false;
		private double _ihmOccludeHeight = 0.0;
		#endregion Member Variables

		#region Constants

		private const string PART_BACK_BUTTON = "SettingsBackButton";
		private const string PART_CONTENT_GRID = "SettingsFlyoutContentGrid";
		private const string PART_ROOT_BORDER = "PART_RootBorder";
		private const string PART_CONTENT_SCROLLVIEWER = "PART_ContentScrollViewer";
		#endregion

		#region Overrides
		/// <summary>
		/// Invoked whenever application code or internal processes (such as a rebuilding layout pass) 
		/// call ApplyTemplate. In simplest terms, this means the method is called just before a UI 
		/// element displays in your app. Override this method to influence the default post-template
		/// logic of a class.
		/// </summary>
		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			// make sure we listen at the right time to add/remove the back button event handlers
			if (_backButton != null)
			{
				_backButton.Click -= OnBackButtonClicked;
			}
			_backButton = GetTemplateChild(PART_BACK_BUTTON) as Button;
			if (_backButton != null)
			{
				_backButton.Click += OnBackButtonClicked;
			}

			// need to get these grids in order to set the offsets correctly in RTL situations
			if (_contentGrid == null)
			{
				_contentGrid = GetTemplateChild(PART_CONTENT_GRID) as Grid;
			}
			if (_contentGrid != null)
			{
				_contentGrid.Transitions = new TransitionCollection();
				_contentGrid.Transitions.Add(new EntranceThemeTransition()
				{
					FromHorizontalOffset = (false) ? CONTENT_HORIZONTAL_OFFSET : (CONTENT_HORIZONTAL_OFFSET * -1)
				});
			}

			// need the root border for RTL scenarios
			_rootBorder = GetTemplateChild(PART_ROOT_BORDER) as Border;

			// need the content scrollviewer to set the fixed width to be the same size as flyout
			_contentScrollViewer = GetTemplateChild(PART_CONTENT_SCROLLVIEWER) as ScrollViewer;

		}
		#endregion Overrides

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="SettingsFlyout"/> class.
		/// </summary>
		public SettingsFlyout()
		{
			this.DefaultStyleKey = typeof(SettingsFlyout);

			_windowBounds = Window.Current.Bounds;

			this.Loaded += OnLoaded;

			_hostPopup = new Popup();
			_hostPopup.ChildTransitions = new TransitionCollection();
			_hostPopup.ChildTransitions.Add(new PaneThemeTransition() { Edge = (false) ? EdgeTransitionLocation.Right : EdgeTransitionLocation.Left });
			_hostPopup.Closed += OnHostPopupClosed;
			_hostPopup.IsLightDismissEnabled = true;
			_hostPopup.Height = _windowBounds.Height;
			_hostPopup.Child = this;
			_hostPopup.SetValue(Canvas.TopProperty, 0);

			this.Height = _windowBounds.Height;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			Window.Current.Activated += OnCurrentWindowActivated;
			Window.Current.SizeChanged += OnCurrentWindowSizeChanged;

			// in RTL languages on the OS, the SettingsPane comes from the left edge
			if (true)
			{
				if (_rootBorder != null) _rootBorder.BorderThickness = new Thickness(0, 0, 1, 0);
			}

			_settingsWidth = (double)this.FlyoutWidth;

			// setting all the widths to be the size of flyout
			_hostPopup.Width = this.Width = _contentScrollViewer.Width = _settingsWidth;

			// ensure it comes from the correct edge location
			_hostPopup.SetValue(Canvas.LeftProperty, false ? (_windowBounds.Width - _settingsWidth) : 0);

			// handling the case where it isn't parented to the visual tree
			// inspect the visual root and adjust.
			if (_hostPopup.Parent == null)
			{
				Windows.UI.ViewManagement.InputPane.GetForCurrentView().Showing += OnInputPaneShowing;
				Windows.UI.ViewManagement.InputPane.GetForCurrentView().Hiding += OnInputPaneHiding;
			}
		}

		private void OnCurrentWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
		{
			IsOpen = false;
		}

		private void OnInputPaneHiding(Windows.UI.ViewManagement.InputPane sender, Windows.UI.ViewManagement.InputPaneVisibilityEventArgs args)
		{
			// if the ihm occluded something and we had to move, we need to adjust back
			if (_ihmFocusMoved)
			{
				_hostPopup.VerticalOffset += _ihmOccludeHeight; // ensure defaults back to normal
				_ihmFocusMoved = false;
			}
		}

		private void OnInputPaneShowing(Windows.UI.ViewManagement.InputPane sender, Windows.UI.ViewManagement.InputPaneVisibilityEventArgs args)
		{
			FrameworkElement focusedItem = FocusManager.GetFocusedElement() as FrameworkElement;

			if (focusedItem != null)
			{
				// if the focused item is within height - occludedrect height - buffer(50)
				// then it doesn't need to be changed
				GeneralTransform gt = focusedItem.TransformToVisual(Window.Current.Content);

				Rect focusedRect = gt.TransformBounds(new Rect(0.0, 0.0, focusedItem.ActualWidth, focusedItem.ActualHeight));

				if (focusedRect.Bottom > (_windowBounds.Height - args.OccludedRect.Top))
				{
					_ihmFocusMoved = true;
					_ihmOccludeHeight = focusedRect.Top < (int)args.OccludedRect.Top ? focusedRect.Top : args.OccludedRect.Top;
					_hostPopup.VerticalOffset -= _ihmOccludeHeight;
				}
			}
		}

		/// <summary>
		/// Occurs when back button is clicked.
		/// </summary>
		public event EventHandler<BackClickedEventArgs> BackClicked;

		private void InvokeOnBackClick(BackClickedEventArgs args)
		{
			var handler = BackClicked;
			if (handler != null) handler(this, args);
		}

		private void OnBackButtonClicked(object sender, object e)
		{
			var backEventArgs = new BackClickedEventArgs
			{
				Cancel = false
			};

			InvokeOnBackClick(backEventArgs);

			if (backEventArgs.Cancel) return;

			if (_hostPopup != null)
			{
				_hostPopup.IsOpen = false;
			}

			// TEMP: wrapping this to ensure back button doesn't happen in snap/portrait
			if (ApplicationView.Value != ApplicationViewState.Snapped)
			{
				//SettingsPane.Show();
			}

		}

		void OnHostPopupClosed(object sender, object e)
		{
			_hostPopup.Child = null;
			Window.Current.Activated -= OnCurrentWindowActivated;
			Window.Current.SizeChanged -= OnCurrentWindowSizeChanged;
			Windows.UI.ViewManagement.InputPane.GetForCurrentView().Showing -= OnInputPaneShowing;
			Windows.UI.ViewManagement.InputPane.GetForCurrentView().Hiding -= OnInputPaneHiding;
			this.Content = null;

			if (null != Closed)
			{
				Closed(this, e);
			}

			IsOpen = false;
		}

		void OnCurrentWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
		{
			if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
			{
				this.IsOpen = false;
			}
		}

		/// <summary>
		/// Gets a reference the to popup.
		/// </summary>
		public Popup HostPopup { get { return _hostPopup; } }
		#endregion Constructor

		#region Dependency Properties
		/// <summary>
		/// Gets or sets a value indicating whether the flyout is open.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is open; otherwise, <c>false</c>.
		/// </value>
		public bool IsOpen
		{
			get { return (bool)GetValue(IsOpenProperty); }
			set { SetValue(IsOpenProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="IsOpen"/> dependency property
		/// </summary>
		public static readonly DependencyProperty IsOpenProperty =
			DependencyProperty.Register("IsOpen", typeof(bool), typeof(SettingsFlyout), new PropertyMetadata(false, (obj, args) =>
			{
				if (args.NewValue != args.OldValue)
				{
					SettingsFlyout f = (SettingsFlyout)obj;
					f._hostPopup.IsOpen = (bool)args.NewValue;
				}
			}));

		/// <summary>
		/// Gets or sets the header brush.
		/// </summary>
		public SolidColorBrush HeaderBrush
		{
			get { return (SolidColorBrush)GetValue(HeaderBrushProperty); }
			set { SetValue(HeaderBrushProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="HeaderBrush"/> dependency property
		/// </summary>
		public static readonly DependencyProperty HeaderBrushProperty =
			DependencyProperty.Register("HeaderBrush", typeof(SolidColorBrush), typeof(SettingsFlyout), new PropertyMetadata(null, OnHeaderBrushColorChanged));

		private static void OnHeaderBrushColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			// determine the contrast and set black or white
			if (e.OldValue != e.NewValue)
			{
				SolidColorBrush newBrush = e.NewValue as SolidColorBrush;
				if (newBrush != null)
				{
					var yiq = ((newBrush.Color.R * 299) + (newBrush.Color.G * 587) + (newBrush.Color.B * 114)) / 1000;

					Debug.WriteLine(yiq >= 128 ? "HeaderText: black" : "HeaderText: white");
				}
			}
		}

		/// <summary>
		/// Gets or sets the width of the flyout.
		/// </summary>
		public SettingsFlyoutWidth FlyoutWidth
		{
			get { return (SettingsFlyoutWidth)GetValue(FlyoutWidthProperty); }
			set { SetValue(FlyoutWidthProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="FlyoutWidth"/> dependency property
		/// </summary>
		public static readonly DependencyProperty FlyoutWidthProperty =
			DependencyProperty.Register("FlyoutWidth", typeof(SettingsFlyoutWidth), typeof(SettingsFlyout), new PropertyMetadata(SettingsFlyoutWidth.Narrow));


		/// <summary>
		/// Gets or sets the header text.
		/// </summary>
		public string HeaderText
		{
			get { return (string)GetValue(HeaderTextProperty); }
			set { SetValue(HeaderTextProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="HeaderText"/> dependency property
		/// </summary>
		public static readonly DependencyProperty HeaderTextProperty =
			DependencyProperty.Register("HeaderText", typeof(string), typeof(SettingsFlyout), new PropertyMetadata(null));


		/// <summary>
		/// Gets or sets the small logo image source.
		/// </summary>
		public ImageSource SmallLogoImageSource
		{
			get { return (ImageSource)GetValue(SmallLogoImageSourceProperty); }
			set { SetValue(SmallLogoImageSourceProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="SmallLogoImageSource"/> dependency property
		/// </summary>
		public static readonly DependencyProperty SmallLogoImageSourceProperty =
			DependencyProperty.Register("SmallLogoImageSource", typeof(ImageSource), typeof(SettingsFlyout), null);

		/* Issue #81 required these back in to enable overriding to ensure existing
         * apps would be able to retain their existing colors if they were expecting the old defaults
         * */
		/// <summary>
		/// Gets or sets the content foreground brush.
		/// </summary>
		public SolidColorBrush ContentForegroundBrush
		{
			get { return (SolidColorBrush)GetValue(ContentForegroundBrushProperty); }
			set { SetValue(ContentForegroundBrushProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="ContentForegroundBrush"/> dependency property
		/// </summary>
		public static readonly DependencyProperty ContentForegroundBrushProperty =
			DependencyProperty.Register("ContentForegroundBrush", typeof(SolidColorBrush), typeof(SettingsFlyout), null);

		/// <summary>
		/// Gets or sets the content background brush.
		/// </summary>
		public SolidColorBrush ContentBackgroundBrush
		{
			get { return (SolidColorBrush)GetValue(ContentBackgroundBrushProperty); }
			set { SetValue(ContentBackgroundBrushProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="ContentBackgroundBrush"/> dependency property
		/// </summary>
		public static readonly DependencyProperty ContentBackgroundBrushProperty =
			DependencyProperty.Register("ContentBackgroundBrush", typeof(SolidColorBrush), typeof(SettingsFlyout), null);

		#endregion Dependency Properties

		#region Events
		/// <summary>
		/// Occurs when the flyout was closed.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Runtime EventHandler")]
		public event EventHandler<object> Closed;
		#endregion

		#region Enums
		/// <summary>
		/// Flyout width enumeration
		/// </summary>
		public enum SettingsFlyoutWidth
		{
			/// <summary>
			/// A narrow flyout at 346 pixels
			/// </summary>
			Narrow = 346,
			/// <summary>
			/// A wide flyout at 646 pixels
			/// </summary>
			Wide = 646
		}
		#endregion Enums
	}

	/// <summary>
	/// Event argument for the <see cref="E:SetttingsFlyout.BackClicked"/> event.
	/// </summary>
	public class BackClickedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="BackClickedEventArgs"/> should be cancelled.
		/// </summary>
		public bool Cancel { get; set; }
	}
}

namespace LinqToVisualTree
{
	/// <summary>
	/// Adapts a DependencyObject to provide methods required for generate
	/// a Linq To Tree API
	/// </summary>
	public class VisualTreeAdapter : ILinqTree<DependencyObject>
	{
		private DependencyObject _item;

		public VisualTreeAdapter(DependencyObject item)
		{
			_item = item;
		}

		public IEnumerable<DependencyObject> Children()
		{
			int childrenCount = VisualTreeHelper.GetChildrenCount(_item);
			for (int i = 0; i < childrenCount; i++)
			{
				yield return VisualTreeHelper.GetChild(_item, i);
			}
		}

		public DependencyObject Parent
		{
			get
			{
				return VisualTreeHelper.GetParent(_item);
			}
		}
	}
}

namespace LinqToVisualTree
{
	/// <summary>
	/// Defines an interface that must be implemented to generate the LinqToTree methods
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ILinqTree<T>
	{
		IEnumerable<T> Children();

		T Parent { get; }
	}

	public static class TreeExtensions
	{
		/// <summary>
		/// Returns a collection of descendant elements.
		/// </summary>
		public static IEnumerable<DependencyObject> Descendants(this DependencyObject item)
		{
			ILinqTree<DependencyObject> adapter = new VisualTreeAdapter(item);
			foreach (var child in adapter.Children())
			{
				yield return child;

				foreach (var grandChild in child.Descendants())
				{
					yield return grandChild;
				}
			}
		}

		/// <summary>
		/// Returns a collection containing this element and all descendant elements.
		/// </summary>
		public static IEnumerable<DependencyObject> DescendantsAndSelf(this DependencyObject item)
		{
			yield return item;

			foreach (var child in item.Descendants())
			{
				yield return child;
			}
		}

		/// <summary>
		/// Returns a collection of ancestor elements.
		/// </summary>
		public static IEnumerable<DependencyObject> Ancestors(this DependencyObject item)
		{
			ILinqTree<DependencyObject> adapter = new VisualTreeAdapter(item);

			var parent = adapter.Parent;
			while (parent != null)
			{
				yield return parent;
				adapter = new VisualTreeAdapter(parent);
				parent = adapter.Parent;
			}
		}

		/// <summary>
		/// Returns a collection containing this element and all ancestor elements.
		/// </summary>
		public static IEnumerable<DependencyObject> AncestorsAndSelf(this DependencyObject item)
		{
			yield return item;

			foreach (var ancestor in item.Ancestors())
			{
				yield return ancestor;
			}
		}

		/// <summary>
		/// Returns a collection of child elements.
		/// </summary>
		public static IEnumerable<DependencyObject> Elements(this DependencyObject item)
		{
			ILinqTree<DependencyObject> adapter = new VisualTreeAdapter(item);
			foreach (var child in adapter.Children())
			{
				yield return child;
			}
		}

		/// <summary>
		/// Returns a collection of the sibling elements before this node, in document order.
		/// </summary>
		public static IEnumerable<DependencyObject> ElementsBeforeSelf(this DependencyObject item)
		{
			if (item.Ancestors().FirstOrDefault() == null)
				yield break;
			foreach (var child in item.Ancestors().First().Elements())
			{
				if (child.Equals(item))
					break;
				yield return child;
			}
		}

		/// <summary>
		/// Returns a collection of the after elements after this node, in document order.
		/// </summary>
		public static IEnumerable<DependencyObject> ElementsAfterSelf(this DependencyObject item)
		{
			if (item.Ancestors().FirstOrDefault() == null)
				yield break;
			bool afterSelf = false;
			foreach (var child in item.Ancestors().First().Elements())
			{
				if (afterSelf)
					yield return child;

				if (child.Equals(item))
					afterSelf = true;
			}
		}

		/// <summary>
		/// Returns a collection containing this element and all child elements.
		/// </summary>
		public static IEnumerable<DependencyObject> ElementsAndSelf(this DependencyObject item)
		{
			yield return item;

			foreach (var child in item.Elements())
			{
				yield return child;
			}
		}

		/// <summary>
		/// Returns a collection of descendant elements which match the given type.
		/// </summary>
		public static IEnumerable<DependencyObject> Descendants<T>(this DependencyObject item)
		{
			return item.Descendants().Where(i => i is T).Cast<DependencyObject>();
		}



		/// <summary>
		/// Returns a collection of the sibling elements before this node, in document order
		/// which match the given type.
		/// </summary>
		public static IEnumerable<DependencyObject> ElementsBeforeSelf<T>(this DependencyObject item)
		{
			return item.ElementsBeforeSelf().Where(i => i is T).Cast<DependencyObject>();
		}

		/// <summary>
		/// Returns a collection of the after elements after this node, in document order
		/// which match the given type.
		/// </summary>
		public static IEnumerable<DependencyObject> ElementsAfterSelf<T>(this DependencyObject item)
		{
			return item.ElementsAfterSelf().Where(i => i is T).Cast<DependencyObject>();
		}

		/// <summary>
		/// Returns a collection containing this element and all descendant elements
		/// which match the given type.
		/// </summary>
		public static IEnumerable<DependencyObject> DescendantsAndSelf<T>(this DependencyObject item)
		{
			return item.DescendantsAndSelf().Where(i => i is T).Cast<DependencyObject>();
		}

		/// <summary>
		/// Returns a collection of ancestor elements which match the given type.
		/// </summary>
		public static IEnumerable<DependencyObject> Ancestors<T>(this DependencyObject item)
		{
			return item.Ancestors().Where(i => i is T).Cast<DependencyObject>();
		}

		/// <summary>
		/// Returns a collection containing this element and all ancestor elements
		/// which match the given type.
		/// </summary>
		public static IEnumerable<DependencyObject> AncestorsAndSelf<T>(this DependencyObject item)
		{
			return item.AncestorsAndSelf().Where(i => i is T).Cast<DependencyObject>();
		}

		/// <summary>
		/// Returns a collection of child elements which match the given type.
		/// </summary>
		public static IEnumerable<DependencyObject> Elements<T>(this DependencyObject item)
		{
			return item.Elements().Where(i => i is T).Cast<DependencyObject>();
		}

		/// <summary>
		/// Returns a collection containing this element and all child elements.
		/// which match the given type.
		/// </summary>
		public static IEnumerable<DependencyObject> ElementsAndSelf<T>(this DependencyObject item)
		{
			return item.ElementsAndSelf().Where(i => i is T).Cast<DependencyObject>();
		}

	}

	public static class EnumerableTreeExtensions
	{
		/// <summary>
		/// Applies the given function to each of the items in the supplied
		/// IEnumerable.
		/// </summary>
		private static IEnumerable<DependencyObject> DrillDown(this IEnumerable<DependencyObject> items,
			Func<DependencyObject, IEnumerable<DependencyObject>> function)
		{
			foreach (var item in items)
			{
				foreach (var itemChild in function(item))
				{
					yield return itemChild;
				}
			}
		}


		/// <summary>
		/// Applies the given function to each of the items in the supplied
		/// IEnumerable, which match the given type.
		/// </summary>
		public static IEnumerable<DependencyObject> DrillDown<T>(this IEnumerable<DependencyObject> items,
			Func<DependencyObject, IEnumerable<DependencyObject>> function)
			where T : DependencyObject
		{
			foreach (var item in items)
			{
				foreach (var itemChild in function(item))
				{
					if (itemChild is T)
					{
						yield return (T)itemChild;
					}
				}
			}
		}


		/// <summary>
		/// Returns a collection of descendant elements.
		/// </summary>
		public static IEnumerable<DependencyObject> Descendants(this IEnumerable<DependencyObject> items)
		{
			return items.DrillDown(i => i.Descendants());
		}

		/// <summary>
		/// Returns a collection containing this element and all descendant elements.
		/// </summary>
		public static IEnumerable<DependencyObject> DescendantsAndSelf(this IEnumerable<DependencyObject> items)
		{
			return items.DrillDown(i => i.DescendantsAndSelf());
		}

		/// <summary>
		/// Returns a collection of ancestor elements.
		/// </summary>
		public static IEnumerable<DependencyObject> Ancestors(this IEnumerable<DependencyObject> items)
		{
			return items.DrillDown(i => i.Ancestors());
		}

		/// <summary>
		/// Returns a collection containing this element and all ancestor elements.
		/// </summary>
		public static IEnumerable<DependencyObject> AncestorsAndSelf(this IEnumerable<DependencyObject> items)
		{
			return items.DrillDown(i => i.AncestorsAndSelf());
		}

		/// <summary>
		/// Returns a collection of child elements.
		/// </summary>
		public static IEnumerable<DependencyObject> Elements(this IEnumerable<DependencyObject> items)
		{
			return items.DrillDown(i => i.Elements());
		}

		/// <summary>
		/// Returns a collection containing this element and all child elements.
		/// </summary>
		public static IEnumerable<DependencyObject> ElementsAndSelf(this IEnumerable<DependencyObject> items)
		{
			return items.DrillDown(i => i.ElementsAndSelf());
		}


		/// <summary>
		/// Returns a collection of descendant elements which match the given type.
		/// </summary>
		public static IEnumerable<DependencyObject> Descendants<T>(this IEnumerable<DependencyObject> items)
			where T : DependencyObject
		{
			return items.DrillDown<T>(i => i.Descendants());
		}

		/// <summary>
		/// Returns a collection containing this element and all descendant elements.
		/// which match the given type.
		/// </summary>
		public static IEnumerable<DependencyObject> DescendantsAndSelf<T>(this IEnumerable<DependencyObject> items)
			where T : DependencyObject
		{
			return items.DrillDown<T>(i => i.DescendantsAndSelf());
		}

		/// <summary>
		/// Returns a collection of ancestor elements which match the given type.
		/// </summary>
		public static IEnumerable<DependencyObject> Ancestors<T>(this IEnumerable<DependencyObject> items)
			where T : DependencyObject
		{
			return items.DrillDown<T>(i => i.Ancestors());
		}

		/// <summary>
		/// Returns a collection containing this element and all ancestor elements.
		/// which match the given type.
		/// </summary>
		public static IEnumerable<DependencyObject> AncestorsAndSelf<T>(this IEnumerable<DependencyObject> items)
			where T : DependencyObject
		{
			return items.DrillDown<T>(i => i.AncestorsAndSelf());
		}

		/// <summary>
		/// Returns a collection of child elements which match the given type.
		/// </summary>
		public static IEnumerable<DependencyObject> Elements<T>(this IEnumerable<DependencyObject> items)
			where T : DependencyObject
		{
			return items.DrillDown<T>(i => i.Elements());
		}

		/// <summary>
		/// Returns a collection containing this element and all child elements.
		/// which match the given type.
		/// </summary>
		public static IEnumerable<DependencyObject> ElementsAndSelf<T>(this IEnumerable<DependencyObject> items)
			where T : DependencyObject
		{
			return items.DrillDown<T>(i => i.ElementsAndSelf());
		}
	}
}