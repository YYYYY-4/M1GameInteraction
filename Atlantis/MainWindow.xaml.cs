using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Media;

using Box2dNet.Interop;
using System.Windows.Threading;

namespace Atlantis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public DispatcherTimer timer;

        List<YYYYWall> walls = [];

        b2WorldId worldId;

        //Vector2 ToRenderCoords(double x, double y)
        //{

        //}

        //Vector2 ToGameCoords(double left, double top)
        //{

        //}

        public MainWindow()
        {
            InitializeComponent();

            var wd = B2Api.b2DefaultWorldDef();
            wd.gravity = new Vector2(0, -10);
            worldId = B2Api.b2CreateWorld(wd);

            //var bodyDef = B2Api.b2DefaultBodyDef();
            //bodyDef.type = b2BodyType.b2_dynamicBody;
            //bodyDef.position = Vector2.Zero;

            //body = B2Api.b2CreateBody(worldId, bodyDef);


            //var shapeDef = B2Api.b2DefaultShapeDef();

            //b2Polygon pol = B2Api.b2MakeBox(0.5f, 0.5f);
            //B2Api.b2CreatePolygonShape(body, shapeDef, pol);

            //timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromMilliseconds(20);
            //timer.Start();

            //timer.Tick += TickUpdate;


            foreach (UIElement el in GameScene.Children)
            {
                var mt = Metadata.GetInfo(el);

                if (mt == "Wall")
                {
                    var wall = new YYYYWall();

                    Rectangle rect = (Rectangle)el;

                    var bodyd = B2Api.b2DefaultBodyDef();
                    bodyd.type = b2BodyType.b2_dynamicBody;
                    bodyd.position = new Vector2((float)Canvas.GetLeft(rect), (float)Canvas.GetBottom(rect));
                    var body = B2Api.b2CreateBody(worldId, bodyd);

                    var shapeDef1 = B2Api.b2DefaultShapeDef();
                    b2Polygon poly = B2Api.b2MakeBox((float)rect.Width, (float)rect.Height);
                    var shape = B2Api.b2CreatePolygonShape(body, shapeDef1, poly);

                    wall.body = body;
                    wall.rect = rect;

                    walls.Add(wall);
                }
            }

            sw.Start();
            CompositionTarget.Rendering += TickUpdate;
        }

        Stopwatch sw = new Stopwatch();

        double N = 2.0;

        private void TickUpdate(object? sender, EventArgs e)
        {
            double dt = sw.Elapsed.TotalSeconds;
            sw.Restart();

            FPSLBL.Content = $"FPS {1.0 / dt}";

            B2Api.b2World_Step(worldId, (float)dt, 4);
            foreach (var wall in walls)
            {
                var pos1 = B2Api.b2Body_GetPosition(wall.body);

                Canvas.SetLeft(wall.rect, pos1.X);
                Canvas.SetBottom(wall.rect, pos1.Y);

                //Debug.WriteLine($"{pos1.X}, {pos1.Y}");
            }

        }
    }

    class YYYYWall
    {
        public b2BodyId body;
        public Rectangle rect;
    }
}


