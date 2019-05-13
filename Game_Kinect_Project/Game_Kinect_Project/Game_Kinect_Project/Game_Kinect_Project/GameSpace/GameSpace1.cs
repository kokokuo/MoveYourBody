using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Utils;
using Game_Kinect_Project.PhysicObjects;
using JigLibX.Vehicles;
using System.Diagnostics;

//Animation
using Xclna.Xna.Animation;

using KinectModel.StateEnum;

namespace Game_Kinect_Project.GameSpace
{
    //第一個遊戲所有的規則與實作接寫入於此
    public class GameSpace1
    {
        //取得聲音
        AudioManager audio;
        
        //物理物件的模型
        Model boxModel, sphereModel, capsuleModel, compoundModel, terrainModel, cylinderModel,
              carModel, wheelModel, staticModel, planeModel, pinModel;
        Model tableModel;
        Model rock_1Model, rock_2Model, rock_3Model, rock_4Model;
        Model tree_1Model, tree_2Model, tree_3Model;
        Model goalModel;

        //金幣
        Model goldModel;
        List<PhysicObject> goldManager = new List<PhysicObject>();

        //物理
        PhysicsSystem physicSystem;
        //物理碰撞的線
        DebugDrawer debugDrawer;
        //遊戲中的視野
        Camera camera;

        //車車的物理
        CarObject carObject;

        //終點
        TriangleMeshObject goalObject;

        //人物用的物理
        CharacterObject character;

        //物理
        ConstraintWorldPoint objectController = new ConstraintWorldPoint();
        ConstraintVelocity damperController = new ConstraintVelocity();

        /// <summary>
        /// 人物動畫
        /// </summary>
        ModelAnimator dwarfAnimator;
        AnimationController idle;
        AnimationController walk, run, nod, crouch, stayCrouched, die, swip, jump;

        /// <summary>
        /// 蜘蛛怪動畫
        /// </summary>
        ModelAnimator beastAnimator;
        AnimationController beastIdle;
        AnimationController beastWalk;
        AnimationController beastBite;

        //人物在相機中的位置
        Vector3 camOffset = new Vector3(0, 4, -15);
        Matrix camerRotation = Matrix.Identity;

        //初始化動畫人物的座標
        Vector3 dwarfPosition = Vector3.Zero;
        Vector3 beastPosition = Vector3.Zero;

        //測試用的
        public Vector3 DwarfPosition { get { return dwarfPosition; } }

        //怪物速度
        Vector3 beastVector;

        //混和參數
        float blendFactor = 0;
        Matrix rotation = Matrix.Identity;

        //腳色控制相關
        string state = "idle";
        float currentSpeed = 0;
        const float WALK_SPEED = .115f;
        //跑步速度
        const float RUN_SPEED = .5f;

        //遊戲一開始的時間
        float gameTimes = 0;

        //讓其他object取得遊戲時間
        public int GameTims
        {
            get
            {
                return (int)((gameTimes) );
            }
            set { gameTimes = value; }
        }

        public Vector3 CurrentUserPosition
        { get { return input.CurrentUserPosition; } }
        /// <summary>
        /// 畫面的中心
        /// </summary>
        public int CenterPosition
        { get { return input.CenterPosition; } }

        //判斷玩家是否有撞到物體
        bool isPlayerHitObject = false;

        //被障礙物阻擋
        bool isBlocked = false;
        int BlockedTime = 0;

        //遊戲開始才能被控制
        bool isCanBeCtrl = false;

        //天空
        Sky.SkyBox skyBox;

        //水物件
        List<WaterI> WaterList = new List<WaterI>();

        //水的減速
        public static bool isSpeedDown = false;
        public bool IsSpeedDown { get { return isSpeedDown; } }
        float speedDownRatio = 1.0f;

        //所有物件的Collision
        List<CollisionSkin> Skin = new List<CollisionSkin>();
        //水的碰撞
        List<CollisionSkin> WaterSkin = new List<CollisionSkin>();
        //金幣
        List<CollisionSkin> GoldSkin = new List<CollisionSkin>();

        List<int> UserScore = new List<int>();

        public List<int> GetUserScore { get { return UserScore; } }

        //Animation的Controller
        private void RunController(ModelAnimator animator, AnimationController controller)
        {
            foreach (BonePose p in animator.BonePoses)
            {
                p.CurrentController = controller;
                p.CurrentBlendController = null;
            }
        }

        //TEST
        public InputManager test { get { return input; } }

        /// <summary>
        /// 管理整個3D遊戲
        /// </summary>
        GameComponentCollection Components;
        Game1 game;
        public GameSpace1(Game1 game, GameComponentCollection Components,AudioManager audio,Camera camera)
        {
            //聲音
            this.audio = audio;
            this.camera = camera;

            //矮人
            dwarfPosition.Y -= 15;
            dwarfPosition.Z += 200;
            beastPosition.Y -= 15;
            beastPosition.Z += 240;
            beastVector = beastPosition;
            //抓XNA framework的東西
            this.game = game;
            this.Components = Components;

            //物理相關初始化
            physicSystem = new PhysicsSystem();
            physicSystem.CollisionSystem = new CollisionSystemSAP();
            physicSystem.EnableFreezing = true;
            physicSystem.SolverType = PhysicsSystem.Solver.Normal;
            physicSystem.CollisionSystem.UseSweepTests = true;

            physicSystem.NumCollisionIterations = 10;
            physicSystem.NumContactIterations = 10;
            physicSystem.NumPenetrationRelaxtionTimesteps = 15;
            //物理相關初始化
            physicSystem.Gravity = new Vector3(0, -50, 0);          

            //碰撞線
            debugDrawer = new DebugDrawer(game);
            debugDrawer.Enabled = false;

            //交給XNA管理          
            Components.Add(debugDrawer);

            //畫線相關參數          
            debugDrawer.DrawOrder = 10;
        }

        public PhysicsSystem PhysicSystem { get { return this.physicSystem; } }

