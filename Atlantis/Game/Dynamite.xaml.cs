using Atlantis.Box2dNet;
using Box2dNet.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Atlantis.Game
{
    public partial class Dynamite : GameControl
    {
        private string type = "dynamite";

        private List<GameShape> shapes = [];

        bool isExploding = false;
        bool isSpawned = false;
        float timer = 0.0f;

        private BitmapImage _dynamiteSprite = new BitmapImage();

        public Dynamite(bool exploding)
        {
            InitializeComponent();
            isExploding = exploding;
            DataContext = this;
            
        }

        public Dynamite()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string Type()
        {
            return type;
        }

        public static void SpawnDynamite(GameScene scene)
        {
            Player player = scene.Controls.OfType<Player>().First();
            if (player.HasDynamite == true)
            {
                Vector2 position = player.Body.GetPosition();
                bool isExploding = true;
                Dynamite dynamite = new Dynamite(isExploding);
                dynamite.isSpawned = true;
                scene.ProcessGameControl(dynamite, new b2Transform(position, b2Rot.Zero));
                dynamite._dynamiteSprite = (BitmapImage)Application.Current.FindResource("DynamiteIdle");
                player.HasDynamite = false;
            }
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if (isExploding)
            {
                timer += dt;

                if (timer >= 0.0f) _dynamiteSprite = (BitmapImage)Application.Current.FindResource("Dynamite0");
                if (timer >= 0.5f) _dynamiteSprite = (BitmapImage)Application.Current.FindResource("Dynamite1");
                if (timer >= 1.0f) _dynamiteSprite = (BitmapImage)Application.Current.FindResource("Dynamite2");
                if (timer >= 1.5f) _dynamiteSprite = (BitmapImage)Application.Current.FindResource("Dynamite3");
                if (timer >= 2.0f) _dynamiteSprite = (BitmapImage)Application.Current.FindResource("Dynamite4");
                if (timer >= 2.5f) _dynamiteSprite = (BitmapImage)Application.Current.FindResource("Dynamite5");

                dynamite.Source = _dynamiteSprite;

                if (timer >= 3.0f) ExplodeDynamite();
            }
        }

        public void ExplodeDynamite()
        {
            Vector2 position = Body.GetPosition();
            b2Circle circle = new b2Circle(position, 4.0f);
            b2ShapeProxy proxy = B2Api.b2MakeProxy([position], 1, circle.radius);
            b2QueryFilter filter = new b2QueryFilter(1, 1);
            shapes = Scene.OverlapCast(proxy, filter);

            foreach (GameShape shape in shapes)
            {
                if (shape.Destructible)
                {
                    Scene.DestroyControl(shape.Control);
                }
            }

            Scene.DestroyControl(this);

            timer = 0.0f;
            isExploding = false;
        }

        public int PickUp(Player player)
        {
            if (!isSpawned)
            {
                Scene.DestroyControl(this);
                player.HasDynamite = true;
                return 1;
            }

            return 0;
        }

        public override void OnSensorStart(GameShape sensor, GameShape visitor)
        {
            if (visitor.Control is Player player)
            {
                PickUp(player);
            }
        }
    }
}
