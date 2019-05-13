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
using System.IO;

namespace Game_Kinect_Project
{
    /// <summary>
    /// 遊戲中2D介面皆是從這裡處理
    /// </summary>
    public class Graphic2D : DrawableGameComponent
    {
        ContentManager content;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont,spriteFont2;
        
        PhysicsSystem physics;

        GameManager gameManager;
        
        GameSpace.GameSpace1 gm;//我們需要知道GAME1與GAME2的資訊(分數...ETC
        GameSpace.GameSpace2 gm2;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        int bbWidth, bbHeight;

        ImageGenerator image;//Kinect相關(從攝影機抓圖片用的)
        ImageMetaData imageMD;
        DepthGenerator depth;
        DepthMetaData depthMD;

        public Graphic2D(Game game, PhysicsSystem physics, GameSpace.GameSpace1 gm, GameSpace.GameSpace2 gm2, GameManager gameManager)
            : base(game)
        {
            content = new ContentManager(game.Services);
            this.physics = physics;
            this.gm = gm;
            this.gm2 = gm2;
            this.gameManager = gameManager;

            ScriptNode scriptNode;
            image = new ImageGenerator(Context.CreateFromXmlFile(@"SamplesConfig.xml", out scriptNode));
            imageMD = new ImageMetaData();

            depth = new DepthGenerator(Context.CreateFromXmlFile(@"SamplesConfig.xml", out scriptNode));
            depthMD = new DepthMetaData();
        }

