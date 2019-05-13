#region File Description
//-----------------------------------------------------------------------------
// Screen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion



namespace Game_Kinect_Project
{
    public enum ScreenType
    {
        ScreenIntro = 0,
        ScreenHelp,
        ScreenPlayer,
        ScreenLevel,
        ScreenGame,
        ScreenEnd
    };

    /// <summary>
    /// 每個screen的父類別
    /// </summary>
    public abstract class Screen
    {
        public abstract void SetFocus(ContentManager content, bool focus);

        public abstract void ProcessInput(float elapsedTime, InputManager input);

        public abstract void Update(float elapsedTime);

        public abstract void Draw3D(GraphicsDevice gd);

        public abstract void Draw2D(GraphicsDevice gd, FontManager font);
    }
}
