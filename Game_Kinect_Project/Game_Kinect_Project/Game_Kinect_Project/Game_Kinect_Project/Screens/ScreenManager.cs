using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using KinectSimulation;

namespace Game_Kinect_Project
{
    public class ScreenManager : IDisposable
    {
        Game1 mainGame;
        GameManager gameManager;             //取得遊戲中的資訊
        FontManager fontManager;             //字體管理
        InputManager inputManager;           //取得input中的資訊
        ContentManager contentManager;       //XNA內建的讀取資源管理
        //AudioManager audioManager;          //取得音樂管理

        List<Screen> screens;         //專門存每個screen的list
       
        Screen current;               //目前只執行的screen
        
        Screen next;                  //下一個準備要被執行的screen 

        float fadeTime = 1.0f;        //screen之間的轉換時間
        float fade = 0.0f;            
        Vector4 fadeColor = Vector4.One;

        //call來的程式碼
        RenderTarget2D colorRT;   // render target for main color buffer
        RenderTarget2D glowRT1;   // render target for glow horizontal blur
        RenderTarget2D glowRT2;   // render target for glow vertical blur

        BlurManager blurManager;     //模糊管理
        //call來的程式碼

        int frameRate;        //FPS資訊
        int frameRateCount;   
        float frameRateTime;  

        Texture2D textureBackground;  //背景圖片
        float backgroundTime = 0.0f;  //背景動畫時間

        //建構子
        public ScreenManager(Game1 mainGame, FontManager font, GameManager game, KinectDisplay kinectDisplay)
        {
            this.mainGame = mainGame;
            gameManager = game;
            fontManager = font;

            screens = new List<Screen>();
            //取得input資訊
            inputManager = new InputManager(kinectDisplay);

            //先把所有的screen創造好
            //給list管理
            screens.Add(new ScreenIntro(this, game));
            screens.Add(new ScreenHelp(this, game));
            screens.Add(new ScreenPlayer(this, game));
            screens.Add(new ScreenLevel(this, game));
            screens.Add(new ScreenGame(this, game));
            screens.Add(new ScreenEnd(this, game));

            //設定好一開始的畫面
            SetNextScreen(ScreenType.ScreenIntro,
                GameOptions.FadeColor, GameOptions.FadeTime);
            fade = fadeTime * 0.5f;
        }

        //Input
        public void ProcessInput(float elapsedTime)
        {
            //傳入輸入為單人模式
            inputManager.BeginInputProcessing(
                gameManager.GameMode == GameMode.SinglePlayer);

            //目前的screen取得input
            if (current != null && next == null)
                current.ProcessInput(elapsedTime, inputManager);

            //按下F5可以改為全螢幕
            if (inputManager.IsKeyPressed(0, Keys.F5) ||
                inputManager.IsKeyPressed(1, Keys.F5))
                mainGame.ToggleFullScreen();

            screens[4].ProcessInput(elapsedTime, inputManager);

            inputManager.EndInputProcessing();
        }

        //Update
        public void Update(float elapsedTime)
        {
            //fade大於0代表畫面正在轉換
            if (fade > 0)
            {
                //畫面傳輸時間
                fade -= elapsedTime;

                //傳輸中
                if (next != null && fade < 0.5f * fadeTime)
                {
                    //告訴下一個畫面準備資源
                    next.SetFocus(contentManager, true);

                    //告訴目前畫面準備卸載資源
                    if (current != null)
                        current.SetFocus(contentManager, false);

                    //傳完
                    current = next;
                    next = null;                   
                }              
                    
            }

            //告訴目前的screen要update
            if (current != null)
                current.Update(elapsedTime);

            //計算FPS
            frameRateTime += elapsedTime;
            if (frameRateTime > 0.5f)
            {
                frameRate = (int)((float)frameRateCount / frameRateTime);
                frameRateCount = 0;
                frameRateTime = 0;
            }

            backgroundTime += elapsedTime;
        }

        //模糊相關計算(call來的)
        void BlurGlowRenterTarget(GraphicsDevice gd)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            gd.DepthStencilState = DepthStencilState.None;          

            // if in game screen and split screen mode
            if (current == ScreenGame &&
                gameManager.GameMode == GameMode.MultiPlayer)
            {
                // blur horizontal with split horizontal blur shader
                gd.SetRenderTarget(glowRT1);
                blurManager.RenderScreenQuad(gd, BlurTechnique.BlurHorizontalSplit,
                            colorRT, Vector4.One);
            }
            else
            {
                // blur horizontal with regular horizontal blur shader
                gd.SetRenderTarget(glowRT1);
                blurManager.RenderScreenQuad(gd, BlurTechnique.BlurHorizontal,
                            colorRT, Vector4.One);
            }

