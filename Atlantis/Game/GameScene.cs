using Atlantis.Box2dNet;
using Atlantis.Menus;
using Atlantis.Scene;
using Box2dNet.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml.Linq;
using static Atlantis.Box2dNet.B2Extension;

namespace Atlantis.Game
{
    /// <summary>
    /// Scene class that loads a xaml hierarchy into runtime.
    /// If resources and events are not cleared up properly (see Unload method) things will go wrong sooner or later.
    /// </summary>
    public class GameScene
    {
        private bool _controlsChanged = true;
        private readonly List<GameControl> _controls = [];
        private readonly List<GameControl> _addedControls = [];
        //private readonly List<GameControl> RemovedControls = [];

        public List<GameControl> Controls => _controls;

        // Copyy of _controls to iterate safely while supporting adding/removing objects
        private List<GameControl> _iterControls = [];

        const float ScalingFactor = 25.0f;

        public b2WorldId World;

        /// <summary>
        /// (Concept) Static control at identity transform (0,0,0) to hold shapes defined in the root canvas.
        /// </summary>
        public GameControl WorldBody;

        public MainWindow Window;
        public Canvas Canvas;

        public DrawingBrush CanvasBrush;

        nint ShapeIdGen = 0;

        // see GameControl.CID
        long ControlIdGen = 0;

        // When removing a Body from the Scene it must also be removed from the lookup
        private Dictionary<nint, GameShape> _shapeLookUp = [];

        public Dictionary<Key, KeyState> Keys;

        public Camera Camera { get; set; }

        /// <summary>
        /// Elapsed simulated time since scene loaded in seconds.
        /// </summary>
        public float Time { get; private set; } = 0.0f;

        private Stopwatch _watch = new();

        private TimeSpan _lastElapsed = TimeSpan.Zero;

        private Vector2 _mousePosition = Vector2.Zero;

        private bool _ms1DownThisFrame = false;

        private GameControl? _dragging;
        private Vector2 _worldMousePosition = Vector2.Zero;
        private Vector2 _draggingOffset = Vector2.Zero;

        private Vector2 _lastDragPosition = Vector2.Zero;
        private Vector2 _lastMousePosition = Vector2.Zero;

        private List<GameShape> _overlapCast;

        private static bool _paused = false;
        private static bool Paused
        {
            get => _paused;
            set => _paused = value;
        }

        public GameScene(MainWindow window, Canvas canvas)
        {
            Window = window;
            Canvas = canvas;

            Camera = new Camera();

            if (Canvas.IsLoaded)
            {
                Content_Loaded(null!, null!);
            }
            else
            {
                Canvas.Loaded += Content_Loaded;
            }
        }

        public void Destroy()
        {
            if (World.IsValid())
            {
                Unload(null, null!);
            }
            else
            {
                Canvas.Loaded -= Content_Loaded;
            }
        }

        private void Content_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas.ClearValue(Canvas.WidthProperty);
            Canvas.ClearValue(Canvas.HeightProperty);

            CanvasBrush = new DrawingBrush
            {
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, 25, 25),
                ViewportUnits = BrushMappingMode.Absolute,
                Drawing = new GeometryDrawing(Brushes.Transparent, new Pen(Brushes.LightGray, 1), new GeometryGroup
                {
                    Children = [new LineGeometry(new Point(0, 0), new Point(25, 0)), new LineGeometry(new Point(0, 0), new Point(0, 25))]
                }),
            };

            Canvas.Background = CanvasBrush;

            b2WorldDef worldDef = B2Api.b2DefaultWorldDef();
            worldDef.enableSleep = false;

            World = B2Api.b2CreateWorld(worldDef);

            B2Api.b2SetAssertFcn(AssertFcn);

            LoadGameControls(Canvas, (float)Canvas.ActualHeight, 0.0, 0.0);
            InvisGroundBody();



            _watch.Start();

            Keys = [];
            foreach (var key in Enum.GetValues(typeof(Key)).Cast<Key>())
            {
                Keys[key] = new();
            }

