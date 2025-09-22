using Box2dNet.Interop;
using System.Diagnostics;
using System.Numerics;

namespace Atlantis.Box2dNet;

/// <summary>
/// Extension methods for B2Api
/// </summary>

// b2WorldId
public static partial class B2Extension
{
    [DebuggerHidden]
    public static bool IsValid(this b2WorldId worldId)
    {
        return B2Api.b2World_IsValid(worldId);
    }
    [DebuggerHidden]
    public static void Step(this b2WorldId worldId, float timeStep, int subStepCount)
    {
        B2Api.b2World_Step(worldId, timeStep, subStepCount);
    }
    [DebuggerHidden]
    public static b2BodyEvents GetBodyEvents(this b2WorldId worldId)
    {
        return B2Api.b2World_GetBodyEvents(worldId);
    }
    [DebuggerHidden]
    public static b2SensorEvents GetSensorEvents(this b2WorldId worldId)
    {
        return B2Api.b2World_GetSensorEvents(worldId);
    }
    [DebuggerHidden]
    public static b2ContactEvents GetContactEvents(this b2WorldId worldId)
    {
        return B2Api.b2World_GetContactEvents(worldId);
    }
    [DebuggerHidden]
    public static b2JointEvents GetJointEvents(this b2WorldId worldId)
    {
        return B2Api.b2World_GetJointEvents(worldId);
    }
    [DebuggerHidden]
    public static b2TreeStats OverlapAABB(this b2WorldId worldId, b2AABB aabb, b2QueryFilter filter, IntPtr fcn, IntPtr context)
    {
        return B2Api.b2World_OverlapAABB(worldId, aabb, filter, fcn, context);
    }
    [DebuggerHidden]
    public static b2TreeStats OverlapAABB(this b2WorldId worldId, b2AABB aabb, b2QueryFilter filter, b2OverlapResultFcn fcn, IntPtr context)
    {
        return B2Api.b2World_OverlapAABB(worldId, aabb, filter, fcn, context);
    }
    [DebuggerHidden]
    public static b2TreeStats OverlapShape(this b2WorldId worldId, in b2ShapeProxy proxy, b2QueryFilter filter, IntPtr fcn, IntPtr context)
    {
        return B2Api.b2World_OverlapShape(worldId, proxy, filter, fcn, context);
    }
    [DebuggerHidden]
    public static b2TreeStats OverlapShape(this b2WorldId worldId, in b2ShapeProxy proxy, b2QueryFilter filter, b2OverlapResultFcn fcn, IntPtr context)
    {
        return B2Api.b2World_OverlapShape(worldId, proxy, filter, fcn, context);
    }
    [DebuggerHidden]
    public static b2TreeStats CastRay(this b2WorldId worldId, Vector2 origin, Vector2 translation, b2QueryFilter filter, IntPtr fcn, IntPtr context)
    {
        return B2Api.b2World_CastRay(worldId, origin, translation, filter, fcn, context);
    }
    [DebuggerHidden]
    public static b2TreeStats CastRay(this b2WorldId worldId, Vector2 origin, Vector2 translation, b2QueryFilter filter, b2CastResultFcn fcn, IntPtr context)
    {
        return B2Api.b2World_CastRay(worldId, origin, translation, filter, fcn, context);
    }
    [DebuggerHidden]
    public static b2RayResult CastRayClosest(this b2WorldId worldId, Vector2 origin, Vector2 translation, b2QueryFilter filter)
    {
        return B2Api.b2World_CastRayClosest(worldId, origin, translation, filter);
    }
    [DebuggerHidden]
    public static b2TreeStats CastShape(this b2WorldId worldId, in b2ShapeProxy proxy, Vector2 translation, b2QueryFilter filter, IntPtr fcn, IntPtr context)
    {
        return B2Api.b2World_CastShape(worldId, proxy, translation, filter, fcn, context);
    }
    [DebuggerHidden]
    public static b2TreeStats CastShape(this b2WorldId worldId, in b2ShapeProxy proxy, Vector2 translation, b2QueryFilter filter, b2CastResultFcn fcn, IntPtr context)
    {
        return B2Api.b2World_CastShape(worldId, proxy, translation, filter, fcn, context);
    }
    [DebuggerHidden]
    public static float CastMover(this b2WorldId worldId, in b2Capsule mover, Vector2 translation, b2QueryFilter filter)
    {
        return B2Api.b2World_CastMover(worldId, mover, translation, filter);
    }
    [DebuggerHidden]
    public static void CollideMover(this b2WorldId worldId, in b2Capsule mover, b2QueryFilter filter, IntPtr fcn, IntPtr context)
    {
        B2Api.b2World_CollideMover(worldId, mover, filter, fcn, context);
    }
    [DebuggerHidden]
    public static void CollideMover(this b2WorldId worldId, in b2Capsule mover, b2QueryFilter filter, b2PlaneResultFcn fcn, IntPtr context)
    {
        B2Api.b2World_CollideMover(worldId, mover, filter, fcn, context);
    }
    [DebuggerHidden]
    public static void EnableSleeping(this b2WorldId worldId, bool flag)
    {
        B2Api.b2World_EnableSleeping(worldId, flag);
    }
    [DebuggerHidden]
    public static bool IsSleepingEnabled(this b2WorldId worldId)
    {
        return B2Api.b2World_IsSleepingEnabled(worldId);
    }
    [DebuggerHidden]
    public static void EnableContinuous(this b2WorldId worldId, bool flag)
    {
        B2Api.b2World_EnableContinuous(worldId, flag);
    }
    [DebuggerHidden]
    public static bool IsContinuousEnabled(this b2WorldId worldId)
    {
        return B2Api.b2World_IsContinuousEnabled(worldId);
    }
    [DebuggerHidden]
    public static void SetRestitutionThreshold(this b2WorldId worldId, float value)
    {
        B2Api.b2World_SetRestitutionThreshold(worldId, value);
    }
    [DebuggerHidden]
    public static float GetRestitutionThreshold(this b2WorldId worldId)
    {
        return B2Api.b2World_GetRestitutionThreshold(worldId);
    }
    [DebuggerHidden]
    public static void SetHitEventThreshold(this b2WorldId worldId, float value)
    {
        B2Api.b2World_SetHitEventThreshold(worldId, value);
    }
    [DebuggerHidden]
    public static float GetHitEventThreshold(this b2WorldId worldId)
    {
        return B2Api.b2World_GetHitEventThreshold(worldId);
    }
    [DebuggerHidden]
    public static void SetGravity(this b2WorldId worldId, Vector2 gravity)
    {
        B2Api.b2World_SetGravity(worldId, gravity);
    }
    [DebuggerHidden]
    public static Vector2 GetGravity(this b2WorldId worldId)
    {
        return B2Api.b2World_GetGravity(worldId);
    }
}

