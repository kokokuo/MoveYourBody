using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Windows.Forms;
using KinectSimulation;
using OpenNI;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
//using CS_WinForms;


namespace Game_Kinect_Project
{
    /// <summary>
    /// 遊戲主體 
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {    
        //變數宣告
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        static Game1 instance;       
        ScreenManager screen;
        GameManager game;
        FontManager font;
        AudioManager audio;
        bool renderVsync = true;
        KinectDisplay kinectDisplay;

        /// <summary>
        /// 建構子
        /// </summary>
        public Game1(KinectDisplay kinectDisplay)
        {
            //遊戲
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            audio = new AudioManager();
            game = new GameManager(this, Components);
            graphics.PreferredBackBufferWidth = GameOptions.ScreenWidth;
            graphics.PreferredBackBufferHeight = GameOptions.ScreenHeight;
            IsFixedTimeStep = renderVsync;
            graphics.SynchronizeWithVerticalRetrace = renderVsync;
            //遊戲
            this.kinectDisplay = kinectDisplay;
        }

        //暫時用步道
        //public GraphicsDevice Graphics { get { return graphics.GraphicsDevice; } }

        /// <summary>
        /// DebugDrawer
        /// </summary>
        public DebugDrawer DebugDrawer
        {
            get { return game.DebugDrawer; }
        }

        /// <summary>
        /// 攝影機
        /// </summary>
        public Camera Camera
        {
            get { return game.Camera; }
        }

        /// <summary>
        /// Initialize
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();           
        }

        /// <summary>
        /// 呼叫每個物件的Load
        /// </summary>
        protected override void LoadContent()
        {
            //遊戲
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = new FontManager(graphics.GraphicsDevice);
            screen = new ScreenManager(this, font, game, kinectDisplay);
            font.LoadContent(Content);
            game.LoadContent(graphics.GraphicsDevice, Content);
            screen.LoadContent(graphics.GraphicsDevice, Content);
            //遊戲
        }

        /// <summary>
        /// 遊戲結束時清除物件
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// 設定渲染狀態
        /// </summary>
        public void SetRenderStates()
        {
            //遊戲
            this.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            this.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            this.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            //遊戲
        }

        /// <summary>
        /// 呼叫每個物件的Update
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            //遊戲         
            float ElapsedTimeFloat = (float)gameTime.ElapsedGameTime.TotalSeconds;
            screen.ProcessInput(ElapsedTimeFloat);
            screen.Update(ElapsedTimeFloat);
            //遊戲                           
            base.Update(gameTime);
            this.gameTime = gameTime;
        }

        public GameTime GetGameTime { get { return gameTime; } }
        GameTime gameTime;

        /// <summary>
        /// 呼叫每個物件的Draw
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            //遊戲
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            screen.Draw(graphics.GraphicsDevice);
            //遊戲
            base.Draw(gameTime);
        }

        /// <summary>
        /// 呼叫此函示可以轉全螢幕模式
        /// </summary>
        public void ToggleFullScreen()
        {
            graphics.ToggleFullScreen();
        }
         
        /// <summary>
        /// Singleton pattern
        /// </summary>
        static public Game1 GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// 程式進入點+Kinect
        /// </summary>
        [STAThread]
        static void Main()
        {
            KinecThread kinecThread = new KinecThread();
            //FacebookThread fb = new FacebookThread();
            using (Game1 game = new Game1(kinecThread.kinectDisplay))
            {
                Thread newThread = new Thread(new ThreadStart(kinecThread.Thread));
               // Thread newThread2 = new Thread(new ThreadStart(fb.Thread));

                //newThread2.ApartmentState = System.Threading.ApartmentState.STA;

                newThread.Start();
                //newThread2.Start();
                instance = game;
                game.Run();
                Thread.Sleep(1);
                newThread.Join();
                //newThread2.Join();
            }
        }

        /// <summary>
        /// 開一個Thread給Kinect
        /// </summary>
        public class KinecThread
        {
            public KinectDisplay kinectDisplay;
            public KinecThread()
            {
                kinectDisplay = new KinectDisplay();
            }
            public void Thread()
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(kinectDisplay);
            }
        };

        ///// <summary>
        ///// 開一個Thread給Facebook
        ///// </summary>
        //public class FacebookThread
        //{
        //    public Form1 FacebookDisplay;
        //    public FacebookThread()
        //    {
        //        FacebookDisplay = new Form1();
        //    }
        //    public void Thread()
        //    {

        //        Application.EnableVisualStyles();
        //        Application.SetCompatibleTextRenderingDefault(false);
        //        Application.Run(new Form1());
        //        //FacebookDisplay.Show();
        //        //while (true)
        //        //{
        //        //    //FacebookDisplay.Show();
        //        //}
        //    }
        //};  
    }
}
