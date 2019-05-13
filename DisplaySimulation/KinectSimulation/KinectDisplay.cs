using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;
//////////////////////////////////////////
//銜接Kinect訊號要加入的命名空間
using OpenNI;
using KinectModel;
using KinectModel.BehaviorStruct;
using KinectModel.StateEnum;
/////////////////////////////////////////


namespace KinectSimulation
{
    /// <summary>
    /// 此類別是Kinect訊號的測試畫面
    /// </summary>
    public partial class KinectDisplay : Form
    {
        #region Member variable
        ////////////////////////////////////////////////////////////////////////

        //record the width and height of the Form screen transforming screen size 
        private double oldWidth = new double();
        private double oldHeight = new double();

        //record the depth data to show image
        private Bitmap bitmap;

        //depth histogram
        private int[] histogram;

        //use thread to calculate kinect data
        private Thread KinectWorkerThread;

        //use the color to show user
        private Color[] userColors = { Color.Red, Color.Blue, Color.ForestGreen, Color.Yellow, Color.Orange, Color.Purple, Color.White };
        private Color[] nodeColors = { Color.Green, Color.Orange, Color.Red, Color.Purple, Color.Blue, Color.Yellow, Color.Black };
        private int ncolors = 6;

        //record user number
        private int userId;

        //boolean signal
        private bool shouldDrawPixels = true;
        private bool shouldDrawBackground = true;
        private bool shouldPrintID = true;
        private bool shouldPrintState = true;
        private bool shouldDrawSkeleton = true;


        //銜接Kinect訊號要用到的類別,裡面包含了許多的訊號事件註冊
        KinectActionModel KinectAction;

        //記錄Kinect訊號的資料結構型態,此為參考,主要是知道他的形態即可
        ////////////////////////////////////////////////////////
        //骨架
        Dictionary<SkeletonJoint, SkeletonJointPosition> drawSk;
        //揮砍的資料型態
        SlashBehaviorData rightHandSlash;
        //揮砍的角度
        double slashAngle;

        //Push的資料型態
        PushBehaviorData rightHandPush;
        PushBehaviorData leftHandPush;
        //跳躍的資料型態
        JumpBehaviorData jumpData;
        //飛行的資料型態
        FlyBehaviorData flyData;
        //紀錄可執行飛行訊號的範圍資料型態
        FlyRangeBoxData rangeRuleBox;
        //Swipe的資料型態
        SwipeBehaviorData swipeData;

        //左右閃躲移動的資料型態
        MoveBehaviorData moveData;

        //紀錄跑步每秒踏的步數
        private double runTimesForSpeed;   
        //紀錄等待進入Pause的時間
        private double pauseWaitingTime;
        /////////////////////////////////////////////////////////////
        //這邊的布林型態是為了畫畫面時的所作的開關
        private bool isGetJoints;           //set the joint data is not null
        private int  lostNumber;            //show the lost user number
        private bool isRightHandSlash;
        private bool isRightHandPush;
        private bool isLeftHandPush;
        private bool isJump;
        private bool isRun;

        //是否進入等待Pause
        private bool isWaitingPause;
        //是否Pause
        private bool isPause;
        //
        private bool isFly;
        //是否已設置了飛行Box的資料
        private bool isSetFlyRangeBox;
        private bool isSwipe;

        //是否左或右移
        private bool isMove;
        ///////////////////////////////////////
        //紀錄Pause時 和肩膀的水平夾角及和肩膀Z軸方向的夾角->測試用
        private double verticalAngle;
        private double horizonAngle;
        
        //紀錄飛行時 符合飛行軌則的手肘角度->測試用
        private double leftAngle;
        private double rightAngle;
        
        //紀錄Swipe的訊息
        private string swipeMessgae;
        //紀錄Swipe時和Y軸方向的向量夾角->測試用
        private double swipeUpDownHorizonAngle;
        //紀錄Swipe時和X軸方向的向量夾角->測試用
        private double swipeRightLeftHorizonAngle;
        //紀錄Swipe時和Z軸方向的向量夾角->測試用
        private double swipeVerticalAngle;

        //紀錄Fly的訊息
        private string flyMessgae;
        ////////////////////////////////////////////////////////////////////////
        #endregion     