        public void ReSetStage()
        {
            dwarfPosition = Vector3.Zero;
            beastPosition = Vector3.Zero;
            GameOptions.DuringBeastTime = false;
            GameOptions.StartRunTime = false;
            GameOptions.PlayerLive = 10;
            ResetScene();

            character.PhysicsBody.CollisionSkin.callbackFn += new CollisionCallbackFn(handleCollisionDetection);

            lockCamera = false;

            //矮人位置
            dwarfPosition.X -= 0;
            dwarfPosition.Y -= 15;
            dwarfPosition.Z += 200;
            beastPosition.X = 0;
            beastPosition.Y -= 15;
            beastPosition.Z = 240;
            beastVector = beastPosition;

            CreateObstructions();

            //攝影機的初始位置
            camera.Position = new Vector3(6.017231f, -14.5945f, 222.464f);
            camera.Angles = new Vector2(0.4166675f, 0.6126655f);

            //主角初始位置
            character.PhysicsBody.Position = new Vector3(0, 0, 220);

            gameTimes = 0;
            beastIdle.IsLooping = true;
            die.IsLooping = true;
            state = "idle";
            RunController(dwarfAnimator, idle);

            GameOptions.GameOver = false;
            GameOptions.MaxBlockedTime = 60;
            GameOptions.GoldEarn = 0;
            GameOptions.GameFinish = false;
            isFinish = false;
            isCanBeCtrl = false;
            character.CharacterBody.DesiredVelocity = Vector3.Zero;
        }

        /// <summary>
        /// DebugDrawer
        /// </summary>
        public DebugDrawer DebugDrawer
        {
            get { return debugDrawer; }
        }

        /// <summary>
        /// 攝影機
        /// </summary>
        public Camera Camera
        {
            get { return camera; }
        }

        //清除Componen所有的物件(結束時會用到)
        public void ClearComponents()
        {
            List<PhysicObject> toBeRemoved = new List<PhysicObject>();
            foreach (GameComponent gc in this.Components)
            {
                if (gc is PhysicObject && !(gc is HeightmapObject)
                    && !(gc is CarObject) && (gc is TriangleMeshObject)
                    && !(gc is PlaneObject))
                {
                    PhysicObject physObj = gc as PhysicObject;
                    TriangleMeshObject triangleObj = gc as TriangleMeshObject;
                    toBeRemoved.Add(physObj);
                    toBeRemoved.Add(triangleObj);
                }
            }

            this.Components.Remove(skyBox);
            skyBox.Dispose();

            foreach (PhysicObject physObj in toBeRemoved)
            {
                physObj.PhysicsBody.DisableBody();
                this.Components.Remove(physObj);
                physObj.Dispose();
            }

            foreach (WaterI water in WaterList) 
            {
                this.Components.Remove(water);
                water.Dispose();
            }

            //把障礙物的skin移除
            foreach (TriangleMeshObject triangleObj in toBeRemoved)
            {
                triangleObj.PhysicsSkin.CollisionSystem.RemoveCollisionSkin(triangleObj.PhysicsSkin);
                this.Components.Remove(triangleObj);
                triangleObj.Dispose();
            }

            this.Components.Remove(character);
            character.Dispose();

            int count = physicSystem.Controllers.Count;
            for (int i = 0; i < count; i++) physicSystem.Controllers[0].DisableController();
            count = physicSystem.Constraints.Count;
            for (int i = 0; i < count; i++) physicSystem.RemoveConstraint(physicSystem.Constraints[0]);
        }

        /// <summary>
        /// 讀取遊戲中的資源
        /// </summary>
        public void LoadFiles(ContentManager content)
        {
            //讀取資源
            boxModel = content.Load<Model>("box");
            sphereModel = content.Load<Model>("sphere");
            capsuleModel = content.Load<Model>("capsule");
            carModel = content.Load<Model>("box");
            wheelModel = content.Load<Model>("wheel");
            staticModel = content.Load<Model>("staticmesh");
            planeModel = content.Load<Model>("plane");
            pinModel = content.Load<Model>("pin");
            compoundModel = content.Load<Model>("compound");
            cylinderModel = content.Load<Model>("cylinder");

            rock_1Model = content.Load<Model>("rock/rock_1");
            rock_2Model = content.Load<Model>("rock/rock_2");
            rock_3Model = content.Load<Model>("rock/rock_4");
            rock_4Model = content.Load<Model>("rock/rock_5");

            tree_1Model = content.Load<Model>("tree/Arvore2");
            tree_2Model = content.Load<Model>("tree/tree");
            tree_3Model = content.Load<Model>("tree/tree1");

            goalModel = content.Load<Model>("goal/goal");
            goldModel = content.Load<Model>("gold/coin");

            //for (int i = 0; i < 50;i++ )
            //{
            //    SphereObject aa = new SphereObject(game, goldModel, 1.0f, Matrix.Identity, new Vector3(i-20, 200 + i*200, 200));
            //    this.Components.Add(aa);
            //}

            //heightMap地圖
            try
            {
                // some video card can't handle the >16 bit index type of the terrain
                terrainModel = content.Load<Model>("testTerr");
                HeightmapObject heightmapObj = new HeightmapObject(game, terrainModel, Vector2.Zero);
                this.Components.Add(heightmapObj);
                Model terrainModel2 = content.Load<Model>("testTerr2");
                Vector2 newTerrain = new Vector2(0, -255 * 2);
                HeightmapObject test = new HeightmapObject(game, terrainModel2, newTerrain);
                this.Components.Add(test);
                newTerrain.Y = -255 * 4;
                HeightmapObject test2 = new HeightmapObject(game, terrainModel, newTerrain);
                this.Components.Add(test2);
            }
            catch (Exception)
            {
                // if that happens just createa a ground plane 
                PlaneObject planeObj = new PlaneObject(game, planeModel, 15.0f);
                this.Components.Add(planeObj);
            }

            tableModel = content.Load<Model>("table");

            CreateObstructions();         

            //終點物理
            goalObject = new TriangleMeshObject(game, goalModel, Matrix.Identity, new Vector3(-30, -15, -1230));
            this.Components.Add(goalObject);

            //攝影機的初始位置
            camera.Position = new Vector3(6.017231f, -14.5945f, 222.464f);
            camera.Angles = new Vector2(0.4166675f, 0.6126655f);
            // camera.Position = new Vector3(0, 0, 100);

            //人物的物理
            character = new CharacterObject(game, new Vector3(0, 0, 220));
            this.Components.Add(character);

            //測試用的 暫時沒用
            CollisonBox testBox = new CollisonBox(game, Matrix.Identity, dwarfPosition);
            this.Components.Add(testBox);

            //SkyBox的物件
            skyBox = new Sky.SkyBox(game, "sky/xFile/skybox", "sky/skybox", "sky/SkyBoxTex", camera);
            Components.Add(skyBox);

            float[] waterPositionZ = new float[3] { 171, 82, 26 };
            Vector3[] waterPosition = new Vector3[] { new Vector3(-5, -14.5f, 171), new Vector3(7, -14.5f, 75), new Vector3(0, -14.5f, 26), new Vector3(-17, -14.5f, -13), new Vector3(4, -14.5f, -78), new Vector3(-16, -14.5f, -123), new Vector3(0, -14.5f, -170),
            new Vector3(-17, -14.5f, -33),new Vector3(5, -14.5f, -216),new Vector3(-1, -14.5f, -248),//第一層
            new Vector3(-6, -14.5f, -315),new Vector3(-2, -14.5f, -394),new Vector3(3.5f, -14.5f, -418),new Vector3(-13, -14.5f, -486),new Vector3(3, -14.5f, -562),new Vector3(-2, -14.5f, -600),new Vector3(-15, -14.5f, -658),new Vector3(13, -14.5f, -693),new Vector3(0, -14.5f, -736),
            };

            for (int i = 0; i < waterPosition.Length; i++)
            {
                //Water
                WaterI water = new WaterI(game, "sky/SkyBoxTex", camera);
                water.Position = waterPosition[i];
                water.BumpSpeed = new Vector2(0, 0.2f);
                water.WaveAmplitude = 0.1f;

                if (i == 10)
                    water.Height = 32;

                Components.Add(water);
                WaterList.Add(water);

                //加入collision
                WaterCollision random = new WaterCollision(game, tableModel, Matrix.Identity, water.Position);
                this.Components.Add(random);
                WaterSkin.Add(random.PhysicsSkin);
            }

            //Animation
            //人物模型
            Model model = content.Load<Model>("animation/dwarfmodel");
            //怪物模型
            Model beast = content.Load<Model>("animation/beast");

            //shader資訊
            Effect myEffect = content.Load<Effect>("animation/skinFX");

            //取代原本的effect shader
            #region 替換EFFECT
            if (GameOptions.Effect)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        BasicEffect oldEffect = (BasicEffect)part.Effect;
                        Effect newEffect = myEffect.Clone();
                        newEffect.Parameters["Texture"].SetValue(oldEffect.Texture);

                        newEffect.Parameters["LightColor"].SetValue(new Vector4(0.3f, 0.3f, 0.3f, 1.0f));
                        newEffect.Parameters["AmbientLightColor"].SetValue(new Vector4(1.25f, 1.25f, 1.25f, 1.0f));
                        newEffect.Parameters["Shininess"].SetValue(0.6f);
                        newEffect.Parameters["SpecularPower"].SetValue(0.4f);

                        newEffect.Parameters["View"].SetValue(camera.View);
                        newEffect.Parameters["Projection"].SetValue(camera.Projection);

                        part.Effect = newEffect;
                        oldEffect.Dispose();
                    }
                }

