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
using static Atlantis.Box2dNet.B2Extension;

namespace Atlantis.Game
{
    public partial class Dynamite : Item
    {
        private List<GameShape> shapes = [];
        
        private bool _isExploding = false;
        private float _timer = 0.0f;
        private bool _isPlayerDestroyed = false;


        private BitmapImage _dynamiteSprite = new BitmapImage();

        /// <summary>
        /// Creates a dynamite where the isExploding variable gets set
        /// </summary>
        /// <param name="exploding"></param>
        public Dynamite(bool exploding)
        {
            InitializeComponent();
            _isExploding = exploding;
            DataContext = this;
            
        }

        /// <summary>
        /// Creates a dynamite
        /// </summary>
        public Dynamite()
        {
            InitializeComponent();
            DataContext = this;
        }

        public override void Drop()
        {
            IsPickup = false;
            _isExploding = true;
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if (_isExploding)
            {
                _timer += dt;
                if (_timer >= 3.5f) Scene.DestroyControl(this);
                else if (_timer >= 3.0f) ExplodeDynamite();
                else if (_timer >= 2.5f) _dynamiteSprite = (BitmapImage)Application.Current.FindResource("Dynamite5");
                else if (_timer >= 2.0f) _dynamiteSprite = (BitmapImage)Application.Current.FindResource("Dynamite4");
                else if (_timer >= 1.5f) _dynamiteSprite = (BitmapImage)Application.Current.FindResource("Dynamite3");
                else if (_timer >= 1.0f) _dynamiteSprite = (BitmapImage)Application.Current.FindResource("Dynamite2");
                else if (_timer >= 0.5f) _dynamiteSprite = (BitmapImage)Application.Current.FindResource("Dynamite1");
                else _dynamiteSprite = (BitmapImage)Application.Current.FindResource("Dynamite0");

                dynamite.Source = _dynamiteSprite;
            }
        }

        /// <summary>
        /// Checks for destructible components within a 8 unit (200 pixel) radius of the dynamite and removes them from the scene.
        /// And casts an explosion sprite over the dynamite while disabling physics.
        /// </summary>
        public void ExplodeDynamite()
        {
            Vector2 position = Body.GetPosition();
            b2Circle circle = new b2Circle(position, 8.0f);
            b2ShapeProxy proxy = B2Api.b2MakeProxy([position], 1, circle.radius);
            b2QueryFilter filter = B2Util.QueryFilter(PhysicsCategory.All, PhysicsMask.Map);
            shapes = Scene.OverlapCast(proxy, filter);

            _dynamiteSprite = (BitmapImage)Application.Current.FindResource("Explosion");
            Explosion.Visibility = Visibility.Visible;
            dynamite.Visibility = Visibility.Hidden;
            Body.Disable();

            // Checks if a shape within the explosion is destructible.
            // Also checks if the shape is a player element and if the player has already been destroyed.
            // This is done as a player element has multiple shapes and removing the same player element multiple times causes a crash.
            foreach (GameShape shape in shapes)
            {
                if (shape.Destructible && shape.Control is not Player)
                    Scene.DestroyControl(shape.Control);
                else if (shape.Destructible && shape.Control is Player && _isPlayerDestroyed == false)
                {
                    Scene.DestroyControl(shape.Control);
                    _isPlayerDestroyed = true;
                }
            }
        }

        public override string GetDisplayName()
        {
            return "Dynamite";
        }

        public override ImageSource GetIconResource()
        {
            return (ImageSource)FindResource("DynamiteIdle");
        }
    }
}