        public KinectDisplay()
        {
            InitializeComponent();
            
            #region KinectPart->包含了須多事件註冊
            ////////////////////////////////////////////////////////////////////////
            KinectAction = new KinectActionModel();
            KinectAction.ShouldRun = true;
            KinectAction.OnReset +=new NoBehaviorEventHandler(KinectAction_OnReset);
            drawSk = new Dictionary<SkeletonJoint, SkeletonJointPosition>();

            //這邊為骨架更新註冊及深度更新註冊(深度更新部分遊戲應該用不到)
            ///////////////////////////////////////////////
            KinectAction.OnDepthMDUpdate += new DepthUpdateEventHandler(KinectAction_OnDepthMDUpdate);
            KinectAction.OnJointsUpdate += new SkeletonJointUpdateEventHandler(KinectAction_OnJointsUpdate);
            isGetJoints = false;

            ///////////////////////////////////////////////

            //Menu部分可能會用到的事件->Push用做選擇,Swipe或許可以用來做一些捲動式畫面選擇
            //////////////////////////////////////////////
            KinectAction.OnHandPush += new PushBehaviorEventHandler(KinectAction_OnHandPush);
            KinectAction.OnHandPushStable += new NoBehaviorMessageEventHandler(KinectAction_OnHandPushStable);
            KinectAction.OnSwipe += new SwipeBehaviorEventHandler(KinectAction_OnSwipe);
            KinectAction.OnNotSwipe += new NoBehaviorMessageEventHandler(KinectAction_OnNotSwipe);
           
            //OnSwipeAngleChanged只是測試得知角度用,不須註冊
            KinectAction.OnSwipeAngleChanged += new TestAngleChangedEventHandler(KinectAction_OnSwipeAngleChanged);
            swipeUpDownHorizonAngle = new double();
            swipeRightLeftHorizonAngle = new double();
            swipeVerticalAngle = new double();
            swipeMessgae = "";
            isSwipe = false;
            /////////////////////////////////////////////

            //遊戲中會用到的Pause事件
            ///////////////////////////////////////////////

            //OnAngelInvoke只是測試得知角度用,不須註冊
            KinectAction.OnPauseAngleChanged += new TestAngleChangedEventHandler(KinectAction_OnAngelInvoke);
            
            KinectAction.OnPause += new PauseBehaviorEventHandler(KinectAction_OnPause);
            KinectAction.OnWaitingPause += new TimesChangedEventHandler(KinectAction_OnWaitingPause);
            KinectAction.OnNotPause += new NoBehaviorEventHandler(KinectAction_OnNotPause);
            pauseWaitingTime = 0;
            isPause = false;
            isWaitingPause = false;
            verticalAngle = new double();
            horizonAngle = new double();
            ///////////////////////////////////////////////

            
            ///////////////////////////////////////////////
            //跳躍和不跳時的事件
            KinectAction.OnJump += new JumpBehaviorEventHandler(KinectAction_OnJump);
            KinectAction.OnJumpStable += new NoBehaviorEventHandler(KinectAction_OnJumpStable);
            ////揮砍和沒揮砍時的事件註冊
            //KinectAction.OnHandSlash += new SlashBehaviorEventHandler(KinectAction_OnHandSlash);
            //KinectAction.OnHandSlashStable += new NoBehaviorEventHandler(KinectAction_OnHandSlashStable);
            ////跑步何沒跑步時的事件註冊
            KinectAction.OnFootRun += new TimesChangedEventHandler(KinectAction_OnFootRun);
            KinectAction.OnFootRunStable += new NoBehaviorEventHandler(KinectAction_OnFootRunStable);
            //左右移動的事件註冊
            KinectAction.OnMoveLeftOrRight += new MoveLeftAndRightEventHandler(KinectAction_OnMoveLeftOrRight);
            KinectAction.OnMoveLeftOrRightStable += new NoBehaviorEventHandler(KinectAction_OnMoveLeftOrRightStable);
            
            //////////////////////////////
            //test
            KinectAction.OnSlashAngleChanged +=new TestAngleChangedEventHandler(KinectAction_OnSlashAngleChanged);
            slashAngle = new double();
           
            isMove = false;
            isRightHandSlash = false;
            isRightHandPush = false;
            isLeftHandPush = false;
            isJump = false;
            isRun = false;
            ///////////////////////////////////////////////

            //飛行關卡->若要觀看訊號,則不要同時開啟飛行和叢林逃跑關卡的訊號,否則畫面會有點亂...
            ///////////////////////////////////////////////
            //KinectAction.OnFly += new FlyBehaviorEventHandler(KinectAction_OnFly);
            //KinectAction.OnBoxChanged += new FlyRangeBoxChanged(KinectAction_OnBoxChanged);
            //KinectAction.OnNotFly += new NoBehaviorMessageEventHandler(KinectAction_OnNotFly);

            //.OnFlyAngleChanged為測試得知角度用不需註冊
            //KinectAction.OnFlyAngleChanged += new TestAngleChangedEventHandler(KinectAction_OnFlyRuleAngleChanged);
            
            rangeRuleBox = new FlyRangeBoxData();
            leftAngle = new double();
            rightAngle = new double();
            flyMessgae = "";
            isFly = false;
            isSetFlyRangeBox = false;
            ///////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////////
            #endregion

            //set the form size
            oldWidth = this.Width;
            oldHeight = this.Height;


            //set histogram size 
            this.histogram = new int[this.KinectAction.GetDevicetMaxDepth()];
            //screen size
            this.Resize += new EventHandler(KinectDisplay_Resize);
            this.bitmap = new Bitmap((int)this.KinectAction.GetMapModeData.XRes, (int)this.KinectAction.GetMapModeData.YRes, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Console.WriteLine("xres = {0}, yres ={1}", this.KinectAction.GetMapModeData.XRes, this.KinectAction.GetMapModeData.YRes);
            #region kinectProcess thread invoke->需要對ModelDoWork()開啟執行緒才會使Kinect行為訊號運行計算及判斷事件通知
            ////////////////////////////////////////////////////////////////////////

            this.KinectWorkerThread = new Thread(new ThreadStart(KinectAction.ModelDoWork));
            this.KinectWorkerThread.Start();

            ////////////////////////////////////////////////////////////////////////
            #endregion
        }

        #region kinectAction event handler->此部分是參考應用,主要是知道處理時要哪些引數即可
        ////////////////////////////////////////////////////////////////////////
        private void KinectAction_OnReset()
        {
            //這邊的布林型態是為了畫畫面時的所作的開關
            isGetJoints = false;           //set the joint data is not null
            drawSk.Clear();
            isRightHandSlash = false;
            isRightHandPush = false;
            isLeftHandPush = false;
            isJump = false;
            isRun = false;
            //是否進入等待Pause
            isWaitingPause = false;
            //是否Pause
            isPause = false;
            isFly = false;
            //是否已設置了飛行Box的資料
            isSetFlyRangeBox = false;
            isSwipe = false;
            //是否左或右移
            isMove = false;
            pauseWaitingTime = 0;
        }
        
        private void KinectAction_OnSlashAngleChanged(double angle, string message)
        {
            slashAngle = angle; 
        }
        private void KinectAction_OnMoveLeftOrRight(MoveBehaviorData move) 
        {
            moveData = move;
            isMove = true;
        }
        private void KinectAction_OnMoveLeftOrRightStable()
        {
            isMove = false;
        }
        private void KinectAction_OnNotPause()
        {
            isWaitingPause = false;
            pauseWaitingTime = 0;
        }
        private void KinectAction_OnWaitingPause(double waitingTime)
        {
            isWaitingPause = true;
            pauseWaitingTime = waitingTime;
        }
        private void KinectAction_OnSwipeAngleChanged(double angle, string message)
        {
            if (message == "SwipeUpDownAngleBetweenHorizonOrientation") swipeUpDownHorizonAngle = angle;
            if (message == "AngleBetweenVerticalOrientation") swipeVerticalAngle = angle;
            if (message == "SwipeRightLeftAngleBetweenHorizonOrientation") swipeRightLeftHorizonAngle = angle;
        }
        private void KinectAction_OnSwipe(SwipeBehaviorData swipe)
        {
            swipeData = swipe;
            isSwipe = true;
        }
        private void KinectAction_OnNotSwipe(string message)
        {
            swipeMessgae = message;
            isSwipe = false;
        }
        private void KinectAction_OnNotFly(string message)
        {
            //不再範圍內,所以不會更新資料,範圍的顯示也要先鎖起來
            if (message == FlyBehaviorStateMessageEnum.User_Z_Position_Over_TwoDotFour_Meter.ToString())
            {
                isSetFlyRangeBox = false;
            }
            flyMessgae = message;
            isFly = false;
        }
        private void KinectAction_OnFlyRuleAngleChanged(double angle, string whichAngle)
        {
            if (whichAngle == "LeftPart") leftAngle = angle;
            if (whichAngle == "RightPart") rightAngle = angle;
        }
        private void KinectAction_OnBoxChanged(FlyRangeBoxData rangeBox)
        {
            isSetFlyRangeBox = true;
            rangeRuleBox = rangeBox;
        }
        private void KinectAction_OnFly(FlyBehaviorData fly)
        {
            flyData = fly;
            isFly = true;
        }
        private void KinectAction_OnAngelInvoke(double angle, string whichAngle)
        {
            if (whichAngle == "vertical")
                verticalAngle = angle;
            if (whichAngle == "horizon")
                horizonAngle = angle;
        }
        private void KinectAction_OnPause(double vertical, double horizon)
        {
            isPause = true;
            verticalAngle = vertical;
            horizonAngle = horizon;
        }

        private void KinectAction_OnFootRun(double runTimes)
        {
            isRun = true;
            runTimesForSpeed = runTimes;
        }
        private void KinectAction_OnFootRunStable()
        {
            isRun = false;
        }

        private void KinectAction_OnHandPushStable(string hand)
        {
            if (hand == "RightHand") isRightHandPush = false;
            else if (hand == "LeftHand") isLeftHandPush = false;
        }
        private void KinectAction_OnHandPush(PushBehaviorData push)
        {
            
            if (push.Hand == "RightHand")
            {
                isRightHandPush = true;
                rightHandPush = push;
                //--close face image function 
                //Bitmap faceImage = KinectAction.GetFaceBitmap();
                //SaveToJpeg.SaveJpeg("faceROI00.jpg",faceImage,100);
            }
            else if (push.Hand == "LeftHand")
            {
                isLeftHandPush = true;
                leftHandPush = push;
            }
        }
        private void KinectAction_OnJump(JumpBehaviorData jump)
        {
            jumpData = jump;
            isJump = true;
        }
        private void KinectAction_OnJumpStable()
        {
            isJump = false;
        }
        private void KinectAction_OnHandSlash(SlashBehaviorData slash)
        {
           
                isRightHandSlash = true;
                rightHandSlash = slash;
          
        }
        private void KinectAction_OnHandSlashStable()
        {
            isRightHandSlash = false;
        }
                                                              
        private void KinectAction_OnJointsUpdate(Dictionary<SkeletonJoint, SkeletonJointPosition> pos)
        {
            drawSk = pos;
            isGetJoints = true;
        }

        int userNowID = 0;
        public int UserNowID { get { return userNowID; } }
        private unsafe void KinectAction_OnDepthMDUpdate(DepthMetaData depthMD)
        {
            ///////////////////////////////////////////////////////////
            // lock ensures that one thread does not enter a critical section 
            // while another thread is in the critical section of code. 
            // If another thread attempts to enter a locked code, 
            // it will wait (block) until the object is released.
            ///////////////////////////////////////////////////////////
            lock (this)
            {
                CalcHist(depthMD);
                if (this.shouldDrawPixels)
                {
                    CalculateDepthColor(this.userColors, this.ncolors, depthMD, this.bitmap);
                }

                Graphics g = Graphics.FromImage(this.bitmap);
                int[] users = this.KinectAction.GetUsers();
                foreach (int user in users)
                {
                    //if the user is in recored
                    if (KinectAction.UserCurrentState.ContainsKey(user))
                    {
                        if (this.shouldPrintID)
                        {

                            //取得user的中心座標
                            Point3D realCom = this.KinectAction.GetUserPosition(user);
                            Point3D com = this.KinectAction.ConvertRealWorldToProjective(realCom);
                            //對應到平面空間的像素點座標

                            userId = user;
                            DrawState(user, g, com, realCom);

                        }
                        //i think i lock the tracking resource at the certain user, so properbaly ok....
                        if (drawSk!= null && this.shouldDrawSkeleton && isGetJoints && (KinectAction.UserCurrentState[user] == UserStateEnum.Tracking || KinectAction.UserCurrentState[user] == UserStateEnum.TrackingButWaring))
                        {
                            DrawSkeleton(g, nodeColors[userId % ncolors], user);
                            DrawActionState(g);
                            userNowID = user;
                        }
                    }
                }
                g.Dispose();
            }
            this.Invalidate();

        }
        ////////////////////////////////////////////////////////////////////////
        #endregion
        
        //此函式:畫出抓到骨架後,做出行為訊號的畫面顯示
        private void DrawActionState(Graphics g)
        {
            ///////////////////////////////////////////////////////////////////////////
            //push
            if (isRightHandPush)
            {
                Point3D proj = KinectAction.ConvertRealWorldToProjective(rightHandPush.RealHandPoint);
                g.FillEllipse(new SolidBrush((Color.Yellow)), new Rectangle((int)proj.X - 5, (int)proj.Y - 5, 10, 10));
                g.FillRectangle(new SolidBrush(Color.FromArgb(98, Color.Green)), 200, 200, 220, 150);
                g.DrawString("right hand push angle", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 220);
                g.DrawString(rightHandPush.PushAngle.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 240);
            }
            if (isLeftHandPush)
            {
                Point3D proj = KinectAction.ConvertRealWorldToProjective(leftHandPush.RealHandPoint);
                g.FillEllipse(new SolidBrush((Color.Yellow)), new Rectangle((int)proj.X - 5, (int)proj.Y - 5, 10, 10));
                g.FillRectangle(new SolidBrush(Color.FromArgb(98, Color.Green)), 200, 200, 220, 150);
                g.DrawString("left hand push angle", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 280);
                g.DrawString(leftHandPush.PushAngle.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 300);
            }
            ///////////////////////////////////////////////////////////////////////////
            //jump
            if (isJump) //draw the displacement of record
            {
                Point3D proj1 = KinectAction.ConvertRealWorldToProjective(jumpData.RealPointRecorder.First.Value.Position);
                g.FillRectangle(new SolidBrush(Color.FromArgb(98, Color.Blue)), 200, 200, 220, 150);
                g.DrawString("Jump!", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 200);
                g.DrawString("Displacement:" + jumpData.YOriDisplacement.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 220);
                g.DrawString("Velocity:" + jumpData.YOriVelocity.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 240);
                foreach (SkeletonJointPosition p in jumpData.RealPointRecorder)
                {
                    Point3D proj2 = KinectAction.ConvertRealWorldToProjective(p.Position);
                    g.DrawLine(new Pen((Color.Yellow), 8), new Point((int)proj1.X, (int)proj1.Y), new Point((int)proj2.X, (int)proj2.Y));
                    proj1 = proj2;
                }
            }
            ///////////////////////////////////////////////////////////////////////////
            //move left or right
            if (isMove) //draw the displacement of record
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(98, Color.Blue)), 200, 200, 220, 150);
                g.DrawString("Move!!", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 220);
            }
            g.FillRectangle(new SolidBrush(Color.FromArgb(98, Color.Green)), 0, 110, 150, 50);
            g.DrawString("projective pos: ", new Font("Console", 15), new SolidBrush(Color.Yellow), 0, 115);
            g.DrawString("(" + ((int)moveData.CurrentUserPosition.X).ToString() + "," + ((int)moveData.CurrentUserPosition.Y).ToString() + "," + ((int)moveData.CurrentUserPosition.Z).ToString() + ")", new Font("Console", 15), new SolidBrush(Color.Yellow), 0, 130);
            g.DrawLine(new Pen((Color.FromArgb(90, Color.Yellow)), 3), moveData.CenterPosition, 0, moveData.CenterPosition, Height);
            g.DrawLine(new Pen((Color.FromArgb(90, Color.Yellow)), 3), moveData.XAxiesMinBorder, 0, moveData.XAxiesMinBorder, Height);
            g.DrawLine(new Pen((Color.FromArgb(90, Color.Yellow)), 3), moveData.XAxiesMaxBorder, 0, moveData.XAxiesMaxBorder, Height);
            ///////////////////////////////////////////////////////////////////////////
            //slash
            
