using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace VMDEditor
{
    public class FastCanvas : FrameworkElement
    {
        public static Point GetLocation(DependencyObject obj)
        {
            return (Point)obj.GetValue(LocationProperty);
        }

        public static void SetLocation(DependencyObject obj, Point value)
        {
            obj.SetValue(LocationProperty, value);
        }
        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.RegisterAttached("Location", typeof(Point), typeof(FastCanvas),
            new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsArrange));

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<UIElement> Children { get; private set; }

        public FastCanvas()
        {
            Children = new ObservableCollection<UIElement>();

            Children.CollectionChanged += Children_CollectionChanged;
        }

        void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (UIElement oldItem in e.OldItems)
                {
                    RemoveVisualChild(oldItem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (UIElement newItem in e.NewItems)
                {
                    AddVisualChild(newItem);
                }
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return Children.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return Children[index];
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var child in Children)
            {
                var location = GetLocation(child);

                child.Arrange(new Rect(location, child.DesiredSize));
            }

            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in Children)
            {
                var fe = child as FrameworkElement;

                if (fe != null)
                {
                    child.Measure(new Size(fe.Width, fe.Height));
                }
            }

            return base.MeasureOverride(availableSize);
        }
    }
}
