using Box2dNet.Interop;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Windows;

namespace Atlantis.Game
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class GameShape
    {
        public GameControl Control;

        public b2ShapeId Shape;
        public Vector2 Offset;
        public Vector2 HalfSize;
        public Vector2 Size => HalfSize * 2.0f;

        // unused
        public FrameworkElement Element;
        //public RotateTransform Rotate;
        //public TranslateTransform Translate;


        private string DebuggerDisplay
        {
            get
            {
                return $"Control[{Control.CID}] S[{Shape.index1} : {Shape.generation}]";
            }
        }
    }
}
