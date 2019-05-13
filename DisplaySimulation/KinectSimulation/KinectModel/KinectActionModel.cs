using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenNI;
using System.ComponentModel;
using System.Runtime.InteropServices;   // needed to call external application
using KinectModel.BehaviorStruct;
using KinectModel.StateEnum;
using KinectModel.Device;
using System.Drawing;
using System.Drawing.Imaging;

namespace KinectModel
{

    ///// <summary>
    ///// 此類別是測試用,可以使用Console mode輸出資料
    ///// </summary>
    //public class Win32
    //{
    //    [DllImport("kernel32.dll")]
    //    public static extern bool AttachConsole(int dwProcessId);
    //    [DllImport("kernel32.dll")]
    //    public static extern Boolean AllocConsole();
    //    [DllImport("kernel32.dll")]
    //    public static extern Boolean FreeConsole();
    //}

    #region Delegate Method
    ///////////////////////////////////////////////////////////
    public delegate void NoBehaviorEventHandler();
    public delegate void NoBehaviorMessageEventHandler(string message);
    public delegate void PushBehaviorEventHandler(PushBehaviorData push);
    public delegate void TimesChangedEventHandler(double time);
    public delegate void SlashBehaviorEventHandler(SlashBehaviorData slash);
    public delegate void JumpBehaviorEventHandler(JumpBehaviorData jump);
    public delegate void TestAngleChangedEventHandler(double angel, string whichAngle);
    public delegate void PauseBehaviorEventHandler(double verticalAngle, double horizonAngle);
    public delegate void FlyBehaviorEventHandler(FlyBehaviorData fly);
    public delegate void FlyRangeBoxChanged(FlyRangeBoxData box);
    public delegate void SwipeBehaviorEventHandler(SwipeBehaviorData swipe);
    ///////////////////////////////////////////////////////////
    public delegate void UserStateChangedEventHandler(int user, UserStateEnum currentState);
    public delegate void DepthUpdateEventHandler(DepthMetaData depth);
    public delegate void SkeletonJointUpdateEventHandler(Dictionary<SkeletonJoint, SkeletonJointPosition> pos);
    public delegate void MoveLeftAndRightEventHandler(MoveBehaviorData move);
    ///////////////////////////////////////////////////////////
    #endregion

    /// <summary>
    /// 1.此類別提供一大堆的遊戲行為訊號,透過註冊事件即可使用,不要使用時請取消註冊
    /// 2.一個行為通常會包含兩個事件(做與不做)
    /// </summary>
    public class KinectActionModel
    {
        #region Event Declartion
        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 應為選單使用:
        /// 若做出Push行為,即會發生此事件,處理事件的引數 (PushBehaviorData push)
        /// </summary>
        public event PushBehaviorEventHandler OnHandPush;
        
        /// <summary>
        /// 應為選單使用:
        /// 若不做出Push,即會發生此事件,處理事件的引數  (string message)->字串訊息兩種:"LeftHand","RightHand"
        /// </summary>
        public event NoBehaviorMessageEventHandler OnHandPushStable;
        
        /// <summary>
        /// 應為關卡一(猛獸)使用:
        /// 若做出原地跑步,即會發生此事件,處理事件的引數  (double RunSpeed)
        /// </summary>
        public event TimesChangedEventHandler OnFootRun;
        
        /// <summary>
        /// 應為關卡一(猛獸)使用:
        /// 若不做出原地跑步,即會發生此事件
        /// </summary>
        public event NoBehaviorEventHandler OnFootRunStable;
       
        /// <summary>
        /// 應為關卡一(猛獸)使用:
        /// 若做出揮砍行為並且速度夠快(左或右手皆可),即會發生此事件,處理事件的引數  (SlashBehaviorData slash)
        /// </summary>
        public event SlashBehaviorEventHandler OnHandSlash;

        /// <summary>
        /// 此為測試用,可以取得目前手揮砍 (double angel,string whichAngle)
        /// </summary>
        public event TestAngleChangedEventHandler OnSlashAngleChanged; //test for show

        /// <summary>
        /// 應為關卡一(猛獸)使用:
        /// 若未做出揮砍,即會發生此事件
        /// </summary>
        public event NoBehaviorEventHandler OnHandSlashStable;
        
        /// <summary>
        /// 應為關卡一(猛獸)使用:
        /// 若向左移動或向右移動,則會發生此事件,處理事件的引數  (MoveBehaviorData move)
        /// </summary>
        public event MoveLeftAndRightEventHandler OnMoveLeftOrRight;
        
        /// <summary>
        /// 若未左或右移動則會發生此事件
        /// </summary>
        public event NoBehaviorEventHandler OnMoveLeftOrRightStable;
        /// <summary>
        /// 應為關卡一(猛獸)使用:
        /// 若做出向上跳的行為則會發生,處理事件的引數 (JumpBehaviorData jump)
        /// </summary>
        public event JumpBehaviorEventHandler OnJump;
        
        /// <summary>
        /// 應為關卡一(猛獸)使用:
        /// 若未做出向上跳的行為則會發生
        /// </summary>
        public event NoBehaviorEventHandler OnJumpStable;
        
        /// <summary>
        /// 各遊戲關卡應皆有,進入選單使用:
        /// 若把左手舉起且水平擺放約3秒,則會發生,處理事件的引數 (double verticalAngle,double horizonAngle)
        /// </summary>
        public event PauseBehaviorEventHandler OnPause;

        /// <summary>
        /// 各遊戲關卡應皆有,進入選單使用:
        /// Pause動作完成,等待時間發生此事件 ,處理事件的引數(int waitingTime)
        /// </summary>
        public event TimesChangedEventHandler OnWaitingPause;

        /// <summary>
        /// 各遊戲關卡應皆有,進入選單使用:
        /// 若沒有擺出Pause動作,則會觸發此事件
        /// </summary>
        public event NoBehaviorEventHandler OnNotPause;
        
        /// <summary>
        /// 此為測試用,可以取得目前左手和腋下的角度以及左手和虛擬的向前向量角度 (double angel,string whichAngle)
        /// </summary>
        public event TestAngleChangedEventHandler OnPauseAngleChanged; //test for show

        /// <summary>
        /// 此為測試用,可以取得目前飛行時兩手手肘的角度 (double angel,string whichAngle)
        /// </summary>
        public event TestAngleChangedEventHandler OnFlyAngleChanged; //test for show
        
        /// <summary>
        /// 應為關卡二(飛行)使用:
        /// 若做飛行的動作,則事件即會發生,處理事件引數(FlyBehaviorData fly) 
        /// </summary>
        public event FlyBehaviorEventHandler OnFly;
        
        /// <summary>
        /// 應為關卡二(飛行)使用:
        /// 此事件可以隨時得知目前範圍Box的資訊,處理事件引數 (FlyRangeBoxData box)
        /// </summary>
        public event FlyRangeBoxChanged OnBoxChanged; //test for show,but maybe can use
        
        /// <summary>
        /// 應為關卡二(飛行)使用:
        /// 若不在是件非行動作的規範內則壞發生此事件 (string message)->字串訊息:FlyBehaviorStateMessageEnum類別中的資料
        /// </summary>
        public event NoBehaviorMessageEventHandler OnNotFly;

        /// <summary>
        /// 應為選單使用:
        /// 若做出向(上/下/左/右)的動作且速度夠則會發生此事件,處理事件的引數 (SwipeBehaviorData swipe)
        /// </summary>
        public event SwipeBehaviorEventHandler OnSwipe;

        /// <summary>
        /// 應為選單使用:
        /// 若未做出Swipe的任何動作即會發生,處理事件的引數 (string message) ->字串訊息:SwipeBehaviorStateMessageEnum類別中的資料
        /// </summary>
        public event NoBehaviorMessageEventHandler OnNotSwipe;

        /// <summary>
        /// 此為測試用,可取得Swipe時的向量和理應成垂直夾角的向量,這中間的夾角角度
        /// </summary>
        public event TestAngleChangedEventHandler OnSwipeAngleChanged; //test for show
        ////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// 應為任何時候接有:
        /// 若使用者的在畫面上的行為改變(校正,失去使用者,尋找姿勢等)時,則會發生此事件,處理事件的引數 (int user, UserStateEnum currentState)
        /// </summary>
        public event UserStateChangedEventHandler OnUserStateChanged;
        
        /// <summary>
        /// 若Kinect更新深度時,會發生此事件(若不需使用深度資料,可以不註冊),處理事件的引數(DepthMetaData depth)
        /// </summary>
        public event DepthUpdateEventHandler OnDepthMDUpdate;
        
        /// <summary>
        /// 應為任何時候接有:
        /// 骨架資料更新時,此事件會發生,處理事件的引數 (Dictionary<SkeletonJoint, SkeletonJointPosition> pos)
        /// </summary>
        public event SkeletonJointUpdateEventHandler OnJointsUpdate;
        /// <summary>
        /// 使用者離開畫面或失去骨架時,需要重置資料所用事件,無處理引數
        /// </summary>
        public event NoBehaviorEventHandler OnReset;
        ////////////////////////////////////////////////////////////////////////
        #endregion

        #region Const variable
        ////////////////////////////////////////////////////////////////////////

        //主要是判斷方向,用數值為正或負來決定(如:向右為正向左為負)
        private const int CHECK_VELOCITY_ORIENTAION = 0;

        ///////////////////////
        // 暫停部分
        private const int PAUSE_VERTICAL_ANGLE_UPPER_THRESHOLD = 110;
        private const int PAUSE_VERTICAL_ANGLE_LOWER_THRESHOLD = 75;
        private const int PAUSE_HORIZON_ANGLE_UPPER_THRESHOLD = 110;
        private const int PAUSE_HORIZON_ANGLE_LOWER_THRESHOLD = 75;
        ///////////////////////
        // 滑動部分
        private const int SWIPE_UP_DOWN_ANGLE_THRESHOLD = 45;
        private const int SWIPE_RIGHT_LEFT_ANGLE_THRESHOLD = 45;
        private const int SWIPE_Z_ORI_THRESHOLD = 45;
        private const int SWIPE_VIRTUAL_POINT_DISTANCE = 300;
        private const int SWIPE_UP_DOWN_DISPLACEMENT  =200;
        private const int SWIPE_RIGHT_LEFT_DISPLACEMENT = 220;
        private const int SWIPE_TORSO_DISTANCE_THRESHOLD = 100;

       
        ///////////////////////
        // 飛行部分
        private const int HANDS_Y_ORI_DIFFERENCE = 100;
        private const int HANDS_Z_ORI_DIFFERENCE = 300;
        private const int HANDS_ANGLE = 130;
        private const int RANGE_BOX_UP_THRESHOLD = 100;
        private const int RANGE_BOX_THRESHOLD = 100;
       
        //建算虛擬假設的向量所用的另一個點座標差距
        private const int PUSH_VIRTUAL_POINT_DISTANCE = 300;

        ////////////////////////////////////////////////////////////////////////
        #endregion

        #region Member Property
        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 可以設定是否要讓KinectActionModel執行
        /// </summary>
        public bool ShouldRun
        {
            set { isWorking = value; }
        }
        
        /// <summary>
        /// 可以取得目前每個使用者的狀態
        /// </summary>
        public Dictionary<int, UserStateEnum> UserCurrentState
        {
            get { return this.userCurrentState; }
        }
        
        /// <summary>
        /// 可以得知Kinect裝置的基本資訊,遊戲應該不需要
        /// </summary>
        public MapOutputMode GetMapModeData
        {
            get { return kinectProcess.MapMode; }
        }
        /////////////////////////////////////////////////////////////
        //跑步部分
        /// <summary>
        /// 跑步時,抬腳的臨界速度設定,預設為10(須為正值)
        /// 數值越小越容易觸發
        /// </summary>
        public int RunLegLiftUpVelocityThreshold
        { set { runLegUpVelocity = value; } }
        
        /// <summary>
        /// 跑步時,下腳的臨界速度設定,預設為-8(須為負值)
        /// 數值越小越容易觸發
        /// </summary>
        public int RunLegLiftDonwVelocityThreshold
        { set { runLegDownVelocity = value; } }
        ///////////////////////////////////////////////////////////
        //移動部分
        /// <summary>
        /// 節點會因為攝影機的訊號有雜訊導致座標不穩定,所以給予一個誤差值
        /// 若變動在此誤差值內一律視為仍在原地,預設值10
        /// </summary>
        public int MoveDeviationBorder
        {
            set { deviationBorder = value; }
        }
        ///////////////////////////////////////////////////////////
        //跳躍部分
        /// <summary>
        /// 跳躍時,向上跳的瞬間速度設定,預設為15(須為正值)
        /// 數值越小越容易觸發
        /// </summary>
        public int JumpVelocityThreshold
        { set { jumpVelocityThreshold = value; } }
        ///////////////////////////////////////////////////////////
        //使用者追蹤位置部分
        /// <summary>
        /// 使用者骨架可追蹤的最遠距離設定,預設為3800
        /// 越遠可能會被視為背景
        /// </summary>
        public int UserMaxTrackingThreshold
        { 
            set { userMaxTrackingPosition = value; }
            get { return userMaxTrackingPosition; }
        }
        
        /// <summary>
        /// 使用者骨架追蹤的警告最遠距離設定,預設為3500
        /// </summary>
        public int UserMaxWaringThreshold
        { 
            set { userMaxWaringPosition = value; }
            get { return userMaxWaringPosition; }
        }

        /// <summary>
        /// 使用者骨架追蹤的警告最近距離設定,預設為2000
        /// </summary>
        public int UserMinWaringThreshold
        { 
            set { userMinWaringPosition = value; }
            get { return userMinWaringPosition; }
        }

        /// <summary>
        /// 使用者骨架可追蹤的最近距離設定,預設為1600
        /// 越近畫面無法照到全身
        /// </summary>
        public int UserMinTrackingThreshold
        { 
            set { userMinTrackingPosition = value; }
            get { return userMinTrackingPosition; }
        }
        ////////////////////////////////////////////////////////////////////////
        //校正範圍部分
        /// <summary>
        /// 使用者要進入追蹤骨架前所需校正的最大範圍,預設為2600
        /// </summary>
        public int UserMaxPoseDetectionThreshold
        {
            set { userMaxPoseDetectionPosition = value; }
            get { return userMaxPoseDetectionPosition; }
        }

