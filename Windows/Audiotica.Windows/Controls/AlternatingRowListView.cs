﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Audiotica.Windows.Controls
{
    public class AlternatingRowListView : ListView
    {
        public static readonly DependencyProperty OddRowBackgroundProperty =
            DependencyProperty.Register("OddRowBackground", typeof (Brush), typeof (AlternatingRowListView), null);

        public static readonly DependencyProperty EvenRowBackgroundProperty =
            DependencyProperty.Register("EvenRowBackground", typeof (Brush), typeof (AlternatingRowListView), null);

        public Brush OddRowBackground

        {
            get { return (Brush) GetValue(OddRowBackgroundProperty); }

            set { SetValue(OddRowBackgroundProperty, value); }
        }

        public Brush EvenRowBackground

        {
            get { return (Brush) GetValue(EvenRowBackgroundProperty); }

            set { SetValue(EvenRowBackgroundProperty, value); }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var listViewItem = element as ListViewItem;

            if (listViewItem != null)

            {
                var index = IndexFromContainer(element);
                listViewItem.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                listViewItem.Margin = new Thickness(10, 0, 10, 0);
                listViewItem.Background = (index + 1)%2 == 1 
                    ? OddRowBackground ?? App.Current.Resources["SystemChromeLowColor"] as Brush 
                    : EvenRowBackground;
            }
        }
    }
}