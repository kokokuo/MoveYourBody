using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using JiggleGame;

namespace JiggleGame
{
    public class SkyBox : DrawableGameComponent
    {
        private Model skyboxMesh;
        public Vector3 myPosition;
        public Quaternion myRotation;
        public Vector3 myScale;

        public TextureCube environ;

        Effect shader;

        Camera camera;

        string modelAsset;
        string shaderAsset;
        string textureAsset;

        public SkyBox(Game game, string modelAsset, string shaderAsset, string textureAsset, Camera c)
            : base(game)
        {

            this.modelAsset = modelAsset;
            this.shaderAsset = shaderAsset;
            this.textureAsset = textureAsset;

            myPosition = new Vector3(0, 0, 0);
            myRotation = new Quaternion(0, 0, 0, 1);
            myScale = new Vector3(55, 55, 55);

            camera = c;
        }

        protected override void LoadContent()
        {
            skyboxMesh = Game.Content.Load<Model>(modelAsset);
            shader = Game.Content.Load<Effect>(shaderAsset);
            environ = Game.Content.Load<TextureCube>(textureAsset);

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            
            Matrix World = Matrix.CreateScale(myScale) *
                            Matrix.CreateFromQuaternion(myRotation) *
                            Matrix.CreateTranslation(camera.Position);

            shader.Parameters["World"].SetValue(World);
            shader.Parameters["View"].SetValue(camera.View);
            shader.Parameters["Projection"].SetValue(camera.Projection);
            shader.Parameters["surfaceTexture"].SetValue(environ);

            shader.Parameters["EyePosition"].SetValue(camera.Position);

            for (int pass = 0; pass < shader.CurrentTechnique.Passes.Count; pass++)
            {
                for (int msh = 0; msh < skyboxMesh.Meshes.Count; msh++)
                {
                    ModelMesh mesh = skyboxMesh.Meshes[msh];
                    for (int prt = 0; prt < mesh.MeshParts.Count; prt++)
                        mesh.MeshParts[prt].Effect = shader;
                    mesh.Draw();
                }
            }

            base.Draw(gameTime);
        }
    }
}
