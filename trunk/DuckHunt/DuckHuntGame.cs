using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.GamerServices;
#if !WINDOWS_PHONE
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using System.Diagnostics;
using System.IO;
#endif
using System.Xml;
using GameCommon;
#if !WINDOWS_PHONE
using System.Threading.Tasks;
#else
using System.Threading;
using System.IO.IsolatedStorage;
#endif

using StorageSampleREST;
using System.Xml.Linq;


namespace DuckHuntCommon
{

    class DuckHuntGame
    {
        // org point
        public Vector2 orgpoint;
        // default scale
        public float defscale;

        Rectangle localViewRect = new Rectangle();
        Rectangle globalViewRect = new Rectangle(0, 0, 1920, 1080);

        GameData gameData;

        public GameData DuckHuntGameData
        {
            get
            {
                return gameData;
            }
        }

        bool snapview = false;
        bool pause = false;
        public bool Pause
        {
            get
            {
                return pause;
            }
            set
            {
                pause = value;
            }
        }

        public bool SnapView
        {
            get
            {
                return snapview;
            }
            set
            {
                snapview = value;
            }
        }

        GamePage oldPage = null;
        public void SwapSnap(bool snap)
        {
            if (snap)
            {
                oldPage = this.currentPage;
                currentPage = snapPage;
            }
            else
            {
                if (oldPage != null)
                {
                    currentPage = oldPage;
                }
            }
        }

        public void PauseGame(bool pause)
        {
            Pause = pause;
            if (pause)
            {
                // go to mnue page
                mainMenuPage.ShowPause(true);               
                GotoMainMenuPage();
            }
            else
            {
                // go to play page
                mainMenuPage.ShowPause(false);
                ReturnToPrevious();
                //GotoPlayPage();
            }
        }

        public Rectangle GetGlobalViewRect()
        {
            return globalViewRect;
        }

        public Rectangle GetLocalViewRect()
        {
            return localViewRect;
        }



        public float GetLogicDefScale()
        {
            return defscale;
        }

        public Vector2 GetOrgPointInLocalView()
        {
            return orgpoint;
        }

        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //


        // we need to extend the backgound so that local screen has black screen
        Vector2 bgorgpoint;
        float bgdefscale;
        Rectangle localViewRect4Background = new Rectangle();

        public Rectangle GetLocalBgViewRect()
        {
            return localViewRect4Background;
        }

        public float GetLogicBgDefScale()
        {
            return bgdefscale;
        }

        public Vector2 GetBgOrgPointInLocalView()
        {
            return bgorgpoint;
        }

        //
        GameBackGroundPage backgroundPage;
        GameMainMenuPage mainMenuPage;
        GameOverPage gameOverPage;
        GamePlayPage playPage;
        LevelGoalDescriptionPage levelGoalPage;
        LevelConclusionPage levelConclusionPage;
        StagePorpSelectPage propSeletPage;
        GameScoreListPage scoreListPage;
        GameConfigPage optionPage;

        GamePage currentPage = null;

        GameSnapPage snapPage;

        List<GamePage> pagestack;

        public DuckHuntGame()
        {
            gameData = new GameData();

            pagestack = new List<GamePage>();

            backgroundPage = new GameBackGroundPage();
            mainMenuPage = new GameMainMenuPage();
            gameOverPage = new GameOverPage();

            levelGoalPage = new LevelGoalDescriptionPage();
            levelConclusionPage = new LevelConclusionPage();
            propSeletPage = new StagePorpSelectPage();

            playPage = new GamePlayPage();
            scoreListPage = new GameScoreListPage();
            optionPage = new GameConfigPage();

            snapPage = new GameSnapPage();

            currentPage = mainMenuPage;
            //currentPage = gameOverPage;
            pagestack.Add(currentPage);

            currentPage.Active();


            //
            // load config

        }

        public List<GameSound> GetSoundList()
        {
            List<GameSound> soundList = new List<GameSound>();
            GameSound sound = new GameSound();
            sound.phase = GAME_PHASE.SEEK_DUCK;
            sound.soundpath = "Sound\\gameMusic";
            soundList.Add(sound);
            return soundList;
        }


        public void GetResources(out List<ObjectResourceItem> resourceList)
        {
            resourceList = new List<ObjectResourceItem>();

            List<ModelObject> objlst = new List<ModelObject>();
            // sky
            objlst.Add(new SkyModel());
            objlst.Add(new SmokeModel());
            objlst.Add(new CloudModel());
            objlst.Add(new GrassModel());
            objlst.Add(new ForegroundGrassModel());
            objlst.Add(new DuckModel());
            objlst.Add(new DogModel());
            objlst.Add(new BulletModel());
            objlst.Add(new HitBoardModel());
            objlst.Add(new DuckIconModel());
            objlst.Add(new BulletBoardModel());
            objlst.Add(new BulletIconModel());
            objlst.Add(new ScroeBoardModel());
            objlst.Add(new MenuItemModel());
            objlst.Add(new TitleItemModel());
            objlst.Add(new ScroeListBoardModel());
            objlst.Add(new TimeBoardModel());
            objlst.Add(new LostDuckBoardModel());
            objlst.Add(new KeyboardModel());
            objlst.Add(new KeyItemModel());
            objlst.Add(new CheckBoxModel());
            //objlst.Add(new PandaModel());
            objlst.Add(new ButtonModel());
            //objlst.Add(new FireworkModel());
            objlst.Add(new PlaneModel());
            objlst.Add(new BaloonModel());
            objlst.Add(new LevelUpBoardModel());
            objlst.Add(new ParrotModel());
            objlst.Add(new ResultSummaryModel());
            objlst.Add(new InfoBoardModel());
            //objlst.Add(new HunterModel());
            //objlst.Add(new ArrowModel());

            foreach (ModelObject obj in objlst)
            {
                ObjectResourceItem objResItem = new ObjectResourceItem();
                objResItem.objType = obj.Type();
                objResItem.resourceList = obj.GetResourceList();
                resourceList.Add(objResItem);
            }

        }

        public void GetObjects(out List<ModelObject> objlst)
        {
            currentPage.GetObjects(out objlst);
            return;
        }

        public GameBackGroundPage GetBackgroundPage()
        {
            return backgroundPage;
        }

        public GamePlayPage GetPlayPage()
        {
            return playPage;
        }

        public GameMainMenuPage GetMainMenuPage()
        {
            return mainMenuPage;
        }

        public void GotoMainMenuPage()
        {
            pagestack.Add(currentPage);
            currentPage = mainMenuPage;
            currentPage.Active();
        }



        public void GotoPropSelctPage()
        {
            currentPage = propSeletPage;
            currentPage.Active();
        }

        public void GotoLevelConclusionPage(int yourscore, int yourlevel)
        {
            levelConclusionPage.SetConclusion(yourscore, curLevelGoal);
            currentPage = levelConclusionPage;
            currentPage.Active();
        }

        public void GotoGameOverPage(int yourscore, int yourlevel)
        {
            gameOverPage.SetGameResult(yourscore, yourlevel);
            int highestScore = 0, highestLevel = 0;
            gameData.GetGameRecord(out highestScore, out highestLevel);
            gameOverPage.SetGameRecord(highestScore, highestLevel);

            pagestack.Add(currentPage);

            currentPage = gameOverPage;


            AskPlayerNameAndSaveScore(yourscore, yourlevel);
        }

        int curLevelGoal = 0;
        int level = 0;
        public int GetCurrentLevelGoal()
        {
            switch (level)
            {
                case 0:
                    {
                        return 100;
                    }
                    break;
                case 1:
                    {
                        return 600;
                    }
                    break;
                case 2:
                    {
                        return 800;
                    }
                    break;
                case 3:
                    break;
                    {
                        return 900;
                    }
                case 4:
                    {
                        return 1000;
                    }
                    break;
                default:
                    return 600 + level * 100;
            }

            return 100;
        }

        public void StartNextLevel()
        {
            level++;
            playPage.ResetGame();
            GotoPlayPage(false);
        }


        public void GotoLevelGoalDescriptionPage()
        {
            // from configuration page, no need to save
            curLevelGoal = GetCurrentLevelGoal();
            levelGoalPage.ShowLevelGoal(curLevelGoal);
            currentPage = levelGoalPage;
            currentPage.Active();
        }

        public void GotoPlayPage(bool savecurepage)
        {
            if (savecurepage)
            {
                pagestack.Add(currentPage);
            }
            currentPage = playPage;
            currentPage.Active();

        }
        public void GotoConfigPage()
        {
            pagestack.Add(currentPage);
            currentPage = optionPage;
            currentPage.Active();

        }

        public void GotoScoreListPage()
        {
            pagestack.Add(currentPage);
            currentPage = scoreListPage;
            currentPage.Active();

        }

        public bool ReturnToPrevious()
        {
            int count = pagestack.Count;
            if (count <= 1)
            {
                return false;
            }
            currentPage = pagestack[count - 1];
            pagestack.RemoveAt(count - 1);
            currentPage.Active();

            return true;
        }

        public void ShowkeyBoard()
        {
            KeyboardPage keyboardPage = new KeyboardPage(currentPage);
            keyboardPage.InitGamePage(this);
            currentPage = keyboardPage;
        }



        public GameMode gameMode = GameMode.GAME_TIME_LIMIT;

        public void NewGame()
        {
            Pause = false;
            playPage = new GamePlayPage();
            playPage.InitGamePage(this);

            backgroundPage.ShowPause(false);

            playPage.NewGame(gameMode);

            GotoLevelGoalDescriptionPage();
            //GotoPlayPage(true);
        }
        Rectangle screenRect = new Rectangle();


        public bool IsExpired()
        {
            if (!TrialVersion)
            {
                return false;
            }

            DateTime now = DateTime.Now;
            TimeSpan span = now - gameData.installedDate;
            if (span.Days >= 7)
            {
                return true;
            }
            return false;
        }

        bool isTrialVersion = false;
        public bool TrialVersion
        {
            set
            {
                isTrialVersion = value;
            }
            get
            {
                return isTrialVersion;
            }
        }

        void InitCamaerView()
        {

            // logic rect 1600x900
            // calculate our background rect

            float logicWtoH = globalViewRect.Width * 1.0f / globalViewRect.Height;
            // calculate background view
            if ((float)screenRect.Width / screenRect.Height > logicWtoH)
            {
                // too wide, we need fullfill width, so that height should exceed the screen height
                float occupiedh = screenRect.Width * 1.0f / logicWtoH;
                this.bgorgpoint.X = 0;
                this.bgorgpoint.Y = -(occupiedh - screenRect.Height);

                localViewRect4Background.X = (int)bgorgpoint.X;
                localViewRect4Background.Y = (int)bgorgpoint.Y;
                localViewRect4Background.Width = (int)screenRect.Width;
                localViewRect4Background.Height = (int)occupiedh;

                bgdefscale = screenRect.Width * 1.0f / globalViewRect.Width;

            }
            else
            {
                // too high, need to fullfill height, so width will exceed the screen width
                // too wide, we need fullfill width, so that height should exceed the screen height
                float occupiedw = screenRect.Height * logicWtoH;
                this.bgorgpoint.X = -(occupiedw - screenRect.Width) / 2;
                this.bgorgpoint.Y = 0;

                localViewRect4Background.X = (int)bgorgpoint.X;
                localViewRect4Background.Y = (int)bgorgpoint.Y;
                localViewRect4Background.Width = (int)occupiedw;
                localViewRect4Background.Height = (int)screenRect.Height;

                bgdefscale = screenRect.Height * 1.0f / globalViewRect.Height;
            }
            BackgorundCamera.ViewPort = bgorgpoint;
            BackgorundCamera.CameraScale = bgdefscale;
            BackgorundCamera.WorldWidth = globalViewRect.Width;
            BackgorundCamera.WorldHeight = globalViewRect.Height;
            BackgorundCamera.ViewWidth = screenRect.Width;
            BackgorundCamera.ViewHeight = screenRect.Height;



            // calcualte the logic view 
            if ((float)screenRect.Width / screenRect.Height > logicWtoH)
            {
                // to wide, will full filll height
                float occupiedw = screenRect.Height * logicWtoH;
                this.orgpoint.X = (screenRect.Width - occupiedw) / 2;
                this.orgpoint.Y = 0;

                localViewRect.X = (int)orgpoint.X;
                localViewRect.Y = (int)orgpoint.Y;
                localViewRect.Width = (int)occupiedw;
                localViewRect.Height = screenRect.Height;

                this.defscale = localViewRect.Height * 1.0f / globalViewRect.Height;
            }
            else
            {
                // to high, will full fill width
                float occupiedh = screenRect.Width * 1.0f / logicWtoH;
                this.orgpoint.X = 0;

                // align to bottom
                this.orgpoint.Y = screenRect.Height - occupiedh;

                localViewRect.X = (int)orgpoint.X;
                localViewRect.Y = (int)orgpoint.Y;
                localViewRect.Width = screenRect.Width;
                localViewRect.Height = (int)occupiedh;

                this.defscale = localViewRect.Width * 1.0f / globalViewRect.Width;
            }

            //ViewObjectFactory.SetLocalViewInfo(screenRect, orgpoint, defscale, bgorgpoint, bgdefscale);
            Camera.ViewPort = orgpoint;
            Camera.CameraScale = defscale;
            Camera.WorldWidth = globalViewRect.Width;
            Camera.WorldHeight = globalViewRect.Height;
            Camera.ViewWidth = screenRect.Width;
            Camera.ViewHeight = screenRect.Height;
        }

        public void StartGame(Rectangle screenRect1)
        {
            screenRect = screenRect1;
            InitCamaerView();

            gameData.Load("duckhunt.xml");
            if (IsExpired())
            {
                return;
            }

            if (gameData.ScoreList.Count == 0)
            {
                /*
                gameData.AddScore("Penner", 1000);
                gameData.AddScore("Fallson", 2000);
                gameData.AddScore("2013/06/05", 3000);
                gameData.Save("duckhunt.xml");
                 */
            }

            backgroundPage.InitGamePage(this);
            playPage.InitGamePage(this);
            mainMenuPage.InitGamePage(this);
            gameOverPage.InitGamePage(this);
            scoreListPage.InitGamePage(this);
            optionPage.InitGamePage(this);

            levelGoalPage.InitGamePage(this); 
            levelConclusionPage.InitGamePage(this);
            propSeletPage.InitGamePage(this);


            snapPage.InitGamePage(this);


        }

        public static void PlayerInputAsyncCallback(IAsyncResult ar)
        {
#if !WINDOWS_PHONE
            DuckHuntGame game = (DuckHuntGame)ar.AsyncState;
            string playername = DuckHunt.PlayerInputPage.EndShowKeyboardInput(ar);
            game.SaveNewScoreWithPlayerName(playername,game.currentscore, game.currentlevel);
#endif

        }

        public void AskPlayerNameAndSaveScore(int score, int level)
        {
#if !WINDOWS_PHONE
            currentscore = score;
            currentlevel = level;
            DuckHunt.PlayerInputPage.BeginShowKeyboardInput(
                "Input Player Name", "Input Player Name", "", PlayerInputAsyncCallback, this);
#endif
        }

        int currentscore;
        int currentlevel;

        public void SaveNewScoreWithPlayerName(string playename, int score, int level)
        {
            // 
            DateTime now = DateTime.Now;
            string name = playename;
            //name += ".";
            //name += now.ToString();
            gameData.AddScore(name.ToString(), score);
            gameData.AddLevel(name.ToString(), level);
            gameData.Save("duckhunt.xml");

        }


        public void GetGameRecord(out int highestScore, out int highestLevel)
        {
            // 
            gameData.GetGameRecord(out highestScore, out highestLevel);
        }

        public void SaveGameData()
        {
            gameData.Save("duckhunt.xml");
        }

        public void Update(GameTime gametime)
        {
            currentPage.Update(gametime);
            return;
        }
        string sipResult = "You type stuff here.";

        /// <summary>
        /// 输入完成回调方法
        /// </summary>
        /// <param name="result"></param>
        void keyboardCallback(IAsyncResult result)
        {
            string retval = Guide.EndShowKeyboardInput(result);

            if (retval != null)
            {
                sipResult = retval;
            }
        }


        Vector2 oldshootposition = Vector2.Zero;
        public void Click(List<Vector2> clickpositionlist)
        {
            currentPage.Click(clickpositionlist);
            return;
        }


        public void Press(List<Vector2> clickpositionlist)
        {
            currentPage.Press(clickpositionlist);
        }
    }


    class ObjectResourceItem
    {
        public ModelType objType;
        public List<ResourceItem> resourceList;
    }

    class GameRound
    {
        public int deadDuck = 0;
        public string shootername;
    }



    enum GAME_PHASE { GAME_SELECT, SEEK_DUCK, DUCK_FLY, DOG_SHOW, SCORELIST_SHOW, OVER };
    class GameSound
    {
        public GAME_PHASE phase;
        public string soundpath;
    }

    enum GameMode { GAME_TIME_LIMIT, GAME_FREE_MODE, GAME_DEMON};

    enum GameChapterPhase { CHAPTER1, CHAPTER2, CHAPTER3, CHAPTER4, CHAPTER5, FOREVER };


    abstract class GameChapter
    {
        public GameChapter()
        {
        }
        abstract public int GetDuckBatch();
        abstract public bool GetDuckList(out List<AnimalModel> ducks);
        abstract public bool CanBeRemoved();
    }