// b2BodyId
public static partial class B2Extension
{
    

    [DebuggerHidden]
    public static bool IsValid(this b2BodyId bodyId)
    {
        return B2Api.b2Body_IsValid(bodyId);
    }

    [DebuggerHidden]
    public static b2BodyType GetType(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetType(bodyId);
    }

    [DebuggerHidden]
    public static void SetType(this b2BodyId bodyId, b2BodyType type)
    {
        B2Api.b2Body_SetType(bodyId, type);
    }

    [DebuggerHidden]
    public static void SetName(this b2BodyId bodyId, string name)
    {
        B2Api.b2Body_SetName(bodyId, name);
    }

    [DebuggerHidden]
    public static void SetUserData(this b2BodyId bodyId, IntPtr userData)
    {
        B2Api.b2Body_SetUserData(bodyId, userData);
    }

    [DebuggerHidden]
    public static IntPtr GetUserData(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetUserData(bodyId);
    }

    [DebuggerHidden]
    public static Vector2 GetPosition(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetPosition(bodyId);
    }

    [DebuggerHidden]
    public static b2Rot GetRotation(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetRotation(bodyId);
    }

    [DebuggerHidden]
    public static b2Transform GetTransform(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetTransform(bodyId);
    }

    [DebuggerHidden]
    public static void SetTransform(this b2BodyId bodyId, Vector2 position, b2Rot rotation)
    {
        B2Api.b2Body_SetTransform(bodyId, position, rotation);
    }

