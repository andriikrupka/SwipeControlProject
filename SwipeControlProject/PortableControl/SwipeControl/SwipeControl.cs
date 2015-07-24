using System;
using Windows.UI.Xaml.Input;

namespace SwipeControl
{
    public sealed class SwipeControl : SwipeControlBase
    {
        
        public SwipeControl()
        {
            DefaultStyleKey = typeof(SwipeControl);
        }

        protected override void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var isEvaluated = false;
            currentOfsset += e.Delta.Translation.X;
            switch (SwipeMode)
            {
                case SwipeMode.FromLeft:
                    if (currentOfsset < 0)
                    {
                        previousOffset = currentOfsset = 0;
                        isEvaluated = true;
                    }
                    break;

                case SwipeMode.FromRight:
                    if (currentOfsset > 0)
                    {
                        previousOffset = currentOfsset = 0;
                        isEvaluated = true;
                    }
                    break;
            }

            if (!isEvaluated)
            {
                if (Math.Abs(currentOfsset) >= this.ActualWidth / 2)
                {
                    previousOffset = this.currentOfsset = this.ActualWidth / 2 * Math.Sign(currentOfsset);
                }
                else
                {
                    previousOffset = currentOfsset - e.Delta.Translation.X;
                }
            }

            AnimateLeftPanel(translateTransfrom, "X", previousOffset, currentOfsset, TimeSpan.FromMilliseconds(300));
        }
    }
}