                //取代原本的effect shader
                foreach (ModelMesh mesh in beast.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        BasicEffect oldEffect = (BasicEffect)part.Effect;
                        Effect newEffect = myEffect.Clone();
                        newEffect.Parameters["Texture"].SetValue(oldEffect.Texture);

                        newEffect.Parameters["LightColor"].SetValue(new Vector4(0.3f, 0.3f, 0.3f, 1.0f));
                        newEffect.Parameters["AmbientLightColor"].SetValue(new Vector4(1.25f, 1.25f, 1.25f, 1.0f));
                        newEffect.Parameters["Shininess"].SetValue(0.6f);
                        newEffect.Parameters["SpecularPower"].SetValue(0.4f);

                        newEffect.Parameters["View"].SetValue(camera.View);
                        newEffect.Parameters["Projection"].SetValue(camera.Projection);

                        part.Effect = newEffect;
                        oldEffect.Dispose();
                    }
                }
                GameOptions.Effect = false;
            }
            #endregion

            //人物動畫
            dwarfAnimator = new ModelAnimator(game, model);
            //怪物動畫
            beastAnimator = new ModelAnimator(game, beast);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    if (effect is BasicEffect)
                    {
                        BasicEffect basic = (BasicEffect)effect;
                        basic.View = camera.View;
                        basic.Projection = camera.Projection;

                    }
                    else if (effect is BasicPaletteEffect)
                    {
                        BasicPaletteEffect palette = (BasicPaletteEffect)effect;
                        palette.View = camera.View;
                        palette.Projection = camera.Projection;

                        //開啟光源
                        palette.EnableDefaultLighting();
                        palette.DirectionalLight0.Direction = new Vector3(0, 0, 1);
                    }
                }
            }

            //人物動畫Controller
            idle = new AnimationController(game, dwarfAnimator.Animations["idle0"]);
            RunController(dwarfAnimator, idle);

            //怪物動畫Controller
            beastIdle = new AnimationController(game, beastAnimator.Animations["run"]);
            beastIdle.SpeedFactor = 5;
            beastWalk = new AnimationController(game, beastAnimator.Animations["walk"]);
            beastWalk.SpeedFactor = 10;
            beastBite = new AnimationController(game, beastAnimator.Animations["bite"]);
            beastBite.SpeedFactor = 10;

            //人物動畫Controller
            run = new AnimationController(game, dwarfAnimator.Animations["run"]);
            walk = new AnimationController(game, dwarfAnimator.Animations["walk"]);
            crouch = new AnimationController(game, dwarfAnimator.Animations["crouchDown"]);
            stayCrouched = new AnimationController(game, dwarfAnimator.Animations["stayCrouched"]);
            nod = new AnimationController(game, dwarfAnimator.Animations["nodHead"]);
            die = new AnimationController(game, dwarfAnimator.Animations["dieForwards"]);
            die.SpeedFactor = 0.2;
            swip = new AnimationController(game, dwarfAnimator.Animations["Attack"]);
            jump = new AnimationController(game, dwarfAnimator.Animations["jump"]);

            //碰撞事件
            character.PhysicsBody.CollisionSkin.callbackFn += new CollisionCallbackFn(handleCollisionDetection);

            ReSetStage();

        }

        //創造障礙物
        private void CreateObstructions()
        {
            Random r = new Random();
            for (int i = 0; i < 50; i++)
            {
                int x = Int32.Parse(r.Next(-15, 15).ToString());
                int z = Int32.Parse(r.Next(-1250, 190).ToString());
                Vector3 vector = new Vector3((float)x, (float)-15, (float)z);
                if (i % 7 == 0)
                {
                    TriangleMeshObject tree = new TriangleMeshObject(game, tree_1Model, Matrix.Identity, vector);
                    this.Components.Add(tree);
                    Skin.Add(tree.PhysicsSkin);
                }
                else if (i % 7 == 1)
                {
                    vector.X -= 10;
                    TriangleMeshObject tree = new TriangleMeshObject(game, tree_2Model, Matrix.Identity, vector);
                    this.Components.Add(tree);
                    Skin.Add(tree.PhysicsSkin);
                }
                else if (i % 7 == 2)
                {
                    vector.X -= 25;
                    TriangleMeshObject tree = new TriangleMeshObject(game, tree_3Model, Matrix.Identity, vector);
                    this.Components.Add(tree);
                    Skin.Add(tree.PhysicsSkin);
                }
                else if (i % 7 == 3)
                {
                    TriangleMeshObject rock = new TriangleMeshObject(game, rock_1Model, Matrix.Identity, vector);
                    this.Components.Add(rock);
                    Skin.Add(rock.PhysicsSkin);
                }
                else if (i % 7 == 4)
                {
                    TriangleMeshObject rock = new TriangleMeshObject(game, rock_2Model, Matrix.Identity, vector);
                    this.Components.Add(rock);
                    Skin.Add(rock.PhysicsSkin);
                }
                else if (i % 7 == 5)
                {
                    TriangleMeshObject rock = new TriangleMeshObject(game, rock_3Model, Matrix.Identity, vector);
                    this.Components.Add(rock);
                    Skin.Add(rock.PhysicsSkin);
                }
                else
                {
                    TriangleMeshObject rock = new TriangleMeshObject(game, rock_4Model, Matrix.Identity, vector);
                    this.Components.Add(rock);
                    Skin.Add(rock.PhysicsSkin);
                }

            }
            Random r2 = new Random();
            for (int i = 0; i < 50; i++)
            {
                int x = Int32.Parse(r2.Next(-15, 15).ToString());
                int z = Int32.Parse(r2.Next(-1250, 190).ToString());
                Vector3 vector = new Vector3((float)x, (float)-15, (float)z);
                vector.Y = -13;
                TriangleMeshObject gold = new TriangleMeshObject(game, goldModel, Matrix.Identity, vector);
                this.Components.Add(gold);
                GoldSkin.Add(gold.PhysicsSkin);
                goldManager.Add(gold);
            }
        }

        /// <summary>
        /// Unload game files
        /// </summary>
        public void UnloadFiles()
        {
            boxModel = null;
            sphereModel = null;
            capsuleModel = null;
            carModel = null;
            wheelModel = null;
            staticModel = null;
            planeModel = null;
            pinModel = null;
            compoundModel = null;
            cylinderModel = null;

            run = null;
            walk = null;
            crouch = null;
            stayCrouched = null;
            nod = null;
            die = null;
            swip = null;
            jump = null;

            rock_1Model = null;
            rock_2Model = null;
            rock_3Model = null;
            rock_4Model = null;

            tree_1Model = null;
            tree_2Model = null;
            tree_3Model = null;

            goalModel = null;
            goldModel = null;

            terrainModel = null;

            tableModel = null;



        }

        InputManager input;
        public void GetInput(InputManager input)
        {
            this.input = input;
        }

        GraphicsDevice gd;
        public void GetGraphicsDevice(GraphicsDevice gd)
        {
            this.gd = gd;
        }

        public void UPDATEEEEE(float elapsedTime)
        {//物理系統的update
            physicSystem.Integrate(elapsedTime);
        }

        int finishCount = 0;

        /// <summary>
        /// 遊戲時間+規則管理
        /// </summary>
        private void UpdateGameTimes()
        {
           // GameOptions.GM1_Over = true;
            for (int i = 0; i < goldManager.Count; i++)
                goldManager[i].PhysicsBody.Orientation *= Matrix.CreateRotationY(0.2f);

            if (gameTimes <= 50000)
                gameTimes += GameOptions.GameTimeStep;

            //怪物出現
            if (gameTimes == GameOptions.BeastStartTime)
            {                
                audio.PlayAudio("19. smb_warning");
                GameOptions.DuringBeastTime = true;

                camera.Position = new Vector3(
                -0.8470028f
                , -3.827537f
                , 222.2229f);

                Vector2 ang = new Vector2(-0.6f, 3.11f);
                camera.Angles = ang;

                RunController(beastAnimator, beastIdle);
                beastIdle.IsLooping = false;
            }

            //開始遊戲
            if (gameTimes == GameOptions.PlayerStartTime)
            {                
                audio.PlayOverworldBGM();
                GameOptions.DuringBeastTime = false;
                GameOptions.StartRunTime = true;
                isCanBeCtrl = true;
                camera.Angles = Vector2.Zero;
                lockCamera = !lockCamera;
            }

            //怪物開始追玩家
            if (GameOptions.StartRunTime && gameTimes > 1400)
            {
                float beastMoveStep = 0.2f;
                RunController(beastAnimator, beastWalk);
                beastPosition.Z += -beastMoveStep;
            }

            //損寫
            if (isPlayerHitObject)
            {
                audio.PlayAudio("15. smb3_tail");
                GameOptions.PlayerLive -= 1;
                isPlayerHitObject = !isPlayerHitObject;
            }

            //計算被阻擋時間
            if (isBlocked)
            {
                const int BlockedTimeStep = 1;
                BlockedTime += BlockedTimeStep;
            }

            //調整攝影機位置
            Vector3 adjustPosition = dwarfPosition;
            adjustPosition.X += 0.1f;
            adjustPosition.Y += 10;
            adjustPosition.Z -= 15;
            Vector2 adjustAngle = new Vector2(-0.6f, 3.11f);

            //死亡
            if (!GameOptions.GameOver)
            {
                if (GameOptions.PlayerLive == 0 || beastPosition.Z < dwarfPosition.Z + 2)
                {
                    isCanBeCtrl = false;
                    lockCamera = false;
                    camera.Angles = adjustAngle;
                    camera.Position = adjustPosition;
                    //ResetScene();
                    state = "die";
                    die.IsLooping = false;
                    RunController(dwarfAnimator, die);

                    beastPosition.X = dwarfPosition.X;
                    beastPosition.Y = dwarfPosition.Y;
                    GameOptions.StartRunTime = false;
                    RunController(beastAnimator, beastBite);
                    GameOptions.GameOver = true;

                    audio.StopOverworldBGM();
                    audio.PlayDeadBGM("01. nsmb_death");
                    GameOptions.PlayerLive = -1;
                }
            }

            if (GameOptions.GameOver)
            {
                //GameOptions.GameOver = !GameOptions.GameOver;
                character.PhysicsBody.Velocity *= 0;

                finishCount++;

                if (finishCount == 500)
                {
                    goToEnd = true;
                    finishCount = 0;

                    UserScore.Add(score);
                }           
            }

            //到達終點
            if (!GameOptions.GameFinish)
            {
                if (isFinish)
                {
                    isCanBeCtrl = false;
                    lockCamera = false;
                    audio.StopOverworldBGM();
                    audio.PlayVictoryBGM();
                    GameOptions.StartRunTime = false;
                    GameOptions.GameFinish = true;
                    character.CharacterBody.DesiredVelocity = Vector3.Zero;                  
                }
                if (dwarfPosition.Z < goalObject.PhysicsBody.Position.Z)
                    isFinish = true;

                score = (int)gameTimes * GameOptions.GoldEarn;
            }

            else if (GameOptions.GameFinish)
            {               
                //goToEnd = true;
                character.CharacterBody.DesiredVelocity = Vector3.Zero;
                camera.Angles = adjustAngle;
                camera.Position = adjustPosition;
                state = "jump";
                RunController(dwarfAnimator, jump);
                jump.IsLooping = true;
                finishCount++;
                
                if (finishCount == 500)
                {
                    goToEnd = true;
                    finishCount = 0;
                    
                    UserScore.Add(score);
                }              
                
            }

            //減速
            if (isSpeedDown)
            {
                speedDownCounter++;
                speedDownRatio = 0.5f;
                if (speedDownCounter == 200)
                    isSpeedDown = !isSpeedDown;
            }
            else
            {
                speedDownCounter = 0;
                speedDownRatio = 1.0f;
            }
        }
        int score = 0;
        int speedDownCounter = 0;
        bool isFinish = false;

        public int GetScore { get { return score; } }

        //清除物理物件
        #region 清除物理
        public void ResetScene()
        {
            character.PhysicsBody.CollisionSkin.callbackFn -= handleCollisionDetection;
            List<PhysicObject> toBeRemoved = new List<PhysicObject>();
            foreach (GameComponent gc in this.Components)
            {
                if (gc is PhysicObject && !(gc is HeightmapObject)
                    && !(gc is CarObject) && (gc is TriangleMeshObject)
                    && !(gc is PlaneObject))
                {
                    PhysicObject physObj = gc as PhysicObject;
                    TriangleMeshObject triangleObj = gc as TriangleMeshObject;
                    toBeRemoved.Add(physObj);
                    toBeRemoved.Add(triangleObj);
                }
            }

            foreach (PhysicObject physObj in toBeRemoved)
            {
                physObj.PhysicsBody.DisableBody();
                this.Components.Remove(physObj);
                physObj.Dispose();
            }

            //把障礙物的skin移除
            foreach (TriangleMeshObject triangleObj in toBeRemoved)
            {
                triangleObj.PhysicsSkin.CollisionSystem.RemoveCollisionSkin(triangleObj.PhysicsSkin);
                this.Components.Remove(triangleObj);
                triangleObj.Dispose();
            }

            int count = physicSystem.Controllers.Count;
            for (int i = 0; i < count; i++) physicSystem.Controllers[0].DisableController();
            count = physicSystem.Constraints.Count;
            for (int i = 0; i < count; i++) physicSystem.RemoveConstraint(physicSystem.Constraints[0]);
        }
        #endregion

        private bool goToEnd = false;
        //儲存正在阻擋玩家的物件
        List<CollisionSkin> Blocked = new List<CollisionSkin>();
        List<CollisionSkin> Golded = new List<CollisionSkin>();
        //碰撞事件
        public bool handleCollisionDetection(CollisionSkin owner, CollisionSkin collidee)
        {
            //測試用(撞到車子)
            //if (collidee.Equals(carObject.PhysicsSkin))
            //{
            //    isPlayerHitObject = true;
            //    return true;
            //}
            //終點
            if (collidee.Equals(goalObject.PhysicsSkin))
            {
                isFinish = true;
                return false;
            }
            //撞到new出來的隨機物件
            for (int i = 0; i < Skin.Count; i++)
            {
                if (collidee.Equals(Skin[i]))
                {
                    BlockedTime = 0;
                    isPlayerHitObject = true;
                    Blocked.Add(Skin[i]);
                    Skin[i] = null;
                    isBlocked = true;
                    return true;
                }
            }
            //被阻擋時間一到可以無視碰撞
            for (int i = 0; i < Blocked.Count; i++)
            {
                if (collidee.Equals(Blocked[i]))
                {
                    if (BlockedTime >= GameOptions.MaxBlockedTime)
                    {
                        //Blocked[i] = null;
                        isBlocked = !isBlocked;
                        return false;
                    }
                }
            }
            //水的減速
            for (int i = 0; i < WaterSkin.Count; i++)
            {
                if (collidee.Equals(WaterSkin[i]))
                {
                    isSpeedDown = true;
                    return false;
                }
            }
            //金幣
            for (int i = 0; i < GoldSkin.Count; i++)
            {
                if (collidee.Equals(GoldSkin[i]))
                {
                    Golded.Add(GoldSkin[i]);
                    GoldSkin[i] = null;
                    goldManager[i].drawing = false;
                    GoldSkin.RemoveAt(i);
                    goldManager.RemoveAt(i);
                    GameOptions.GoldEarn += 1;
                    audio.PlayAudio("10. smb3_coin");
                    return false;
                }
            }
            for (int i = 0; i < Golded.Count; i++)
            {
                if (collidee.Equals(Golded[i]))
                {
                    return false;
                }
            }
            //其他物件
            return true;
        }

        //動畫管理
        private void UpdateAnimation(InputManager input, float elapsedTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            
            Vector3 adjust = new Vector3(0.0f, -1.5f, 0.0f);
            dwarfPosition = character.PhysicsBody.Position + adjust;

            //人物動畫控制
            if (GameOptions.PlayerLive > 0 && beastPosition.Z > dwarfPosition.Z + 5 && GameOptions.GameFinish == false)
            {
                #region 人物動畫控制
                BonePoseCollection poses = dwarfAnimator.BonePoses;
                if (state == "idle")
                {
                    currentSpeed = 0;
                    // Add this to the beginning of the if (state=="idle") block in the UpdateState method
                    // Remove the old if (keyState.IsKeyDown(Keys.W)) block
                    //if (input != null)逼A柔
                    //    if (keyState.IsKeyDown(Keys.Up) || input.GetRun)
                    //    {
                    //        blendFactor = 0;
                    //        state = "idleToWalk";
                    //    }
                    if (input != null)
                        if (keyState.IsKeyDown(Keys.Up) || input.IsRun)
                        {
                            blendFactor = 0;
                            state = "idleToWalk";
                        }
                    // Add this in the if (state=="idle") block of the UpdateState method
                    //暫時不用蹲下
                    if (keyState.IsKeyDown(Keys.Space))
                    {
                        crouch.ElapsedTime = 0;
                        //crouch.IsLooping = false;
                        //state = "crouchDown";
                    }
                    RunController(dwarfAnimator, idle);
                }
                else if (state == "walk")
                {
                    currentSpeed = WALK_SPEED;
                    // Add this to the beginning of the if (state=="walk") block in the UpdateState method
                    // Remove the old if (keyState.IsKeyUp(Keys.W)) block
                    if (keyState.IsKeyUp(Keys.Up))
                    {
                        blendFactor = 0;
                        state = "walkToIdle";
                    }
                    if (keyState.IsKeyDown(Keys.LeftShift) && keyState.IsKeyDown(Keys.Up))
                    {
                        blendFactor = 0;
                        state = "walkToRun";
                        run.SpeedFactor = 0;
                    }
                    RunController(dwarfAnimator, walk);
                }
                // Add this to the UpdateState method
                else if (state == "idleToWalk")
                {
                    blendFactor += .1f;
                    currentSpeed = blendFactor * WALK_SPEED;
                    if (blendFactor >= 1)
                    {
                        blendFactor = 1;
                        state = "walk";
                    }
                    foreach (BonePose p in poses)
                    {
                        p.CurrentController = idle;
                        p.CurrentBlendController = walk;
                        p.BlendFactor = blendFactor;
                    }
                }
                // Add this to the UpdateState method
                else if (state == "walkToIdle")
                {
                    blendFactor += .1f;
                    currentSpeed = (1f - blendFactor) * WALK_SPEED;
                    if (blendFactor >= 1)
                    {
                        blendFactor = 1;
                        state = "idle";
                    }
                    foreach (BonePose p in poses)
                    {
                        p.CurrentController = walk;
                        p.CurrentBlendController = idle;
                        p.BlendFactor = blendFactor;
                    }
                }
                // Add this in the UpdateState method
                else if (state == "crouchDown")
                {
                    crouch.AnimationEnded += crouch_AnimationEnded;
                    RunController(dwarfAnimator, crouch);
                }
                else if (state == "stayCrouched")
                {
                    // Add this to the if (state == "stayCrouched") block in the UpdateState method
                    if (keyState.IsKeyDown(Keys.Space))
                    {
                        crouch.ElapsedTime = crouch.AnimationSource.Duration;
                        crouch.SpeedFactor = 0;
                        state = "standUp";
                    }
                    RunController(dwarfAnimator, stayCrouched);
                }
                // Add this to the UpdateState method
                else if (state == "standUp")
                {
                    //gameTime有些問題
                    if (crouch.ElapsedTime - elapsedTime <= 0)
                    {
                        crouch.SpeedFactor = 1;
                        crouch.ElapsedTime = 0;
                        idle.ElapsedTime = 0;
                        state = "idle";
                    }
                    else
                        crouch.ElapsedTime -= (long)elapsedTime;
                    RunController(dwarfAnimator, crouch);
                }
                // Add this to the UpdateState method
                else if (state == "walkToRun")
                {
                    blendFactor += .05f;
                    if (blendFactor >= 1)
                    {
                        blendFactor = 1;
                        run.SpeedFactor = 1;
                        state = "run";
                    }
                    double factor = (double)walk.ElapsedTime / walk.AnimationSource.Duration;
                    run.ElapsedTime = (long)(factor * run.AnimationSource.Duration);
                    currentSpeed = WALK_SPEED + blendFactor * (RUN_SPEED - WALK_SPEED);
                    foreach (BonePose p in poses)
                    {
                        p.CurrentController = walk;
                        p.CurrentBlendController = run;
                        p.BlendFactor = blendFactor;
                    }
                }
                else if (state == "run")
                {
                    currentSpeed = RUN_SPEED;
                    if (keyState.IsKeyUp(Keys.LeftShift))
                    {
                        blendFactor = 0;
                        state = "runToWalk";
                        walk.SpeedFactor = 0;
                    }
                    foreach (BonePose p in poses)
                    {
                        p.CurrentController = run;
                        p.CurrentBlendController = null;
                    }
                }
                else if (state == "runToWalk")
                {
                    blendFactor += .05f;
                    if (blendFactor >= 1)
                    {
                        blendFactor = 1;
                        walk.SpeedFactor = 1;
                        state = "walk";
                    }
                    double factor = (double)run.ElapsedTime / run.AnimationSource.Duration;
                    walk.ElapsedTime = (long)(factor * walk.AnimationSource.Duration);
                    currentSpeed = WALK_SPEED + (1f - blendFactor) * (RUN_SPEED - WALK_SPEED);
                    foreach (BonePose p in poses)
                    {
                        p.CurrentController = run;
                        p.CurrentBlendController = walk;
                        p.BlendFactor = blendFactor;
                    }
                }
                else if (state == "swip")
                {
                    RunController(dwarfAnimator, swip);
                    if (keyState.IsKeyUp(Keys.F))
                        state = "idle";

                }
                if (input != null)
                    if (keyState.IsKeyDown(Keys.F))
                    {
                        state = "swip";
                    }
                #endregion
            }
        }


        /// <summary>
        /// Update
        /// </summary>      
        //拿取物件
        float camPickDistance = 0.0f;
        bool middleButton = false;
        int oldWheel = 0;
        Stopwatch sw = new Stopwatch();
        public void Update(float elapsedTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            if (keyState.IsKeyDown(Keys.R))
                ReSetStage();

            if (keyState.IsKeyDown(Keys.H) || goToEnd)
            {
                goToEnd = false;
                GameOptions.GM1_Over = true;
                GameOptions.GameFinish = false;
            }

            if (keyState.IsKeyDown(Keys.Escape) || GameOptions.isQuit || GameOptions.GM1_Over)
            {
                ClearComponents();
                dwarfAnimator.Dispose();
                beastAnimator.Dispose();
                gameTimes = 0;              
            }

            //暫停
            if (input != null)
                //if (input.UserState == UserStateEnum.Tracking)
                {

                    //更新
                    UpdateGameTimes();
                    UpdateAnimation(input, elapsedTime);

                    if (input != null)
                    {



                        /// <summary>
                        /// 腳色控制
                        /// </summary>
                        Vector3 moveVector = new Vector3();
                        float amountOfMovement = 10.0f;

                        if (keyState.IsKeyDown(Keys.LeftShift))
                            amountOfMovement = 20.0f * speedDownRatio;
                        else
                            amountOfMovement = 10.0f * speedDownRatio;
                        //if (input.Move)BLLLLLLL
                        //{
                        //    if (keyState.IsKeyDown(Keys.Right) || !input.MoveChange)
                        //        moveVector += Vector3.Right;
                        //    if (keyState.IsKeyDown(Keys.Left) || input.MoveChange)
                        //        moveVector += Vector3.Left;
                        //}


                        //Vector3 aa;
                        //aa = character.PhysicsBody.Position;
                        //aa.X = (input.CurrentUserPosition.X - 320) * (15 / 320);
                        //character.PhysicsBody.MoveTo(aa, Matrix.Identity);


                        Vector3 tempPosition = character.PhysicsBody.Position;
                        if (tempPosition.X > 15)
                        {
                            tempPosition.X = 15;
                            character.PhysicsBody.MoveTo(tempPosition, Matrix.Identity);
                        }
                        else if (tempPosition.X < -15)
                        {
                            tempPosition.X = -15;
                            character.PhysicsBody.MoveTo(tempPosition, Matrix.Identity);
                        }

                       
                        if (isCanBeCtrl)
                        {
                            if ((keyState.IsKeyDown(Keys.Right) || input.IsMoveToRight) || input.CurrentUserPosition.X > input.CenterPosition + 50)
                                moveVector += new Vector3(0.5f, 0, 0);
                            if (keyState.IsKeyDown(Keys.Left) || input.IsMoveToLeft || input.CurrentUserPosition.X < input.CenterPosition - 50)
                                moveVector += new Vector3(-0.5f, 0, 0);

                            if (!GameOptions.GameFinish && !GameOptions.GameOver)
                            {
                                if (keyState.IsKeyDown(Keys.Up) || input.IsRun)
                                {
                                    moveVector += Vector3.Forward;
                                }
                                JiggleMath.NormalizeSafe(ref moveVector);
                                //moveVector *= ((input.Speed+1) * amountOfMovement);
                                moveVector *= ((float)input.Speed+1)*100;
                                character.CharacterBody.DesiredVelocity = moveVector;
                            }

                            if (keyState.IsKeyDown(Keys.Space) || input.IsJump)
                            {
                                audio.PlayAudio("05. Jump");
                                character.CharacterBody.DoJump();
                                RunController(dwarfAnimator, jump);
                                jump.IsLooping = false;
                                //state = "jump";
                            }

                            //點左鍵可以丟出物件
                            if (mouseState.LeftButton == ButtonState.Pressed)
                            {
                                PhysicObject physObj = SpawnPrimitive(camera.Position, Matrix.CreateRotationX(0.5f));
                                physObj.PhysicsBody.Velocity = (camera.Target - camera.Position) * 20.0f;
                                Components.Add(physObj);
                            }
                        }
                        //點中鍵可以拿取物件
                        #region Picking Objects with the mouse
                        if (mouseState.MiddleButton == ButtonState.Pressed)
                        {
                            if (middleButton == false)
                            {
                                Vector3 ray = RayTo(mouseState.X, mouseState.Y);
                                float frac; CollisionSkin skin;
                                Vector3 pos, normal;

                                ImmovableSkinPredicate pred = new ImmovableSkinPredicate();

                                physicSystem.CollisionSystem.SegmentIntersect(out frac, out skin, out pos, out normal,
                                    new Segment(camera.Position, ray * 1000.0f), pred);

                                if (skin != null && (skin.Owner != null))
                                {
                                    if (!skin.Owner.Immovable)
                                    {
                                        Vector3 delta = pos - skin.Owner.Position;
                                        delta = Vector3.Transform(delta, Matrix.Transpose(skin.Owner.Orientation));

                                        camPickDistance = (camera.Position - pos).Length();
                                        oldWheel = mouseState.ScrollWheelValue;

                                        skin.Owner.SetActive();
                                        objectController.Destroy();
                                        damperController.Destroy();
                                        objectController.Initialise(skin.Owner, delta, pos);
                                        damperController.Initialise(skin.Owner, ConstraintVelocity.ReferenceFrame.Body, Vector3.Zero, Vector3.Zero);
                                        objectController.EnableConstraint();
                                        damperController.EnableConstraint();
                                    }
                                }

                                middleButton = true;
                            }

                            if (objectController.IsConstraintEnabled && (objectController.Body != null))
                            {
                                Vector3 delta = objectController.Body.Position - camera.Position;
                                Vector3 ray = RayTo(mouseState.X, mouseState.Y); ray.Normalize();
                                float deltaWheel = mouseState.ScrollWheelValue - oldWheel;
                                camPickDistance += deltaWheel * 0.01f;
                                Vector3 result = camera.Position + camPickDistance * ray;
                                oldWheel = mouseState.ScrollWheelValue;
                                objectController.WorldPosition = result;
                                objectController.Body.SetActive();
                            }
                        }
                        else
                        {
                            objectController.DisableConstraint();
                            damperController.DisableConstraint();
                            middleButton = false;
                        }
                        #endregion

                        //物理系統的update
                        physicSystem.Integrate(elapsedTime);

                        //其他座標調整之類的

                        //deBug線
                        debugDrawer.Enabled = input.IsKeyDown(0, Keys.C);

                        //原本寫在update的
                        //物理測試
                        if (currentSpeed > 0)
                        {
                            //character.CharacterBody.DesiredVelocity += (-Matrix.CreateTranslation(0, 0, currentSpeed) * rotation).Translation;
                        }

                        if (input.IsKeyDown(0, Keys.Right))
                        {
                            //rotation *= Matrix.CreateFromAxisAngle(Vector3.Up, -MathHelper.Pi / 25.0f);
                        }

                        if (input.IsKeyDown(0, Keys.Left))
                        {
                            //rotation *= Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi / 25.0f);
                        }

                        //人物的武器
                        BonePose weapon = dwarfAnimator.BonePoses["weapon"];
                        weapon.CurrentController = null;
                        weapon.CurrentBlendController = null;
                        weapon.DefaultTransform =
                            Matrix.CreateRotationX(MathHelper.Pi) *
                            Matrix.CreateRotationY(MathHelper.Pi) *
                            Matrix.CreateRotationZ(MathHelper.Pi) *
                            Matrix.CreateTranslation(weapon.DefaultTransform.Translation);

                        //人物的world
                        dwarfAnimator.World = rotation * Matrix.CreateTranslation(dwarfPosition);

                        //怪物的一開始座標(需要被調整)
                        Matrix adjustBeast = rotation;
                        adjustBeast *= Matrix.CreateFromAxisAngle(Vector3.Up, 3.15f);
                        beastAnimator.World = adjustBeast * Matrix.CreateTranslation(beastPosition);

                        camerRotation = Matrix.CreateRotationX(MathHelper.ToRadians(-18.0f));
                    }

                    //人物update
                    foreach (ModelMesh mesh in dwarfAnimator.Model.Meshes)
                        foreach (Effect effect in mesh.Effects)                            
                            effect.Parameters["View"].SetValue(camera.View);

                    //蜘蛛update
                    foreach (ModelMesh mesh in beastAnimator.Model.Meshes)
                        foreach (Effect effect in mesh.Effects)
                            effect.Parameters["View"].SetValue(camera.View);

                    //測試第三人稱攝影機
                    if (input != null)
                        if (input.IsKeyDown(0, Keys.F1))
                            if (lockCamera)
                                lockCamera = false;
                            else
                                lockCamera = true;

                    //測試鎖定第三人稱
                    if (lockCamera)
                    {
                        Vector3 testCameraPosition = dwarfPosition;
                        testCameraPosition.Z = testCameraPosition.Z + 20;
                        testCameraPosition.Y += 15;
                        camera.Position = testCameraPosition;

                        Vector2 testCameraAngles = camera.Angles;
                        testCameraAngles.X = -0.5235f;
                        testCameraAngles.Y = 0f;
                        camera.Angles = testCameraAngles;
                    }

                }
        }
        //測試鎖定第三人稱
        bool lockCamera = false;

        //蹲下事件
        void crouch_AnimationEnded(object sender, EventArgs e)
        {
            state = "stayCrouched";
            crouch.AnimationEnded -= crouch_AnimationEnded;
        }

        //座標換算
        private Vector3 RayTo(int x, int y)
        {
            Vector3 nearSource = new Vector3(x, y, 0);
            Vector3 farSource = new Vector3(x, y, 1);

            Matrix world = Matrix.CreateTranslation(0, 0, 0);

            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }
            Vector3 nearPoint = gd.Viewport.Unproject(nearSource, camera.Projection, camera.View, world);
            Vector3 farPoint = gd.Viewport.Unproject(farSource, camera.Projection, camera.View, world);

            Vector3 direction = farPoint - nearPoint;
            return direction;
        }

        //隨機射出物件
        Random random = new Random();
        private PhysicObject SpawnPrimitive(Vector3 pos, Matrix ori)
        {
            int prim = random.Next(3);
            PhysicObject physicObj;

            float a = 1.0f + (float)random.NextDouble() * 1.0f;
            float b = a + (float)random.NextDouble() * 0.5f;
            float c = 2.0f / a / b;

            switch (prim)
            {
                case 0:
                    physicObj = new BoxObject(game, boxModel, new Vector3(a, b, c), ori, pos);
                    break;
                case 1:
                    physicObj = new SphereObject(game, sphereModel, 0.5f, ori, pos);
                    break;
                case 2:
                    physicObj = new CapsuleObject(game, capsuleModel, 0.5f, 1f, ori, pos);
                    break;
                default:
                    physicObj = new SphereObject(game, sphereModel, (float)random.Next(5, 15), ori, pos);
                    break;
            }
            return physicObj;
        }

        public void Draw(GraphicsDevice gd) 
        {
            gd.Clear(Color.Blue);
          
            foreach (GameComponent gc in this.Components)
            {
                if (gc is TriangleMeshObject)
                {
                    //PhysicObject physObj = gc as PhysicObject;
                    TriangleMeshObject triangleObj = gc as TriangleMeshObject;
                    triangleObj.draww();
                    //triangleObj.isFrog = true;
                }
                if (gc is HeightmapObject)
                {
                    //PhysicObject physObj = gc as PhysicObject;
                    HeightmapObject triangleObj = gc as HeightmapObject;
                    triangleObj.draww();
                    //triangleObj.isFrog = true;
                }
                if (gc is SphereObject)
                {
                    //PhysicObject physObj = gc as PhysicObject;
                    SphereObject triangleObj = gc as SphereObject;
                    triangleObj.draww();
                    //triangleObj.isFrog = true;
                }
            }
        }

        #region IDisposable Members

        bool isDisposed = false;
        public bool IsDisposed
        {
            get { return isDisposed; }
        }

        public void Dispose()
        {

            GC.SuppressFinalize(this);
        }

        #endregion

        #region ImmovableSkinPredicate
        class ImmovableSkinPredicate : CollisionSkinPredicate1
        {
            public override bool ConsiderSkin(CollisionSkin skin0)
            {
                if (skin0.Owner != null && !skin0.Owner.Immovable && !(skin0.Owner is Character))
                    return true;
                else
                    return false;
            }
        }
        #endregion

    }
}