            if (isRightHandSlash)  //draw the displacement of record
            {
                //draw slash type
                g.FillRectangle(new SolidBrush(Color.FromArgb(98, Color.Yellow)), 200, 200, 220, 150);
                g.DrawString("Slash Type:" +rightHandSlash.SlashActionType.ToString(), new Font("Console", 15), new SolidBrush(Color.GreenYellow), 230, 220);
                //draw point
                Point3D proj1 = KinectAction.ConvertRealWorldToProjective(rightHandSlash.RealPointRecorder.First.Value.Position);
                foreach (SkeletonJointPosition p in rightHandSlash.RealPointRecorder)
                {
                    Point3D proj2 = KinectAction.ConvertRealWorldToProjective(p.Position);
                    g.DrawLine(new Pen((Color.Yellow), 8), new Point((int)proj1.X, (int)proj1.Y), new Point((int)proj2.X, (int)proj2.Y));
                    proj1 = proj2;
                }
            }
            //g.DrawString("slash angle => " + slashAngle.ToString(), new Font("Console", 12), new SolidBrush(Color.Yellow), 0, 100);
            ///////////////////////////////////////////////////////////////////////////
            //run
            if (isRun)
            {
                g.DrawString("is running ", new Font("Console", 18), new SolidBrush(Color.Red), 30, 420);
                g.DrawString("runSpeed => " + Math.Round(runTimesForSpeed,2), new Font("Console", 18), new SolidBrush(Color.Red), 30, 440);
            }
            else
            {
                g.DrawString("is Stable ", new Font("Console", 18), new SolidBrush(Color.Red), 30, 420);
            }
            g.FillRectangle(new SolidBrush(Color.FromArgb(98, Color.Yellow)), 0, 400, 200, 80);
            ///////////////////////////////////////////////////////////////////////////
            //pause
            if (isWaitingPause)
            {
                g.FillRectangle(new SolidBrush(Color.LightBlue), 0, 85, (int)pauseWaitingTime, 10);
                g.DrawString("vertical angle => " + verticalAngle.ToString(), new Font("Console", 12), new SolidBrush(Color.Yellow), 0, 60);
                g.DrawString("horizon angle => " + horizonAngle.ToString(), new Font("Console", 12), new SolidBrush(Color.Yellow), 0, 75);
                if (isPause)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(98, Color.Green)), 200, 200, 220, 150);
                    g.DrawString("Pause!", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 250);
                    isPause = false;
                    isWaitingPause = false;
                }
            }
            ///////////////////////////////////////////////////////////////////////////
            //fly
            if (isFly)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(98, Color.Blue)), 200, 200, 220, 150);
                if (flyData.OrientaionFly == FlyBehaviorActionEnum.FlyLeft)
                {
                    g.DrawString("FlyLeft", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 220);
                    g.DrawString("move =>" + flyData.YOriDisplacement.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 240);
                    g.DrawString("handsDifference " + flyData.ZOriDifference.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 260);

                }
                else if (flyData.OrientaionFly == FlyBehaviorActionEnum.FlyRight)
                {
                    g.DrawString("FlyRight", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 220);
                    g.DrawString("move =>" + flyData.YOriDisplacement.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 240);
                    g.DrawString("handsDifference " + flyData.ZOriDifference.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 260);

                }
                else if (flyData.OrientaionFly == FlyBehaviorActionEnum.FlyDown)
                {
                    g.DrawString("FlyDown", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 220);
                    g.DrawString("move =>" + flyData.YOriDisplacement.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 240);
                    g.DrawString("handsDifference " + flyData.ZOriDifference.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 260);

                }
                else if (flyData.OrientaionFly == FlyBehaviorActionEnum.FlyStraight)
                {                  
                    g.DrawString("Straight", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 220);
                    g.DrawString("move =>" + flyData.YOriDisplacement.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 240);
                    g.DrawString("handsDifference " + flyData.ZOriDifference.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 260);

                }
            }
            else //show erreor message
            {
                //g.FillRectangle(new SolidBrush(Color.FromArgb(98, Color.Blue)), 200, 200, 220, 150);
                //g.DrawString("Error! Not fly...", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 220);
                //g.DrawString("Messgae =>", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 240);
                //g.DrawString(flyMessgae, new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 260);

            }
            //show angle
            //g.DrawString("left angle => " + leftAngle.ToString(), new Font("Console", 12), new SolidBrush(Color.Yellow), 0, 100);
            //g.DrawString("right angle => " + rightAngle.ToString(), new Font("Console", 12), new SolidBrush(Color.Yellow), 0, 115);
            if (isSetFlyRangeBox)
            {
                Point3D xleft = KinectAction.ConvertRealWorldToProjective(new Point3D((float)rangeRuleBox.XLeftThreshold, rangeRuleBox.DistributionCenter.Y, rangeRuleBox.DistributionCenter.Z));
                Point3D xright = KinectAction.ConvertRealWorldToProjective(new Point3D((float)rangeRuleBox.XRightThreshold, rangeRuleBox.DistributionCenter.Y, rangeRuleBox.DistributionCenter.Z));
                Point3D yheigher = KinectAction.ConvertRealWorldToProjective(new Point3D(rangeRuleBox.DistributionCenter.X, (float)rangeRuleBox.YHeigherThreshold, rangeRuleBox.DistributionCenter.Z));
                Point3D ylower = KinectAction.ConvertRealWorldToProjective(new Point3D(rangeRuleBox.DistributionCenter.X, (float)rangeRuleBox.YLowerThreshold, rangeRuleBox.DistributionCenter.Z));
                Point3D center = KinectAction.ConvertRealWorldToProjective(new Point3D(rangeRuleBox.DistributionCenter.X, rangeRuleBox.DistributionCenter.Y, rangeRuleBox.DistributionCenter.Z));
                g.DrawLine(new Pen((Color.FromArgb(90, Color.Yellow)), 3), 0, center.Y, Width, center.Y);
                g.DrawLine(new Pen((Color.FromArgb(90, Color.Red)), 3), xleft.X, 0, xleft.X, Height);
                g.DrawLine(new Pen((Color.FromArgb(90, Color.Red)), 3), xright.X, 0, xright.X, Height);
                g.DrawLine(new Pen((Color.FromArgb(90, Color.Pink)), 3), 0, ylower.Y, Width, ylower.Y);
                g.DrawLine(new Pen((Color.FromArgb(90, Color.Pink)), 3), 0, yheigher.Y, Width, yheigher.Y);

            }
            ///////////////////////////////////////////////////////////////////////////
            //swipe
            //g.DrawString("UpDownHorizon angle => " + swipeUpDownHorizonAngle.ToString(), new Font("Console", 12), new SolidBrush(Color.Yellow), 0, 100);
            //g.DrawString("RightLeftHorizon angle => " + swipeRightLeftHorizonAngle.ToString(), new Font("Console", 12), new SolidBrush(Color.Yellow), 0, 115);
            //g.DrawString("Vertical angle => " + swipeVerticalAngle.ToString(), new Font("Console", 12), new SolidBrush(Color.Yellow), 0, 130);
            if (isSwipe)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(98, Color.Blue)), 200, 200, 220, 150);
                if (swipeData.SwipeOrientation == SwipeBehaviorActionEnum.Left)
                {
                    g.DrawString("swipe left", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 210);
                }
                else if (swipeData.SwipeOrientation == SwipeBehaviorActionEnum.Right)
                {
                    g.DrawString("swipe right", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 210);
                }
                else if (swipeData.SwipeOrientation == SwipeBehaviorActionEnum.Up)
                {
                    g.DrawString("swipe up", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 210);
                }
                else if (swipeData.SwipeOrientation == SwipeBehaviorActionEnum.Down)
                {
                    g.DrawString("swipe down", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 210);
                }

                g.DrawString("displacement x =>" + swipeData.XOrientationDisplacement.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 225);
                g.DrawString("displacement y =>" + swipeData.YOrientationDisplacement.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 240);
                g.DrawString("speed x =>" + swipeData.XOrientationVelocity.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 255);
                g.DrawString("speed y =>" + swipeData.XOrientationVelocity.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 270);
                g.DrawString("Horizon angle " + swipeData.AngleBetweenHorizonOrientation.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 285);
                g.DrawString("vertical angle " + swipeData.AngleBetweenVerticalOrientation.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 300);
            }
            else
            {
                //g.FillRectangle(new SolidBrush(Color.FromArgb(98, Color.Blue)), 200, 200, 220, 150);
                //g.DrawString("Not trigger swipe...", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 220);
                //g.DrawString("Messgae =>", new Font("Console", 12), new SolidBrush(Color.Yellow), 230, 240);
                //g.DrawString(swipeMessgae, new Font("Console", 12), new SolidBrush(Color.Yellow), 230, 260);
            }
            ///////////////////////////////////////////////////////////////////////////
        }
        //此函式:不需抓取骨架,只要人一進步畫面判斷到有使用者即會執行
        private void DrawState(int user, Graphics g, Point3D com, Point3D realCom)
        {
            int maxDeppth = KinectAction.GetDevicetMaxDepth();
            int ratio = 50;
            g.FillRectangle(new SolidBrush(Color.FromArgb(98, Color.White)), 640 - 100, 0, 100, (int)(maxDeppth / ratio));
            if (com.X >0 && com.Z > 0)
            {
                //最近的追蹤線
                g.DrawLine(new Pen((Color.FromArgb(90, Color.Red)), 1), 640 - 100, (int)(KinectAction.UserMinTrackingThreshold / ratio), 640, (int)(KinectAction.UserMinTrackingThreshold / ratio));
                g.DrawLine(new Pen((Color.FromArgb(90, Color.Red)), 1), 640 - 100, (int)(KinectAction.UserMinWaringThreshold / ratio), 640, (int)(KinectAction.UserMinWaringThreshold / ratio));
                //校正線
                g.DrawLine(new Pen((Color.FromArgb(90, Color.Green)), 1), 640 - 100, (int)(KinectAction.UserMinPoseDetectionThreshold / ratio), 640, (int)(KinectAction.UserMinPoseDetectionThreshold / ratio));
                g.DrawLine(new Pen((Color.FromArgb(90, Color.Green)), 1), 640 - 100, (int)(KinectAction.UserMaxPoseDetectionThreshold / ratio), 640, (int)(KinectAction.UserMaxPoseDetectionThreshold / ratio));
                //最遠的可追蹤線
                g.DrawLine(new Pen((Color.FromArgb(90, Color.Blue)), 1), 640 - 100, (int)(KinectAction.UserMaxWaringThreshold / ratio), 640, (int)(KinectAction.UserMaxWaringThreshold / ratio));
                g.DrawLine(new Pen((Color.FromArgb(90, Color.Blue)), 1), 640 - 100, (int)(KinectAction.UserMaxTrackingThreshold / ratio), 640, (int)(KinectAction.UserMaxTrackingThreshold / ratio));
                g.FillEllipse(new SolidBrush(nodeColors[userId % ncolors]), new Rectangle((int)((640 - 100) + (com.X - 5) * 100 / 640), (int)((com.Z) / ratio) - 5, 5, 5));

            }
            //test drawstate user and poschanged user number
            g.DrawString("drawState =>" + user + "(" + ((int)realCom.X).ToString() + "," + ((int)realCom.Y).ToString() + "," + ((int)realCom.Z).ToString() + ")", new Font("Console", 14), new SolidBrush(nodeColors[user % ncolors]), com.X + 50, com.Y + 30);
            g.DrawString("stateChanged=>" + KinectAction.UserCurrentState[user].ToString() + "(" + ((int)realCom.X).ToString() + "," + ((int)realCom.Y).ToString() + "," + ((int)realCom.Z).ToString() + ")", new Font("Console", 14), new SolidBrush(nodeColors[user % ncolors]), com.X + 50, com.Y + 50);
            g.DrawString("FPS:" + KinectAction.GetMapModeData.FPS.ToString(), new Font("Console", 12), new SolidBrush(Color.Yellow), 0, 15);
            g.DrawString("User counter in view=> " + KinectAction.UserCurrentState.Count, new Font("Console", 12), new SolidBrush(Color.Yellow), 0, 45);
            if (KinectAction.UserCurrentState.ContainsKey(user))
            {
                if (KinectAction.UserCurrentState[user] == UserStateEnum.StopTracking)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(98, Color.Green)), 200, 200, 220, 150);
                    g.DrawString("Stop Tracking!", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 250);
                    g.DrawString("Please stand in the Safe Range", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 280);
                }
                if (KinectAction.UserCurrentState[user] == UserStateEnum.UserExit)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(98, Color.Green)), 200, 200, 220, 150);
                    g.DrawString("Number" + user.ToString() + "UserExit View", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 250);
                    g.DrawString("Looking For User...", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 280);
                }
                if (KinectAction.UserCurrentState[user] == UserStateEnum.UserReEnter)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(98, Color.Green)), 200, 200, 220, 150);
                    g.DrawString("Number" + user.ToString() + "ReEnter", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 250);

                }
                if (KinectAction.UserCurrentState[user] == UserStateEnum.LostUser)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(98, Color.Green)), 200, 200, 220, 150);
                    g.DrawString("LostUser...", new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 250);
                    g.DrawString("number : " + user.ToString(), new Font("Console", 15), new SolidBrush(Color.Yellow), 230, 280);
                }
            }
        }

        #region Draw Skeleton->畫骨架的部分,只是這邊的座標式投影過的,並非Kinect直接抓到三維座標
        ///////////////////////////////////////////////////////////////////
        //Draw skeleton joint point
        private void DrawPiont(Graphics g, Color color, Dictionary<SkeletonJoint, SkeletonJointPosition> dict, SkeletonJoint j)
        {
            //test
            Point3D pos = KinectAction.ConvertRealWorldToProjective(dict[j].Position);
            //Point3D pos = dict[j].Position;
            if (dict[j].Confidence == 0)
                return;

            //-5 means the sphere center
            g.FillEllipse(new SolidBrush(nodeColors[userId % ncolors]), new Rectangle((int)pos.X - 5, (int)pos.Y - 5, 10, 10));

        }

        /////////////////////////////////////////////
        //Draw skeleton joint line
        private void DrawLine(Graphics g, Color color, Dictionary<SkeletonJoint, SkeletonJointPosition> dict, SkeletonJoint j1, SkeletonJoint j2)
        {
            //test
            Point3D pos1 = KinectAction.ConvertRealWorldToProjective(dict[j1].Position);
            Point3D pos2 = KinectAction.ConvertRealWorldToProjective(dict[j2].Position);
            //Point3D pos1 = dict[j1].Position;
            //Point3D pos2 = dict[j2].Position;

            if (dict[j1].Confidence == 0 || dict[j2].Confidence == 0)
                return;

            g.DrawLine(new Pen(color, 3),
                    new Point((int)pos1.X, (int)pos1.Y),
                    new Point((int)pos2.X, (int)pos2.Y));
        }

        /////////////////////////////////////////////
        //Draw Skeleton
        private void DrawSkeleton(Graphics g, Color color, int user)
        {
            Dictionary<SkeletonJoint, SkeletonJointPosition> dict = drawSk;

            DrawLine(g, color, dict, SkeletonJoint.Head, SkeletonJoint.Neck);
            DrawLine(g, color, dict, SkeletonJoint.LeftShoulder, SkeletonJoint.Torso);
            DrawLine(g, color, dict, SkeletonJoint.RightShoulder, SkeletonJoint.Torso);
            DrawLine(g, color, dict, SkeletonJoint.Neck, SkeletonJoint.LeftShoulder);
            DrawLine(g, color, dict, SkeletonJoint.LeftShoulder, SkeletonJoint.LeftElbow);
            DrawLine(g, color, dict, SkeletonJoint.LeftElbow, SkeletonJoint.LeftHand);
            DrawLine(g, color, dict, SkeletonJoint.Neck, SkeletonJoint.RightShoulder);
            DrawLine(g, color, dict, SkeletonJoint.RightShoulder, SkeletonJoint.RightElbow);
            DrawLine(g, color, dict, SkeletonJoint.RightElbow, SkeletonJoint.RightHand);
            DrawLine(g, color, dict, SkeletonJoint.LeftHip, SkeletonJoint.Torso);
            DrawLine(g, color, dict, SkeletonJoint.RightHip, SkeletonJoint.Torso);
            DrawLine(g, color, dict, SkeletonJoint.LeftHip, SkeletonJoint.RightHip);
            DrawLine(g, color, dict, SkeletonJoint.LeftHip, SkeletonJoint.LeftKnee);
            DrawLine(g, color, dict, SkeletonJoint.LeftKnee, SkeletonJoint.LeftFoot);
            DrawLine(g, color, dict, SkeletonJoint.RightHip, SkeletonJoint.RightKnee);
            DrawLine(g, color, dict, SkeletonJoint.RightKnee, SkeletonJoint.RightFoot);

            //point 
            DrawPiont(g, color, dict, SkeletonJoint.Head);
            DrawPiont(g, color, dict, SkeletonJoint.Neck);
            DrawPiont(g, color, dict, SkeletonJoint.LeftShoulder);
            DrawPiont(g, color, dict, SkeletonJoint.Torso);
            DrawPiont(g, color, dict, SkeletonJoint.RightShoulder);
            DrawPiont(g, color, dict, SkeletonJoint.LeftElbow);
            DrawPiont(g, color, dict, SkeletonJoint.LeftHand);
            DrawPiont(g, color, dict, SkeletonJoint.RightElbow);
            DrawPiont(g, color, dict, SkeletonJoint.RightHand);
            DrawPiont(g, color, dict, SkeletonJoint.LeftHip);
            DrawPiont(g, color, dict, SkeletonJoint.RightHip);
            DrawPiont(g, color, dict, SkeletonJoint.LeftKnee);
            DrawPiont(g, color, dict, SkeletonJoint.LeftFoot);
            DrawPiont(g, color, dict, SkeletonJoint.RightKnee);
            DrawPiont(g, color, dict, SkeletonJoint.RightFoot);
        }
        ///////////////////////////////////////////////////////////////////
        #endregion

        #region Calculate histogram
        ////////////////////////////////////////////////////////////////////////
        /*值方圖計算:
       * 先對histogram對初始設定=>0
       * 透過DepthMetaData型態使用裏頭的深度size大小(x,y)
       * 迴圈，抓出裏頭深度的資料並判斷是否有無值，有值則以此值的數值作為索引對histogram陣列索引作+1
       * 同時需要用到一個變數來計算所有有深度值的點數量
       * 最後去做正規劃把數值算出一個合理的值
       * 
       *
       */
        public unsafe void CalcHist(DepthMetaData depthMD)
        {
            // reset
            for (int i = 0; i < this.histogram.Length; ++i)
                this.histogram[i] = 0;

            // This will point to our depth image
            ushort* pDepth = (ushort*)depthMD.DepthMapPtr.ToPointer();

            int points = 0;
            for (int y = 0; y < depthMD.YRes; ++y)
            {
                for (int x = 0; x < depthMD.XRes; ++x, ++pDepth)
                {
                    //指向深度資料的value
                    ushort depthVal = *pDepth;
                    //深度不為0,則對運算值方圖的陣列++
                    if (depthVal != 0)
                    {
                        //在深度圖中位於這個深度值的數量++
                        this.histogram[depthVal]++;
                        //calculate the pixel of depth data in image for normaizing
                        points++;
                    }
                }
            }

            for (int i = 1; i < this.histogram.Length; i++)
            {
                this.histogram[i] += this.histogram[i - 1];
            }
            //if the image has the depth data, normalize
            if (points > 0)
            {
                for (int i = 1; i < this.histogram.Length; i++)
                {
                    this.histogram[i] = (int)(256 * (1.0f - (this.histogram[i] / (float)points)));
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////
        #endregion

        #region Calculate Depth Color For Show
        ////////////////////////////////////////////////////////////////////////
        //計算值方圖的顏色
        public unsafe void CalculateDepthColor(Color[] userColors, int ncolors, DepthMetaData depthMD, Bitmap bitmap)
        {
            /* 透過配置一個矩陣大小來把bitmap資源鎖住在記憶體中,其型態為BitmapData
                     * 宣告一個指標指向g_Depth的位置
                     */
            // Lock the bitmap we will be copying to just in case. This will also give us a pointer to the bitmap.
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);


            // This will point to our depth image
            //ToPointer():轉成指標
            //
            ushort* pDepth = (ushort*)this.KinectAction.DepthMapPtr().ToPointer();
            //用來做為深度圖中找出user的pixels,SceneMapPtr: int的變數的指標
            ushort* pLabels = (ushort*)this.KinectAction.GetUserPixels(0).LabelMapPtr.ToPointer();


            // Go over the depth image and set the bitmap we're copying to based on our depth value.
            // set pixels
            for (int y = 0; y < depthMD.YRes; ++y)
            {
                //指向BistmapData,所以下面修改pDest資料是對指向的位置裏頭修改(bitmap)
                byte* pDest = (byte*)data.Scan0.ToPointer() + y * data.Stride;
                //++pLabels 移動plabel位置:裡面是一個2維圖矩陣存成的1維陣列
                for (int x = 0; x < depthMD.XRes; ++x, ++pDepth, ++pLabels, pDest += 3)
                {
                    //pDepth的值設為0
                    pDest[0] = pDest[1] = pDest[2] = 0;

                    ushort label = *pLabels;
                    //目前掃到的pixel中在*plabel的值 !=0 (有user的話)
                    if (this.shouldDrawBackground || *pLabels != 0)
                    {
                        //依使用者所給予的基本色來設定label的顏色
                        Color labelColor = Color.White;
                        if (label != 0)
                        {
                            labelColor = userColors[label % ncolors];
                        }

                        byte pixel = (byte)this.histogram[*pDepth];
                        pDest[0] = (byte)(pixel * (labelColor.B / 256.0));
                        pDest[1] = (byte)(pixel * (labelColor.G / 256.0));
                        pDest[2] = (byte)(pixel * (labelColor.R / 256.0));
                    }
                }

            }
            //解除lock鎖住的資源,並讓出
            bitmap.UnlockBits(data);

        }
        ////////////////////////////////////////////////////////////////////////
        #endregion

        //只式螢幕縮放用途
        //handle the screen size when size was transformed
        protected void KinectDisplay_Resize(object sender, EventArgs e)
        {
            double x = (this.Width / oldWidth);
            double y = (this.Height / oldHeight);

            this.kinectDisplayPanel.Width = Convert.ToInt32(x * this.kinectDisplayPanel.Width);
            this.kinectDisplayPanel.Height = Convert.ToInt32(y * this.kinectDisplayPanel.Height);

            oldWidth = this.Width;
            oldHeight = this.Height;

        }
        public KinectActionModel GetKinectAction { get { return KinectAction; } }
        #region override part
        ////////////////////////////////////////////////////////////////////////
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {
                Close();
            }
            switch (e.KeyChar)
            {
                case (char)27:
                    break;
                case 'b':
                    this.shouldDrawBackground = !this.shouldDrawBackground;
                    break;
                case 'x':
                    this.shouldDrawPixels = !this.shouldDrawPixels;
                    break;
                case 's':
                    this.shouldDrawSkeleton = !this.shouldDrawSkeleton;
                    break;
                case 'i':
                    this.shouldPrintID = !this.shouldPrintID;
                    break;
                case 'l':
                    this.shouldPrintState = !this.shouldPrintState;
                    break;

            }
            base.OnKeyPress(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            lock (this)
            {
                e.Graphics.DrawImage(KinectAction.GetFaceBitmap(),
                    this.kinectDisplayPanel.Location.X,
                    this.kinectDisplayPanel.Location.Y,
                    this.kinectDisplayPanel.Size.Width,
                    this.kinectDisplayPanel.Size.Height);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //Don't allow the background to paint
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            KinectAction.ShouldRun = false;
            this.KinectWorkerThread.Join();
            base.OnClosing(e);
        }
        ////////////////////////////////////////////////////////////////////////
        #endregion

    }
}