    [DebuggerHidden]
    public static Vector2 GetLocalPoint(this b2BodyId bodyId, Vector2 worldPoint)
    {
        return B2Api.b2Body_GetLocalPoint(bodyId, worldPoint);
    }

    [DebuggerHidden]
    public static Vector2 GetWorldPoint(this b2BodyId bodyId, Vector2 localPoint)
    {
        return B2Api.b2Body_GetWorldPoint(bodyId, localPoint);
    }

    [DebuggerHidden]
    public static Vector2 GetLocalVector(this b2BodyId bodyId, Vector2 worldVector)
    {
        return B2Api.b2Body_GetLocalVector(bodyId, worldVector);
    }

    [DebuggerHidden]
    public static Vector2 GetWorldVector(this b2BodyId bodyId, Vector2 localVector)
    {
        return B2Api.b2Body_GetWorldVector(bodyId, localVector);
    }

    [DebuggerHidden]
    public static Vector2 GetLinearVelocity(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetLinearVelocity(bodyId);
    }

    [DebuggerHidden]
    public static float GetAngularVelocity(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetAngularVelocity(bodyId);
    }

    [DebuggerHidden]
    public static void SetLinearVelocity(this b2BodyId bodyId, Vector2 linearVelocity)
    {
        B2Api.b2Body_SetLinearVelocity(bodyId, linearVelocity);
    }

    [DebuggerHidden]
    public static void SetAngularVelocity(this b2BodyId bodyId, float angularVelocity)
    {
        B2Api.b2Body_SetAngularVelocity(bodyId, angularVelocity);
    }

    [DebuggerHidden]
    public static void SetTargetTransform(this b2BodyId bodyId, b2Transform target, float timeStep)
    {
        B2Api.b2Body_SetTargetTransform(bodyId, target, timeStep);
    }

    [DebuggerHidden]
    public static Vector2 GetLocalPointVelocity(this b2BodyId bodyId, Vector2 localPoint)
    {
        return B2Api.b2Body_GetLocalPointVelocity(bodyId, localPoint);
    }

    [DebuggerHidden]
    public static Vector2 GetWorldPointVelocity(this b2BodyId bodyId, Vector2 worldPoint)
    {
        return B2Api.b2Body_GetWorldPointVelocity(bodyId, worldPoint);
    }

    [DebuggerHidden]
    public static void ApplyForce(this b2BodyId bodyId, Vector2 force, Vector2 point, bool wake)
    {
        B2Api.b2Body_ApplyForce(bodyId, force, point, wake);
    }

    [DebuggerHidden]
    public static void ApplyForceToCenter(this b2BodyId bodyId, Vector2 force, bool wake)
    {
        B2Api.b2Body_ApplyForceToCenter(bodyId, force, wake);
    }

    [DebuggerHidden]
    public static void ApplyTorque(this b2BodyId bodyId, float torque, bool wake)
    {
        B2Api.b2Body_ApplyTorque(bodyId, torque, wake);
    }

    [DebuggerHidden]
    public static void ApplyLinearImpulse(this b2BodyId bodyId, Vector2 impulse, Vector2 point, bool wake)
    {
        B2Api.b2Body_ApplyLinearImpulse(bodyId, impulse, point, wake);
    }

    [DebuggerHidden]
    public static void ApplyLinearImpulseToCenter(this b2BodyId bodyId, Vector2 impulse, bool wake)
    {
        B2Api.b2Body_ApplyLinearImpulseToCenter(bodyId, impulse, wake);
    }

    [DebuggerHidden]
    public static void ApplyAngularImpulse(this b2BodyId bodyId, float impulse, bool wake)
    {
        B2Api.b2Body_ApplyAngularImpulse(bodyId, impulse, wake);
    }

    [DebuggerHidden]
    public static float GetMass(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetMass(bodyId);
    }

    [DebuggerHidden]
    public static float GetRotationalInertia(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetRotationalInertia(bodyId);
    }

    [DebuggerHidden]
    public static Vector2 GetLocalCenterOfMass(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetLocalCenterOfMass(bodyId);
    }

    [DebuggerHidden]
    public static Vector2 GetWorldCenterOfMass(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetWorldCenterOfMass(bodyId);
    }