        private void GraphicsDevice_DeviceReset(object sender, EventArgs e)
        {
            bbWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            bbHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        protected override void LoadContent()
        {
            this.GraphicsDevice.DeviceReset += new EventHandler<EventArgs>(GraphicsDevice_DeviceReset);
            GraphicsDevice_DeviceReset(null, null);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = content.Load<SpriteFont>("Content/Font");
            spriteFont2 = content.Load<SpriteFont>("Content/Font2");
            damageTexture = content.Load<Texture2D>("Content/warning");
            runTexture = content.Load<Texture2D>("Content/run");
            gameOverTexture = content.Load<Texture2D>("Content/gameOver");
            playerLiveTexture = content.Load<Texture2D>("Content/live");
            speedDownTexture = content.Load<Texture2D>("Content/speeddown");
            UI = content.Load<Texture2D>("Content/BL_UI");
            goalTexture = content.Load<Texture2D>("Content/goal");
            pass = content.Load<Texture2D>("Content/passOrNot/pass");
            fail = content.Load<Texture2D>("Content/passOrNot/fail");
            UI2 = content.Load<Texture2D>("Content/UI2");
            go = content.Load<Texture2D>("Content/go");
            frameTexture = content.Load<Texture2D>("Content/frame");
            GM2_UI = content.Load<Texture2D>("Content/GM2_UI");
            board = content.Load<Texture2D>("Content/borad");

            
            //跳躍動畫
            jump0 = content.Load<Texture2D>("Content/manual/jump/jump-0");
            jump1 = content.Load<Texture2D>("Content/manual/jump/jump-1");
            jump2 = content.Load<Texture2D>("Content/manual/jump/jump-2");
            jump3 = content.Load<Texture2D>("Content/manual/jump/jump-3");
            jump4 = content.Load<Texture2D>("Content/manual/jump/jump-4");
            jump5 = content.Load<Texture2D>("Content/manual/jump/jump-5");

            jumpText = content.Load<Texture2D>("Content/manual/jump/jumpText");

            jump.Add(jump0);
            jump.Add(jump1);
            jump.Add(jump2);
            jump.Add(jump3);
            jump.Add(jump4);
            jump.Add(jump5);

            //移動動畫
            move0 = content.Load<Texture2D>("Content/manual/move/move-0");
            move1 = content.Load<Texture2D>("Content/manual/move/move-1");
            move2 = content.Load<Texture2D>("Content/manual/move/move-2");

            moveText = content.Load<Texture2D>("Content/manual/move/moveText");

            move.Add(move0);
            move.Add(move1);
            move.Add(move2);

            pause = content.Load<Texture2D>("Content/manual/pause/pause");

            psi = content.Load<Texture2D>("Content/manual/pause/psi");

            PSIaction = content.Load<Texture2D>("Content/manual/pause/action");

            push0 = content.Load<Texture2D>("Content/manual/push/push-0");
            push1 = content.Load<Texture2D>("Content/manual/push/push-1");
            push2 = content.Load<Texture2D>("Content/manual/push/push-2");

            pushText = content.Load<Texture2D>("Content/manual/push/pushText");

            push.Add(push0);
            push.Add(push1);
            push.Add(push2);
            push.Add(push1);
            push.Add(push0);

            run0 = content.Load<Texture2D>("Content/manual/run/run-0");
            run1 = content.Load<Texture2D>("Content/manual/run/run-1");
            run2 = content.Load<Texture2D>("Content/manual/run/run-2");

            runText = content.Load<Texture2D>("Content/manual/run/runText");

            run.Add(run0);
            run.Add(run1);
            run.Add(run2);

            swipe0 = content.Load<Texture2D>("Content/manual/swipe/swipe-0");
            swipe1 = content.Load<Texture2D>("Content/manual/swipe/swipe-1");
            swipe2 = content.Load<Texture2D>("Content/manual/swipe/swipe-2");
            swipe3 = content.Load<Texture2D>("Content/manual/swipe/swipe-3");
            swipe4 = content.Load<Texture2D>("Content/manual/swipe/swipe-4");

            swipeFrame = content.Load<Texture2D>("Content/manual/swipe/frame");

            swipeText = content.Load<Texture2D>("Content/manual/swipe/swipeText");

            swipe.Add(swipe0);
            swipe.Add(swipe1);
            swipe.Add(swipe2);
            swipe.Add(swipe3);
            swipe.Add(swipe4);

            actionFrame = content.Load<Texture2D>("Content/manual/backgroung");

            score = content.Load<Texture2D>("Content/score");

            uploading = content.Load<Texture2D>("Content/uploading");

            frames = new Rectangle[2];
            int frameWidth = damageTexture.Height / frames.Length;
            for (int i = 0; i < 2; i++)
                frames[i] = new Rectangle(100, 200, damageTexture.Width, frameWidth * i);

            frames2 = new Rectangle[2];
            frameWidth = runTexture.Height / frames2.Length;
            for (int i = 0; i < 2; i++)
                frames2[i] = new Rectangle(100, 200, damageTexture.Width, frameWidth * i);

            frames3 = new Rectangle[2];
            frameWidth = pass.Height / frames3.Length;
            for (int i = 0; i < 2; i++)
                frames3[i] = new Rectangle(301, 30, pass.Width, frameWidth * i);
        }
       
        protected override void UnloadContent()
        {
            content.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        public bool DrawHelp { get; internal set; }
        bool removeQuitUser = true;
        Texture2D damageTexture;
        Rectangle[] frames;
        int currentFrame = 0;
        Texture2D runTexture;
        Rectangle[] frames2;
        Texture2D gameOverTexture;
        Texture2D playerLiveTexture;
        Texture2D speedDownTexture;
        Texture2D UI;
        Texture2D goalTexture;
        Rectangle[] frames3;
        Texture2D pass, fail, go;
        Texture2D UI2;
        Texture2D frameTexture;
        Texture2D GM2_UI;
        Texture2D board;
        List<Texture2D> UserFaceList = new List<Texture2D>();
        List<Texture2D> UserFaceList2 = new List<Texture2D>();
        List<Vector2> UserFaceVector2List = new List<Vector2>();
        List<Vector2> UserFaceVector2List2 = new List<Vector2>();
        Vector2 UserFaceVector2 = new Vector2(0, 0);

        List<Texture2D> move = new List<Texture2D>();
        List<Texture2D> jump = new List<Texture2D>();
        List<Texture2D> push = new List<Texture2D>();
        List<Texture2D> run = new List<Texture2D>();
        List<Texture2D> swipe = new List<Texture2D>();

        Texture2D uploading;

        /// <summary>
        /// 操作手冊
        /// </summary>        
        Texture2D jump0, jump1, jump2, jump3, jump4, jump5;
        Texture2D move0, move1, move2;
        Texture2D pause;
        Texture2D psi;
        Texture2D PSIaction;
        Texture2D push0, push1, push2, push3;
        Texture2D run0, run1, run2;
        Texture2D swipe0, swipe1, swipe2, swipe3, swipe4;
        Texture2D swipeFrame;
        Texture2D actionFrame;
        Texture2D swipeText;
        Texture2D pushText;
        Texture2D moveText;
        Texture2D runText;
        Texture2D jumpText;

        Texture2D score;

        public Vector2 GetUserFaceVector2 { get { return UserFaceVector2; } }
        public List<Texture2D> GetUserFaceList { get { return UserFaceList; } }
        public List<Texture2D> GetUserFaceList2 { get { return UserFaceList2; } }
        public List<Vector2> GetUserFaceVector2List { get { return UserFaceVector2List; } }
        public List<Vector2> GetUserFaceVector2List2 { get { return UserFaceVector2List2; } }
        
        public override void Draw(GameTime gameTime)
        {
            frameCounter++;

            string fps = frameRate.ToString();

            spriteBatch.Begin();

            //if(UserFaceList.Count>0)
            spriteBatch.Draw(GetImageTexture(), new Rectangle(9, 45, 173 * 2, 114 * 2), Color.White);

            //if (UserFaceList.Count > 0)
            //    spriteBatch.Draw(UserFaceList[UserFaceList.Count - 1], new Rectangle(9, 45, 173, 114), new Rectangle((int)UserFaceVector2.X, (int)UserFaceVector2.Y, 100, 100), Color.White);

            UserFaceVector2.X = (int)gameManager.GetHead.X - 50;
            UserFaceVector2.Y = -((int)gameManager.GetHead.Y) - 50;

            
            if (GameOptions.CatchFace)
            {
                //UserFaceList.Add(GetImageTexture());             
                
                //UserFaceVector2List.Add(UserFaceVector2);
                GameOptions.CatchFace = false;
            }
            
            if (GameOptions.isLevelScreen) 
            {
                if (GameOptions.GameNumber == 1)
                {
                    UserFaceList.Add(GetImageTexture());
                    UserFaceVector2.X = (int)gameManager.GetHead.X - 50;
                    UserFaceVector2.Y = -((int)gameManager.GetHead.Y) - 50;
                    UserFaceVector2List.Add(UserFaceVector2);
                    FileStream stream = File.OpenWrite("C:/Users/Public/Pictures/Sample Pictures/Chrysanthemum" + UserFaceList.Count + ".jpg");
                    GetImageTexture().SaveAsJpeg(stream, 500, 500);
                    stream.Close();                  
                }
                else 
                {
                    UserFaceList2.Add(GetImageTexture());
                    UserFaceVector2.X = (int)gameManager.GetHead.X - 50;
                    UserFaceVector2.Y = -((int)gameManager.GetHead.Y) - 50;
                    UserFaceVector2List2.Add(UserFaceVector2);
                    FileStream stream = File.OpenWrite("C:/Users/Public/Pictures/Sample Pictures/Chrysanthemum" + UserFaceList2.Count + ".jpg");
                    GetImageTexture().SaveAsJpeg(stream, 500, 500);
                    stream.Close();   
                }
                GameOptions.isLevelScreen = false;
            }

          

            

            //有BUG(待修)
            //if (GameOptions.isQuit)
            //{                
            //        if (GameOptions.GameNumber == 1)
            //        {
            //            UserFaceList.RemoveAt(UserFaceList.Count - 1);
            //            UserFaceVector2List.RemoveAt(UserFaceVector2List.Count - 1);
            //        }
            //        else
            //        {
            //            UserFaceList2.RemoveAt(UserFaceList2.Count - 1);
            //            UserFaceVector2List2.RemoveAt(UserFaceVector2List2.Count - 1);
            //        }               
            //}

            
            //  spriteBatch.Draw(UI2, new Rectangle(0, 0, 1280, 720), Color.White);

            //DEBUG專用的文字(第一關)
            if (gm.GameTims > 10)
            {
                spriteBatch.Draw(UI, new Rectangle(0, 0, 1280, 720), Color.White);

               //////// //spriteBatch.DrawString(spriteFont, "Camera: X:" + ((Game1)this.Game).Camera.Position.X
               //////// //        + "\nY:" + ((Game1)this.Game).Camera.Position.Y
               //////// //        + "\nZ:" + ((Game1)this.Game).Camera.Position.Z, new Vector2(0, 20), Color.Yellow);

               //////// //spriteBatch.DrawString(spriteFont, "FPS: " + fps, new Vector2(0, 0), Color.Yellow);               
            
                spriteBatch.DrawString(spriteFont, "GameTime:" + gm.GameTims.ToString(), new Vector2(0, 80), Color.Yellow);

               //////// //spriteBatch.DrawString(spriteFont, "CameraTarget: " + ((Game1)this.Game).Camera.Target.X + "   " + ((Game1)this.Game).Camera.Target.Y + "   " + ((Game1)this.Game).Camera.Target.Z, new Vector2(0, 100), Color.Yellow);

               //////// //spriteBatch.DrawString(spriteFont, "CameraAngel: " + ((Game1)this.Game).Camera.Angles.X + "   " + ((Game1)this.Game).Camera.Angles.Y + "   ", new Vector2(0, 120), Color.Yellow);

               //////// //spriteBatch.DrawString(spriteFont, "DwarfPosition: " + gm.DwarfPosition.X, new Vector2(0, 140), Color.Blue);

               //////// //spriteBatch.DrawString(spriteFont, "Distance to Goal: " + Math.Abs(GameOptions.EndPoint - gm.DwarfPosition.Z), new Vector2(70, 640), Color.Red);

               //////// //spriteBatch.DrawString(spriteFont, "OriDisplacement: " + gm.test, new Vector2(0, 180), Color.Blue);

               //////// //spriteBatch.DrawString(spriteFont, "PlayLive: " + GameOptions.PlayerLive, new Vector2(980, 0), Color.Red);
                
               //////// //spriteBatch.DrawString(spriteFont, "Gold: " + GameOptions.GoldEarn, new Vector2(960, 640), Color.Gold);

               //////// //spriteBatch.DrawString(spriteFont, "Pos: " + gm.k.GetPosition(MrJoints.Head), new Vector2(50, 10), Color.Red);
               //////// spriteBatch.DrawString(spriteFont, "Pos: " + gm.CurrentUserPosition.ToString(), new Vector2(50, 10), Color.Red);
               //////// spriteBatch.DrawString(spriteFont, "Pos: " + gm.CenterPosition.ToString(), new Vector2(50, 50), Color.Red);

                if (GameOptions.DuringBeastTime)
                {
                    if (frameCounter % 30 == 0)
                        currentFrame = (currentFrame + 1) % frames.Length;
                    spriteBatch.Draw(damageTexture, new Rectangle(68, 209, 1165, 646), Color.White);
                }

                if (GameOptions.StartRunTime)
                {
                    if (frameCounter % 30 == 0)
                        currentFrame = (currentFrame + 1) % frames2.Length;
                    if (gm.GameTims < (1200 / 60))
                        spriteBatch.Draw(runTexture, new Rectangle(68, 209, 1165, 646), Color.White);
                }

                if (GameOptions.GameOver)
                {
                    Rectangle gameOverRectangle = new Rectangle(100, 200, 1024, 256);
                    spriteBatch.Draw(gameOverTexture, new Rectangle(68, 209, 1165, 646), Color.White);
                }

                if (GameOptions.GameFinish)
                {
                    Rectangle gameOverRectangle = new Rectangle(100, 200, 1024, 256);
                    spriteBatch.Draw(goalTexture, new Rectangle(68, 209, 1165, 646), Color.White);
                    spriteBatch.DrawString(spriteFont2, "" + gm.GetScore, new Vector2(400, 20), Color.Red);
                    spriteBatch.Draw(score, new Rectangle(0, 0, 879, 420), Color.White);
                }

                if (gm.IsSpeedDown)
                {
                    Rectangle speedDownRectangle = new Rectangle(100, 400, 600, 300);
                    spriteBatch.Draw(speedDownTexture, speedDownRectangle, Color.White);
                }

                for (int i = 0; i < GameOptions.PlayerLive; i++)
                {
                    Rectangle playerLiveRectangle = new Rectangle(i * 31 + 760, 2, 16, 23);
                    spriteBatch.Draw(playerLiveTexture, playerLiveRectangle, Color.White);
                }

            }

            /////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////
            ///////第二個遊戲/////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////

            if (gm2.GameTimeCount > 100)
            {

                //失敗-碰到板子
                if (GameOptions.isBodyPass && GameOptions.isPass)
                {
                    spriteBatch.Draw(fail, new Rectangle(301, 30, 650, 1300), Color.White);
                    //borad.Add(GetImageTexture());
                }
                //成功-無碰到
                if (!GameOptions.isBodyPass && GameOptions.isPass)
                {
                    spriteBatch.Draw(pass, new Rectangle(301, 30, 650, 1300), Color.White);
                    //borad.Add(GetImageTexture());
                }

                //spriteBatch.DrawString(spriteFont, "TIME: " + gm2.GameTimeCount, new Vector2(0, 0), Color.Yellow);

                //spriteBatch.DrawString(spriteFont, "NUMBER OF WALL: " + gm2.WallQueue, new Vector2(0, 80), Color.Yellow);

                //spriteBatch.DrawString(spriteFont, "COUNT OF TEXTURE: " + borad.Count, new Vector2(0, 100), Color.Yellow);

                //spriteBatch.DrawString(spriteFont, "X: " + gm2.x + "Y: " + gm2.y, new Vector2(0, 150), Color.Yellow);

                //spriteBatch.DrawString(spriteFont, "CameraAngel: " + ((Game1)this.Game).Camera.Angles.X + "   " + ((Game1)this.Game).Camera.Angles.Y + "   ", new Vector2(0, 120), Color.Yellow);

                if (gm2.GameTimeCount > 400 && gm2.GameTimeCount < 500)
                {
                    spriteBatch.Draw(go, new Rectangle(222, 147, 837, 427), Color.White);
                }

                if (gm2.GameTimeCount > 25)
                {
                    spriteBatch.Draw(GM2_UI, new Rectangle(0, 0, 1280, 720), Color.White);
                    spriteBatch.DrawString(spriteFont2, GameOptions.passCount.ToString(), new Vector2(400, 630), Color.Black);
                }


                //spriteBatch.DrawString(spriteFont, "Camera: X:" + ((Game1)this.Game).Camera.Position.X
                //            + "\nY:" + ((Game1)this.Game).Camera.Position.Y
                //            + "\nZ:" + ((Game1)this.Game).Camera.Position.Z, new Vector2(0, 20), Color.Yellow);
                //spriteBatch.DrawString(spriteFont, "CameraAngel: " + ((Game1)this.Game).Camera.Angles.X + "   " + ((Game1)this.Game).Camera.Angles.Y + "   ", new Vector2(0, 120), Color.Yellow);

                //spriteBatch.DrawString(spriteFont, "HeadPos: " + gameManager.GetHead, new Vector2(0, 180), Color.Yellow);

                if (GameOptions.isShowPic)
                {
                    for (int i = 0; i < borad.Count; i++)
                        spriteBatch.Draw(borad[i], new Rectangle(320, 130, 640, 480), Color.White);
                    spriteBatch.Draw(frameTexture, new Rectangle(295, 114, 693, 546), Color.White);
                }

                if (GameOptions.isPlayerScreen)
                {
                     //spriteBatch.Draw(GetImageTexture(), new Rectangle(320, 180, 640, 480), Color.White);
                }

            }





            //顯示狀態
            if (gm.test != null)
            {
                
            }

            //spriteBatch.DrawString(spriteFont2, "asdasd:" + gm2.coll.ToString(), new Vector2(55, 45), Color.Black);
            if (gm.test.UserState.ToString() != "Tracking" && gm.test.UserState.ToString() != "TrackingButWaring")
            {
                spriteBatch.Draw(actionFrame, new Rectangle(0, 0, 1280, 720), Color.White);
                spriteBatch.Draw(psi, new Rectangle(0, 0, 1280, 720), Color.White);
                spriteBatch.Draw(PSIaction, new Rectangle(0, 0, 1280, 720), Color.White);
                spriteBatch.Draw(board, new Rectangle(0, 0, 400, 160), Color.White);
                spriteBatch.DrawString(spriteFont2, "" + gm.test.UserState.ToString(), new Vector2(20, 20), Color.Black);
            }


            if (GameOptions.isHelpScreen)
            {
                counter++;

                if (counter % 20 == 0)
                {
                    runNumber++;
                    if (runNumber == 3) runNumber = 0;
                }
                spriteBatch.Draw(actionFrame, new Rectangle(782, 0 + 100, 350, 179), Color.White);
                spriteBatch.Draw(run[runNumber], new Rectangle(782, 0 + 100, 350, 179), Color.White);
                spriteBatch.Draw(runText, new Rectangle(782, 0 + 100, 350, 179), Color.White);


                if (counter % 60 == 0)
                {
                    moveNumber++;
                    if (moveNumber == 3) moveNumber = 0;
                }
                spriteBatch.Draw(actionFrame, new Rectangle(782, 200 + 100, 350, 179), Color.White);
                spriteBatch.Draw(move[moveNumber], new Rectangle(782, 200 + 100, 350, 179), Color.White);
                spriteBatch.Draw(moveText, new Rectangle(782, 200 + 100, 350, 179), Color.White);

                if (counter % 20 == 0)
                {
                    jumpNumber++;
                    if (jumpNumber == 6) jumpNumber = 0;
                }
                spriteBatch.Draw(actionFrame, new Rectangle(782, 400 + 100, 350, 179), Color.White);
                spriteBatch.Draw(jump[jumpNumber], new Rectangle(782, 400 + 100, 350, 179), Color.White);
                spriteBatch.Draw(jumpText, new Rectangle(782, 400+100, 350, 179), Color.White);


                if (counter % 10 == 0)
                {
                    swipeNumber++;
                    if (swipeNumber == 5) swipeNumber = 0;
                }
                //spriteBatch.Draw(swipeFrame, new Rectangle(0, 0, 1280 / 3, 720 / 3), Color.White);
                spriteBatch.Draw(actionFrame, new Rectangle(120, 0 + 200, 350, 179), Color.White);
                spriteBatch.Draw(swipe[swipeNumber], new Rectangle(120, 0+200, 350, 179), Color.White);
                spriteBatch.Draw(swipeText, new Rectangle(120, 0 + 200, 350, 179), Color.White);

                if (counter % 20 == 0)
                {
                    pushNumber++;
                    if (pushNumber == 5) pushNumber = 0;
                }
                spriteBatch.Draw(actionFrame, new Rectangle(120, 200+200, 350, 179), Color.White);
                spriteBatch.Draw(push[pushNumber], new Rectangle(120, 200 + 200, 350, 179), Color.White);
                spriteBatch.Draw(pushText, new Rectangle(120, 200 + 200, 350, 179), Color.White);

            }

            if (gm.test.UserState.ToString() == "Tracking" || gm.test.UserState.ToString() == "TrackingButWaring")
            {
                if (GameOptions.isGameScreen && GameOptions.GameNumber == 1)
                {
                    counter++;
                    if (gm.GameTims < 200)
                    {
                        if (counter % 60 == 0)
                        {
                            moveNumber++;
                            if (moveNumber == 3) moveNumber = 0;
                        }
                        spriteBatch.Draw(actionFrame, new Rectangle(0, 0, 1280, 720), Color.White);
                        spriteBatch.Draw(move[moveNumber], new Rectangle(0, 0, 1280, 720), Color.White);
                        spriteBatch.Draw(moveText, new Rectangle(0, 0, 1280, 720), Color.White);
                    }
                    else if (gm.GameTims > 200 && gm.GameTims < 400)
                    {
                        if (counter % 20 == 0)
                        {
                            jumpNumber++;
                            if (jumpNumber == 6) jumpNumber = 0;
                        }
                        spriteBatch.Draw(actionFrame, new Rectangle(0, 0, 1280, 720), Color.White);
                        spriteBatch.Draw(jump[jumpNumber], new Rectangle(0, 0, 1280, 720), Color.White);
                        spriteBatch.Draw(jumpText, new Rectangle(0, 0, 1280, 720), Color.White);
                    }
                    else if (gm.GameTims > 400 && gm.GameTims < 600)
                    {
                        if (counter % 20 == 0)
                        {
                            runNumber++;
                            if (runNumber == 3) runNumber = 0;
                        }
                        spriteBatch.Draw(actionFrame, new Rectangle(0, 0, 1280, 720), Color.White);
                        spriteBatch.Draw(run[runNumber], new Rectangle(0, 0, 1280, 720), Color.White);
                        spriteBatch.Draw(runText, new Rectangle(0, 0, 1280, 720), Color.White);
                    }
                    else if (gm.GameTims > 1000 && gm.GameTims < 1400)
                    {
                        if (counter % 20 == 0)
                        {
                            runNumber++;
                            if (runNumber == 3) runNumber = 0;
                        }
                        spriteBatch.Draw(actionFrame, new Rectangle(782, 202, 500, 281), Color.White);
                        spriteBatch.Draw(run[runNumber], new Rectangle(782, 202, 500, 281), Color.White);
                        spriteBatch.Draw(runText, new Rectangle(782, 202, 500, 281), Color.White);
                    }
                    else if (gm.GameTims > 1500 && gm.GameTims < 1700)
                    {
                        if (counter % 60 == 0)
                        {
                            moveNumber++;
                            if (moveNumber == 3) moveNumber = 0;
                        }
                        spriteBatch.Draw(actionFrame, new Rectangle(782, 202, 500, 281), Color.White);
                        spriteBatch.Draw(move[moveNumber], new Rectangle(782, 202, 500, 281), Color.White);
                        spriteBatch.Draw(moveText, new Rectangle(782, 202, 500, 281), Color.White);
                    }
                    else if (gm.GameTims > 1800 && gm.GameTims < 2100)
                    {
                        if (counter % 20 == 0)
                        {
                            jumpNumber++;
                            if (jumpNumber == 6) jumpNumber = 0;
                        }
                        spriteBatch.Draw(actionFrame, new Rectangle(782, 202, 500, 281), Color.White);
                        spriteBatch.Draw(jump[jumpNumber], new Rectangle(782, 202, 500, 281), Color.White);
                        spriteBatch.Draw(jumpText, new Rectangle(782, 202, 500, 281), Color.White);
                    }
                }

            

                if (GameOptions.isLevelsScreen)
                {
                    counter++;
                    if (counter % 10 == 0)
                    {
                        swipeNumber++;
                        if (swipeNumber == 5) swipeNumber = 0;
                    }
                    //spriteBatch.Draw(swipeFrame, new Rectangle(0, 0, 1280 / 3, 720 / 3), Color.White);
                    spriteBatch.Draw(actionFrame, new Rectangle(782, 202, 500, 281), Color.White);
                    spriteBatch.Draw(swipe[swipeNumber], new Rectangle(782, 202, 500, 281), Color.White);
                    spriteBatch.Draw(swipeText, new Rectangle(782, 202, 500, 281), Color.White);

                    if (counter % 20 == 0)
                    {
                        pushNumber++;
                        if (pushNumber == 5) pushNumber = 0;
                    }
                    spriteBatch.Draw(actionFrame, new Rectangle(0, 202, 500, 281), Color.White);
                    spriteBatch.Draw(push[pushNumber], new Rectangle(0, 202, 500, 281), Color.White);
                    spriteBatch.Draw(pushText, new Rectangle(0, 202, 500, 281), Color.White);
                }

                if (GameOptions.isIntroScreen)
                {
                    counter++;

                    if (counter % 20 == 0)
                    {
                        pushNumber++;
                        if (pushNumber == 5) pushNumber = 0;
                    }
                    spriteBatch.Draw(actionFrame, new Rectangle(0, 202, 500, 281), Color.White);
                    spriteBatch.Draw(push[pushNumber], new Rectangle(0, 202, 500, 281), Color.White);
                    spriteBatch.Draw(pushText, new Rectangle(0, 202, 500, 281), Color.White);
                }

                if (GameOptions.isPlayerScreen)
                {
                    counter++;

                    if (counter % 20 == 0)
                    {
                        pushNumber++;
                        if (pushNumber == 5) pushNumber = 0;
                    }
                    spriteBatch.Draw(actionFrame, new Rectangle(0, 202, 500, 281), Color.White);
                    spriteBatch.Draw(push[pushNumber], new Rectangle(0, 202, 500, 281), Color.White);
                    spriteBatch.Draw(pushText, new Rectangle(0, 202, 500, 281), Color.White);
                }

            }

            if (GameOptions.isUpload) 
            {
                uploadCount++;
                spriteBatch.Draw(uploading, new Rectangle(0, 0, 1280, 720), Color.White);
                if (uploadCount == 100) { uploadCount = 0; GameOptions.isUpload = false; }
            }

            spriteBatch.End();

            if (GameOptions.isCatchPic) { borad.Add(GetImageTexture()); }
         
            ((Game1)this.Game).SetRenderStates();
        }
        int runNumber = 0;
        int jumpNumber = 0;
        int pushNumber = 0;
        int moveNumber = 0;
        int swipeNumber = 0;
        int counter = 0;
        int uploadCount = 0;       

        List<Texture2D> borad = new List<Texture2D>();

        public unsafe Texture2D GetImageTexture()
        {
            Texture2D imageTexture = null;
            byte[] bmpBytes = null;
            this.image.GetMetaData(imageMD);
            lock (this)
            {
                int byteCount = imageMD.XRes * imageMD.YRes * 4;
                if (bmpBytes == null || bmpBytes.Length != byteCount)
                    bmpBytes = new byte[byteCount];
                fixed (byte* texturePointer = &bmpBytes[0])
                {
                    RGB24Pixel* pImage =
                    (RGB24Pixel*)this.image.ImageMapPtr.ToPointer();
                    int pointerWalker = 0;
                    for (int y = 0; y < imageMD.YRes; ++y)
                    {
                        for (int x = 0; x < imageMD.XRes; ++x, ++pImage, pointerWalker += 4)
                        {
                            texturePointer[pointerWalker] = pImage->Red;

                            texturePointer[pointerWalker + 1] = pImage->Green;

                            texturePointer[pointerWalker + 2] = pImage->Blue;

                            texturePointer[pointerWalker + 3] = 255;
                        }
                    }
                }
            }
            if (imageTexture == null)
                imageTexture = new Texture2D(GraphicsDevice,
            imageMD.XRes, imageMD.YRes);
            imageTexture.SetData(bmpBytes);
            return imageTexture;
        }
     
        private unsafe Texture2D GetDepthTextureMy()
        {
            byte[] bmpBytes = null;
            Texture2D background = null;
            this.depth.GetMetaData(depthMD);
            lock (this)
            {         
                int byteCount = depthMD.XRes * depthMD.YRes * 4;

                if (bmpBytes == null ||
                    bmpBytes.Length != byteCount)
                    bmpBytes = new byte[byteCount];


                fixed (byte* texturePointer = &bmpBytes[0])
                {
                    ushort* pDepth = (ushort*)this.depth.DepthMapPtr.ToPointer();

                    int pointerWalker = 0;
                    for (int y = 0; y < depthMD.YRes; ++y)
                    {
                        for (int x = 0; x < depthMD.XRes; ++x, ++pDepth)
                        {
                            byte pixel = (byte)(*pDepth / (float)depthMD.ZRes * 255f);
                            texturePointer[pointerWalker] = pixel;
                            texturePointer[pointerWalker + 3] = 255;
                            pointerWalker += 4;
                        }
                    }
                }
            }

            if (background == null)
                background = new Texture2D(GraphicsDevice, depthMD.XRes, depthMD.YRes);

            background.SetData(bmpBytes);
            return background;
        }



    }
}
