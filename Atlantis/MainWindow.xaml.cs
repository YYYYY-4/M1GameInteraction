using Box2dNet.Interop;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Atlantis.Game;
using Atlantis.Scene;
using static System.Formats.Asn1.AsnWriter;

namespace Atlantis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GameScene Scene;

        public MainWindow()
        {
            // BOX2D ASSERTION: result.distanceSquared > 0.0f, C:\repos\box2d\src\manifold.c, line 848

            InitializeComponent();
            //Content = ((Page)Content).Content;
            //Scene = new(this);

            //LoadScene<TestPage>();

            // Expanding upon the designer to use it for Game Design:
            //  - To design a level 'elements' need to be placed
            //  - Are bindings relevant? Answer: For rendering, physics, game features. Probably not because they have a specific context/purpose which isn't very expandable. (It's not 1:1 but it does not expand well when the code grows bigger)
            //  - The Designer provides: Dragging base WPF elements. Which could be a powerfull tool if it can be used to design levels.
            // Unity?: GameObject rendering/physics in Frame done for user, Components can be added to a GameObject. Complex and difficult to fully implement.
            // UE?: I don't know, Actors and components, but also requires complex infrastructure (code routing) to fully implement.
            // Tags: Code/Classes can be registered to handle certain tagged objects, this way the if if if chain is replaced with a loop matching the tags and invoking callbacks.
            //       Simple to implement and easy to use, however some code might want to know of other tagged objects, button wants to know it's doors or other, which causes a dependency problem.
            //       If a button is loaded before it's door, it either can't initialize or would have to listen for added objects. Not desirable.
            // 
            // Result: Unti GameObject used as inspiration for XAML hierarchy. It allows defining a scene tree.
            //         UserControls allow visually rendering the scene
            //         Tags denied because the concept comes from lua and does not fit the c# language or wpf.

            //{ // ground, this will probably become a 'border' around the level with 4 box shapes
            //    b2BodyDef def = B2Api.b2DefaultBodyDef();
            //    def.position = new Vector2(0, 0);

            //    b2BodyId groundId = B2Api.b2CreateBody(WorldId, def);
            //    b2Polygon poly = B2Api.b2MakeBox(1e4f, 1.0f);

            //    b2ShapeDef groundShapeDef = B2Api.b2DefaultShapeDef();
            //    b2ShapeId shapeId = B2Api.b2CreatePolygonShape(groundId, groundShapeDef, poly);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Scene which inherits Page and defines a Canvas at it's root.</typeparam>
        public void LoadScene<T>() where T : Page
        {
            Scene.Destroy();

            var page = (Page)typeof(T).GetConstructors().First().Invoke(null);

            Content = page.Content;
            Scene = new GameScene(this);
        }

        //private void OnLoaded(object sender, RoutedEventArgs e)
        //{
        //    Debug.WriteLine("Window Loaded");
        //
        //    // Set viewport to entire canvas, Position to center of the viewport
        //    Camera.Viewport = new Vector2((float)GameCanvas.Width / ScalingFactor, (float)GameCanvas.Height / ScalingFactor);
        //    Camera.Position = Camera.Viewport / 2;
        //
        //    // In the designer the canvas is a fixed size, this is to translate their pos/angle to physics space.
        //    // Then remove the fixed size to have it use the entire screen
        //    double canvasActualHeight = GameCanvas.ActualHeight;
        //
        //    //GameCanvas.ClearValue(WidthProperty);
        //    //GameCanvas.ClearValue(HeightProperty);
        //
        //    Debug.WriteLine("Loading scene");
        //
        //    foreach (UIElement element in GameCanvas.Children.OfType<UIElement>().Where(el => el is Window))
        //    {
        //        string mt = Metadata.GetInfo(element);
        //        var shape = (Shape)element;
        //
        //        // Allow only Left and Top when loading elements
        //        Debug.Assert(double.IsNaN(Canvas.GetBottom(shape)));
        //        Debug.Assert(double.IsNaN(Canvas.GetRight(shape)));
        //
        //        if (double.IsNaN(Canvas.GetTop(shape)))
        //        {
        //            Canvas.SetTop(shape, 0.0);
        //        }
        //
        //        if (double.IsNaN(Canvas.GetLeft(shape)))
        //        {
        //            Canvas.SetLeft(shape, 0.0);
        //        }
        //
        //        // Translate Top to Bottom
        //        double bottom = canvasActualHeight - Canvas.GetTop(shape);
        //
        //        //if (shape is Polygon polyFix && polyFix.Points.Count > 0)
        //        //{
        //        //    var p1 = polyFix.Points.First();
        //        //    double minX = p1.X;
        //        //    double minY = p1.Y;
        //        //    double maxX = p1.X;
        //        //    double maxY = p1.Y;
        //        //    foreach (var p in polyFix.Points)
        //        //    {
        //        //        if (p.X < minX) minX = p.X;
        //        //        if (p.Y < minY) minY = p.Y;
        //        //        if (p.X > maxX) maxX = p.X;
        //        //        if (p.Y > maxY) maxY = p.Y;
        //        //    }
        //        //    if (double.IsNaN(shape.Width))
        //        //    {
        //        //        shape.Width = maxX - minX;
        //        //    }
        //        //    if (double.IsNaN(shape.Height))
        //        //    {
        //        //        shape.Height = maxY - minY;
        //        //    }
        //        //}
        //
        //        double width = shape.Width;
        //        double height = shape.Height;
        //
        //        var position = new Vector2((float)Canvas.GetLeft(shape), (float)bottom) / ScalingFactor;
        //        var size = new Vector2((float)width, (float)height) / ScalingFactor;
        //        var halfSize = size * 0.5f;
        //
        //        RotateTransform? designerRotate = null;
        //        if (shape.RenderTransform is RotateTransform tmpRotate)
        //        {
        //            designerRotate = tmpRotate;
        //        }
        //        else if (shape.RenderTransform is TransformGroup designerGroup)
        //        {
        //            designerRotate = (RotateTransform?)designerGroup.Children.FirstOrDefault(el => el is RotateTransform);
        //        }
        //
        //        // WPF rotates shapes Clockwise, but defines angles CounterClockwise.
        //        // Or it has something to do with Top Left based rendering. 360 - a works. Maybe -a would work too?
        //
        //        float definedAngleDeg = (float)(designerRotate?.Angle ?? 0.0);
        //        float dirAngle = (360.0f - definedAngleDeg).DegToRad();
        //        var directionX = (dirAngle + MathF.PI * 0.0f).AngleToDirection() * halfSize.X;
        //        var directionY = (dirAngle + MathF.PI * 0.5f).AngleToDirection() * halfSize.Y;
        //        position += directionX - directionY;
        //
        //        var bodyDef = B2Api.b2DefaultBodyDef();
        //        bodyDef.position = position;
        //        bodyDef.rotation = b2Rot.FromAngle((360.0f - definedAngleDeg).DegToRad());
        //
        //        bodyDef.type = b2BodyType.b2_dynamicBody;
        //        if (mt == "Ground")
        //        {
        //            bodyDef.type = b2BodyType.b2_staticBody;
        //        }
        //
        //        var body = B2Api.b2CreateBody(Scene.WorldId, bodyDef);
        //
        //        var shapeDef = B2Api.b2DefaultShapeDef();
        //        shapeDef.filter.maskBits = 0x1;
        //        shapeDef.filter.categoryBits = 0x1;
        //        shapeDef.filter.groupIndex = 0;
        //
        //        if (shape is Ellipse)
        //        {
        //            var circle = new b2Circle(Vector2.Zero, Math.Min(halfSize.X, halfSize.Y));
        //            B2Api.b2CreateCircleShape(body, shapeDef, circle);
        //        }
        //        else
        //        {
        //            var poly = B2Api.b2MakeBox(halfSize.X, halfSize.Y);
        //            B2Api.b2CreatePolygonShape(body, shapeDef, poly);
        //        }
        //
        //        // Make the shape rotate around it's center
        //        var rotate = new RotateTransform(definedAngleDeg, width * 0.5, height * 0.5);
        //        var translate = new TranslateTransform(-width * 0.5, -height * 0.5);
        //        var group = new TransformGroup
        //        {
        //            Children = [rotate, translate]
        //        };
        //
        //        shape.RenderTransform = group;
        //
        //        //Platforms.Add(new PlatformOld()
        //        //{
        //        //    Body = body,
        //        //    Shape = shape,
        //        //    Rotate = rotate,
        //        //    Translate = translate,
        //        //});
        //    }
        //
        //    //int i = 0;
        //    //while (true)
        //    //{
        //    //    i += 1;
        //    //    OnRendering(null, null);
        //    //
        //    //    if (watch.Elapsed.TotalSeconds >= 1.0)
        //    //    {
        //    //        Debug.WriteLine($"ticks: {i}");
        //    //        i = 0;
        //    //        watch.Restart();
        //    //    }
        //    //}
        //}

        static Matrix3x2 M3X2Inverse(Matrix3x2 m)
        {
            _ = Matrix3x2.Invert(m, out var result);
            return result;
        }

        static Matrix3x2 ToWorld(Matrix3x2 self, Matrix3x2 b)
        {
            return self * b;
        }

        static Matrix3x2 ToLocal(Matrix3x2 self, Matrix3x2 b)
        {
            return Matrix3x2.Identity;
        }
    }
}