    [DebuggerHidden]
    public static void SetMassData(this b2BodyId bodyId, b2MassData massData)
    {
        B2Api.b2Body_SetMassData(bodyId, massData);
    }

    [DebuggerHidden]
    public static b2MassData GetMassData(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetMassData(bodyId);
    }

    [DebuggerHidden]
    public static void ApplyMassFromShapes(this b2BodyId bodyId)
    {
        B2Api.b2Body_ApplyMassFromShapes(bodyId);
    }

    [DebuggerHidden]
    public static void SetLinearDamping(this b2BodyId bodyId, float linearDamping)
    {
        B2Api.b2Body_SetLinearDamping(bodyId, linearDamping);
    }

    [DebuggerHidden]
    public static float GetLinearDamping(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetLinearDamping(bodyId);
    }

    [DebuggerHidden]
    public static void SetAngularDamping(this b2BodyId bodyId, float angularDamping)
    {
        B2Api.b2Body_SetAngularDamping(bodyId, angularDamping);
    }

    [DebuggerHidden]
    public static float GetAngularDamping(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetAngularDamping(bodyId);
    }

    [DebuggerHidden]
    public static void SetGravityScale(this b2BodyId bodyId, float gravityScale)
    {
        B2Api.b2Body_SetGravityScale(bodyId, gravityScale);
    }

    [DebuggerHidden]
    public static float GetGravityScale(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetGravityScale(bodyId);
    }

    [DebuggerHidden]
    public static bool IsAwake(this b2BodyId bodyId)
    {
        return B2Api.b2Body_IsAwake(bodyId);
    }

    [DebuggerHidden]
    public static void SetAwake(this b2BodyId bodyId, bool awake)
    {
        B2Api.b2Body_SetAwake(bodyId, awake);
    }

    [DebuggerHidden]
    public static void EnableSleep(this b2BodyId bodyId, bool enableSleep)
    {
        B2Api.b2Body_EnableSleep(bodyId, enableSleep);
    }

    [DebuggerHidden]
    public static bool IsSleepEnabled(this b2BodyId bodyId)
    {
        return B2Api.b2Body_IsSleepEnabled(bodyId);
    }

    [DebuggerHidden]
    public static void SetSleepThreshold(this b2BodyId bodyId, float sleepThreshold)
    {
        B2Api.b2Body_SetSleepThreshold(bodyId, sleepThreshold);
    }

    [DebuggerHidden]
    public static float GetSleepThreshold(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetSleepThreshold(bodyId);
    }

    [DebuggerHidden]
    public static bool IsEnabled(this b2BodyId bodyId)
    {
        return B2Api.b2Body_IsEnabled(bodyId);
    }

    [DebuggerHidden]
    public static void Disable(this b2BodyId bodyId)
    {
        B2Api.b2Body_Disable(bodyId);
    }

    [DebuggerHidden]
    public static void Enable(this b2BodyId bodyId)
    {
        B2Api.b2Body_Enable(bodyId);
    }

    [DebuggerHidden]
    public static void SetMotionLocks(this b2BodyId bodyId, b2MotionLocks locks)
    {
        B2Api.b2Body_SetMotionLocks(bodyId, locks);
    }

    [DebuggerHidden]
    public static b2MotionLocks GetMotionLocks(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetMotionLocks(bodyId);
    }

    [DebuggerHidden]
    public static void SetBullet(this b2BodyId bodyId, bool flag)
    {
        B2Api.b2Body_SetBullet(bodyId, flag);
    }

    [DebuggerHidden]
    public static bool IsBullet(this b2BodyId bodyId)
    {
        return B2Api.b2Body_IsBullet(bodyId);
    }

    [DebuggerHidden]
    public static void EnableContactEvents(this b2BodyId bodyId, bool flag)
    {
        B2Api.b2Body_EnableContactEvents(bodyId, flag);
    }

    [DebuggerHidden]
    public static void EnableHitEvents(this b2BodyId bodyId, bool flag)
    {
        B2Api.b2Body_EnableHitEvents(bodyId, flag);
    }

    [DebuggerHidden]
    public static b2WorldId GetWorld(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetWorld(bodyId);
    }

    [DebuggerHidden]
    public static int GetShapeCount(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetShapeCount(bodyId);
    }

