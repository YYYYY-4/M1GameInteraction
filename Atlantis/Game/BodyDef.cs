using Box2dNet.Interop;
using System.ComponentModel;
using System.Numerics;
using System.Windows;

namespace Atlantis.Game
{

    /// <summary>
    /// Definition class that can be used in xaml to attach b2BodyDef properties to a GameControl Body
    /// </summary>
    public class BodyDef
    {
        public static readonly DependencyProperty BodyDefProperty = DependencyProperty.RegisterAttached("BodyDef", typeof(BodyDef), typeof(BodyDef), new PropertyMetadata(null));
        public static void SetBodyDef(UIElement element, BodyDef value) => element.SetValue(BodyDefProperty, value);
        public static BodyDef GetBodyDef(UIElement element) => (BodyDef)element.GetValue(BodyDefProperty);

        public b2BodyType? Type { get; set; }
        //public Vector2? Position { get; set; }
        //public b2Rot? Rotation { get; set; }
        [TypeConverter(typeof(Vector2Converter))]
        public Vector2? LinearVelocity { get; set; }
        public float? AngularVelocity { get; set; }
        public float? LinearDamping { get; set; }
        public float? AngularDamping { get; set; }
        public float? GravityScale { get; set; }
        public float? SleepThreshold { get; set; }
        public string? Name { get; set; }
        public IntPtr? UserData { get; set; }

        [TypeConverter(typeof(MotionLocksConverter))]
        public b2MotionLocks? MotionLocks { get; set; }
        public bool? EnableSleep { get; set; }
        public bool? IsAwake { get; set; }
        public bool? IsBullet { get; set; }
        public bool? IsEnabled { get; set; }
        public bool? AllowFastRotation { get; set; }

        public void ApplyBodyDef(ref b2BodyDef def)
        {
            if (Type.HasValue) def.type = Type.Value;
            //if (Position.HasValue) def.position = Position.Value;
            //if (Rotation.HasValue) def.rotation = Rotation.Value;
            if (LinearVelocity.HasValue) def.linearVelocity = LinearVelocity.Value;
            if (AngularVelocity.HasValue) def.angularVelocity = AngularVelocity.Value;
            if (LinearDamping.HasValue) def.linearDamping = LinearDamping.Value;
            if (AngularDamping.HasValue) def.angularDamping = AngularDamping.Value;
            if (GravityScale.HasValue) def.gravityScale = GravityScale.Value;
            if (SleepThreshold.HasValue) def.sleepThreshold = SleepThreshold.Value;
            if (Name != null) def.name = Name;
            if (UserData.HasValue) def.userData = UserData.Value;
            if (MotionLocks.HasValue) def.motionLocks = MotionLocks.Value;
            if (EnableSleep.HasValue) def.enableSleep = EnableSleep.Value;
            if (IsAwake.HasValue) def.isAwake = IsAwake.Value;
            if (IsBullet.HasValue) def.isBullet = IsBullet.Value;
            if (IsEnabled.HasValue) def.isEnabled = IsEnabled.Value;
            if (AllowFastRotation.HasValue) def.allowFastRotation = AllowFastRotation.Value;
        }
    }
}
