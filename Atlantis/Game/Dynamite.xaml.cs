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
        float timer = 0.0f;

        public Dynamite()
        {
            InitializeComponent();
        }

        public string Type()
        {
            return type;
        }

        public static void SpawnDynamite(GameScene scene)
        {
            Player player = scene.Controls.OfType<Player>().First();
            Vector2 position = player.Body.GetPosition();
            Dynamite dynamite = new Dynamite();
            scene.ProcessGameControl(dynamite, new b2Transform(position, b2Rot.Zero));

        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            //if (isExploding)
            //{
                timer += dt;

                if (timer >= 3.0f)
                {
                    ExplodeDynamite();
                }
            //}

            //if (Scene.Keys[Key.R].isPressed)
            //{
            //    isExploding = true;
            //}
        }

        public void ExplodeDynamite()
        {
            Vector2 position = Body.GetPosition();
            b2Circle circle = new b2Circle(position, 3.0f);
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
    }
}
