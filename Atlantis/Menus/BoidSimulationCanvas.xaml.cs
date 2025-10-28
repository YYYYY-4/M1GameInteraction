using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Atlantis.Menus
{
    /// <summary>
    /// Interaction logic for BoidSimTest.xaml
    /// </summary>
    public partial class BoidSimulationCanvas : Canvas
    {
        private List<Boid> _boids = [];

        private List<Boid> _deadBoids = [];

        private Stopwatch _stopwatch = new();

        public Panel? MountedPanel = null;

        public static readonly List<string> NiceFish = [
            "Fish0_0",
            "Fish0_1",
            "Fish1_0",
            "Fish1_1",
            "Fish2_0",
            "Fish2_1",
            "Fish3_0",
            "Fish4_0",
            "Fish5_0"
        ];

        public static readonly List<string> EvilFish = [
            "Fish3_1",
            "Fish4_1",
            "Fish5_1"
        ];

        public BoidSimulationCanvas()
        {
            InitializeComponent();

            _stopwatch.Start();
        }

        public void MountPanel(Panel panel)
        {
            UnmountPanel();

            MountedPanel = panel;
            MountedPanel.Children.Add(this);

            Grid.SetRowSpan(this, int.MaxValue);
            Grid.SetColumnSpan(this, int.MaxValue);
            Panel.SetZIndex(this, -200);

            CompositionTarget.Rendering += OnRendering;
        }

        public void UnmountPanel()
        {
            CompositionTarget.Rendering -= OnRendering;

            MountedPanel?.Children.Remove(this);
            MountedPanel = null;
        }

        private void SpawnRandomFish(int fishCount, int predatorCount)
        {
            for (var i = 0; i < fishCount; i++)
            {
                bool predator = i < predatorCount;

                var element = new Image();

                string fish = predator ? EvilFish[Random.Shared.Next(EvilFish.Count)] : NiceFish[Random.Shared.Next(NiceFish.Count)];
                Canvas.SetZIndex(element, predator ? 1 : 0);
                element.Source = (BitmapImage)App.Current.FindResource(fish);
                element.Stretch = Stretch.Fill;
                element.Width = 50.0;
                element.Height = 50.0;

                var scale = new ScaleTransform(1.0, 1.0);
                var rotate = new RotateTransform(0.0);

                element.RenderTransform = new TransformGroup() { Children = [new TranslateTransform(-element.Width / 2, -element.Height / 2), scale, rotate] };
                Children.Add(element);

                _boids.Add(new Boid()
                {
                    Position = new Vector2((float)(Random.Shared.NextDouble() * ActualWidth), (float)(Random.Shared.NextDouble() * ActualHeight)),
                    Velocity = Vector2.Zero,

                    Name = fish,
                    Predator = predator,

                    Element = element,
                    Scale = scale,
                    Rotate = rotate,
                });
            }
        }

        private void OnRendering(object? sender, EventArgs e)
        {
            var deltaTime = _stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Restart();

            UpdateBoids((float)deltaTime);
        }

        float FishTimer = 10.0f;

        // Update boid simulation
        public void UpdateBoids(float dt)
        {
            // Used as reference https://swharden.com/csdv/simulations/boids/

            if (!IsLoaded)
            {
                return;
            }

            FishTimer += dt;
            if (FishTimer > 10.0f)
            {
                FishTimer = 0.0f;
                SpawnRandomFish(10, 2);
            }

            float visionDistance = 200.0f;
            float avoidDistance = 50.0f;
            float flockPower = 0.005f;
            float alignPower = 0.001f;
            float avoidPower = 0.01f;

            foreach (var boid in _boids)
            {
                int meanCount = 0;
                Vector2 meanPosition = Vector2.Zero;
                Vector2 meanVelocity = Vector2.Zero;

                Vector2 avoidVelocity = Vector2.Zero;

                foreach (var nearBoid in _boids)
                {
                    if (nearBoid == boid) continue;

                    float distance = Vector2.Distance(boid.Position, nearBoid.Position);

                    if (distance <= visionDistance)
                    {
                        meanCount++;
                        meanPosition += nearBoid.Position;
                        meanVelocity += nearBoid.Velocity;
                    }

                    if (distance <= avoidDistance)
                    {
                        float delta = avoidDistance - Vector2.Distance(nearBoid.Position, boid.Position);

                        // Predators 'avoid' towards nearby innocent fish.  (:
                        // They only avoid their own species.
                        if (boid.Predator && nearBoid.Name != boid.Name)
                        {
                            avoidVelocity += (nearBoid.Position - boid.Position) * delta;
                        }
                        else
                        {
                            avoidVelocity += (boid.Position - nearBoid.Position) * delta;
                        }
                    }
                }

                float crowdFactor = Math.Clamp(meanCount / 10f, 0f, 2f);

                if (meanCount > 0)
                {
                    float _flockPower = flockPower / (1f + crowdFactor);

                    meanPosition /= meanCount;
                    meanVelocity /= meanCount;

                    var deltaCenter = meanPosition - boid.Position;

                    var flock = deltaCenter * _flockPower;
                    var align = meanVelocity * alignPower;

                    boid.Velocity += flock + align;
                }

                float _avoidPower = avoidPower * (1f + crowdFactor * 0.5f);
                var avoid = avoidVelocity * _avoidPower;

                boid.Velocity += avoid;
            }

            foreach (var boid in _boids.Where(b => b.Predator).ToList())
            {
                var dead = _boids.Where(near => boid != near && Vector2.Distance(near.Position, boid.Position) < 25);

                foreach (var deadBoid in dead.ToList())
                {
                    _boids.Remove(deadBoid);
                    _deadBoids.Add(deadBoid);

                    deadBoid.Rotate.Angle = 0.0;
                    deadBoid.Scale.ScaleX = 1f;
                    deadBoid.Scale.ScaleY = -1f;

                    deadBoid.Velocity = new Vector2(0f, 40f);

                    boid.Velocity *= 2f;
                }
            }

            foreach (var boid in _deadBoids)
            {
                boid.DeadTime += dt;
                boid.Position += boid.Velocity * dt;
                Canvas.SetBottom(boid.Element, boid.Position.Y);
                Canvas.SetLeft(boid.Element, boid.Position.X);
                boid.Element.Opacity = (5f - boid.DeadTime) / 5f;
            }

            foreach (var boid in _deadBoids.Where(b => b.DeadTime >= 5f).ToList())
            {
                Children.Remove(boid.Element);
                _deadBoids.Remove(boid);
            }

            foreach (var boid in _boids)
            {
                // Keep velocity between min and max
                float maxVelocity = 200f;
                if (boid.Predator)
                {
                    maxVelocity = 250f;
                }

                if (boid.Velocity.Length() > maxVelocity)
                {
                    boid.Velocity = Vector2.Normalize(boid.Velocity) * maxVelocity;
                }
                else if (boid.Velocity.Length() < 10.0f)
                {
                    boid.Velocity = Vector2.Normalize(boid.Velocity) * 10f;
                }

                if (float.IsNaN(boid.Velocity.X))
                {
                    boid.Velocity.X = 0.0f;
                }

                if (float.IsNaN(boid.Velocity.Y))
                {
                    boid.Velocity.Y = 0.0f;
                }

                boid.Position += boid.Velocity * dt;

                // Move away from edge
                float edgePadding = 5.0f;
                float edgeTurn = 10f;

                if (boid.Position.X < edgePadding)
                {
                    boid.Velocity.X += edgeTurn;
                }

                if (boid.Position.X > ((float)ActualWidth) - edgePadding)
                {
                    boid.Velocity.X -= edgeTurn;
                }

                if (boid.Position.Y < edgePadding)
                {
                    boid.Velocity.Y += edgeTurn;
                }

                if (boid.Position.Y > ((float)ActualHeight) - edgePadding)
                {
                    boid.Velocity.Y -= edgeTurn;
                }

                // Position and rotation in canvas
                Canvas.SetBottom(boid.Element, boid.Position.Y);
                Canvas.SetLeft(boid.Element, boid.Position.X);

                var vn = Vector2.Normalize(boid.Velocity);
                float angle = float.Atan2(vn.Y, vn.X);
                angle = -float.RadiansToDegrees(angle);

                boid.Scale.ScaleY = vn.X >= 0f ? 1f : -1f;
                boid.Rotate.Angle = angle;
            }
        }
    }

    public class Boid
    {
        public Vector2 Position;
        public Vector2 Velocity;

        public string Name;
        public bool Predator = false;
        public float DeadTime = 0f;

        public UIElement Element = null!;
        public ScaleTransform Scale = null!;
        public RotateTransform Rotate = null!;

        public Boid()
        {
            {
                Position = Vector2.Zero;
                Velocity = Vector2.Zero;
            }
        }

        public Boid(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
        }
    }
}
