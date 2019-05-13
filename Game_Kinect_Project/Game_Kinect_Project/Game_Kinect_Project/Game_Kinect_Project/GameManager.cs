using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Utils;
using Game_Kinect_Project.PhysicObjects;
using JigLibX.Vehicles;
using System.Diagnostics;

//using CS_WinForms;

//Animation
using Xclna.Xna.Animation;

using KinectModel.StateEnum;

using MrozKinect;
using MrozKinect.KinectModels;


namespace Game_Kinect_Project
{
    //遊戲模式
    public enum GameMode
    {
        None = 0,
        SinglePlayer,            //單人遊戲
        MultiPlayer              //多人
    }

    /// <summary>
    /// 遊戲真正在執行的地方 
    /// </summary>
    public class GameManager : IDisposable
    {

        MrKinect kinect;
        KinectSkeleton skeleton;
        Effect kinectEffect;
        MrozKinect.KinectModels.KinectModel modelK;
        
        //debug用字型
        SpriteFont rightHandPos;
        GameMode gameMode = GameMode.SinglePlayer;    // current game mode

        //音效
        AudioManager audio;

        //第一個遊戲
        GameSpace.GameSpace1 gameSpace1;
        //地二個
        GameSpace.GameSpace2 gameSpace2;

        //寫給未來聲音用的
        public void PlaySound(String soundName)
        {   
            //好像用不到
        }

        public GameSpace.GameSpace1 game1 { get { return gameSpace1; } }

        //TEST
        public InputManager test { get { return input; } }

        //讓其他object取得遊戲時間
        public float GameTims { get { return 0; } }

        //遊戲中的視野
        Camera camera;
        //物理碰撞的線
        DebugDrawer debugDrawer;
        BloomComponent bloom;
        //Pause
        Pause pause;

        Graphic2D physStats;

        //Form1 fb;

        /// <summary>
        /// 管理整個3D遊戲
        /// </summary>
        GameComponentCollection Components;
        Game1 game;
        public GameManager(Game1 game, GameComponentCollection Components)
        {
            //音悅
            this.audio = new AudioManager();
                      
            //抓XNA framework的東西
            this.game = game;
            this.Components = Components;

            //this.fb = fb;

            //遊戲中的相機
            camera = new Camera(game);
            Components.Add(camera);
                  
            this.gameSpace1 = new GameSpace.GameSpace1(game, Components, audio, camera);
            this.gameSpace2 = new GameSpace.GameSpace2(game, Components, audio, camera);

            //碰撞線
            debugDrawer = new DebugDrawer(game);
            debugDrawer.Enabled = false;
            Components.Add(debugDrawer);

            //計算FPS
            physStats = new Graphic2D(game, gameSpace1.PhysicSystem, gameSpace1, gameSpace2, this);
            Components.Add(physStats);
            physStats.DrawOrder = 10;

            //視窗設定
            game.IsMouseVisible = true;
            game.Window.Title = "NTUTCSIE_Kinect_GAME " + System.Reflection.Assembly.GetAssembly(typeof(PhysicsSystem)).GetName().Version.ToString();
        }

        /// <summary>
        /// 取得遊戲模式
        /// </summary>
        public GameMode GameMode
        {
            get { return gameMode; }
            set { gameMode = value; }
        }

        public Game1 Game 
        {
            get { return game; }
        }

        /// <summary>
        /// GET
        /// </summary>
        public Texture2D GetImageTexture { get { return physStats.GetImageTexture(); } }
        public Vector2 GetUserFaceVector2 { get { return physStats.GetUserFaceVector2; } }
        public List<Texture2D> GetUserFaceList { get { return physStats.GetUserFaceList; } }
        public List<Vector2> GetUserFaceVector2List { get { return physStats.GetUserFaceVector2List; } }
        public List<Texture2D> GetUserFaceList2 { get { return physStats.GetUserFaceList2; } }
        public List<Vector2> GetUserFaceVector2List2 { get { return physStats.GetUserFaceVector2List2; } }
        public List<int> GetScore { get { return gameSpace1.GetUserScore; } }
        public List<int> GetScore2 { get { return gameSpace2.GetUserScore2; } }
        public AudioManager Audio { get { return audio; } }
        public KinectSkeleton k { get { return skeleton; } }

        
        /// <summary>
        /// DebugDrawer
        /// </summary>
        public DebugDrawer DebugDrawer
        {
            get { return debugDrawer; }
        }

