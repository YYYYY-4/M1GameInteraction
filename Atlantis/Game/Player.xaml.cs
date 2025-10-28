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
    public partial class Player : WaterGameControl
    {
        public Inventory Inventory;
        private float timer = 0;
        
        public Player()
        {
            InitializeComponent();
        }

        float Mass;

        public override void OnStart()
        {
            Mass = Shape0.Shape.ComputeMassData().mass;
            Inventory = new(this, Scene);
        }

        bool OnGround = false;

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

            filter.categoryBits = ((ulong)PhysicsCategory.Map);
            filter.maskBits = (ulong)(PhysicsCategory.All & ~PhysicsCategory.WaterArea);

            var overlap = Scene.OverlapCast(proxy, filter);

            OnGround = overlap.Any(s => !Shapes.Contains(s));

            //Trace.WriteLine($"{CID} {s}");
            //Trace.WriteLine($"{CID} OnGround {OnGround}");
        }

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

       
        ScaleTransform right = new ScaleTransform(1,1,25,0);
        ScaleTransform left = new ScaleTransform(-1,1,25,0);

        // -1 if facing left, 1 if facing right.
        public int FacingDirection = 1;

        public override void OnUpdate(float dt)
        {
            UpdateGround();

            timer = timer + dt;


            if (Scene.Keys[Key.G].pressedNow)
            {
                Inventory.DropItem(this);
            }

            var input = new Vector2(IsKeyDown01(Key.D) - IsKeyDown01(Key.A), IsKeyDown01(Key.W) - IsKeyDown01(Key.S));

            var direction = Body.GetLinearVelocity();

            if (direction.X > 0.01) // checks if player is moving right
            {
                _player.RenderTransform = right;
                FacingDirection = 1;
            }
            else if (direction.X < -0.01) // checks if player is moving left
            {
                _player.RenderTransform = left;
                FacingDirection = -1;
            }

            //var r = Scene.RayCastClosest(Body.GetPosition() - new Vector2(0.0f, 1.95f), new Vector2(0.0f, -1.5f), new b2QueryFilter(0x1, 0xFFFFFFFu));
            //var normal = new Vector2(0.0f, 1.0f);
            //if (r.hit)
            //{
            //    normal = r.normal; 
            //}
            // vector2 cross apparently
            //var moveDir = new Vector2(normal.Y, -normal.X);

            //Trace.WriteLine($"FPS: {1.0f / dt}");

            if (timer > 2.0f)
                timer = 0;

            if (IsInWater())
            {
                // I tried looking into water physics but I don't know if I did it correctly. (I didn't do it correctly)
                // https://en.wikipedia.org/wiki/Drag_equation
                // https://en.wikipedia.org/wiki/Drag_coefficient
                // https://en.wikipedia.org/wiki/Buoyancy

                UpdateWaterForces(Shape0);

                if (timer > 1.5f)
                    _player.Source = (ImageSource)App.Current.FindResource("PlayerWater4");
                else if (timer > 1.0f)
                    _player.Source = (ImageSource)App.Current.FindResource("PlayerWater3");
                else if (timer > 0.5f)
                    _player.Source = (ImageSource)App.Current.FindResource("PlayerWater2");
                else
                    _player.Source = (ImageSource)App.Current.FindResource("PlayerWater1");
            }
            else
            {
                if (timer > 1.0f)
                    _player.Source = (ImageSource)App.Current.FindResource("PlayerLand3");
                else if (timer > 0.5f)
                    _player.Source = (ImageSource)App.Current.FindResource("PlayerLand2");
                else
                    _player.Source = (ImageSource)App.Current.FindResource("PlayerLand1");
            }

            bool inWaterMovement = IsInWater();

            if (inWaterMovement && OnGround && _submergeFactor < 0.4f)
            {
                inWaterMovement = false;
            }

            if (inWaterMovement)
            {
                var moveForce = input * 100.0f;
                Body.ApplyForceToCenter(moveForce);
            }
            else
            {
                var inputDir = new Vector2(input.X, Scene.Keys[Key.W].pressedNow ? 1.0f : 0.0f);
                var velocity = Body.GetLinearVelocity();

                const float MaxSpd = 15.0f;

                float inpDirX = inputDir.X;

                float vX = velocity.X;

                
                float AirResistance = 1.05f;
                float AirDensity = 1.3f;
                float MoveForce = 500.0f;
                if (OnGround)
                { 
                    MoveForce = 1000.0f;
                }


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
                    float height;
                    if (Inventory.GetItem()?.GetType() == typeof(JumpBoots))
                    {
                        height = 18.0f;
                    }
                    else
                        height = 9.0f;
                    var force = (height - Scene.World.GetGravity().Y * duration * (duration / 2)) / duration;

                    //Trace.WriteLine("Mass = " + Mass);
                    Body.ApplyLinearImpulseToCenter(new Vector2(0.0f, force * Mass));
                }
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
    }
}
