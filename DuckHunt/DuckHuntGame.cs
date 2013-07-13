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
using System.IO.IsolatedStorage;
#endif


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
                backgroundPage.ShowPause(true);
                GotoMainMenuPage();
            }
            else
            {
                // go to play page
                backgroundPage.ShowPause(false);
                GotoPlayPage();
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
            playPage = new GamePlayPage();
            scoreListPage = new GameScoreListPage();
            optionPage = new GameConfigPage();

            snapPage = new GameSnapPage();

            currentPage = mainMenuPage;
            //currentPage = gameOverPage;
            pagestack.Add(currentPage);


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
            currentPage = mainMenuPage;
        }


        public void GotoGameOverPage()
        {
            currentPage = gameOverPage;
        }

        public void GotoPlayPage()
        {
            currentPage = playPage;
        }
        public void GotoConfigPage()
        {
            pagestack.Add(currentPage);
            currentPage = optionPage;

        }

        public void GotoScoreListPage()
        {
            pagestack.Add(currentPage);
            currentPage = scoreListPage;
        }

        public void ReturnToPrevious()
        {
            int count = pagestack.Count;
            currentPage = pagestack[count - 1];
            pagestack.RemoveAt(count - 1);
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
            GotoPlayPage();
        }
        Rectangle screenRect = new Rectangle();

        public void StartGame(Rectangle screenRect1)
        {
            screenRect = screenRect1;

            gameData.Load("duckhunt.xml");
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

            snapPage.InitGamePage(this);


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

            ViewObjectFactory.SetLocalViewInfo(screenRect, orgpoint, defscale, bgorgpoint, bgdefscale);
        }

        public void SaveNewScore(int score, int level)
        {
            // 
            DateTime now = DateTime.Now;
            gameData.AddScore(now.ToString(), score);
            gameData.AddLevel(now.ToString(), level);
            gameData.Save("duckhunt.xml");
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

    enum GameMode { GAME_TIME_LIMIT, GAME_FREE_MODE };

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
            if (duckcount >= 6)
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
            if (duckcount >= 6)
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
                duck.SetSpeedRatio(1.2f);
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
                duck.SetSpeedRatio(1.3f);
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
                duck.SetSpeedRatio(1.5f);
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
            pilotypelist.Add(PilotType.DUCKCIRCLE);
            pilotypelist.Add(PilotType.DUCKELLIPSE);
            pilotypelist.Add(PilotType.DUCKEIGHT);
        }
        override public bool GetDuckList(out List<AnimalModel> ducks)
        {
            ducks = null;
            ducks = new List<AnimalModel>();

            DuckModel duck;
            pilottypeindex = pilottypeindex % pilotypelist.Count;


            string name = "chaptershowfuncurve_" + duckcount.ToString();

            for (int i = 0; i < 7; i++)
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
                duck.SetSpeedRatio(speedratio);
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

            if (mode == GameMode.GAME_TIME_LIMIT)
            {
                chapters = new List<GameChapter>();
                GameChapter chapter;
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
                chapters.Add(loveuChapter);

            }
            else
            {
                chapters = new List<GameChapter>();
                GameChapter chapter;
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

            while (true)
            {
                if (chapters.Count <= 0)
                {
                    return false;
                }
                chapter = chapters[0];
                if (chapter.CanBeRemoved())
                {
                    chapters.RemoveAt(0);
                }
                else
                {
                    return true;
                }
            }
        }
    }

    interface GamePage
    {
        void InitGamePage(DuckHuntGame game);
        void GetObjects(out List<ModelObject> objlst);
        void Update(GameTime gametime);
        void Click(List<Vector2> clickpositionlist);
    }

    class GamePlayPage : GamePage
    {

        GAME_PHASE phase = GAME_PHASE.GAME_SELECT;

        GameChapterManager gameChapterMgr;


        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //


        TimeBoardModel leftTime;
        LostDuckBoardModel lostDuck;
        Rectangle leftTimeBoardSpace;


        List<BulletModel> bulletsList;
        List<AnimalModel> duckList;
        Rectangle duckFlySpace;

        DogModel dog;
        Rectangle dogRunSpace;

        PandaModel panda;
        BaloonModel baloon;
        PlaneModel plane;
        ParrotModel parrot;
        Rectangle baloonSpace;

        HitBoardModel hitBoard;
        Rectangle hitBoardSpace;

        ScroeBoardModel scoreBoard;
        Rectangle scoreBoardSpace;

        ButtonModel pause;
        Rectangle pauseButtonSpace;


        //FireworkModel firework;
        Rectangle fireworkSpace;

        LevelUpBoardModel levelUp;
        Rectangle levelUpSpace;

        GameBackGroundPage backgroundPage = null;
        DuckHuntGame duckHuntGame = null;

        public GamePlayPage()
        {
            bulletsList = new List<BulletModel>();
            gameChapterMgr = new GameChapterManager();
        }

        public void InitGamePage(DuckHuntGame game)
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

            baloonSpace.Width = rectBackground.Width;
            baloonSpace.Y = 100;
            baloonSpace.Height = 150;

            ScroeBoardModel scoreBoard1 = new ScroeBoardModel();
            scoreBoardSpace.X = rectBackground.Left + 20;
            scoreBoardSpace.Y = rectBackground.Top + 10;
            scoreBoardSpace.Width = scoreBoard1.GetSpace().Width;
            scoreBoardSpace.Height = scoreBoard1.GetSpace().Height;


            TimeBoardModel timeBoard = new TimeBoardModel();
            if (rectBackground.Width < rectBackground.Height)
            {
                leftTimeBoardSpace.X = rectBackground.Height - timeBoard.GetSpace().Width - 20;
                leftTimeBoardSpace.Y = 20;
                leftTimeBoardSpace.Width = timeBoard.GetSpace().Width;
                leftTimeBoardSpace.Height = timeBoard.GetSpace().Height;
            }
            else
            {

                leftTimeBoardSpace.X = rectBackground.Width - timeBoard.GetSpace().Width - 20;
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
        }


        public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = new List<ModelObject>();
            if (phase == GAME_PHASE.SEEK_DUCK)
            {
                objlst.Add(dog);
                //objlst.Add(panda);
                //objlst.Add(levelUp);

            }
            else if (phase == GAME_PHASE.DUCK_FLY)
            {
                //objlst.Add(panda);
                objlst.Add(pause);
                if (levelUp.Show())
                {
                    objlst.Add(levelUp);
                }

                //objlst.Add(firework);
                if (parrot != null)
                {
                    objlst.Add(parrot);
                }
                if (baloon != null)
                {
                    objlst.Add(baloon);
                }
                if (plane != null)
                {
                    objlst.Add(plane);
                }
                //objlst.Add(plane);
                foreach (AnimalModel duck in duckList)
                {
                    objlst.Add(duck);
                }
                foreach (BulletModel bullet in bulletsList)
                {
                    objlst.Add(bullet);
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
            }
            List<ModelObject> backgroundobjlst;
            backgroundPage.GetObjects(out backgroundobjlst);
            foreach (ModelObject obj in backgroundobjlst)
            {
                objlst.Add(obj);
            }
        }
        GameChapter chapter;
        public void Update(GameTime gametime)
        {
            if (duckHuntGame.Pause)
            {
                return;
            }
            //
            backgroundPage.Update(gametime);
            //panda.Update(gametime);
            //firework.Update(gametime);
            if (parrot != null)
            {
                parrot.Update(gametime);
                if (parrot.Gone())
                {
                    parrot = null;
                }
            }
            if (baloon != null)
            {
                baloon.Update(gametime);
                if (baloon.Gone)
                {
                    baloon = null;
                }
            }

            if (plane != null)
            {
                plane.Update(gametime);
                if (plane.Gone)
                {
                    plane = null;
                }
            }

            if (phase == GAME_PHASE.GAME_SELECT)
            {
            }
            else if (phase == GAME_PHASE.SEEK_DUCK)
            {
                //
                dog.Update(gametime);
                if (dog.Gone/* || true*/)
                {
                    // show duck
                    phase = GAME_PHASE.DUCK_FLY;

                    // create two new duck
                    NewDuck();
                }
            }
            else if (phase == GAME_PHASE.DUCK_FLY)
            {
                if (gameMode == GameMode.GAME_TIME_LIMIT)
                {
                    leftTime.Update(gametime);
                }


                if (gameMode != GameMode.GAME_TIME_LIMIT)
                {

                    if (lostDuck.LostDuckCount >= 3)
                    {
                        duckList.Clear();

                        phase = GAME_PHASE.OVER;
                        // save new score

                        int score = scoreBoard.TotalScore;
                        int level = scoreBoard.GetLevel();
                        duckHuntGame.SaveNewScore(score, level);

                        duckHuntGame.GotoGameOverPage(); 
                        return;
                    }
                }
                else
                {
                    if (leftTime.LeftTime <= 0)
                    {
                        duckList.Clear();
                        phase = GAME_PHASE.OVER;
                        // save new score

                        int score = scoreBoard.TotalScore;
                        int level = scoreBoard.GetLevel();
                        duckHuntGame.SaveNewScore(score, level);

                        duckHuntGame.GotoGameOverPage();
                        return;
                    }
                }


                levelUp.Update(gametime);



                bool finished = true;
                int deadcount = 0;
                foreach (AnimalModel duck in duckList)
                {
                    duck.Update(gametime);
                    if (duck.Type() == ModelType.DUCK && !duck.Gone())
                    {
                        finished = false;
                    }
                    if (duck.Dead())
                    {
                        deadcount++;
                    }
                }

                for (int i = 0; i < bulletsList.Count; )
                {
                    bulletsList[i].Update(gametime);
                    if (bulletsList[i].Gone)
                    {
                        // remove it from this one 
                        bulletsList.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }

                if (finished)
                {
                    //phase = GAME_PHASE.DOG_SHOW;
                    //dog.ShowDog(deadcount);
                    NewDuck();
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
                        duckHuntGame.SaveNewScore(score, level);

                        duckHuntGame.GotoGameOverPage();
                    }
                }
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
                }
            }
        }

        void ShowBomb()
        {
            // show plane
            if (parrot == null)
            {
                parrot = new ParrotModel();

                parrot.Initialize(null, this.duckFlySpace, 0);
                parrot.StartPilot();
            }
        }

        void ShowPlane()
        {
            if (plane == null)
            {
                plane = new PlaneModel();
                DateTime now = DateTime.Now;
                plane.Initialize(null, this.baloonSpace, now.Millisecond);
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

        public void Click(List<Vector2> clickpositionlist)
        {

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

                BulletModel bullet = new BulletModel(clickpos);
                foreach (AnimalModel duck in duckList)
                {
                    duck.Shoot(bullet);
                }
                if (parrot != null)
                {
                    parrot.Shoot(bullet);
                }
                if (baloon != null)
                {
                    baloon.Shoot(bullet);
                }

                bullet.AdjustForFlyEffect();
                bulletsList.Add(bullet);

                if (bullet.GetShootDucks() != null)
                {
                    //
                    float score = 100;
                    bool showplane = false;
                    int shootduckcount = 0;
                    foreach (AnimalModel animal in bullet.GetShootDucks())
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
                        ShowBalloon();
                    }

                    ShowEastEgg();
                }

                if (bullet.GetBaloon() != null)
                {
                    // show award
                    AddBonusDuck(clickpos);
                    baloon = null;
                }

                /*
                if (bullet.GetParrot() != null)
                {
                    // show award
                    //AddBonusDuck(clickpos);
                    scoreBoard.AddScore(-500 - showbaloonCount*10);
                }
                 */

            }
        }



        /////////////////////////////////////////////////////

        void NewDog()
        {
            // dog seek duck
            dog = new DogModel();
            dog.Initialize(null, dogRunSpace, 0);
            dog.StartPilot();

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

        void NewScoreBoard()
        {
            scoreBoard = new ScroeBoardModel();
            scoreBoard.Initialize(null, scoreBoardSpace, 0);

            pause = new ButtonModel();
            pause.Initialize(null, pauseButtonSpace, 0);

            levelUp = new LevelUpBoardModel();
            levelUp.Initialize(null, levelUpSpace, 0);

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

                        }
                    }
                    if (lostDuck.LostDuckCount >= 3)
                    {
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
                    }

                    if (leftTime.LeftTime <= 0)
                    {
                        duckList.Clear();
                        return;
                    }
                }
                duckList.Clear();
            }

            List<AnimalModel> ducks = null;

            do
            {
                if (chapter != null && chapter.GetDuckList(out ducks) && ducks != null && ducks.Count > 0)
                {
                    break;
                }
                gameChapterMgr.GetNextChapter(out chapter);
                ShowLevelUp();
            } while (chapter != null);

            if (ducks == null)
            {
                return;
            }

            int i = 0;
            DateTime now = System.DateTime.Now;
            foreach (AnimalModel duck in ducks)
            {
                int s = now.Hour * 60 * 60 + now.Minute * 60 + now.Second;
                duck.Initialize(null, duckFlySpace, s + (i++) * 7);
                duck.StartPilot();
                duckList.Add(duck);
            }
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
            if (batchcount == 3)
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
                    duck.StartPilot(startPos);
                    duck.SetStartPos(startPos);
                    duck.SetEndPos(endpos);
                    duck.SetShowTime(showTime);
                    duckList.Add(duck);

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
            NewDog();
            if (duckList != null)
            {
                duckList.Clear();
            }

            gameMode = gameMode1;
            gameChapterMgr.Init(gameMode);
            leftTime.SetTime(2 * 60);
            lostDuck.ResetLostCount();

            gameChapterMgr.GetNextChapter(out chapter);
            phase = GAME_PHASE.SEEK_DUCK;


        }
    }

    class GameMainMenuPage : GamePage
    {
        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //

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
        public void InitGamePage(DuckHuntGame game)
        {
            duckHuntGame = game;
            backgroundPage = game.GetBackgroundPage();

            Rectangle rectBackground = game.GetGlobalViewRect();


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


            NewMenu();
        }

        public void GetObjects(out List<ModelObject> objlst)
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
        public void Update(GameTime gametime)
        {
            backgroundPage.Update(gametime);
            menuTimeModelItem.Update(gametime);
            menuFreeModelItem.Update(gametime);
            menuOptionItem.Update(gametime);
            menuScoreListItem.Update(gametime);

        }


        public void Click(List<Vector2> clickpositionlist)
        {
            //
            backgroundPage.Click(clickpositionlist);

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
    }



    class GameOverPage : GamePage
    {
        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //

        TitleItemModel gameOverTitleItem;
        MenuItemModel menuRetryItem;
        MenuItemModel menuReturnItem;
        MenuItemModel menuExitItem;
        MenuItemModel menuScoreListItem;

        Rectangle gameOverTitleSpace;
        Rectangle retryMenuSpace;
        Rectangle returnMenuSpace;
        Rectangle exitMenuSpace;
        Rectangle scoreListMenuSpace;

        GameBackGroundPage backgroundPage = null;
        DuckHuntGame duckHuntGame = null;
        public void InitGamePage(DuckHuntGame game)
        {
            duckHuntGame = game;
            backgroundPage = game.GetBackgroundPage();

            Rectangle rectBackground = game.GetGlobalViewRect();


            TitleItemModel titleItem = new TitleItemModel();
            gameOverTitleSpace.X = (rectBackground.Width - titleItem.GetSpace().Width) / 2;
            gameOverTitleSpace.Y = 50;
            gameOverTitleSpace.Width = titleItem.GetSpace().Width;
            gameOverTitleSpace.Height = titleItem.GetSpace().Height;

            MenuItemModel menuItem = new MenuItemModel();
            retryMenuSpace.X = 700;
            retryMenuSpace.Y = 200;
            retryMenuSpace.Width = menuItem.GetSpace().Width;
            retryMenuSpace.Height = menuItem.GetSpace().Height;

            returnMenuSpace = retryMenuSpace;
            returnMenuSpace.X = 400;
            returnMenuSpace.Y = 310;

            exitMenuSpace = returnMenuSpace;
            exitMenuSpace.X = 1200;
            exitMenuSpace.Y = 300;

            scoreListMenuSpace = exitMenuSpace;
            scoreListMenuSpace.X = 1000;
            scoreListMenuSpace.Y = rectBackground.Top + 400;


            NewMenu();
        }

        public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = new List<ModelObject>();

            objlst.Add(gameOverTitleItem);
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
        public void Update(GameTime gametime)
        {
            backgroundPage.Update(gametime);
            gameOverTitleItem.Update(gametime);
            menuRetryItem.Update(gametime);
            menuReturnItem.Update(gametime);
            menuExitItem.Update(gametime);
            menuScoreListItem.Update(gametime);

        }


        public void Click(List<Vector2> clickpositionlist)
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
        public void InitGamePage(DuckHuntGame game)
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
        }

        public void GetObjects(out List<ModelObject> objlst)
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
        public void Update(GameTime gametime)
        {
            //backgroundPage.Update(gametime);
        }


        public void Click(List<Vector2> clickpositionlist)
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

        public void InitGamePage(DuckHuntGame game)
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

            returnMenuItem.Initialize(null, returnMenuSpace, 0);

        }
        public void GetObjects(out List<ModelObject> objlst)
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
        public void Update(GameTime gametime)
        {
            backgroundPage.Update(gametime);
            scoreListBoard.Update(gametime);
            returnMenuItem.Update(gametime);

            // 
        }
        public void Click(List<Vector2> clickpositionlist)
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

        public void InitGamePage(DuckHuntGame game)
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
        public void GetObjects(out List<ModelObject> objlst)
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
        public void Update(GameTime gametime)
        {
            parentPage.Update(gametime);
            keyboard.Update(gametime);
            //scoreListBoard.Update(gametime);

            // 
        }
        public void Click(List<Vector2> clickpositionlist)
        {
            // check return button
            //keyboard.cl
        }


        /////
    }


    class GameHelpPage : GamePage
    {
        public void InitGamePage(DuckHuntGame game)
        {
        }
        public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = null;
        }
        public void Update(GameTime gametime)
        {
        }
        public void Click(List<Vector2> clickpositionlist)
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

        public void InitGamePage(DuckHuntGame game)
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


        }
        public void GetObjects(out List<ModelObject> objlst)
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
        public void Update(GameTime gametime)
        {
            backgroundMusic.Update(gametime);
            gameSound.Update(gametime);
            returnMenuItem.Update(gametime);

            this.backgroundPage.Update(gametime);
        }
        public void Click(List<Vector2> clickpositionlist)
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
        }

        public void InitGamePage(DuckHuntGame game)
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
        }

        public void GetObjects(out List<ModelObject> objlst)
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
        public void Update(GameTime gametime)
        {
            cloud.Update(gametime);
        }
        public void Click(List<Vector2> clickpositionlist)
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

        // score list
        Dictionary<int, string> scorelist;
        Dictionary<int, string> levellist;
        public Dictionary<int, string> ScoreList
        {
            get
            {
                return scorelist;
            }
        }
        
        public Dictionary<int, string> LevelList
        {
            get
            {
                return levellist;
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

        public GameData()
        {
            scorelist = new Dictionary<int, string>();
            levellist = new Dictionary<int, string>();
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
            /*
            System.IO.Stream stream = TitleContainer.OpenStream(filename);
            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
            {
                content = reader.ReadToEnd();
            }
             */

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
        }

        private void SaveGameData(ref string content)
        {
            // <?xml version='1.0'?>
            // <DuckHunt>
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
            // </DuckHunt>

            content += "<?xml version='1.0'?>";
            content += "<DuckHunt>";
            SaveScoreList(ref content);
            SaveLevelList(ref content);

            // could save other configuration
            SaveGameConfig(ref content);
            content += "</DuckHunt>";
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
                            if (reader.Name == "scorelist")
                            {
                                // find score list element
                                LoadScoreList(reader);
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

    }

}