    abstract class GameChapterBase : GameChapter
    {
        override public int GetDuckBatch()
        {
            return 1;
        }
    }

    class BatchDucks
    {
        // a batch should have compose a shape,
        int count;
        int curve;
    }

    class CommonGameChapter : GameChapterBase
    {
        public int totalcount = 0;
        public int minReleaseInterval = 0;
        public int maxReleaseInterval = 3000; //ms
        public int nextrleaseInterval = 1000;

        int batchcount = 1;
        int releasedDuckCount = 0;
        List<PilotType> pilottypeList = null;
        List<int> duckTypeList = null;

        DateTime lastReleaseTime = DateTime.Now;

        public int MinSpeedRatio
        {
            set;
            get;
        }
        public int MaxSpeedRatio
        {
            get;
            set;
        }

        public CommonGameChapter() 
        {
            pilottypeList = new List<PilotType>();
            pilottypeList.Add(PilotType.DUCK_BEZIER);
            duckTypeList = new List<int>();
            duckTypeList.Add(0);
            duckTypeList.Add(0);
            duckTypeList.Add(0);
            duckTypeList.Add(1);
            duckTypeList.Add(1);
            duckTypeList.Add(1);
            duckTypeList.Add(2);
            MinSpeedRatio = 1;
            MaxSpeedRatio = 1;
        }

        override public int GetDuckBatch()
        {
            return batchcount++;
        }


        Random random = null;
        override public bool GetDuckList(out List<AnimalModel> ducks)
        {
            ducks = null;
            if (CanBeRemoved())
            {
                return false;
            }

            TimeSpan losttime = DateTime.Now - lastReleaseTime;
            if (losttime.TotalMilliseconds < nextrleaseInterval)
            {
                return false;
            }
            lastReleaseTime = DateTime.Now;
            if(random == null)
            {
                random = new Random((int)DateTime.Now.Ticks);
            }
            nextrleaseInterval = random.Next(minReleaseInterval, maxReleaseInterval);

            ducks = null;
            ducks = new List<AnimalModel>();
            AnimalModel duck = null;
            string name = "CommonGameChapter";
            int typeindex = random.Next(99) % duckTypeList.Count;
            int type = duckTypeList[typeindex];
            if (type == 2)
            {
                duck = new ParrotModel(pilottypeList[releasedDuckCount % pilottypeList.Count], name);
            }
            else
            {
                duck = new DuckModel(pilottypeList[releasedDuckCount % pilottypeList.Count], name);
                ((DuckModel)duck).DuckStyle = type;
            }
            duck.SetSpeedRatio(random.Next(MinSpeedRatio, MaxSpeedRatio));
            ducks.Add(duck);
            releasedDuckCount++;

            return true;
        }


        public override bool CanBeRemoved()
        {
            if (releasedDuckCount > totalcount)
            {
                return true;
            }
            return false;
        }
    }

    class TimeChapter1 : GameChapterBase
    {
        int duckcount = 0;
        int batchcount = 1;
        override public int GetDuckBatch()
        {
            return batchcount++;
        }

        override public bool GetDuckList(out List<AnimalModel> ducks)
        {
            ducks = null;
            ducks = new List<AnimalModel>();

            DuckModel duck;
            if (duckcount >= 50)
            {
                ducks = null;
                return false;
            }
            string name = "chapter1_" + duckcount.ToString();
            for (int i = 0; i < 10; i++)
            {
                duck = new DuckModel(PilotType.DUCKLINE, name);
                ducks.Add(duck);
                duckcount++;
            }

            return true;
        }


        public override bool CanBeRemoved()
        {
            if (duckcount >= 50)
            {
                return true;
            }
            return false;
        }
    }


    class TimeChapter2 : GameChapterBase
    {
        int count = 5;
        int times = 0;
        int level = 1;

        List<PilotType> pilotypelist;
        public TimeChapter2()
        {
            pilotypelist = new List<PilotType>();
            pilotypelist.Add(PilotType.DUCKNORMAL);
            //pilotypelist.Add(PilotType.DUCKQUICK);
            //pilotypelist.Add(PilotType.DUCKFLOWER);
            pilotypelist.Add(PilotType.DUCKEIGHT);
            pilotypelist.Add(PilotType.DUCKCIRCLE);
            pilotypelist.Add(PilotType.DUCKELLIPSE);
            pilotypelist.Add(PilotType.DUCKSIN);
            pilotypelist.Add(PilotType.DUCKLINE);

            pilotypelist.Add(PilotType.DUCKEIGHTDEPTH);
            pilotypelist.Add(PilotType.DUCKCIRCLEDEPTH);
            pilotypelist.Add(PilotType.DUCKELLIPSEDEPTH);
            pilotypelist.Add(PilotType.DUCKSINDEPTH);
            //pilotypelist.Add(PilotType.DUCKREN);
        }
        override public bool GetDuckList(out List<AnimalModel> ducks)
        {
            ducks = null;
            ducks = new List<AnimalModel>();

            DuckModel duck;
            if (times >= 1)
            {
                times = 0;
                count += 2;
                ducks = null;
                level++;
                if (level == 3)
                {
                    pilotypelist.Add(PilotType.DUCKEIGHTDEPTH);
                }

                if (level == 5)
                {
                    pilotypelist.Add(PilotType.DUCKCIRCLEDEPTH);
                }

                if (level == 7)
                {
                    pilotypelist.Add(PilotType.DUCKELLIPSEDEPTH);
                }

                if (level == 9)
                {
                    pilotypelist.Add(PilotType.DUCKSINDEPTH);
                }
                return false;
            }
            for (int i = 0; i < count; i++)
            {
                string name = "chapter2_" + count.ToString() + "_" + i.ToString();
                duck = new DuckModel(pilotypelist[i%pilotypelist.Count], name);
                duck.SetSpeedRatio(1 + level * 0.1f);
                ducks.Add(duck);
            }
            for (int i = 0; i < level; i++)
            {
                ducks.Add(new ParrotModel(pilotypelist[i * level % pilotypelist.Count], ""));
            }


            times++;

            return true;
        }


        public override bool CanBeRemoved()
        {
            if (times >= 15)
            {
                return true;
            }
            return false;
        }
    }

    class GameChapter1 : GameChapterBase
    {
        int duckcount = 0;
        override public bool GetDuckList(out List<AnimalModel> ducks)
        {
            ducks = null;
            ducks = new List<AnimalModel>();

            DuckModel duck;
            if (duckcount >= 5)
            {
                ducks = null;
                return false;
            }
            string name = "chapter1_" + duckcount.ToString();
            duck = new DuckModel(PilotType.DUCKNORMAL, name);
            ducks.Add(duck);
            duckcount++;

            return true;
        }


        public override bool CanBeRemoved()
        {
            if (duckcount >= 5)
            {
                return true;
            }
            return false;
        }
    }

    class GameChapter2 : GameChapterBase
    {
        int duckcount = 0;
        override public bool GetDuckList(out List<AnimalModel> ducks)
        {
            ducks = null;
            ducks = new List<AnimalModel>();

            DuckModel duck;
            if (duckcount >= 8)
            {
                ducks = null;
                return false;
            }

            for (int i = 0; i < 2; i++)
            {
                string name = "chapter2_" + duckcount.ToString();
                duck = new DuckModel(PilotType.DUCKNORMAL, name);
                ducks.Add(duck);
                duckcount++;
            }
            return true;
        }

        public override bool CanBeRemoved()
        {
            if (duckcount >= 8)
            {
                return true;
            }
            return false;
        }
    }

    class GameChapter3 : GameChapterBase
    {
        int duckcount = 0;
        int totalcount = 12;
        override public bool GetDuckList(out List<AnimalModel> ducks)
        {
            ducks = null;
            ducks = new List<AnimalModel>();

            AnimalModel duck;
            if (duckcount >= totalcount)
            {
                ducks = null;
                return false;
            }

            string name = "chapter3_" + duckcount.ToString();

            duck = new DuckModel(PilotType.DUCKNORMAL, name);
            ducks.Add(duck);
            duckcount++;

            name = "chapter3_" + duckcount.ToString();
            duck = new DuckModel(PilotType.DUCKEIGHT, name);
            ducks.Add(duck);
            duckcount++;

            name = "chapter3_parrot" + duckcount.ToString();
            duck = new ParrotModel(PilotType.DUCKEIGHT, name);
            ducks.Add(duck);
            duckcount++;


            return true;
        }
        public override bool CanBeRemoved()
        {
            if (duckcount >= totalcount)
            {
                return true;
            }
            return false;
        }
    }

    class GameChapter4 : GameChapterBase
    {
        int duckcount = 0;
        int totalcount = 12;

        override public bool GetDuckList(out List<AnimalModel> ducks)
        {
            ducks = null;
            ducks = new List<AnimalModel>();
            AnimalModel duck;
            if (duckcount >= totalcount)
            {
                ducks = null;
                return false;
            }

            string name = "chapter4_" + duckcount.ToString();

            duck = new DuckModel(PilotType.DUCKSIN, name);
            ducks.Add(duck);
            duckcount++;

            name = "chapter4_" + duckcount.ToString();
            duck = new DuckModel(PilotType.DUCKEIGHT, name);
            ducks.Add(duck);
            duckcount++;

            name = "chapter4_parrot" + duckcount.ToString();
            duck = new ParrotModel(PilotType.DUCKELLIPSEDEPTH, name);
            ducks.Add(duck);
            duckcount++;


            return true;
        }
        public override bool CanBeRemoved()
        {
            if (duckcount >= totalcount)
            {
                return true;
            }
            return false;
        }
    }

    class GameChapter5 : GameChapterBase
    {
        int duckcount = 0;
        int totalcount = 12;
        override public bool GetDuckList(out List<AnimalModel> ducks)
        {
            ducks = null;
            ducks = new List<AnimalModel>();

            AnimalModel duck;
            if (duckcount >= totalcount)
            {
                ducks = null;
                return false;
            }

            string name = "chapter5_" + duckcount.ToString();

            duck = new DuckModel(PilotType.DUCKSIN, name);
            ducks.Add(duck);
            duckcount++;

            name = "chapter5_" + duckcount.ToString();
            duck = new DuckModel(PilotType.DUCKEIGHT, name);
            ducks.Add(duck);
            duckcount++;


            name = "chapter5_" + duckcount.ToString(); 
            duck = new DuckModel(PilotType.DUCKNORMAL, name);
            ducks.Add(duck);
            duckcount++;

            name = "chapter5_parrot" + duckcount.ToString();
            duck = new ParrotModel(PilotType.DUCKNORMAL, name);
            ducks.Add(duck);
            duckcount++;


            return true;
        }

        public override bool CanBeRemoved()
        {
            if (duckcount >= totalcount)
            {
                return true;
            }
            return false;
        }
    }

    class GameChapter6 : GameChapterBase
    {
        int duckcount = 0;
        int totalcount = 15;
        override public bool GetDuckList(out List<AnimalModel> ducks)
        {
            ducks = null;
            ducks = new List<AnimalModel>();

            AnimalModel duck;
            if (duckcount >= totalcount)
            {
                ducks = null;
                return false;
            }

            string name = "chapter6_" + duckcount.ToString();


            duck = new DuckModel(PilotType.DUCKSIN, name);
            ducks.Add(duck);
            duckcount++;

            name = "chapter6_" + duckcount.ToString();
            duck = new DuckModel(PilotType.DUCKEIGHT, name);
            ducks.Add(duck);
            duckcount++;

            name = "chapter6_" + duckcount.ToString();
            duck = new DuckModel(PilotType.DUCKELLIPSE, name);
            ducks.Add(duck);
            duckcount++;

            name = "chapter6_" + duckcount.ToString();
            duck = new DuckModel(PilotType.DUCKCIRCLE, name);
            ducks.Add(duck);
            duckcount++;

            name = "chapter6_parrot" + duckcount.ToString();
            duck = new ParrotModel(PilotType.DUCKNORMAL, name);
            ducks.Add(duck);
            duckcount++;



            return true;
        }

        public override bool CanBeRemoved()
        {
            if (duckcount >= totalcount)
            {
                return true;
            }
            return false;
        }
    }

    class GameChapter7 : GameChapterBase
    {
        int duckcount = 0;
        int totalcount = 18;
        override public bool GetDuckList(out List<AnimalModel> ducks)
        {
            ducks = null;
            ducks = new List<AnimalModel>();

            AnimalModel duck;
            if (duckcount >= totalcount)
            {
                ducks = null;
                return false;
            }

            string name = "chapter7_" + duckcount.ToString();

            duck = new DuckModel(PilotType.DUCKSIN, name);
            ducks.Add(duck);
            duckcount++;

            name = "chapter7_" + duckcount.ToString();
            duck = new DuckModel(PilotType.DUCKEIGHT, name);
            ducks.Add(duck);
            duckcount++;

            name = "chapter7_" + duckcount.ToString();
            duck = new DuckModel(PilotType.DUCKELLIPSE, name);
            ducks.Add(duck);
            duckcount++;

            name = "chapter7_" + duckcount.ToString();
            duck = new DuckModel(PilotType.DUCKCIRCLE, name);
            ducks.Add(duck);
            duckcount++;

            name = "chapter7_" + duckcount.ToString();
            duck = new DuckModel(PilotType.DUCKNORMAL, name);
            ducks.Add(duck);
            duckcount++;

            name = "chapter7_parrot" + duckcount.ToString();
            duck = new ParrotModel(PilotType.DUCKCIRCLE, name);
            ducks.Add(duck);
            duckcount++;


            return true;
        }

        public override bool CanBeRemoved()
        {
            if (duckcount >= 15)
            {
                return true;
            }
            return false;
        }
    }


    class GameChapter8 : GameChapterBase
    {
        int duckcount = 0;
        List<PilotType> pilotypelist;
        int pilotTypeIndex = 0;

        public GameChapter8()
        {
            pilotypelist = new List<PilotType>();
            pilotypelist.Add(PilotType.DUCKCIRCLE);
            pilotypelist.Add(PilotType.DUCKELLIPSE);
            pilotypelist.Add(PilotType.DUCKEIGHT);
            pilotypelist.Add(PilotType.DUCKSIN);
        }
        override public bool GetDuckList(out List<AnimalModel> ducks)
        {
            ducks = null;
            ducks = new List<AnimalModel>();

            AnimalModel duck;
            if (pilotTypeIndex >= pilotypelist.Count)
            {
                ducks = null;
                return false;
            }

            string name = "chapter8_" + duckcount.ToString();

            for (int i = 0; i < 6; i++)
            {
                name = "chapter8_" + duckcount.ToString();
                duck = new DuckModel(pilotypelist[pilotTypeIndex], name);
                //duck.SetSpeedRatio(1.2f);
                ducks.Add(duck);
                duckcount++;
            }
            name = "chapter8_parrot_i" + duckcount.ToString();
            duck = new ParrotModel(PilotType.DUCKEIGHTDEPTH, name);
            ducks.Add(duck);
            name = "chapter8_parrot_l" + duckcount.ToString();
            duck = new ParrotModel(PilotType.DUCKELLIPSEDEPTH, name);
            ducks.Add(duck);

            pilotTypeIndex++;

            return true;
        }

        public override bool CanBeRemoved()
        {
            if (pilotTypeIndex >= pilotypelist.Count)
            {
                return true;
            }
            return false;
        }
    }

    class GameChapter9 : GameChapterBase
    {
        int duckcount = 0;
        List<PilotType> pilotypelist;
        public GameChapter9()
        {
            pilotypelist = new List<PilotType>();
            pilotypelist.Add(PilotType.DUCKCIRCLE);
            pilotypelist.Add(PilotType.DUCKELLIPSE);
            pilotypelist.Add(PilotType.DUCKEIGHT);
            pilotypelist.Add(PilotType.DUCKSIN);
            pilotypelist.Add(PilotType.DUCKEIGHTDEPTH);
            pilotypelist.Add(PilotType.DUCKNORMAL);
            pilotypelist.Add(PilotType.DUCKNORMAL);

        }
        override public bool GetDuckList(out List<AnimalModel> ducks)
        {
            ducks = null;
            ducks = new List<AnimalModel>();

            AnimalModel duck;
            if (duckcount >= 3 * pilotypelist.Count)
            {
                ducks = null;
                return false;
            }

            string name = "chapter9_" + duckcount.ToString();

            int pilottypeindex = 0;
            for (int i = 0; i < pilotypelist.Count; i++)
            {
                name = "chapter9_" + duckcount.ToString();
                duck = new DuckModel(pilotypelist[pilottypeindex % pilotypelist.Count], name);
                //duck.SetSpeedRatio(1.3f);
                ducks.Add(duck);
                duckcount++;
                pilottypeindex++;
            }

            name = "chapter9_parrot_i" + duckcount.ToString();
            duck = new ParrotModel(PilotType.DUCKILOVEU_L, name);
            ducks.Add(duck);
            name = "chapter9_parrot_l" + duckcount.ToString();
            duck = new ParrotModel(PilotType.DUCKELLIPSEDEPTH, name);
            ducks.Add(duck);

            name = "chapter9_parrot_u" + duckcount.ToString();
            duck = new ParrotModel(PilotType.DUCKSINDEPTH, name);
            ducks.Add(duck);


            return true;
        }

        public override bool CanBeRemoved()
        {
            if (duckcount >= 3 * pilotypelist.Count)
            {
                return true;
            }
            return false;
        }
    }


