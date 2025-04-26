using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Intranet02
{
    public class PlaceholderAdorner : Adorner
    {
        private readonly TextBlock _placeholderText;

        public PlaceholderAdorner(UIElement adornedElement, string placeholder) : base(adornedElement)
        {
            _placeholderText = new TextBlock
            {
                Text = placeholder,
                Foreground = Brushes.Gray,
                Margin = new Thickness(5, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            IsHitTestVisible = false; // Adorner가 클릭 이벤트를 차단하지 않도록 설정
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (AdornedElement is TextBox textBox && string.IsNullOrEmpty(textBox.Text))
            {
                DrawPlaceholder(drawingContext);
            }
            else if (AdornedElement is PasswordBox passwordBox && string.IsNullOrEmpty(passwordBox.Password))
            {
                DrawPlaceholder(drawingContext);
            }
        }

        private void DrawPlaceholder(DrawingContext drawingContext)
        {
            var location = new Point(5, 2); // 텍스트 위치 조정
            drawingContext.DrawText(
                new FormattedText(
                    _placeholderText.Text,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI"),
                    12, // 폰트 크기
                    Brushes.Gray,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip),
                location);
        }

    }
}
