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

namespace Game_Kinect_Project
{
    public class WaterI : DrawableGameComponent
    {
        public struct VertexMultitextured
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 TextureCoordinate;
            public Vector3 Tangent;
            public Vector3 BiNormal;

            public static int SizeInBytes = (3 + 3 + 2 + 3 + 3) * 4;
            public static VertexDeclaration VertexDeclaration = new VertexDeclaration
             (
                 new VertexElement( 0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0 ),
                 new VertexElement( sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0 ),
                 new VertexElement( sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0 ),
                 new VertexElement( sizeof(float) * 8, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0 ),
                 new VertexElement( sizeof(float) * 11, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0 )
             );

        }

        private VertexBuffer vb;
        private IndexBuffer ib;
        VertexMultitextured[] myVertices;
        private int myHeight = 128;
        private int myWidth = 128;

        private Vector3 myPosition;
        private Vector3 myScale;
        private Quaternion myRotation;

        Effect effect;

        private Vector3 basePosition;

        Camera camera;

        //水的位置
        public Vector3 Position
        {
            get { return basePosition; }
            set { basePosition = value; }
        }

        private string EnvAsset;

        float bumpHeight = 0.5f;
        Vector2 textureScale = new Vector2(4, 4);
        Vector2 bumpSpeed = new Vector2(0, .05f);
        float fresnelBias = .025f;
        float fresnelPower = 1.0f;
        float hdrMultiplier = 1.0f;
        Color deepWaterColor = Color.Black;
        Color shallowWaterColor = Color.SkyBlue;
        Color reflectionColor = Color.White;
        float reflectionAmount = 0.5f;
        float waterAmount = 0f;
        float waveAmplitude = 0.5f;
        float waveFrequency = 0.1f;

        /// <summary>
        /// Height of water bump texture.
        /// Min 0.0 Max 2.0 Default = .5
        /// </summary>
        public float BumpHeight
        {
            get { return bumpHeight; }
            set { bumpHeight = value; }
        }
        /// <summary>
        /// Scale of bump texture.
        /// </summary>
        public Vector2 TextureScale
        {
            get { return textureScale; }
            set { textureScale = value; }
        }
        /// <summary>
        /// Velocity of water flow
        /// </summary>
        public Vector2 BumpSpeed
        {
            get { return bumpSpeed; }
            set { bumpSpeed = value; }
        }
        /// <summary>
        /// Min 0.0 Max 1.0 Default = .025
        /// </summary>
        public float FresnelBias
        {
            get { return fresnelBias; }
            set { fresnelBias = value; }
        }
        /// <summary>
        /// Min 0.0 Max 10.0 Default = 1.0;
        /// </summary>
        public float FresnelPower
        {
            get { return FresnelPower; }
            set { fresnelPower = value; }
        }
        /// <summary>
        /// Min = 0.0 Max = 100 Default = 1.0
        /// </summary>
        public float HDRMultiplier
        {
            get { return hdrMultiplier; }
            set { hdrMultiplier = value; }
        }
        /// <summary>
        /// Color of deep water Default = Black;
        /// </summary>
        public Color DeepWaterColor
        {
            get { return deepWaterColor; }
            set { deepWaterColor = value; }
        }
        /// <summary>
        /// Color of shallow water Default = SkyBlue
        /// </summary>
        public Color ShallowWaterColor
        {
            get { return shallowWaterColor; }
            set { shallowWaterColor = value; }
        }
        /// <summary>
        /// Default = White
        /// </summary>
        public Color ReflectionColor
        {
            get { return reflectionColor; }
            set { reflectionColor = value; }
        }
        /// <summary>
        /// Min = 0.0 Max = 2.0 Default = .5
        /// </summary>
        public float ReflectionAmount
        {
            get { return reflectionAmount; }
            set { reflectionAmount = value; }
        }
        /// <summary>
        /// Amount of water color to use.
        /// Min = 0 Max = 2 Default = 0;
        /// </summary>
        public float WaterAmount
        {
            get { return waterAmount; }
            set { waterAmount = value; }
        }
        /// <summary>
        /// Min = 0.0 Max = 10 Defatult = 0.5
        /// </summary>
        public float WaveAmplitude
        {
            get { return waveAmplitude; }
            set { waveAmplitude = value; }
        }
        /// <summary>
        /// Min = 0 Max = 1 Default .1
        /// </summary>
        public float WaveFrequency
        {
            get { return waveFrequency; }
            set { waveFrequency = value; }
        }

