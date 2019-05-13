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
    public class ScreenLevel : Screen
    {
        ScreenManager screenManager;    //���ofacade��T
        GameManager gameManager;        //���o�C��������T
        
        const int NumberLevels = 2;   // number of available levels to choose from

        //Ū���C�����Ϥ��귽
        String[] levels = new String[NumberLevels] { "level1", "level2" };

        //����
        bool pushed;

        //�귽�ܼ�
        Texture2D[] levelShots = new Texture2D[NumberLevels];

        Texture2D selectBack, textureSelectCancel;     // select and back texture
        Texture2D changeLevel;    // change level texture

        int selection = 0;

        //�غc�l
        public ScreenLevel(ScreenManager manager, GameManager game)
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
                pushed = false;
                //Ū���귽
                for (int i = 0; i < NumberLevels; i++)
                    levelShots[i] = content.Load<Texture2D>(
                            "screens/" + levels[i]);
                selectBack = content.Load<Texture2D>(
                            "screens/push");
                textureSelectCancel = content.Load<Texture2D>(
                            "screens/pushed");
                changeLevel = content.Load<Texture2D>(
                            "screens/change_level");

                GameOptions.isLevelsScreen = true;
            }
            //�������Lscreen��
            else
            {
                //����
                for (int i = 0; i < NumberLevels; i++)
                    levelShots[i] = null;
                selectBack = null;
                changeLevel = null;
                GameOptions.isLevelsScreen = false;
            }
        }

        //�޲z��J
        public override void ProcessInput(float elapsedTime, InputManager input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            } 

            int i, j = (int)gameManager.GameMode;
            for (i = 0; i < j; i++)
            {
                // select
                if (input.IsKeyPressed(i, Keys.Enter) || input.IsMouseClickLeft(i) || input.GetLeftPushSignal)
                {
                    gameManager.Audio.PlayAudio("smw_message");
                    screenManager.SetNextScreen(ScreenType.ScreenGame);
                    GameOptions.GameNumber = selection + 1;
                    GameOptions.isLevelScreen = true;
                    pushed = true;
                }

                // cancel
                if (input.IsKeyPressed(i, Keys.Escape) || input.IsMouseClickRight(i))
                {
                    
                    screenManager.SetNextScreen(ScreenType.ScreenPlayer);
                    gameManager.PlaySound("menu_cancel");
                }

                //�W�@��
                if (input.IsKeyPressed(i, Keys.Left) || input.GetSwip)
                {
                    if (selection == 0)
                        selection = levels.GetLength(0) - 1;
                    else
                        selection = selection - 1;
                    gameManager.PlaySound("menu_change");
                    input.GetSwip = false;
                }

                //�U�@��
                if (input.IsKeyPressed(i, Keys.Right))
                {
                    selection = (selection + 1) % levels.GetLength(0);
                    gameManager.PlaySound("menu_change");
                }
            }
        }

        //update
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
            gd.Clear(Color.Black);
            screenManager.DrawBackground(gd);
        }

        //�e2D GUI
        public override void Draw2D(GraphicsDevice gd, FontManager font)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            int screenSizeX = gd.Viewport.Width;
            int screenSizeY = gd.Viewport.Height;

            Rectangle rect = new Rectangle(0, 0, 0, 0);

            //draw level
            rect.Width = levelShots[selection].Width;
            rect.Height = levelShots[selection].Height;
            rect.X = (screenSizeX - rect.Width) / 2;
            rect.Y = (screenSizeY - rect.Height) / 2 + 30;
            screenManager.DrawTexture(levelShots[selection], rect,
                Color.White, BlendState.AlphaBlend);


            if (pushed)
            {
                rect.Width = selectBack.Width;
                rect.Height = selectBack.Height;
                rect.X = (screenSizeX - rect.Width) / 2;
                rect.Y = 30;
                screenManager.DrawTexture(textureSelectCancel, rect,
                    Color.White, BlendState.AlphaBlend);
            }
            else
            {
                //draw back
                rect.Width = selectBack.Width;
                rect.Height = selectBack.Height;
                rect.X = (screenSizeX - rect.Width) / 2;
                rect.Y = 30;
                screenManager.DrawTexture(selectBack, rect,
                    Color.White, BlendState.AlphaBlend);
            }

            //draw change
            rect.Width = changeLevel.Width;
            rect.Height = changeLevel.Height;
            rect.X = (screenSizeX - rect.Width) / 2;
            rect.Y = screenSizeY - rect.Height - 30;
            screenManager.DrawTexture(changeLevel, rect,
                Color.White, BlendState.AlphaBlend);
        }
    }
}
