using Box2dNet.Interop;

namespace Atlantis.Game
{
    // This struct should be removed, unless materials are going to be re-used as wpf resources.
    // It has been replaced by Mat[Name] properties in ShapeDef
    public struct Material
    {
        public float? Friction { get; set; }
        public float? Restitution { get; set; }
        public float? RollingResistance { get; set; }
        public float? TangentSpeed { get; set; }
        public int? UserMaterialId { get; set; }
        public uint? CustomColor { get; set; }

        public void ApplyToSurfaceMaterial(ref b2SurfaceMaterial material)
        {
            if (Friction.HasValue) material.friction = Friction.Value;
            if (Restitution.HasValue) material.restitution = Restitution.Value;
            if (RollingResistance.HasValue) material.rollingResistance = RollingResistance.Value;
            if (TangentSpeed.HasValue) material.tangentSpeed = TangentSpeed.Value;
            if (UserMaterialId.HasValue) material.userMaterialId = UserMaterialId.Value;
            if (CustomColor.HasValue) material.customColor = CustomColor.Value;
        }
    }
}
