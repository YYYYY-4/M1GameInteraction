using Atlantis.Box2dNet;
using Box2dNet.Interop;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Windows;

namespace Atlantis.Game
{
    // Defines a sensor in the scene
    // A GameControl must inherit IWaterAreaReceiver to be updated,
    // and have sensor events be true
    public partial class WaterArea : GameControl
    {
        public float Density { get; set; } = 1.0f;

        public float Resistance { get; set; } = 1.0f;

        [TypeConverter(typeof(Vector2Converter))]
        public Vector2 WaterDirection { get; set; } = Vector2.Zero;

        public WaterArea()
        {
            InitializeComponent();

            _waterRect.Width = Width;
            _waterRect.Height = Height;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (DesignerMode)
            {
                if (e.Property == WidthProperty)
                {
                    _waterRect.Width = Width;
                }
                else if (e.Property == HeightProperty)
                {
                    _waterRect.Height = Height;
                }
            }
        }

        // Add this area to the receiver
        public override void OnSensorStart(GameShape sensor, GameShape visitor)
        {
            if (visitor.Control is WaterGameControl waterControl && !waterControl.WaterAreas.Contains(this))
            {
                waterControl.WaterAreas.Add(this);
            }
        }

        // Remove this area from receiver
        public override void OnSensorEnd(GameShape sensor, GameShape visitor)
        {
            if (visitor.Control is WaterGameControl waterControl)
            {
                waterControl.WaterAreas.Remove(this);
            }
        }
    }

    // This Control is updated by WaterArea, and provides a method to calculate the forces in water.
    // The reason this is inherited from instead of a IWaterGameControl
    // is because each user would have to implement the same code for the List.
    // And not every 
    // Future limitations: 
    public class WaterGameControl : GameControl
    {
        public List<WaterArea> WaterAreas { get; private set; } = [];

        protected float _submergeFactor = 0.0f;

        public bool IsInWater()
        {
            return WaterAreas.Count > 0;
        }

        protected float CalculateBuoyancyForce(IEnumerable<GameShape> shapes)
        {
            return 0.0f;
        }

        protected void UpdateWaterForces(GameShape shape)
        {
            // I tried looking into water physics but I don't know if I did it correctly. (I didn't do it correctly)
            // https://en.wikipedia.org/wiki/Drag_equation
            // https://en.wikipedia.org/wiki/Drag_coefficient
            // https://en.wikipedia.org/wiki/Buoyancy

            Debug.Assert(IsInWater());

            var waterSurface = WaterAreas.Max(area => area.Body.GetPosition().Y);

            var bodyGravity = Scene.World.GetGravity() * Body.GetGravityScale();

            float density = 0.95f;

            //float waterSurface = 20.0f;
            float bodyLowerY = Body.GetPosition().Y - 2.0f;
            float underWater = waterSurface - bodyLowerY;
            float submergeFactor = float.Clamp(underWater / shape.Size.Y, 0.0f, 1.0f);
            _submergeFactor = submergeFactor;

            var velocity = Body.GetLinearVelocity().Length();
            var area = shape.Size.X * shape.Size.Y;
            var submergedArea = area * submergeFactor;

            var buoyancy = (-bodyGravity) * submergedArea * density;

            float coDrag = (2.0f * -velocity) / (density * (velocity * velocity) * area);
            if (float.IsNaN(coDrag))
            {
                coDrag = 0.0f;
            }
            var drag = 0.5f * density * (velocity * velocity) * coDrag * submergedArea;

            Body.ApplyForceToCenter(buoyancy - (Body.GetLinearVelocity() * -drag));

            //Trace.WriteLine($"d: {drag}, cd: {coDrag}");
        }
    }
}