using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace SwipeControl
{
    public abstract class SwipeControlBase : ContentControl
    {
        protected TranslateTransform translateTransfrom;
        protected bool isUserDrugging;
        protected double currentOfsset;
        protected double previousOffset;
        protected double middleValue;

        public Panel LeftPanel
        {
            get { return (Panel)GetValue(LeftPanelProperty); }
            set { SetValue(LeftPanelProperty, value); }
        }

        public Panel RightPanel
        {
            get { return (Panel)GetValue(RightPanelProperty); }
            set { SetValue(RightPanelProperty, value); }
        }

        public SwipeMode SwipeMode
        {
            get { return (SwipeMode)GetValue(SwipeModeProperty); }
            set { SetValue(SwipeModeProperty, value); }
        }

        public static readonly DependencyProperty SwipeModeProperty =
            DependencyProperty.Register("SwipeMode", typeof(SwipeMode), typeof(SwipeControl), new PropertyMetadata(SwipeMode.FromLeft));

        public static readonly DependencyProperty RightPanelProperty =
            DependencyProperty.Register("RightPanel", typeof(Panel), typeof(SwipeControl), new PropertyMetadata(null));

        public static readonly DependencyProperty LeftPanelProperty =
            DependencyProperty.Register("LeftPanel", typeof(Panel), typeof(SwipeControl), new PropertyMetadata(null));

        protected SwipeControlBase()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var contentPresenter = base.GetTemplateChild("ContentPresenter") as ContentPresenter;

            if (contentPresenter != null)
            {
                contentPresenter.ManipulationMode = ManipulationModes.System | ManipulationModes.TranslateX;
                contentPresenter.ManipulationStarting += OnManipulationStarting;
                contentPresenter.ManipulationStarted += OnManipulationStarted;
                contentPresenter.ManipulationDelta += OnManipulationDelta;
                contentPresenter.ManipulationCompleted += OnManipulationCompleted;
                translateTransfrom = GetTemplateChild("ContentPresenterTranslateTransform") as TranslateTransform;
                middleValue = ActualWidth / 2;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            middleValue = ActualWidth / 2;

            SizeChanged += OnSizeChanged;
            Window.Current.CoreWindow.PointerPressed += OnCoreWindowPointerPressed;
        }  

        private void OnCoreWindowPointerPressed(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            var ttv = TransformToVisual(Window.Current.Content);
            var point = ttv.TransformPoint(new Point(0, 0));
            if (!args.CurrentPoint.Position.X.IsBetween(point.X, point.X + ActualWidth) ||
                !args.CurrentPoint.Position.Y.IsBetween(point.Y, point.Y + ActualHeight))
            {
                args.Handled = true;
                Close();
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
            SizeChanged -= OnSizeChanged;
            Window.Current.CoreWindow.PointerPressed -= OnCoreWindowPointerPressed;
        }

        public void Close()
        {
            AnimateLeftPanel(translateTransfrom, "X", null, 0, TimeSpan.FromMilliseconds(300));
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            middleValue = this.ActualWidth / 2;

            if (!isUserDrugging && translateTransfrom != null && translateTransfrom.X != 0)
            {
                AnimateLeftPanel(translateTransfrom, "X", null, middleValue * Math.Sign(translateTransfrom.X), TimeSpan.FromMilliseconds(100));
            }
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            isUserDrugging = false;
            if (Math.Abs(currentOfsset) > middleValue / 2)
            {
                AnimateLeftPanel(translateTransfrom, "X", currentOfsset, middleValue * Math.Sign(currentOfsset), TimeSpan.FromMilliseconds(300));
            }
            else
            {
                AnimateLeftPanel(translateTransfrom, "X", currentOfsset, 0, TimeSpan.FromMilliseconds(300));
            }
        }

        protected abstract void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e);

        private void OnManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            this.currentOfsset = translateTransfrom.X;
        }

        private void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            this.isUserDrugging = true;
        }

        protected void AnimateLeftPanel(DependencyObject target, string property, double? from, double to, TimeSpan durationTimeSpane)
        {
            var doubleAnimation = new DoubleAnimation
            {
                To = to,
                From = from,
                EnableDependentAnimation = true,
            };

            var storyBoard = new Storyboard
            {
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = doubleAnimation.Duration = new Duration(durationTimeSpane),
            };

            storyBoard.Children.Add(doubleAnimation);
            Storyboard.SetTarget(doubleAnimation, target);
            Storyboard.SetTargetProperty(doubleAnimation, property);
            storyBoard.Begin();
        }
    }
}