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
    public class ScreenGame : Screen
    {
        ScreenManager screenManager;    //���ofacade��T
        GameManager gameManager;        //���o�C��������T

        //�غc�l
        public ScreenGame(ScreenManager manager, GameManager game)
        {
            screenManager = manager;
            gameManager = game;
        }

        //�����b�e�X��screen���e����
        public override void SetFocus(ContentManager content, bool focus)
        {
            //�P�_�O�_�ثefocus�b���e��
            if (focus == true)
            {
                //Ū���귽
                gameManager.LoadFiles(content);
                gameManager.Audio.StopOpenBGM();
                GameOptions.HDR = true;
                GameOptions.isGameScreen = true;
            }
            //�������Lscreen��
            else
            {
                //����
                gameManager.UnloadFiles();
                GameOptions.isGameScreen = false;
            }
        }

        //�޲z��J
        public override void ProcessInput(float elapsedTime, InputManager input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            gameManager.ProcessInput(elapsedTime, input);
            int i, j = (int)gameManager.GameMode;
            for (i = 0; i < j; i++)
                if (input.IsKeyPressed(i, Keys.Escape) || GameOptions.GM2_Over || GameOptions.GM1_Over)
                {
                    screenManager.SetNextScreen(ScreenType.ScreenEnd);
                    gameManager.PlaySound("menu_cancel");
                    GameOptions.GM1_Over = false;
                    GameOptions.GM2_Over = false;
                }

            if (GameOptions.isQuit) 
            {
                screenManager.SetNextScreen(ScreenType.ScreenIntro);
               // GameOptions.Effect = false;
               // GameOptions.isQuit = false;
                GameOptions.GM1_Over = false;
                GameOptions.GM2_Over = false;            
            }
        }

        //�C������updat
        public override void Update(float elapsedTime)
        {
            //�C������update
            gameManager.Update(elapsedTime);
        
        }

        //�e�C������3d
        public override void Draw3D(GraphicsDevice gd)
        {
            //�e�C������3d
            gd.Clear(Color.Black);
            gameManager.Draw3D(gd);
        }

        //�e�C������GUI
        public override void Draw2D(GraphicsDevice gd, FontManager font)
        {
            //�e�C������GUI
            gameManager.Draw2D(font, gd);
        }
    }
}
