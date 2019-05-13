using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;

namespace Game_Kinect_Project.PhysicObjects
{
    public class CollisonBox : DrawableGameComponent
    {
        protected Body body;
        protected CollisionSkin collision;

        protected Model model;
        protected Vector3 color;

        protected Vector3 scale = Vector3.One;

        public Body PhysicsBody{get { return body; }}
        public CollisionSkin PhysicsSkin{ get { return collision; }}

        protected static Random random = new Random();

        public CollisonBox(Game game, Matrix orientation, Vector3 position)
            : base(game)
        {
            this.model = null;
            color = new Vector3(random.Next(255), random.Next(255), random.Next(255));
            color /= 255.0f;
            body = new Body();
            collision = new CollisionSkin(body);
            // add a small box at the buttom
            Primitive box = new Box(new Vector3(-0.1f, -0.1f, -0.1f), Matrix.Identity, Vector3.One * 1.5f);
            collision.AddPrimitive(box, new MaterialProperties(0.1f, 0.5f, 0.5f));
            body.CollisionSkin = this.collision;
            Vector3 com = SetMass(0.5f);

            body.MoveTo(position, orientation);
            collision.ApplyLocalTransform(new Transform(-com, Matrix.Identity));

            body.EnableBody();
            
            
        }

        protected Vector3 SetMass(float mass)
        {
            PrimitiveProperties primitiveProperties =
                new PrimitiveProperties(PrimitiveProperties.MassDistributionEnum.Solid, PrimitiveProperties.MassTypeEnum.Density, mass);

            float junk; Vector3 com; Matrix it, itCoM;

            collision.GetMassProperties(primitiveProperties, out junk, out com, out it, out itCoM);
            body.BodyInertia = itCoM;
            body.Mass = junk;

            return com;
        }

        public override void Draw(GameTime gameTime)
        {
            Camera camera = ((Game1)this.Game).Camera;
            if (((Game1)this.Game).DebugDrawer.Enabled)
            {
                wf = collision.GetLocalSkinWireframe();

                // if the collision skin was also added to the body
                // we have to transform the skin wireframe to the body space
                if (body.CollisionSkin != null)
                {
                    body.TransformWireframe(wf);
                }

                ((Game1)this.Game).DebugDrawer.DrawShape(wf);
            }        
        }
        VertexPositionColor[] wf;
    }
}
