#region File Description
//-----------------------------------------------------------------------------
// GameOptions.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace Game_Kinect_Project
{
    public class GameOptions
    {
        //遊戲將來的設定值
        public static bool Effect = true;

        //遊戲的關卡
        public static int GameNumber = 1;

        //遊戲時間增加維度
        public static int GameTimeStep = 1;

        //gameScreen水平長度
        public static int ScreenWidth = 1280;

        //gameScreen垂直長度
        public static int ScreenHeight = 720;

        // glowBuffer
        public static int GlowResolution = 512;

        //最多可以的玩家數
        public static int MaxPlayers = 2;
        
        //傳輸顏色
        public static Vector4 FadeColor = Vector4.Zero;

        //screen的傳輸時間
        public static float FadeTime = 0.5f;

        //怪物出現的時間
        public static int BeastStartTime = 600;

        //開始遊戲的時間
        public static int PlayerStartTime = 1000;

        //是否正在怪物的畫面
        public static bool DuringBeastTime = false;

        //是否在遊戲開始
        public static bool StartRunTime = false;

        //玩家的生命
        public static int PlayerLive = 200;

        //被阻擋的時間
        public static int MaxBlockedTime = 60;

        //是否處於gameOver
        public static bool GameOver = false;

        //終點
        public static float EndPoint = -1200;

        //吃到的金幣
        public static int GoldEarn = 0;

        //過關
        public static bool GameFinish = false;

        //////////////////2//////////////////////

        //板子通過以後
        public static bool isPass = false;

        //通過
        public static bool isBodyPass = false;

        //////////////////通用//////////////////////////////////
        //////////////////通用//////////////////////////////////
        //////////////////通用//////////////////////////////////
        public static bool isCatchPic = false;

        public static bool isShowPic = false;

        public static bool isPlayerScreen = false;

        public static bool isLevelScreen = false;

        public static bool isLevelsScreen = false;

        public static bool isIntroScreen = false;

        public static bool isHelpScreen = false;

        //public static bool isGameScreen = false;

        public static bool isGameScreen = false;

        public static bool HDR = false;

        public static bool GM2_Over = false;

        public static bool GM1_Over = false;

        public static bool CatchFace = false;

        public static int passCount = 0;

        public static bool isQuit = false;

        public static bool isUpload = false;
    }
}
