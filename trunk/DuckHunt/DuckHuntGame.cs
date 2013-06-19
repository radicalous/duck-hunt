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
        Rectangle globalViewRect = new Rectangle(0, 0, 1600, 900);

        GameData gameData;

        public GameData DuckHuntGameData
        {
            get
            {
                return gameData;
            }
        }

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
        GamePlayPage playPage;
        GameScoreListPage scoreListPage;
        GameConfigPage optionPage;

        GamePage currentPage = null;

        List<GamePage> pagestack;

        public DuckHuntGame()
        {
            gameData = new GameData();

            pagestack = new List<GamePage>();

            backgroundPage = new GameBackGroundPage();
            mainMenuPage = new GameMainMenuPage();
            playPage = new GamePlayPage();
            scoreListPage = new GameScoreListPage();
            optionPage = new GameConfigPage();

            currentPage = mainMenuPage;
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

        public void GotoPlayPage()
        {
            currentPage = playPage;
        }
        public void GotoConfigPage()
        {
            pagestack.Add(currentPage);
            currentPage = optionPage;

        }

        public void GotoGameOverPage()
        {
            pagestack.Add(currentPage);
            //currentPage = optionPage;

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
            playPage = new GamePlayPage();
            playPage.InitGamePage(this);

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
            scoreListPage.InitGamePage(this);
            optionPage.InitGamePage(this);


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

        public void SaveNewScore(int score)
        {
            // 
            DateTime now = DateTime.Now;
            gameData.AddScore(now.ToString(), score);
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
        abstract public bool GetDuckList(out List<DuckModel> ducks);
        abstract public bool CanBeRemoved();
    }


    class GameChapter1 : GameChapter
    {
        int duckcount = 0;
        override public bool GetDuckList(out List<DuckModel> ducks)
        {
            ducks = null;
            ducks = new List<DuckModel>();

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

    class GameChapter2 : GameChapter
    {
        int duckcount = 0;
        override public bool GetDuckList(out List<DuckModel> ducks)
        {
            ducks = null;
            ducks = new List<DuckModel>();

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

    class GameChapter3 : GameChapter
    {
        int duckcount = 0;
        override public bool GetDuckList(out List<DuckModel> ducks)
        {
            ducks = null;
            ducks = new List<DuckModel>();

            DuckModel duck;
            if (duckcount >= 8)
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

    class GameChapter4 : GameChapter
    {
        int duckcount = 0;
        override public bool GetDuckList(out List<DuckModel> ducks)
        {
            ducks = null;
            ducks = new List<DuckModel>();

            DuckModel duck;
            if (duckcount >= 8)
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

    class GameChapter5 : GameChapter
    {
        int duckcount = 0;
        override public bool GetDuckList(out List<DuckModel> ducks)
        {
            ducks = null;
            ducks = new List<DuckModel>();

            DuckModel duck;
            if (duckcount >= 9)
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

            return true;
        }

        public override bool CanBeRemoved()
        {
            if (duckcount >= 9)
            {
                return true;
            }
            return false;
        }
    }

    class GameChapter6 : GameChapter
    {
        int duckcount = 0;
        override public bool GetDuckList(out List<DuckModel> ducks)
        {
            ducks = null;
            ducks = new List<DuckModel>();

            DuckModel duck;
            if (duckcount >= 12)
            {
                ducks = null;
                return false;
            }

            string name = "chapter6_" + duckcount.ToString();


            duck = new DuckModel(PilotType.DUCKSIN, name);
            ducks.Add(duck);
            duckcount++;

            duck = new DuckModel(PilotType.DUCKEIGHT, name);
            ducks.Add(duck);
            duckcount++;

            duck = new DuckModel(PilotType.DUCKELLIPSE, name);
            ducks.Add(duck);
            duckcount++;

            duck = new DuckModel(PilotType.DUCKCIRCLE, name);
            ducks.Add(duck);
            duckcount++;

            return true;
        }

        public override bool CanBeRemoved()
        {
            if (duckcount >= 12)
            {
                return true;
            }
            return false;
        }
    }

    class GameChapter7 : GameChapter
    {
        int duckcount = 0;
        override public bool GetDuckList(out List<DuckModel> ducks)
        {
            ducks = null;
            ducks = new List<DuckModel>();

            DuckModel duck;
            if (duckcount >= 15)
            {
                ducks = null;
                return false;
            }

            string name = "chapter7_" + duckcount.ToString();

            duck = new DuckModel(PilotType.DUCKSIN, name);
            ducks.Add(duck);
            duckcount++;

            duck = new DuckModel(PilotType.DUCKEIGHT, name);
            ducks.Add(duck);
            duckcount++;

            duck = new DuckModel(PilotType.DUCKELLIPSE, name);
            ducks.Add(duck);
            duckcount++;

            duck = new DuckModel(PilotType.DUCKCIRCLE, name);
            ducks.Add(duck);
            duckcount++;

            duck = new DuckModel(PilotType.DUCKNORMAL, name);
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


    class GameChapter8 : GameChapter
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
        override public bool GetDuckList(out List<DuckModel> ducks)
        {
            ducks = null;
            ducks = new List<DuckModel>();

            DuckModel duck;
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
                ducks.Add(duck);
                duckcount++;
            }
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

    class GameChapter9 : GameChapter
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
        override public bool GetDuckList(out List<DuckModel> ducks)
        {
            ducks = null;
            ducks = new List<DuckModel>();

            DuckModel duck;
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
                ducks.Add(duck);
                duckcount++;
                pilottypeindex++;
            }

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


    class GameChapter10 : GameChapter
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
        override public bool GetDuckList(out List<DuckModel> ducks)
        {
            ducks = null;
            ducks = new List<DuckModel>();

            DuckModel duck;
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
                ducks.Add(duck);
                duckcount++;
                pilottypeindex++;
            }

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


    class GameChapterFunShowCurve : GameChapter
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
        override public bool GetDuckList(out List<DuckModel> ducks)
        {
            ducks = null;
            ducks = new List<DuckModel>();

            DuckModel duck;
            if (pilottypeindex >= pilotypelist.Count)
            {
                ducks = null;
                return false;
            }

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


    class GameChapterForever : GameChapter
    {
        int duckcount = 0;
        List<PilotType> pilotypelist;
        int concurrentduck = 8;
        //int concurrentduck = 100;
        //int duckstyle = 8;
        int duckstyle = 1;
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
        override public bool GetDuckList(out List<DuckModel> ducks)
        {
            ducks = null;
            ducks = new List<DuckModel>();

            DuckModel duck;

            string name = "chapterforever_" + concurrentduck.ToString();
            for (int i = 0; i < concurrentduck; i++)
            {
                name = "chapterforever_" + concurrentduck.ToString();
                int pilottypeindex = i % duckstyle;
               
                duck = new DuckModel(pilotypelist[pilottypeindex], name);
                ducks.Add(duck);
                duckcount++;
            }

            concurrentduck++;

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
        public GameChapterManager()
        {

        }

        public bool Init(GameMode mode)
        {
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

                chapter = new GameChapterFunShowCurve();
                chapters.Add(chapter);

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
            }
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
        List<DuckModel> duckList;
        Rectangle duckFlySpace;

        DogModel dog;
        Rectangle dogRunSpace;

        PandaModel panda;
        PlaneModel plane;
        BaloonModel baloon;
        Rectangle planeSpace;


        HitBoardModel hitBoard;
        Rectangle hitBoardSpace;

        ScroeBoardModel scoreBoard;
        Rectangle scoreBoardSpace;

        ButtonModel pause;
        Rectangle pauseButtonSpace;


        //FireworkModel firework;
        Rectangle fireworkSpace;


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

            planeSpace.Width = rectBackground.Width;
            planeSpace.Y = 100;
            planeSpace.Height = 150;

            HitBoardModel hitBoard1 = new HitBoardModel();
            if (rectBackground.Width < rectBackground.Height)
            {
                hitBoardSpace.X = rectBackground.Top + 20;
                hitBoardSpace.Y = rectBackground.Left + 10;
                hitBoardSpace.Width = hitBoard1.GetSpace().Width;
                hitBoardSpace.Height = hitBoard1.GetSpace().Height;
            }
            else
            {
                hitBoardSpace.X = rectBackground.Left + 20;
                hitBoardSpace.Y = rectBackground.Top + 10;
                hitBoardSpace.Width = hitBoard1.GetSpace().Width;
                hitBoardSpace.Height = hitBoard1.GetSpace().Height;
            }

            ScroeBoardModel scoreBoard1 = new ScroeBoardModel();
            if (rectBackground.Width < rectBackground.Height)
            {
                scoreBoardSpace.X = rectBackground.Top + 20;
                scoreBoardSpace.Y = rectBackground.Left + 80;
                scoreBoardSpace.Width = scoreBoard1.GetSpace().Width;
                scoreBoardSpace.Height = scoreBoard1.GetSpace().Height;
            }
            else
            {
                scoreBoardSpace.X = rectBackground.Left + 20;
                scoreBoardSpace.Y = rectBackground.Top + 80;
                scoreBoardSpace.Width = scoreBoard1.GetSpace().Width;
                scoreBoardSpace.Height = scoreBoard1.GetSpace().Height;
            }


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


            NewDog();
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


            }
            else if (phase == GAME_PHASE.DUCK_FLY)
            {
                //objlst.Add(panda);
                objlst.Add(pause);
                //objlst.Add(firework);
                if (plane != null)
                {
                    objlst.Add(plane);
                }
                if (baloon != null)
                {
                    objlst.Add(baloon);
                }
                //objlst.Add(plane);
                foreach (DuckModel duck in duckList)
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
            if (plane != null)
            {
                plane.Update(gametime);
                if (plane.Gone)
                {
                    plane = null;
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

            if (phase == GAME_PHASE.GAME_SELECT)
            {
            }
            else if (phase == GAME_PHASE.SEEK_DUCK)
            {
                //
                dog.Update(gametime);
                if (dog.Gone || true)
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

                bool finished = true;
                int deadcount = 0;
                foreach (DuckModel duck in duckList)
                {
                    duck.Update(gametime);
                    if (!duck.Gone)
                    {
                        finished = false;
                    }
                    if (duck.dead)
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
                        duckHuntGame.SaveNewScore(score);

                        duckHuntGame.GotoMainMenuPage();
                    }
                }
            }
        }

        int previousTotalScore = 0;
        int previousHitCount = 0;
        void ShowEastEgg()
        {
            if (previousTotalScore < 1000 && scoreBoard.TotalScore >= 1000)
            {
                // show plane
                if (plane == null)
                {
                    plane = new PlaneModel();

                    plane.Initialize(null, this.duckFlySpace, 0);
                }
            }

            if (hitBoard.HitCount - previousHitCount > 20)
            {
                // show baloon
                previousHitCount = hitBoard.HitCount;
                if (baloon == null)
                {
                    baloon = new BaloonModel();
                    baloon.Initialize(null, planeSpace, 0);
                }

            }


            previousTotalScore = scoreBoard.TotalScore;
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
                    duckHuntGame.Pause = !duckHuntGame.Pause;
                    continue;
                }

                if (duckHuntGame.Pause)
                {
                    continue;
                }

                BulletModel bullet = new BulletModel(clickpos);
                foreach (DuckModel duck in duckList)
                {
                    duck.Shoot(bullet);
                }
                if (plane != null)
                {
                    plane.Shoot(bullet);
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
                    for (int i = 0; i < bullet.GetShootDucks().Count; i++)
                    {
                        score = 100;
                        score *= (i + 1);
                        score /= bullet.GetShootDucks()[i].GetSacle();
                        scoreBoard.AddScore((int)score);

                    }

                    int ii = 0;
                    foreach (DuckModel duck2 in duckList)
                    {
                        if (duck2.dead)
                        {
                            hitBoard.AddHitCount(1);
                        }
                        ii++;
                    }

                    ShowEastEgg();
                }

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

        int flycount = 0;

        GameMode gameMode = GameMode.GAME_TIME_LIMIT;
        void NewDuck()
        {
            if (duckList == null)
            {
                duckList = new List<DuckModel>();
            }
            else
            {
                // set duck state
                int ii = 0;

                if (gameMode != GameMode.GAME_TIME_LIMIT)
                {
                    foreach (DuckModel duck2 in duckList)
                    {
                        if (!duck2.dead)
                        {
                            //flycount++;
                            lostDuck.AddDuck(1);
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
                    if (leftTime.LeftTime <= 0)
                    {
                        duckList.Clear();
                        return;
                    }
                }
                duckList.Clear();
            }

            List<DuckModel> ducks = null;

            do
            {
                if (chapter != null && chapter.GetDuckList(out ducks) && ducks != null && ducks.Count > 0)
                {
                    break;
                }
                gameChapterMgr.GetNextChapter(out chapter);
            } while (chapter != null);

            if (ducks == null)
            {
                return;
            }

            int i = 0;
            DateTime now = System.DateTime.Now;
            foreach (DuckModel duck in ducks)
            {
                int s = now.Hour * 60 * 60 + now.Minute * 60 + now.Second;
                duck.Initialize(null, duckFlySpace, s + (i++) * 7);
                duck.StartPilot();
                duckList.Add(duck);

            }
        }

        public void NewGame(GameMode gameMode1)
        {
            gameMode = gameMode1;
            gameChapterMgr.Init(gameMode);
            leftTime.SetTime(2 * 60);
            lostDuck.ResetLostCount();
            phase = GAME_PHASE.SEEK_DUCK;
            //phase = GAME_PHASE.DUCK_FLY;
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
            if (rectBackground.Width < rectBackground.Height)
            {
                timeModelMenuSpace.X = rectBackground.Height - 150;
                timeModelMenuSpace.Y = rectBackground.Width - 250;
                timeModelMenuSpace.Width = menuItem.GetSpace().Width;
                timeModelMenuSpace.Height = menuItem.GetSpace().Height;

                freeModelModelMenuSpace = timeModelMenuSpace;
                freeModelModelMenuSpace.Y += 100;

                optionMenuSpace = freeModelModelMenuSpace;
                optionMenuSpace.Y += 100;

                scoreListMenuSpace = optionMenuSpace;
                scoreListMenuSpace.Y += 100;
            }
            else
            {
                timeModelMenuSpace.X = 250;
                timeModelMenuSpace.Y = 100;
                timeModelMenuSpace.Width = menuItem.GetSpace().Width;
                timeModelMenuSpace.Height = menuItem.GetSpace().Height;

                freeModelModelMenuSpace = timeModelMenuSpace;
                freeModelModelMenuSpace.X = 400;
                freeModelModelMenuSpace.Y = 280;

                optionMenuSpace = freeModelModelMenuSpace;
                optionMenuSpace.X = 1000;
                optionMenuSpace.Y = 200;

                scoreListMenuSpace = optionMenuSpace;
                scoreListMenuSpace.X = rectBackground.Width / 2;
                scoreListMenuSpace.Y = rectBackground.Top + 300;

            }

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
                scoreListBoardSpace.Y = rectBackground.Top + 100;
                scoreListBoardSpace.Width = scoreListBoard1.GetSpace().Width;
                scoreListBoardSpace.Height = scoreListBoard1.GetSpace().Height;

                returnMenuSpace.X = 150;
                returnMenuSpace.Y = 150;
                returnMenuSpace.Width = menuItm1.GetSpace().Width;
                returnMenuSpace.Height = menuItm1.GetSpace().Height;

            }


            scoreListBoard.Initialize(null, scoreListBoardSpace, 0);
            scoreListBoard.ScoreList = game.DuckHuntGameData.ScoreList;

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

            // 
        }
        public void Click(List<Vector2> clickpositionlist)
        {
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

    class GameOverPage : GamePage
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

            this.backgroundPage.Update(gametime);
        }
        public void Click(List<Vector2> clickpositionlist)
        {
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
        CloudModel cloud;
        GrassModel grass;
        ForegroundGrassModel forground;

        public void InitGamePage(DuckHuntGame game)
        {
            rectBackground = game.GetGlobalViewRect();


            // calculate background settings
            NewBackground();
        }

        public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = new List<ModelObject>();
            objlst.Add(blueSky);
            objlst.Add(cloud);
            objlst.Add(grass);
            objlst.Add(forground);

        }
        public void Update(GameTime gametime)
        {
            cloud.Update(gametime);
        }
        public void Click(List<Vector2> clickpositionlist)
        {
        }


        //////
        void NewBackground()
        {
            // dog seek duck
            blueSky = new SkyModel();
            blueSky.Initialize(null, rectBackground, 0);

            cloud = new CloudModel();
            cloud.Initialize(null, rectBackground, 0);

            grass = new GrassModel();
            grass.Initialize(null, rectBackground, 0);

            forground = new ForegroundGrassModel();
            forground.Initialize(null, rectBackground, 0);
        }
    }

    class GameData
    {
        //SortedSet<KeyValuePair<string, int>> scorelist;

        // configuration
        public bool EnableBgMusic = false;
        public bool EnableGameSound = false;

        // score list
        Dictionary<int, string> scorelist;
        public Dictionary<int, string> ScoreList
        {
            get
            {
                return scorelist;
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
                if (i > 10)
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

        public GameData()
        {
            scorelist = new Dictionary<int, string>();
            var key = scorelist.OrderByDescending(c => c.Key);
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
            // </DuckHunt>

            content += "<?xml version='1.0'?>";
            content += "<DuckHunt>";
            SaveScoreList(ref content);

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