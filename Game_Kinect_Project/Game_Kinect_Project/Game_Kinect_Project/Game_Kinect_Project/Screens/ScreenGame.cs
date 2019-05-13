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
        ScreenManager screenManager;    //取得facade資訊
        GameManager gameManager;        //取得遊戲中的資訊

        //建構子
        public ScreenGame(ScreenManager manager, GameManager game)
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
                gameManager.LoadFiles(content);
                gameManager.Audio.StopOpenBGM();
                GameOptions.HDR = true;
                GameOptions.isGameScreen = true;
            }
            //切換到其他screen時
            else
            {
                //卸載
                gameManager.UnloadFiles();
                GameOptions.isGameScreen = false;
            }
        }

        //管理輸入
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

        //遊戲中的updat
        public override void Update(float elapsedTime)
        {
            //遊戲中的update
            gameManager.Update(elapsedTime);
        
        }

        //畫遊戲中的3d
        public override void Draw3D(GraphicsDevice gd)
        {
            //畫遊戲中的3d
            gd.Clear(Color.Black);
            gameManager.Draw3D(gd);
        }

        //畫遊戲中的GUI
        public override void Draw2D(GraphicsDevice gd, FontManager font)
        {
            //畫遊戲中的GUI
            gameManager.Draw2D(font, gd);
        }
    }
}