    class GameChapter10 : GameChapterBase
    {
        int duckcount = 0;
        List<PilotType> pilotypelist;
        public GameChapter10()
        {
            pilotypelist = new List<PilotType>();
            pilotypelist.Add(PilotType.DUCKCIRCLE);
            pilotypelist.Add(PilotType.DUCKELLIPSE);
            pilotypelist.Add(PilotType.DUCKEIGHT);
            pilotypelist.Add(PilotType.DUCKSIN);
            pilotypelist.Add(PilotType.DUCKEIGHTDEPTH);
            pilotypelist.Add(PilotType.DUCKEIGHTDEPTH);
            pilotypelist.Add(PilotType.DUCKNORMAL);
            pilotypelist.Add(PilotType.DUCKNORMAL);

        }
        override public bool GetDuckList(out List<AnimalModel> ducks)
        {
            ducks = null;
            ducks = new List<AnimalModel>();

            AnimalModel duck;
            if (duckcount >= 3 * pilotypelist.Count)
            {
                ducks = null;
                return false;
            }

            string name = "chapter10_" + duckcount.ToString();

            int pilottypeindex = 0;
            for (int i = 0; i < 7; i++)
            {
                name = "chapter10_" + duckcount.ToString();
                duck = new DuckModel(pilotypelist[pilottypeindex % pilotypelist.Count], name);
                //duck.SetSpeedRatio(1.5f);
                ducks.Add(duck);
                duckcount++;
                pilottypeindex++;
            }

            name = "chapter10_parrot_i" + duckcount.ToString();
            duck = new ParrotModel(PilotType.DUCKILOVEU_L, name);
            ducks.Add(duck);
            name = "chapter10_parrot_l" + duckcount.ToString();
            duck = new ParrotModel(PilotType.DUCKSINDEPTH, name);
            ducks.Add(duck);

            name = "chapter10_parrot_u" + duckcount.ToString();
            duck = new ParrotModel(PilotType.DUCKEIGHTDEPTH, name);
            ducks.Add(duck);


            return true;
        }

        public override bool CanBeRemoved()
        {
            if (duckcount >= 3 * pilotypelist.Count)
            {
                return true;
            }
            return false;
        }
    }


    class GameChapterFunShowCurve : GameChapterBase
    {
        int duckcount = 0;
        List<PilotType> pilotypelist;
        int pilottypeindex = 0;

        public GameChapterFunShowCurve()
        {
            pilotypelist = new List<PilotType>();
            pilotypelist.Add(PilotType.DUCKEIGHT);
            pilotypelist.Add(PilotType.DUCKCIRCLE);
            pilotypelist.Add(PilotType.DUCKELLIPSE);
            pilotypelist.Add(PilotType.DUCKSIN);
        }
        override public bool GetDuckList(out List<AnimalModel> ducks)
        {
            ducks = null;
            ducks = new List<AnimalModel>();

            DuckModel duck;
            pilottypeindex = pilottypeindex % pilotypelist.Count;


            string name = "chaptershowfuncurve_" + duckcount.ToString();

            for (int i = 0; i < 20; i++)
            {
                duck = new DuckModel(pilotypelist[pilottypeindex % pilotypelist.Count], name);
                ducks.Add(duck);
                duckcount++;
            }
            pilottypeindex++;

            return true;
        }

        public override bool CanBeRemoved()
        {
            if (pilottypeindex >= pilotypelist.Count)
            {
                return true;
            }
            return false;
        }
    }



    class GameChapterILoveU : GameChapterBase
    {
        int duckcount = 0;
        List<PilotType> pilotypelist;
        int pilottypeindex = 0;


        public GameChapterILoveU()
        {
            pilotypelist = new List<PilotType>();
            pilotypelist.Add(PilotType.DUCKILOVEU_I);
            pilotypelist.Add(PilotType.DUCKILOVEU_L);
            pilotypelist.Add(PilotType.DUCKILOVEU_U);
        }
        override public bool GetDuckList(out List<AnimalModel> ducks)
        {
            ducks = null;
            ducks = new List<AnimalModel>();

            DuckModel duck;
            pilottypeindex = pilottypeindex % pilotypelist.Count;


            string name = "";
            if (pilottypeindex == 0)
            {
                name = "chapteriloveui" + duckcount.ToString();

                for (int i = 0; i < 5; i++)
                {
                    duck = new DuckModel(pilotypelist[0], name);
                    ducks.Add(duck);
                    duckcount++;
                }
                pilottypeindex++;
            }
            else if (pilottypeindex == 1)
            {

                name = "chapteriloveul" + duckcount.ToString();
                for (int i = 0; i < 20; i++)
                {
                    duck = new DuckModel(pilotypelist[1], name);
                    ducks.Add(duck);
                    duckcount++;
                }
                pilottypeindex++;
            }
            else if (pilottypeindex == 2)
            {
                name = "chapteriloveuu" + duckcount.ToString();
                for (int i = 0; i < 11; i++)
                {
                    duck = new DuckModel(pilotypelist[2], name);
                    ducks.Add(duck);
                    duckcount++;
                }
                pilottypeindex++;
                pilottypeindex %= 3;
            }

            return true;
        }

        public override int GetDuckBatch()
        {
            return 3;
        }

        public override bool CanBeRemoved()
        {
            if (pilottypeindex >= pilotypelist.Count)
            {
                return true;
            }
            return false;
        }
    }

    class GameChapterForever : GameChapterBase
    {
        int duckcount = 0;
        List<PilotType> pilotypelist;
        int concurrentduck = 8;
        //int concurrentduck = 100;
        //int duckstyle = 8;
        int duckstyle = 1;
        float speedratio = 1f;
        public GameChapterForever()
        {
            pilotypelist = new List<PilotType>();
            pilotypelist.Add(PilotType.DUCKCIRCLE);
            pilotypelist.Add(PilotType.DUCKELLIPSE);
            pilotypelist.Add(PilotType.DUCKEIGHT);
            pilotypelist.Add(PilotType.DUCKSIN);
            pilotypelist.Add(PilotType.DUCKEIGHTDEPTH);
            pilotypelist.Add(PilotType.DUCKNORMAL);
            pilotypelist.Add(PilotType.DUCKLINE);

        }
        override public bool GetDuckList(out List<AnimalModel> ducks)
        {
            ducks = null;
            ducks = new List<AnimalModel>();

            AnimalModel duck;

            string name = "chapterforever_" + concurrentduck.ToString();
            for (int i = 0; i < concurrentduck; i++)
            {
                name = "chapterforever_" + concurrentduck.ToString();
                int pilottypeindex = i % duckstyle;
               
                duck = new DuckModel(pilotypelist[pilottypeindex], name);
                //duck.SetSpeedRatio(speedratio);
                ducks.Add(duck);
                duckcount++;
            }

            name = "chapterlast_parrot_i" + duckcount.ToString();
            duck = new ParrotModel(PilotType.DUCKILOVEU_I, name);
            ducks.Add(duck);
            name = "chapterlast_parrot_l" + duckcount.ToString();
            duck = new ParrotModel(PilotType.DUCKILOVEU_I, name);
            ducks.Add(duck);

            name = "chapterlast_parrot_u" + duckcount.ToString();
            duck = new ParrotModel(PilotType.DUCKILOVEU_I, name);
            ducks.Add(duck);


            concurrentduck++;
            speedratio += 0.1f;

            return true;
        }

        public override bool CanBeRemoved()
        {
            return false;
        }
    }


    class GameChapterManager
    {
        List<GameChapter> chapters;
        List<GameChapter> bonousChapter;

        public GameChapterManager()
        {

        }

        public bool Init(GameMode mode)
        {
            PilotManager.Reset();

            chapters = new List<GameChapter>();
            if (mode == GameMode.GAME_TIME_LIMIT)
            {
                CommonGameChapter chapter = null;
                chapter = new CommonGameChapter();
                chapter.totalcount = 10;
                chapter.MinSpeedRatio = 1;
                chapter.MaxSpeedRatio = 3;
                chapters.Add(chapter);

                chapter = new CommonGameChapter();
                chapter.totalcount = 20;
                chapter.MinSpeedRatio = 2;
                chapter.MaxSpeedRatio = 3;
                chapters.Add(chapter);

                chapter = new CommonGameChapter();
                chapter.totalcount = 30;
                chapter.MinSpeedRatio = 3;
                chapter.MaxSpeedRatio = 5;
                chapters.Add(chapter);

                bonousChapter = new List<GameChapter>();
                GameChapterFunShowCurve curveChapter = new GameChapterFunShowCurve();
                GameChapterILoveU loveuChapter = new GameChapterILoveU();
                bonousChapter.Add(loveuChapter);
                bonousChapter.Add(curveChapter);

            }
            else if (mode == GameMode.GAME_FREE_MODE)
            {
                CommonGameChapter chapter = null;
                chapter = new CommonGameChapter();
                chapter.totalcount = 10;
                chapter.MinSpeedRatio = 1;
                chapter.MaxSpeedRatio = 3;
                chapters.Add(chapter);

                chapter = new CommonGameChapter();
                chapter.totalcount = 20;
                chapter.MinSpeedRatio = 2;
                chapter.MaxSpeedRatio = 3;
                chapters.Add(chapter);

                chapter = new CommonGameChapter();
                chapter.totalcount = 30;
                chapter.MinSpeedRatio = 3;
                chapter.MaxSpeedRatio = 5;
                chapters.Add(chapter);

                bonousChapter = new List<GameChapter>();
                GameChapterFunShowCurve curveChapter = new GameChapterFunShowCurve();
                GameChapterILoveU loveuChapter = new GameChapterILoveU();
                bonousChapter.Add(loveuChapter);
                bonousChapter.Add(curveChapter);

                /*
                chapter = new GameChapter1();
                chapters.Add(chapter);
                chapter = new GameChapter2();
                chapters.Add(chapter);
                chapter = new GameChapter3();
                chapters.Add(chapter);
                chapter = new GameChapter4();
                chapters.Add(chapter);
                chapter = new GameChapter5();
                chapters.Add(chapter);

                //chapter = new GameChapterFunShowCurve();
                //chapters.Add(chapter);

                chapter = new GameChapter6();
                chapters.Add(chapter);
                chapter = new GameChapter7();
                chapters.Add(chapter);
                chapter = new GameChapter8();
                chapters.Add(chapter);
                chapter = new GameChapter9();
                chapters.Add(chapter);

                chapter = new GameChapter10();
                chapters.Add(chapter);
                chapter = new GameChapterForever();
                chapters.Add(chapter);

                bonousChapter = new List<GameChapter>();
                GameChapterFunShowCurve curveChapter = new GameChapterFunShowCurve();
                GameChapterILoveU loveuChapter = new GameChapterILoveU();
                bonousChapter.Add(loveuChapter);
                bonousChapter.Add(curveChapter);
                bonousChapter.Add(curveChapter);
                bonousChapter.Add(curveChapter);
                bonousChapter.Add(curveChapter);
                */
            }
            else
            {
                CommonGameChapter chapter = null;
                chapter = new CommonGameChapter();
                chapter.totalcount = 5000;
                chapter.MinSpeedRatio = 1;
                chapter.MaxSpeedRatio = 3;
                chapters.Add(chapter);
            }
            return true;
        }

        int bonousindex = 0;
        public bool GetBonusChapter(out GameChapter bounusChapter)
        {

            bounusChapter = bonousChapter[bonousindex];
            bonousindex += 1;
            bonousindex %= bonousChapter.Count;
            return true;
        }

        public bool GetNextChapter(out GameChapter chapter)
        {
            chapter = null;

            //while (true)
            {
                if (chapters.Count <= 0)
                {
                    return false;
                }
                chapter = chapters[0];
                //if (chapter.CanBeRemoved())
                {
                    chapters.RemoveAt(0);
                }
               // else
                {
                    return true;
                }
            }
        }
    }

    abstract class  GamePage
    {
        abstract public void InitGamePage(DuckHuntGame game);
        abstract public void GetObjects(out List<ModelObject> objlst);
        abstract public void Update(GameTime gametime);
        abstract public void Click(List<Vector2> clickpositionlist);
        abstract public void Press(List<Vector2> clickpositionlist);

        Object showLayer = null;
        protected void AddShowLayer()
        {
            showLayer = DuckHuntGameControler.controler.AddShowLayer();
        }

        public GamePage()
        {
            AddShowLayer();
        }

        public void Active()
        {
            DuckHuntGameControler.controler.SwitchShowLayer(showLayer);
        }
        public void AddObjectToShowLayer(ModelObject model)
        {
            DuckHuntGameControler.controler.AddObjectToShowLayer(showLayer, model);
        }

        public void RemoveObjectFromShowLayer(ModelObject modelobj)
        {
            DuckHuntGameControler.controler.RemoveObjectFromShowLayer(showLayer, modelobj);
        }

        public void ClearObjectFromShowLayer()
        {
            DuckHuntGameControler.controler.ClearObjectFromShowLayer(showLayer);
        }
    }

    class GamePlayPage : GamePage
    {
        GameChapterManager gameChapterMgr;


        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //


        TimeBoardModel leftTime;
        LostDuckBoardModel lostDuck;
        Rectangle leftTimeBoardSpace;


        //List<BulletModel> bulletsList;
        List<AnimalModel> duckList;
        Rectangle duckFlySpace;


        PandaModel panda;
        BaloonModel baloon;
        PlaneModel plane;
        //ParrotModel parrot;
        Rectangle baloonSpace;

        HitBoardModel hitBoard;
        Rectangle hitBoardSpace;

        ScroeBoardModel scoreBoard;
        Rectangle scoreBoardSpace;

        InfoBoardModel infoBoard;
        Rectangle infoBoardSpace;

        ButtonModel pause;
        Rectangle pauseButtonSpace;


        //FireworkModel firework;
        Rectangle fireworkSpace;

        LevelUpBoardModel levelUp;
        Rectangle levelUpSpace;

        //HunterModel hunter;
        List<BulletModel> arrowList;


        GameBackGroundPage backgroundPage = null;
        DuckHuntGame duckHuntGame = null;

        public GamePlayPage()
        {
            arrowList = new List<BulletModel>();
            gameChapterMgr = new GameChapterManager();
            duckList = new List<AnimalModel>();
            tmplist = new List<AnimalModel>();
        }

        public void ResetGame()
        {
            ClearObjectFromShowLayer();
            
            List<ModelObject> objlst = null;
            GetObjects(out objlst);
            foreach (var obj in objlst)
            {
                AddObjectToShowLayer(obj);
            }
            
            //NewDog();
            /*
            gameMode = GameMode.GAME_FREE_MODE;// gameMode1;
            gameChapterMgr.Init(gameMode);
            gameMode = GameMode.GAME_TIME_LIMIT;
             */
            leftTime.SetTime(30);
            lostDuck.ResetLostCount();
            
            scoreBoard.AddScore(-scoreBoard.TotalScore);


            levelUpInShowLayer = false;

            hitBoardInShowLayer = false;

            scoreBoardInShowLayer = false;

            leftTimeBoardInShowLayer = false;


            lostDuckBoardInShowLayer = false;

            pauseInShowLayer = false;

            gameChapterMgr.GetNextChapter(out chapter);

        }