    [DebuggerHidden]
    public static int GetShapes(this b2BodyId bodyId, b2ShapeId[] shapeArray, int capacity)
    {
        return B2Api.b2Body_GetShapes(bodyId, shapeArray, capacity);
    }

    [DebuggerHidden]
    public static int GetJointCount(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetJointCount(bodyId);
    }

    [DebuggerHidden]
    public static int GetJoints(this b2BodyId bodyId, b2JointId[] jointArray, int capacity)
    {
        return B2Api.b2Body_GetJoints(bodyId, jointArray, capacity);
    }

    [DebuggerHidden]
    public static int GetContactCapacity(this b2BodyId bodyId)
    {
        return B2Api.b2Body_GetContactCapacity(bodyId);
    }

    [DebuggerHidden]
    public static int GetContactData(this b2BodyId bodyId, b2ContactData[] contactData, int capacity)
    {
        return B2Api.b2Body_GetContactData(bodyId, contactData, capacity);
    }

    [DebuggerHidden]
    public static b2AABB ComputeAABB(this b2BodyId bodyId)
    {
        return B2Api.b2Body_ComputeAABB(bodyId);
    }
}

// b2ShapeId
public static partial class B2Extension
{
    

    [DebuggerHidden]
    public static bool IsValid(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_IsValid(shapeId);
    }

    [DebuggerHidden]
    public static b2ShapeType GetType(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetType(shapeId);
    }

    [DebuggerHidden]
    public static b2BodyId GetBody(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetBody(shapeId);
    }

    [DebuggerHidden]
    public static b2WorldId GetWorld(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetWorld(shapeId);
    }

    [DebuggerHidden]
    public static bool IsSensor(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_IsSensor(shapeId);
    }

    [DebuggerHidden]
    public static void SetUserData(this b2ShapeId shapeId, IntPtr userData)
    {
        B2Api.b2Shape_SetUserData(shapeId, userData);
    }

    [DebuggerHidden]
    public static IntPtr GetUserData(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetUserData(shapeId);
    }

    [DebuggerHidden]
    public static void SetDensity(this b2ShapeId shapeId, float density, bool updateBodyMass)
    {
        B2Api.b2Shape_SetDensity(shapeId, density, updateBodyMass);
    }

    [DebuggerHidden]
    public static float GetDensity(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetDensity(shapeId);
    }

    [DebuggerHidden]
    public static void SetFriction(this b2ShapeId shapeId, float friction)
    {
        B2Api.b2Shape_SetFriction(shapeId, friction);
    }

    [DebuggerHidden]
    public static float GetFriction(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetFriction(shapeId);
    }

    [DebuggerHidden]
    public static void SetRestitution(this b2ShapeId shapeId, float restitution)
    {
        B2Api.b2Shape_SetRestitution(shapeId, restitution);
    }

    [DebuggerHidden]
    public static float GetRestitution(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetRestitution(shapeId);
    }

    [DebuggerHidden]
    public static void SetMaterial(this b2ShapeId shapeId, int material)
    {
        B2Api.b2Shape_SetMaterial(shapeId, material);
    }

    [DebuggerHidden]
    public static int GetMaterial(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetMaterial(shapeId);
    }

    [DebuggerHidden]
    public static void SetSurfaceMaterial(this b2ShapeId shapeId, b2SurfaceMaterial surfaceMaterial)
    {
        B2Api.b2Shape_SetSurfaceMaterial(shapeId, surfaceMaterial);
    }

    [DebuggerHidden]
    public static b2SurfaceMaterial GetSurfaceMaterial(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetSurfaceMaterial(shapeId);
    }

    [DebuggerHidden]
    public static b2Filter GetFilter(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetFilter(shapeId);
    }

    [DebuggerHidden]
    public static void SetFilter(this b2ShapeId shapeId, b2Filter filter)
    {
        B2Api.b2Shape_SetFilter(shapeId, filter);
    }

    [DebuggerHidden]
    public static void EnableSensorEvents(this b2ShapeId shapeId, bool flag)
    {
        B2Api.b2Shape_EnableSensorEvents(shapeId, flag);
    }

