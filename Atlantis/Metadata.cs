using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Media;

namespace Atlantis
{
    [DebuggerTypeProxy(nameof(Matrix4x4), Target = typeof(Matrix4x4))]
    public static class Metadata
    {
        public static readonly DependencyProperty InfoProperty = DependencyProperty.RegisterAttached("Info", typeof(string), typeof(Metadata), new PropertyMetadata(null));
        public static void SetInfo(UIElement element, string value) => element.SetValue(InfoProperty, value);
        public static string GetInfo(UIElement element) => (string)element.GetValue(InfoProperty);
    }

    public static class RotateUtil
    {
        public static RotateTransform? GetRotateTransform(UIElement element)
        {
            RotateTransform? rotateTransform = null;

            if (element.RenderTransform is RotateTransform tmpRotate)
            {
                rotateTransform = tmpRotate;
            }
            else if (element.RenderTransform is TransformGroup designerGroup)
            {
                rotateTransform = (RotateTransform?)designerGroup.Children.FirstOrDefault(el => el is RotateTransform);
            }

            return rotateTransform;
        }
    }
}