        override public void InitGamePage(DuckHuntGame game)
        {
            duckHuntGame = game;
            backgroundPage = game.GetBackgroundPage();

            Rectangle rectBackground = duckHuntGame.GetGlobalViewRect();
            if (rectBackground.Width < rectBackground.Height)
            {
                duckFlySpace.Width = rectBackground.Height - 150;
                duckFlySpace.Height = rectBackground.Width;
            }
            else
            {
                duckFlySpace.Width = rectBackground.Width;
                duckFlySpace.Height = rectBackground.Height - 150;
            }
            /*
            if (rectBackground.Width < rectBackground.Height)
            {
                dogRunSpace.Width = rectBackground.Height;
                dogRunSpace.Y = rectBackground.Width - 200 - 180;
                dogRunSpace.Height = 150;
            }
            else
            {
                dogRunSpace.Width = rectBackground.Width;
                dogRunSpace.Y = rectBackground.Height - (150 + 140);
                dogRunSpace.Height = 150;
            }
            */
            baloonSpace.Width = rectBackground.Width;
            baloonSpace.Y = 100;
            baloonSpace.Height = 150;

            ScroeBoardModel scoreBoard1 = new ScroeBoardModel();
            scoreBoardSpace.X = rectBackground.Left + 20;
            scoreBoardSpace.Y = rectBackground.Top + 10;
            scoreBoardSpace.Width = scoreBoard1.GetSpace().Width;
            scoreBoardSpace.Height = scoreBoard1.GetSpace().Height;

            InfoBoardModel infoBoard1 = new InfoBoardModel();
            infoBoardSpace.X = rectBackground.Left + (rectBackground.Width - infoBoard1.GetSpace().Width)/2;
            infoBoardSpace.Y = rectBackground.Top + 30;
            infoBoardSpace.Width = infoBoard1.GetSpace().Width;
            infoBoardSpace.Height = infoBoard1.GetSpace().Height;

            TimeBoardModel timeBoard = new TimeBoardModel();
            if (rectBackground.Width < rectBackground.Height)
            {
                leftTimeBoardSpace.X = (int)
                    (rectBackground.Height - timeBoard.GetSpace().Width * timeBoard.GetSacle() - 20);
                leftTimeBoardSpace.Y = 20;
                leftTimeBoardSpace.Width = timeBoard.GetSpace().Width;
                leftTimeBoardSpace.Height = timeBoard.GetSpace().Height;
            }
            else
            {

                leftTimeBoardSpace.X =
                    (int)(rectBackground.Width - timeBoard.GetSpace().Width*timeBoard.GetSacle() - 20);
                leftTimeBoardSpace.Y = 20;
                leftTimeBoardSpace.Width = timeBoard.GetSpace().Width;
                leftTimeBoardSpace.Height = timeBoard.GetSpace().Height;
            }

            HitBoardModel hitBoard1 = new HitBoardModel();
            hitBoardSpace = leftTimeBoardSpace;
            hitBoardSpace.Y = 70;
            hitBoardSpace.Width = hitBoard1.GetSpace().Width;
            hitBoardSpace.Height = hitBoard1.GetSpace().Height;



            ButtonModel button = new ButtonModel();
            if (rectBackground.Width < rectBackground.Height)
            {
                pauseButtonSpace.X = rectBackground.Height - button.GetSpace().Width+130 ;
                pauseButtonSpace.Y = rectBackground.Right - 150; ;
                pauseButtonSpace.Width = button.GetSpace().Width;
                pauseButtonSpace.Height = button.GetSpace().Height;
            }
            else
            {

                pauseButtonSpace.X = rectBackground.Width - button.GetSpace().Width +130;
                pauseButtonSpace.Y = rectBackground.Bottom - 150;
                pauseButtonSpace.Width = button.GetSpace().Width;
                pauseButtonSpace.Height = button.GetSpace().Height;
            }

            FireworkModel firework1 = new FireworkModel();

            fireworkSpace.X = (rectBackground.Width - firework1.GetSpace().Width) / 2;
            fireworkSpace.Y = 20;
            fireworkSpace.Width = firework1.GetSpace().Width;
            fireworkSpace.Height = firework1.GetSpace().Height;

            LevelUpBoardModel levelUp1 = new LevelUpBoardModel();
            levelUpSpace.X = rectBackground.Width / 2 - levelUp1.GetSpace().Width / 2;
            levelUpSpace.Y = rectBackground.Height / 2 - levelUp1.GetSpace().Height / 2 - 100;
            levelUpSpace.Width = levelUp1.GetSpace().Width;
            levelUpSpace.Height = levelUp1.GetSpace().Height;



            NewScoreBoard();
            NewAssistBoard();

           // firework = new FireworkModel();
           // firework.Initialize(null, fireworkSpace, 0);

            List<ModelObject> objlst = null;
            GetObjects(out objlst);
            foreach (var obj in objlst)
            {
                AddObjectToShowLayer(obj);
            }

        }


        override public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = new List<ModelObject>();
            objlst.Add(pause);
            if (levelUp.Show())
            {
                objlst.Add(levelUp);
            }

            if (baloon != null)
            {
                objlst.Add(baloon);
            }
            if (plane != null)
            {
                objlst.Add(plane);
            }
            foreach (AnimalModel duck in duckList)
            {
                objlst.Add(duck);
            }

            objlst.Add(hitBoard);
            objlst.Add(scoreBoard);

            if (this.gameMode == GameMode.GAME_TIME_LIMIT)
            {
                objlst.Add(leftTime);
            }
            else
            {
                objlst.Add(lostDuck);
            }

            //objlst.Add(hunter);
            foreach (var arrow in arrowList)
            {
                objlst.Add(arrow);
            }

            List<ModelObject> backgroundobjlst;
            backgroundPage.GetObjects(out backgroundobjlst);
            foreach (ModelObject obj in backgroundobjlst)
            {
                objlst.Add(obj);
            }
        }


        bool levelUpInShowLayer = false;
        bool hitBoardInShowLayer = false;
        bool scoreBoardInShowLayer = false;
        bool leftTimeBoardInShowLayer = false;
        bool lostDuckBoardInShowLayer = false;
        bool pauseInShowLayer = false;

        GameChapter chapter;

        void updateShootProgress()
        {
            //hunter.Update(gametime);
            // check if the arrow shoot the duck
            foreach (var arrow in arrowList)
            {
                foreach (AnimalModel duck in duckList)
                {
                    arrow.Shoot(duck);
                }
                arrow.Shoot(baloon);
            }

        }

        bool addBalloon = false;
        Vector2 bonousDuckStartPos = Vector2.Zero;
        void updateShootResult()
        {
            foreach (var arrow in arrowList)
            {
                List<AnimalModel> lastshotducklist = arrow.RetrieveShootDucks();
                if (lastshotducklist != null)
                {
                    //
                    float score = 100;
                    int shootduckcount = 0;
                    foreach (AnimalModel animal in lastshotducklist)
                    {
                        if (animal.Type() == ModelType.DUCK)
                        {
                            shootduckcount++;
                            score = 100;
                            score *= shootduckcount;
                            score /= animal.GetSacle();
                            scoreBoard.AddScore((int)score);
                            hitBoard.AddHitCount(1);
                        }
                        else if (animal.Type() == ModelType.PARROT)
                        {
                            // shot a parrot
                            scoreBoard.AddScore(-500 - showbaloonCount * 10);
                        }
                    }

                    if (shootduckcount > 1)
                    {
                        addBalloon = true;
                        //ShowBalloon();
                    }

                    //ShowEastEgg();
                }

                if (arrow.RetrieveBaloon() != null)
                {
                    // show award
                    //AddBonusDuck(arrow.GetAbsolutePosition());
                    bonousDuckStartPos = arrow.GetShootPoint();
                }
            }
        }

        DateTime lastrelaseducktime = DateTime.Now;
        void checkIfneed2Releaseducks()
        {
            // change the logic
            // 1. every level, they will about N ducks, M parrots, they will released
            //  with a time span, batch by batch
            // 2. When no more birds left, the leve is finished
            // 3. check if meet the level result ( accuracy)
            //
            //
            if (addBalloon)
            {
                addBalloon = false;
                ShowBalloon();
            }
            if (bonousDuckStartPos != Vector2.Zero)
            {
                AddBonusDuck(bonousDuckStartPos);
                bonousDuckStartPos = Vector2.Zero;
            }
            /*
            TimeSpan timegone = DateTime.Now - lastrelaseducktime;
            if (timegone.TotalSeconds < 2)
            {
                return;
            }
             */
            lastrelaseducktime = DateTime.Now;

            // check if all left items are not duck, if so , next chapter
            //if (duckList.Count == 0)
            {
                NewDuck();
                /*
                if (duckList.Count > 0)
                {
                    phase = GAME_PHASE.DUCK_FLY;
                }
                else
                {
                    phase = GAME_PHASE.OVER;
                    // save new score

                    int score = scoreBoard.TotalScore;
                    int level = scoreBoard.GetLevel();

                    duckHuntGame.GotoGameOverPage(score, level);
                }
                 */
            }

        }

        void updateThisBatchOfDucks(GameTime gametime)
        {
            if (baloon != null)
            {
                baloon.Update(gametime);
                if (baloon.Gone)
                {
                    RemoveObjectFromShowLayer(baloon);
                    baloon = null;
                }

            }

            if (plane != null)
            {
                plane.Update(gametime);
                if (plane.Gone)
                {
                    RemoveObjectFromShowLayer(plane);
                    plane = null;
                }
            }

            if (duckList == null)
            {
                return;
            }
            int deadcount = 0;
            List<AnimalModel> goneducklist = new List<AnimalModel>();
            foreach (AnimalModel duck in duckList)
            {
                duck.Update(gametime);
                if (duck.Type() == ModelType.DUCK && !duck.Gone())
                {
                }
                if (duck.Dead())
                {
                    deadcount++;
                }
                if (duck.Gone())
                {
                    // new duck will clear the list
                    goneducklist.Add(duck);
                }
            }
            foreach (var duck in goneducklist)
            {
                RemoveObjectFromShowLayer(duck);
                duckList.Remove(duck);
                duck.Cleanup();
            }
            goneducklist.Clear();

        }

        void updateWeaponStatus(GameTime gametime)
        {
            /*
            foreach (var arrow in arrowList)
            {
                arrow.Update(gametime);
            }
            */

            for (int i = 0; i < arrowList.Count; )
            {
                if (arrowList[i].Gone)
                {
                    // remove it from this one 
                    RemoveObjectFromShowLayer(arrowList[i]);
                    arrowList.RemoveAt(i);
                }
                else
                {
                    arrowList[i].Update(gametime);
                    i++;
                }
            }
        }

        void updateGameProgress(GameTime gametime, out bool finishedThisLevel)
        {
            finishedThisLevel = false;
            if (gameMode != GameMode.GAME_TIME_LIMIT)
            {
                if ((chapter.CanBeRemoved() && duckList.Count == 0))
                {
                    foreach (var duck in duckList)
                    {
                        duck.Cleanup();
                        RemoveObjectFromShowLayer(duck);
                    }
                    duckList.Clear();

                    int score = scoreBoard.TotalScore;
                    int level = scoreBoard.GetLevel();

                    duckHuntGame.GotoLevelConclusionPage(score, level);

                    finishedThisLevel = true;
                    return;
                }
            }
            else
            {
                if (leftTime.LeftTime <= 0 || (chapter.CanBeRemoved() && duckList.Count == 0))
                {
                    foreach (var duck in duckList)
                    {
                        duck.Cleanup();
                        RemoveObjectFromShowLayer(duck);
                    }
                    duckList.Clear();

                    // save new score

                    int score = scoreBoard.TotalScore;
                    int level = scoreBoard.GetLevel();
                    //duckHuntGame.SaveNewScore(score, level);

                    duckHuntGame.GotoLevelConclusionPage(score, level);

                    finishedThisLevel = true;

                    return;
                }
            }

        }


        void showInfoBoards()
        {
            if (!hitBoardInShowLayer)
            {
                AddObjectToShowLayer(hitBoard);
                hitBoardInShowLayer = true;
            }
            if (!scoreBoardInShowLayer)
            {
                AddObjectToShowLayer(scoreBoard);
                scoreBoardInShowLayer = true;
            }

            if (!pauseInShowLayer)
            {
                AddObjectToShowLayer(pause);
                pauseInShowLayer = true;
            }

            if (this.gameMode == GameMode.GAME_TIME_LIMIT)
            {
                if (!leftTimeBoardInShowLayer)
                {
                    AddObjectToShowLayer(leftTime);
                    leftTimeBoardInShowLayer = true;
                }
            }
            else
            {
                if (!lostDuckBoardInShowLayer)
                {
                    AddObjectToShowLayer(lostDuck);
                    lostDuckBoardInShowLayer = true;
                }
            }

        }
        void hideInfoBoards()
        {
            if (levelUpInShowLayer)
            {
                RemoveObjectFromShowLayer(levelUp);
                levelUpInShowLayer = false;
            }

            if (hitBoardInShowLayer)
            {
                RemoveObjectFromShowLayer(hitBoard);
                hitBoardInShowLayer = false;
            }
            if (scoreBoardInShowLayer)
            {
                RemoveObjectFromShowLayer(scoreBoard);
                scoreBoardInShowLayer = false;
            }

            if (leftTimeBoardInShowLayer)
            {
                RemoveObjectFromShowLayer(leftTime);
                leftTimeBoardInShowLayer = false;
            }

            if (lostDuckBoardInShowLayer)
            {
                RemoveObjectFromShowLayer(lostDuck);
                lostDuckBoardInShowLayer = false;
            }
            if (pauseInShowLayer)
            {
                RemoveObjectFromShowLayer(pause);
                pauseInShowLayer = false;
            }
               

        }

        void updateInforBoards(GameTime gametime)
        {
            /*
            if (levelUp.Show())
            {
                if (!levelUpInShowLayer)
                {
                    AddObjectToShowLayer(levelUp);
                    levelUpInShowLayer = true;
                }
            }
            else
            {
                if (levelUpInShowLayer)
                {
                    RemoveObjectFromShowLayer(levelUp);
                    levelUpInShowLayer = false;
                }
            }
            levelUp.Update(gametime);
             */

            showInfoBoards();
            if (gameMode == GameMode.GAME_TIME_LIMIT)
            {
                leftTime.Update(gametime);
            }

        }

        override public void Update(GameTime gametime)
        {
            if (duckHuntGame.Pause)
            {
                return;
            }
            //
            backgroundPage.Update(gametime);
            //panda.Update(gametime);
            //firework.Update(gametime);

            //
            // update weapons
            //
            updateShootProgress();
            updateShootResult();

            updateWeaponStatus(gametime);

            //
            // update objects
            //
            updateThisBatchOfDucks(gametime);


            //
            // update information board
            //
            updateInforBoards(gametime);

            //
            // check if need to release any ducks
            //
            checkIfneed2Releaseducks();

            //
            // update game progress
            //
            bool finishedThisLevel = false;
            updateGameProgress(gametime, out finishedThisLevel);
            if (finishedThisLevel)
            {
                return ;
            }

        }

        int previousTotalScore = 0;
        int previousHitCount = 0;
        int showbaloonCount = 0;
        int showplanescore = 1000;
        int showplanecount = 0;

        DateTime lastbaloon = new DateTime(0);
        void ShowBalloon()
        {
            if (baloon == null)
            {
                DateTime now = System.DateTime.Now;
                TimeSpan elaspedtime = now - lastbaloon;
                if (elaspedtime.TotalSeconds > 30)
                {
                    lastbaloon = now;
                    baloon = new BaloonModel();
                    baloon.Initialize(null, baloonSpace, 0);
                    AddObjectToShowLayer(baloon);
                }
            }
        }

        void ShowBomb()
        {
            // show plane
            /*
            if (parrot == null)
            {
                parrot = new ParrotModel();

                parrot.Initialize(null, this.duckFlySpace, 0);
                parrot.StartPilot();
            }
             */
        }

        void ShowPlane()
        {
            if (plane == null)
            {
                plane = new PlaneModel();
                DateTime now = DateTime.Now;
                plane.Initialize(null, this.baloonSpace, now.Millisecond);
                AddObjectToShowLayer(plane);

            }
        }

        void ShowEastEgg()
        {
            // 
            // show bounous

            if (scoreBoard.TotalScore > showplanescore)
            {

                ShowPlane();
                showplanescore += 1000 + 50 * showplanecount;
                showplanecount++;
            }

            if (hitBoard.HitCount - previousHitCount > 20 + showbaloonCount * 2)
            {
                // show baloon
                previousHitCount = hitBoard.HitCount;
                ShowBalloon();
            }

        }

        override public void Press(List<Vector2> clickpositionlist)
        {
            // new a bullet
            foreach (Vector2 clickpos in clickpositionlist)
            {
                //
                //hunter.SetTargetPos(clickpos);
                return;
            }
        }


        override public void Click(List<Vector2> clickpositionlist)
        {
#if false
            if (phase != GAME_PHASE.DUCK_FLY)
            {
                return;
            }

            if (bulletsList.Count > 0)
            {
                return;
            }

            // new a bullet
            foreach (Vector2 clickpos in clickpositionlist)
            {
                if (pause.Click(clickpos))
                {
                    // reset the icon
                    pause.Click(clickpos);
                    duckHuntGame.Pause = !duckHuntGame.Pause;
                    if (duckHuntGame.Pause)
                    {
                        duckHuntGame.PauseGame(true);
                    }
                    continue;
                }
                if (duckHuntGame.Pause)
                {
                    continue;
                }


                ArrowModel arrow = null;// hunter.Shoot();
                if (arrow != null)
                {
                    arrowList.Add(arrow);
                    AddObjectToShowLayer(arrow);
                }
                return;
            }

#endif
            ////
            /*
            if (bulletsList.Count > 0)
            {
                return;
            }
            */
            // new a bullet
            foreach (Vector2 clickpos in clickpositionlist)
            {
                if (pause.Click(clickpos))
                {
                    // reset the icon
                    pause.Click(clickpos);
                    duckHuntGame.Pause = !duckHuntGame.Pause;
                    if (duckHuntGame.Pause)
                    {
                        duckHuntGame.PauseGame(true);
                    }
                    continue;
                }
                if (duckHuntGame.Pause)
                {
                    continue;
                }

                BulletModel bullet = new BulletModel(clickpos);

                //bullet.AdjustForFlyEffect();
                arrowList.Add(bullet);

            }

        }



        /////////////////////////////////////////////////////


        void NewScoreBoard()
        {
            scoreBoard = new ScroeBoardModel();
            scoreBoard.Initialize(null, scoreBoardSpace, 0);

            pause = new ButtonModel();
            pause.Initialize(null, pauseButtonSpace, 0);

            levelUp = new LevelUpBoardModel();
            levelUp.Initialize(null, levelUpSpace, 0);

            infoBoard = new InfoBoardModel();
            infoBoard.Initialize(null, infoBoardSpace, 0);

            infoBoard.AddStr("Duck hunt is a simple game.");
            infoBoard.AddStr("Just shoot(click) the flying duck.");
            infoBoard.AddStr("If you shoot more than one ducks in one click.");
            infoBoard.AddStr("You will get additional score.");
            infoBoard.AddStr("Please do not shoot parrot.");
            infoBoard.AddStr("If parrot is shoot, score will be dcreased.");
            infoBoard.AddStr("If you shoot a balloon.");
            infoBoard.AddStr("You will get additional ducks as bonous.");
            infoBoard.AddStr("Good Luck!");

        }

        void NewAssistBoard()
        {
            hitBoard = new HitBoardModel();
            hitBoard.Initialize(null, hitBoardSpace, 0);

            leftTime = new TimeBoardModel();
            leftTime.Initialize(null, leftTimeBoardSpace, 0);

            lostDuck = new LostDuckBoardModel();
            lostDuck.Initialize(null, leftTimeBoardSpace, 0);

        }