    [DebuggerHidden]
    public static bool AreSensorEventsEnabled(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_AreSensorEventsEnabled(shapeId);
    }

    [DebuggerHidden]
    public static void EnableContactEvents(this b2ShapeId shapeId, bool flag)
    {
        B2Api.b2Shape_EnableContactEvents(shapeId, flag);
    }

    [DebuggerHidden]
    public static bool AreContactEventsEnabled(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_AreContactEventsEnabled(shapeId);
    }

    [DebuggerHidden]
    public static void EnablePreSolveEvents(this b2ShapeId shapeId, bool flag)
    {
        B2Api.b2Shape_EnablePreSolveEvents(shapeId, flag);
    }

    [DebuggerHidden]
    public static bool ArePreSolveEventsEnabled(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_ArePreSolveEventsEnabled(shapeId);
    }

    [DebuggerHidden]
    public static void EnableHitEvents(this b2ShapeId shapeId, bool flag)
    {
        B2Api.b2Shape_EnableHitEvents(shapeId, flag);
    }

    [DebuggerHidden]
    public static bool AreHitEventsEnabled(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_AreHitEventsEnabled(shapeId);
    }

    [DebuggerHidden]
    public static bool TestPoint(this b2ShapeId shapeId, Vector2 point)
    {
        return B2Api.b2Shape_TestPoint(shapeId, point);
    }

    [DebuggerHidden]
    public static b2CastOutput RayCast(this b2ShapeId shapeId, in b2RayCastInput input)
    {
        return B2Api.b2Shape_RayCast(shapeId, input);
    }

    [DebuggerHidden]
    public static b2Circle GetCircle(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetCircle(shapeId);
    }

    [DebuggerHidden]
    public static b2Segment GetSegment(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetSegment(shapeId);
    }

    [DebuggerHidden]
    public static b2ChainSegment GetChainSegment(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetChainSegment(shapeId);
    }

    [DebuggerHidden]
    public static b2Capsule GetCapsule(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetCapsule(shapeId);
    }

    [DebuggerHidden]
    public static b2Polygon GetPolygon(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetPolygon(shapeId);
    }

    [DebuggerHidden]
    public static void SetCircle(this b2ShapeId shapeId, in b2Circle circle)
    {
        B2Api.b2Shape_SetCircle(shapeId, circle);
    }

    [DebuggerHidden]
    public static void SetCapsule(this b2ShapeId shapeId, in b2Capsule capsule)
    {
        B2Api.b2Shape_SetCapsule(shapeId, capsule);
    }

    [DebuggerHidden]
    public static void SetSegment(this b2ShapeId shapeId, in b2Segment segment)
    {
        B2Api.b2Shape_SetSegment(shapeId, segment);
    }

    [DebuggerHidden]
    public static void SetPolygon(this b2ShapeId shapeId, in b2Polygon polygon)
    {
        B2Api.b2Shape_SetPolygon(shapeId, polygon);
    }

    [DebuggerHidden]
    public static b2ChainId GetParentChain(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetParentChain(shapeId);
    }

    [DebuggerHidden]
    public static int GetContactCapacity(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetContactCapacity(shapeId);
    }

    [DebuggerHidden]
    public static int GetContactData(this b2ShapeId shapeId, b2ContactData[] contactData, int capacity)
    {
        return B2Api.b2Shape_GetContactData(shapeId, contactData, capacity);
    }

    [DebuggerHidden]
    public static int GetSensorCapacity(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetSensorCapacity(shapeId);
    }

    [DebuggerHidden]
    public static int GetSensorData(this b2ShapeId shapeId, b2ShapeId[] visitorIds, int capacity)
    {
        return B2Api.b2Shape_GetSensorData(shapeId, visitorIds, capacity);
    }

    [DebuggerHidden]
    public static b2AABB GetAABB(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_GetAABB(shapeId);
    }

    [DebuggerHidden]
    public static b2MassData ComputeMassData(this b2ShapeId shapeId)
    {
        return B2Api.b2Shape_ComputeMassData(shapeId);
    }

    [DebuggerHidden]
    public static Vector2 GetClosestPoint(this b2ShapeId shapeId, Vector2 target)
    {
        return B2Api.b2Shape_GetClosestPoint(shapeId, target);
    }
}