            // blur vertical with regular vertical blur shader
            gd.SetRenderTarget(glowRT2);
            blurManager.RenderScreenQuad(gd, BlurTechnique.BlurVertical,
                            glowRT1, Vector4.One);

            gd.DepthStencilState = DepthStencilState.Default;

            gd.SetRenderTarget(null);
        }

        //著色相關(call來的)
        void DrawRenderTargetTexture(
            GraphicsDevice gd,
            RenderTarget2D renderTarget,
            float intensity,
            bool additiveBlend)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            gd.DepthStencilState = DepthStencilState.None;
            if (additiveBlend)
            {
                gd.BlendState = BlendState.Additive;
            }

            // draw render tareget as fullscreen texture
            blurManager.RenderScreenQuad(gd, BlurTechnique.ColorTexture,
                renderTarget, new Vector4(intensity));

            // restore render state and blend mode
            gd.DepthStencilState = DepthStencilState.Default;

        }

        //畫圖(以overloading實作)
        public void DrawTexture(
            Texture2D texture,
            Rectangle rect,
            Color color,
            BlendState blend)
        {
            fontManager.DrawTexture(texture, rect, color, blend);
        }

        //畫圖
        public void DrawTexture(
            Texture2D texture,
            Rectangle destinationRect,
            Rectangle sourceRect,
            Color color,
            BlendState blend)
        {
            fontManager.DrawTexture(texture, destinationRect, sourceRect, color, blend);
        }

        //畫圖
        public void DrawTexture(
            Texture2D texture,
            Rectangle rect,
            float rotation,
            Color color,
            BlendState blend)
        {
            fontManager.DrawTexture(texture, rect, rotation, color, blend);
        }

        //文字
        public void DrawText(string text, Vector2 position)
        {
          //fontManager.BeginText();
            fontManager.DrawText(FontType.ArialSmall, text, position, Color.Black);
          //fontManager.EndText();
        }

        //畫出背景的動畫
        public void DrawBackground(GraphicsDevice gd)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            const float animationTime = 3.0f;
            const float animationLength = 0.4f;
            const int numberLayers = 2;
            const float layerDistance = 1.0f / numberLayers;

            //正規化時間
            float normalizedTime = ((backgroundTime / animationTime) % 1.0f);

            //設定著色狀態
            DepthStencilState ds = gd.DepthStencilState;
            BlendState bs = gd.BlendState;
            gd.DepthStencilState = DepthStencilState.DepthRead;
            gd.BlendState = BlendState.AlphaBlend;

            float scale;
            Vector4 color;

            //把所有背景層著色
            for (int i = 0; i < numberLayers; i++)
            {
                if (normalizedTime > 0.5f)
                    scale = 2 - normalizedTime * 2;
                else
                    scale = normalizedTime * 2;
                color = new Vector4(scale, scale, scale, 0);

                //把原本scale變小(BY BL 修改)
                scale = 1 + normalizedTime * animationLength * 0.5f;

                blurManager.RenderScreenQuad(gd,
                    BlurTechnique.ColorTexture, textureBackground, color, scale);

                normalizedTime = (normalizedTime + layerDistance) % 1.0f;
            }

            //儲存目前著色狀態
            gd.DepthStencilState = ds;
            gd.BlendState = bs;

        }

        //畫出目前執行的screen
        public void Draw(GraphicsDevice gd)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            //FPS的計數器
            frameRateCount++;

            //為了不讓Screen中的模糊來影響遊戲畫面
            //因此必須用current != screens[4]卡住
            if (current != null && current != screens[4])
            {
                //設定著色
                gd.SetRenderTarget(colorRT);

                //畫出目前screen中的3D
                current.Draw3D(gd);

                //釋放著色
                gd.SetRenderTarget(null);

                //模糊著色
                BlurGlowRenterTarget(gd);

                //畫出背景
                DrawRenderTargetTexture(gd, colorRT, 1.0f, false);

                //畫出背景與blending
                //DrawRenderTargetTexture(gd, glowRT2, 2.0f, true);

                //初始化文字的繪圖方式
                fontManager.BeginText();

                //目前screen中的畫平面(把剛剛文字的繪圖方式傳進去)
                current.Draw2D(gd, fontManager);

                //遊戲偵率
                fontManager.DrawText(
                    FontType.ArialSmall,
                    "FPS: " + frameRate,
                    new Vector2(gd.Viewport.Width - 80, 0), Color.White);

                //還原繪圖裝置
                fontManager.EndText();

                //不是遊戲模式下須調回參數
                //GameOptions.GameNumber = 0;
            }

            //對GameScreen做專門的處裡
            if (current == screens[4])
            {
                //目前screen中的畫平面(把剛剛文字的繪圖方式傳進去)
                current.Draw2D(gd, fontManager);
            }

            //這是在處理畫面轉換而變暗
            if (fade > 0)
            {
                float size = fadeTime * 0.5f;
                fadeColor.W = 1.25f * (1.0f - Math.Abs(fade - size) / size);

                //設定alpha與blend
                gd.DepthStencilState = DepthStencilState.None;
                gd.BlendState = BlendState.AlphaBlend;

                //畫出轉換時漸漸變暗的顏色
                blurManager.RenderScreenQuad(gd, BlurTechnique.Color, null, fadeColor);

                //儲存目前著色狀態
                gd.DepthStencilState = DepthStencilState.Default;
                gd.BlendState = BlendState.Opaque;
            }
           
            //讓遊戲畫面不會被影響
            gd.DepthStencilState = DepthStencilState.Default;
            gd.BlendState = BlendState.AlphaBlend;
        }

        //讀取資源
        public void LoadContent(GraphicsDevice gd,
            ContentManager content)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            contentManager = content;
            textureBackground = content.Load<Texture2D>("screens/background");
            //管理模糊
            blurManager = new BlurManager(gd,
                content.Load<Effect>("shaders/Blur"),
                GameOptions.GlowResolution, GameOptions.GlowResolution);

            int width = gd.Viewport.Width;
            int height = gd.Viewport.Height;


            //創造著色目標
            colorRT = new RenderTarget2D(gd,width, height,
                true, SurfaceFormat.Color, DepthFormat.Depth24);
            glowRT1 = new RenderTarget2D(gd, GameOptions.GlowResolution, GameOptions.GlowResolution,
                true, SurfaceFormat.Color, DepthFormat.Depth24);
            glowRT2 = new RenderTarget2D(gd, GameOptions.GlowResolution, GameOptions.GlowResolution,
                true, SurfaceFormat.Color, DepthFormat.Depth24);

        }

        //Unload
        public void UnloadContent()
        {
            textureBackground = null;
            if (blurManager != null)
            {
                blurManager.Dispose();
                blurManager = null;
            }

            if (colorRT != null)
            {
                colorRT.Dispose();
                colorRT = null;
            }
            if (glowRT1 != null)
            {
                glowRT1.Dispose();
                glowRT1 = null;
            }
            if (glowRT2 != null)
            {
                glowRT2.Dispose();
                glowRT2 = null;
            }
        }

        //轉換到下一個screen
        //利用1秒的fadeTime去做顏色個改變
        public bool SetNextScreen(ScreenType screenType, Vector4 fadeColor,
            float fadeTime)
        {
            //沒有轉換的目標的話
            if (next == null)
            {
                //先設定fadeTime等等狀態
                next = screens[(int)screenType];
                this.fadeTime = fadeTime;
                this.fadeColor = fadeColor;
                this.fade = this.fadeTime;
                return true;
            }
            return false;
        }

        //開始轉換畫面
        //利用1秒的fadeTime去做顏色個改變
        public bool SetNextScreen(ScreenType screenType, Vector4 fadeColor)
        {
            return SetNextScreen(screenType, fadeColor, 1.0f);
        }

        public bool SetNextScreen(ScreenType screenType)
        {
            return SetNextScreen(screenType, Vector4.Zero, 1.0f);
        }

        //取得目前的是甚麼screen
        public Screen GetScreen(ScreenType screenType)
        {
            return screens[(int)screenType];
        }

        public ScreenIntro ScreenIntro
        { get { return (ScreenIntro)screens[(int)ScreenType.ScreenIntro]; } }

        public ScreenIntro ScreenHelp
        { get { return (ScreenIntro)screens[(int)ScreenType.ScreenHelp]; } }

        public ScreenPlayer ScreenPlayer
        { get { return (ScreenPlayer)screens[(int)ScreenType.ScreenPlayer]; } }

        public ScreenLevel ScreenLevel
        { get { return (ScreenLevel)screens[(int)ScreenType.ScreenLevel]; } }

        public ScreenGame ScreenGame
        { get { return (ScreenGame)screens[(int)ScreenType.ScreenGame]; } }

        public ScreenEnd ScreenEnd
        { get { return (ScreenEnd)screens[(int)ScreenType.ScreenEnd]; } }

        public void Exit() { mainGame.Exit(); }

        //釋放資源
        #region IDisposable Members

        bool isDisposed = false;
        public bool IsDisposed
        {
            get { return isDisposed; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposing && !isDisposed)
            {
                UnloadContent();
            }
        }
        #endregion
    }
}
