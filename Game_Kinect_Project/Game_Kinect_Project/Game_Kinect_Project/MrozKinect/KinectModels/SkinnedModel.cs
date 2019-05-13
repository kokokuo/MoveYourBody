using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkinnedModel;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MrozKinect.KinectModels
{
    public class SkinnedModel
    {
        internal Model model;
        internal AnimationPlayer animationPlayer;
        internal SkinningData skinningData;

        public SkinnedModel(Game game, string modelName)
        {
            model = game.Content.Load<Model>(modelName);
            skinningData = model.Tag as SkinningData;

            if (skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");

            
            animationPlayer = new AnimationPlayer(skinningData);
        }

        public virtual void Draw(Matrix world, Matrix projection, Matrix view, GameTime gameTime)
        {
            Matrix[] bones = animationPlayer.GetSkinTransforms();
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);
                    effect.World = world;
                    //effect.World = Matrix.CreateScale(modelScale) * Matrix.CreateTranslation(0, -1000, 3000);

                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                    //effect.Alpha = .3f;
                }

                mesh.Draw();
            }            
        }
    }
}