        void ShowLevelUp()
        {
            levelUp.Reset();
            scoreBoard.IncreaseLevel();
        }

        int flycount = 0;

        GameMode gameMode = GameMode.GAME_TIME_LIMIT;
        List<AnimalModel> tmplist = null;
        void NewDuck()
        {
            if (duckList == null)
            {
                duckList = new List<AnimalModel>();
                tmplist = new List<AnimalModel>();
            }
            else
            {
                // set duck state
                int ii = 0;
                /*
                if (gameMode != GameMode.GAME_TIME_LIMIT)
                {
                    foreach (AnimalModel duck2 in duckList)
                    {
                        if (!duck2.Dead() && duck2.Type() == ModelType.DUCK)
                        {
                            //flycount++;
                            lostDuck.AddDuck(1);
                        }
                        if (!duck2.Gone() && duck2.Type() == ModelType.PARROT)
                        {
                            tmplist.Add(duck2);
                        }

                        if (duck2.Gone())
                        {
                            // animal gone, reutrn pilot
                            duck2.Cleanup();
                            RemoveObjectFromShowLayer(duck2);
                        }
                    }
                    if (lostDuck.LostDuckCount >= 3)
                    {

                        foreach (AnimalModel duck2 in tmplist)
                        {
                            duck2.Cleanup();
                            RemoveObjectFromShowLayer(duck2);
                        }
                        tmplist.Clear();
                        duckList.Clear();

                        return;
                    }
                }
                else
                {
                    foreach (AnimalModel duck2 in duckList)
                    {
                        if (!duck2.Gone() && duck2.Type() == ModelType.PARROT)
                        {
                            tmplist.Add(duck2);
                        }
                        else
                        {
                            duck2.Cleanup();
                            RemoveObjectFromShowLayer(duck2);
                        }
                    }

                    if (leftTime.LeftTime <= 0)
                    {
                        foreach (AnimalModel duck2 in tmplist)
                        {
                            duck2.Cleanup();
                            RemoveObjectFromShowLayer(duck2);
                        }
                        tmplist.Clear();
                        duckList.Clear();
                        return;
                    }
                }
                duckList.Clear();
                 */
            }
            
            List<AnimalModel> ducks = null;

            if (chapter != null && chapter.GetDuckList(out ducks) && ducks != null && ducks.Count > 0)
            {
               
            }

            if (ducks == null)
            {
                return;
            }


            int batchcount = chapter.GetDuckBatch();
            do
            {

                int i = 0;
                DateTime now = System.DateTime.Now;
                foreach (AnimalModel duck in ducks)
                {
                    int s = now.Hour * 60 * 60 + now.Minute * 60 + now.Second;
                    duck.Initialize(null, duckFlySpace, s + (i++) * 7);
                    duck.StartPilot();
                    duckList.Add(duck);
                    AddObjectToShowLayer(duck);
                }
                batchcount--;
                if (batchcount > 0)
                {
                    chapter.GetDuckList(out ducks);
                    if (ducks == null || ducks.Count == 0)
                    {
                        break;
                    }
                }
            } while (batchcount > 0);

            duckList.AddRange(tmplist);
            tmplist.Clear();
        }


        void AddBonusDuck(Vector2 startPos)
        {
            if (duckList == null)
            {
                return ;
            }

            if (gameMode != GameMode.GAME_TIME_LIMIT)
            {
                if (lostDuck.LostDuckCount >= 3)
                {
                    return;
                }
            }
            else
            {
                if (leftTime.LeftTime <= 0)
                {
                    return;
                }
            }

            List<AnimalModel> ducks = null;
            GameChapter bonousChapter = null;
            gameChapterMgr.GetBonusChapter(out bonousChapter);

            int batchcount = bonousChapter.GetDuckBatch();

            // add I love u
            int i = 0;

            Vector2 endpos = Vector2.Zero;
            endpos.Y = duckFlySpace.Height *1.0f/ 2;
            endpos.X = duckFlySpace.Width / batchcount/3;

            int showTime = 15;
            //if (batchcount == 3)
            {
                // i love you
                showTime = 50;
            }
            for (int ii = 0; ii < batchcount; ii++)
            {
                bonousChapter.GetDuckList(out ducks);
                if (ducks == null)
                {
                    return;
                }

                DateTime now = System.DateTime.Now;
                foreach (AnimalModel duck in ducks)
                {
                    int s = now.Hour * 60 * 60 + now.Minute * 60 + now.Second;
                    duck.Initialize(null, duckFlySpace, s + (i++) * 73);
                    duck.StartPilot();

                    duck.SetEndPos(endpos);
                    duck.SetShowTime(showTime);
                    duckList.Add(duck);
                    AddObjectToShowLayer(duck);

                }
                endpos.X += duckFlySpace.Width / batchcount;
                if (ii == 1)
                {
                    endpos.Y = duckFlySpace.Height * 1.0f / 3;
                }
            }
        }