        /// <summary>
        /// 使用者要進入追蹤骨架前所需校正的最近範圍,預設為1600
        /// 越近畫面無法照到全身
        /// </summary>
        public int UserMinPoseDetectionThreshold
        {
            set { userMinPoseDetectionPosition = value; }
            get { return userMinPoseDetectionPosition; }
        }

        ////////////////////////////////////////////////////////////////////////
        //推部分
        /// <summary>
        /// 向前推的時候,可設定速度的臨界值,預設為-55(須為負值)
        /// 數值越小越可能隨意觸發
        /// </summary>
        public int PushVelocityThreshold
        { set { pushSpeedThreshold = value; } }

        /// <summary>
        /// 向前推的時候,推的角度容忍值設定,預設為40
        /// 數值越大斜推的時候越容易觸發
        /// </summary>
        public int PushAngleThreshold
        { set { pushAngleThreshold = value; } }

        /// <summary>
        /// 向前推的時候與軀幹的距離差設定,預設為300
        /// 數值越大推的時候手要越遠
        /// </summary>
        public int PushDistanceWithTorso
        { set { pushDistanceWithTorsoThreshold = value; } }
        ///////////////////////////////////////////////////////////////////////
        //揮砍部分
        /// <summary>
        /// 揮砍時,對於橫砍的瞬間速度臨界值設定,預設為120
        /// 數值越大手須要揮越快
        /// </summary>
        public int SashRightLeftVelocityThreshold
        { set { slashRightLeftVelocity = value; } }

        /// <summary>
        /// 揮砍時,對於垂直砍的瞬間速度臨界值設定,預設為-130(須為負值)
        /// 數值越大手須要揮越快
        /// </summary>
        public int SlashDownVelocityThreshold
        { set { slashDownVelocity = value; } }

        /// <summary>
        /// 揮砍時,對於橫砍的角度臨界值設定,預設為80
        /// 數值越大手須抬越高揮,越小可能會變成斜砍
        /// </summary>
        public int SlashHorizonAngleThreshold
        { set { slashMaxHorizonAngle = value; } }

        /// <summary>
        /// 揮砍時,對於垂直砍的角度臨界值設定,預設為30
        /// 數值越小容忍值會越低須砍越直,越大可能會變成斜砍
        /// </summary>
        public int SlashVerticalAngleThreshold
        { set { slashVerticalAngle = value; } }

        /// <summary>
        /// 揮砍時,對於斜砍的最大角度臨界值設定,預設為65
        /// 數值越大可能會變成和橫砍重疊,越小的話會使斜砍的範圍變小
        /// </summary>
        public int SlashRakeMaxAngleThreshold
        { set { slashRakeMaxAngleThreshold = value; } }

        /// <summary>
        /// 揮砍時,對於斜砍的最小角度臨界值設定,預設為35
        /// 數值越大會使斜砍的範圍變小,越小的話可能會變成和垂直砍重疊
        /// </summary>
        public int SlashRakeMinAngleThreshold
        { set { slashRakeMinAngleThreshold = value; } }

        /// <summary>
        /// 斜砍的X軸瞬間速度設定,預設為60
        /// </summary>
        public int SlashRakeRightLeftVelocityThreshold
        { set { slashRakeRightLeftVelocity = value; } }

        /// <summary>
        /// 斜砍的Y軸瞬間速度設定,預設為-60(須為負值)
        /// </summary>
        public int SlashRakeDownVelocityThreshold
        { set { slashRakeDownVelocity = value; } }
        ////////////////////////////////////////////////////////////////////////
        #endregion

        #region Member variable
        ////////////////////////////////////////////////////////////////////////
        
        //控制Thread是否執行用的布林值
        private bool isWorking;
        //記錄每一個使用者目前狀態的資料結構
        Dictionary<int, UserStateEnum> userCurrentState;
        KinectProcess kinectProcess;
        
        //布林值部分
        private bool[] isLeftSwing;         //跑步時,用來記錄左腳向上抬及向下放是否超臨界值,都有則為true
        private bool[] isRightSwing;         //跑步時,用來記錄右腳向上抬及向下放是否超臨界值,都有則為true
        
        //這部分的布林是減少table事件的次數(一旦進入stable後就設置避免下次再進入)
        private bool preIsRightSlash;
        private bool preIsLeftPush;
        private bool preIsRightPush;  
        private bool preIsJump;
        private bool preIsRun;
        private bool preIsPause;
        private bool preIsMove;
        //此列舉用來記錄目前的對於飛行的規則狀態
        private FlyBehaviorStateMessageEnum flyRuleState;
        //此列舉用來記錄目前的對於滑動的規則狀態
        private SwipeBehaviorStateMessageEnum SwipeRuleState;

        
        private int runCounter;                 //跑步用,紀錄目前frame的次數,30為一單位
        private int runStableCounter;           //跑步切換至stable用,紀錄目前有幾張frame是進入stable狀態,域設是兩張frame就發動stable事件
        private int curTotalSwingTimes;        //先在跑步的總踏步次數 
        private int preTotalSwingTimes;        //先前跑步的總踏步次數 
        
        //暫停用,記錄進入暫停的時間
        private int pauseInvokeTimeCounter;

        private Dictionary<SkeletonJoint, SkeletonJointPosition> jointNodeBufferForFly;     //計算飛行用的資料結構Buffer
        private Dictionary<SkeletonJoint, LinkedList<SkeletonJointPosition>> jointNodeBufferForRun;     //計算跑步用的資料結構Buffer
        private Dictionary<SkeletonJoint, LinkedList<SkeletonJointPosition>> jointNodeBufferForSpeed;   //計算揮砍及跳躍用的資料結構Buffer
        private Dictionary<SkeletonJoint, LinkedList<SkeletonJointPosition>> jointNodeBufferForPushAndDragAndSwipe;    //計算向前推及滑動e用的資料結構Buffer
        private Dictionary<SkeletonJoint, SkeletonJointPosition> jointNodeBufferForPauseAngel;  //計算暫停用的資料結構Buffer
        private Dictionary<SkeletonJoint, LinkedList<SkeletonJointPosition>> jointNodeForMove;  //計算左右移動用的資料結構Buffer
       
        /////////////////////
        //此部分為buffer的長度,單位為frame的量
        private int runTimeSpan = 2;
        private int runStableTimeSpan = 2; //代表左右腳各一張frame
        private int jumpAndSlashTimeSpan = 6;
        private int pushAndSwipeTimeSpan = 3;       
        private int pauseTimeSpan = 90;      //等待進入暫停為代表3秒(保持暫停姿勢到3秒)
        private int moveRightAndLeftTimeSpan = 6;

        
        ///////////////////////
        //為使用者骨架追蹤的可移動範圍區塊,為預設值
        private int userMaxTrackingPosition = 3800; //最遠可追蹤的距離
        private int userMaxWaringPosition = 3500;   //最遠進入警告的距離
        private int userMinWaringPosition = 2000;   //最近進入警告的距離
        private int userMinTrackingPosition = 1600; //最近可追蹤的距離,也是進入校正的最小範圍距離
        
        ///////////////////////
        //為使用者校正的範圍
        private int userMaxPoseDetectionPosition = 2600;   //進入校正的最大範圍距離
        private int userMinPoseDetectionPosition = 1600; //進入校正的最小範圍距離

        /////////////////////
        //向前推的部分,為預設值
        private int pushSpeedThreshold = -55;   //向前推的瞬間速度臨界值
        private int pushAngleThreshold = 40;    //向前推的角度容忍值
        private int pushDistanceWithTorsoThreshold = 250;   //向前推時手伸出的與骨架中心的距離臨界值

        /////////////////////
        //向上跳躍的部分,為預設值
        private int jumpVelocityThreshold = 40;    //也就是瞬間速度的臨界值

        //////////////////////
        //左右移動部分
        private int deviationBorder = 10;    //也就是平均速度的臨界值
        private int deviceXMaxBorder;
        private int deviceXMinBorder;
        ///////////////////////
        // 跑步部分
        private int runLegUpVelocity = 15;  //膝蓋向上抬的瞬間速度臨界值
        private int runLegDownVelocity = -12;   //膝蓋向下放的瞬間速度臨界值
        private double leftLegSpeed = 0;
        private double rightLegSpeed = 0;
        ///////////////////////
        // 揮砍部分
        private int slashRightLeftVelocity = 130;   //橫砍的X軸瞬間速度
        private int slashDownVelocity = -110;   //縱砍的Y軸瞬間速度
        private int slashMaxHorizonAngle = 110; //橫砍的水平角度(與假想的Y軸垂直向量做計算得知)
        private int slashMinHorizonAngle = 80; //橫砍的水平角度(與假想的Y軸垂直向量做計算得知)
        private int slashVerticalAngle = 30; //縱砍的水平角度(與假想的Y軸垂直向量做計算得知)
        private int slashRakeMaxAngleThreshold = 75; //斜砍的最大角度(與假想的Y軸垂直向量做計算得知)
        private int slashRakeMinAngleThreshold = 35; //斜砍的最小角度(與假想的Y軸垂直向量做計算得知)
        private int slashRakeRightLeftVelocity = 65; //斜砍的X軸瞬間速度
        private int slashRakeDownVelocity = -65;    //斜砍的Y軸瞬間速度

        //紀錄是否有移除骨架資料,用來判斷使用者離開和使用者遺失時是否帶有骨架
        private bool isRemoveJoint; 

        //ist can let "console.writeline" print to the console rather the output 
        private const int ATTACH_PARENT_PROCESS = -1;
        private ImageMetaData sourceImageMD;
        private Point3D headJoint; 
        ////////////////////////////////////////////////////////////////////////
        #endregion

        #region Public Fuction
        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 取得臉部的圖片
        /// </summary>
        /// <returns></returns>
        public Bitmap GetFaceBitmap()
        {
            return FaceImageAlgriothm();
        }
        /// <summary>
        /// 可以知道目前畫面上有多少使用者
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfUsers()
        {
            return kinectProcess.UserGenerator.NumberOfUsers;
        }

        /// <summary>
        /// 可以取得目前畫面上的所有使用者,透過迴圈即可一一取得各使用者
        /// </summary>
        /// <returns></returns>
        public int[] GetUsers()
        {
            return kinectProcess.UserGenerator.GetUsers();
        }
        
        /// <summary>
        /// 取得使用者中心在畫面上的位置;和GetDevicetMaxDepth()搭配即可畫出使用者所在的深度圖分布
        /// </summary>
        /// <param name="userId">使用者編號</param>
        /// <returns>此使用者的位置(實際3維座標)</returns>
        public Point3D GetUserPosition(int userId)
        {
            return kinectProcess.UserGenerator.GetCoM(userId);
        }

        /// <summary>
        /// 此功能可以得知Kinect深度裝置可以到的最遠距離;和GetUserPosition(int userId)的函式搭配即可畫出使用者所在的深度圖分布
        /// </summary>
        /// <returns></returns>
        public int GetDevicetMaxDepth()
        {
            return kinectProcess.GeneratorDepth.DeviceMaxDepth;
        }

       
        /// <summary>
        /// 可以取得使用者的深度像素,遊戲應該用不到
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public SceneMetaData GetUserPixels(int userId)
        {
            return kinectProcess.UserGenerator.GetUserPixels(userId);
        }
              
        /// <summary>
        /// 可以得到目前Kinect的深度畫面,遊戲應該用不到
        /// </summary>
        /// <returns></returns>
        public IntPtr DepthMapPtr()
        {
            return kinectProcess.GeneratorDepth.DepthMapPtr;
        }

        /// <summary>
        /// 可以把目前的平面座標點轉回三維資訊座標
        /// </summary>
        /// <param name="realWorldPoint"></param>
        /// <returns></returns>
        public Point3D ConvertProjectiveToRealWorld(Point3D projectivePoint)
        {
            return kinectProcess.GeneratorDepth.ConvertProjectiveToRealWorld(projectivePoint);
        }

        /// <summary>
        /// 可以把目前的三維座標點轉成平面座標點(但平面座標仍會有Z軸資訊,不影響)
        /// </summary>
        /// <param name="projectivePoint"></param>
        /// <returns>x,y是投影到平面畫面的座標點,z則仍是原三維的深度點</returns>
        public Point3D ConvertRealWorldToProjective(Point3D realWorldPoint)
        {
            return kinectProcess.GeneratorDepth.ConvertRealWorldToProjective(realWorldPoint);
        }

