using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Physics;
using Microsoft.Xna.Framework.Input;
using MrozKinect;
using OpenNI;

namespace Game_Kinect_Project
{
    /// <summary>
    /// 暫停介面的處理
    /// </summary>
    class Pause : DrawableGameComponent
    {
        ContentManager content;
        SpriteBatch spriteBatch;
        Texture2D background, quitImg, continueImg, textureCursorAnim, textureCursorBullet;

        int bbWidth, bbHeight;

        bool pauseEnable = false;
        public bool PauseEnable { get { return pauseEnable; } set { pauseEnable = value; } }

        enum PauseSelect //狀態控制用的
        {
            quitSelect,
            continueSelect
        }

        PauseSelect pauseSelect;
        public Pause(Game game, GameSpace.GameSpace2 gm2)
            : base(game)
        {
            content = new ContentManager(game.Services);          
        }

        private void GraphicsDevice_DeviceReset(object sender, EventArgs e)
        {
            bbWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;//這段可有可無  之前測試畫面切割用
            bbHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        protected override void LoadContent()
        {
            this.GraphicsDevice.DeviceReset += new EventHandler<EventArgs>(GraphicsDevice_DeviceReset);
            GraphicsDevice_DeviceReset(null, null);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = content.Load<Texture2D>("Content/pause/pause");//讀取resource
            quitImg = content.Load<Texture2D>("Content/pause/quit_pause");
            continueImg = content.Load<Texture2D>("Content/pause/continue_pause");
            textureCursorAnim = content.Load<Texture2D>("Content/screens/cursor_anim");
            textureCursorBullet = content.Load<Texture2D>("Content/screens/cursor_bullet");
        }

        protected override void UnloadContent()
        {
            content.Unload();
        }

        InputManager input;//我必須要知道目前使用者的輸入資訊
        public void GetInput(InputManager input) 
        {
            this.input = input; 
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            //KINECT
            if (input.IsPause)//輸入告訴遊戲要暫停
            {
                PauseEnable = true;
            }

            //鍵盤
            if (keyState.IsKeyDown(Keys.B))//鍵盤強制暫停
            {
                PauseEnable = true;
            }

            if (GameOptions.isGameScreen)
            {
                if (pauseEnable)
                {
                    //按下按鈕
                    if (keyState.IsKeyDown(Keys.Enter) || input.GetLeftPushSignal)
                    {
                        if (pauseSelect == PauseSelect.continueSelect)//判斷使用者要離開還是繼續
                        {
                            PauseEnable = false;
                            input.IsPause = false;
                        }
                        else
                        {
                            GameOptions.isQuit = true;
                            PauseEnable = false;
                            input.IsPause = false;
                        }
                    }
                }
            }
            base.Update(gameTime);
        }

        //畫箭頭
        void DrawCursor(int x, int y)
        {
            Rectangle rect = new Rectangle(0, 0, 0, 0);

            rect.X = x - textureCursorAnim.Width / 2;
            rect.Y = y - textureCursorAnim.Height / 2;
            rect.Width = textureCursorAnim.Width;
            rect.Height = textureCursorAnim.Height;
            spriteBatch.Draw(textureCursorAnim, rect, Color.White);

            rect.X = x - textureCursorBullet.Width / 2;
            rect.Y = y - textureCursorBullet.Height / 2;
            rect.Width = textureCursorBullet.Width;
            rect.Height = textureCursorBullet.Height;
            spriteBatch.Draw(textureCursorBullet, rect, Color.White);
        }

        public override void Draw(GameTime gameTime)
        {
            MouseState m = Mouse.GetState();
            Vector2 handPos = new Vector2(0, 0);
            if (input != null)
            {
                //把手的位置資訊傳給游標
                handPos = input.GetRightHand(0);
                handPos = input.GetMousePosittion(0);
                handPos = input.GetRightHand(0);
                handPos.X *= 1280 / 640;
                handPos.Y *= 720 / 320;
                handPos.X -= 200;
              //handPos = input.GetMousePosittion(0);
            }
            spriteBatch.Begin();
            if (pauseEnable)
            {
                spriteBatch.Draw(background, new Rectangle(240, 160, 800, 400), Color.White);
                //spriteBatch.Draw(continueImg, new Rectangle(335, 425, 250, 100), Color.White);
                spriteBatch.Draw(quitImg, new Rectangle(710, 425, 250, 100), Color.White);

                //這裡是判斷看你游標指到哪裡
                if (handPos.X < 335 + continueImg.Width && handPos.X > 335
                        && -handPos.Y > 425 && -handPos.Y < 425 + continueImg.Height)
                {
                    spriteBatch.Draw(continueImg, new Rectangle(330, 420, 260, 110), Color.White);
                    pauseSelect = PauseSelect.continueSelect;
                }
                else
                    spriteBatch.Draw(continueImg, new Rectangle(335, 425, 250, 100), Color.White);


                if (handPos.X < 710 + quitImg.Width && handPos.X > 710
                        && -handPos.Y > 425 && -handPos.Y < 425 + quitImg.Height)
                {
                    spriteBatch.Draw(quitImg, new Rectangle(705, 420, 260, 110), Color.White);
                    pauseSelect = PauseSelect.quitSelect;
                }
                else
                    spriteBatch.Draw(quitImg, new Rectangle(710, 425, 250, 100), Color.White);


                DrawCursor((int)handPos.X, -(int)handPos.Y);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
