using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game_Kinect_Project;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using JigLibX.Collision;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Geometry;
using JigLibX.Math;
using Microsoft.Xna.Framework.Input;

namespace Game_Kinect_Project.PhysicObjects
{
    class CharacterObject : PhysicObject
    {

        public Character CharacterBody {get; set;}

        public CharacterObject(Game game,Vector3 position)
            : base(game,null)
        {
            body = new Character();
            collision = new CollisionSkin(body);
            body.Mass = 20000000000;
            Capsule capsule = new Capsule(Vector3.Zero, Matrix.CreateRotationX(MathHelper.PiOver2), 1.0f, 1.0f);
            collision.AddPrimitive(capsule, (int)MaterialTable.MaterialID.NotBouncyNormal);
            body.CollisionSkin = this.collision;
            Vector3 com = SetMass(200000.0f);          
            body.MoveTo(position + com, Matrix.Identity);
            collision.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            
            body.SetBodyInvInertia(0.0f, 0.0f, 0.0f);          
            CharacterBody = body as Character;
            
            body.AllowFreezing = false;
            body.EnableBody();          
        }

        public override void ApplyEffects(BasicEffect effect)
        {
            //throw new NotImplementedException();
        }
    }

    class ASkinPredicate : CollisionSkinPredicate1
    {
        public override bool ConsiderSkin(CollisionSkin skin0)
        {
            if (!(skin0.Owner is Character))
                return true;
            else
                return false;
        }
    }

    class Character : Body
    {

        public Character() : base()
        {
        }

        public Vector3 DesiredVelocity { get; set; }

        private bool doJump = false;

        public void DoJump()
        {
            doJump = true;
        }


        public override void AddExternalForces(float dt)
        {
            ClearForces();

            if (doJump)
            {
                foreach (CollisionInfo info in CollisionSkin.Collisions)
                {
                    Vector3 N = info.DirToBody0;
                    if (this == info.SkinInfo.Skin1.Owner)
                        Vector3.Negate(ref N, out N);

                    if (Vector3.Dot(N, Orientation.Up) > 0.7f)
                    {
                        Vector3 vel = Velocity; vel.Y = 40.0f;
                        Velocity = vel;
                        break;
                    }
                }
            }

            Vector3 deltaVel = DesiredVelocity - Velocity;

            bool running = true;

            if (DesiredVelocity.LengthSquared() < JiggleMath.Epsilon) running = false;
            else deltaVel.Normalize();

            deltaVel.Y = 0.0f;

            // start fast, slow down slower
            if (running) deltaVel *= 10.0f;
            else deltaVel *= 2.0f;

            float forceFactor = 1000.0f;

            AddBodyForce(deltaVel * Mass * dt * forceFactor);

            doJump = false;
            AddGravityToExternalForce();
        }



    }

}
