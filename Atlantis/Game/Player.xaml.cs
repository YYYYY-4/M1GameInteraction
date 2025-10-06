using Box2dNet.Interop;
using System.Windows.Input;
using System.Numerics;
using Atlantis.Box2dNet;

namespace Atlantis.Game
{
    public partial class Player : GameControl
    {
        public Player()
        {
            InitializeComponent();
        }

        public override void ModifyBodyDef(ref b2BodyDef bodyDef)
        {
            base.ModifyBodyDef(ref bodyDef);
        }

        float Mass;

        public override void OnStart()
        {
            Mass = Shapes[0].Shape.ComputeMassData().mass;
        }

        bool OnGround = false;

        public float CastResultFcn(b2ShapeId shapeId, Vector2 point, Vector2 normal, float fraction, IntPtr context)
        {
            if (Shapes.Any(s => s.Shape == shapeId))
            {
                return 1.0f;
            }

            OnGround = true;
            return 0.0f;
        }

        //List<b2ShapeId> Overlap = new List<b2ShapeId>();

        //public bool OverlapResultFcn(b2ShapeId shapeId, IntPtr context)
        //{
        //    if (shapeId != Shapes[0].Shape)
        //    {
        //        Overlap.Add(shapeId);
        //    }
        //    return true;
        //}

        public override void OnUpdate(float dt)
        {
            var inputDir = new Vector2(IsKeyDown01(Key.D) - IsKeyDown01(Key.A), Scene.Keys[Key.W].pressedNow ? 1.0f : 0.0f);

            b2QueryFilter filter = B2Api.b2DefaultQueryFilter();
            filter.categoryBits = 0x1;
            filter.maskBits = 0x1f;

            //var box = B2Api.b2MakeBox(5, 5);
            var box = B2Api.b2MakeBox(Shapes[0].HalfSize.X - 0.05f, 0.2f);
            Vector2[] points = new Vector2[box.count];
            for (int i = 0; i < box.count; i++)
            {
                points[i] = box.vertices(i);
            }
            b2ShapeProxy proxy = B2Api.b2MakeOffsetProxy(points, points.Length, 0.0f, Body.GetPosition() + new Vector2(0, -Shapes[0].HalfSize.Y), b2Rot.Zero);

            var overlap = Scene.OverlapCast(proxy, filter);

            OnGround = overlap.Any(s => !Shapes.Contains(s));

            //Trace.WriteLine($"{CID} {s}");
            //Trace.WriteLine($"{CID} OnGround {OnGround}");

            var move = Vector2.Zero;

            if (OnGround)
            {
                move.X = inputDir.X;
                move.Y += inputDir.Y * 2.0f;
            }
            else
            {
                move.X = inputDir.X * 0.1f;
            }

            var force = MathF.Abs(Mass * Scene.World.GetGravity().Y) * 1.0f;
            B2Api.b2Body_ApplyForceToCenter(Body, new Vector2(move.X * 500.0f, move.Y * force), true);
            Body.ApplyLinearImpulseToCenter(new Vector2(0.0f, move.Y * force), true);

            //var capsule = new b2Capsule(new Vector2(0, 1.0f), new Vector2(0, -1.0f), MathF.PI * 0.5f);
            //var uh = B2Api.b2World_CastMover(Scene.World, capsule, BodyId.GetPosition(), filter);


        }
    }
}
