using Box2dNet.Interop;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Atlantis.Box2dNet;
using Atlantis.Scene;

namespace Atlantis.Game;

/// <summary>
/// Scene class that loads a xaml hierarchy into runtime.
/// If resources and events are not cleared up properly (see Unload method) things will go wrong sooner or later.
/// </summary>
public class GameScene
{
    private bool _controlsChanged = true;
    private readonly List<GameControl> _controls = new List<GameControl>();
    private readonly List<GameControl> _addedControls = new List<GameControl>();

    // Copyy of Controls to iterate safely while supporting adding/removing objects
    private List<GameControl> _iterControls = new List<GameControl>();

    const float ScalingFactor = 25.0f;

    public b2WorldId World;

    private readonly MainWindow _window;
    private readonly Canvas _canvas;

    private DrawingBrush _canvasBrush;

    private nint _shapeIdGen = 0;

    // see GameControl.CID
    private long _controlIdGen = 0;

    // When removing a Body from the Scene it must also be removed from the lookup
    private readonly Dictionary<nint, GameShape> _shapeLookup = new Dictionary<nint, GameShape>();

    public Dictionary<Key, KeyState> Keys;

    private Camera Camera { get; set; }

    /// <summary>
    /// Elapsed simulated time since scene loaded in seconds.
    /// </summary>
    public float Time { get; private set; } = 0.0f;

    private readonly Stopwatch _watch = new Stopwatch();

    private TimeSpan _lastElapsed = TimeSpan.Zero;
        
    private Vector2 _mousePosition = Vector2.Zero;

    private bool _ms1DownThisFrame = false;

    private GameControl? _dragging;
    private Vector2 _worldMousePosition = Vector2.Zero;
    private Vector2 _draggingOffset = Vector2.Zero;

    private Vector2 _lastDragPosition = Vector2.Zero;
    private Vector2 _lastMousePosition = Vector2.Zero;
        
    private List<GameShape> _overlapCast;

    public GameScene(MainWindow window)
    {
        _window = window;
        _canvas = (Canvas)window.Content;

        Camera = new Camera();

        if (_canvas.IsLoaded)
        {
            Content_Loaded(null!, null!);
        }
        else
        {
            _canvas.Loaded += Content_Loaded;
        }
    }

    public void Destroy()
    {
        if (B2Api.b2World_IsValid(World))
        {
            Unload(null, null!);
        }
        else
        {
            _canvas.Loaded -= Content_Loaded;
        }
    }

    private void Content_Loaded(object sender, RoutedEventArgs e)
    {
        _canvas.ClearValue(Canvas.WidthProperty);
        _canvas.ClearValue(Canvas.HeightProperty);

        _canvasBrush = new DrawingBrush
        {
            TileMode = TileMode.Tile,
            Viewport = new Rect(0, 0, 25, 25),
            ViewportUnits = BrushMappingMode.Absolute,
            Drawing = new GeometryDrawing(Brushes.Transparent, new Pen(Brushes.LightGray, 1), new GeometryGroup
            {
                Children = [new LineGeometry(new Point(0, 0), new Point(25, 0)), new LineGeometry(new Point(0, 0), new Point(0, 25))]
            })
        };

        _canvas.Background = _canvasBrush;

        b2WorldDef worldDef = B2Api.b2DefaultWorldDef();
        worldDef.enableSleep = false;

        World = B2Api.b2CreateWorld(worldDef);

        B2Api.b2SetAssertFcn(AssertFcn);

        LoadGameControls(_canvas, (float)_canvas.ActualHeight, 0.0, 0.0);
        InvisGroundBody();

            

        _watch.Start();

        Keys = new Dictionary<Key, KeyState>();
        foreach (var key in Enum.GetValues(typeof(Key)).Cast<Key>())
        {
            Keys[key] = new KeyState();
        }

        CompositionTarget.Rendering += OnRendering;
        _window.MouseMove += MainWindow_MouseMove;
        _window.KeyDown += MainWindow_KeyDown;
        _window.KeyUp += MainWindow_KeyUp;

        _window.Closed += Unload;
    }

