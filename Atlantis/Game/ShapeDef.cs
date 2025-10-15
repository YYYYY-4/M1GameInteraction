using Box2dNet.Interop;
using System.Windows;
using System.Windows.Media.Media3D;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Atlantis.Game
{
    public class ShapeDef
    {
        public static readonly DependencyProperty ShapeDefProperty = DependencyProperty.RegisterAttached("ShapeDef", typeof(ShapeDef), typeof(ShapeDef), new PropertyMetadata(null));
        public static void SetShapeDef(UIElement element, ShapeDef value) => element.SetValue(ShapeDefProperty, value);
        public static ShapeDef? GetShapeDef(UIElement element) => (ShapeDef)element.GetValue(ShapeDefProperty);

        public Material? Material { get; set; }
        public float? Density { get; set; }
        public bool? EnableCustomFiltering { get; set; }
        public bool? IsSensor { get; set; }
        public bool? EnableSensorEvents { get; set; }
        public bool? EnableContactEvents { get; set; }
        public bool? EnableHitEvents { get; set; }
        public bool? EnablePreSolveEvents { get; set; }
        public bool? InvokeContactCreation { get; set; }
        public bool? UpdateBodyMass { get; set; }
        public bool Destructible { get; set; }

        // Filter, prefer the Category/Mask setter
        public ulong? CategoryBits { get; set; }
        public ulong? MaskBits { get; set; }
        public int? GroupIndex { get; set; }

        // Material helper properties to reduce amount of xaml code needed to specify material properties.
        // They are applied before the Material if defined.
        public float? MatFriction { get; set; }
        public float? MatRestitution { get; set; }
        public float? MatRollingResistance { get; set; }
        public float? MatTangentSpeed { get; set; }
        public int? MatUserMaterialId { get; set; }
        public uint? MatCustomColor { get; set; }

        // See https://box2d.org/documentation/group__shape.html#structb2_filter

        // Typically 1 bit set
        public PhysicsCategory Category
        {
            set
            {
                CategoryBits = (ulong)value;
            }
        }

        // The categories this shape interacts with (collide/sensor)
        public PhysicsMask Mask
        {
            set
            {
                MaskBits = (ulong)value;
            }
        }

        public void ApplyShapeDef(ref b2ShapeDef def)
        {
            if (MatFriction.HasValue) def.material.friction = MatFriction.Value;
            if (MatRestitution.HasValue) def.material.restitution = MatRestitution.Value;
            if (MatRollingResistance.HasValue) def.material.rollingResistance = MatRollingResistance.Value;
            if (MatTangentSpeed.HasValue) def.material.tangentSpeed = MatTangentSpeed.Value;
            if (MatUserMaterialId.HasValue) def.material.userMaterialId = MatUserMaterialId.Value;
            if (MatCustomColor.HasValue) def.material.customColor = MatCustomColor.Value;
            if (Material.HasValue) Material.Value.ApplyToSurfaceMaterial(ref def.material);

            if (Density.HasValue) def.density = Density.Value;
            if (CategoryBits.HasValue) def.filter.categoryBits = CategoryBits.Value;
            if (MaskBits.HasValue) def.filter.maskBits = MaskBits.Value;
            if (GroupIndex.HasValue) def.filter.groupIndex = GroupIndex.Value;
            if (EnableCustomFiltering.HasValue) def.enableCustomFiltering = EnableCustomFiltering.Value;
            if (IsSensor.HasValue) def.isSensor = IsSensor.Value;
            if (EnableSensorEvents.HasValue) def.enableSensorEvents = EnableSensorEvents.Value;
            if (EnableContactEvents.HasValue) def.enableContactEvents = EnableContactEvents.Value;
            if (EnableHitEvents.HasValue) def.enableHitEvents = EnableHitEvents.Value;
            if (EnablePreSolveEvents.HasValue) def.enablePreSolveEvents = EnablePreSolveEvents.Value;
            if (InvokeContactCreation.HasValue) def.invokeContactCreation = InvokeContactCreation.Value;
            if (UpdateBodyMass.HasValue) def.updateBodyMass = UpdateBodyMass.Value;
        }
    }
}
