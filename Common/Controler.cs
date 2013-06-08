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
    class ObjectTexturesItem
    {
        public ModelType objType;
        public List<Texture2D> textureList;
        public List<SpriteFont> fontList;
        public List<SoundEffect> soundList;
        
    }

    class ViewObjectFactory
    {
        static Vector2 s_playgroundOrgPoint;
        static float s_playgroundDefScale;
        static Vector2 s_backgroundOrgPoint;
        static float s_backgroundDefScale;
        static Rectangle s_localViewRect;

        public static void SetLocalViewInfo(Rectangle localViewRc, Vector2 playgroundOrgPoint, float playgroundDefScale, Vector2 backgroundOrgPoint, float backgroundDefScale)
        {
            s_playgroundOrgPoint = playgroundOrgPoint;
            s_playgroundDefScale = playgroundDefScale;
            s_backgroundOrgPoint = backgroundOrgPoint;
            s_backgroundDefScale = backgroundDefScale;
            s_localViewRect = localViewRc;
        }

        public static ViewObject CreateViewObject(ModelObject model)
        {
            CommonViewObject commViewObj = null;
            ScoreBoardViewObject scoreboardObj = null;
            HitBoardViewObject hitBoardObj = null;
            ScoreListBoardViewObject scorelistobj = null;
            ViewObject viewObject = null;
            switch (model.Type())
            {
                case ModelType.SKY:
                case ModelType.CLOUD:
                case ModelType.GRASS:
                case ModelType.FORGROUND:
                    {
                        commViewObj = new CommonViewObject(model, s_backgroundOrgPoint, s_backgroundDefScale);
                    }
                    break;
                case ModelType.PANDA:
                case ModelType.DOG:
                case ModelType.DUCK:
                case ModelType.BULLET:
                    {
                        commViewObj = new CommonViewObject(model, s_playgroundOrgPoint, s_playgroundDefScale);
                    }
                    break;
                case ModelType.HITBOARD:
                    {
                        hitBoardObj = new HitBoardViewObject(model);
                    }
                    break;
                case ModelType.SCORELISTBOARD:
                    {
                        scorelistobj = new ScoreListBoardViewObject(model);
                    }
                    break;
                case ModelType.DUCKICON:
                    {
                        commViewObj = new CommonViewObject(model, s_playgroundOrgPoint, s_playgroundDefScale);
                    }
                    break;
                case ModelType.BULLETBOARD:
                    {
                        commViewObj = new CommonViewObject(model, s_playgroundOrgPoint, s_playgroundDefScale);
                    }
                    break;
                case ModelType.BUTTON:
                    {
                        commViewObj = new CommonViewObject(model, s_playgroundOrgPoint, s_playgroundDefScale);
                    }
                    break;
                case ModelType.KEYITEM:
                    {
                        //Vector2 ogpoint = Vector2.Zero;
                        viewObject = new KeyItemViewObject(model/*, Vector2.Zero, 1.0f*/);
                    }
                    break;
                case ModelType.KEYBORD:
                    {
                        viewObject = new KeyboardViewObject(model, Vector2.Zero, 1.0f);
                    }
                    break;
                case ModelType.BULLETICON:
                    {
                        commViewObj = new CommonViewObject(model, s_playgroundOrgPoint, s_playgroundDefScale);
                    }
                    break;
                case ModelType.SCOREBOARD:
                    {
                        scoreboardObj = new ScoreBoardViewObject(model);
                    }
                    break;
                case ModelType.MENUITEM:
                    {
                        viewObject = new MenuItemViewObject(model);
                    }
                    break;
                case ModelType.TIMEBOARD:
                    {
                        viewObject = new TimeBoardViewObject(model);
                    }
                    break;
                case ModelType.LOSTDUCKBOARD:
                    {
                        viewObject = new LostDuckBoardViewObject(model);
                    }
                    break;
                case ModelType.CHECKBOX:
                    {
                        viewObject = new CheckBoxViewObject(model);
                    }
                    break;
            }
            if (commViewObj != null)
            {
                commViewObj.screenRc = s_localViewRect;
                viewObject = commViewObj;
            }
            if (scoreboardObj != null)
            {
                viewObject = scoreboardObj;
            }
            if( hitBoardObj != null )
            {
                viewObject = hitBoardObj;
            }
            if (scorelistobj != null)
            {
                viewObject = scorelistobj;
            }
            return viewObject;
        }
    }

    class DuckHuntGameControler
    {
        ContentManager Content = null;
        DuckHuntGame game = null;
        Rectangle viewRect = new Rectangle();

        Dictionary<ModelType, ObjectTexturesItem> objTextureLst;

        bool pause = false;

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public void Initialize(Rectangle viewScene)
        {
            game = new DuckHuntGame();
            viewRect = viewScene;
            objTextureLst = new Dictionary<ModelType, ObjectTexturesItem>();


        }

        Song gamebackgorundsound;

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public void LoadContent(ContentManager Content1)
        {
            // TODO: use this.Content to load your game content here
            Content = Content1;
            LoadResources();

            List<GameSound> soundList = game.GetSoundList();
            gamebackgorundsound = Content.Load<Song>(soundList[0].soundpath);
            MediaPlayer.IsRepeating = true;
            //MediaPlayer.Volume = 50;
            MediaPlayer.Play(gamebackgorundsound);

            game.StartGame(viewRect);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        public void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here


        }


        public void LoadResources()
        {
            List<ObjectResourceItem> resourceList;
            game.GetResources(out resourceList);
            if (resourceList != null)
            {
                foreach (ObjectResourceItem objResourceItm in resourceList)
                {
                    ObjectTexturesItem textureItm;
                    textureItm = new ObjectTexturesItem();
                    textureItm.objType = objResourceItm.objType;
                    textureItm.textureList = new List<Texture2D>();
                    textureItm.fontList = new List<SpriteFont>();
                    textureItm.soundList = new List<SoundEffect>();

                    foreach (ResourceItem resourceItm in objResourceItm.resourceList)
                    {
                        if (resourceItm.type == ResourceType.TEXTURE)
                        {
                            textureItm.textureList.Add(Content.Load<Texture2D>(resourceItm.path));
                        }
                        else if (resourceItm.type == ResourceType.FONT)
                        {
                            textureItm.fontList.Add(Content.Load<SpriteFont>(resourceItm.path));
                        }
                        else if (resourceItm.type == ResourceType.SOUND)
                        {
                            textureItm.soundList.Add(Content.Load<SoundEffect>(resourceItm.path));
                        }
                    }
                    objTextureLst[textureItm.objType] = textureItm;
                }
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            game.Update(gameTime);
          
            if (pause)
            {
                pause = game.Pause;
                return;
            }
            pause = game.Pause;

            List<ModelObject> objlst = null;
            game.GetObjects(out objlst);
            if (objlst == null)
            {
                return;
            }

            foreach (ModelObject obj in objlst)
            {
                ViewObject viewObject = obj.GetViewObject();
                if (viewObject == null)
                {
                    //viewObject = ViewObjectFactory.CreateViewObject(this.viewRect, obj, game.orgpoint, game.defscale);
                    Vector2 bgorgpoint = game.GetBgOrgPointInLocalView();
                    float bgdefscale = game.GetLogicBgDefScale();
                    viewObject = ViewObjectFactory.CreateViewObject(obj);

                    viewObject.Init(game.orgpoint, game.defscale, obj, objTextureLst, obj.GetSpace());
                    obj.SetViewObject(viewObject);
                }
                viewObject.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // TODO: Add your drawing code here
            List<ModelObject> objlst = null;
            game.GetObjects(out objlst);
            if (objlst == null)
            {
                return;
            }

            foreach (ModelObject obj in objlst)
            {
                ViewObject viewObject = obj.GetViewObject();
                if (viewObject == null)
                {
                    Vector2 bgorgpoint = game.GetBgOrgPointInLocalView();
                    float bgdefscale = game.GetLogicBgDefScale();
                    viewObject = ViewObjectFactory.CreateViewObject(obj);
                    viewObject.Init(game.orgpoint, game.defscale, obj, objTextureLst, obj.GetSpace());
                    obj.SetViewObject(viewObject);
                    obj.Update(gameTime);
                }
                viewObject.Draw(spriteBatch);
            }
        }


        public void Click(List<Vector2> clickPositions)
        {
            //
            // local rect, global rect
            // (local rect - orgpoint ) = global rect * default scale
            // local rect = orgpoint + global rect * default scale
            // global rect = (local rect - orgpoint)/ defalult scale
            //
            //Vector2 globalshotpos = (shootPosition - game.orgpoint) / game.defscale;
            List<Vector2> globalpointposlst = new List<Vector2>();
            foreach (Vector2 localclickpos in clickPositions)
            {
                globalpointposlst.Add((localclickpos - game.orgpoint) / game.defscale);
            }
            game.Click(globalpointposlst);
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



    enum GAME_PHASE { GAME_SELECT, SEEK_DUCK, DUCK_FLY, DOG_SHOW,SCORELIST_SHOW, OVER };
    class GameSound
    {
        public GAME_PHASE phase;
        public string soundpath;
    }

    enum GameMode { GAME_TIME_LIMIT, GAME_FREE_MODE };

    enum GameChapterPhase { CHAPTER1, CHAPTER2, CHAPTER3, CHAPTER4, CHAPTER5, FOREVER };


    class GameChapter
    {
        GameChapterPhase chapter;
        int duckcount = 0;
        int concurrentcount = 6;
        public GameChapter(GameChapterPhase chapter1)
        {
            chapter = chapter1;
        }

        public bool GetDuckList(out List<DuckModel> ducks)
        {
            ducks = new List<DuckModel>();
            DuckModel duck;
            switch (chapter)
            {
                case GameChapterPhase.CHAPTER1:
                    {
                        if (duckcount >= 5)
                        {
                            ducks = null;
                            return false;
                        }
                        duck = new DuckModel(PilotType.DUCKEIGHT, "charpter1");
                        ducks.Add(duck);
                        duckcount++;
                    }
                    break;
                case GameChapterPhase.CHAPTER2:
                    {
                        if (duckcount >= 10)
                        {
                            ducks = null;
                            return false;
                        }
                        duck = new DuckModel(PilotType.DUCKEIGHT, "charpter2");
                        ducks.Add(duck);
                        duck = new DuckModel(PilotType.DUCKEIGHT, "chapter2");
                        ducks.Add(duck);
                        duckcount += 2;
                    }
                    break;
                case GameChapterPhase.CHAPTER3:
                    {
                        if (duckcount >= 15)
                        {
                            ducks = null;
                            return false;
                        }
                        duck = new DuckModel(/*PilotType.DUCKEIGHT, "chapter3"*/);
                        ducks.Add(duck);
                        duck = new DuckModel(/*PilotType.DUCKEIGHT, "chapter3"*/);
                        ducks.Add(duck);
                        duck = new DuckModel(PilotType.DUCKEIGHT, "chapter3");
                        ducks.Add(duck);
                        duckcount += 3;
                    }
                    break;
                case GameChapterPhase.CHAPTER4:
                    {
                        if (duckcount >= 20)
                        {
                            ducks = null;
                            return false;
                        }
                        duck = new DuckModel(/*PilotType.DUCKEIGHT, "chapter4"*/);
                        ducks.Add(duck);
                        duck = new DuckModel(/*PilotType.DUCKEIGHT, "chapter4"*/);
                        ducks.Add(duck);
                        duck = new DuckModel(/*PilotType.DUCKEIGHT, "chapter4"*/);
                        ducks.Add(duck);
                        duck = new DuckModel(PilotType.DUCKEIGHT, "chapter4");
                        ducks.Add(duck);
                        duckcount += 4;
                    }
                    break;
                case GameChapterPhase.CHAPTER5:
                    {
                        if (duckcount >= 25)
                        {
                            ducks = null;
                            return false;
                        }
                        duck = new DuckModel();
                        ducks.Add(duck);
                        duck = new DuckModel();
                        ducks.Add(duck);
                        duck = new DuckModel();
                        ducks.Add(duck);
                        duck = new DuckModel();
                        ducks.Add(duck);
                        duck = new DuckModel();
                        ducks.Add(duck);
                        duckcount += 5;
                    }
                    break;
                case GameChapterPhase.FOREVER:
                    {
                        for (int i = 0; i < concurrentcount; i++)
                        {
                            duck = new DuckModel(/*PilotType.DUCKEIGHT, "forever"*/);
                            ducks.Add(duck);
                            duckcount += 1;
                        }
                        concurrentcount++;
                    }
                    break;
            }

            return true;
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
                chapter = new GameChapter(GameChapterPhase.CHAPTER1);
                chapters.Add(chapter);
                chapter = new GameChapter(GameChapterPhase.CHAPTER2);
                chapters.Add(chapter);
                chapter = new GameChapter(GameChapterPhase.CHAPTER3);
                chapters.Add(chapter);
                chapter = new GameChapter(GameChapterPhase.CHAPTER4);
                chapters.Add(chapter);
                chapter = new GameChapter(GameChapterPhase.CHAPTER5);
                chapters.Add(chapter);
            }
            else
            {
                chapters = new List<GameChapter>();
                GameChapter chapter;
                chapter = new GameChapter(GameChapterPhase.CHAPTER1);
                chapters.Add(chapter);
                chapter = new GameChapter(GameChapterPhase.CHAPTER2);
                chapters.Add(chapter);
                chapter = new GameChapter(GameChapterPhase.CHAPTER3);
                chapters.Add(chapter);
                chapter = new GameChapter(GameChapterPhase.CHAPTER4);
                chapters.Add(chapter);
                chapter = new GameChapter(GameChapterPhase.CHAPTER5);
                chapters.Add(chapter);
                chapter = new GameChapter(GameChapterPhase.FOREVER);
                chapters.Add(chapter);
            }
            return true;
        }
        public bool GetNextChapter(out GameChapter chapter)
        {
            chapter = null;
            if (chapters.Count <= 0)
            {
                return false;
            }
            chapter = chapters[0];
            chapters.RemoveAt(0);
            return true;
        }
    }

    interface GamePage
    {
        void InitGamePage(DuckHuntGame game);
        void GetObjects(out List<ModelObject> objlst);
        void Update(GameTime gametime);
        void Click(List<Vector2> clickpositionlist);
    }

    class GamePlayPage: GamePage
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


        HitBoardModel hitBoard;
        Rectangle hitBoardSpace;

        ScroeBoardModel scoreBoard;
        Rectangle scoreBoardSpace;

        ButtonModel pause;
        Rectangle  pauseButtonSpace;



        GameBackGroundPage backgroundPage = null;
        DuckHuntGame duckHuntGame = null;

        public  GamePlayPage()
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
                pauseButtonSpace.X = rectBackground.Height - button.GetSpace().Width - 20;
                pauseButtonSpace.Y = rectBackground.Right - 300; ;
                pauseButtonSpace.Width = button.GetSpace().Width;
                pauseButtonSpace.Height = button.GetSpace().Height;
            }
            else
            {

                pauseButtonSpace.X = rectBackground.Width - button.GetSpace().Width - 20;
                pauseButtonSpace.Y = rectBackground.Bottom - 300;
                pauseButtonSpace.Width = button.GetSpace().Width;
                pauseButtonSpace.Height = button.GetSpace().Height;
            }


            NewDog();
            NewScoreBoard();
            NewAssistBoard();
        }


        public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = new List<ModelObject>();
            if (phase == GAME_PHASE.SEEK_DUCK)
            {
                objlst.Add(dog);
                objlst.Add(panda);
            }
            else if (phase == GAME_PHASE.DUCK_FLY)
            {
                objlst.Add(panda);
                objlst.Add(pause);

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
            panda.Update(gametime);
            if (phase == GAME_PHASE.GAME_SELECT)
            {
            }
            else if (phase == GAME_PHASE.SEEK_DUCK)
            {
                //
                dog.Update(gametime);
                if (dog.Gone)
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

                for(int i=0; i<bulletsList.Count; )
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

                bullet.AdjustForFlyEffect();
                bulletsList.Add(bullet);

                if (bullet.GetShootDucks() != null)
                {
                    //
                    float score = 100;
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

            panda = new PandaModel();
            panda.Initialize(null, dogRunSpace, 0);
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
            leftTime.SetTime(5 * 60);
            lostDuck.ResetLostCount();
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
                scoreListMenuSpace.X = rectBackground.Width/2;
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
            if(returnMenuItem.Hit(clickpositionlist[0]))
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
            pos.Y = game.GetGlobalViewRect().Height - keyboard.GetSpace().Height  ;
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

            //
            // load config
            /*
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            composite = (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["exampleCompositeSetting"];
            if (composite == null)
            {
                composite = new Windows.Storage.ApplicationDataCompositeValue();
                composite["GameBackGroundMusic"] = "true";
                composite["GameSound"] = "true";
                localSettings.Values["exampleCompositeSetting"] = composite;
            }
            string value = composite["GameBackGroundMusic"].ToString();
            backgroundMusic.Checked = !(value == "false");
            value = composite["GameSound"].ToString();
            gameSound.Checked = !(value == "false");
             */
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
                    (rectBackground.Width ) / 2 - 100;
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

            ByteOut = new byte[ByteStrings.Length ];

            for (int i = 0; i < ByteStrings.Length; i++)
            {

                ByteOut[i] = (byte)ByteStrings[i];
            }

            return ByteOut;

        }


        public static string  ByteToString(byte[] bytein)
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
            content += this.EnableBgMusic? "1" : "0";
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

        private void LoadBackGroundMusic(XmlReader reader,ref  bool enablebgmusic)
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
    class DuckHuntGame
    {
        // org point
        public Vector2 orgpoint;
        // default scale
        public float defscale;

        Rectangle localViewRect = new Rectangle();
        Rectangle globalViewRect = new Rectangle(0,0, 1600,900);

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
            /*
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            // Create a composite setting 
            Windows.Storage.ApplicationDataCompositeValue composite = new Windows.Storage.ApplicationDataCompositeValue();
            composite["GameBackGroundMusic"] = true;
            composite["GameSound"] = false;
             */

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
            objlst.Add(new PandaModel());
            objlst.Add(new ButtonModel());
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

        public  void StartGame(Rectangle screenRect1)
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
                float occupiedh = screenRect.Width * 1.0f/logicWtoH;
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
                this.orgpoint.X = (screenRect.Width-occupiedw)/2;
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
            /*
            if (phase == GAME_PHASE.GAME_SELECT)
            {
                foreach (Vector2 clickpos in clickpositionlist)
                {

                    if (menuTimeModelItem.Hit(clickpos))
                    {
                        // time model game begin
                        phase = GAME_PHASE.SEEK_DUCK;
                        gameMode = GameMode.GAME_TIME_LIMIT;
                        NewGame();

                        return;
                    }

                    if (menuFreeModelItem.Hit(clickpos))
                    {
                        // free model

                        phase = GAME_PHASE.SEEK_DUCK;
                        gameMode = GameMode.GAME_FREE_MODE;
                        NewGame();

                        return;
                    }


                    if (menuGameOverItem.Hit(clickpos))
                    {
                        // free model
#if WINDOWS_PHONE
                        if (!Guide.IsVisible)
                            //弹出软键盘输入框
                            Guide.BeginShowKeyboardInput(PlayerIndex.One, "test", "test description",
                                sipResult, keyboardCallback, new object());
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

                }
             */
                return;
            }


            /*
            if (phase == GAME_PHASE.SCORELIST_SHOW)
            {

                foreach (Vector2 clickpos in clickpositionlist)
                {

                    if (menuReturnItem.Hit(clickpos))
                    {
                        // free model

                        phase = GAME_PHASE.GAME_SELECT;
                        gameMode = GameMode.GAME_FREE_MODE;
                        NewGame();
                        return;
                    }

                }


            }

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

                BulletModel bullet = new BulletModel(clickpos);
                foreach (DuckModel duck in duckList)
                {
                    duck.Shoot(bullet);
                }

                bullet.AdjustForFlyEffect();
                bulletsList.Add(bullet);

                if (bullet.GetShootDucks() != null)
                {
                    //
                    float score = 100;
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
                            //hitBoard.SetDuckIconsState(currentduck + ii, DuckIconModel.DuckIconState.Dead);
                            hitBoard.AddHitCount(1);
                        }
                        ii++;
                    }

                }

            }
        }
        */
    }

}
