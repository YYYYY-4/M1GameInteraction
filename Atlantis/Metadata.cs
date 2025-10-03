using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Media;

namespace Atlantis
{
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