        /// <summary>
        /// 攝影機
        /// </summary>
        public Camera Camera
        {
            get { return camera; }
        }


        public Vector2 GetHead { get { return input != null ? input.GetHead() : new Vector2(0, 0); } }

        /// <summary>
        /// 讀取遊戲中的資源
        /// </summary>
        public void LoadFiles(ContentManager content)
        {
            if (GameOptions.GameNumber == 1)
                gameSpace1.LoadFiles(content);//讀第一個遊戲
            else
                gameSpace2.LoadFiles(content);//讀第二個遊戲

            resetEffect = true;
            resetEffectLayer = 0;

            //HDREnable = false;
            //Components.Remove(bloom);
            //CanBeDisableHDR = true;
            //Components.Remove(pause);

            //if (CanBeDisableHDR)
            //{
            //    HDREnable = true;
            //    Components.Add(bloom);
            //    if (!Components.Contains(pause))
            //        Components.Add(pause);
            //}
            //CanBeDisableHDR = false;
            lockFB = true;           
        }

        bool lockFB = false;

        /// <summary>
        /// Unload game files
        /// </summary>
        public void UnloadFiles()
        {            
            //if (fb.GET != null)
                //this.fb.GET.UploadPhoto();

            if (GameOptions.GameNumber == 1)
                gameSpace1.UnloadFiles();
            else
                gameSpace2.UnloadFiles();
        }
 
        /// <summary>
        /// 得到screen中的input資訊
        /// </summary>
        InputManager input;
        public void ProcessInput(float elapsedTime, InputManager input)
        {         
            this.input = input;
            //gameSpace2.GetInput(input);
            if (GameOptions.GameNumber == 1)
                gameSpace1.GetInput(input);
            else
                gameSpace2.GetInput(input);
            if(pause!=null)
            pause.GetInput(input);

        }

