using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SLAESolver
{
	static class Styles
	{
		// стилі для текстових полів
		public static void AddStylesTextBox(TextBox textBox, int height, int width, Brush color, int thickness,
			int marginLeft, int marginTop, int marginRight, int marginBottom)
		{
			textBox.Height = height;
			textBox.Width = width;
			textBox.BorderBrush = color;
			textBox.BorderThickness = new Thickness(thickness);
			textBox.Margin = new Thickness(marginLeft, marginTop, marginRight, marginBottom);
		}

		// стилі для кнопок
		public static void AddStylesButton(Button button, string content, HorizontalAlignment horizontalAlignment,
			VerticalAlignment verticalAlignment, int marginLeft, int marginTop, int marginRight, int marginBottom)
		{
			button.Content = content;
			button.HorizontalAlignment = horizontalAlignment;
			button.VerticalAlignment = verticalAlignment;
			button.Margin = new Thickness(marginLeft, marginTop, marginRight, marginBottom);
		}
	}
}