            CompositionTarget.Rendering += OnRendering;
            Window.MouseMove += MainWindow_MouseMove;
            Window.KeyDown += MainWindow_KeyDown;
            Window.KeyUp += MainWindow_KeyUp;

            Window.Closed += Unload;
        }

        private void Unload(object? sender, EventArgs e)
        {
            CompositionTarget.Rendering -= OnRendering;
            Window.MouseMove -= MainWindow_MouseMove;
            Window.KeyDown -= MainWindow_KeyDown;
            Window.KeyUp -= MainWindow_KeyUp;
            Window.Closed -= Unload;
            
            B2Api.b2DestroyWorld(World);

            _watch.Stop();
        }

        private void InvisGroundBody()
        {
            var wall = new Wall
            {
                Width = 5e4f * ScalingFactor,
                Height = 50.0f
            };
            ProcessGameControl(wall, new b2Transform(new Vector2(-2.5e4f, 0.0f), b2Rot.Zero));
        }

        public void ProcessGameControl(GameControl control, b2Transform transform)
        {
            if (double.IsNaN(Canvas.GetTop(control)))
            {
                Canvas.SetTop(control, 0.0);
            }

            if (double.IsNaN(Canvas.GetLeft(control)))
            {
                Canvas.SetLeft(control, 0.0);
            }

            control.CID = ++ControlIdGen;
            control.Scene = this;

            List<FrameworkElement>? shapes = null;
            if (control.Content is Shape tmpShape)
            {
                shapes = [tmpShape];
            }
            else if (control.Content is Image tmpImage)
            {
                shapes = [tmpImage]; 
            }
            else if (control.Content is Canvas canvas)
            {
                shapes = [];
                foreach (FrameworkElement child in canvas.Children)
                {
                    if (child is Shape || child is Image)
                    {
                        shapes.Add(child);
                    }
                }
            }

            if (shapes == null || shapes.Count == 0)
            {
                shapes = [];
            }

            var bodyDef = B2Api.b2DefaultBodyDef(); // probably WPF properties Body.<Properties> for loading the Body. And then Body.GetBodyDef(Shape shape).

            // Origin of Body, it is the top left translated to physics coordiante space
            bodyDef.position = transform.p;
            bodyDef.rotation = transform.q;

            control.ModifyBodyDef(ref bodyDef);
            var body = B2Api.b2CreateBody(World, bodyDef);

            var designerBodyAngle = RotateUtil.GetRotateTransform(control)?.Angle ?? 0.0;
            var bodyRotate = new RotateTransform(designerBodyAngle);
            var bodyTranslate = new TranslateTransform(0.0, 0.0); // useless.

            control.RenderTransform = new TransformGroup
            {
                Children = [bodyRotate, bodyTranslate]
            };
            control.Rotate = bodyRotate;
            control.Translate = bodyTranslate;
            control.Body = body;
            control.Shapes = [];

            foreach (var shape in shapes)
            {

                // Automatically set Width and Height, they are not needed in the designer for Polygon and Line
                if (shape is Polygon polyFix && polyFix.Points.Count > 0)
                {
                    var p1 = polyFix.Points.First();
                    double minX = p1.X;
                    double minY = p1.Y;
                    double maxX = p1.X;
                    double maxY = p1.Y;
                    foreach (var p in polyFix.Points)
                    {
                        if (p.X < minX) minX = p.X;
                        if (p.Y < minY) minY = p.Y;
                        if (p.X > maxX) maxX = p.X;
                        if (p.Y > maxY) maxY = p.Y;
                    }
                    if (double.IsNaN(shape.Width))
                    {
                        shape.Width = maxX - minX;
                    }
                    if (double.IsNaN(shape.Height))
                    {
                        shape.Height = maxY - minY;
                    }
                }
                else if (shape is Line lineFix)
                {
                    List<Vector2> points = [new Vector2((float)lineFix.X1, (float)lineFix.Y1), new Vector2((float)lineFix.X2, (float)lineFix.Y2)];
                    var p1 = points[0];
                    double minX = p1.X;
                    double minY = p1.Y;
                    double maxX = p1.X;
                    double maxY = p1.Y;
                    foreach (var p in points)
                    {
                        if (p.X < minX) minX = p.X;
                        if (p.Y < minY) minY = p.Y;
                        if (p.X > maxX) maxX = p.X;
                        if (p.Y > maxY) maxY = p.Y;
                    }
                    if (double.IsNaN(shape.Width))
                    {
                        shape.Width = maxX - minX;
                    }
                    if (double.IsNaN(shape.Height))
                    {
                        shape.Height = maxY - minY;
                    }
                }

                if (double.IsNaN(shape.Width))
                {
                    throw new NotFiniteNumberException("Shape Width is NaN", shape.Width);
                }
                if (double.IsNaN(shape.Height))
                {
                    throw new NotFiniteNumberException("Shape Height is NaN", shape.Height);
                }

                if (double.IsNaN(Canvas.GetTop(shape)))
                {
                    Canvas.SetTop(shape, 0.0);
                }

                if (double.IsNaN(Canvas.GetLeft(shape)))
                {
                    Canvas.SetLeft(shape, 0.0);
                }

                var shapeDef = B2Api.b2DefaultShapeDef(); // probably WPF properties Shape.<Properties> for loading the Shape. And then Shape.GetShapeDef(Shape shape).
                shapeDef.filter.categoryBits = ((ulong)PhysicsCategory.Map);
                shapeDef.filter.maskBits = ulong.MaxValue;
                shapeDef.filter.groupIndex = 0;

                // Body ShapeDef applied before direct ShapeDef
                control.ModifyShapeDef(ref shapeDef);
                var bodyShapeDef = ShapeDef.GetShapeDef(control);
                bodyShapeDef?.ApplyShapeDef(ref shapeDef);
                var wpfShapeDef = ShapeDef.GetShapeDef(shape);
                wpfShapeDef?.ApplyShapeDef(ref shapeDef);

                // Userdata is sacred
                shapeDef.userData = ShapeIdGen++;

                double width = shape.Width;
                double height = shape.Height;
                var halfSize = new Vector2((float)width / ScalingFactor * 0.5f, (float)height / ScalingFactor * 0.5f);

                var shapeRotate = RotateUtil.GetRotateTransform(shape);
                float definedAngleDeg = (float)(shapeRotate?.Angle ?? 0.0);
                // WPF clockwise rotation is defined as ccw, or shapes are rendered backwards
                float dirAngle = (360.0f - definedAngleDeg).DegToRad();
                var directionX = (dirAngle + MathF.PI * 0.0f).AngleToDirection() * halfSize.X;
                var directionY = (dirAngle + MathF.PI * 0.5f).AngleToDirection() * halfSize.Y;

                var shapeLocalPosition = new Vector2((float)Canvas.GetLeft(shape), -(float)Canvas.GetTop(shape)) / ScalingFactor;
                var offset = shapeLocalPosition + (directionX - directionY);

                b2ShapeId? physShape = null;
                if (shape is Rectangle || shape is Image)
                {
                    var polygon = B2Api.b2MakeBox(halfSize.X, halfSize.Y);

                    // Offset the polygon
                    var shapeTransform = new b2Transform(offset, b2Rot.FromAngle(dirAngle));
                    polygon = B2Api.b2TransformPolygon(shapeTransform, polygon);

                    physShape = B2Api.b2CreatePolygonShape(body, shapeDef, polygon);
                }
                else if (shape is Ellipse)
                {
                    // Use smallest size for radius
                    var circle = new b2Circle(offset, Math.Min(halfSize.X, halfSize.Y));
                    physShape = B2Api.b2CreateCircleShape(body, shapeDef, circle);
                }
                else if (shape is Polygon designerPolygon)
                {
                    var points = designerPolygon.Points.ToArray();
                    if (points == null || points.Length < 3 || points.Length > B2Api.B2_MAX_POLYGON_VERTICES)
                    {
                        throw new Exception($"Polygon Points invalid {points?.Length}");
                    }

                    var vectorPoints = points.Select(p => new Vector2((float)p.X / ScalingFactor, -(float)p.Y / ScalingFactor)).ToArray();
                    var hull = B2Api.b2ComputeHull(vectorPoints, points.Length);
                    var polygon = B2Api.b2MakeOffsetPolygon(hull, shapeLocalPosition, b2Rot.FromAngle(dirAngle));
                    physShape = B2Api.b2CreatePolygonShape(body, shapeDef, polygon);
                }
                else if (shape is Line line)
                {
                    // segement shape has two way collision. To support one way collision look into b2 b2ChainSegment
                    // if ((line.Tag as string)?.ToLower() == "oneway") ...

                    b2Segment segment = new b2Segment(new Vector2((float)line.X1, (float)line.Y1) / ScalingFactor, new Vector2((float)line.X2, (float)line.Y2) / ScalingFactor);
                    physShape = B2Api.b2CreateSegmentShape(body, shapeDef, segment);
                }
                else
                {
                    // All types that inherit Shape:
                    // Ellipse, Line, Path, Polygon, Polyline, Rectangle

                    // Add Line, Path, Polyline support?
                    throw new NotImplementedException($"Shape not supported: {shape.GetType()}");
                }

                // Commented out because: Shape is defined/rendered relative to the Body root Element and doesn't need to be centered.

                //var rotate = new RotateTransform(shapeRotate?.Angle ?? 0.0, width * 0.5, height * 0.5);
                //var translate = new TranslateTransform(-width * 0.5, -height * 0.5);
                //var group = new TransformGroup
                //{
                //    Children = [rotate, translate]
                //};
                //shape.RenderTransform = group;

                if (physShape != null)
                {
                    var gameShape = new GameShape()
                    {
                        Control = control,
                        Shape = (b2ShapeId)physShape,
                        Element = shape,
                        Destructible = bodyShapeDef?.Destructible ?? false,
                        Offset = offset,
                        HalfSize = halfSize,
                        Index = control.Shapes.Count,
                    };

                    _shapeLookUp.Add(shapeDef.userData, gameShape);
                    control.Shapes.Add(gameShape);

                    var a = Convert.ToString((long)shapeDef.filter.categoryBits, 2);
                    var b = Convert.ToString((long)shapeDef.filter.maskBits, 2);

                    Trace.WriteLine($"LOAD {control.GetType().Name}[{control.CID},{control.Shapes.Count-1}] : CATEGORY={a} MASK={b}");
                }
            }

            _addedControls.Add(control);
        }

        public void DestroyControl(GameControl control)
        {
            Debug.Assert(control.Body.IsValid());

            _controlsChanged = true;
            _controls.Remove(control);

            foreach (var shape in control.Shapes)
            {
                _shapeLookUp.Remove(shape.Shape.GetUserData());
            }

            B2Api.b2DestroyBody(control.Body);
            control.Body = new b2BodyId();

            Canvas.Children.Remove(control);
        }

        // left and top are in wpf units the absolute distances from 0,0. therefore 0,0 is initally the root.
        private void LoadGameControls(Canvas canvas, double canvasActualHeight, double left, double top)
        {
            void processGameControl(GameControl control)
            {
                var designerBodyAngle = RotateUtil.GetRotateTransform(control)?.Angle ?? 0.0;

                // Origin of Body, it is the top left translated to physics coordiante space
                double bottom = canvasActualHeight - (top + Canvas.GetTop(control));
                var p = new Vector2((float)(left + Canvas.GetLeft(control)), (float)bottom) / ScalingFactor;
                var q = b2Rot.FromAngle(((float)designerBodyAngle));

                ProcessGameControl(control, new b2Transform(p, q));
            }

            void processShape(FrameworkElement shape)
            {
                var control = new GameControl();

                Canvas.SetTop(control, Canvas.GetTop(shape));
                Canvas.SetLeft(control, Canvas.GetLeft(shape));
                Canvas.SetTop(shape, 0.0);
                Canvas.SetLeft(shape, 0.0);

                // Note: RenderTransform is not handled and Angle might be wrong.

                canvas.Children.Remove(shape);
                control.Content = shape;

                Canvas.Children.Add(control);
                processGameControl(control);
            }

            void processLabel(Label label)
            {
                var control = new GameControl();

                Canvas.SetTop(control, Canvas.GetTop(label));
                Canvas.SetLeft(control, Canvas.GetLeft(label));
                Canvas.SetTop(label, 0.0);
                Canvas.SetLeft(label, 0.0);

                // Note: RenderTransform is not handled and Angle might be wrong.

                canvas.Children.Remove(label);
                control.Content = label;

                Canvas.Children.Add(control);
                processGameControl(control);
            }

            double NanToZero(double d)
            {
                return double.IsNaN(d) ? 0.0 : d;
            }

            void processCanvas(Canvas c)
            {
                LoadGameControls(c, canvasActualHeight, left + NanToZero(Canvas.GetLeft(c)), top + NanToZero(Canvas.GetTop(c)));
                canvas.Children.Remove(c);
            }

            // Store as array because otherwise cannot remove elements while iterating
            var elements = canvas.Children.OfType<UIElement>().ToArray();
            foreach (UIElement element in elements)
            {
                if (element is Canvas childCanvas)
                {
                    processCanvas(childCanvas);
                }
                else if (element is Shape childShape)
                {
                    processShape(childShape);
                }
                else if (element is GameControl control)
                {
                    processGameControl(control);
                }
                else if (element is Label label)
                {
                    processLabel(label);
                }
                else if (element is Image imageShape)
                {
                    processShape(imageShape);
                }
                else
                {
                    throw new NotImplementedException($"Unhandled Type: {element.GetType()}");
                }
            }
        }

        private void OnRendering(object? sender, EventArgs e)
        {
            var now = _watch.Elapsed;
            float dt = (float)(now - _lastElapsed).TotalSeconds;
            _lastElapsed = now;

            dt = Math.Min(dt, 1.0f / 30.0f);
            if (!Paused)
            {
                Time += dt;
            }

            // Start new controls
            if (_addedControls.Count > 0)
            {
                _controlsChanged = true;
                foreach (var control in _addedControls.ToList())
                {
                    _controls.Add(control);
                    if (control.Parent == null) Canvas.Children.Add(control);
                    control.OnStart();
                }
                _addedControls.Clear();
            }

            // Start new controls
            if (_addedControls.Count > 0)
            {
                _controlsChanged = true;
                foreach (var control in _addedControls.ToList())
                {
                    _controls.Add(control);
                    if (control.Parent == null) Canvas.Children.Add(control);
                    control.OnStart();
                }
                _addedControls.Clear();
            }

            if (_controlsChanged)
            {
                _iterControls = [.. _controls];
            }
            GameUpdate(dt);

            if (_controlsChanged)
            {
                _iterControls = [.. _controls];
            }
            GameRender(dt);

            foreach (var state in Keys)
            {
                state.Value.pressedNow = false;
                state.Value.releasedNow = false;
            }
        }

        bool qfn(b2ShapeId shapeId, nint ctx)
        {
            var body = B2Api.b2Shape_GetBody(shapeId);
            Dragging = _controls.FirstOrDefault(p => p.Body == body);
            DraggingOffset = body.GetPosition() - WorldMousePosition;
            return false;
        }

        public void GameUpdate(float dt)
        {
            if (Paused) return;

            float speed = 1.0f;

            World.Step(dt / speed, 64);

            // process sensor events, note one shape must be a Sensor and the both must enable sensor events in ShapeDef.
            var sensorEvents = World.GetSensorEvents();

            foreach (var ev in sensorEvents.beginEventsAsSpan)
            {
                if (_shapeLookUp.TryGetValue(ev.sensorShapeId.GetUserData(), out var sensorShape) && _shapeLookUp.TryGetValue(ev.visitorShapeId.GetUserData(), out var visotorShape))
                {
                    sensorShape.Control.OnSensorStart(sensorShape, visotorShape);
                }
            }

            foreach (var ev in sensorEvents.endEventsAsSpan)
            {
                if (_shapeLookUp.TryGetValue(ev.sensorShapeId.GetUserData(), out var sensorShape) && _shapeLookUp.TryGetValue(ev.visitorShapeId.GetUserData(), out var visotorShape))
                {
                    sensorShape.Control.OnSensorEnd(sensorShape, visotorShape);
                }
            }

            // process contact events
            // - probleem voor later

            // update all controls
            foreach (var control in _iterControls)
            {
                if (!_controlsChanged || _controls.Contains(control))
                {
                    control.OnUpdate(dt);
                }
            }

            // The reverse of rendering to the screen, TODO support Camera rotation.

            // TODO fixme: When dragging a control while it gets remove it should not get accessed

            var worldCoords = new Vector2(MousePosition.X / ScalingFactor, (float)(Canvas.ActualHeight - MousePosition.Y) / ScalingFactor);
            var halfCanvas = new Vector2((float)Canvas.ActualWidth / 2, (float)Canvas.ActualHeight / 2) / ScalingFactor;
            worldCoords += Camera.Position - halfCanvas;

            var worldPosition = worldCoords;
            WorldMousePosition = worldPosition;

            bool ms1LastFrame = Ms1DownThisFrame;
            Ms1DownThisFrame = Mouse.LeftButton == MouseButtonState.Pressed;

            if (Ms1DownThisFrame && Ms1DownThisFrame != ms1LastFrame)
            {
                var ab = new b2AABB()
                {
                    lowerBound = worldPosition - Vector2.One,
                    upperBound = worldPosition + Vector2.One
                };

                var queryFilter = B2Util.QueryFilter(PhysicsCategory.All, PhysicsMask.All);

                B2Api.b2World_OverlapAABB(World, ab, queryFilter, qfn, 0);
            }
            else if (!Ms1DownThisFrame && Ms1DownThisFrame != ms1LastFrame)
            {
                if (Dragging != null)
                {
                    var delta = worldPosition - LastDragPosition;
                    var msDelta = MousePosition - LastMousePosition;

                    B2Api.b2Body_SetLinearVelocity(Dragging.Body, delta * msDelta.Length());
                }

                Dragging = null;
            }

            if (Dragging != null)
            {
                var body = Dragging.Body;

                LastDragPosition = worldPosition;
                LastMousePosition = MousePosition;

                B2Api.b2Body_SetLinearVelocity(body, Vector2.Zero);
                B2Api.b2Body_SetAngularVelocity(body, 0.0f);
                B2Api.b2Body_SetTransform(body, worldPosition + DraggingOffset, B2Api.b2Body_GetTransform(body).q);
            }
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition(Canvas);
            MousePosition = new Vector2((float)point.X, (float)point.Y);
        }

        Vector2 MousePosition = Vector2.Zero;

        bool Ms1DownThisFrame = false;

        GameControl? Dragging;
        Vector2 WorldMousePosition = Vector2.Zero;
        Vector2 DraggingOffset = Vector2.Zero;

        Vector2 LastDragPosition = Vector2.Zero;
        Vector2 LastMousePosition = Vector2.Zero;

        public bool IsKeyDown(Key key)
        {
            return Keys[key].isPressed;
        }

        /// <summary>
        /// returns 0/1
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int IsKeyDown01(Key key)
        {
            return IsKeyDown(key) ? 1 : 0;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            var state = Keys[e.Key];
            if (!state.isPressed)
            {
                state.pressedNow = true;
                state.pressedAt = Time;
            }
            state.isPressed = true;

            switch (e.Key)
            {
                case Key.Escape:
                    Paused = !Paused;
                    break;
                case Key.F1:
                    Window.LoadScene<DemoLevel>();
                    break;
            }
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            var state = Keys[e.Key];
            if (state.isPressed)
            {
                state.releasedNow = true;
                state.releasedAt = Time;
            }
            state.isPressed = false;
        }

        

        public void GameRender(float dt)
        {
            float SPD = 20.0f;

            var inputDir = new Vector2(IsKeyDown01(Key.D) - IsKeyDown01(Key.A), IsKeyDown01(Key.W) - IsKeyDown01(Key.S));

            var camRot = b2Rot.FromAngle(Camera.Angle);
            var camRot90 = b2Rot.FromAngle(Camera.Angle + MathF.PI * 0.5f);
            var camDir = new Vector2(camRot.c, camRot90.s);
            camDir = camDir.Length() > 0.0 ? Vector2.Normalize(camDir) : camDir;
            camDir *= inputDir * SPD;
            Camera.Position += camDir;

            //Console.WriteLine($"{camRot.c},{camRot.s} | {camRot90.c},{camRot90.s}");
            //Console.WriteLine(Camera.Angle);

            Camera.Angle += dt * (IsKeyDown01(Key.E) - IsKeyDown01(Key.Q));

            var halfCanvas = new Vector2((float)Canvas.ActualWidth / 2, (float)Canvas.ActualHeight / 2);

            var subject = _controls.OfType<Player>().First();

            //subj.BodyId.SetTransform(Camera.Position, b2Rot.Zero);
            Camera.Position = B2Api.b2Body_GetPosition(subject.Body);

            World.SetGravity(new Vector2(camRot90.c, -camRot90.s) * 10.0f);

            // Render GameControls
            foreach (var control in _iterControls)
            {
                Debug.Assert(B2Api.b2Body_IsValid(control.Body));

                var t = control.Body.GetTransform();
                var p = Vector2.Transform(t.p - Camera.Position, Matrix3x2.CreateRotation(Camera.Angle));
                var screenPosition = (p * ScalingFactor) + halfCanvas;

                control.Rotate.Angle = -(Camera.Angle + t.q.GetAngle()).RadToDeg();
                Canvas.SetLeft(control, screenPosition.X);
                Canvas.SetTop(control, (float)Canvas.ActualHeight - screenPosition.Y);
            }

            // The grid rotation is correct but the calculation does not account for the shifting grid
            // It does not preserve the grid where it should be.

            var pxCamera = Camera.Position * ScalingFactor;
            CanvasBrush.Transform = new TransformGroup()
            {
                Children = [
                    new TranslateTransform(-pxCamera.X % ScalingFactor, pxCamera.Y % ScalingFactor),
                    new RotateTransform(-Camera.Angle.RadToDeg()),
                ]
            };
        }

        // Concept Overlap method.
        // Use case: get shapes overlapping a shape without having to define a callback

        private List<GameShape> overlapCast;

        private bool OverlapCastFcn(b2ShapeId shapeId, IntPtr context)
        {
            if (_shapeLookUp.TryGetValue(shapeId.GetUserData(), out var result))
            {
                overlapCast.Add(result);
            }
            return true;
        }

        public List<GameShape> OverlapCast(in b2ShapeProxy proxy, b2QueryFilter filter)
        {
            overlapCast = [];

            World.OverlapShape(proxy, filter, OverlapCastFcn, 0);

            return overlapCast;
        }

        public b2RayResult RayCastClosest(Vector2 origin, Vector2 translation, b2QueryFilter filter)
        {
            return World.CastRayClosest(origin, translation, filter);
        }

        // Instead of closing the application, an exception is thrown.
        private int AssertFcn(string condition, string fileName, int lineNumber)
        {
            Trace.WriteLine($"BOX2D ASSERT FAILED: {condition}\n{fileName} : {lineNumber}");
            throw new InvalidOperationException($"BOX2D ASSERT FAILED: {condition}\n{fileName} : {lineNumber}");
        }
    }
}