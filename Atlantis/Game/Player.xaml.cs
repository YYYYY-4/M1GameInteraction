using Box2dNet.Interop;
using System.Windows.Input;
using System.Numerics;
using Atlantis.Box2dNet;
using System.Windows.Shapes;
using System.Diagnostics;
using WpfAnimatedGif;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Atlantis.Game
{
    public partial class Player : GameControl
    {
        public Player()
        {
            InitializeComponent();
        }

        float Mass;

        public override void OnStart()
        {
            Mass = Shape0.Shape.ComputeMassData().mass;
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

        private void UpdateGround()
        {
            b2QueryFilter filter = B2Api.b2DefaultQueryFilter();
            filter.categoryBits = 0x1;
            filter.maskBits = 0x1f;

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
        }

        bool IsInWater = false;

        // Concept utility
        // This is not typed with ? because the caller knows the argument they provide.
        public GameShape ShapeOf(Shape element)
        {
            foreach (var shape in Shapes)
            {
                if (shape.Element == element)
                {
                    return shape;
                }
            }
            return null!;
        }

        private bool _hasDynamite = false;
        public bool HasDynamite 
        { 
            get { return _hasDynamite; }
            set { _hasDynamite = value; }
        }


        public override void OnUpdate(float dt)
        {
            UpdateGround();

            if (Scene.Keys[Key.T].pressedNow)
            {
                IsInWater = !IsInWater;
            }

            if (Scene.Keys[Key.G].pressedNow)
            {
                Dynamite.SpawnDynamite(Scene);
            }

            var input = new Vector2(IsKeyDown01(Key.D) - IsKeyDown01(Key.A), IsKeyDown01(Key.W) - IsKeyDown01(Key.S));

            //var r = Scene.RayCastClosest(Body.GetPosition() - new Vector2(0.0f, 1.95f), new Vector2(0.0f, -1.5f), new b2QueryFilter(0x1, 0xFFFFFFFu));
            //var normal = new Vector2(0.0f, 1.0f);
            //if (r.hit)
            //{
            //    normal = r.normal; 
            //}
            // vector2 cross apparently
            //var moveDir = new Vector2(normal.Y, -normal.X);

            if (!IsInWater)
            {
                var inputDir = new Vector2(input.X, Scene.Keys[Key.W].pressedNow ? 1.0f : 0.0f);
                var velocity = Body.GetLinearVelocity();

                const float MaxSpd = 15.0f;

                float inpDirX = inputDir.X;

                float vX = velocity.X;

                float MoveForce = 1000.0f;
                float AirResistance = 1.05f;
                float AirDensity = 1.3f;

                var moveForce = new Vector2(MoveForce * inputDir.X, 0.0f);
                Body.ApplyForceToCenter(moveForce);
                
                float Fdrag = 0.5f * AirResistance * AirDensity * Shape0.Size.Y * float.Pow(vX, 2f);
                Fdrag *= float.Sign(vX);
                Body.ApplyForceToCenter(new Vector2(-Fdrag, 0.0f));

                if (inputDir.X != 0.0f)
                {
                    Shape0.Shape.SetSurfaceMaterial(new b2SurfaceMaterial()
                    {
                        friction = 0.0f,
                    });
                }
                else
                {
                    Shape0.Shape.SetSurfaceMaterial(new b2SurfaceMaterial()
                    {
                        friction = 1.0f,
                    });
                }

                Body.SetLinearVelocity(velocity);

                if (OnGround && inputDir.Y > 0.0f)
                {
                    float duration = 0.5f;
                    float height = 6.0f;
                    var force = (height - Scene.World.GetGravity().Y * duration * (duration / 2)) / duration;

                    //Trace.WriteLine("Mass = " + Mass);
                    Body.ApplyLinearImpulseToCenter(new Vector2(0.0f, force * Mass));
                }
            }
            else
            {
                // I tried looking into water physics but I don't know if I did it correctly. (I didn't do it correctly)
                // https://en.wikipedia.org/wiki/Drag_equation
                // https://en.wikipedia.org/wiki/Drag_coefficient
                // https://en.wikipedia.org/wiki/Buoyancy

                float density = 0.95f;

                var playerGravity = Scene.World.GetGravity() * Body.GetGravityScale();

                float waterSurface = 20.0f;
                float bodyLowerY = Body.GetPosition().Y - 2.0f;
                float underWater = waterSurface - bodyLowerY;
                float submergeFactor = float.Clamp(underWater / Shape0.Size.Y, 0.0f, 1.0f);
                
                var velocity = Body.GetLinearVelocity().Length();
                var area = Shapes[0].Size.X * Shapes[0].Size.Y;
                var submergedArea = area * submergeFactor;

                var buoyancy = (-playerGravity) * submergedArea * density;

                float coDrag = (2.0f * -velocity) / (density * (velocity * velocity) * area);
                if (float.IsNaN(coDrag))
                {
                    coDrag = 0.0f;
                }
                var drag = 0.5f * density * (velocity * velocity) * coDrag * submergedArea;

                Body.ApplyForceToCenter(buoyancy - (Body.GetLinearVelocity() * -drag));

                var moveForce = input * 100.0f;
                Body.ApplyForceToCenter(moveForce);

                //Trace.WriteLine($"d: {drag}, cd: {coDrag}");
                //Trace.WriteLine($"FPS: {1.0f / dt}");
            }

            // something something character mover 
            // https://box2d.org/documentation/md_character.html
            //var filter = Shape0.Shape.GetFilter();
            //var qf = new b2QueryFilter(filter.categoryBits, filter.maskBits);
            //
            //var capsule = new b2Capsule(new Vector2(0, 1.0f), new Vector2(0, -1.0f), MathF.PI * 0.5f);
            //var t = Body.GetTransform().ToMatrix3x2();
            //capsule.center1 = Vector2.Transform(capsule.center1, t);
            //capsule.center2 = Vector2.Transform(capsule.center2, t);
            //
            //List<b2CollisionPlane> planes = [];
            //Scene.World.CollideMover(capsule, qf, (b2ShapeId shapeId, in b2PlaneResult result, nint context) =>
            //{
            //    planes.Add(new b2CollisionPlane(result.plane, 1.0f, 1.0f, true));
            //    return true;
            //}, 0);
            //
            //var solve = B2Api.b2SolvePlanes(Body.GetPosition(), planes.ToArray(), planes.Count);
            //
            //var uh = B2Api.b2World_CastMover(Scene.World, capsule, Body.GetPosition(), qf);
            //Scene.World.CastMover(capsule, Body.GetPosition(), qf);
            //
            //Trace.WriteLine(planes);
        }

        private void GameControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