        /// <summary>
        ///使KinectActionModel執行(需要開執行緒)
        /// </summary>
        public void ModelDoWork()
        {
            this.Initiallize();
            DepthMetaData depthMD = new DepthMetaData();
            while (isWorking)
            {
                try { kinectProcess.UpdateKinectContext(); }
                catch (Exception) { }
                kinectProcess.GeneratorDepth.GetMetaData(depthMD);
                //kinectProcess.GeneratorImage.GetMetaData(sourceImageMD);
               
                int[] users = kinectProcess.UserGenerator.GetUsers();
                foreach (int user in users)
                {
                    Point3D userRealPos = kinectProcess.UserGenerator.GetCoM(user);
                    //是否正在校正
                        if (kinectProcess.SkeletonCapbility.IsCalibrating(user))
                        {
                            //prevent send event all the time
                            if (userCurrentState.Count != 0 && userCurrentState[user] != UserStateEnum.Calibrating)
                            {
                                userCurrentState[user] = UserStateEnum.Calibrating;
                                if (OnUserStateChanged != null)
                                { OnUserStateChanged(user, userCurrentState[user]); }
                            }
                        }
                        //若此人有骨架資源
                        else  if (kinectProcess.Joints.ContainsKey(user) )
                        {
                            //是否為正在追蹤
                            if (kinectProcess.SkeletonCapbility.IsTracking(user))
                            {
                                //使否在穩定範圍內
                                if (userRealPos.Z >  userMinWaringPosition  && userRealPos.Z < userMaxWaringPosition)
                                {
                                    kinectProcess.GetJoints(user);
                                    ///////////////////////////
                                    this.UpdateBehaviorCounter();
                                    this.JointUpdate(kinectProcess.Joints[user]);
                                    //////////////////////////
                                    if (OnJointsUpdate != null)
                                    { OnJointsUpdate.Invoke(kinectProcess.Joints[user]); }
                                    if (userCurrentState.Count != 0 && userCurrentState[user] != UserStateEnum.Tracking)
                                    {
                                        userCurrentState[user] = UserStateEnum.Tracking;
                                        if (OnUserStateChanged != null)
                                        { OnUserStateChanged.Invoke(user, userCurrentState[user]); }
                                    }

                                }
                                //使用者超出穩定範圍但仍可追蹤-> 在追蹤但是會警告
                                else if ((userRealPos.Z > userMinTrackingPosition && userRealPos.Z < userMinWaringPosition) || (userRealPos.Z > userMaxWaringPosition && userRealPos.Z < userMaxTrackingPosition))
                                {
                                    kinectProcess.GetJoints(user);
                                    ///////////////////////////
                                    this.UpdateBehaviorCounter();
                                    this.JointUpdate(kinectProcess.Joints[user]);
                                    //////////////////////////
                                    if (OnJointsUpdate != null)
                                    { OnJointsUpdate.Invoke(kinectProcess.Joints[user]); }

                                    if (userCurrentState.Count != 0 && userCurrentState[user] != UserStateEnum.TrackingButWaring)
                                    {
                                        userCurrentState[user] = UserStateEnum.TrackingButWaring;
                                        // set stop waring signal
                                        if (OnUserStateChanged != null)
                                        { OnUserStateChanged.Invoke(user, userCurrentState[user]); }
                                    }

                                }
                                //上訴兩個條件皆不是超出追蹤範圍
                                else
                                {
                                    //超出追蹤範圍且使用者深度不為0(仍畫面中)
                                    if ((userRealPos.Z < userMinTrackingPosition || userRealPos.Z > userMaxTrackingPosition) && userRealPos.Z != 0)
                                    {
                                        //set stop tracking signal
                                        kinectProcess.SkeletonCapbility.StopTracking(user);
                                        this.Reset();
                                        //重置事件發動
                                        if (OnReset != null)
                                        { OnReset(); }
                                    }
                                    else if(userRealPos.Z == 0 )
                                    {
                                        isRemoveJoint = kinectProcess.RemoveJointAndResource(user);
                                        if (OnUserStateChanged != null)
                                        {
                                            if (isRemoveJoint)
                                            {
                                                OnUserStateChanged.Invoke(user, UserStateEnum.JointUserMissed);
                                                Console.WriteLine(user.ToString() + " JointUserMissed");
                                            }
                                        }
                                        this.Reset();
                                        //重置事件發動
                                        if (OnReset != null)
                                        { OnReset(); }
                                    }
                                }
                            }
                            else //未在追蹤
                            {
                                if (userRealPos.Z > userMinTrackingPosition && userRealPos.Z < userMaxTrackingPosition)
                                {
                                    if (userCurrentState.Count != 0 && userCurrentState[user] != UserStateEnum.ReTracking)
                                    {
                                        userCurrentState[user] = UserStateEnum.ReTracking;
                                        //start tarcking
                                        try
                                        {
                                            kinectProcess.SkeletonCapbility.StartTracking(user);
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine("restart..........");
                                            kinectProcess.ReStart();
                                            userCurrentState = new Dictionary<int, UserStateEnum>();
                                            this.Reset();
                                            //重置事件發動
                                            if (OnReset != null)
                                            { OnReset(); }
                                        }
                                        if (OnUserStateChanged != null)
                                        { OnUserStateChanged(user, userCurrentState[user]); }
                                    }

                                }
                                else if ((userRealPos.Z < userMinTrackingPosition || userRealPos.Z > userMaxTrackingPosition))
                                {
                                    if (userCurrentState.Count != 0 && userCurrentState[user] != UserStateEnum.StopTracking)
                                    {
                                        userCurrentState[user] = UserStateEnum.StopTracking;
                                        if (OnUserStateChanged != null)
                                        { OnUserStateChanged(user, userCurrentState[user]); }
                                    }

                                }
                            }// is the user not tracking(because stop the tracking)
                        }// if: the user has the resource
                        //否則是正在尋找姿勢
                        else
                        {
                            if (userCurrentState.Count != 0 && userCurrentState[user] != UserStateEnum.LookingForPose)
                            {
                                userCurrentState[user] = UserStateEnum.LookingForPose;
                                if (OnUserStateChanged != null)
                                { OnUserStateChanged(user, userCurrentState[user]); }
                            }

                        }
                } //foreach loop  
                if (OnDepthMDUpdate != null)
                { OnDepthMDUpdate.Invoke(depthMD); }
            } // loop: thread run

        }

        ////////////////////////////////////////////////////////////////////////
        #endregion

        #region Constructor
        ////////////////////////////////////////////////////////////////////////
        public KinectActionModel()
        {
            kinectProcess = new KinectProcess(userMinPoseDetectionPosition,userMaxPoseDetectionPosition);
            userCurrentState = new Dictionary<int, UserStateEnum>();
            kinectProcess.OnNewUser += new UserEventHandler(kinectProcess_OnNewUser);
            kinectProcess.OnLostUser += new UserEventHandler(kinectProcess_OnLostUser);
            kinectProcess.OnUserExit +=new UserEventHandler(kinectProcess_OnUserExit);
            kinectProcess.OnUserReEnter +=new UserEventHandler(kinectProcess_OnUserReEnter);
            kinectProcess.OnUserTrackingResouceChangedtoAnotherUser +=new UserEventHandler(kinectProcess_OnUserTrackingResouceChangedtoAnotherUser);
            deviceXMaxBorder = kinectProcess.MapMode.XRes - 40;
            deviceXMinBorder = 40;
            //the part is console testing
            //Win32.AttachConsole(ATTACH_PARENT_PROCESS);
            //Win32.AllocConsole();
            
            Console.WriteLine("open Console For testing.......");
            Console.WriteLine();
            Console.WriteLine("-----------------------------------------");
        }
        ////////////////////////////////////////////////////////////////////////
        #endregion

        #region Private Function
        ////////////////////////////////////////////////////////////////////////

