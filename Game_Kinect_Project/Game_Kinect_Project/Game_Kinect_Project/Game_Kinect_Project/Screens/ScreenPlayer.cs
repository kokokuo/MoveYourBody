#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

#endregion

using Xclna.Xna.Animation;

namespace Game_Kinect_Project
{
    public class ScreenPlayer : Screen
    {
        ScreenManager screenManager;    //取得facade資訊
        GameManager gameManager;        //取得遊戲中的資訊
  
        Texture2D textureSelectBack;      // select and back texture
        Texture2D textureSelectCancel;    // select and cancel texture
        Texture2D textureCatchFace;

        static TextureCube reflectCube;

        // ship selection for each player
        int[] selection = new int[2] { 0, 1 };

        // confirmed status for each player
        bool[] confirmed = new bool[2] { false, false };
        

        // rotation matrix for each player ship model
        Matrix[] rotation = new Matrix[2] { Matrix.Identity, Matrix.Identity };

        // total elapsed time for ship model rotation
        float elapsedTime = 0.0f;

        bool isReplace = false;

        //建構子
        public ScreenPlayer(ScreenManager manager, GameManager game)
        {
            screenManager = manager;
            gameManager = game;
        }

        //必須在畫出此screen之前執行
        public override void SetFocus(ContentManager content, bool focus)
        {
            //判斷是否目前focus在此畫面
            if (focus == true)
            {
                //讀取資源
                confirmed[0] = false;
                confirmed[1] = (gameManager.GameMode == GameMode.SinglePlayer);

                rotation[0] = Matrix.Identity;
                rotation[1] = Matrix.Identity;
            
                textureSelectBack = content.Load<Texture2D>(
                                            "screens/push");
                textureSelectCancel = content.Load<Texture2D>(
                                            "screens/pushed");

                textureCatchFace = content.Load<Texture2D>("screens/catch_face");

                GameOptions.isPlayerScreen = true;

            }
            //切換到其他screen時
            else
            {
                // free all resources                      
                textureSelectBack = null;
                textureSelectCancel = null;

                GameOptions.isPlayerScreen = false;
            }
        }

        public override void ProcessInput(float elapsedTime, InputManager input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }       

            int i, j = (int)gameManager.GameMode;

            for (i = 0; i < j; i++)
                if (confirmed[i] == false)
                {               
                    // confirm selection
                    if (input.IsKeyPressed(i, Keys.Enter) || input.IsMouseClickLeft(i) || input.GetLeftPushSignal)
                    {
                        gameManager.Audio.PlayAudio("smw_message");
                        confirmed[i] = true;
                        GameOptions.CatchFace = true;
                    }

                    // cancel and return to intro menu
                    if (input.IsKeyPressed(i, Keys.Escape) || input.IsMouseClickRight(i))
                    {                       
                        screenManager.SetNextScreen(ScreenType.ScreenIntro);
                        gameManager.PlaySound("menu_cancel");
                    }                                   
                }
                else
                {
                    // cancel selection
                    if (input.IsKeyPressed(i, Keys.Escape))
                    {
                        confirmed[i] = false;
                        gameManager.PlaySound("menu_cancel");
                    }
                }

            // if both ships confirmed, go to game screen
            if (confirmed[0] && confirmed[1])
            {
                screenManager.SetNextScreen(ScreenType.ScreenLevel);
            }
        }

        public override void Update(float elapsedTime)
        {
            // accumulate elapsed time
            this.elapsedTime += elapsedTime;
        }

        //畫背景
        public override void Draw3D(GraphicsDevice gd)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            // clear backgournd
            gd.Clear(Color.Black);

            // draw background animation
            screenManager.DrawBackground(gd);

            // screen aspect
            float aspect = (float)gd.Viewport.Width / (float)gd.Viewport.Height;

            // camera position
            Vector3 cameraPosition = new Vector3(0, 240, -800);

            // view and projection matrices
            Matrix view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
            Matrix projection =
                Matrix.CreatePerspectiveFieldOfView(0.25f, aspect, 1, 1000);
            Matrix viewProjection = view * projection;

            // translation matrix
            Matrix transform = Matrix.CreateTranslation(0, -40, 0);

            // restore blend modes
            gd.DepthStencilState = DepthStencilState.Default;
            gd.BlendState = BlendState.Opaque;
        }

        //畫2d GUI
        public override void Draw2D(GraphicsDevice gd, FontManager font)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            Rectangle rect = new Rectangle(0, 0, 0, 0);

            int screenSizeX = gd.Viewport.Width;
            int screenSizeY = gd.Viewport.Height;

            //單人遊戲
            if (gameManager.GameMode == GameMode.SinglePlayer)
            {
                rect.Width = textureSelectBack.Width;
                rect.Height = textureSelectBack.Height;
                rect.X = screenSizeX / 2 - rect.Width / 2;
                rect.Y = 50;
                if (confirmed[0])
                {
                    rect.Width = textureSelectCancel.Width;
                    rect.Height = textureSelectCancel.Height;
                    screenManager.DrawTexture(textureSelectCancel, rect,
                        Color.White, BlendState.AlphaBlend);
                }
                else
                    screenManager.DrawTexture(textureSelectBack, rect,
                        Color.White, BlendState.AlphaBlend);

                screenManager.DrawTexture(gameManager.GetImageTexture, new Rectangle(320, 180, 640, 480),
                        Color.White, BlendState.AlphaBlend);

                screenManager.DrawTexture(textureCatchFace, new Rectangle((int)gameManager.GetUserFaceVector2.X+320, (int)gameManager.GetUserFaceVector2.Y+180, 500, 500),
                        Color.White, BlendState.AlphaBlend);
            }          
        }
        /// <summary>
        /// Performs effect initialization, which is required in XNA 4.0
        /// </summary>
        /// <param name="model"></param>
        private void FixupShip(Model model, string path)
        {
            Game1 game = Game1.GetInstance();

            foreach (ModelMesh mesh in model.Meshes)
            {
                // for each mesh part
                foreach (Effect effect in mesh.Effects)
                {
                    effect.Parameters["Reflect"].SetValue(GetReflectCube());
                }
            }
        }

        /// <summary>
        /// Creates a reflection textureCube
        /// </summary>
        static TextureCube GetReflectCube()
        {
            if (reflectCube != null)
                return reflectCube;

            Color[] cc = new Color[]
            {
                new Color(1,0,0), new Color(0.9f,0,0.1f), 
                new Color(0.8f,0,0.2f), new Color(0.7f,0,0.3f),
                new Color(0.6f,0,0.4f), new Color(0.5f,0,0.5f),
                new Color(0.4f,0,0.6f), new Color(0.3f,0,0.7f),
                new Color(0.2f,0,0.8f), new Color(0.1f,0,0.9f),
                new Color(0.1f,0,0.9f), new Color(0.0f,0,1.0f),
            };

            reflectCube = new TextureCube(Game1.GetInstance().GraphicsDevice,
                8, true, SurfaceFormat.Color);

            Random rand = new Random();

            for (int s = 0; s < 6; s++)
            {
                Color[] sideData = new Color[reflectCube.Size * reflectCube.Size];
                for (int i = 0; i < sideData.Length; i++)
                {
                    sideData[i] = cc[rand.Next(cc.Length)];
                }
                reflectCube.SetData((CubeMapFace)s, sideData);
            }

            return reflectCube;
        }

    }
}
