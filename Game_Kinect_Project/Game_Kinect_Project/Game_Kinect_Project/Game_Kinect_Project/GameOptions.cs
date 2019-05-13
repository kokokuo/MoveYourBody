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
        //�C���N�Ӫ��]�w��
        public static bool Effect = true;

        //�C�������d
        public static int GameNumber = 1;

        //�C���ɶ��W�[����
        public static int GameTimeStep = 1;

        //gameScreen��������
        public static int ScreenWidth = 1280;

        //gameScreen��������
        public static int ScreenHeight = 720;

        // glowBuffer
        public static int GlowResolution = 512;

        //�̦h�i�H�����a��
        public static int MaxPlayers = 2;
        
        //�ǿ��C��
        public static Vector4 FadeColor = Vector4.Zero;

        //screen���ǿ�ɶ�
        public static float FadeTime = 0.5f;

        //�Ǫ��X�{���ɶ�
        public static int BeastStartTime = 600;

        //�}�l�C�����ɶ�
        public static int PlayerStartTime = 1000;

        //�O�_���b�Ǫ����e��
        public static bool DuringBeastTime = false;

        //�O�_�b�C���}�l
        public static bool StartRunTime = false;

        //���a���ͩR
        public static int PlayerLive = 200;

        //�Q���ת��ɶ�
        public static int MaxBlockedTime = 60;

        //�O�_�B��gameOver
        public static bool GameOver = false;

        //���I
        public static float EndPoint = -1200;

        //�Y�쪺����
        public static int GoldEarn = 0;

        //�L��
        public static bool GameFinish = false;

        //////////////////2//////////////////////

        //�O�l�q�L�H��
        public static bool isPass = false;

        //�q�L
        public static bool isBodyPass = false;

        //////////////////�q��//////////////////////////////////
        //////////////////�q��//////////////////////////////////
        //////////////////�q��//////////////////////////////////
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
