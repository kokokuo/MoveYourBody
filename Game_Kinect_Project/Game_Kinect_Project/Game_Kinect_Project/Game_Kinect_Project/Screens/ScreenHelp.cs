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
    public class ScreenHelp : Screen
    {
        ScreenManager screenManager;    //���ofacade��T
        GameManager gameManager;        //���o�C��������T

        Texture2D textureControls;    //�귽�ܼ�
        Texture2D textureDisplay;     
        Texture2D textureContinue;
        Texture2D help_UI;

        //�غc�l
        public ScreenHelp(ScreenManager manager, GameManager game)
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
                textureControls = content.Load<Texture2D>(
                                    "screens/controls");
                textureDisplay = content.Load<Texture2D>(
                                    "screens/controls_display");
                textureContinue = content.Load<Texture2D>(
                                    "screens/continue");

                help_UI = content.Load<Texture2D>(
                                    "help_UI");

                GameOptions.isHelpScreen = true;
            }
            //�������Lscreen��
            else
            {
                //free�귽
                textureControls = null;
                textureDisplay = null;
                textureContinue = null;

                GameOptions.isHelpScreen = false;
            }
        }

        //�޲z��J
        public override void ProcessInput(float elapsedTime, InputManager input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            for (int i = 0; i < 2; i++)
            {
                //�޲z��J
                if (input.IsKeyPressed(i, Keys.Enter) ||
                    input.IsKeyPressed(i, Keys.Escape) ||
                    input.IsKeyPressed(i, Keys.Space))
                {
                    screenManager.SetNextScreen(ScreenType.ScreenIntro);
                    gameManager.PlaySound("menu_cancel");
                }
            }
        }

        //�S��
        public override void Update(float elapsedTime)
        {
        }

        //�e�I��
        public override void Draw3D(GraphicsDevice gd)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            gd.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1, 0);
            screenManager.DrawBackground(gd);
        }

        //�e2D����
        public override void Draw2D(GraphicsDevice gd, FontManager font)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            screenManager.DrawTexture(help_UI, new Rectangle(0, 0, 1280, 720),
                Color.White, BlendState.AlphaBlend);

            //Rectangle rect = new Rectangle(0, 0, 0, 0);

            //int screenSizeX = gd.Viewport.Width;
            //int screenSizeY = gd.Viewport.Height;

            //rect.Width = textureControls.Width;
            //rect.Height = textureControls.Height;
            //rect.X = screenSizeX / 2 - rect.Width / 2;
            //rect.Y = 40;
            //screenManager.DrawTexture(textureControls, rect, 
            //    Color.White, BlendState.AlphaBlend);

            //rect.Width = textureDisplay.Width;
            //rect.Height = textureDisplay.Height;
            //rect.X = screenSizeX / 2 - rect.Width / 2;
            //rect.Y = screenSizeY / 2 - rect.Height / 2 + 10;
            //screenManager.DrawTexture(textureDisplay, rect, 
            //    Color.White, BlendState.AlphaBlend);

            //rect.Width = textureContinue.Width;
            //rect.Height = textureContinue.Height;
            //rect.X = screenSizeX / 2 - rect.Width / 2;
            //rect.Y = screenSizeY - rect.Height - 60;
            //screenManager.DrawTexture(textureContinue, rect, 
            //    Color.White, BlendState.AlphaBlend);
        }
    }
}
