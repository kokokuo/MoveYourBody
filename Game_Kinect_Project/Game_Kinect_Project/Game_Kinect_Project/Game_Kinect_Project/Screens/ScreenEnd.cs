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



namespace Game_Kinect_Project
{
    public class ScreenEnd : Screen
    {
        ScreenManager screenManager;    //取得facade資訊
        GameManager gameManager;        //取得遊戲中的資訊           
        Texture2D textureContinue, textureUI;      //資源變數

        float elapsedTime;        //執行時間

        //建構子
        public ScreenEnd(ScreenManager manager, GameManager game)
        {
            screenManager = manager;
            gameManager = game;
        }

        //必須在畫出此screen之前執行
        public override void SetFocus(ContentManager content, bool focus)
        {
            //判斷是否目前focus在此畫面
            if (focus)
            {
                //讀取資源            
                textureContinue = content.Load<Texture2D>("screens/continue");
                textureUI = content.Load<Texture2D>("record_UI");
                gameManager.game1.GameTims = 0;
                
            }
            //切換到其他screen時
            else
            {
               // gameManager.Audio.StopOpenBGM();
            }
        }

        // process input
        public override void ProcessInput(float elapsedTime, InputManager input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            int i, j = (int)gameManager.GameMode;
            for (i = 0; i < j; i++)
            {
                // Any key/button to go back
                if (
                    input.IsKeyPressed(i, Keys.Enter) ||
                    input.IsKeyPressed(i, Keys.Escape) ||
                    input.IsKeyPressed(i, Keys.Space) ||
                    input.GetLeftPushSignal)
                {
                    screenManager.SetNextScreen(ScreenType.ScreenIntro);
                }
            }
        }

        // update screen
        public override void Update(float elapsedTime)
        {
            // accumulate elapsed time
            this.elapsedTime += elapsedTime;
        }

        // draw 3D scene
        public override void Draw3D(GraphicsDevice gd)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            //// clear background
            //gd.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1, 0);

            //// draw background animation
            //screenManager.DrawBackground(gd);

            //// screen aspect
            //float aspect = (float)gd.Viewport.Width / (float)gd.Viewport.Height;

            //// camera position
            //Vector3 cameraPosition = new Vector3(0, 240, -800);

            //// view and projection matrices
            //Matrix view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
            //Matrix projection =
            //    Matrix.CreatePerspectiveFieldOfView(0.25f, aspect, 1, 1000);
            //Matrix viewProjection = view * projection;

            //// rotation matrix
            //Matrix rotation = Matrix.CreateRotationY(0.5f * elapsedTime);
            //// translation matrix
            //Matrix translation = Matrix.CreateTranslation(0, -40, 0);

            //// set additive blend with no glow (zero on alpha)
            //gd.DepthStencilState = DepthStencilState.DepthRead;
            //gd.BlendState = BlendState.AlphaBlend;

            //// restore blend modes
            //gd.BlendState = BlendState.Opaque;
            //gd.DepthStencilState = DepthStencilState.Default;
            gd.Clear(Color.Black);
            screenManager.DrawBackground(gd);
        }

        // draw 2D gui
        public override void Draw2D(GraphicsDevice gd, FontManager font)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            Rectangle rect = new Rectangle(0, 0, 0, 0);

            int screenSizeX = gd.Viewport.Width;
            int screenSizeY = gd.Viewport.Height;

            // draw continue message
            rect.Width = textureContinue.Width;
            rect.Height = textureContinue.Height;
            rect.Y = screenSizeY - rect.Height - 60;
            rect.X = screenSizeX / 2 - rect.Width / 2;
            //screenManager.DrawTexture(textureContinue, rect,
            //    Color.White, BlendState.AlphaBlend);

            for (int i = 0; i < gameManager.GetUserFaceList.Count; i++)
            {
                screenManager.DrawTexture(gameManager.GetUserFaceList[gameManager.GetUserFaceList.Count-1-i], new Rectangle(i * 9 + 50, 45 * i * 3 + 50 + 50, 173, 114), new Rectangle((int)gameManager.GetUserFaceVector2List[i].X, (int)gameManager.GetUserFaceVector2List[i].Y, 100, 100),
                    Color.White, BlendState.AlphaBlend);
                Random s=new Random();
                
            }
            for (int i = 0; i < gameManager.GetScore.Count; i++)
                font.DrawText(FontType.ArialLarge, "Score: " + gameManager.GetScore[gameManager.GetScore.Count-1-i], new Vector2(i * 9 + 250, 45 * i * 3 + 50 + 50 + 20), Color.Black);
            
            for (int i = 0; i < gameManager.GetUserFaceList2.Count; i++)
            {
                screenManager.DrawTexture(gameManager.GetUserFaceList2[gameManager.GetUserFaceList2.Count-1-i], new Rectangle(i * 9 + 50 + 650 + 50 + 50, 45 * i * 3 + 50 + 50, 173, 114), new Rectangle((int)gameManager.GetUserFaceVector2List2[i].X, (int)gameManager.GetUserFaceVector2List2[i].Y, 100, 100),
                    Color.White, BlendState.AlphaBlend);                
            }

            for (int i = 0; i < gameManager.GetScore2.Count;i++ )
                font.DrawText(FontType.ArialLarge, "Pass : " + gameManager.GetScore2[gameManager.GetScore2.Count-1-i], new Vector2(i * 9 + 250 + 650 + 50 + 50, 45 * i * 3 + 50 + 50 + 20), Color.Black);

                screenManager.DrawTexture(textureUI, new Rectangle(200, 0, 800, 500), Color.White, BlendState.AlphaBlend);
        }
    }
}
