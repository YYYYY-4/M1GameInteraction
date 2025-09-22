using Box2dNet.Interop;
using System.Numerics;
using System.Windows;
using System.Windows.Media;

namespace Atlantis.Game
{
    public class GameShape
    {
        public GameControl Control;

        public b2ShapeId Shape;
        public Vector2 Offset;
        public Vector2 HalfSize;

        // unused
        public FrameworkElement Element;
        //public RotateTransform Rotate;
        //public TranslateTransform Translate;
    }
}