        /// <summary>
        /// Default 128
        /// </summary>
        public int Height
        {
            get { return myHeight; }
            set { myHeight = value; }
        }
        /// <summary>
        /// Default 128
        /// </summary>
        public int Width
        {
            get { return myWidth; }
            set { myWidth = value; }
        }

        public WaterI(Game game, string Environment,Camera c)
            : base(game)
        {
            myWidth = 16;
            myHeight = 16;

            myPosition = new Vector3(0, 0, 0);
            myScale = Vector3.One;
            myRotation = new Quaternion(0, 0, 0, 1);

            EnvAsset = Environment;
            camera = c;
        }

        public void SetDefault()
        {
            bumpHeight = 0.5f;
            textureScale = new Vector2(4, 4);
            bumpSpeed = new Vector2(0, .05f);
            fresnelBias = .025f;
            fresnelPower = 1.0f;
            hdrMultiplier = 1.0f;
            deepWaterColor = Color.Black;
            shallowWaterColor = Color.SkyBlue;
            reflectionColor = Color.White;
            reflectionAmount = 0.5f;
            waterAmount = 0f;
            waveAmplitude = 0.5f;
            waveFrequency = 0.1f;
        }

        protected override void LoadContent()
        {
            effect = Game.Content.Load<Effect>("Water");

            effect.Parameters["tEnvMap"].SetValue(Game.Content.Load<TextureCube>(EnvAsset));
            effect.Parameters["tNormalMap"].SetValue(Game.Content.Load<Texture2D>("waves2"));

            myPosition = new Vector3(basePosition.X - (myWidth / 2), basePosition.Y, basePosition.Z - (myHeight / 2));

            // Vertices
            myVertices = new VertexMultitextured[myWidth * myHeight];

            for (int x = 0; x < myWidth; x++)
                for (int y = 0; y < myHeight; y++)
                {
                    myVertices[x + y * myWidth].Position = new Vector3(y, 0, x);
                    myVertices[x + y * myWidth].Normal = new Vector3(0, -1, 0);
                    myVertices[x + y * myWidth].TextureCoordinate.X = (float)x / 30.0f;
                    myVertices[x + y * myWidth].TextureCoordinate.Y = (float)y / 30.0f;
                }

            // Calc Tangent and Bi Normals.
            for (int x = 0; x < myWidth; x++)
                for (int y = 0; y < myHeight; y++)
                {
                    // Tangent Data.
                    if (x != 0 && x < myWidth - 1)
                        myVertices[x + y * myWidth].Tangent = myVertices[x - 1 + y * myWidth].Position - myVertices[x + 1 + y * myWidth].Position;
                    else
                        if (x == 0)
                            myVertices[x + y * myWidth].Tangent = myVertices[x + y * myWidth].Position - myVertices[x + 1 + y * myWidth].Position;
                        else
                            myVertices[x + y * myWidth].Tangent = myVertices[x - 1 + y * myWidth].Position - myVertices[x + y * myWidth].Position;

                    // Bi Normal Data.
                    if (y != 0 && y < myHeight - 1)
                        myVertices[x + y * myWidth].BiNormal = myVertices[x + (y - 1) * myWidth].Position - myVertices[x + (y + 1) * myWidth].Position;
                    else
                        if (y == 0)
                            myVertices[x + y * myWidth].BiNormal = myVertices[x + y * myWidth].Position - myVertices[x + (y + 1) * myWidth].Position;
                        else
                            myVertices[x + y * myWidth].BiNormal = myVertices[x + (y - 1) * myWidth].Position - myVertices[x + y * myWidth].Position;
                }


            vb = new VertexBuffer(Game.GraphicsDevice, VertexMultitextured.VertexDeclaration, VertexMultitextured.SizeInBytes * myWidth * myHeight, BufferUsage.WriteOnly);
            vb.SetData(myVertices);

            short[] terrainIndices = new short[(myWidth - 1) * (myHeight - 1) * 6];
            for (short x = 0; x < myWidth - 1; x++)
            {
                for (short y = 0; y < myHeight - 1; y++)
                {
                    terrainIndices[(x + y * (myWidth - 1)) * 6] = (short)((x + 1) + (y + 1) * myWidth);
                    terrainIndices[(x + y * (myWidth - 1)) * 6 + 1] = (short)((x + 1) + y * myWidth);
                    terrainIndices[(x + y * (myWidth - 1)) * 6 + 2] = (short)(x + y * myWidth);

                    terrainIndices[(x + y * (myWidth - 1)) * 6 + 3] = (short)((x + 1) + (y + 1) * myWidth);
                    terrainIndices[(x + y * (myWidth - 1)) * 6 + 4] = (short)(x + y * myWidth);
                    terrainIndices[(x + y * (myWidth - 1)) * 6 + 5] = (short)(x + (y + 1) * myWidth);
                }
            }

            ib = new IndexBuffer(Game.GraphicsDevice, typeof(short), (myWidth - 1) * (myHeight - 1) * 6, BufferUsage.WriteOnly);
            ib.SetData(terrainIndices);

            //Game.GraphicsDevice.VertexDeclaration = new VertexDeclaration(Game.GraphicsDevice, VertexMultitextured.VertexElements);

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix World = Matrix.CreateScale(myScale) *
                            Matrix.CreateFromQuaternion(myRotation) *
                            Matrix.CreateTranslation(myPosition);

            Matrix WVP = World * camera.View * camera.Projection;
            Matrix WV = World * camera.View;
            Matrix viewI = Matrix.Invert(camera.View);

            effect.Parameters["matWorldViewProj"].SetValue(WVP);
            effect.Parameters["matWorld"].SetValue(World);
            effect.Parameters["matWorldView"].SetValue(WV);
            effect.Parameters["matViewI"].SetValue(viewI);

            effect.Parameters["fBumpHeight"].SetValue(bumpHeight);
            effect.Parameters["vTextureScale"].SetValue(textureScale);
            effect.Parameters["vBumpSpeed"].SetValue(bumpSpeed);
            effect.Parameters["fFresnelBias"].SetValue(fresnelBias);
            effect.Parameters["fFresnelPower"].SetValue(fresnelPower);
            effect.Parameters["fHDRMultiplier"].SetValue(hdrMultiplier);
            effect.Parameters["vDeepColor"].SetValue(deepWaterColor.ToVector4());
            effect.Parameters["vShallowColor"].SetValue(shallowWaterColor.ToVector4());
            effect.Parameters["vReflectionColor"].SetValue(reflectionColor.ToVector4());
            effect.Parameters["fReflectionAmount"].SetValue(reflectionAmount);
            effect.Parameters["fWaterAmount"].SetValue(waterAmount);
            effect.Parameters["fWaveAmp"].SetValue(waveAmplitude);
            effect.Parameters["fWaveFreq"].SetValue(waveFrequency);

            //Game.GraphicsDevice.Vertices[0].SetSource(vb, 0, VertexMultitextured.SizeInBytes);
            Game.GraphicsDevice.SetVertexBuffer(vb);
            Game.GraphicsDevice.Indices = ib;

            //Game.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;

            //effect.Begin(SaveStateMode.SaveState);
            for (int p = 0; p < effect.CurrentTechnique.Passes.Count; p++)
            {
                //effect.CurrentTechnique.Passes[p].Begin();
                effect.CurrentTechnique.Passes[0].Apply();
                Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, myWidth * myHeight, 0, (myWidth - 1) * (myHeight - 1) * 2);

                //effect.CurrentTechnique.Passes[p].End();
            }
            //effect.End();

            //Game.GraphicsDevice.RenderState.FillMode = FillMode.Solid;

            base.Draw(gameTime);
        }
        public override void Update(GameTime gameTime)
        {
            effect.Parameters["fTime"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
            base.Update(gameTime);
        }
    }
}