    private void Unload(object? sender, EventArgs e)
    {
        CompositionTarget.Rendering -= OnRendering;
        _window.MouseMove -= MainWindow_MouseMove;
        _window.KeyDown -= MainWindow_KeyDown;
        _window.KeyUp -= MainWindow_KeyUp;
        _window.Closed -= Unload;

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

        control.CID = ++_controlIdGen;
        control.Scene = this;

        List<Shape>? shapes = null;
        if (control.Content is Shape cShape)
        {
            shapes = [cShape];
        }
        else if (control.Content is Canvas canvas)
        {
            shapes = canvas.Children.OfType<Shape>().ToList();
        }

        if (shapes == null || shapes.Count == 0)
        {
            throw new NotImplementedException();
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
        control.Shapes = new List<GameShape>();

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
                    else if (p.X > maxX) maxX = p.X;
                        
                    if (p.Y < minY) minY = p.Y;
                    else if (p.Y > maxY) maxY = p.Y;
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
                    else if (p.X > maxX) maxX = p.X;
                        
                    if (p.Y < minY) minY = p.Y;
                    else if (p.Y > maxY) maxY = p.Y;
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
            shapeDef.filter.maskBits = 0x1;
            shapeDef.filter.categoryBits = 0x1;
            shapeDef.filter.groupIndex = 0;

            // Body ShapeDef applied before direct ShapeDef
            control.ModifyShapeDef(ref shapeDef);
            ShapeDef.GetShapeDef(shape)?.ApplyShapeDef(ref shapeDef);

            // Userdata is sacred
            shapeDef.userData = _shapeIdGen++;

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
            if (shape is Rectangle)
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

            // Shape is defined/rendered relative to the Body root Element and doesn't need to be centered. That's why it's skipped.

            if (physShape != null)
            {
                var gameShape = new GameShape()
                {
                    Control = control,
                    Shape = (b2ShapeId)physShape,
                    Element = shape,

                    Offset = offset,
                    HalfSize = halfSize,
                };

                _shapeLookup.Add(shapeDef.userData, gameShape);
                control.Shapes.Add(gameShape);
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
            _shapeLookup.Remove(shape.Shape.GetUserData());
        }

        B2Api.b2DestroyBody(control.Body);
        control.Body = new b2BodyId();

        _canvas.Children.Remove(control);
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
            var q = b2Rot.FromAngle((float)designerBodyAngle);

            ProcessGameControl(control, new b2Transform(p, q));
        }

        void processShape(Shape shape)
        {
            var control = new GameControl();

            Canvas.SetTop(control, Canvas.GetTop(shape));
            Canvas.SetLeft(control, Canvas.GetLeft(shape));
            Canvas.SetTop(shape, 0.0);
            Canvas.SetLeft(shape, 0.0);

            // Note: RenderTransform is not handled and Angle might be wrong.

            canvas.Children.Remove(shape);
            control.Content = shape;

            _canvas.Children.Add(control);
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
            else
            {
                throw new NotImplementedException($"Unhandled Type: {element.GetType()}");
            }
        }
    }

