using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;

namespace Game_Kinect_Project.PhysicObjects
{

    /// <summary>
    /// Helps to combine the physics with the graphics.
    /// </summary>
    public abstract class PhysicObject : DrawableGameComponent
    {

        protected Body body;
        protected CollisionSkin collision;

        protected Model model;
        protected Vector3 color;

        protected Vector3 scale = Vector3.One;

        public Body PhysicsBody{get { return body; }}
        public CollisionSkin PhysicsSkin{ get { return collision; }}

        protected static Random random = new Random();

        public bool drawing = true;

        public PhysicObject(Game game,Model model) : base(game)
        {
            this.model = model;
            color = new Vector3(random.Next(255), random.Next(255), random.Next(255));
            color /= 255.0f;
            
        }

        public PhysicObject(Game game)
            : base(game)
        {
            this.model = null;
            color = new Vector3(random.Next(255), random.Next(255), random.Next(255));
            color /= 255.0f;
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
        Matrix[] boneTransforms = null;
        int boneCount = 0;

        public void FrogEanble(bool boolean) 
        {
            isFrog = boolean;
        }
        public bool isFrog = false;


        public bool isCameraView = true;

        public abstract void ApplyEffects(BasicEffect effect);
        public override void Draw(GameTime gameTime)
        {
        //    if (drawing)
        //    {
        //        if (model != null)
        //        {
        //            if (boneTransforms == null || boneCount != model.Bones.Count)
        //            {
        //                boneTransforms = new Matrix[model.Bones.Count];
        //                boneCount = model.Bones.Count;
        //            }

        //            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

        //            Camera camera = ((Game1)this.Game).Camera;
        //            foreach (ModelMesh mesh in model.Meshes)
        //            {
        //                foreach (BasicEffect effect in mesh.Effects)
        //                {

        //                    // the body has an orientation but also the primitives in the collision skin
        //                    // owned by the body can be rotated!
        //                    if (body.CollisionSkin != null)
        //                        effect.World = boneTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(scale) * body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation * body.Orientation * Matrix.CreateTranslation(body.Position);
        //                    else
        //                        effect.World = boneTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(scale) * body.Orientation * Matrix.CreateTranslation(body.Position);

        //                    effect.View = camera.View;
        //                    effect.Projection = camera.Projection;

        //                    ApplyEffects(effect);

        //                    effect.EnableDefaultLighting();
        //                    effect.PreferPerPixelLighting = true;
        //                    effect.LightingEnabled = true;
        //                    //effect.DiffuseColor = new Vector3(1f);
        //                    effect.AmbientLightColor = new Vector3(0.5f);

        //                    effect.DirectionalLight0.Enabled = true;
        //                    effect.DirectionalLight0.DiffuseColor = Vector3.One;
        //                    effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-1, -0.1f, 0.3f));

        //                    if (isFrog)
        //                    {
        //                        effect.FogEnabled = true;
        //                        effect.FogStart = 50;
        //                        effect.FogEnd = 100;
        //                        effect.FogColor = Color.White.ToVector3();
        //                    }
        //                }
        //                mesh.Draw();
        //            }

        //        }
        //    }
        //    if (((Game1)this.Game).DebugDrawer.Enabled)
        //    {

        //        wf = collision.GetLocalSkinWireframe();

        //        // if the collision skin was also added to the body
        //        // we have to transform the skin wireframe to the body space
        //        if (body.CollisionSkin != null)
        //        {
        //            body.TransformWireframe(wf);
        //        }

        //        ((Game1)this.Game).DebugDrawer.DrawShape(wf);
        //    }


        //    //base.Draw(gameTime);
        }

        public Matrix view, projection;

        public VertexPositionColor[] wf;

        public void draww()
        {
            if (drawing)
            {
                if (model != null)
                {
                    if (boneTransforms == null || boneCount != model.Bones.Count)
                    {
                        boneTransforms = new Matrix[model.Bones.Count];
                        boneCount = model.Bones.Count;
                    }

                    model.CopyAbsoluteBoneTransformsTo(boneTransforms);

                    Camera camera = ((Game1)this.Game).Camera;
                    foreach (ModelMesh mesh in model.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {

                            // the body has an orientation but also the primitives in the collision skin
                            // owned by the body can be rotated!
                            if (body.CollisionSkin != null)
                                effect.World = boneTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(scale) * body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation * body.Orientation * Matrix.CreateTranslation(body.Position);
                            else
                                effect.World = boneTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(scale) * body.Orientation * Matrix.CreateTranslation(body.Position);

                            if (isCameraView)
                            {
                                effect.View = camera.View;
                                effect.Projection = camera.Projection;
                            }
                            else 
                            {
                                effect.View =view;
                                effect.Projection = projection;
                            }

                            ApplyEffects(effect);

                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;
                            effect.LightingEnabled = true;
                            //effect.DiffuseColor = new Vector3(1f);
                            effect.AmbientLightColor = new Vector3(0.5f);

                            effect.DirectionalLight0.Enabled = true;
                            effect.DirectionalLight0.DiffuseColor = Vector3.One;
                            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-1, -0.1f, 0.3f));

                            if (isFrog)
                            {
                                effect.FogEnabled = true;
                                effect.FogStart = 50;
                                effect.FogEnd = 100;
                                effect.FogColor = Color.White.ToVector3();
                            }
                        }
                        mesh.Draw();
                    }

                }
            }
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

    }
}
