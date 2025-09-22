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

                var control = (GameControl)SpawnType.GetConstructor([])!.Invoke(null);
                Scene.ProcessGameControl(control, Body.GetTransform());

                var v = new Vector2((Random.Shared.NextSingle() - 0.5f) * 1000.0f, (Random.Shared.NextSingle() - 0.5f) * 1000.0f);
                control.Body.SetLinearVelocity(v);

                Spawned.Add(control, Scene.Time);
            }

            List<GameControl> Rm = [];
            foreach (var pair in Spawned)
            {
                if (Scene.Time - pair.Value > 1.0f)
                {
                    Rm.Add(pair.Key);
                }
            }

            Rm.ForEach(el => {
                Debug.WriteLine($"REMOVE {el}");
                Spawned.Remove(el);
                Scene.DestroyControl(el);
            });
        }
    }
}