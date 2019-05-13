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
        GameManager gameManager;             //���o�C��������T
        FontManager fontManager;             //�r��޲z
        InputManager inputManager;           //���oinput������T
        ContentManager contentManager;       //XNA���ت�Ū���귽�޲z
        //AudioManager audioManager;          //���o���ֺ޲z

        List<Screen> screens;         //�M���s�C��screen��list
       
        Screen current;               //�ثe�u���檺screen
        
        Screen next;                  //�U�@�ӷǳƭn�Q���檺screen 

        float fadeTime = 1.0f;        //screen�������ഫ�ɶ�
        float fade = 0.0f;            
        Vector4 fadeColor = Vector4.One;

        //call�Ӫ��{���X
        RenderTarget2D colorRT;   // render target for main color buffer
        RenderTarget2D glowRT1;   // render target for glow horizontal blur
        RenderTarget2D glowRT2;   // render target for glow vertical blur

        BlurManager blurManager;     //�ҽk�޲z
        //call�Ӫ��{���X

        int frameRate;        //FPS��T
        int frameRateCount;   
        float frameRateTime;  

        Texture2D textureBackground;  //�I���Ϥ�
        float backgroundTime = 0.0f;  //�I���ʵe�ɶ�

        //�غc�l
        public ScreenManager(Game1 mainGame, FontManager font, GameManager game, KinectDisplay kinectDisplay)
        {
            this.mainGame = mainGame;
            gameManager = game;
            fontManager = font;

            screens = new List<Screen>();
            //���oinput��T
            inputManager = new InputManager(kinectDisplay);

            //����Ҧ���screen�гy�n
            //��list�޲z
            screens.Add(new ScreenIntro(this, game));
            screens.Add(new ScreenHelp(this, game));
            screens.Add(new ScreenPlayer(this, game));
            screens.Add(new ScreenLevel(this, game));
            screens.Add(new ScreenGame(this, game));
            screens.Add(new ScreenEnd(this, game));

            //�]�w�n�@�}�l���e��
            SetNextScreen(ScreenType.ScreenIntro,
                GameOptions.FadeColor, GameOptions.FadeTime);
            fade = fadeTime * 0.5f;
        }

        //Input
        public void ProcessInput(float elapsedTime)
        {
            //�ǤJ��J����H�Ҧ�
            inputManager.BeginInputProcessing(
                gameManager.GameMode == GameMode.SinglePlayer);

            //�ثe��screen���oinput
            if (current != null && next == null)
                current.ProcessInput(elapsedTime, inputManager);

            //���UF5�i�H�אּ���ù�
            if (inputManager.IsKeyPressed(0, Keys.F5) ||
                inputManager.IsKeyPressed(1, Keys.F5))
                mainGame.ToggleFullScreen();

            screens[4].ProcessInput(elapsedTime, inputManager);

            inputManager.EndInputProcessing();
        }

        //Update
        public void Update(float elapsedTime)
        {
            //fade�j��0�N��e�����b�ഫ
            if (fade > 0)
            {
                //�e���ǿ�ɶ�
                fade -= elapsedTime;

                //�ǿ餤
                if (next != null && fade < 0.5f * fadeTime)
                {
                    //�i�D�U�@�ӵe���ǳƸ귽
                    next.SetFocus(contentManager, true);

                    //�i�D�ثe�e���ǳƨ����귽
                    if (current != null)
                        current.SetFocus(contentManager, false);

                    //�ǧ�
                    current = next;
                    next = null;                   
                }              
                    
            }

            //�i�D�ثe��screen�nupdate
            if (current != null)
                current.Update(elapsedTime);

            //�p��FPS
            frameRateTime += elapsedTime;
            if (frameRateTime > 0.5f)
            {
                frameRate = (int)((float)frameRateCount / frameRateTime);
                frameRateCount = 0;
                frameRateTime = 0;
            }

            backgroundTime += elapsedTime;
        }

        //�ҽk�����p��(call�Ӫ�)
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

        //�ۦ����(call�Ӫ�)
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

        //�e��(�Hoverloading��@)
        public void DrawTexture(
            Texture2D texture,
            Rectangle rect,
            Color color,
            BlendState blend)
        {
            fontManager.DrawTexture(texture, rect, color, blend);
        }

        //�e��
        public void DrawTexture(
            Texture2D texture,
            Rectangle destinationRect,
            Rectangle sourceRect,
            Color color,
            BlendState blend)
        {
            fontManager.DrawTexture(texture, destinationRect, sourceRect, color, blend);
        }

        //�e��
        public void DrawTexture(
            Texture2D texture,
            Rectangle rect,
            float rotation,
            Color color,
            BlendState blend)
        {
            fontManager.DrawTexture(texture, rect, rotation, color, blend);
        }

        //��r
        public void DrawText(string text, Vector2 position)
        {
          //fontManager.BeginText();
            fontManager.DrawText(FontType.ArialSmall, text, position, Color.Black);
          //fontManager.EndText();
        }

        //�e�X�I�����ʵe
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

            //���W�Ʈɶ�
            float normalizedTime = ((backgroundTime / animationTime) % 1.0f);

            //�]�w�ۦ⪬�A
            DepthStencilState ds = gd.DepthStencilState;
            BlendState bs = gd.BlendState;
            gd.DepthStencilState = DepthStencilState.DepthRead;
            gd.BlendState = BlendState.AlphaBlend;

            float scale;
            Vector4 color;

            //��Ҧ��I���h�ۦ�
            for (int i = 0; i < numberLayers; i++)
            {
                if (normalizedTime > 0.5f)
                    scale = 2 - normalizedTime * 2;
                else
                    scale = normalizedTime * 2;
                color = new Vector4(scale, scale, scale, 0);

                //��쥻scale�ܤp(BY BL �ק�)
                scale = 1 + normalizedTime * animationLength * 0.5f;

                blurManager.RenderScreenQuad(gd,
                    BlurTechnique.ColorTexture, textureBackground, color, scale);

                normalizedTime = (normalizedTime + layerDistance) % 1.0f;
            }

            //�x�s�ثe�ۦ⪬�A
            gd.DepthStencilState = ds;
            gd.BlendState = bs;

        }

        //�e�X�ثe���檺screen
        public void Draw(GraphicsDevice gd)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            //FPS���p�ƾ�
            frameRateCount++;

            //���F����Screen�����ҽk�Ӽv�T�C���e��
            //�]��������current != screens[4]�d��
            if (current != null && current != screens[4])
            {
                //�]�w�ۦ�
                gd.SetRenderTarget(colorRT);

                //�e�X�ثescreen����3D
                current.Draw3D(gd);

                //����ۦ�
                gd.SetRenderTarget(null);

                //�ҽk�ۦ�
                BlurGlowRenterTarget(gd);

                //�e�X�I��
                DrawRenderTargetTexture(gd, colorRT, 1.0f, false);

                //�e�X�I���Pblending
                //DrawRenderTargetTexture(gd, glowRT2, 2.0f, true);

                //��l�Ƥ�r��ø�Ϥ覡
                fontManager.BeginText();

                //�ثescreen�����e����(�����r��ø�Ϥ覡�Ƕi�h)
                current.Draw2D(gd, fontManager);

                //�C�����v
                fontManager.DrawText(
                    FontType.ArialSmall,
                    "FPS: " + frameRate,
                    new Vector2(gd.Viewport.Width - 80, 0), Color.White);

                //�٭�ø�ϸ˸m
                fontManager.EndText();

                //���O�C���Ҧ��U���զ^�Ѽ�
                //GameOptions.GameNumber = 0;
            }

            //��GameScreen���M�����B��
            if (current == screens[4])
            {
                //�ثescreen�����e����(�����r��ø�Ϥ覡�Ƕi�h)
                current.Draw2D(gd, fontManager);
            }

            //�o�O�b�B�z�e���ഫ���ܷt
            if (fade > 0)
            {
                float size = fadeTime * 0.5f;
                fadeColor.W = 1.25f * (1.0f - Math.Abs(fade - size) / size);

                //�]�walpha�Pblend
                gd.DepthStencilState = DepthStencilState.None;
                gd.BlendState = BlendState.AlphaBlend;

                //�e�X�ഫ�ɺ����ܷt���C��
                blurManager.RenderScreenQuad(gd, BlurTechnique.Color, null, fadeColor);

                //�x�s�ثe�ۦ⪬�A
                gd.DepthStencilState = DepthStencilState.Default;
                gd.BlendState = BlendState.Opaque;
            }
           
            //���C���e�����|�Q�v�T
            gd.DepthStencilState = DepthStencilState.Default;
            gd.BlendState = BlendState.AlphaBlend;
        }

        //Ū���귽
        public void LoadContent(GraphicsDevice gd,
            ContentManager content)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            contentManager = content;
            textureBackground = content.Load<Texture2D>("screens/background");
            //�޲z�ҽk
            blurManager = new BlurManager(gd,
                content.Load<Effect>("shaders/Blur"),
                GameOptions.GlowResolution, GameOptions.GlowResolution);

            int width = gd.Viewport.Width;
            int height = gd.Viewport.Height;


            //�гy�ۦ�ؼ�
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

        //�ഫ��U�@��screen
        //�Q��1��fadeTime�h���C��ӧ���
        public bool SetNextScreen(ScreenType screenType, Vector4 fadeColor,
            float fadeTime)
        {
            //�S���ഫ���ؼЪ���
            if (next == null)
            {
                //���]�wfadeTime�������A
                next = screens[(int)screenType];
                this.fadeTime = fadeTime;
                this.fadeColor = fadeColor;
                this.fade = this.fadeTime;
                return true;
            }
            return false;
        }

        //�}�l�ഫ�e��
        //�Q��1��fadeTime�h���C��ӧ���
        public bool SetNextScreen(ScreenType screenType, Vector4 fadeColor)
        {
            return SetNextScreen(screenType, fadeColor, 1.0f);
        }

        public bool SetNextScreen(ScreenType screenType)
        {
            return SetNextScreen(screenType, Vector4.Zero, 1.0f);
        }

        //���o�ثe���O�ƻ�screen
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

        //����귽
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
