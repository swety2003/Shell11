namespace Shell11.Common
{
    using System.Windows;
    using System.Windows.Controls;


    namespace Controls
    {

        public class AlignPanel : Panel
        {
            public static readonly DependencyProperty AlignmentProperty =
                DependencyProperty.RegisterAttached(
                    "Alignment",
                    typeof(HorizontalAlignment),
                    typeof(AlignPanel),
                    new FrameworkPropertyMetadata(HorizontalAlignment.Left, FrameworkPropertyMetadataOptions.AffectsParentArrange));

            public static HorizontalAlignment GetAlignment(UIElement element)
            {
                return (HorizontalAlignment)element.GetValue(AlignmentProperty);
            }

            public static void SetAlignment(UIElement element, HorizontalAlignment value)
            {
                element.SetValue(AlignmentProperty, value);
            }

            protected override Size MeasureOverride(Size availableSize)
            {
                Size desiredSize = new Size();

                foreach (UIElement child in InternalChildren)
                {
                    child.Measure(new Size(availableSize.Width, double.PositiveInfinity));
                    desiredSize.Width += child.DesiredSize.Width;
                    desiredSize.Height = Math.Max(desiredSize.Height, child.DesiredSize.Height);
                }

                return desiredSize;
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                double left = 0;
                double right = finalSize.Width;
                double center_width = InternalChildren.Cast<UIElement>()
                    .Where(x => GetAlignment(x) == HorizontalAlignment.Center)
                    .Sum(c => c.DesiredSize.Width);
                double center_start = (finalSize.Width - center_width) / 2;
                double center_end = (finalSize.Width + center_width) / 2;

                foreach (UIElement child in InternalChildren)
                {
                    HorizontalAlignment alignment = GetAlignment(child);
                    Rect rect = new Rect();

                    switch (alignment)
                    {
                        case HorizontalAlignment.Left:
                            rect = new Rect(new Point(left, 0), new Size(child.DesiredSize.Width, finalSize.Height));
                            left += child.DesiredSize.Width;

                            if (rect.Right > center_start)
                            {
                                rect = new Rect(rect.TopLeft, new Point(center_start, rect.BottomRight.Y));
                            }
                            break;
                        case HorizontalAlignment.Center:
                            rect = new Rect(new Point(center_start, 0), new Size(child.DesiredSize.Width, finalSize.Height));
                            center_start += child.DesiredSize.Width;
                            break;
                        case HorizontalAlignment.Right:
                            right -= child.DesiredSize.Width;
                            rect = new Rect(new Point(right, 0), new Size(child.DesiredSize.Width, finalSize.Height));

                            if (rect.Left < center_end)
                            {
                                rect = new Rect(new Point(center_end, 0), rect.BottomRight);
                            }
                            break;
                        default: break;
                    }

                    child.Arrange(rect);
                }

                return finalSize;
            }
        }
    }

}
