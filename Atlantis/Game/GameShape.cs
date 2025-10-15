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
        public bool Destructible;
        public Vector2 Size => HalfSize * 2.0f;
        public bool Deadly;

        public int Index;

        // unused
        public FrameworkElement Element;
        //public RotateTransform Rotate;
        //public TranslateTransform Translate;


        private string DebuggerDisplay
        {
            get
            {
                return $"GameControl.{GetType().Name}[{Control.CID}] S[{Shape.index1} : {Shape.generation}]";
            }
        }
    }
}