        public void NewGame(GameMode gameMode1)
        {
            // reset dog, duck and so on
           // NewDog();
            if (duckList != null)
            {
                duckList.Clear();
            }

            gameMode = duckHuntGame.gameMode;
            gameChapterMgr.Init(gameMode);
            leftTime.SetTime(30);
            lostDuck.ResetLostCount();

            //gameChapterMgr.GetNextChapter(out chapter);
        }
    }

    class GameMainMenuPage : GamePage
    {
        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //
        GameChapterManager gameChapterMgr;
        GameChapter chapter = null;

        List<AnimalModel> duckList;
        Rectangle duckFlySpace;

        MenuItemModel menuTimeModelItem;
        MenuItemModel menuFreeModelItem;
        MenuItemModel menuOptionItem;
        MenuItemModel menuScoreListItem;

        Rectangle timeModelMenuSpace;
        Rectangle freeModelModelMenuSpace;
        Rectangle optionMenuSpace;
        Rectangle scoreListMenuSpace;

        GameBackGroundPage backgroundPage = null;
        DuckHuntGame duckHuntGame = null;
        override public void InitGamePage(DuckHuntGame game)
        {
            duckHuntGame = game;
            backgroundPage = game.GetBackgroundPage();

            pause = backgroundPage.GetPauseButton();

            Rectangle rectBackground = game.GetGlobalViewRect();
            if (rectBackground.Width < rectBackground.Height)
            {
                duckFlySpace.Width = rectBackground.Height - 150;
                duckFlySpace.Height = rectBackground.Width;
            }
            else
            {
                duckFlySpace.Width = rectBackground.Width;
                duckFlySpace.Height = rectBackground.Height - 150;
            }


            MenuItemModel menuItem = new MenuItemModel();
            timeModelMenuSpace.X = 700;
            timeModelMenuSpace.Y = 200;
            timeModelMenuSpace.Width = menuItem.GetSpace().Width;
            timeModelMenuSpace.Height = menuItem.GetSpace().Height;

            freeModelModelMenuSpace = timeModelMenuSpace;
            freeModelModelMenuSpace.X = 400;
            freeModelModelMenuSpace.Y = 310;

            optionMenuSpace = freeModelModelMenuSpace;
            optionMenuSpace.X = 1200;
            optionMenuSpace.Y = 300;

            scoreListMenuSpace = optionMenuSpace;
            scoreListMenuSpace.X = rectBackground.Width / 2 - 150;
            scoreListMenuSpace.Y = rectBackground.Top + 400;


            gameChapterMgr = new GameChapterManager();
            gameChapterMgr.Init(GameMode.GAME_DEMON);
            gameChapterMgr.GetNextChapter(out chapter);

            NewMenu();

            List<ModelObject> objlst = null;
            GetObjects(out objlst);
            foreach (var obj in objlst)
            {
                AddObjectToShowLayer(obj);
            }
        }

        override public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = new List<ModelObject>();

            objlst.Add(menuTimeModelItem);
            objlst.Add(menuFreeModelItem);
            objlst.Add(menuOptionItem);
            objlst.Add(menuScoreListItem);

            List<ModelObject> backgroundobjlst;
            backgroundPage.GetObjects(out backgroundobjlst);
            foreach (ModelObject obj in backgroundobjlst)
            {
                objlst.Add(obj);
            }

        }
        override public void Update(GameTime gametime)
        {
            backgroundPage.Update(gametime);
            menuTimeModelItem.Update(gametime);
            menuFreeModelItem.Update(gametime);
            menuOptionItem.Update(gametime);
            menuScoreListItem.Update(gametime);

            updateThisBatchOfDucks(gametime);

            checkIfneed2Releaseducks();

        }

        List<AnimalModel> tmplist = null;
        void NewDuck()
        {
            if (duckList == null)
            {
                duckList = new List<AnimalModel>();
                tmplist = new List<AnimalModel>();
            }

            List<AnimalModel> ducks = null;

            if (chapter != null && chapter.GetDuckList(out ducks) && ducks != null && ducks.Count > 0)
            {

            }

            if (ducks == null)
            {
                return;
            }


            int batchcount = chapter.GetDuckBatch();
            do
            {

                int i = 0;
                DateTime now = System.DateTime.Now;
                foreach (AnimalModel duck in ducks)
                {
                    int s = now.Hour * 60 * 60 + now.Minute * 60 + now.Second;
                    duck.Initialize(null, duckFlySpace, s + (i++) * 7);
                    duck.StartPilot();
                    duck.EnableSound = false;
                    duckList.Add(duck);
                    AddObjectToShowLayer(duck);
                }
                batchcount--;
                if (batchcount > 0)
                {
                    chapter.GetDuckList(out ducks);
                    if (ducks == null || ducks.Count == 0)
                    {
                        break;
                    }
                }
            } while (batchcount > 0);

            duckList.AddRange(tmplist);
            tmplist.Clear();
        }

        DateTime lastrelaseducktime = DateTime.Now;
        void checkIfneed2Releaseducks()
        {
            lastrelaseducktime = DateTime.Now;

            NewDuck();

        }


        void updateThisBatchOfDucks(GameTime gametime)
        {
            if (duckList == null)
            {
                return;
            }
            int deadcount = 0;
            List<AnimalModel> goneducklist = new List<AnimalModel>();
            foreach (AnimalModel duck in duckList)
            {
                duck.Update(gametime);
                if (duck.Type() == ModelType.DUCK && !duck.Gone())
                {
                }
                if (duck.Dead())
                {
                    deadcount++;
                }
                if (duck.Gone())
                {
                    // new duck will clear the list
                    goneducklist.Add(duck);
                }
            }
            foreach (var duck in goneducklist)
            {
                RemoveObjectFromShowLayer(duck);
                duckList.Remove(duck);
                duck.Cleanup();
            }
            goneducklist.Clear();
        }


        override public void Press(List<Vector2> clickpositionlist)
        {

        }

        override public void Click(List<Vector2> clickpositionlist)
        {
            //
           // backgroundPage.Click(clickpositionlist);
            if (showPause)
            {
                if (pause.Click(clickpositionlist[0]))
                {
                    // reset the icon
                    pause.Click(clickpositionlist[0]);

                    duckHuntGame.PauseGame(false);
                }
            }


            foreach (Vector2 clickpos in clickpositionlist)
            {

                if (menuTimeModelItem.Hit(clickpos))
                {

                    //duckHuntGame.StartGame
                    duckHuntGame.gameMode = GameMode.GAME_TIME_LIMIT;

                    duckHuntGame.NewGame();

                    return;
                }

                if (menuFreeModelItem.Hit(clickpos))
                {
                    // free model

                    duckHuntGame.gameMode = GameMode.GAME_FREE_MODE;

                    duckHuntGame.NewGame();

                    return;
                }


                if (menuOptionItem.Hit(clickpos))
                {
                    // duckHuntGame.ShowkeyBoard();
                    duckHuntGame.GotoConfigPage();

                    return;
                    // free model
#if WINDOWS_PHONE
                    /*
                    if (!Guide.IsVisible)
                        //弹出软键盘输入框
                        Guide.BeginShowKeyboardInput(PlayerIndex.One, "test", "test description",
                            sipResult, keyboardCallback, new object());
                    */
#else
                    //Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.System) + Path.DirectorySeparatorChar + "osk.exe");
                    /*
                    DuckHunt.App app = (DuckHunt.App)DuckHunt.App.Current;

                    if (app.playerPage == null)
                    {
                        app.playerPage = new DuckHunt.PlayerInputPage();
                    }
                    app.playerPage.PreviousPage = Window.Current.Content;
                    app.playerPage.Focus(FocusState.Keyboard);
                    Window.Current.Content = app.playerPage;
                    Window.Current.Activate();
                    app.playerPage.Focus(FocusState.Keyboard);
                     */
#endif

                    return;
                }

                if (menuScoreListItem.Hit(clickpos))
                {
                    // show score list
                    duckHuntGame.GotoScoreListPage();
                }


            }
            return;
        }

        /////////

        void NewMenu()
        {
            menuTimeModelItem = new MenuItemModel();
            menuTimeModelItem.Initialize(null, timeModelMenuSpace, 0);
            menuTimeModelItem.Conent = "Time Model";

            menuFreeModelItem = new MenuItemModel();
            menuFreeModelItem.Initialize(null, freeModelModelMenuSpace, 0);
            menuFreeModelItem.Conent = "Free Model";

            menuOptionItem = new MenuItemModel();
            menuOptionItem.Initialize(null, optionMenuSpace, 0);
            menuOptionItem.Conent = "Option";

            menuScoreListItem = new MenuItemModel();
            menuScoreListItem.Initialize(null, scoreListMenuSpace, 0);
            menuScoreListItem.Conent = "Score List";
        }

        ButtonModel pause = null;
        bool showPause = false;
        public void ShowPause(bool show)
        {
            showPause = show;
            if (showPause)
            {
                AddObjectToShowLayer(pause);
                pause.Checked = false;
            }
            else
            {
                pause.Checked = true;
                RemoveObjectFromShowLayer(pause);
            }
        }

    }



    class GameOverPage : GamePage
    {
        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //

        TitleItemModel gameOverTitleItem;
        ResultSummaryModel resultSummary;

        MenuItemModel menuRetryItem;
        MenuItemModel menuReturnItem;
        MenuItemModel menuExitItem;
        MenuItemModel menuScoreListItem;


        Rectangle gameOverTitleSpace;
        Rectangle resultSummarySpace;
        Rectangle retryMenuSpace;
        Rectangle returnMenuSpace;
        Rectangle exitMenuSpace;
        Rectangle scoreListMenuSpace;

        GameBackGroundPage backgroundPage = null;
        DuckHuntGame duckHuntGame = null;
        override public void InitGamePage(DuckHuntGame game)
        {
            duckHuntGame = game;
            backgroundPage = game.GetBackgroundPage();

            Rectangle rectBackground = game.GetGlobalViewRect();


            TitleItemModel titleItem = new TitleItemModel();
            gameOverTitleSpace.X = (rectBackground.Width - titleItem.GetSpace().Width) / 2;
            gameOverTitleSpace.Y = 30;
            gameOverTitleSpace.Width = titleItem.GetSpace().Width;
            gameOverTitleSpace.Height = titleItem.GetSpace().Height;

            ResultSummaryModel resultSummaryItm = new ResultSummaryModel();
            resultSummarySpace.X = (rectBackground.Width - resultSummaryItm.GetSpace().Width) / 2;
            resultSummarySpace.Y = 150;

            MenuItemModel menuItem = new MenuItemModel();
            retryMenuSpace.X = 700;
            retryMenuSpace.Y = 300;
            retryMenuSpace.Width = menuItem.GetSpace().Width;
            retryMenuSpace.Height = menuItem.GetSpace().Height;

            returnMenuSpace = retryMenuSpace;
            returnMenuSpace.X = 400;
            returnMenuSpace.Y = 310;

            exitMenuSpace = returnMenuSpace;
            exitMenuSpace.X = 1200;
            exitMenuSpace.Y = 300;

            scoreListMenuSpace = exitMenuSpace;
            scoreListMenuSpace.X = 1200;
            scoreListMenuSpace.Y = rectBackground.Top + 300;


            NewMenu();

            List<ModelObject> objlst = null;
            GetObjects(out objlst);
            foreach (var obj in objlst)
            {
                AddObjectToShowLayer(obj);
            }

        }

        override public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = new List<ModelObject>();

            objlst.Add(gameOverTitleItem);
            objlst.Add(resultSummary);
            objlst.Add(menuRetryItem);
            objlst.Add(menuReturnItem);
            //objlst.Add(menuExitItem);
            objlst.Add(menuScoreListItem);

            List<ModelObject> backgroundobjlst;
            backgroundPage.GetObjects(out backgroundobjlst);
            foreach (ModelObject obj in backgroundobjlst)
            {
                objlst.Add(obj);
            }

        }
        override public void Update(GameTime gametime)
        {
            backgroundPage.Update(gametime);
            gameOverTitleItem.Update(gametime);
            menuRetryItem.Update(gametime);
            menuReturnItem.Update(gametime);
            menuExitItem.Update(gametime);
            menuScoreListItem.Update(gametime);

        }


        override public void Press(List<Vector2> clickpositionlist)
        {

        }
        override public void Click(List<Vector2> clickpositionlist)
        {
            //
            backgroundPage.Click(clickpositionlist);

            foreach (Vector2 clickpos in clickpositionlist)
            {

                if (menuRetryItem.Hit(clickpos))
                {
                    duckHuntGame.NewGame();
                    return;
                }

                if (menuReturnItem.Hit(clickpos))
                {
                    duckHuntGame.GotoMainMenuPage();

                    return;
                }

                /*
                if (menuExitItem.Hit(clickpos))
                {
                    // exit
                    //System.
                    return;
                }
                */

                if (menuScoreListItem.Hit(clickpos))
                {
                    // show score list
                    duckHuntGame.GotoScoreListPage();
                }




            }
            return;
        }

        /////////

        void NewMenu()
        {
            gameOverTitleItem = new TitleItemModel();
            gameOverTitleItem.Initialize(null, gameOverTitleSpace, 0);
            gameOverTitleItem.Conent = "Game Over";

            resultSummary = new ResultSummaryModel();
            resultSummary.Initialize(null, resultSummarySpace, 0);

            this.menuRetryItem = new MenuItemModel();
            menuRetryItem.Initialize(null, retryMenuSpace, 0);
            menuRetryItem.Conent = "Retry";

            menuReturnItem = new MenuItemModel();
            menuReturnItem.Initialize(null, returnMenuSpace, 0);
            menuReturnItem.Conent = "Return";

            menuExitItem = new MenuItemModel();
            menuExitItem.Initialize(null, exitMenuSpace, 0);
            menuExitItem.Conent = "Exit";

            menuScoreListItem = new MenuItemModel();
            menuScoreListItem.Initialize(null, scoreListMenuSpace, 0);
            menuScoreListItem.Conent = "Score List";
        }

        public void SetGameRecord(int highestScore, int highestLevel)
        {
            resultSummary.HighestScore = highestScore;
            resultSummary.HighestLevel = highestLevel;
        }

        public void SetGameResult(int yourScore, int yourLevel)
        {
            resultSummary.YourLevel = yourLevel;
            resultSummary.YourScore = yourScore;
        }
    }



    class LevelGoalDescriptionPage : GamePage
    {
        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //

        //TitleItemModel gameOverTitleItem;
        DogModel dog;
        Rectangle dogRunSpace;

        DuckModel blackDuck = null;
        TitleItemModel blackDuckScore = null;

        DuckModel blueDuck = null;
        TitleItemModel blueDuckScore = null;

        ParrotModel parrotDuck = null;
        TitleItemModel parrotDuckScore = null;


        //Rectangle gameOverTitleSpace;

        GameBackGroundPage backgroundPage = null;
        DuckHuntGame duckHuntGame = null;
        override public void InitGamePage(DuckHuntGame game)
        {
            duckHuntGame = game;
            backgroundPage = game.GetBackgroundPage();

            Rectangle rectBackground = game.GetGlobalViewRect();
            if (rectBackground.Width < rectBackground.Height)
            {
                dogRunSpace.Width = rectBackground.Height;
                dogRunSpace.Y = rectBackground.Width - 200 - 180;
                dogRunSpace.Height = 150;
            }
            else
            {
                dogRunSpace.Width = rectBackground.Width;
                dogRunSpace.Y = rectBackground.Height - (150 + 140);
                dogRunSpace.Height = 150;
            }
            NewDog();

            Rectangle scoreInfoRc = new Rectangle();
            Vector2 duckpos = new Vector2();
            duckpos.X = rectBackground.Width/3;
            duckpos.Y = rectBackground.Height/5;

            blackDuck = new DuckModel();
            blackDuck.Initialize(null, rectBackground, 0);
            blackDuck.ShowTime = 100;
            blackDuck.SetStartPos(duckpos);
            AddObjectToShowLayer(blackDuck);

            scoreInfoRc.X = (int)(duckpos.X + 100);
            scoreInfoRc.Y = (int)duckpos.Y - 10;
            blackDuckScore = new TitleItemModel();
            scoreInfoRc.Width = blackDuckScore.GetSpace().Width;
            scoreInfoRc.Height = blackDuckScore.GetSpace().Height;
            blackDuckScore.Initialize(null, scoreInfoRc, 0);
            blackDuckScore.Conent = "= X 100";
            AddObjectToShowLayer(blackDuckScore);

            duckpos.Y += 120;
            blueDuck = new DuckModel();
            blueDuck.Initialize(null, rectBackground, 0);
            blueDuck.ShowTime = 100;
            blueDuck.DuckStyle = 1;
            blueDuck.SetStartPos(duckpos);
            AddObjectToShowLayer(blueDuck);

            scoreInfoRc.Y = (int)duckpos.Y-10;
            blueDuckScore = new TitleItemModel();
            blueDuckScore.Initialize(null, scoreInfoRc, 0);
            blueDuckScore.Conent = "= X 200";
            AddObjectToShowLayer(blueDuckScore);

            duckpos.Y += 120;
            duckpos.X -= 40;
            parrotDuck = new ParrotModel();
            parrotDuck.Initialize(null, rectBackground, 0);
            parrotDuck.SetStartPos(duckpos);
            AddObjectToShowLayer(parrotDuck);

            scoreInfoRc.Y = (int)duckpos.Y-10;
            scoreInfoRc.X += 25;
            parrotDuckScore = new TitleItemModel();
            parrotDuckScore.Initialize(null, scoreInfoRc, 0);
            parrotDuckScore.Conent = "= X -100";
            AddObjectToShowLayer(parrotDuckScore);


            List<ModelObject> objlst = null;
            GetObjects(out objlst);
            foreach (var obj in objlst)
            {
                AddObjectToShowLayer(obj);
            }

        }


        void NewDog()
        {
            // dog seek duck
            if (dog != null)
            {
                // remove the dog from current layer
                RemoveObjectFromShowLayer(dog);
                dog = null;
            }
            dog = new DogModel();
            dog.Initialize(null, dogRunSpace, 0);
            dog.StartPilot();

            AddObjectToShowLayer(dog);

            /*
            panda = new PandaModel();
            panda.Initialize(null, dogRunSpace, 0);
             */

            /*
            plane = new PlaneModel();
            plane.Initialize(null, planeSpace, 0);

            baloon = new BaloonModel();
            baloon.Initialize(null, planeSpace, 0);
            */
        }

        override public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = new List<ModelObject>();

            List<ModelObject> backgroundobjlst;
            backgroundPage.GetObjects(out backgroundobjlst);
            foreach (ModelObject obj in backgroundobjlst)
            {
                objlst.Add(obj);
            }

        }

        override public void Update(GameTime gametime)
        {
            backgroundPage.Update(gametime);
            //gameOverTitleItem.Update(gametime);
            blackDuck.Update(gametime);
            blueDuck.Update(gametime);
            parrotDuck.Update(gametime);

            TimeSpan showtime = DateTime.Now - startTime;
            /*
            if (showtime.TotalSeconds > 20)
            {
                duckHuntGame.StartNextLevel();
            }
            */

            dog.Update(gametime);
            if (dog.Gone)
            {
                RemoveObjectFromShowLayer(dog);
                dog = null;
                blackDuck = null;
                blueDuck = null;
                parrotDuck = null;
                duckHuntGame.StartNextLevel();
            }
        }


        override public void Press(List<Vector2> clickpositionlist)
        {

        }
        override public void Click(List<Vector2> clickpositionlist)
        {
            //
            return;
        }

        /////////
        DateTime startTime = DateTime.Now;
        public void ShowLevelGoal(int score)
        {
            LevelGoal = score;
            startTime = DateTime.Now;
            //gameOverTitleItem.Conent = "Level Target: " + LevelGoal.ToString();
        }

        public int LevelGoal
        {
            get;
            set;
        }
    }


    class LevelConclusionPage : GamePage
    {
        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //
        TitleItemModel gameOverTitleItem;

        Rectangle gameOverTitleSpace;

        GameBackGroundPage backgroundPage = null;
        DuckHuntGame duckHuntGame = null;
        override public void InitGamePage(DuckHuntGame game)
        {
            duckHuntGame = game;
            backgroundPage = game.GetBackgroundPage();

            Rectangle rectBackground = game.GetGlobalViewRect();


            TitleItemModel titleItem = new TitleItemModel();
            gameOverTitleSpace.X = (rectBackground.Width - titleItem.GetSpace().Width) / 2;
            gameOverTitleSpace.Y = 30;
            gameOverTitleSpace.Width = titleItem.GetSpace().Width;
            gameOverTitleSpace.Height = titleItem.GetSpace().Height;

            NewMenu();


            List<ModelObject> objlst = null;
            GetObjects(out objlst);
            foreach (var obj in objlst)
            {
                AddObjectToShowLayer(obj);
            }

        }

        override public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = new List<ModelObject>();

            objlst.Add(gameOverTitleItem);

            List<ModelObject> backgroundobjlst;
            backgroundPage.GetObjects(out backgroundobjlst);
            foreach (ModelObject obj in backgroundobjlst)
            {
                objlst.Add(obj);
            }

        }

        override public void Update(GameTime gametime)
        {
            backgroundPage.Update(gametime);
            gameOverTitleItem.Update(gametime);

            TimeSpan showtime = DateTime.Now - startTime;
            if (showtime.TotalSeconds > 2)
            {
                if (acchievedScore > targetScore)
                {
                    // goto stage prop page
                    duckHuntGame.GotoPropSelctPage();

                }
                else
                {
                    // goto game over page
                    duckHuntGame.GotoGameOverPage(acchievedScore, 1);
                }
            }
        }


        override public void Press(List<Vector2> clickpositionlist)
        {

        }
        override public void Click(List<Vector2> clickpositionlist)
        {
            //
            return;
        }

        /////////

        void NewMenu()
        {
            gameOverTitleItem = new TitleItemModel();
            gameOverTitleItem.Initialize(null, gameOverTitleSpace, 0);
            gameOverTitleItem.Conent = "Acchieved Goal ";
        }

        DateTime startTime = DateTime.Now;
        public void SetConclusion(int score, int targetscore1)
        {
            targetScore = targetscore1;
            acchievedScore = score;
            startTime = DateTime.Now;

            if (acchievedScore > targetScore)
            {
                gameOverTitleItem.Conent = "Acchieved Goal";
            }
            else
            {
                gameOverTitleItem.Conent = "Fail Goal";
            }
        }

        int acchievedScore = 0;
        int targetScore = 0;

    }


    class StagePorpSelectPage : GamePage
    {
        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //
        TitleItemModel gameOverTitleItem;

        Rectangle gameOverTitleSpace;
        MenuItemModel menuRetryItem;
        Rectangle retryMenuSpace;

        GameBackGroundPage backgroundPage = null;
        DuckHuntGame duckHuntGame = null;
        override public void InitGamePage(DuckHuntGame game)
        {
            duckHuntGame = game;
            backgroundPage = game.GetBackgroundPage();

            Rectangle rectBackground = game.GetGlobalViewRect();


            TitleItemModel titleItem = new TitleItemModel();
            gameOverTitleSpace.X = (rectBackground.Width - titleItem.GetSpace().Width) / 2;
            gameOverTitleSpace.Y = 30;
            gameOverTitleSpace.Width = titleItem.GetSpace().Width;
            gameOverTitleSpace.Height = titleItem.GetSpace().Height;


            MenuItemModel menuItem = new MenuItemModel();
            retryMenuSpace.X = 700;
            retryMenuSpace.Y = 300;
            retryMenuSpace.Width = menuItem.GetSpace().Width;
            retryMenuSpace.Height = menuItem.GetSpace().Height;


            NewMenu();


            List<ModelObject> objlst = null;
            GetObjects(out objlst);
            foreach (var obj in objlst)
            {
                AddObjectToShowLayer(obj);
            }

        }

        override public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = new List<ModelObject>();

            objlst.Add(gameOverTitleItem);
            objlst.Add(menuRetryItem);

            List<ModelObject> backgroundobjlst;
            backgroundPage.GetObjects(out backgroundobjlst);
            foreach (ModelObject obj in backgroundobjlst)
            {
                objlst.Add(obj);
            }

        }

        override public void Update(GameTime gametime)
        {
            backgroundPage.Update(gametime);
            gameOverTitleItem.Update(gametime);

            TimeSpan showtime = DateTime.Now - startTime;

        }


        override public void Press(List<Vector2> clickpositionlist)
        {

        }
        override public void Click(List<Vector2> clickpositionlist)
        {
            //
            foreach(var pos in clickpositionlist)
            {
                if (menuRetryItem.Hit(pos))
                {
                    // got o next level
                    duckHuntGame.StartNextLevel();
                }
            }
            return;
        }

        /////////

        void NewMenu()
        {
            gameOverTitleItem = new TitleItemModel();
            gameOverTitleItem.Initialize(null, gameOverTitleSpace, 0);
            gameOverTitleItem.Conent = "Please select stage prop";

            this.menuRetryItem = new MenuItemModel();
            menuRetryItem.Initialize(null, retryMenuSpace, 0);
            menuRetryItem.Conent = "Next";

        }

        DateTime startTime = DateTime.Now;
        

        int acchievedScore = 0;
        int targetScore = 0;

    }


    class GameSnapPage : GamePage
    {
        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //

        MenuItemModel menuPausedInfoItem;
        Rectangle pausedMenuSpace;

        GameBackGroundPage backgroundPage = null;
        DuckHuntGame duckHuntGame = null;
        override public void InitGamePage(DuckHuntGame game)
        {
            duckHuntGame = game;
            backgroundPage = game.GetBackgroundPage();

            Rectangle rectBackground = game.GetGlobalViewRect();

            pausedMenuSpace.X = 50;
            pausedMenuSpace.Y = rectBackground.Height/3 - 20;
            pausedMenuSpace.Height = rectBackground.Height;
            pausedMenuSpace.Width = (int)(rectBackground.Width * 1.0f / 3);

            menuPausedInfoItem = new MenuItemModel();

            menuPausedInfoItem.Initialize(null, pausedMenuSpace, 0);
            menuPausedInfoItem.Conent = "Paused";

            List<ModelObject> objlst = null;
            GetObjects(out objlst);
            foreach (var obj in objlst)
            {
                AddObjectToShowLayer(obj);
            }

        }

        override public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = new List<ModelObject>();

            objlst.Add(menuPausedInfoItem);

            List<ModelObject> backgroundobjlst;
            backgroundPage.GetObjects(out backgroundobjlst);
            foreach (ModelObject obj in backgroundobjlst)
            {
                objlst.Add(obj);
            }

        }
        override public void Update(GameTime gametime)
        {
            //backgroundPage.Update(gametime);
        }


        override public void Press(List<Vector2> clickpositionlist)
        {

        }
        override public void Click(List<Vector2> clickpositionlist)
        {
            //
            return;
        }

        /////////

        public  GameSnapPage()
        {
            menuPausedInfoItem = new MenuItemModel();
        }
    }


    class GameScoreListPage : GamePage
    {

        ScroeListBoardModel scoreListBoard;
        Rectangle scoreListBoardSpace;

        MenuItemModel returnMenuItem;
        Rectangle returnMenuSpace;


        GameBackGroundPage backgroundPage = null;
        DuckHuntGame duckHuntGame = null;


        public GameScoreListPage()
        {
            scoreListBoard = new ScroeListBoardModel();
            returnMenuItem = new MenuItemModel();
            returnMenuItem.Conent = "Return";
        }

        override public void InitGamePage(DuckHuntGame game)
        {
            duckHuntGame = game;
            backgroundPage = duckHuntGame.GetBackgroundPage();

            Rectangle rectBackground = duckHuntGame.GetGlobalViewRect();
            ScroeListBoardModel scoreListBoard1 = new ScroeListBoardModel();
            MenuItemModel menuItm1 = new MenuItemModel();

            if (rectBackground.Width < rectBackground.Height)
            {
                scoreListBoardSpace.X = rectBackground.Top +
                    (rectBackground.Height - scoreListBoard1.GetSpace().Width) / 2;
                scoreListBoardSpace.Y = rectBackground.Left + 100;
                scoreListBoardSpace.Width = scoreListBoard1.GetSpace().Width;
                scoreListBoardSpace.Height = scoreListBoard1.GetSpace().Height;

                returnMenuSpace.X = 150;
                returnMenuSpace.Y = 50;
                returnMenuSpace.Width = menuItm1.GetSpace().Width;
                returnMenuSpace.Height = menuItm1.GetSpace().Height;
            }
            else
            {
                scoreListBoardSpace.X = rectBackground.Left +
                    (rectBackground.Width - scoreListBoard1.GetSpace().Width) / 2;
                scoreListBoardSpace.Y = rectBackground.Top + 50;
                scoreListBoardSpace.Width = scoreListBoard1.GetSpace().Width;
                scoreListBoardSpace.Height = scoreListBoard1.GetSpace().Height;

                returnMenuSpace.X = 150;
                returnMenuSpace.Y = 150;
                returnMenuSpace.Width = menuItm1.GetSpace().Width;
                returnMenuSpace.Height = menuItm1.GetSpace().Height;

            }


            scoreListBoard.Initialize(null, scoreListBoardSpace, 0);
            scoreListBoard.ScoreList = game.DuckHuntGameData.ScoreList;
            scoreListBoard.LevelList = game.DuckHuntGameData.LevelList;
            scoreListBoard.GlobalScoreList = game.DuckHuntGameData.GlobalScoreList;

            returnMenuItem.Initialize(null, returnMenuSpace, 0);

            List<ModelObject> objlst = null;
            GetObjects(out objlst);
            foreach (var obj in objlst)
            {
                AddObjectToShowLayer(obj);
            }


        }
        override public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = new List<ModelObject>();

            objlst.Add(scoreListBoard);
            objlst.Add(returnMenuItem);

            List<ModelObject> backgroundobjlst;
            backgroundPage.GetObjects(out backgroundobjlst);
            foreach (ModelObject obj in backgroundobjlst)
            {
                objlst.Add(obj);
            }
        }
        override public void Update(GameTime gametime)
        {
            backgroundPage.Update(gametime);
            scoreListBoard.Update(gametime);
            returnMenuItem.Update(gametime);

            // 
        }


        override public void Press(List<Vector2> clickpositionlist)
        {


        }

        override public void Click(List<Vector2> clickpositionlist)
        {
            backgroundPage.Click(clickpositionlist);

            // check return button
            if (returnMenuItem.Hit(clickpositionlist[0]))
            {
                duckHuntGame.ReturnToPrevious();
            }
        }


        /////
        void NewScoreListBoard()
        {
            scoreListBoard = new ScroeListBoardModel();
            scoreListBoard.Initialize(null, scoreListBoardSpace, 0);
        }

        public ScroeListBoardModel GetScoreListModel()
        {
            return scoreListBoard;
        }
    }



    class KeyboardPage : GamePage
    {

        GamePage parentPage = null;
        DuckHuntGame duckHuntGame = null;

        KeyboardModel keyboard;

        public KeyboardPage(GamePage parent)
        {
            keyboard = new KeyboardModel();
            parentPage = parent;
        }

        override public void InitGamePage(DuckHuntGame game)
        {
            duckHuntGame = game;
            Vector2 pos;
            pos.X = (game.GetGlobalViewRect().Width - keyboard.GetSpace().Width) / 2;
            pos.Y = game.GetGlobalViewRect().Height - keyboard.GetSpace().Height;
            Rectangle keyboardspace = new Rectangle();
            keyboardspace = keyboard.GetSpace();
            keyboardspace.Offset((int)pos.X, (int)pos.Y);
            keyboard.Initialize(null, keyboardspace, 0);
        }
        override public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = new List<ModelObject>();

            objlst.Add(keyboard);

            List<ModelObject> backgroundobjlst;
            parentPage.GetObjects(out backgroundobjlst);
            foreach (ModelObject obj in backgroundobjlst)
            {
                objlst.Add(obj);
            }
        }
        override public void Update(GameTime gametime)
        {
            parentPage.Update(gametime);
            keyboard.Update(gametime);
            //scoreListBoard.Update(gametime);

            // 
        }


        override public void Press(List<Vector2> clickpositionlist)
        {

        }

        override public void Click(List<Vector2> clickpositionlist)
        {
            // check return button
            //keyboard.cl
        }


        /////
    }


    class GameHelpPage : GamePage
    {
        override public void InitGamePage(DuckHuntGame game)
        {
        }
        override public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = null;
        }
        override public void Update(GameTime gametime)
        {
        }

        override public void Press(List<Vector2> clickpositionlist)
        {

        }
        override public void Click(List<Vector2> clickpositionlist)
        {
        }
    }

    class GameConfigPage : GamePage
    {
        CheckBoxModel backgroundMusic;
        Rectangle bgMusicCheckboxSpace;

        CheckBoxModel gameSound;
        Rectangle gameSoundCheckboxSpace;


        MenuItemModel returnMenuItem;
        Rectangle returnMenuSpace;



        GameBackGroundPage backgroundPage = null;
        DuckHuntGame duckHuntGame = null;


        // Create a composite setting 
        /*
        Windows.Storage.ApplicationDataCompositeValue composite;
        Windows.Storage.ApplicationDataContainer localSettings;
        */


        public GameConfigPage()
        {
            backgroundMusic = new CheckBoxModel();
            backgroundMusic.Content = "BackGround Music";
            gameSound = new CheckBoxModel();
            gameSound.Content = "Game Sound";

            returnMenuItem = new MenuItemModel();

        }

        override public void InitGamePage(DuckHuntGame game)
        {
            duckHuntGame = game;
            backgroundPage = duckHuntGame.GetBackgroundPage();

            Rectangle rectBackground = duckHuntGame.GetGlobalViewRect();
            CheckBoxModel checkBoxModel = new CheckBoxModel();
            MenuItemModel menuItm1 = new MenuItemModel();

            if (rectBackground.Width < rectBackground.Height)
            {
                bgMusicCheckboxSpace.X = rectBackground.Top +
                    (rectBackground.Width) / 2 - 100;
                bgMusicCheckboxSpace.Y = rectBackground.Left + 150;
                bgMusicCheckboxSpace.Width = checkBoxModel.GetSpace().Width;
                bgMusicCheckboxSpace.Height = checkBoxModel.GetSpace().Height;


                gameSoundCheckboxSpace = bgMusicCheckboxSpace;
                gameSoundCheckboxSpace.Y += 100;

                returnMenuSpace.X = 350;
                returnMenuSpace.Y = 150;
                returnMenuSpace.Width = menuItm1.GetSpace().Width;
                returnMenuSpace.Height = menuItm1.GetSpace().Height;

            }
            else
            {
                bgMusicCheckboxSpace.X = rectBackground.Left +
                    (rectBackground.Width) / 2 - 100;
                bgMusicCheckboxSpace.Y = rectBackground.Top + 150;
                bgMusicCheckboxSpace.Width = checkBoxModel.GetSpace().Width;
                bgMusicCheckboxSpace.Height = checkBoxModel.GetSpace().Height;


                gameSoundCheckboxSpace = bgMusicCheckboxSpace;
                gameSoundCheckboxSpace.Y += 100;

                returnMenuSpace.X = 350;
                returnMenuSpace.Y = 150;
                returnMenuSpace.Width = menuItm1.GetSpace().Width;
                returnMenuSpace.Height = menuItm1.GetSpace().Height;


            }


            backgroundMusic.Initialize(null, bgMusicCheckboxSpace, 0);
            gameSound.Initialize(null, gameSoundCheckboxSpace, 0);
            returnMenuItem.Initialize(null, returnMenuSpace, 0);

            gameSound.Checked = duckHuntGame.DuckHuntGameData.EnableGameSound;
            backgroundMusic.Checked = duckHuntGame.DuckHuntGameData.EnableBgMusic;

            returnMenuItem.Conent = "Return";


            List<ModelObject> objlst = null;
            GetObjects(out objlst);
            foreach (var obj in objlst)
            {
                AddObjectToShowLayer(obj);
            }



        }
        override public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = new List<ModelObject>();

            objlst.Add(backgroundMusic);
            objlst.Add(gameSound);
            objlst.Add(returnMenuItem);

            List<ModelObject> backgroundobjlst;
            backgroundPage.GetObjects(out backgroundobjlst);
            foreach (ModelObject obj in backgroundobjlst)
            {
                objlst.Add(obj);
            }
        }
        override public void Update(GameTime gametime)
        {
            backgroundMusic.Update(gametime);
            gameSound.Update(gametime);
            returnMenuItem.Update(gametime);

            this.backgroundPage.Update(gametime);
        }


        override public void Press(List<Vector2> clickpositionlist)
        {

        }

        override public void Click(List<Vector2> clickpositionlist)
        {
            backgroundPage.Click(clickpositionlist);

            // check return button
            backgroundMusic.Click(clickpositionlist[0]);
            gameSound.Click(clickpositionlist[0]);
            if (returnMenuItem.Hit(clickpositionlist[0]))
            {
                duckHuntGame.ReturnToPrevious();
            }

            bool saveData = false;
            if (duckHuntGame.DuckHuntGameData.EnableGameSound != gameSound.Checked)
            {
                duckHuntGame.DuckHuntGameData.EnableGameSound = gameSound.Checked;
                saveData = true;
            }
            if (duckHuntGame.DuckHuntGameData.EnableBgMusic != backgroundMusic.Checked)
            {
                duckHuntGame.DuckHuntGameData.EnableBgMusic = backgroundMusic.Checked;
                saveData = true;
            }
            if (saveData)
            {
                duckHuntGame.SaveGameData();
            }
        }

    }

    class GameBackGroundPage : GamePage
    {
        Rectangle rectBackground;
        SkyModel blueSky;
        SmokeModel smoke;
        CloudModel cloud;
        GrassModel grass;
        ForegroundGrassModel forground;

        ButtonModel pause;
        Rectangle pauseButtonSpace;

        bool showPause = false;

        DuckHuntGame duckHuntGame = null;

        public void ShowPause(bool show)
        {
            showPause = show;
            if (showPause)
            {
                AddObjectToShowLayer(pause);
                pause.Checked = true;
            }
            else
            {
                pause.Checked = false;
                RemoveObjectFromShowLayer(pause);
            }
        }

        public ButtonModel GetPauseButton()
        {
            return pause;
        }

        override public void InitGamePage(DuckHuntGame game)
        {
            duckHuntGame = game;
            rectBackground = game.GetGlobalViewRect();


            ButtonModel button = new ButtonModel();
            if (rectBackground.Width < rectBackground.Height)
            {
                pauseButtonSpace.X = rectBackground.Height - button.GetSpace().Width + 130;
                pauseButtonSpace.Y = rectBackground.Right - 150; ;
                pauseButtonSpace.Width = button.GetSpace().Width;
                pauseButtonSpace.Height = button.GetSpace().Height;
            }
            else
            {

                pauseButtonSpace.X = rectBackground.Width - button.GetSpace().Width + 130;
                pauseButtonSpace.Y = rectBackground.Bottom - 150;
                pauseButtonSpace.Width = button.GetSpace().Width;
                pauseButtonSpace.Height = button.GetSpace().Height;
            }


            // calculate background settings
            NewBackground();

            List<ModelObject> objlst = null;
            GetObjects(out objlst);
            foreach (var obj in objlst)
            {
                AddObjectToShowLayer(obj);
            }

        }

        override public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = new List<ModelObject>();
            objlst.Add(blueSky);
            objlst.Add(smoke);
            objlst.Add(cloud);
            objlst.Add(grass);
            objlst.Add(forground);
            if (showPause)
            {
                objlst.Add(pause);
            }

        }
        override public void Update(GameTime gametime)
        {
            cloud.Update(gametime);
        }


        override public void Press(List<Vector2> clickpositionlist)
        {

        }

        override public void Click(List<Vector2> clickpositionlist)
        {
            if (showPause)
            {
                if (pause.Click(clickpositionlist[0]))
                {
                    // reset the icon
                    pause.Click(clickpositionlist[0]);

                    duckHuntGame.PauseGame(false);
                }
            }
        }


        //////
        void NewBackground()
        {
            // dog seek duck
            blueSky = new SkyModel();
            blueSky.Initialize(null, rectBackground, 0);

            smoke = new SmokeModel();
            smoke.Initialize(null, rectBackground, 0);

            cloud = new CloudModel();
            cloud.Initialize(null, rectBackground, 0);

            grass = new GrassModel();
            grass.Initialize(null, rectBackground, 0);

            forground = new ForegroundGrassModel();
            forground.Initialize(null, rectBackground, 0);

            pause = new ButtonModel();
            pause.Initialize(null, pauseButtonSpace, 0);
            pause.Checked = false;
        }
    }

    class GameData
    {
        //SortedSet<KeyValuePair<string, int>> scorelist;

        // configuration
        public bool EnableBgMusic = true;
        public bool EnableGameSound = true;
        public DateTime installedDate = DateTime.Now;
        public DateTime globalscorelistUpdateDate = DateTime.Now - new TimeSpan(1, 0, 0, 0);

        // score list
        List<KeyValuePair<int, string>> globalscorelist;
        Dictionary<int, string> scorelist;
        Dictionary<int, string> levellist;
        public Dictionary<int, string> ScoreList
        {
            get
            {
                return scorelist;
            }
        }
        public List<KeyValuePair<int, string>> GlobalScoreList
        {
            get
            {
                return globalscorelist;
            }
        }
        
        public Dictionary<int, string> LevelList
        {
            get
            {
                return levellist;
            }
        }


        internal class ScoreEntry
        {
            public string Score
            {
                get;
                set;
            }
            public string PlayerName
            {
                get;
                set;
            }
        }


        void UploadScore2Server(string name, int score)
        {
             bool needupload = true;
            if (globalscorelist.Count >= 10)
            {
                var worldscorelist = globalscorelist.OrderBy(c => c.Key);
                foreach (var scoreitm in worldscorelist)
                {
                    if (score < scoreitm.Key)
                    {
                        needupload = false;
                    }
                    break;
                }
            }
            if (needupload)
            {
                //
                UInt16 svrscore = 0;
                svrscore -= (UInt16)score;
                string svrscorestr = svrscore.ToString();

                int index = 0;
                bool trynext = false;
                do
                {
                    trynext = false;

                    svrscore = 0;
                    svrscore -= (UInt16)score;
                    svrscorestr = svrscore.ToString();


                    // score_year.mon.day.hour_index
                    svrscorestr += "_";
                    DateTime now = DateTime.Now;
                    svrscorestr += now.Year.ToString();
                    svrscorestr += ".";
                    svrscorestr += now.Month.ToString();
                    svrscorestr += ".";
                    svrscorestr += now.Day.ToString();
                    svrscorestr += ".";
                    svrscorestr += now.Hour.ToString();

                    svrscorestr += "_" + index.ToString();

                    TableHelper TableHelper = new TableHelper(StorageAccount, StorageKey);
                    try
                    {
                        bool ret = TableHelper.InsertEntity("pfsgameduckhunt", "duckhunt", svrscorestr,
                            new ScoreEntry { Score = svrscorestr, PlayerName = name });
                        if (!ret)
                        {
                            trynext = true;
                        }
                    }
                    catch (Exception)
                    {
                        trynext = true;
                    }
                    index++;
                } while (trynext && index < 512);
            }
            
        }
        public void AddScore(string name, int score)
        {
            scorelist[score] = name;
            var result = scorelist.OrderByDescending(c => c.Key);
            Dictionary<int, string> tmp = new Dictionary<int, string>();
            int i = 0;
            foreach (var item in result)
            {
                i++;
                if (i > 5)
                {
                    break;
                }
                tmp[item.Key] = item.Value;
            }
            scorelist.Clear();
            foreach (var item in tmp)
            {
                scorelist[item.Key] = item.Value;
            }

            UploadScoreState state = new UploadScoreState();
            state.gamedata = this;
            state.name = name;
            state.score = score;
#if WINDOWS_PHONE
            //ThreadPool.QueueUserWorkItem(AsyncUploadScore2Server, state);
#else

            Task task = Task.Run(async () =>AsyncUploadScore2Server(state));
#endif
        }

        internal class UploadScoreState: Object
        {
            public GameData gamedata;
            public string name;
            public int score;
        }
        void AsyncUploadScore2Server(UploadScoreState state)
        {
            state.gamedata.UploadScore2Server(state.name, state.score);
        }


        public void AddLevel(string name, int level)
        {
            levellist[level] = name;

            var result = levellist.OrderByDescending(c => c.Key);
            Dictionary<int, string> tmp = new Dictionary<int, string>();
            int i = 0;
            foreach (var item in result)
            {
                i++;
                if (i > 5)
                {
                    break;
                }
                tmp[item.Key] = item.Value;
            }
            levellist.Clear();
            foreach (var item in tmp)
            {
                levellist[item.Key] = item.Value;
            }
        }

        public void GetGameRecord(out int highestScore, out int highestLevel)
        {
            highestLevel = 0;
            highestScore = 0;
            var result = scorelist.OrderByDescending(c => c.Key);
            foreach (var item in result)
            {
                highestScore = item.Key;
                break;
            }


            result = levellist.OrderByDescending(c => c.Key);
            foreach (var item in result)
            {
                highestLevel = item.Key;
                break;
            }
        }

        public GameData()
        {
            scorelist = new Dictionary<int, string>();
            levellist = new Dictionary<int, string>();

            globalscorelist = new List<KeyValuePair<int, string>>();
            var key = scorelist.OrderByDescending(c => c.Key);
            var key1 = levellist.OrderByDescending(c => c.Key);
        }

#if !WINDOWS_PHONE
        private async Task _SaveAsync(string filename, string content)
        {
            // Get a reference to the Local Folder
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Create the file in the local folder, or if it already exists, just open it
            Windows.Storage.StorageFile storageFile =
                await localFolder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.OpenIfExists);

            Stream writeStream = await storageFile.OpenStreamForWriteAsync();
            using (StreamWriter writer = new StreamWriter(writeStream))
            {
                await writer.WriteAsync(content);
            }

            return;
        }
