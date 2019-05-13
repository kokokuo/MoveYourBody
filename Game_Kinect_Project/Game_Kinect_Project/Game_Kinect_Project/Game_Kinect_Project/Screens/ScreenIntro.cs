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
    public class ScreenIntro : Screen
    {
        ScreenManager screenManager;    //取得facade資訊
        GameManager gameManager;        //取得遊戲中的資訊

        int menuSelection=0;              //被選到的menu
        float menuTime;                 //menutime for animation

        Texture2D textureLogo;          //資源變數
        Texture2D textureLens;          
        
        Texture2D textureCursorAnim;    //游標資源
        Texture2D textureCursorBullet;

        //標題
        Texture2D ntut_title;

        SpriteFont rightHandPos;        //DeBUG用的文字
        SpriteBatch spriteBatch;


        //menu的資源
        const int NumberMenuItems = 4;
        String[] menuNames = new String[NumberMenuItems] 
                 { "game_start", "MP", "help", "quit" };

        //小圖
        Texture2D[] textureMenu = new Texture2D[NumberMenuItems];
        //大圖
        Texture2D[] textureMenuHover = new Texture2D[NumberMenuItems];

        //滑鼠資訊(從inputManager取得)
        Vector2 mousePosition=new Vector2();
        bool isClickLeft=false;

        //建構子
        public ScreenIntro(ScreenManager manager, GameManager game)
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
                gameManager.GameMode = GameMode.SinglePlayer;

                textureLogo = content.Load<Texture2D>(
                                            "screens/background");
                textureLens = content.Load<Texture2D>(
                                            "screens/intro_lens");

                textureCursorAnim = content.Load<Texture2D>(
                                            "screens/cursor_anim");
               
                textureCursorBullet = content.Load<Texture2D>(
                                            "screens/cursor_bullet");

                ntut_title = content.Load<Texture2D>(
                                            "ntut");

                for (int i = 0; i < NumberMenuItems; i++)
                {
                    textureMenu[i] = content.Load<Texture2D>(
                                    "screens/" + menuNames[i]);
                    textureMenuHover[i] = content.Load<Texture2D>(
                                    "screens/" + menuNames[i] + "_hover");
                }

                rightHandPos = content.Load<SpriteFont>("Font");//font
                GameOptions.isIntroScreen = true;
                if (GameOptions.isQuit)
                {
                    if (GameOptions.GameNumber == 1)
                    {
                        gameManager.GetUserFaceList.RemoveAt(gameManager.GetUserFaceList.Count - 1);
                        gameManager.GetUserFaceVector2List.RemoveAt(gameManager.GetUserFaceVector2List.Count - 1);
                    }
                    else
                    {
                        gameManager.GetUserFaceList2.RemoveAt(gameManager.GetUserFaceList2.Count - 1);
                        gameManager.GetUserFaceVector2List2.RemoveAt(gameManager.GetUserFaceVector2List2.Count - 1);
                    }
                }
                GameOptions.isQuit = false;               
                gameManager.Audio.PlayOpenBGM();
            }
            //切換到其他screen時
            else
            {
                // free all resources
                textureLogo = null;
                textureLens = null;
                textureCursorAnim = null;           
                textureCursorBullet = null;

                for (int i = 0; i < NumberMenuItems; i++)
                {
                    textureMenu[i] = null;
                    textureMenuHover[i] = null;
                }
                GameOptions.isIntroScreen = false;
            }
        }

        InputManager input;
        //管理輸入
        public override void ProcessInput(float elapsedTime, InputManager input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            mousePosition = input.GetMousePosittion(0);
            this.input = input;
               
            for (int i = 0; i < 2; i++)
            {
                mousePosition = input.GetMousePosittion(i);
                isClickLeft = input.IsMouseClickLeft(i);

                //管理輸入
                if (input.IsKeyPressed(i, Keys.Enter) || 
                    input.IsKeyPressed(i, Keys.Space) ||
                    input.GetLeftPushSignal
                    ) 
                {
                    gameManager.Audio.PlayAudio("smw_message");
                    switch (menuSelection)
                    {
                        case 0:
                            //開始遊戲
                            gameManager.GameMode = GameMode.SinglePlayer;
                            screenManager.SetNextScreen(ScreenType.ScreenPlayer);
                            break;
                        case 1:
                            //記分版
                            //gameManager.GameMode = GameMode.MultiPlayer;
                            screenManager.SetNextScreen(ScreenType.ScreenEnd);
                            break;
                        case 2:
                            //Help
                            screenManager.SetNextScreen(ScreenType.ScreenHelp);
                            break;
                        case 3:
                            //離開遊戲
                            screenManager.Exit();
                            break;
                    }
                    gameManager.PlaySound("menu_select");
                }                          
            }
          //  GameOptions.isQuit = false;
        }

        //update
        public override void Update(float elapsedTime)
        {
            menuTime += elapsedTime;
        }

        //畫背景3D
        public override void Draw3D(GraphicsDevice gd)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }
            gd.Clear(Color.Black);
            screenManager.DrawBackground(gd);
        }

        //畫箭頭
        void DrawCursor(int x, int y)
        {
            Rectangle rect = new Rectangle(0, 0, 0, 0);

            float rotation = menuTime * 2;

            // draw animated cursor texture
            rect.X = x - textureCursorAnim.Width / 2;
            rect.Y = y - textureCursorAnim.Height / 2;
            rect.Width = textureCursorAnim.Width;
            rect.Height = textureCursorAnim.Height;
            screenManager.DrawTexture(textureCursorAnim, rect, rotation, 
                Color.White, BlendState.AlphaBlend);

            // draw bullet cursor texture
            rect.X = x - textureCursorBullet.Width / 2;
            rect.Y = y - textureCursorBullet.Height / 2;
            rect.Width = textureCursorBullet.Width;
            rect.Height = textureCursorBullet.Height;
            //screenManager.DrawTexture(textureCursorBullet, rect,
            //    Color.White, BlendState.AlphaBlend);          
        }

        //畫2dGUI
        public override void Draw2D(GraphicsDevice gd, FontManager font)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            //screen的矩形
            Rectangle rect = new Rectangle(gd.Viewport.X, gd.Viewport.Y, 
                            gd.Viewport.Width, gd.Viewport.Height);
            
            // draw lens flare texture
            screenManager.DrawTexture(textureLens, rect,
                Color.White, BlendState.Additive);
            
            //// draw logo texture
            //screenManager.DrawTexture(textureLogo, rect,
            //    Color.White, BlendState.AlphaBlend);

            //NTUT大冒險
            screenManager.DrawTexture(ntut_title, new Rectangle(60, 45, 1161, 646),
                            Color.White, BlendState.AlphaBlend);


            int X = 100;

            //畫出menu的標題          
            for (int i = 0; i < NumberMenuItems; i++)
            {
                rect.X = X;
                rect.Y = 400;
                rect.Width = textureMenu[i].Width;
                rect.Height = textureMenu[i].Height;
                if (input != null)
                {
                    //if (input.GetMousePosittion(0).X < rect.X + textureMenu[i].Width && input.GetMousePosittion(0).X > rect.X
                    //       && input.GetMousePosittion(0).Y > rect.Y && input.GetMousePosittion(0).Y < rect.Y + textureMenu[i].Height)
                    //{
                    //    screenManager.DrawTexture(textureMenuHover[i], rect,
                    //    Color.White, BlendState.AlphaBlend);
                    //    menuSelection = i;
                    //}
                    //else
                    //{
                    //    screenManager.DrawTexture(textureMenu[i], rect,
                    //     Color.White, BlendState.AlphaBlend);
                    //}
                    //X += 300;
                    //rect.X = (int)input.GetMousePosittion(0).X;
                    //rect.Y = (int)input.GetMousePosittion(0).Y;
                }

                //Kinect
                if (input != null)
                {
                    Vector2 handPos = new Vector2(0, 0);
                    if (input != null)
                    {
                        handPos = input.GetRightHand(0);
                        //handPos.X -= 200;
                        //handPos.Y -= 200;
                        handPos.X *= (1280 / 640);
                        handPos.Y *= (720 / 320);
                        handPos.X -= 400;
                        //handPos = input.GetMousePosittion(0);
                    }
                    if (handPos.X < rect.X + textureMenu[i].Width && handPos.X > rect.X
                        && -handPos.Y > rect.Y && -handPos.Y < rect.Y + textureMenu[i].Height)
                    {
                        screenManager.DrawTexture(textureMenuHover[i], rect,
                        Color.White, BlendState.AlphaBlend);
                        menuSelection = i;
                    }
                    else
                        screenManager.DrawTexture(textureMenu[i], rect,
                            Color.White, BlendState.AlphaBlend);
                    X += 300;


                    rect.X = (int)handPos.X;
                    rect.Y = -(int)handPos.Y;
                }
                DrawCursor(rect.X, rect.Y);


            }
        }
    }
}
