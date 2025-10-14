using Box2dNet.Interop;
using System.Windows.Input;
using System.Numerics;
using Atlantis.Box2dNet;
using System.Windows.Shapes;
using System.Diagnostics;

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

        //List<b2ShapeId> Overlap = [];
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

        public override void OnUpdate(float dt)
        {
            UpdateGround();

            if (Scene.Keys[Key.T].pressedNow)
            {
                IsInWater = !IsInWater;
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
                var v = Body.GetLinearVelocity();

                const float MaxSpd = 15.0f;

                float inpDirX = inputDir.X;

                int method = 2;
                if (method == 1)
                {
                    // L1: f(x) = 5^x - 1
                    float spdForm1(float x)
                    {
                        return MathF.Pow(5, x) - 1;
                    }

                    float spdForm1Inv(float x)
                    {
                        return MathF.Log(x + 1, 5);
                    }

                    // variables: vX, vY, dt, inputX
                    // vX = -400 ~ 400
                    // dt = ~0.016
                    // inputX = -1, 0, 1
                    // maxSpeed = n, using 15

                    // find x in the graph of L1
                    float vX = v.X;
                    float invX = spdForm1Inv(MathF.Abs(vX));
                    invX = MathF.CopySign(invX, vX);

                    if (float.IsNaN(invX))
                    {
                        invX = 0.0f;
                    }

                    // increase/decrease x based on input, interpret x as seconds and y as Vx
                    float tScale = 1.0f;
                    float t = invX + (dt * inputDir.X * tScale);
                    float x = spdForm1(MathF.Abs(t));
                    x = MathF.CopySign(x, t);

                    if (x > MaxSpd) x = MaxSpd;
                    if (x < -MaxSpd) x = -MaxSpd;
                    if (float.IsNaN(x)) x = 0.0f;

                    v.X = x;

                    Trace.WriteLine($"TIME:{Scene.Time} | T:{t} | INVX:{invX} | X:{x} | IDIRX:{inputDir.X}");
                }
                else if (method == 2)
                {
                    // this offset brings the x closer to 0.0, otherwise the Vx does not react
                    float offset = 0.01584893f;
                    float @base = 30.0f;

                    // y = log (x + offset) + 1.8
                    float spdForm2(float x)
                    {
                        return (MathF.Log(x + offset, @base) + 1.8f) * 6.0f;
                    }

                    float spdForm2Inv(float y)
                    {
                        return MathF.Pow(@base, y / 6.0f - 1.8f) - offset;
                    }

                    float vX = v.X;
                    float invX = spdForm2Inv(MathF.Abs(vX));
                    if (float.IsNaN(invX))
                    {
                        invX = 0.0f;
                    }
                    invX = MathF.CopySign(invX, vX);

                    // if no input, slow down to 0.0
                    if (inpDirX == 0.0f)
                    {
                        inpDirX = Math.Clamp(0.0f - vX, -1.0f, 1.0f);
                        //Trace.WriteLine($"{inpDirX}, {v}");
                    }

                    // increase/decrease x based on input, interpret x as seconds and y as Vx
                    float tScale = 1.0f;
                    float t = invX + (dt * inpDirX * tScale);
                    float x = spdForm2(MathF.Abs(t));
                    x = MathF.CopySign(x, t);

                    if (x > MaxSpd) x = MaxSpd;
                    if (x < -MaxSpd) x = -MaxSpd;
                    if (float.IsNaN(x)) x = 0.0f;

                    v.X = x;

                    Trace.WriteLine($"TIME:{Scene.Time} | T:{t} | INVX:{invX} | X:{x} | IDIRX:{inpDirX}");
                }

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

                // convert speed to f(x) range to something to add dt
                // map the current |vX| = y to the x of f(x) by inverse of f(x)
                // x = dt * TimeScale to inv f(x)
                // vX = x => f(x)

                // goal:
                // mapToSpeed(f(speedToX(v.X) (+/-) dt*scale))

                Body.SetLinearVelocity(v);

                if (OnGround && inputDir.Y > 0.0f)
                {
                    float duration = 0.5f;
                    float height = 6.0f;
                    var force = (height - Scene.World.GetGravity().Y * duration * (duration / 2)) / duration;

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

                var g = Scene.World.GetGravity() * Body.GetGravityScale();

                float waterSurface = 20.0f;
                float y = Body.GetPosition().Y - 2.0f;
                float underWater = waterSurface - y;
                float co = float.Min(1.0f, underWater / Shape0.Size.Y);

                Trace.WriteLine($"{waterSurface} {y} {underWater}");

                var v = Body.GetLinearVelocity().Length();
                var a = Shapes[0].Size.X * Shapes[0].Size.Y * co;

                var buoyancy = (-g) * a * density;

                float cd = (2.0f * -v) / (density * (v * v) * a);
                if (float.IsNaN(cd))
                {
                    cd = 0.0f;
                }
                var drag = 0.5f * density * (v * v) * cd * a;

                Body.ApplyForceToCenter(buoyancy - (Body.GetLinearVelocity() * -drag));

                var moveForce = input * 100.0f;
                Body.ApplyForceToCenter(moveForce);

                Trace.WriteLine($"d: {drag}, cd: {cd}");
                Trace.WriteLine($"FPS: {1.0f / dt}");
            }

            // something something character mover 
            // https://box2d.org/documentation/md_character.html
            if (false)
            {
                var filter = Shape0.Shape.GetFilter();
                var qf = new b2QueryFilter(filter.categoryBits, filter.maskBits);

                var capsule = new b2Capsule(new Vector2(0, 1.0f), new Vector2(0, -1.0f), MathF.PI * 0.5f);
                var t = Body.GetTransform().ToMatrix3x2();
                capsule.center1 = Vector2.Transform(capsule.center1, t);
                capsule.center2 = Vector2.Transform(capsule.center2, t);

                List<b2CollisionPlane> planes = [];
                Scene.World.CollideMover(capsule, qf, (b2ShapeId shapeId, in b2PlaneResult result, nint context) =>
                {
                    planes.Add(new b2CollisionPlane(result.plane, 1.0f, 1.0f, true));
                    return true;
                }, 0);

                var solve = B2Api.b2SolvePlanes(Body.GetPosition(), planes.ToArray(), planes.Count);

                var uh = B2Api.b2World_CastMover(Scene.World, capsule, Body.GetPosition(), qf);
                Scene.World.CastMover(capsule, Body.GetPosition(), qf);

                Trace.WriteLine(planes);
            }

            if (false)
            {
                var move = Vector2.Zero;

                if (OnGround)
                {
                    move.X = input.X;
                    move.Y += Scene.Keys[Key.W].pressedNow ? 2.0f : 0.0f;
                }
                else
                {
                    move.X = input.X * 0.1f;
                }

                var force = MathF.Abs(Mass * Scene.World.GetGravity().Y) * 1.0f;
                B2Api.b2Body_ApplyForceToCenter(Body, new Vector2(move.X * 500.0f, move.Y * force), true);
                Body.ApplyLinearImpulseToCenter(new Vector2(0.0f, move.Y * force), true);
            }

            // If the friction of player is 0, meaning velocity keeps scaling.
            // It is 0 so that the player doesn't stick to walls when moving into them.

            // Some ways to move the player:
            // X:
            // 1. Every frame apply a force on the X direction
            // 2. Every frame set the X based on A/D input -1 0 1
            //      A lot of control, character does not get pushed by physics
            //      Maybe a variation of this concept that limites the max velocity?
            // 3. Every frame apply a force on the X direction, but also apply a force to counter exceeding the move speed.
            //      The player starts moving slowly when using this method.
            //      This method makes more sense when resistance is needed like in water or a car.
            // Y:
            //    Jump applies a Impulse
            //    Jump applies a Impulse but also allows holding the button
            //    

            /* Water movement:
             *  Water resistance counters the velocity of the player (or any object...) - maybe this is called buoyancy
             *  Gravity pulls objects down but the effect 
             *  Buoyancy could be used but the player would be floaty
             */
        }

        private void GameControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