#endif

        public static byte[] StringToByte(string InString)
        {

            char[] ByteStrings = InString.ToCharArray();

            byte[] ByteOut;

            ByteOut = new byte[ByteStrings.Length];

            for (int i = 0; i < ByteStrings.Length; i++)
            {

                ByteOut[i] = (byte)ByteStrings[i];
            }

            return ByteOut;

        }


        public static string ByteToString(byte[] bytein)
        {

            string content = "";


            for (int i = 0; i < bytein.Length; i++)
            {

                content += (char)bytein[i];
            }

            return content;

        }



        public void Save(string filename)
        {
            //Task task = Task.Run(async () => await _SaveAsync(filename));
            //task.Wait();
            // Get a reference to the Local Folder
            string content = "";
            SaveGameData(ref content);
#if WINDOWS_PHONE

            IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForApplication();

            // open isolated storage, and write the savefile.
            IsolatedStorageFileStream fs = null;
            using (fs = savegameStorage.CreateFile(filename))
            {
                if (fs != null)
                {
                    // just overwrite the existing info for this example.
                    byte[] bytes = StringToByte(content);
                    fs.Write(bytes, 0, bytes.Length);
                }
            }


#else
            Task task = Task.Run(async () => await _SaveAsync(filename, content));
            task.Wait();
#endif


            return;
        }