    private void OnRendering(object? sender, EventArgs e)
    {
        TimeSpan now = _watch.Elapsed;
        float dt = (float)(now - _lastElapsed).TotalSeconds;
        _lastElapsed = now;

        dt = Math.Min(dt, 1.0f / 30.0f);
        Time += dt;

        // Start new controls
        if (_addedControls.Count > 0)
        {
            _controlsChanged = true;
            foreach (var control in _addedControls.ToList())
            {
                _controls.Add(control);
                if (control.Parent == null) _canvas.Children.Add(control);
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
                if (control.Parent == null) _canvas.Children.Add(control);
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

    bool Qfn(b2ShapeId shapeId, nint ctx)
    {
        var body = B2Api.b2Shape_GetBody(shapeId);
        _dragging = _controls.FirstOrDefault(p => p.Body == body);
        _draggingOffset = body.GetPosition() - _worldMousePosition;
        return false;
    }

    public void GameUpdate(float dt)
    {
        float speed = 1.0f;

        World.Step(dt / speed, 64);

        // process sensor events, note one shape must be a Sensor and the both must enable sensor events in ShapeDef.
        var sensorEvents = World.GetSensorEvents();

        foreach (var ev in sensorEvents.beginEventsAsSpan)
        {
            if (_shapeLookup.TryGetValue(ev.sensorShapeId.GetUserData(), out var sensorShape) && _shapeLookup.TryGetValue(ev.visitorShapeId.GetUserData(), out var visotorShape))
            {
                sensorShape.Control.OnSensorStart(sensorShape, visotorShape);
            }
        }

        foreach (var ev in sensorEvents.endEventsAsSpan)
        {
            if (_shapeLookup.TryGetValue(ev.sensorShapeId.GetUserData(), out var sensorShape) && _shapeLookup.TryGetValue(ev.visitorShapeId.GetUserData(), out var visotorShape))
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

        Vector2 worldCoords = new Vector2(_mousePosition.X / ScalingFactor, (float)(_canvas.ActualHeight - _mousePosition.Y) / ScalingFactor);
        Vector2 halfCanvas = new Vector2((float)_canvas.ActualWidth / 2, (float)_canvas.ActualHeight / 2) / ScalingFactor;
        worldCoords += Camera.Position - halfCanvas;

        Vector2 worldPosition = worldCoords;
        _worldMousePosition = worldPosition;

        bool ms1LastFrame = _ms1DownThisFrame;
        _ms1DownThisFrame = Mouse.LeftButton == MouseButtonState.Pressed;

        if (_ms1DownThisFrame && _ms1DownThisFrame != ms1LastFrame)
        {
            var ab = new b2AABB()
            {
                lowerBound = worldPosition - Vector2.One,
                upperBound = worldPosition + Vector2.One
            };

            var queryFilter = new b2QueryFilter
            {
                categoryBits = 0x1,
                maskBits = 0x1
            };

            B2Api.b2World_OverlapAABB(World, ab, queryFilter, Qfn, 0);
        }
        else if (!_ms1DownThisFrame && _ms1DownThisFrame != ms1LastFrame)
        {
            if (_dragging != null)
            {
                Vector2 delta = worldPosition - _lastDragPosition;
                Vector2 msDelta = _mousePosition - _lastMousePosition;

                B2Api.b2Body_SetLinearVelocity(_dragging.Body, delta * msDelta.Length());
            }

            _dragging = null;
        }

        if (_dragging != null)
        {
            var body = _dragging.Body;

            _lastDragPosition = worldPosition;
            _lastMousePosition = _mousePosition;

            B2Api.b2Body_SetLinearVelocity(body, Vector2.Zero);
            B2Api.b2Body_SetAngularVelocity(body, 0.0f);
            B2Api.b2Body_SetTransform(body, worldPosition + _draggingOffset, B2Api.b2Body_GetTransform(body).q);
        }
    }

    private void MainWindow_MouseMove(object sender, MouseEventArgs e)
    {
        var point = e.GetPosition(_canvas);
        _mousePosition = new Vector2((float)point.X, (float)point.Y);
    }

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
                _window.Close();
                break;
            case Key.F1:
                _window.LoadScene<TestPage>();
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

        Camera.Angle += dt * (IsKeyDown01(Key.E) - IsKeyDown01(Key.Q));

        var halfCanvas = new Vector2((float)_canvas.ActualWidth / 2, (float)_canvas.ActualHeight / 2);

        var subject = _controls.OfType<Player>().First();
            
        Camera.Position = B2Api.b2Body_GetPosition(subject.Body);

        World.SetGravity(new Vector2(camRot90.c, -camRot90.s) * 10.0f);

        // Render GameControls
        foreach (var control in _iterControls)
        {
            Debug.Assert(B2Api.b2Body_IsValid(control.Body));

            b2Transform t = control.Body.GetTransform();
            Vector2 p = Vector2.Transform(t.p - Camera.Position, Matrix3x2.CreateRotation(Camera.Angle));
            Vector2 screenPosition = (p * ScalingFactor) + halfCanvas;

            control.Rotate.Angle = -(Camera.Angle + t.q.GetAngle()).RadToDeg();
            Canvas.SetLeft(control, screenPosition.X);
            Canvas.SetTop(control, (float)_canvas.ActualHeight - screenPosition.Y);
        }

        Vector2 pxCamera = Camera.Position * ScalingFactor;
        _canvasBrush.Transform = new TranslateTransform(-pxCamera.X % ScalingFactor, pxCamera.Y % ScalingFactor);
    }

    // Concept Overlap method.
    // Use case: get shapes overlapping a shape without having to define a callback

    private bool OverlapCastFcn(b2ShapeId shapeId, IntPtr context)
    {
        if (_shapeLookup.TryGetValue(shapeId.GetUserData(), out GameShape? result))
        {
            _overlapCast.Add(result);
        }
        return true;
    }

    public List<GameShape> OverlapCast(in b2ShapeProxy proxy, b2QueryFilter filter)
    {
        _overlapCast = new List<GameShape>();

        World.OverlapShape(proxy, filter, OverlapCastFcn, 0);

        return _overlapCast;
    }

    // Instead of closing the application, an exception is thrown.
    private int AssertFcn(string condition, string fileName, int lineNumber)
    {
        Trace.WriteLine($"BOX2D ASSERT FAILED: {condition}\n{fileName} : {lineNumber}");
        throw new InvalidOperationException($"BOX2D ASSERT FAILED: {condition}\n{fileName} : {lineNumber}");
    }
}