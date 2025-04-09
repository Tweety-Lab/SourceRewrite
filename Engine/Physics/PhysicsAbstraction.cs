using SourceRewrite.Entities;
using SourceRewrite.Windowing.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.PhysicsSystem
{
    public static class Physics
    {
        public static PhysicsContext Context => GameModules.GetModule<PhysicsModule>().Context;

        public static void InitPhysicsEntity(PointEntity physEntity, PhysicsBody body)
        {
            Context.APIInterface.InitPhysicsEntity(physEntity, body);
        }

        public static void SetEntityAbsVelocity(PointEntity entity, Vector3 velocity)
        {
            Context.APIInterface.SetEntityAbsVelocity(entity, velocity);
        }

        public static BaseEntity RayCast(PhysicsRay ray)
        {
            return Context.APIInterface.RayCast(ray);
        }

        public static void DestroyPhysicsEntity(PointEntity physEntity)
        {
            Context.APIInterface.DestroyPhysicsEntity(physEntity);
        }
    }
}