        #region GetFaceImage 演算法
        ////////////////////////////////////////////////////////////////////////
        private Bitmap FaceImageAlgriothm()
        {
           
            //取得已投射過的頭中心的座標
            Point3D projHeadPos = kinectProcess.GeneratorDepth.ConvertRealWorldToProjective(this.headJoint);
            //計算方形框的比例:預設深度2000為100*100
            float ratio = 2000 / projHeadPos.Z;
            float faceROI_Width = 100 * ratio;
            float faceROI_Height = 100 * ratio;
            //Point startPos = new Point(Convert.ToInt32(projHeadPos.X - faceROI_Width / 2), Convert.ToInt32(projHeadPos.Y - faceROI_Height / 2));
            //Rectangle cropArea = new Rectangle(startPos, new Size(Convert.ToInt32(faceROI_Width), Convert.ToInt32(faceROI_Height)));
            //轉換成bitmap類別
            Bitmap imageBitmap = ConvertImageMetaDataToBitmap(this.sourceImageMD);
            //抓出臉部的圖像
           // Bitmap faceImage = imageBitmap.Clone(cropArea, imageBitmap.PixelFormat);
            return imageBitmap;
            
            
        }
        private Bitmap ConvertImageMetaDataToBitmap(ImageMetaData ImageMD)
        {

            unsafe
            {
                Rectangle rect = new Rectangle(0, 0, kinectProcess.MapMode.XRes, kinectProcess.MapMode.YRes);
                //create an empty bitmap the same size as original
                Bitmap imageMapBitmap = new Bitmap(kinectProcess.MapMode.XRes, kinectProcess.MapMode.YRes, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                
                //lock the new bitmap in memory
                BitmapData imageData = imageMapBitmap.LockBits(
                   new Rectangle(0, 0, imageMapBitmap.Width, imageMapBitmap.Height),
                   ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                //取得RGB 像素
                MapData<RGB24Pixel> pImageRGB = ImageMD.GetRGB24ImageMap(); 
                for (int y = 0; y < ImageMD.YRes; y++)
                {
                    byte* pDest = (byte*)imageData.Scan0.ToPointer() + y * imageData.Stride;
                    for (int x = 0; x < ImageMD.XRes; x++)
                    {
                        //對應到bitmap位置
                        *(pDest++) = pImageRGB[x,y].Blue; // blue 
                        *(pDest++) = pImageRGB[x, y].Green; // green 
                        *(pDest++) = pImageRGB[x, y].Red; // red 
                    }
                }
                //unlock the bitmaps
                imageMapBitmap.UnlockBits(imageData);
                return imageMapBitmap;
            }
        }
        ////////////////////////////////////////////////////////////////////////
        #endregion

        #region KinectProcess event handler
        ////////////////////////////////////////////////////////////////////////\
        private void kinectProcess_OnUserTrackingResouceChangedtoAnotherUser(int userId)
        {
            Console.WriteLine("Skeleton resource owner was Changed");
            if (OnUserStateChanged != null)
            {
                userCurrentState[userId] = UserStateEnum.LookingForPose;
                OnUserStateChanged.Invoke(userId, UserStateEnum.LookingForPose);
            }
            this.Reset();
            //重置事件發動
            if (OnReset != null)
            { OnReset(); }
        }

        private void kinectProcess_OnUserExit(int userId)
        {
            isRemoveJoint = kinectProcess.RemoveJointAndResource(userId);
            if (OnUserStateChanged != null)
            {
                if (isRemoveJoint)
                {
                    userCurrentState[userId] = UserStateEnum.JointUserExit;
                    OnUserStateChanged.Invoke(userId, UserStateEnum.JointUserExit);
                    Console.WriteLine("number "+ userId.ToString() + " JointUserExit");
                }
                else
                {
                    userCurrentState[userId] = UserStateEnum.UserExit;
                    OnUserStateChanged.Invoke(userId, UserStateEnum.UserExit);
                }
            }
            this.Reset();
            //重置事件發動
            if (OnReset != null)
            { OnReset(); }

        }
        private void kinectProcess_OnUserReEnter(int userId)
        {
            if (!userCurrentState.ContainsKey(userId))
            {
                userCurrentState.Add(userId, UserStateEnum.UserReEnter);
            }
            else
            {
                userCurrentState[userId] = UserStateEnum.UserReEnter;
            }
            if (OnUserStateChanged != null)
            {
                OnUserStateChanged.Invoke(userId, UserStateEnum.UserReEnter);
            }

        }
        private void kinectProcess_OnNewUser(int userId)
        {
            userCurrentState.Add(userId, UserStateEnum.LookingForPose);
            if (OnUserStateChanged != null)
            {
                OnUserStateChanged.Invoke(userId, UserStateEnum.NewUser);
            }
        }
        private void kinectProcess_OnLostUser(int userId)
        {
            userCurrentState.Remove(userId);
            isRemoveJoint = kinectProcess.RemoveJointAndResource(userId);
            if (OnUserStateChanged != null)
            {
                //如果失去的使用者式帶有骨架的話,則使用者狀態設為JointUserLost
                if (isRemoveJoint)
                {
                    userCurrentState[userId] = UserStateEnum.JointUserLost;
                    OnUserStateChanged.Invoke(userId, UserStateEnum.JointUserLost);
                    Console.WriteLine("number " + userId.ToString() + " JointUserLost");
                }
                else
                {
                    userCurrentState[userId] = UserStateEnum.LostUser;
                    OnUserStateChanged.Invoke(userId, UserStateEnum.LostUser);
                }
            }
            this.Reset();
            //重置事件發動
            if (OnReset != null)
            { OnReset(); }
        }
        ////////////////////////////////////////////////////////////////////////\
        #endregion

        #region Initillize and reset 
        ////////////////////////////////////////////////////////////////////////
        //初始化資料當使用者開始追蹤時
        private void Initiallize()
        {
            flyRuleState = FlyBehaviorStateMessageEnum.Init;
            SwipeRuleState = SwipeBehaviorStateMessageEnum.Init;
            sourceImageMD = new ImageMetaData();
            jointNodeBufferForRun = new Dictionary<SkeletonJoint, LinkedList<SkeletonJointPosition>>();
            jointNodeBufferForPushAndDragAndSwipe = new Dictionary<SkeletonJoint, LinkedList<SkeletonJointPosition>>();
            jointNodeBufferForSpeed = new Dictionary<SkeletonJoint, LinkedList<SkeletonJointPosition>>();
            jointNodeBufferForPauseAngel = new Dictionary<SkeletonJoint, SkeletonJointPosition>();
            jointNodeBufferForFly = new Dictionary<SkeletonJoint, SkeletonJointPosition>();
            jointNodeForMove = new Dictionary<SkeletonJoint, LinkedList<SkeletonJointPosition>>();

            //布林部分
            preIsPause = false;
            preIsRightSlash = false;
            preIsLeftPush = false;
            preIsRightPush = false;
            preIsJump = false;
            preIsRun = false;
            preIsMove = false;

            //跑步部分
            curTotalSwingTimes = 0;
            preTotalSwingTimes = 0;
            runStableCounter = 0;
            runCounter = 0;
            isLeftSwing = new bool[2];
            isRightSwing = new bool[2];
            //暫停部分
            pauseInvokeTimeCounter = 0;
        }
        //重置資料當使用者停止追蹤時
        private void Reset()
        {
            //Buffer資料清空
            jointNodeBufferForRun.Clear();
            jointNodeBufferForPushAndDragAndSwipe.Clear();
            jointNodeBufferForSpeed.Clear();
            jointNodeBufferForPauseAngel.Clear();
            jointNodeBufferForFly.Clear();
            jointNodeForMove.Clear();

            flyRuleState = FlyBehaviorStateMessageEnum.Init;
            SwipeRuleState = SwipeBehaviorStateMessageEnum.Init;

            //布林部分
            preIsPause = false;
            preIsRightSlash = false;
            preIsLeftPush = false;
            preIsRightPush = false;
            preIsJump = false;
            preIsRun = false;
            preIsMove = false;

            //跑步部分
            curTotalSwingTimes = 0;
            preTotalSwingTimes = 0;
            runStableCounter = 0;
            runCounter = 0;
            isLeftSwing = new bool[2];
            isRightSwing = new bool[2];
            //暫停部分
            pauseInvokeTimeCounter = 0;
        }
        ////////////////////////////////////////////////////////////////////////
        #endregion        

        #region Update JointData for calculate
        //////////////////////////////////////////////
        //依據要計算的演算法來收集關節的資料
        private void JointUpdate(Dictionary<SkeletonJoint, SkeletonJointPosition> userJointPos)
        {
            //取得每一個關節的資料
            foreach (KeyValuePair<SkeletonJoint, SkeletonJointPosition> jpos in userJointPos)
            {
                //過濾骨架的座標
               // CollectFilterData(jpos.Key, jpos.Value);
                //關節的可信度是否大於0.5
                if (jpos.Value.Confidence > 0.5)
                {
                    if (jpos.Key == SkeletonJoint.Head) { headJoint = jpos.Value.Position; }
                    //////////////////////////////////////////////////////////////////////
                    //Swipe/Push
                    if (jpos.Key == SkeletonJoint.RightHand || jpos.Key == SkeletonJoint.LeftHand || jpos.Key == SkeletonJoint.Torso )
                    {
                        if (OnSwipe != null || OnHandPush != null)
                        {
                            UpdatePushAndDragAndSwipeData(jpos.Key, jpos.Value);
                        }
                    }
                    //////////////////////////////////////////////////////////////////////
                    //Slash/Jump
                    if (jpos.Key == SkeletonJoint.RightHand || jpos.Key == SkeletonJoint.Torso || jpos.Key == SkeletonJoint.RightShoulder) 
                    {
                        if (OnHandSlash != null || OnJump != null)
                        {
                            UpdateSpeedData(jpos.Key, jpos.Value);
                        }
                    }
                    //////////////////////////////////////////////////////////////////////
                    //Fly
                    if (jpos.Key == SkeletonJoint.RightHand || jpos.Key == SkeletonJoint.LeftHand || jpos.Key == SkeletonJoint.LeftElbow 
                        || jpos.Key == SkeletonJoint.RightElbow || jpos.Key == SkeletonJoint.LeftShoulder || jpos.Key == SkeletonJoint.RightShoulder 
                        || jpos.Key == SkeletonJoint.Head || jpos.Key == SkeletonJoint.Torso)
                    {
                        if (OnFly != null)
                        {
                            UpdateFlyData(jpos.Key, jpos.Value);
                        }
                    }
                    //////////////////////////////////////////////////////////////////////
                    //Run: 
                    if (jpos.Key == SkeletonJoint.LeftKnee || jpos.Key == SkeletonJoint.RightKnee)
                    {
                        if (OnFootRun != null)
                        {
                            UpdateRunData(jpos.Key, jpos.Value);
                        }
                    }
                    //////////////////////////////////////////////////////////////////////
                    //Pause: 
                    if (jpos.Key == SkeletonJoint.LeftShoulder || jpos.Key == SkeletonJoint.LeftHand || jpos.Key ==  SkeletonJoint.LeftElbow)
                    {
                        if (OnPause != null)
                        {
                            UpdatePauseData(jpos.Key, jpos.Value);
                        }
                    }
                    ////////////////////////////////////////////////////////////////////
                    // move left or right
                    if (jpos.Key == SkeletonJoint.Torso)
                    {
                        if (OnMoveLeftOrRight != null)
                        {
                            UpdateMoveData(jpos.Key, jpos.Value);
                        }
                    }
                }
            }
        }
        //////////////////////////////////////////////
        #endregion
        
        #region Move 演算法
        //////////////////////////////////////////////
        //更新資料
        private void UpdateMoveData(SkeletonJoint j, SkeletonJointPosition pos)
        {
            if (!jointNodeForMove.ContainsKey(j))
            {
                jointNodeForMove.Add(j, new LinkedList<SkeletonJointPosition>());
                jointNodeForMove[j].AddLast(pos);
            }
            else
            {
                //若未收集到足夠的關節資料
                if (jointNodeForMove[j].Count != moveRightAndLeftTimeSpan)
                { jointNodeForMove[j].AddLast(pos); }
                else
                {
                    //如果有註冊此演算法事件
                    if (OnMoveLeftOrRight != null )
                    { CalculateMoveVelocityTrigger(jointNodeForMove[j]); }
                    //移除較早的關節資料
                    jointNodeForMove[j].RemoveFirst();
                    jointNodeForMove[j].RemoveFirst();
                    jointNodeForMove[j].RemoveFirst();
                    jointNodeForMove[j].AddLast(pos);
                }
            }
        }
        //////////////////////////////////////////////
        //演算法
        private void CalculateMoveVelocityTrigger(LinkedList<SkeletonJointPosition> listPos)
        {
            SkeletonJointPosition lastestPos = listPos.Last.Value;
            SkeletonJointPosition firstPos = listPos.First.Value;
            SkeletonJointPosition lastestPrevPos = listPos.Last.Previous.Value;
            //////////////////////////////////////////////////////////////
            //計算誤差的邊界值
            double deviation = lastestPos.Position.X - lastestPrevPos.Position.X;
            
            Point3D currentPosition =  kinectProcess.GeneratorDepth.ConvertRealWorldToProjective(lastestPos.Position);
            //如果有超過誤差邊界值代表有移動(誤差值是因為攝影機取得的資料會有些微的不準確)
            if ((Math.Abs(deviation) > deviationBorder) && currentPosition.X > deviceXMinBorder && currentPosition.X < deviceXMaxBorder)
            {
                    preIsMove = true;
                    OnMoveLeftOrRight.Invoke(new MoveBehaviorData(deviceXMaxBorder, deviceXMinBorder, kinectProcess.MapMode.XRes/2 , currentPosition));
            } 
            else //stable...
            {
                if (preIsMove)
                {
                    preIsMove = false;
                    if (OnMoveLeftOrRightStable != null)
                    { OnMoveLeftOrRightStable.Invoke(); }
                }
            }
        }
        /////////////////////////////////////////////////
        #endregion

        #region Pause part 演算法
        //////////////////////////////////////////////////////////////////////
        //更新資料
        private void UpdatePauseData(SkeletonJoint j, SkeletonJointPosition pos)
        {
            if (!jointNodeBufferForPauseAngel.ContainsKey(j))
            {
                jointNodeBufferForPauseAngel.Add(j, pos);
            }
            else
            {
                //若有收集到此三個節點則計算
                if (jointNodeBufferForPauseAngel.ContainsKey(SkeletonJoint.LeftShoulder) && jointNodeBufferForPauseAngel.ContainsKey(SkeletonJoint.LeftHand) && jointNodeBufferForPauseAngel.ContainsKey(SkeletonJoint.LeftElbow)) 
                {
                    if (OnPause != null)
                    {
                        { CalculatePauseAngelTrigger(jointNodeBufferForPauseAngel); }
                    }
                    //更新節點資料
                    jointNodeBufferForPauseAngel[j] = pos;
                }
            }
        }
        //計算二為空間中兩向量的夾角
        private double CalculateVector2DAngle(Vector2D vectorA, Vector2D vectorB)
        {
            double dotProduct = (double)(vectorA.X * vectorB.X) + (double)(vectorA.Y * vectorB.Y);
            double vectorALength = Math.Sqrt(Math.Pow(vectorA.X, 2) + Math.Pow(vectorA.Y, 2));
            double vectorBLength = Math.Sqrt(Math.Pow(vectorB.X, 2) + Math.Pow(vectorB.Y, 2));
            double cosTheta = dotProduct / (vectorALength * vectorBLength);
            double angle = (Math.Acos(cosTheta)) * 180 / Math.PI;
            return angle;
        }
        //////////////////////////////////////////////
        //演算法
        private void CalculatePauseAngelTrigger(Dictionary<SkeletonJoint, SkeletonJointPosition> jpos)
        {
            float x;
            float y;
            float z;
            //架設一個點座標為肩膀向下往y軸延伸(-300),為了計算肩膀到此點的向量(算x和y軸夾角)
            Point3D horizonPoint = new Point3D(jpos[SkeletonJoint.LeftShoulder].Position.X, jpos[SkeletonJoint.LeftShoulder].Position.Y - 300, jpos[SkeletonJoint.LeftShoulder].Position.Z);
            //架設一個點座標為肩膀向前往Z軸延伸(-300),為了計算肩膀到此點的向量(算x和z軸夾角)
            Point3D verticalPoint = new Point3D(jpos[SkeletonJoint.LeftShoulder].Position.X, jpos[SkeletonJoint.LeftShoulder].Position.Y, jpos[SkeletonJoint.LeftShoulder].Position.Z - 300);
            
            //計算向量 
            //左肩到左手的向量
            x = jpos[SkeletonJoint.LeftHand].Position.X - jpos[SkeletonJoint.LeftShoulder].Position.X;
            y = jpos[SkeletonJoint.LeftHand].Position.Y - jpos[SkeletonJoint.LeftShoulder].Position.Y;
            z = jpos[SkeletonJoint.LeftHand].Position.Z - jpos[SkeletonJoint.LeftShoulder].Position.Z;
            //此向量用來計算水平角度
            Vector2D leftShoulderToLeftHandVectorForHorizonAngle = new Vector2D(x, y);
            //此向量用來計算垂直角度
            Vector2D leftShoulderToleftHandVectorForVerticalAngle = new Vector2D(x, z);
            //計算左肩到左肩往y軸向下延伸的向量
            x = horizonPoint.X - jpos[SkeletonJoint.LeftShoulder].Position.X;
            y = horizonPoint.Y - jpos[SkeletonJoint.LeftShoulder].Position.Y;
            Vector2D leftShoulderToHorizonPointVector = new Vector2D(x, y);
            //計算夾角
            double horizonAngle = CalculateVector2DAngle(leftShoulderToHorizonPointVector, leftShoulderToLeftHandVectorForHorizonAngle);

            //計算左肩到左肩往z軸向前延伸的向量
            x = horizonPoint.X - jpos[SkeletonJoint.LeftShoulder].Position.X;
            z = verticalPoint.Z - jpos[SkeletonJoint.LeftShoulder].Position.Z;
            Vector2D leftShoulderToVerticalPointVector = new Vector2D(x, z);
            //計算夾角
            double verticalAngle = CalculateVector2DAngle(leftShoulderToVerticalPointVector, leftShoulderToleftHandVectorForVerticalAngle);

            //計算左肘到左手的向量和左肘到左肩的向量,算三維的
            //1.左肘到左手的向量
            x = jpos[SkeletonJoint.LeftHand].Position.X - jpos[SkeletonJoint.LeftElbow].Position.X;
            y = jpos[SkeletonJoint.LeftHand].Position.Y - jpos[SkeletonJoint.LeftElbow].Position.Y;
            z = jpos[SkeletonJoint.LeftHand].Position.Z - jpos[SkeletonJoint.LeftElbow].Position.Z;
            Vector3D leftElbowToLeftHand = new Vector3D(x, y, z);

            //2.左肘到左肩的向量
            x = jpos[SkeletonJoint.LeftShoulder].Position.X - jpos[SkeletonJoint.LeftElbow].Position.X;
            y = jpos[SkeletonJoint.LeftShoulder].Position.Y - jpos[SkeletonJoint.LeftElbow].Position.Y;
            z = jpos[SkeletonJoint.LeftShoulder].Position.Z - jpos[SkeletonJoint.LeftElbow].Position.Z;
            Vector3D leftElbowToLeftShoulder = new Vector3D(x, y, z);
            //計算兩個向量的夾角-->左肘的夾角
            double leftElbowAngleBetweenLefHandAndLeftShoulder = CalculateVector3DAngle(leftElbowToLeftHand, leftElbowToLeftShoulder);
            //////////////////////
            if (OnPauseAngleChanged != null)
            {
                //tester message
                OnPauseAngleChanged.Invoke(verticalAngle, "vertical");
                OnPauseAngleChanged.Invoke(horizonAngle, "horizon");
            }
            // 如果水平及垂直夾角皆位於85到105度
            if (horizonAngle < PAUSE_HORIZON_ANGLE_UPPER_THRESHOLD && horizonAngle > PAUSE_HORIZON_ANGLE_LOWER_THRESHOLD && verticalAngle < PAUSE_VERTICAL_ANGLE_UPPER_THRESHOLD && verticalAngle > PAUSE_VERTICAL_ANGLE_LOWER_THRESHOLD)
            {
                //並且手肘的夾角要大於140
                if (leftElbowAngleBetweenLefHandAndLeftShoulder > 140)
                {
                    preIsPause = true;
                    pauseInvokeTimeCounter++;
                    //發動進入暫停的等待時間
                    OnWaitingPause.Invoke(pauseInvokeTimeCounter);
                    //如果時間滿足臨界值
                    if (pauseInvokeTimeCounter == pauseTimeSpan)
                    {
                        //發動等待事件
                        if (OnWaitingPause != null)
                        {
                            OnPause.Invoke(verticalAngle, horizonAngle);
                        }
                        pauseInvokeTimeCounter = 0;
                    }
                }
            }
            else
            {
                if (preIsPause)
                {
                    preIsPause = false;
                    pauseInvokeTimeCounter = 0;
                    OnNotPause.Invoke();
                }
            }
        }
        //////////////////////////////////////////////////////////////////////
        #endregion

        #region Run part 演算法
        //////////////////////////////////////////////////////////////////////
        //更新資料
        private void UpdateRunData(SkeletonJoint j, SkeletonJointPosition pos)
        {
            if (!jointNodeBufferForRun.ContainsKey(j))
            {
                jointNodeBufferForRun.Add(j, new LinkedList<SkeletonJointPosition>());
                jointNodeBufferForRun[j].AddLast(pos);
            }
            else
            {
                if (jointNodeBufferForRun[j].Count != runTimeSpan)
                    jointNodeBufferForRun[j].AddLast(pos);
                else
                {
                    if (OnFootRun != null)
                    {
                        CalculateRunTimesTrigger(j, jointNodeBufferForRun[j]);
                    }
                    jointNodeBufferForRun[j].RemoveFirst();
                    jointNodeBufferForRun[j].AddLast(pos);
                }
            }
        }
        //////////////////////////////////////////////
        //演算法
        private void CalculateRunTimesTrigger(SkeletonJoint j, LinkedList<SkeletonJointPosition> listPos)
        {
            SkeletonJointPosition lastestPos = listPos.Last.Value;
            SkeletonJointPosition firstPos = listPos.First.Value;
            SkeletonJointPosition lastestPrevPos = listPos.Last.Previous.Value;
            //計算速度
            double run_ivy = (lastestPos.Position.Y - lastestPrevPos.Position.Y);
            double run_ivz = (lastestPos.Position.Z) - (lastestPrevPos.Position.Z);
            double run_averVy = (lastestPos.Position.Y - firstPos.Position.Y) / runTimeSpan;
            double run_averVz = (lastestPos.Position.Z - firstPos.Position.Z) / runTimeSpan;
            double runSpeed = 0;
            //已30張frame(1秒)為單位重置資料
            if ((runCounter % 30) == 0)
            {
                //重置
                curTotalSwingTimes = 0;
                runStableCounter = 0;
            }
            //判斷膝蓋
            if (j == SkeletonJoint.LeftKnee)
            {
                //判斷速度
                if (run_ivy > runLegUpVelocity)
                {
                    leftLegSpeed = run_ivy;
                    isLeftSwing[0] = true;
                }
                //速度有過臨界值且之前有向上抬膝蓋
                else if (run_ivy < runLegDownVelocity && isLeftSwing[0]) isLeftSwing[1] = true;
                //Console.WriteLine("LeftKnee:\nbool right[0]= {0} ", isLeftSwing[0]);
                //Console.WriteLine("bool left[1]= {0} ", isLeftSwing[1]);
                //若皆為true則增加步數且重置布林值(有向上抬且有向下放)
                if (isLeftSwing[0] && isLeftSwing[1])
                {
                    curTotalSwingTimes++;
                    runSpeed = rightLegSpeed + leftLegSpeed;
                    isLeftSwing[0] = false;
                    isLeftSwing[1] = false;
                }
                //Console.WriteLine("ivy = {0}, ivz={1}", run_ivy, run_ivz);
                //Console.WriteLine("avy = {0}, avz ={1}", run_averVy, run_averVz);
                //Console.WriteLine();
            }
            else if (j == SkeletonJoint.RightKnee)
            {
                if (run_ivy > runLegUpVelocity)
                {
                    rightLegSpeed = run_ivy;
                    isRightSwing[0] = true;
                }
                else if (run_ivy < runLegDownVelocity && isRightSwing[0]) isRightSwing[1] = true;
                //Console.WriteLine("RightKnee:\nbool left[0]= {0} ", isRightSwing[0]);
                //Console.WriteLine("bool right[1]= {0} ", isRightSwing[1]); 
                //若皆為true則增加步數且重置布林值(有向上抬且有向下放)
                if (isRightSwing[0] && isRightSwing[1])
                {
                    curTotalSwingTimes++;
                    runSpeed = rightLegSpeed + leftLegSpeed;
                    isRightSwing[0] = false;
                    isRightSwing[1] = false;
                }
                //Console.WriteLine("ivy = {0}, ivz={1}", run_ivy, run_ivz);
                //Console.WriteLine("avy = {0}, avz={1}", run_averVy, run_averVz);
                //Console.WriteLine();
            }
            //Console.WriteLine("curent run times = {0}", curTotalSwingTimes);
            //Console.WriteLine("previous run times = {0}", preTotalSwingTimes);
            //表示步伐數有更新
            if (preTotalSwingTimes != curTotalSwingTimes)
            {
                preIsRun = true;
                OnFootRun(runSpeed);
            }
            //若未更新步伐數,計算沒有跑的時間
            else
            {
                //累積時間
                runStableCounter++;
                //若時間到達臨界值則發動事件
                if (runStableCounter == runStableTimeSpan)
                {
                    if (preIsRun)
                    {
                        preIsRun = false;
                        if (OnFootRunStable != null)
                        { OnFootRunStable.Invoke(); }
                    }
                }
            }
            //紀錄目前的步伐數以便下次用來做比較
            preTotalSwingTimes = curTotalSwingTimes;

        }
        //更新跑步的記錄frame
        private void UpdateBehaviorCounter()
        {
            if (OnFootRun != null && OnFootRunStable != null)
            { this.runCounter++; }

        }
        //////////////////////////////////////////////////////////////////////
        #endregion

        #region Jump part 演算法
        //////////////////////////////////////////////////////////////////////// 
        //更新資料(slash和jump)
        private void UpdateSpeedData(SkeletonJoint j, SkeletonJointPosition pos)
        {
            if (!jointNodeBufferForSpeed.ContainsKey(j))
            {
                jointNodeBufferForSpeed.Add(j, new LinkedList<SkeletonJointPosition>());
                jointNodeBufferForSpeed[j].AddLast(pos);
            }
            else
            {
                if (jointNodeBufferForSpeed[j].Count != jumpAndSlashTimeSpan)
                { jointNodeBufferForSpeed[j].AddLast(pos); }
                else
                {
                    if (jointNodeBufferForSpeed.Count == 3)
                    {
                        //if registerd slash
                        if (OnHandSlash != null && jointNodeBufferForSpeed[SkeletonJoint.RightShoulder].Count != 0 && jointNodeBufferForSpeed[SkeletonJoint.RightHand].Count == jumpAndSlashTimeSpan)
                        { CalculateSlashVelocityTrigger(jointNodeBufferForSpeed); }
                        //if registerd jump
                        if (OnJump != null && j == SkeletonJoint.Torso)
                        { CalculateJumpTrigger(jointNodeBufferForSpeed[j]); }
                        jointNodeBufferForSpeed[j].RemoveFirst();
                        jointNodeBufferForSpeed[j].RemoveFirst();
                        jointNodeBufferForSpeed[j].RemoveFirst();
                        jointNodeBufferForSpeed[j].AddLast(pos);
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////
        //演算法
        private void CalculateJumpTrigger(LinkedList<SkeletonJointPosition> listPos)
        {
            SkeletonJointPosition lastestPos = listPos.Last.Value;
            SkeletonJointPosition firstPos = listPos.First.Value;
            SkeletonJointPosition lastestPrevPos = listPos.Last.Previous.Value;
            //////////////////////////////////////////////////////////////
            //計算
            double jump_ivy = lastestPos.Position.Y - lastestPrevPos.Position.Y;
            double JumpDy = lastestPos.Position.Y - firstPos.Position.Y;
            //若瞬間速度滿足臨界值
            if ((jump_ivy > jumpVelocityThreshold))
            {
                preIsJump = true;
                //Console.WriteLine("JumpDy = {0}", JumpDy);
                OnJump.Invoke(new JumpBehaviorData(jump_ivy, JumpDy, listPos));
            }
            else //stable...
            {
                if (preIsJump)
                {
                    preIsJump = false;
                    if (OnJumpStable != null)
                    { OnJumpStable.Invoke(); }
                }
            }

        }

        //////////////////////////////////////////////////////////////////////
        #endregion

        #region Push/Drag/Swipe Part
        ////////////////////////////////////////////////////////////////////////
        //更新資料(push和swipe)
        private void UpdatePushAndDragAndSwipeData(SkeletonJoint j, SkeletonJointPosition pos)
        {
            if (!jointNodeBufferForPushAndDragAndSwipe.ContainsKey(j))
            {
                jointNodeBufferForPushAndDragAndSwipe.Add(j, new LinkedList<SkeletonJointPosition>());
                jointNodeBufferForPushAndDragAndSwipe[j].AddLast(pos);
            }
            else
            {
                if (jointNodeBufferForPushAndDragAndSwipe[j].Count != pushAndSwipeTimeSpan)
                { jointNodeBufferForPushAndDragAndSwipe[j].AddLast(pos); }
                else
                {
                    if (jointNodeBufferForPushAndDragAndSwipe.Count == 3)
                    {
                        if (OnHandPush != null && (jointNodeBufferForPushAndDragAndSwipe[SkeletonJoint.RightHand].Count == pushAndSwipeTimeSpan || jointNodeBufferForPushAndDragAndSwipe[SkeletonJoint.LeftHand].Count == pushAndSwipeTimeSpan) && jointNodeBufferForPushAndDragAndSwipe[SkeletonJoint.Torso].Count != 0)
                        { CalculatePushVelocityTrigger(j, jointNodeBufferForPushAndDragAndSwipe); }
                        //do swipe Algriothm
                        if (OnSwipe != null && jointNodeBufferForPushAndDragAndSwipe[SkeletonJoint.RightHand].Count == pushAndSwipeTimeSpan && jointNodeBufferForPushAndDragAndSwipe[SkeletonJoint.Torso].Count == pushAndSwipeTimeSpan)
                        { CalculateSwipeTrigger(jointNodeBufferForPushAndDragAndSwipe); }
                    }
                    jointNodeBufferForPushAndDragAndSwipe[j].RemoveFirst();
                    jointNodeBufferForPushAndDragAndSwipe[j].RemoveFirst();
                    jointNodeBufferForPushAndDragAndSwipe[j].AddLast(pos);
                }
            }

        }
        #region swipe part
        /////////////////////////////////////////////////////////////////////////
        //swipe part
        private void CalculateSwipeTrigger(Dictionary<SkeletonJoint, LinkedList<SkeletonJointPosition>> listPos)
        {
            SkeletonJointPosition lastestPos = listPos[SkeletonJoint.RightHand].Last.Value;
            SkeletonJointPosition firstPos = listPos[SkeletonJoint.RightHand].First.Value;
            SkeletonJointPosition lastestPrevPos = listPos[SkeletonJoint.RightHand].Last.Previous.Value;
            SkeletonJointPosition torsoPoint = listPos[SkeletonJoint.Torso].Last.Previous.Value;

            //1.calculate ivy,ivx
            double swipe_ivy = (lastestPos.Position.Y) - (lastestPrevPos.Position.Y);
            double swipe_ivx = (lastestPos.Position.X) - (lastestPrevPos.Position.X);
            //calculate displacement y,x
            double swipe_disy = (lastestPos.Position.Y) - (firstPos.Position.Y);
            double swipe_disx = (lastestPos.Position.X) - (firstPos.Position.X);
            //length ok
            if (lastestPos.Position.Z < (torsoPoint.Position.Z - SWIPE_TORSO_DISTANCE_THRESHOLD))
            {
                //2.check
                if (Math.Abs(swipe_disy) > SWIPE_UP_DOWN_DISPLACEMENT && Math.Abs(swipe_disx) < SWIPE_RIGHT_LEFT_DISPLACEMENT)
                {
                    float x;
                    float y;
                    float z;
                    //1.calculate virtual point (first.x + 300,first.y ,first.z)
                    Point3D xAxiesVirtualPoint = new Point3D(firstPos.Position.X + SWIPE_VIRTUAL_POINT_DISTANCE, firstPos.Position.Y, firstPos.Position.Z);
                    //2.calculate vector(x,y) comprised by first to virtual point 
                    x = xAxiesVirtualPoint.X - firstPos.Position.X;
                    y = xAxiesVirtualPoint.Y - firstPos.Position.Y;
                    Vector2D yOriVirtualVector = new Vector2D(x, y);
                    //3.calculate  vector(x,y) comprised by first point to current point
                    x = lastestPos.Position.X - firstPos.Position.X;
                    y = lastestPos.Position.Y - firstPos.Position.Y;
                    Vector2D xyOriFirstPosToLastPosVector = new Vector2D(x, y);
                    //4.calculate angle comprised by swipe up(x orientation vector) and virtual vector(y orientation vector) angle
                    double xyOriAngle = CalculateVector2DAngle(xyOriFirstPosToLastPosVector, yOriVirtualVector);

                    //5.calculate virtual point (first.x,first.y ,first.z -300)
                    Point3D zAxiesVirtualPoint = new Point3D(firstPos.Position.X, firstPos.Position.Y, firstPos.Position.Z - SWIPE_VIRTUAL_POINT_DISTANCE);
                    //6.calculate vector(x,z) comprised by first to virtual point 
                    x = zAxiesVirtualPoint.X - firstPos.Position.X;
                    z = zAxiesVirtualPoint.Z - firstPos.Position.Z;
                    Vector2D zOriVirtualVector = new Vector2D(x, z);
                    //7.calculate vector(x,z) comprised by first point to current point
                    x = lastestPos.Position.X - firstPos.Position.X;
                    z = lastestPos.Position.Z - firstPos.Position.Z;
                    Vector2D xzOriFirstPosToLastPosVector = new Vector2D(x, z);
                    //8.calculate swipe up(x orientation vector) and virtual vector(z orientation vector) angle
                    double xzOriAngle = CalculateVector2DAngle(xzOriFirstPosToLastPosVector, zOriVirtualVector);

                    //test
                    if (OnSwipeAngleChanged != null)
                    {
                        OnSwipeAngleChanged(xzOriAngle, "AngleBetweenVerticalOrientation");
                        OnSwipeAngleChanged(xyOriAngle, "SwipeUpDownAngleBetweenHorizonOrientation");

                    }
                    //swipe up (value is positive)
                    if (swipe_disy > CHECK_VELOCITY_ORIENTAION)
                    {
                        //9.check angle -> horizon
                        if (xyOriAngle > SWIPE_UP_DOWN_ANGLE_THRESHOLD && xyOriAngle < 180 - SWIPE_UP_DOWN_ANGLE_THRESHOLD)
                        {
                            //check angle -> vertical
                            if (xzOriAngle > SWIPE_Z_ORI_THRESHOLD)
                            {
                                if (SwipeRuleState != SwipeBehaviorStateMessageEnum.Correct)
                                {
                                    //notify
                                    SwipeRuleState = SwipeBehaviorStateMessageEnum.Correct;
                                    Console.WriteLine("up!");
                                    OnSwipe(new SwipeBehaviorData(SwipeBehaviorActionEnum.Up, swipe_ivx, swipe_ivy, swipe_disx, swipe_disy, xzOriAngle, xyOriAngle));
                                }
                            }
                            else
                            {
                                if (SwipeRuleState != SwipeBehaviorStateMessageEnum.Swipe_Up_AngleBetweenVerticalOrientation_Over)
                                {
                                    SwipeRuleState = SwipeBehaviorStateMessageEnum.Swipe_Up_AngleBetweenVerticalOrientation_Over;
                                    if (OnNotSwipe != null)
                                    { OnNotSwipe.Invoke(SwipeRuleState.ToString()); }
                                    //{ OnNotSwipe.Invoke("SwipeUp Angle Between Vertical Over:" + xzOriAngle.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            if (SwipeRuleState != SwipeBehaviorStateMessageEnum.Swipe_Up_AngleBetweenHorizonOrientation_Over)
                            {
                                SwipeRuleState = SwipeBehaviorStateMessageEnum.Swipe_Up_AngleBetweenHorizonOrientation_Over;
                                if (OnNotSwipe != null)
                                { OnNotSwipe.Invoke(SwipeRuleState.ToString()); }
                                //{ OnNotSwipe.Invoke("SwipeUp Angle Between Horizon Over :" + xyOriAngle.ToString()); }
                            }
                        }
                    }
                    //swipe down (value is negative)
                    else if (swipe_disy < CHECK_VELOCITY_ORIENTAION)
                    {
                        //9.check angle horizon
                        if (xyOriAngle > SWIPE_UP_DOWN_ANGLE_THRESHOLD && xyOriAngle < 180 - SWIPE_UP_DOWN_ANGLE_THRESHOLD)
                        {
                            //check angle -> vertical
                            if (xzOriAngle > SWIPE_Z_ORI_THRESHOLD)
                            {
                                if (SwipeRuleState != SwipeBehaviorStateMessageEnum.Correct)
                                {
                                    //notify
                                    SwipeRuleState = SwipeBehaviorStateMessageEnum.Correct;
                                    Console.WriteLine("down!");
                                    OnSwipe(new SwipeBehaviorData(SwipeBehaviorActionEnum.Down, swipe_ivx, swipe_ivy, swipe_disx, swipe_disy, xzOriAngle, xyOriAngle));
                                }
                            }
                            else
                            {
                                if (SwipeRuleState != SwipeBehaviorStateMessageEnum.Swipe_Down_AngleBetweenVerticalOrientation_Over)
                                {
                                    SwipeRuleState = SwipeBehaviorStateMessageEnum.Swipe_Down_AngleBetweenVerticalOrientation_Over;
                                    if (OnNotSwipe != null)
                                    { OnNotSwipe.Invoke(SwipeRuleState.ToString()); }
                                    //{ OnNotSwipe.Invoke("SwipeDown Angle Between Vertical Over : " + xzOriAngle.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            if (SwipeRuleState != SwipeBehaviorStateMessageEnum.Swipe_Down_AngleBetweenHorizonOrientation_Over)
                            {
                                SwipeRuleState = SwipeBehaviorStateMessageEnum.Swipe_Down_AngleBetweenHorizonOrientation_Over;
                                if (OnNotSwipe != null)
                                { OnNotSwipe.Invoke(SwipeRuleState.ToString()); }
                                //{ OnNotSwipe.Invoke("SwipeDown Angle Between Horizon Over : " + xyOriAngle.ToString()); }
                            }
                        }
                    }
                }
                else if (Math.Abs(swipe_disx) > SWIPE_RIGHT_LEFT_DISPLACEMENT && Math.Abs(swipe_disy) < SWIPE_UP_DOWN_DISPLACEMENT)
                {

                    float x;
                    float y;
                    float z;
                    //1.calculate virtual point (first.x,first.y + 300,first.z)
                    Point3D yAxiesVirtualPoint = new Point3D(firstPos.Position.X, firstPos.Position.Y + SWIPE_VIRTUAL_POINT_DISTANCE, firstPos.Position.Z);
                    //2.calculate vector(x,y) comprised by first to virtual point 
                    x = yAxiesVirtualPoint.X - firstPos.Position.X;
                    y = yAxiesVirtualPoint.Y - firstPos.Position.Y;
                    Vector2D yOriVirtualVector = new Vector2D(x, y);
                    //3.calculate  vector(x,y) comprised by first point to current point
                    x = lastestPos.Position.X - firstPos.Position.X;
                    y = lastestPos.Position.Y - firstPos.Position.Y;
                    Vector2D xyOriFirstPosToLastPosVector = new Vector2D(x, y);
                    //4.calculate angle comprised by swipe up(x orientation vector) and virtual vector(y orientation vector) angle
                    double xyOriAngle = CalculateVector2DAngle(xyOriFirstPosToLastPosVector, yOriVirtualVector);

                    //5.calculate virtual point (first.x,first.y ,first.z -300)
                    Point3D zAxiesVirtualPoint = new Point3D(firstPos.Position.X, firstPos.Position.Y, firstPos.Position.Z - SWIPE_VIRTUAL_POINT_DISTANCE);
                    //6.calculate vector(x,z) comprised by first to virtual point 
                    x = zAxiesVirtualPoint.X - firstPos.Position.X;
                    z = zAxiesVirtualPoint.Z - firstPos.Position.Z;
                    Vector2D zOriVirtualVector = new Vector2D(x, z);
                    //7.calculate vector(x,z) comprised by first point to current point
                    x = lastestPos.Position.X - firstPos.Position.X;
                    z = lastestPos.Position.Z - firstPos.Position.Z;
                    Vector2D xzOriFirstPosToLastPosVector = new Vector2D(x, z);
                    //8.calculate swipe up(x orientation vector) and virtual vector(z orientation vector) angle
                    double xzOriAngle = CalculateVector2DAngle(xzOriFirstPosToLastPosVector, zOriVirtualVector);

                    //test
                    if (OnSwipeAngleChanged != null)
                    {
                        OnSwipeAngleChanged(xzOriAngle, "AngleBetweenVerticalOrientation");
                        OnSwipeAngleChanged(xyOriAngle, "SwipeRightLeftAngleBetweenHorizonOrientation");

                    }

                    //swipe right(value is positive)
                    if (swipe_disx > CHECK_VELOCITY_ORIENTAION)
                    {
                        //9.check angle -> horizon
                        if (xyOriAngle > SWIPE_UP_DOWN_ANGLE_THRESHOLD && xyOriAngle < 180 - SWIPE_UP_DOWN_ANGLE_THRESHOLD)
                        {
                            //check angle -> vertical
                            if (xzOriAngle > SWIPE_Z_ORI_THRESHOLD)
                            {
                                if (SwipeRuleState != SwipeBehaviorStateMessageEnum.Correct)
                                {
                                    //notify
                                    SwipeRuleState = SwipeBehaviorStateMessageEnum.Correct;
                                    Console.WriteLine("right!");
                                    OnSwipe(new SwipeBehaviorData(SwipeBehaviorActionEnum.Right, swipe_ivx, swipe_ivy, swipe_disx, swipe_disy, xzOriAngle, xyOriAngle));
                                }
                            }
                            else
                            {
                                if (SwipeRuleState != SwipeBehaviorStateMessageEnum.Swipe_Right_AngleBetweenVerticalOrientation_Over)
                                {
                                    SwipeRuleState = SwipeBehaviorStateMessageEnum.Swipe_Right_AngleBetweenVerticalOrientation_Over;
                                    if (OnNotSwipe != null)
                                    { OnNotSwipe.Invoke(SwipeRuleState.ToString()); }
                                    // { OnNotSwipe.Invoke("SwipeRight Angle Between Vertical Over : " + xzOriAngle.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            if (SwipeRuleState != SwipeBehaviorStateMessageEnum.Swipe_Right_AngleBetweenHorizonOrientation_Over)
                            {
                                SwipeRuleState = SwipeBehaviorStateMessageEnum.Swipe_Right_AngleBetweenHorizonOrientation_Over;
                                if (OnNotSwipe != null)
                                { OnNotSwipe.Invoke(SwipeRuleState.ToString()); }
                                //{ OnNotSwipe.Invoke("SwipeRight Angle Between Horizon Over : " + xyOriAngle.ToString()); }
                            }

                        }
                    }
                    //swipe left(value is negative)
                    else if (swipe_disx < CHECK_VELOCITY_ORIENTAION)
                    {
                        //9.check angle -> horizon
                        if (xyOriAngle > SWIPE_RIGHT_LEFT_ANGLE_THRESHOLD && xyOriAngle < 180 - SWIPE_RIGHT_LEFT_ANGLE_THRESHOLD)
                        {
                            //check angle -> vertical
                            if (xzOriAngle > SWIPE_Z_ORI_THRESHOLD)
                            {
                                if (SwipeRuleState != SwipeBehaviorStateMessageEnum.Correct)
                                {
                                    //notify
                                    SwipeRuleState = SwipeBehaviorStateMessageEnum.Correct;
                                    Console.WriteLine("left!");
                                    OnSwipe(new SwipeBehaviorData(SwipeBehaviorActionEnum.Left, swipe_ivx, swipe_ivy, swipe_disx, swipe_disy, xzOriAngle, xyOriAngle));
                                }
                            }
                            else
                            {
                                if (SwipeRuleState != SwipeBehaviorStateMessageEnum.Swipe_Left_AngleBetweenVerticalOrientation_Over)
                                {
                                    SwipeRuleState = SwipeBehaviorStateMessageEnum.Swipe_Left_AngleBetweenVerticalOrientation_Over;
                                    if (OnNotSwipe != null)
                                    { OnNotSwipe.Invoke(SwipeRuleState.ToString()); }
                                    //{ OnNotSwipe.Invoke("SwipeLeft Angle Between Vertical Over :" + xzOriAngle.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            if (SwipeRuleState != SwipeBehaviorStateMessageEnum.Swipe_Left_AngleBetweenHorizonOrientation_Over)
                            {
                                SwipeRuleState = SwipeBehaviorStateMessageEnum.Swipe_Left_AngleBetweenHorizonOrientation_Over;
                                if (OnNotSwipe != null)
                                { OnNotSwipe.Invoke(SwipeRuleState.ToString()); }
                                //{ OnNotSwipe.Invoke("SwipeLeft Angle Between Horizon Over :" + xyOriAngle.ToString()); }
                            }
                        }
                    }
                }
                else
                {
                    if (SwipeRuleState != SwipeBehaviorStateMessageEnum.Swipe_Velocity_Not_Enough)
                    {
                        SwipeRuleState = SwipeBehaviorStateMessageEnum.Swipe_Velocity_Not_Enough;
                        if (OnNotSwipe != null)
                        { OnNotSwipe.Invoke(SwipeRuleState.ToString()); }
                    }
                }
            }
            //length is not over
            else if (Math.Abs(swipe_ivx) < SWIPE_RIGHT_LEFT_DISPLACEMENT && Math.Abs(swipe_ivy) < SWIPE_UP_DOWN_DISPLACEMENT)
            {
                if (SwipeRuleState != SwipeBehaviorStateMessageEnum.Swipe_Length_Not_Enough)
                {
                    SwipeRuleState = SwipeBehaviorStateMessageEnum.Swipe_Length_Not_Enough;
                    if (OnNotSwipe != null)
                    { OnNotSwipe.Invoke(SwipeRuleState.ToString()); }
                }
            }

        }
        //////////////////////////////////////////////////////////////////////////////
        #endregion

        #region push part
        //////////////////////////////////////////////////////////////////////////////
        //push part
        /////////////////////////////////////////////////
        //calculaet angle in 3 dimension vector 
        private double CalculateVector3DAngle(Vector3D vectorA, Vector3D vectorB)
        {
            double dotProduct = (double)(vectorA.X * vectorB.X) + (double)(vectorA.Y * vectorB.Y) + (double)(vectorA.Z * vectorB.Z);
            double vectorALength = Math.Sqrt(Math.Pow(vectorA.X, 2) + Math.Pow(vectorA.Y, 2) + Math.Pow(vectorA.Z, 2));
            double vectorBLength = Math.Sqrt(Math.Pow(vectorB.X, 2) + Math.Pow(vectorB.Y, 2) + Math.Pow(vectorB.Z, 2));
            double cosTheta = dotProduct / (vectorALength * vectorBLength);
            double angle = (Math.Acos(cosTheta)) * 180 / Math.PI;
            return angle;
        }
        /////////////////////////////////////////////////
        //push algriothm
        private void CalculatePushVelocityTrigger(SkeletonJoint j, Dictionary<SkeletonJoint, LinkedList<SkeletonJointPosition>> listPos)
        {
            SkeletonJointPosition lastestPos = listPos[j].Last.Value;
            SkeletonJointPosition firstPos = listPos[j].First.Value;
            SkeletonJointPosition lastestPrevPos = listPos[j].Last.Previous.Value;
            SkeletonJointPosition torsoPoint = listPos[SkeletonJoint.Torso].Last.Previous.Value;

            double push_averVz = (lastestPos.Position.Z - firstPos.Position.Z) / pushAndSwipeTimeSpan;
            double push_ivz = (lastestPos.Position.Z) - (lastestPrevPos.Position.Z);
            double push_ivx = Math.Abs((lastestPos.Position.X) - (lastestPrevPos.Position.X));
            double push_ivy = Math.Abs((lastestPos.Position.Y) - (lastestPrevPos.Position.Y));

            double push_dx = Math.Abs((lastestPos.Position.X) - (firstPos.Position.X));
            double push_dy = Math.Abs((lastestPos.Position.Y) - (firstPos.Position.Y));

            //push length ok
            if (lastestPos.Position.Z < (torsoPoint.Position.Z - pushDistanceWithTorsoThreshold))
            {
                //push speed of Z-axie ok
                if (push_ivz < pushSpeedThreshold)
                {
                    //calculate push straightly vector ,and not push straightly vector and get angle
                    float x;
                    float y;
                    float z;
                    //assume firstPos pushed ,then straight position is...
                    Point3D assumePushedPoint = new Point3D(firstPos.Position.X, firstPos.Position.Y, firstPos.Position.Z - PUSH_VIRTUAL_POINT_DISTANCE);
                    //calculate firstPos to lastPos vector
                    x = lastestPos.Position.X - firstPos.Position.X;
                    y = lastestPos.Position.Y - firstPos.Position.Y;
                    z = lastestPos.Position.Z - firstPos.Position.Z;
                    //calculate vector
                    Vector3D firstPosToLastPosVector = new Vector3D(x, y, z);
                    //calculate the phisycal vector
                    x = assumePushedPoint.X - firstPos.Position.X;
                    y = assumePushedPoint.Y - firstPos.Position.Y;
                    z = assumePushedPoint.Z - firstPos.Position.Z;
                    Vector3D assumeStraightPushVector = new Vector3D(x, y, z);

                    if (j == SkeletonJoint.LeftHand)
                    {
                        double angle = CalculateVector3DAngle(firstPosToLastPosVector, assumeStraightPushVector);
                        //add angle check 
                        if (angle < pushAngleThreshold)
                        {
                            OnHandPush(new PushBehaviorData(j.ToString(), push_ivz, angle, lastestPos.Position));
                            //Console.WriteLine("left hand push!\npush_ivz= {0}\npush_averVz={1}", push_ivz, push_averVz);
                            //Console.WriteLine("push_ivx= {0}\npush_ivy={1}", push_ivx, push_ivy);
                            //Console.WriteLine("push_dx= {0}\npush_dy={1}", push_dx, push_dy);
                            preIsLeftPush = true;
                        }
                    }
                    else if (j == SkeletonJoint.RightHand)
                    {
                        double angle = CalculateVector3DAngle(firstPosToLastPosVector, assumeStraightPushVector);
                        //add angle check 
                        if (angle < pushAngleThreshold)
                        {
                            OnHandPush(new PushBehaviorData(j.ToString(), push_ivz, angle, lastestPos.Position));
                            //Console.WriteLine("right hand push!\npush_ivz= {0}\npush_averVz={1}", push_ivz, push_averVz);
                            //Console.WriteLine("push_ivx= {0}\npush_ivy={1}", push_ivx, push_ivy);
                            //Console.WriteLine("push_dx= {0}\npush_dy={1}", push_dx, push_dy);
                            preIsRightPush = true;
                        }
                    }
                }
                else
                {
                    if (j == SkeletonJoint.LeftHand)
                    {
                        if (preIsLeftPush)
                        {
                            //invoke stable
                            preIsLeftPush = false;
                            if (OnHandPushStable != null)
                            { OnHandPushStable(j.ToString()); }
                        }
                    }
                    else if (j == SkeletonJoint.RightHand)
                    {
                        if (preIsRightPush)
                        {
                            preIsRightPush = false;
                            if (OnHandPushStable != null)
                            { OnHandPushStable(j.ToString()); }
                        }
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////
        #endregion

        ////////////////////////////////////////////////////////////////////////
        #endregion

        #region Speed(slash) part 演算法 -->未被使用
        /////////////////////////////////////////////////
        //演算法
        private void CalculateSlashVelocityTrigger(Dictionary<SkeletonJoint, LinkedList<SkeletonJointPosition>> listPos)
        {

            SkeletonJointPosition lastestPos = listPos[SkeletonJoint.RightHand].Last.Value;
            SkeletonJointPosition firstPos = listPos[SkeletonJoint.RightHand].First.Value;
            SkeletonJointPosition lastestPrevPos = listPos[SkeletonJoint.RightHand].Last.Previous.Value;
            SkeletonJointPosition rightShoulderPos = listPos[SkeletonJoint.RightShoulder].Last.Previous.Value;
           
            //////////////////////////////////////////////////////////////
            //Slash part
            //瞬間速度
            double ivx = lastestPos.Position.X - lastestPrevPos.Position.X;
            double ivy = lastestPos.Position.Y - lastestPrevPos.Position.Y;
            double ivz = lastestPos.Position.Z - lastestPrevPos.Position.Z;
            double averVx = lastestPos.Position.X - firstPos.Position.X;
            double averVy = lastestPos.Position.Y - firstPos.Position.Y;
            double averVz = lastestPos.Position.Z - firstPos.Position.Z;
            //位移
            double dx = Math.Abs(lastestPos.Position.X - firstPos.Position.X);
            double dy =lastestPos.Position.Y - firstPos.Position.Y;
            double dz = Math.Abs(lastestPos.Position.Z - firstPos.Position.Z);
       
            ////////////////////////////////////////////////////////////////////
            //是否為橫砍
            //判斷X軸速度是否超過臨界值且y軸速度未超過臨界值
            if (Math.Abs(ivx) > slashRightLeftVelocity && ivy > slashDownVelocity)
            {
                //Console.WriteLine("vx={0} vy = {1} vz={2}", ivx, ivy, ivz);
                //Console.WriteLine("aver vx={0} aver vy = {1} aver vz={2}", averVx, averVy, averVz);
                //Console.WriteLine("橫砍速度判段進入");
                ////////////////////////////////////////////////////////////////////
                //計算角度
                float x;
                float y;
                //1.計算虛擬座標為先前手點往Y軸向下延伸的點t (first.x,first.y - 600,first.z)
                Point3D yAxiesVirtualPoint = new Point3D(firstPos.Position.X, firstPos.Position.Y - 600, firstPos.Position.Z);
                //2.計算虛擬座標到先前手點的向量
                x = yAxiesVirtualPoint.X - firstPos.Position.X;
                y = yAxiesVirtualPoint.Y - firstPos.Position.Y;
                Vector2D yOriVirtualVector = new Vector2D(x, y);
                //3.計算目前的手點位置到先前手點的向量
                x = lastestPos.Position.X - firstPos.Position.X;
                y = lastestPos.Position.Y - firstPos.Position.Y;
                Vector2D xyOriFirstPosToLastPosVector = new Vector2D(x, y);
                //4.計算夾角(為目前移動方向向量和虛擬向下Y軸向量所形成的夾角)
                double xyOriAngle = CalculateVector2DAngle(xyOriFirstPosToLastPosVector, yOriVirtualVector);
                ////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////
                //test
                if (OnSwipeAngleChanged != null)
                {
                    OnSlashAngleChanged(xyOriAngle, "slashAngleChanged");
                }
                //判斷角度是否為水平(要大於80度且小於105)
                if (xyOriAngle > slashMinHorizonAngle&& xyOriAngle < slashMaxHorizonAngle)
                {
                    //是向右
                    if (ivx > CHECK_VELOCITY_ORIENTAION)
                    {
                        preIsRightSlash = true;
                        this.OnHandSlash.Invoke(new SlashBehaviorData(SlashBehaviorEnum.RightHorizonSlash, ivx, ivy, ivz, listPos[SkeletonJoint.RightHand]));
                        //Console.WriteLine("右橫砍");
                    }
                    //是向左
                    else
                    {
                        preIsRightSlash = true;
                        this.OnHandSlash.Invoke(new SlashBehaviorData(SlashBehaviorEnum.LeftHorizonSlash, ivx, ivy, ivz, listPos[SkeletonJoint.RightHand]));
                        //Console.WriteLine("左橫砍");
                    }
                }
            }
            //縱砍
            //判斷y軸速度是否超過臨界值且x軸速度未超過臨界值
            else if (Math.Abs(ivx) < slashRightLeftVelocity && ivy < slashDownVelocity)
            {
                //Console.WriteLine("vx={0} vy = {1} vz={2}", ivx, ivy, ivz);
                //Console.WriteLine("aver vx={0} aver vy = {1} aver vz={2}", averVx, averVy, averVz);
                //Console.WriteLine("縱砍速度判段進入");
                //Console.WriteLine(" RightHand Y =>" + firstPos.Position.Y.ToString());
                //Console.WriteLine(" RightShoulder Y =>" + rightShoulderPos.Position.Y.ToString());
                          
                ////////////////////////////////////////////////////////////////////
                //計算角度
                float x;
                float y;
                //1.calculate virtual point (first.x,first.y - 600,first.z)
                Point3D yAxiesVirtualPoint = new Point3D(firstPos.Position.X, firstPos.Position.Y - 600, firstPos.Position.Z);
                //2.calculate vector(x,y) comprised by first to virtual point 
                x = yAxiesVirtualPoint.X - firstPos.Position.X;
                y = yAxiesVirtualPoint.Y - firstPos.Position.Y;
                Vector2D yOriVirtualVector = new Vector2D(x, y);
                //3.calculate  vector(x,y) comprised by first point to current point
                x = lastestPos.Position.X - firstPos.Position.X;
                y = lastestPos.Position.Y - firstPos.Position.Y;
                Vector2D xyOriFirstPosToLastPosVector = new Vector2D(x, y);
                //4.calculate angle comprised by swipe up(x orientation vector) and virtual vector(y orientation vector) angle
                double xyOriAngle = CalculateVector2DAngle(xyOriFirstPosToLastPosVector, yOriVirtualVector);
                ////////////////////////////////////////////////////////////////////
                //判斷角度是否為水平(要否小於30度)
                if (xyOriAngle < slashVerticalAngle)
                {
                    preIsRightSlash = true;
                    this.OnHandSlash.Invoke(new SlashBehaviorData(SlashBehaviorEnum.VerticalSlash, ivx, ivy, ivz, listPos[SkeletonJoint.RightHand]));
                    //Console.WriteLine("縱砍");
                }
                
            }
            //斜砍
            //判斷y軸速度和x軸速度是否超過臨界值
            else if (Math.Abs(ivx) > slashRakeRightLeftVelocity && ivy < slashRakeDownVelocity)
            {
                //Console.WriteLine("vx={0} vy = {1} vz={2}", ivx, ivy, ivz);
                //Console.WriteLine("aver vx={0} aver vy = {1} aver vz={2}", averVx, averVy, averVz);
                //Console.WriteLine("斜砍速度判段進入");
                ////////////////////////////////////////////////////////////////////
                //計算角度
                float x;
                float y;
                //1.calculate virtual point (first.x,first.y - 600,first.z)
                Point3D yAxiesVirtualPoint = new Point3D(firstPos.Position.X, firstPos.Position.Y - 600, firstPos.Position.Z);
                //2.calculate vector(x,y) comprised by first to virtual point 
                x = yAxiesVirtualPoint.X - firstPos.Position.X;
                y = yAxiesVirtualPoint.Y - firstPos.Position.Y;
                Vector2D yOriVirtualVector = new Vector2D(x, y);
                //3.calculate  vector(x,y) comprised by first point to current point
                x = lastestPos.Position.X - firstPos.Position.X;
                y = lastestPos.Position.Y - firstPos.Position.Y;
                Vector2D xyOriFirstPosToLastPosVector = new Vector2D(x, y);
                //4.calculate angle comprised by swipe up(x orientation vector) and virtual vector(y orientation vector) angle
                double xyOriAngle = CalculateVector2DAngle(xyOriFirstPosToLastPosVector, yOriVirtualVector);
                ////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////
                //test
                if (OnSwipeAngleChanged != null)
                {
                    OnSlashAngleChanged(xyOriAngle, "slashAngleChanged");
                }
               
                //介於兩個夾角之間
                if (xyOriAngle > slashRakeMinAngleThreshold && xyOriAngle < slashRakeMaxAngleThreshold)
                {
                    //向左斜砍
                    if (ivx < CHECK_VELOCITY_ORIENTAION)
                    {
                        preIsRightSlash = true;
                        this.OnHandSlash.Invoke(new SlashBehaviorData(SlashBehaviorEnum.LeftRakedSlash, ivx, ivy, ivz, listPos[SkeletonJoint.RightHand]));
                        //Console.WriteLine("向左斜砍");
                    }
                    else
                    {
                        preIsRightSlash = true;
                        this.OnHandSlash.Invoke(new SlashBehaviorData(SlashBehaviorEnum.RightRakedSlash, ivx, ivy, ivz, listPos[SkeletonJoint.RightHand]));
                        //Console.WriteLine("向右斜砍");
                    }
                        
                }
                
            }
            else
            {
                if (preIsRightSlash)
                {
                    preIsRightSlash = false;
                    if (OnHandSlashStable != null)
                    { this.OnHandSlashStable.Invoke(); }
                }
            }
           

        }
        //////////////////////////////////////////////////////////////////////
        #endregion

        #region fly part 演算法 -->未被使用
        ////////////////////////////////////////////////////////////////////////
        //此部分不須收集用到的關節frame
        private void UpdateFlyData(SkeletonJoint j, SkeletonJointPosition pos)
        {
            if (!jointNodeBufferForFly.ContainsKey(j))
            {
                jointNodeBufferForFly.Add(j, pos);
            }
            else
            {
                //收集關節資料
                if (jointNodeBufferForFly.Count == 8)
                {
                    if (OnFly != null)
                    { CalculateFlyTrriger(jointNodeBufferForFly); }
                    //更新資料
                    jointNodeBufferForFly[j] = pos;
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////
        //演算法
        private void CalculateFlyTrriger(Dictionary<SkeletonJoint, SkeletonJointPosition> jpos)
        {
            //設定飛行演算法執行的範圍
            //以頭部為範圍核心(Y軸要拉高100,Z軸往前移200)
            Point3D distributionCenter = new Point3D(jpos[SkeletonJoint.Head].Position.X, jpos[SkeletonJoint.Head].Position.Y + 100, jpos[SkeletonJoint.Head].Position.Z - 200);
            //(長方形)範圍邊界計算=>RANGE_BOX_THRESHOLD為距離
            double yLowerThreshold = distributionCenter.Y - RANGE_BOX_THRESHOLD;
            double yHeigherThreshold = distributionCenter.Y + RANGE_BOX_UP_THRESHOLD;
            double xRightThreshold = distributionCenter.X + RANGE_BOX_THRESHOLD;
            double xLeftThreshold = distributionCenter.X - RANGE_BOX_THRESHOLD;
            double zThreshold = distributionCenter.Z + (RANGE_BOX_THRESHOLD * 2);
            //記錄現在左右手的位置
            Point3D leftHand = jpos[SkeletonJoint.LeftHand].Position;
            Point3D rightHand = jpos[SkeletonJoint.RightHand].Position;

            //計算
            //計算左右手和飛行犯為核心Y軸的位移量(是否左或右手有離開飛行的Y軸邊界以判斷為哪一種飛行模式)
            //計算飛行向下時和核心的位移量
            double flyLeftYOriDisplacement = (jpos[SkeletonJoint.LeftHand].Position.Y - distributionCenter.Y);
            double flyRightYOriDisplacement = (jpos[SkeletonJoint.RightHand].Position.Y - distributionCenter.Y);
            double flyDownYOriDisplacement = ((double)(jpos[SkeletonJoint.RightHand].Position.Y + jpos[SkeletonJoint.LeftHand].Position.Y) / 2 - distributionCenter.Y);

            //計算手和手肘連成向量方向(應為正值表手向上舉)
            double yOriVectorOfLeftHandAndLeftElbow = (jpos[SkeletonJoint.LeftHand].Position.Y - jpos[SkeletonJoint.LeftElbow].Position.Y);
            double yOriVectorOfRightHandAndRightElbow = (jpos[SkeletonJoint.RightHand].Position.Y - jpos[SkeletonJoint.RightElbow].Position.Y);

            //左右手之間Z軸的差距計算(因為兩手的差距不能太遠)
            //只需純量(不須知道使哪一隻手比哪一隻手前面)
            double zOriDifferenceOfHands = Math.Abs(leftHand.Z - rightHand.Z);

            //此變數用來判斷使否向下飛的條件
            //為正表左手比右手舉的高,相反為右手舉得比左手高
            double yOriDifferenceOfHands = leftHand.Y - rightHand.Y;

            //4.計算手肘到手的向量及手肘到肩膀的向量(為了計算手肘夾角)
            float x;
            float y;
            float z;
            //手和手肘部分
            //左手
            x = jpos[SkeletonJoint.LeftHand].Position.X - jpos[SkeletonJoint.LeftElbow].Position.X;
            y = jpos[SkeletonJoint.LeftHand].Position.Y - jpos[SkeletonJoint.LeftElbow].Position.Y;
            z = jpos[SkeletonJoint.LeftHand].Position.Z - jpos[SkeletonJoint.LeftElbow].Position.Z;
            Vector3D leftElbowToLeftHandVector = new Vector3D(x, y, z);
            //右手
            x = jpos[SkeletonJoint.RightHand].Position.X - jpos[SkeletonJoint.RightElbow].Position.X;
            y = jpos[SkeletonJoint.RightHand].Position.Y - jpos[SkeletonJoint.RightElbow].Position.Y;
            z = jpos[SkeletonJoint.RightHand].Position.Z - jpos[SkeletonJoint.RightElbow].Position.Z;
            Vector3D rightElbowToRightHandVector = new Vector3D(x, y, x);
            //手肘和肩膀部分
            //左手
            x = jpos[SkeletonJoint.LeftShoulder].Position.X - jpos[SkeletonJoint.LeftElbow].Position.X;
            y = jpos[SkeletonJoint.LeftShoulder].Position.Y - jpos[SkeletonJoint.LeftElbow].Position.Y;
            z = jpos[SkeletonJoint.LeftShoulder].Position.Z - jpos[SkeletonJoint.LeftElbow].Position.Z;
            Vector3D leftElbowToLeftShoulderVector = new Vector3D(x, y, z);
            //右手
            x = jpos[SkeletonJoint.RightShoulder].Position.X - jpos[SkeletonJoint.RightElbow].Position.X;
            y = jpos[SkeletonJoint.RightShoulder].Position.Y - jpos[SkeletonJoint.RightElbow].Position.Y;
            z = jpos[SkeletonJoint.RightShoulder].Position.Z - jpos[SkeletonJoint.RightElbow].Position.Z;
            Vector3D rightElbowToRightShoulderVector = new Vector3D(x, y, x);
            //計算左肘和右肘的夾角
            double leftPartAngle = CalculateVector3DAngle(leftElbowToLeftHandVector, leftElbowToLeftShoulderVector);
            double rightPartAngle = CalculateVector3DAngle(rightElbowToRightHandVector, rightElbowToRightShoulderVector);

            if (OnBoxChanged != null)
            { OnBoxChanged.Invoke(new FlyRangeBoxData(distributionCenter, yLowerThreshold, yHeigherThreshold, xRightThreshold, xLeftThreshold)); }
            //test...
            if (OnFlyAngleChanged != null)
            {
                OnFlyAngleChanged(leftPartAngle, "LeftPart");
                OnFlyAngleChanged(rightPartAngle, "RightPart");
            }
            /////////////////////////////////////////////////////////
            //是否為飛行動做的判斷
            //角度是否皆小於110度
            if (rightPartAngle < HANDS_ANGLE && leftPartAngle < HANDS_ANGLE)
            {
                //兩手是否皆向上舉
                if (yOriVectorOfLeftHandAndLeftElbow > 0 && yOriVectorOfRightHandAndRightElbow > 0)
                {
                    //兩手之間的距離差要小於100
                    if (zOriDifferenceOfHands < HANDS_Z_ORI_DIFFERENCE)
                    {
                        //範圍判斷
                        //左右手的x位置是否分別位於手左和右手區域
                        if (leftHand.X < xLeftThreshold && rightHand.X > xRightThreshold)
                        {
                            //手的距離是否有在遠離核心的Z軸
                            if (rightHand.Z < zThreshold && leftHand.Z < zThreshold)
                            {
                                //兩手是否皆在飛行犯為的裡面=>直飛
                                if (leftHand.Y < yHeigherThreshold && leftHand.Y > yLowerThreshold && rightHand.Y < yHeigherThreshold && rightHand.Y > yLowerThreshold)
                                {
                                    flyRuleState = FlyBehaviorStateMessageEnum.Correct;
                                    OnFly(new FlyBehaviorData(leftPartAngle, rightPartAngle, yOriVectorOfLeftHandAndLeftElbow, yOriVectorOfRightHandAndRightElbow, zOriDifferenceOfHands, 0, FlyBehaviorActionEnum.FlyStraight));
                                }
                                //右手是否低於範圍且左手仍在範圍=>向右飛
                                else if (rightHand.Y < yLowerThreshold && leftHand.Y > yLowerThreshold && leftHand.Y < yHeigherThreshold)
                                {
                                    //兩手的y軸差距不要過大(因為兩手在三為空間中除x以外的位置因為差不多如同鏡子)
                                    if (yOriDifferenceOfHands > HANDS_Y_ORI_DIFFERENCE)
                                    {
                                        flyRuleState = FlyBehaviorStateMessageEnum.Correct;
                                        OnFly(new FlyBehaviorData(leftPartAngle, rightPartAngle, yOriVectorOfLeftHandAndLeftElbow, yOriVectorOfRightHandAndRightElbow, zOriDifferenceOfHands, flyRightYOriDisplacement, FlyBehaviorActionEnum.FlyRight));
                                    }
                                }
                                //左手是否低於範圍且右手仍在範圍=>向左飛
                                else if (leftHand.Y < yLowerThreshold && rightHand.Y > yLowerThreshold && rightHand.Y < yHeigherThreshold)
                                {
                                    //兩手的y軸差距不要過大(因為兩手在三為空間中除x以外的位置因為差不多如同鏡子)
                                    if (yOriDifferenceOfHands < -HANDS_Y_ORI_DIFFERENCE)
                                    {
                                        flyRuleState = FlyBehaviorStateMessageEnum.Correct;
                                        OnFly(new FlyBehaviorData(leftPartAngle, rightPartAngle, yOriVectorOfLeftHandAndLeftElbow, yOriVectorOfRightHandAndRightElbow, zOriDifferenceOfHands, flyLeftYOriDisplacement, FlyBehaviorActionEnum.FlyLeft));
                                    }
                                }
                                //l兩手是否皆低於範圍=>向下飛
                                else if (rightHand.Y < yLowerThreshold && leftHand.Y < yLowerThreshold)
                                {
                                    flyRuleState = FlyBehaviorStateMessageEnum.Correct;
                                    OnFly(new FlyBehaviorData(leftPartAngle, rightPartAngle, yOriVectorOfLeftHandAndLeftElbow, yOriVectorOfRightHandAndRightElbow, zOriDifferenceOfHands, flyDownYOriDisplacement, FlyBehaviorActionEnum.FlyDown));
                                }
                                //左手仍在範圍內或離開範圍且右手高於Y軸邊界範圍=>右手舉過高
                                else if ((leftHand.Y < yLowerThreshold || (leftHand.Y > yLowerThreshold && leftHand.Y < yHeigherThreshold)) && rightHand.Y > yHeigherThreshold)
                                {
                                    if (flyRuleState != FlyBehaviorStateMessageEnum.RightHand_UpLift_Over_High)
                                    {
                                        flyRuleState = FlyBehaviorStateMessageEnum.RightHand_UpLift_Over_High;
                                        if (OnNotFly != null)
                                        { OnNotFly.Invoke(flyRuleState.ToString()); }
                                        //{ OnNotFly.Invoke("RightHand is heigher than range : " + leftHand.Y.ToString()); }
                                    }
                                }
                                //右仍在範圍內或離開範圍且左手高於Y軸邊界範圍=>左手舉過高
                                else if ((rightHand.Y < yLowerThreshold || (rightHand.Y > yLowerThreshold && rightHand.Y < yHeigherThreshold)) && leftHand.Y > yHeigherThreshold)
                                {
                                    if (flyRuleState != FlyBehaviorStateMessageEnum.LefttHand_UpLift_Over_High)
                                    {
                                        flyRuleState = FlyBehaviorStateMessageEnum.LefttHand_UpLift_Over_High;
                                        if (OnNotFly != null)
                                        { OnNotFly.Invoke(flyRuleState.ToString()); }
                                        //{ OnNotFly.Invoke("LeftHand is heigher than range : " + rightHand.Y.ToString()); }
                                    }
                                }
                                //兩手皆舉過高離開Y軸邊界
                                else if (rightHand.Y > yHeigherThreshold && leftHand.Y > yHeigherThreshold)
                                {
                                    if (flyRuleState != FlyBehaviorStateMessageEnum.BothtHands_UpLift_Over_High)
                                    {
                                        flyRuleState = FlyBehaviorStateMessageEnum.BothtHands_UpLift_Over_High;
                                        if (OnNotFly != null)
                                        { OnNotFly.Invoke(flyRuleState.ToString()); }
                                        //{ OnNotFly.Invoke("BothtHands is heigher than range"); }
                                    }
                                }
                            }
                            else //手的Z軸座標接近肩膀
                            {
                                //右手
                                if (rightHand.Z > zThreshold)
                                {
                                    if (flyRuleState != FlyBehaviorStateMessageEnum.RightHand_Z_Point_Over_Shoulder)
                                    {
                                        flyRuleState = FlyBehaviorStateMessageEnum.RightHand_Z_Point_Over_Shoulder;
                                        if (OnNotFly != null)
                                        { OnNotFly.Invoke(flyRuleState.ToString()); }
                                        //{ OnNotFly.Invoke("RightHand Z Position Over : " + rightHand.Z.ToString()); }
                                    }
                                }
                                //左手
                                else if (leftHand.Z > distributionCenter.Z)
                                {
                                    if (flyRuleState != FlyBehaviorStateMessageEnum.LeftHand_Z_Point_Over_Shoulder)
                                    {
                                        flyRuleState = FlyBehaviorStateMessageEnum.LeftHand_Z_Point_Over_Shoulder;
                                        if (OnNotFly != null)
                                        { OnNotFly.Invoke(flyRuleState.ToString()); }
                                        //{ OnNotFly.Invoke("LeftHand Z Position Over : " + leftHand.Z.ToString()); }
                                    }
                                }
                            }
                        }
                        //手的x軸座標不在自己的範圍
                        else
                        {
                            //左手
                            if (leftHand.X < xRightThreshold)
                            {
                                if (flyRuleState != FlyBehaviorStateMessageEnum.LeftHand_Position_Not_In_LeftZone)
                                {
                                    flyRuleState = FlyBehaviorStateMessageEnum.LeftHand_Position_Not_In_LeftZone;
                                    if (OnNotFly != null)
                                    { OnNotFly.Invoke(flyRuleState.ToString()); }
                                    //{ OnNotFly.Invoke("position of  lefthand is incorrect"); }
                                }

                            }
                            //右手
                            else if (rightHand.X > xLeftThreshold)
                            {
                                if (flyRuleState != FlyBehaviorStateMessageEnum.RightHand_Position_Not_In_RightZone)
                                {
                                    flyRuleState = FlyBehaviorStateMessageEnum.RightHand_Position_Not_In_RightZone;
                                    if (OnNotFly != null)
                                    { OnNotFly.Invoke(flyRuleState.ToString()); }
                                    //{ OnNotFly.Invoke("Position of  righthand is incorrect"); }
                                }
                            }
                        }
                    }
                    //兩手的Z軸座標之間距離過大
                    else if (zOriDifferenceOfHands > HANDS_Z_ORI_DIFFERENCE)
                    {
                        if (flyRuleState != FlyBehaviorStateMessageEnum.RightHand_And_LeftHand_Z_Axies_Difference_Over)
                        {
                            flyRuleState = FlyBehaviorStateMessageEnum.RightHand_And_LeftHand_Z_Axies_Difference_Over;
                            if (OnNotFly != null)
                            { OnNotFly.Invoke(flyRuleState.ToString()); }
                            //{ OnNotFly.Invoke("Hands Z difference over"); }
                        }
                    }
                }
                //手沒有向上舉
                else
                {
                    //左手
                    if (yOriVectorOfLeftHandAndLeftElbow < 0)
                    {
                        if (flyRuleState != FlyBehaviorStateMessageEnum.LeftHand_Not_UpLift)
                        {
                            flyRuleState = FlyBehaviorStateMessageEnum.LeftHand_Not_UpLift;
                            if (OnNotFly != null)
                            { OnNotFly.Invoke(flyRuleState.ToString()); }
                            //{ OnNotFly.Invoke("LeftHand vector not turn up"); }
                        }
                    }
                    //右手
                    else if (yOriVectorOfRightHandAndRightElbow < 0)
                    {
                        if (flyRuleState != FlyBehaviorStateMessageEnum.RightHand_Not_UpLift)
                        {
                            flyRuleState = FlyBehaviorStateMessageEnum.RightHand_Not_UpLift;
                            if (OnNotFly != null)
                            { OnNotFly.Invoke(flyRuleState.ToString()); }
                            //{ OnNotFly.Invoke("RightHand vector not turn up"); }
                        }
                    }
                }
            }
            //手肘的角度不對
            else
            {
                //右手
                if (rightPartAngle > HANDS_ANGLE)
                {
                    if (flyRuleState != FlyBehaviorStateMessageEnum.RightElbow_And_RightShoulder_Not_Vetical)
                    {
                        flyRuleState = FlyBehaviorStateMessageEnum.RightElbow_And_RightShoulder_Not_Vetical;
                        if (OnNotFly != null)
                        { OnNotFly.Invoke(flyRuleState.ToString()); }
                        //{ OnNotFly.Invoke("Right Angle Over : " + rightPartAngle.ToString()); }
                    }
                }
                //左手
                else if (leftPartAngle > HANDS_ANGLE)
                {
                    if (flyRuleState != FlyBehaviorStateMessageEnum.LeftElbow_And_LeftShoulder_Not_Vetical)
                    {
                        flyRuleState = FlyBehaviorStateMessageEnum.LeftElbow_And_LeftShoulder_Not_Vetical;
                        if (OnNotFly != null)
                        { OnNotFly.Invoke(flyRuleState.ToString()); }
                        //{ OnNotFly.Invoke("Left Angle Over : " + leftPartAngle.ToString()); }
                    }
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////
        #endregion

        ////////////////////////////////////////////////////////////////////////
        #endregion

        public Dictionary<int, Dictionary<SkeletonJoint, SkeletonJointPosition>> Joints
        {
            get { return kinectProcess.Joints; }
        }

        public SkeletonCapability SkeletonCapbility
        {
            get { return kinectProcess.SkeletonCapbility; }
        }
    }
}