#if !WINDOWS_PHONE
        private async Task<string> _LoadAsync(string filename)
        {
            string content = "";
            // Get a reference to the Local Folder
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Create the file in the local folder, or if it already exists, just open it
            Windows.Storage.StorageFile storageFile =
                        await localFolder.CreateFileAsync(
                        filename,
                        Windows.Storage.CreationCollisionOption.OpenIfExists);

            Stream readStream = await storageFile.OpenStreamForReadAsync();
            using (StreamReader reader = new StreamReader(readStream))
            {
                content = reader.ReadToEnd();
            }

            return content;
        }
#endif

        public void Load(string filename)
        {
            string content = "";
#if WINDOWS_PHONE

            IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForApplication();
            // open isolated storage, and write the savefile.

            if (savegameStorage.FileExists(filename))
            {
                using (IsolatedStorageFileStream fs = savegameStorage.OpenFile(filename, System.IO.FileMode.Open))
                {
                    if (fs != null)
                    {
                        // Reload the saved high-score data.
                        byte[] saveBytes = new byte[fs.Length];
                        int count = fs.Read(saveBytes, 0, (int)fs.Length);
                        if (count > 0)
                        {
                            content = ByteToString(saveBytes);
                        }
                    }
                }
            }


            try
            {
                LoadGameData(content);
            }
            catch (Exception)
            {
            }
            //LoadGameData(reader);
#else
            Task<string> task = Task.Run(async () => await _LoadAsync(filename));
            task.Wait();
            content = task.Result;
            LoadGameData(content);
#endif

            //
            //Task.Run(UpdateGlobaScoreList(filename));
            //UpdateGlobaScoreList(filename);


            UpdateGlobalScoreState state = new UpdateGlobalScoreState();
            state.gamedata = this;
            state.filename = filename;
#if WINDOWS_PHONE
            //ThreadPool.QueueUserWorkItem(AsyncUpdateGlobalScore, state);
#else
            Task task1  = Task.Run(async () => AsyncUpdateGlobalScore(state));
#endif
        }

   

        internal class UpdateGlobalScoreState: Object
        {
            public GameData gamedata;
            public string filename;
        }

        void AsyncUpdateGlobalScore(UpdateGlobalScoreState state)
        {
            state.gamedata.UpdateGlobaScoreList(state.filename);
        }

        private void SaveGameData(ref string content)
        {
            // <?xml version='1.0'?>
            // <DuckHunt>
            //      <installeddate>
            //          
            //      </installeddate>
            //      <scorelist>
            //          <count> </count>
            //          <record>
            //              <name></name>
            //              <score></score>
            //          </record>
            //      </scorelist>
            //      <levellist>
            //          <count> </count>
            //          <record>
            //              <name></name>
            //              <level></level>
            //          </record>
            //      </levellist>
            //      <globalscorelist>
            //          <count> </count>
            //          <record>
            //              <name></name>
            //              <score></score>
            //          </record>
            //      </globalscorelist>
            // </DuckHunt>

            content += "<?xml version='1.0'?>";
            content += "<DuckHunt>";
            SaveInstalledDate(ref content, this.installedDate);
            SaveGlobalScoreListUpdateDate(ref content, this.globalscorelistUpdateDate);
            
            SaveScoreList(ref content);
            SaveLevelList(ref content);
            SaveGlobalScoreList(ref content);

            // could save other configuration
            SaveGameConfig(ref content);
            content += "</DuckHunt>";
        }

        private void SaveInstalledDate(ref string content, DateTime installedDate)
        {
            content += "<installeddate>";
            content += installedDate.ToString();
            content += "</installeddate>";
        }

        private void SaveGlobalScoreListUpdateDate(ref string content, DateTime updateDate)
        {
            content += "<globalscorelistupdatedate>";
            content += updateDate.ToString();
            content += "</globalscorelistupdatedate>";
        }


        private void SaveCount(ref string content, int count)
        {
            content += "<count>";
            content += count.ToString();
            content += "</count>";
        }

        private void SaveRecord(ref string content, string name, int score)
        {
            content += "<record>";
            content += "<name>" + name + "</name>";
            content += "<score>" + score.ToString() + "</score>";
            content += "</record>";
        }

        private void SaveLevelRecord(ref string content, string name, int score)
        {
            content += "<record>";
            content += "<name>" + name + "</name>";
            content += "<level>" + score.ToString() + "</level>";
            content += "</record>";
        }

        private void SaveScoreList(ref string content)
        {
            content += "<scorelist>";
            SaveCount(ref content, scorelist.Count);
            foreach (KeyValuePair<int, string> pair in scorelist)
            {
                SaveRecord(ref content, pair.Value, pair.Key);

            }
            content += "</scorelist>";
        }

        private void SaveGlobalScoreList(ref string content)
        {
            content += "<globalscorelist>";
            SaveCount(ref content, globalscorelist.Count);
            foreach (KeyValuePair<int, string> pair in globalscorelist)
            {
                SaveRecord(ref content, pair.Value, pair.Key);

            }
            content += "</globalscorelist>";
        }

        private void SaveLevelList(ref string content)
        {
            content += "<levellist>";
            SaveCount(ref content, levellist.Count);
            foreach (KeyValuePair<int, string> pair in levellist)
            {
                SaveLevelRecord(ref content, pair.Value, pair.Key);

            }
            content += "</levellist>";
        }

        private void SaveGameConfig(ref string content)
        {
            content += "<configuration>";
            content += "<GameBackgorundSound>";
            content += this.EnableBgMusic ? "1" : "0";
            content += "</GameBackgorundSound>";
            content += "<GameSound>";
            content += this.EnableGameSound ? "1" : "0";
            content += "</GameSound>";
            content += "</configuration>";
        }


        private void LoadPlayerScore(XmlReader reader, ref int score)
        {
            if (reader.NodeType != XmlNodeType.Element || reader.Name != "score")
            {
                // error
                return;
            }
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    score = Convert.ToInt32(reader.Value);
                }
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "score")
                {
                    return;
                }
            }
        }


        private void LoadPlayerLevel(XmlReader reader, ref int level)
        {
            if (reader.NodeType != XmlNodeType.Element || reader.Name != "level")
            {
                // error
                return;
            }
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    level = Convert.ToInt32(reader.Value);
                }
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "level")
                {
                    return;
                }
            }
        }

        private void LoadPlayerName(XmlReader reader, ref string name)
        {
            if (reader.NodeType != XmlNodeType.Element || reader.Name != "name")
            {
                // error
                return;
            }
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    name = reader.Value;
                }
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "name")
                {
                    return;
                }
            }
        }

        private void LoadOneRecord(XmlReader reader, ref string name, ref int score)
        {
            if (reader.NodeType != XmlNodeType.Element || reader.Name != "record")
            {
                // error
                return;
            }
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "name")
                    {
                        LoadPlayerName(reader, ref name);
                    }
                    if (reader.Name == "score")
                    {
                        LoadPlayerScore(reader, ref score);
                    }
                }
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "record")
                {
                    return;
                }
            }

        }


        private void LoadOneLevelRecord(XmlReader reader, ref string name, ref int level)
        {
            if (reader.NodeType != XmlNodeType.Element || reader.Name != "record")
            {
                // error
                return;
            }
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "name")
                    {
                        LoadPlayerName(reader, ref name);
                    }
                    if (reader.Name == "level")
                    {
                        LoadPlayerLevel(reader, ref level);
                    }
                }
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "record")
                {
                    return;
                }
            }

        }

        private void LoadCount(XmlReader reader, ref int count)
        {
            if (reader.NodeType != XmlNodeType.Element || reader.Name != "count")
            {
                // error
                return;
            }
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    count = Convert.ToInt32(reader.Value);
                }
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Name == "count")
                    {
                        return;
                    }
                }
            }
        }

        private void LoadGlobalScoreListUpdateDate(XmlReader reader, ref DateTime updateDate)
        {
            if (reader.NodeType != XmlNodeType.Element || reader.Name != "globalscorelistupdatedate")
            {
                // error
                return;
            }
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    updateDate = Convert.ToDateTime(reader.Value);
                }
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Name == "globalscorelistupdatedate")
                    {
                        return;
                    }
                }
            }
        }

        private void LoadInstalledDate(XmlReader reader, ref DateTime installedDate)
        {
            if (reader.NodeType != XmlNodeType.Element || reader.Name != "installeddate")
            {
                // error
                return;
            }
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    installedDate = Convert.ToDateTime(reader.Value);
                }
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Name == "installeddate")
                    {
                        return;
                    }
                }
            }
        }

        private void LoadBackGroundMusic(XmlReader reader, ref  bool enablebgmusic)
        {
            if (reader.NodeType != XmlNodeType.Element || reader.Name != "GameBackgorundSound")
            {
                // error
                return;
            }

            // next item should be scorelist
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    enablebgmusic = Convert.ToInt32(reader.Value) == 1;
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    // end of element
                    if (reader.Name == "GameBackgorundSound")
                    {
                        return;
                    }
                }
            }
        }

        private void LoadGameSound(XmlReader reader, ref  bool enablegamesound)
        {
            if (reader.NodeType != XmlNodeType.Element || reader.Name != "GameSound")
            {
                // error
                return;
            }

            // next item should be scorelist
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    enablegamesound = Convert.ToInt32(reader.Value) == 1;
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    // end of element
                    if (reader.Name == "GameSound")
                    {
                        return;
                    }
                }
            }
        }

        private void LoadGameConfiguration(XmlReader reader)
        {
            if (reader.NodeType != XmlNodeType.Element || reader.Name != "configuration")
            {
                // error
                return;
            }


            // next item should be scorelist
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "GameBackgorundSound")
                    {
                        LoadBackGroundMusic(reader, ref this.EnableBgMusic);
                    }
                    if (reader.Name == "GameSound")
                    {
                        LoadGameSound(reader, ref this.EnableGameSound);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    // end of element
                    if (reader.Name == "configuration")
                    {
                        return;
                    }
                }
            }
        }

        private void LoadScoreList(XmlReader reader)
        {
            if (reader.NodeType != XmlNodeType.Element || reader.Name != "scorelist")
            {
                // error
                return;
            }

            // next item should be scorelist
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "count")
                    {
                        // find score list element
                        int count = 0;
                        LoadCount(reader, ref count);
                    }
                    if (reader.Name == "record")
                    {
                        string name = "";
                        int score = 0;
                        LoadOneRecord(reader, ref name, ref score);
                        scorelist[score] = name;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    // end of element
                    if (reader.Name == "scorelist")
                    {
                        return;
                    }
                }
            }
        }



        private void LoadGlobalScoreList(XmlReader reader)
        {
            if (reader.NodeType != XmlNodeType.Element || reader.Name != "globalscorelist")
            {
                // error
                return;
            }

            // next item should be scorelist
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "count")
                    {
                        // find score list element
                        int count = 0;
                        LoadCount(reader, ref count);
                    }
                    if (reader.Name == "record")
                    {
                        string name = "";
                        int score = 0;
                        LoadOneRecord(reader, ref name, ref score);
                        globalscorelist.Add(new KeyValuePair<int, string>(score, name));
                        //globalscorelist[score] = name;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    // end of element
                    if (reader.Name == "globalscorelist")
                    {
                        return;
                    }
                }
            }
        }

        private void LoadLevelList(XmlReader reader)
        {
            if (reader.NodeType != XmlNodeType.Element || reader.Name != "levellist")
            {
                // error
                return;
            }

            // next item should be scorelist
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "count")
                    {
                        // find score list element
                        int count = 0;
                        LoadCount(reader, ref count);
                    }
                    if (reader.Name == "record")
                    {
                        string name = "";
                        int level = 0;
                        LoadOneLevelRecord(reader, ref name, ref level);
                        levellist[level] = name;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    // end of element
                    if (reader.Name == "levellist")
                    {
                        return;
                    }
                }
            }
        }

        private void LoadGameData(string content)
        {
            //
#if WINDOWS_PHONE
            byte[] contentinbyte = StringToByte(content);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(contentinbyte);
#else
            StringReader stream = new StringReader(content);
#endif
            using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(stream))
            {
                try
                {
                    if (reader == null || reader.EOF || !reader.Read())
                    {
                        return;
                    }

                    while (reader.Read())
                    {
                        if (reader.Name == "DuckHunt")
                        {
                            // error
                            break;
                        }
                    }
                    if (reader.EOF)
                    {
                        return;
                    }

                    // next item should be scorelist
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "installeddate")
                            {
                                LoadInstalledDate(reader, ref installedDate);
                            }

                            if (reader.Name == "globalscorelistupdatedate")
                            {
                                LoadGlobalScoreListUpdateDate(reader, ref globalscorelistUpdateDate);
                            }

                            if (reader.Name == "scorelist")
                            {
                                // find score list element
                                LoadScoreList(reader);
                            }

                            if (reader.Name == "globalscorelist")
                            {
                                LoadGlobalScoreList(reader);
                            }

                            if (reader.Name == "levellist")
                            {
                                // find score list element
                                LoadLevelList(reader);
                            }
                            // could add other data

                        }
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "configuration")
                            {
                                // find score list element
                                LoadGameConfiguration(reader);
                            }

                            // could add other data

                        }
                        else if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            // end of element
                            if (reader.Name == "DuckHunt")
                            {
                                return;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    e.ToString();
                }
            }
        }

        const string StorageAccount = "pfsgame";
        const string StorageKey = "t/uUczMLzHscEBDXUJzoaaQTFw0AD8JpvWJ0mXi8A9kRo3/1wdMxWeS97tkwq0GdloPNBXqLBC0cxgxBEkxBvQ==";

        void UpdateGlobaScoreList(string localfilename) //"duckhunt.xml"
        {
            DateTime now = DateTime.Now;
            if (now.Date <= globalscorelistUpdateDate.Date)
            {
                return;
            }

            //
            TableHelper TableHelper = new TableHelper(StorageAccount, StorageKey);
            string result = TableHelper.QueryEntities("pfsgameduckhunt", 10);
            //var task = TableHelper.GetEntity("pfsgameduckhunt", "duckhunt", "");
            //task.Wait();

            // parse the result
            //<entry>
            //<content>
            //  <m:properties>
            //      <d:PartitionKey/>
            //      <d:PlayerName/>
            //      <d:Score>211_2013.10.17.20</d:Score>
            //  </m:properties>
            //</content>
            //</entry>

            LoadGlobalScoreListFromServer(result);

            globalscorelistUpdateDate = DateTime.Now;

            Save(localfilename);         
        }



        private bool ParseScoreEntry(System.Xml.XmlReader reader, ref string player, ref UInt16 score)
        {

            //<entry>
            //<content>
            //  <m:properties>
            //      <d:PartitionKey/>
            //      <d:PlayerName/>
            //      <d:Score>211_2013.10.17.20</d:Score>
            //  </m:properties>
            //</content>
            //</entry>

            string playsubfix = "";
            if (reader.NodeType != XmlNodeType.Element || reader.Name != "entry")
            {
                return false;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "m:properties")
                {
                    break;
                }
            }

            if (reader.NodeType != XmlNodeType.Element || reader.Name != "m:properties")
            {
                return false;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "d:PlayerName")
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Text)
                        {
                            player = reader.Value;
                            if (playsubfix.Length > 0)
                            {
                                player += playsubfix;
                            }
                            break;
                        }
                        //count = Convert.ToInt32(reader.Value);
                    }
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "d:Score")
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Text)
                        {
                            //
                            string value = reader.Value;
                            int index = value.IndexOf('_');
                            if (index > 0)
                            {
                                playsubfix = value.Substring(index, value.Length - index);
                                value = value.Substring(0, index);

                                if (player.Length > 0)
                                {
                                    player += playsubfix;
                                }
                            }
                            score = Convert.ToUInt16(value);
                            score = (UInt16)(0 - score);

                            break;
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "m:properties")
                {
                    break;
                }
            }

            // find </content>
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "content")
                {
                    break;
                }
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "entry")
                {
                    break;
                }
            }

            if (reader.NodeType != XmlNodeType.EndElement || reader.Name != "entry")
            {
                return false;
            }

            return true;
        }

        private void LoadGlobalScoreListFromServer(string content)
        {
            //
#if WINDOWS_PHONE
            byte[] contentinbyte = StringToByte(content);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(contentinbyte);
#else
            StringReader stream = new StringReader(content);
#endif

            //<entry>
            //<content>
            //  <m:properties>
            //      <d:PartitionKey/>
            //      <d:PlayerName/>
            //      <d:Score>211_2013.10.17.20</d:Score>
            //  </m:properties>
            //</content>
            //</entry>
            using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(stream))
            {
                try
                {
                    if (reader == null || reader.EOF || !reader.Read())
                    {
                        return;
                    }

                    while (reader.Read())
                    {
                        if (reader.Name == "entry" && reader.NodeType == XmlNodeType.Element)
                        {
                            // error
                            string player = "";
                            UInt16 score = 0;
                            if (ParseScoreEntry(reader, ref player, ref score))
                            {
                                try
                                {
                                    this.globalscorelist.Add(new KeyValuePair<int, string>(score, player));
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    e.ToString();
                }
            }
        }
    }

 

}