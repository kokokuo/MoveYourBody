

//*****************
//blog.arbuzz.eu
//*****************


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
using OpenNI;
using System.Threading;


namespace MrozKinect
{
    public class MrKinect : IDisposable
   { 

        public MrKinect(Game game)
        {
            this.game = game;
        }

        
        private Game game;
        internal Context context;
        private readonly string SAMPLE_XML_FILE = @"C://Users//NXT-1323//Desktop//Game_Kinect_Project_v15_8_29//Game_Kinect_Project//Game_Kinect_Project//MrozKinect";

        DepthGenerator depth;
        private ImageGenerator image;
        

        private bool shouldRun = true;
        private Thread readerThread;
        //private DepthMetaData depthMD = new DepthMetaData();
                
        public Texture2D colorTexture;
        public Texture2D depthTexture;

        public int frameNr = 0;

        private bool newFrame;
        public bool NewFrame
        {
            get { return newFrame; }
            set { newFrame = value; }
        }

       

       

        public Texture2D prevColorTexture;
        
        public void StartKinect(DepthGenerator depth)
        {
                      
           // Joint.depth = depth;
            //StartReaderThread();           
        }

        private void StartReaderThread()
        {
            readerThread = new Thread(ReaderThread);
            readerThread.Start();
            
        }
       

        private void ReaderThread()
        {
            while (this.shouldRun)
            {
                try
                {
                    this.context.WaitOneUpdateAll(this.depth);
                }
                catch (Exception)
                {
                }
                //this.depth.GetMetaData(depthMD);
                frameNr++;
                newFrame = true;
            }
        }

        public void Dispose()
        {
            this.shouldRun = false;
            if (readerThread != null)
                this.readerThread.Join();
        }

        public void GetKinectData()
        {
            if (newFrame)
            {
                newFrame = false;
                depthTexture = GetDepthTexture();
                colorTexture = GetColorTexture();
                //userTexture  = GetUserTexture();
            }
        }

        public Texture2D GetColorTexture()
        {
            return null;// simpleCV.GetColorTex(image.GetImageMapPtr(), game);            
        }

        private Texture2D GetDepthTexture()
        {
            return null;//simpleCV.GetDepthTex(depth.GetDepthMapPtr(), game);
        }

        private void FreeTexture(Texture2D tex)
        {
            if (game.GraphicsDevice != null)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (game.GraphicsDevice.Textures[i] == tex)
                    {
                        game.GraphicsDevice.Textures[i] = null;
                        break;
                    }
                }
            }
        }
    }
}