        bool resetEffect = true;
        int resetEffectLayer = 0;
        public void Update(float elapsedTime)
        { 
            //kinect.GetKinectData();    
            //for (int i = 0; i < BoneCollision.Count; i++)
            //    BoneCollision[i].PhysicsBody.CollisionSkin.callbackFn -= (handleCollisionDetection2);
            //for (int i = 0; i < BoneCollision.Count; i++)
            //    BoneCollision[i].PhysicsBody.CollisionSkin.callbackFn -= (handleCollisionDetection3);
            //for (int i = 0; i < BoneCollision.Count; i++)
            //    BoneCollision[i].PhysicsBody.CollisionSkin.callbackFn += new CollisionCallbackFn(handleCollisionDetection2);
            //for (int i = 0; i < BoneCollision.Count; i++)
            //    BoneCollision[i].PhysicsBody.CollisionSkin.callbackFn += new CollisionCallbackFn(handleCollisionDetection3);

            if (gameSpace2.GameTimeCount == 4100) 
            {             
                GameOptions.isUpload = true;
              //  if (lockFB)
                //{
                gameSpace2.GameTimeCount++;
                    //fb.aa();                   
                //}
            }

            if (pause != null)
            {
                if (!pause.PauseEnable)
                {
                    gameSpace1.UPDATEEEEE(elapsedTime);
                    if (GameOptions.GameNumber == 1)
                        gameSpace1.Update(elapsedTime);
                    else
                        gameSpace2.Update(elapsedTime);
                }
                if (gameSpace1.test.UserState.ToString()
                    != "Tracking" && gameSpace1.test.UserState.ToString() != "TrackingButWaring") 
                {
                    pause.PauseEnable = true;
                }
            }

            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if (keyState.IsKeyDown(Keys.Escape) || GameOptions.GM2_Over || GameOptions.isQuit||GameOptions.GM1_Over)
            {
                HDREnable = false;
                Components.Remove(bloom);
                CanBeDisableHDR = true;
                Components.Remove(pause);
                //pause.Dispose();
                //pause = null;
                //bloom.Dispose();
                //gameSpace1.ResetScene();
            }

            if (keyState.IsKeyDown(Keys.Z))
            {
                if (CanBeDisableHDR)
                {
                    HDREnable = true;
                    Components.Add(bloom);
                }
                CanBeDisableHDR = false;
                          
            }

            if (keyState.IsKeyDown(Keys.Q))
            {
                HDREnable = false;
                Components.Remove(bloom);
                CanBeDisableHDR = true;
            }

            if (resetEffect) 
            {
                if (resetEffectLayer == 0)
                {
                    HDREnable = false;
                    Components.Remove(bloom);
                    CanBeDisableHDR = true;
                    Components.Remove(pause);
                }

                if (resetEffectLayer == 1)
                {
                    if (CanBeDisableHDR)
                    {
                        HDREnable = true;
                        Components.Add(bloom);
                    }
                    CanBeDisableHDR = false;

                    resetEffect = false;
                }
                resetEffectLayer++;
            }

            // Switch to the next bloom settings preset?
            if (keyState.IsKeyDown(Keys.I))
            {
                if (!s)
                {
                    bloomSettingsIndex = (bloomSettingsIndex + 1) %
                                         BloomSettings.PresetSettings.Length;

                    bloom.Settings = BloomSettings.PresetSettings[bloomSettingsIndex];
                    bloom.Visible = true;
                }
                s = true;
            }
            if (keyState.IsKeyUp(Keys.I)) { s = false; }
            if (keyState.IsKeyUp(Keys.O)) { s = false; }
            if (keyState.IsKeyUp(Keys.P)) { s = false; }
            // Toggle bloom on or off?
            if (keyState.IsKeyDown(Keys.O))
            {
                if(!s)
                bloom.Visible = !bloom.Visible;
            }

            // Cycle through the intermediate buffer debug display modes?
            if (keyState.IsKeyDown(Keys.P))
            {
                if (!s)
                {
                    bloom.Visible = true;
                    bloom.ShowBuffer++;

                    if (bloom.ShowBuffer > BloomComponent.IntermediateBuffer.FinalResult)
                        bloom.ShowBuffer = 0;
                }
            }
        }

        bool s = true;
        int bloomSettingsIndex = 0;
        bool CanBeDisableHDR = false;
        /// <summary>
        /// 測試用
        /// </summary>
        GraphicsDevice gd;
        bool HDREnable = true;
        public void Draw2D(FontManager font, GraphicsDevice gd)
        {
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }
            this.gd = gd;

            if (pause == null)
            {
                //Pause
                pause = new Pause(game, gameSpace2);
                Components.Add(pause);
            }
            if (!Components.Contains(pause))
                Components.Add(pause);
            if (HDREnable)
            {            
                if (bloom == null)
                {                    
                    bloom = new BloomComponent(game);
                    Components.Add(bloom);
                }
                else { bloom.BeginDraw(); }
            }            
            
            if (GameOptions.GameNumber == 1)
                gameSpace1.Draw(gd);
            else
                gameSpace2.Draw(gd);           
        }
       
        /// <summary>
        /// 測試用
        /// </summary>    
        public void Draw3D(GraphicsDevice gd)
        {      
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }
        }

        /// <summary>
        /// 提早讀取物件測試
        /// </summary>
        public void LoadContent(GraphicsDevice gd,
            ContentManager content)
        {
            //LoadFiles(content);
        }

        /// <summary>
        /// 卸載
        /// </summary>
        public void UnloadContent()
        {
            UnloadFiles();
        }

        #region IDisposable Members

        bool isDisposed = false;
        public bool IsDisposed
        {
            get { return isDisposed; }
        }

        public void Dispose()
        {

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
