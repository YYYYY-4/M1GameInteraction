using Box2dNet.Interop;
using System.Windows;

namespace Atlantis.Game
{
    public class ShapeDef
    {
        public static readonly DependencyProperty ShapeDefProperty = DependencyProperty.RegisterAttached("ShapeDef", typeof(ShapeDef), typeof(ShapeDef), new PropertyMetadata(null));
        public static void SetShapeDef(UIElement element, ShapeDef value) => element.SetValue(ShapeDefProperty, value);
        public static ShapeDef GetShapeDef(UIElement element) => (ShapeDef)element.GetValue(ShapeDefProperty);

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

        // Filter
        public ulong? CategoryBits { get; set; }
        public ulong? MaskBits { get; set; }
        public int? GroupIndex { get; set; }

        public void ApplyShapeDef(ref b2ShapeDef def)
        {
            Material?.ApplyToSurfaceMaterial(ref def.material);
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
