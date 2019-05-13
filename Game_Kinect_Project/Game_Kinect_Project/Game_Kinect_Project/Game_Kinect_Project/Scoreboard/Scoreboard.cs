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

namespace Game_Kinect_Project.Scoreboard
{
    //專門處理計分板資訊
    class Scoreboard : DrawableGameComponent
    {
        ContentManager content;
        SpriteBatch spriteBatch;       

        int bbWidth, bbHeight;

        bool scoreBoardEnable = false;
        public bool ScoreBoardEnable { get { return scoreBoardEnable; } set { scoreBoardEnable = value; } }

        public Scoreboard(Game game, GameSpace.GameSpace2 gm2)
            : base(game)
        {
            content = new ContentManager(game.Services);
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
        }
       
        protected override void UnloadContent()
        {
                content.Unload();
        }

    
    }
}
