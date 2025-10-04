using System.Diagnostics;
using System.Windows;
using Atlantis.Box2dNet;
using Box2dNet.Interop;
using System.Numerics;

namespace Atlantis.Game
{



    /// <summary>
    /// Spawns boxes to test performance
    /// </summary>
    public partial class BoxSpawner : GameControl
    {
        public BoxSpawner()
        {
            InitializeComponent();

            SpawnRect.Width = Width;
            SpawnRect.Height = Height;
        }

        // Adjust to Parent dimension
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (DesignerMode)
            {
                if (e.Property == WidthProperty)
                {
                    SpawnRect.Width = Width;
                }
                else if (e.Property == HeightProperty)
                {
                    SpawnRect.Height = Height;
                }
            }
        }

        public float SpawnSpeed { get; set; }

        public float SpawnAngle0 { get; set; }

        public float SpawnAngle1 { get; set; }

        public float SpawnWidth { get; set; } = 50.0f;

        public float SpawnHeight { get; set; } = 50.0f;

        /// <summary>
        /// Seconds to wait between spawns
        /// </summary>
        public float SpawnDelay { get; set; } = 1.0f;

        public Type SpawnType { get; set; }

        public override void OnStart()
        {
            base.OnStart();

            Debug.Assert(typeof(GameControl).IsAssignableFrom(SpawnType));
            Watch = Stopwatch.StartNew();
        }

        Stopwatch Watch;

        Dictionary<GameControl, float> Spawned = [];

        public override void OnUpdate(float dt)
        {
            if (Watch.Elapsed.TotalSeconds >= SpawnDelay)
            {
                Watch.Restart();

                var t = Body.GetTransform();

                var offset = Shapes[0].HalfSize * 2.0f * new Vector2(Random.Shared.NextSingle(), Random.Shared.NextSingle());
                offset.Y = -offset.Y;

                t.p += offset;

                var control = (GameControl)SpawnType.GetConstructor([])!.Invoke(null);

                control.Width = SpawnWidth;
                control.Height = SpawnHeight;

                Scene.ProcessGameControl(control, t);

                var v = new Vector2((Random.Shared.NextSingle() - 0.5f) * 1.0f, (Random.Shared.NextSingle() - 0.5f) * 1.0f);
                v = Vector2.Zero;
                control.Body.SetLinearVelocity(v);

                Spawned.Add(control, Scene.Time);
            }

            List<GameControl> rm = [];
            foreach (var pair in Spawned)
            {
                if (Scene.Time - pair.Value > 1.0f)
                {
                    rm.Add(pair.Key);
                }
            }

            rm.ForEach(el => {
                Spawned.Remove(el);
                Scene.DestroyControl(el);
            });
        }
    }
}