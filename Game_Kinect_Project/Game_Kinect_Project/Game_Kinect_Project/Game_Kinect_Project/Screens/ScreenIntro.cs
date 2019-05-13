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
        ScreenManager screenManager;    //���ofacade��T
        GameManager gameManager;        //���o�C��������T

        int menuSelection=0;              //�Q��쪺menu
        float menuTime;                 //menutime for animation

        Texture2D textureLogo;          //�귽�ܼ�
        Texture2D textureLens;          
        
        Texture2D textureCursorAnim;    //��и귽
        Texture2D textureCursorBullet;

        //���D
        Texture2D ntut_title;

        SpriteFont rightHandPos;        //DeBUG�Ϊ���r
        SpriteBatch spriteBatch;


        //menu���귽
        const int NumberMenuItems = 4;
        String[] menuNames = new String[NumberMenuItems] 
                 { "game_start", "MP", "help", "quit" };

        //�p��
        Texture2D[] textureMenu = new Texture2D[NumberMenuItems];
        //�j��
        Texture2D[] textureMenuHover = new Texture2D[NumberMenuItems];

        //�ƹ���T(�qinputManager���o)
        Vector2 mousePosition=new Vector2();
        bool isClickLeft=false;

        //�غc�l
        public ScreenIntro(ScreenManager manager, GameManager game)
        {
            screenManager = manager;
            gameManager = game;        
        }

        //�����b�e�X��screen���e����
        public override void SetFocus(ContentManager content, bool focus)
        {
            //�P�_�O�_�ثefocus�b���e��
            if (focus)
            {
                //Ū���귽
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
            //�������Lscreen��
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
        //�޲z��J
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

                //�޲z��J
                if (input.IsKeyPressed(i, Keys.Enter) || 
                    input.IsKeyPressed(i, Keys.Space) ||
                    input.GetLeftPushSignal
                    ) 
                {
                    gameManager.Audio.PlayAudio("smw_message");
                    switch (menuSelection)
                    {
                        case 0:
                            //�}�l�C��
                            gameManager.GameMode = GameMode.SinglePlayer;
                            screenManager.SetNextScreen(ScreenType.ScreenPlayer);
                            break;
                        case 1:
                            //�O����
                            //gameManager.GameMode = GameMode.MultiPlayer;
                            screenManager.SetNextScreen(ScreenType.ScreenEnd);
                            break;
                        case 2:
                            //Help
                            screenManager.SetNextScreen(ScreenType.ScreenHelp);
                            break;
                        case 3:
                            //���}�C��
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

        //�e�I��3D
        public override void Draw3D(GraphicsDevice gd)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }
            gd.Clear(Color.Black);
            screenManager.DrawBackground(gd);
        }

        //�e�b�Y
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

        //�e2dGUI
        public override void Draw2D(GraphicsDevice gd, FontManager font)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            //screen���x��
            Rectangle rect = new Rectangle(gd.Viewport.X, gd.Viewport.Y, 
                            gd.Viewport.Width, gd.Viewport.Height);
            
            // draw lens flare texture
            screenManager.DrawTexture(textureLens, rect,
                Color.White, BlendState.Additive);
            
            //// draw logo texture
            //screenManager.DrawTexture(textureLogo, rect,
            //    Color.White, BlendState.AlphaBlend);

            //NTUT�j�_�I
            screenManager.DrawTexture(ntut_title, new Rectangle(60, 45, 1161, 646),
                            Color.White, BlendState.AlphaBlend);


            int X = 100;

            //�e�Xmenu�����D          
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
