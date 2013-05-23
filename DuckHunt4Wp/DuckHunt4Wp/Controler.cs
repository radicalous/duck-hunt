using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using GameCommon;

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
        public static ViewObject CreateViewObject(Rectangle screerc, ModelObject model, Vector2 orgpoint, float defscale)
        {
            CommonViewObject commViewObj = null;
            ScoreBoardViewObject scoreboardObj = null;
            HitBoardViewObject hitBoardObj = null;
            ViewObject viewObject = null;
            switch (model.Type())
            {
                case ModelType.SKY:
                case ModelType.CLOUD:
                case ModelType.GRASS:
                case ModelType.FORGROUND:
                case ModelType.DOG:
                case ModelType.DUCK:
                case ModelType.BULLET:
                    {
                        commViewObj = new CommonViewObject(model, orgpoint, defscale);
                    }
                    break;
                case ModelType.HITBOARD:
                    {
                        hitBoardObj = new HitBoardViewObject(model);
                    }
                    break;
                case ModelType.DUCKICON:
                    {
                        commViewObj = new CommonViewObject(model, orgpoint, defscale);
                    }
                    break;
                case ModelType.BULLETBOARD:
                    {
                        commViewObj = new CommonViewObject(model, orgpoint, defscale);
                    }
                    break;
                case ModelType.BULLETICON:
                    {
                        commViewObj = new CommonViewObject(model, orgpoint, defscale);
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
            }
            if (commViewObj != null)
            {
                commViewObj.screenRc = screerc;
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
            return viewObject;
        }
    }

    class DuckHuntGameControler
    {
        ContentManager Content = null;
        DuckHuntGame game = null;
        Rectangle viewRect = new Rectangle();

        Dictionary<ModelType, ObjectTexturesItem> objTextureLst;


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
                    viewObject = ViewObjectFactory.CreateViewObject(this.viewRect, obj, game.orgpoint, game.defscale);
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
                    viewObject = ViewObjectFactory.CreateViewObject(this.viewRect, obj, game.orgpoint, game.defscale);
                    viewObject.Init(game.orgpoint, game.defscale, obj, objTextureLst, obj.GetSpace());
                    obj.SetViewObject(viewObject);
                }
                viewObject.Draw(spriteBatch);
            }
        }


        public void HuntDuck(Vector2 shootPosition)
        {
            //
            // local rect, global rect
            // (local rect - orgpoint ) = global rect * default scale
            // local rect = orgpoint + global rect * default scale
            // global rect = (local rect - orgpoint)/ defalult scale
            //
            Vector2 globalshotpos = (shootPosition - game.orgpoint) / game.defscale;
            game.ShootDuck(globalshotpos);
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



    enum GAME_PHASE { GAME_SELECT, SEEK_DUCK, DUCK_FLY, DOG_SHOW, OVER };
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
                        duck = new DuckModel();
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
                        duck = new DuckModel();
                        ducks.Add(duck);
                        duck = new DuckModel();
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
                        duck = new DuckModel();
                        ducks.Add(duck);
                        duck = new DuckModel();
                        ducks.Add(duck);
                        duck = new DuckModel();
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
                        duck = new DuckModel();
                        ducks.Add(duck);
                        duck = new DuckModel();
                        ducks.Add(duck);
                        duck = new DuckModel();
                        ducks.Add(duck);
                        duck = new DuckModel();
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
                            duck = new DuckModel();
                            ducks.Add(duck);
                            duckcount += 1;
                        }
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


    class DuckHuntGame
    {

        GAME_PHASE phase = GAME_PHASE.GAME_SELECT;

        GameChapterManager gameChapterMgr;

        // org point
        public Vector2 orgpoint;
        // default scale
        public float defscale;

        Rectangle localViewRect = new Rectangle();
        Rectangle globalViewRect = new Rectangle(0,0, 1600,900);


        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //
       

        // we need to extend the backgound so that local screen has black screen
        Vector2 bgorgpoint;
        float bgdefscale;
        Rectangle localViewRect4Background = new Rectangle();



        Rectangle rectBackground;
        SkyModel blueSky;
        CloudModel cloud;
        GrassModel grass;
        ForegroundGrassModel forground;

        MenuItemModel menuTimeModelItem;
        MenuItemModel menuFreeModelItem;
        MenuItemModel menuGameOverItem;
        MenuItemModel menuRestartItem;

        Rectangle timeModelMenuSpace;
        Rectangle freeModelModelMenuSpace;


        List<BulletModel> bulletsList;
        List<DuckModel> duckList;
        Rectangle duckFlySpace;

        DogModel dog;
        Rectangle dogRunSpace;

        HitBoardModel hitBoard;
        Rectangle hitBoardSpace;

        BulletBoardModel bulletBoard;
        Rectangle bulletBoardSpace;

        ScroeBoardModel scoreBoard;
        Rectangle scoreBoardSpace;

        int round = 1;
        List<GameRound> gameRounds;

        int currentduck = 0;
        int bulletcount = 3;

        public DuckHuntGame()
        {
            gameRounds = new List<GameRound>();
            bulletsList = new List<BulletModel>();

            gameChapterMgr = new GameChapterManager();
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
            objlst = new List<ModelObject>();
            if (phase == GAME_PHASE.GAME_SELECT)
            {
                objlst.Add(blueSky);
                objlst.Add(cloud);
                objlst.Add(grass);

                objlst.Add(this.hitBoard);
                //objlst.Add(bulletBoard);
                objlst.Add(scoreBoard);

                objlst.Add(forground);

                objlst.Add(menuTimeModelItem);
                objlst.Add(menuFreeModelItem);
            }
            else if (phase == GAME_PHASE.SEEK_DUCK)
            {
                objlst.Add(blueSky);
                objlst.Add(cloud);
                objlst.Add(grass);
                objlst.Add(dog);
                objlst.Add(this.hitBoard);
                //objlst.Add(bulletBoard);
                objlst.Add(scoreBoard);
                objlst.Add(forground);
            }
            else if (phase == GAME_PHASE.DUCK_FLY)
            {
                objlst.Add(blueSky);
                objlst.Add(cloud);
                objlst.Add(grass);
                objlst.Add(forground);
                //objlst.Add(bulletBoard);

                foreach (DuckModel duck in duckList)
                {
                    objlst.Add(duck);
                }
                foreach (BulletModel bullet in bulletsList)
                {
                    objlst.Add(bullet);
                }
                objlst.Add(this.hitBoard);
                objlst.Add(scoreBoard);
            }
            else if (phase == GAME_PHASE.DOG_SHOW)
            {
                objlst.Add(blueSky);
                objlst.Add(cloud);
                objlst.Add(grass);
                objlst.Add(forground);
                //objlst.Add(bulletBoard);

                objlst.Add(dog);
                objlst.Add(this.hitBoard);
                objlst.Add(scoreBoard);
            }
            else if (phase == GAME_PHASE.OVER)
            {
                objlst.Add(blueSky);
                objlst.Add(cloud);
                objlst.Add(grass);
                objlst.Add(forground);

                //objlst.Add(bulletBoard);

                objlst.Add(this.hitBoard);
                objlst.Add(scoreBoard);
                objlst.Add(menuGameOverItem);
                objlst.Add(menuRestartItem);
                

            }
        }

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

        void NewDog()
        {
            // dog seek duck
            dog = new DogModel();
            dog.Initialize(null, dogRunSpace, 0);
            dog.StartPilot();
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
                /*
                foreach (DuckModel duck2 in duckList)
                {
                    if (duck2.dead)
                    {
                        hitBoard.SetDuckIconsState(currentduck + ii, DuckIconModel.DuckIconState.Dead);
                    }
                    else
                    {
                        hitBoard.SetDuckIconsState(currentduck + ii, DuckIconModel.DuckIconState.Alive);
                    }
                    ii++;
                }
                currentduck += duckList.Count;
                 */

                if (  gameMode != GameMode.GAME_TIME_LIMIT)
                {
                    foreach (DuckModel duck2 in duckList)
                    {
                        if ( !duck2.dead)
                        {
                            flycount++;
                        }
                    }
                    if (flycount >= 3)
                    {
                        duckList.Clear();
                        return;
                    }
                }
                duckList.Clear();

                /*
                bulletcount = 3;
                bulletBoard.LoadBullet(bulletcount);
                 */
            }

            List<DuckModel> ducks = null;

            do
            {
                if (chapter != null && chapter.GetDuckList(out ducks) && ducks != null && ducks.Count > 0)
                {
                    break;
                }
                gameChapterMgr.GetNextChapter(out chapter);
            }while(chapter != null);

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

                /*
                for (i = 0; i < concurrentduckcnt; i++)
                {
                    hitBoard.SetDuckIconsState(this.currentduck + i, DuckIconModel.DuckIconState.Ongoing);
                }
                 */
            
            }


        }

        void NewHitBoard()
        {
            hitBoard = new HitBoardModel();
            hitBoard.Initialize(null, hitBoardSpace, 0);
            hitBoard.LoadDuckIconsModel(10);
        }

        void NewBulletBoard()
        {
            bulletBoard = new BulletBoardModel();
            bulletBoard.Initialize(null, bulletBoardSpace, 0);
            bulletBoard.LoadBullet(3);
        }

        void NewScoreBoard()
        {
            scoreBoard = new ScroeBoardModel();
            scoreBoard.Initialize(null, scoreBoardSpace, 0);
        }

        void NewMenu()
        {
            menuTimeModelItem = new MenuItemModel();
            menuTimeModelItem.Initialize(null, timeModelMenuSpace, 0);
            menuTimeModelItem.Conent = "Time Model";
            menuFreeModelItem = new MenuItemModel();
            menuFreeModelItem.Initialize(null, freeModelModelMenuSpace, 0);
            menuFreeModelItem.Conent = "Free Model";

            menuGameOverItem = new MenuItemModel();
            menuGameOverItem.Initialize(null, timeModelMenuSpace, 0);
            menuGameOverItem.Conent = "Game Over";

            menuRestartItem = new MenuItemModel();
            menuRestartItem.Initialize(null, freeModelModelMenuSpace, 0);
            menuRestartItem.Conent = "Restart";  
        }

        void NewGame()
        {
            gameChapterMgr.Init(gameMode);
        }
        public void StartGame(Rectangle screenRect)
        {

            // logic rect 1600x900
            // calculate our background rect
            if ((float)screenRect.Width / screenRect.Height > 16.0 / 9)
            {
                // to wide, will full filll height
                float occupiedw = screenRect.Height * 16.0f / 9;
                this.orgpoint.X = (screenRect.Width-occupiedw)/2;
                this.orgpoint.Y = 0;

                localViewRect.X = (int)orgpoint.X;
                localViewRect.Y = (int)orgpoint.Y;
                localViewRect.Width = (int)occupiedw;
                localViewRect.Height = screenRect.Height;
            }
            else
            {
                // to high, will full fill width
                float occupiedh = screenRect.Width * 9.0f / 16;
                this.orgpoint.X = 0;
                this.orgpoint.Y = (screenRect.Height - occupiedh)/2;

                localViewRect.X = (int)orgpoint.X;
                localViewRect.Y = (int)orgpoint.Y;
                localViewRect.Width = screenRect.Width;
                localViewRect.Height = (int)occupiedh;
            }       
            // calculate default scale
            this.defscale = localViewRect.Width * 1.0f / globalViewRect.Width;

            // calculate background settings



            //
            // load textures

            rectBackground = globalViewRect;
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
                dogRunSpace.Height = 180;
            }
            else
            {
                dogRunSpace.Width = rectBackground.Width;
                dogRunSpace.Y = rectBackground.Height - 150-150;
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
                hitBoardSpace.Y = rectBackground.Top +10;
                hitBoardSpace.Width = hitBoard1.GetSpace().Width;
                hitBoardSpace.Height = hitBoard1.GetSpace().Height;
            }


            BulletBoardModel bulletBoard1 = new BulletBoardModel();
            if (rectBackground.Width < rectBackground.Height)
            {
                bulletBoardSpace.X = 52;
                bulletBoardSpace.Y = rectBackground.Width - 68;
                bulletBoardSpace.Width = bulletBoard1.GetSpace().Width;
                bulletBoardSpace.Height = bulletBoard1.GetSpace().Height;
            }
            else
            {
                bulletBoardSpace.X = 52;
                bulletBoardSpace.Y = rectBackground.Height - 68;
                bulletBoardSpace.Width = bulletBoard1.GetSpace().Width;
                bulletBoardSpace.Height = bulletBoard1.GetSpace().Height;
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

            MenuItemModel menuItem = new MenuItemModel();
            if (rectBackground.Width < rectBackground.Height)
            {
                timeModelMenuSpace.X = rectBackground.Height - 150;
                timeModelMenuSpace.Y = rectBackground.Width - 150;
                timeModelMenuSpace.Width = menuItem.GetSpace().Width;
                timeModelMenuSpace.Height = menuItem.GetSpace().Height;

                freeModelModelMenuSpace = timeModelMenuSpace;
                freeModelModelMenuSpace.Y += 100;
            }
            else
            {
                timeModelMenuSpace.X = rectBackground.Width/2 - 150;
                timeModelMenuSpace.Y = rectBackground.Height/2 - 150;
                timeModelMenuSpace.Width = menuItem.GetSpace().Width;
                timeModelMenuSpace.Height = menuItem.GetSpace().Height;

                freeModelModelMenuSpace = timeModelMenuSpace;
                freeModelModelMenuSpace.Y += 100;
            }

            NewBackground();
            NewMenu();
            NewDog();
            NewHitBoard();
            NewBulletBoard();
            NewScoreBoard();
        }

        GameChapter chapter;
        public void Update(GameTime gametime)
        {
            //
            cloud.Update(gametime);
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
                    }
                }
            }
            /* will not let dog show
            else if (phase == GAME_PHASE.DOG_SHOW)
            {
                dog.Update(gametime);
                if (dog.Gone)
                {
                    NewDuck();
                    if (duckList.Count > 0 )
                    {
                        phase = GAME_PHASE.DUCK_FLY;
                    }
                    else
                    {
                        phase = GAME_PHASE.OVER;
                    }
                    round++;
                }
            }
            */
        }

        Vector2 oldshootposition = Vector2.Zero;
        public void ShootDuck(Vector2 shootposition)
        {

            if (phase == GAME_PHASE.GAME_SELECT)
            {
                if (menuTimeModelItem.Hit(shootposition))
                {
                    // time model game begin
                    phase = GAME_PHASE.SEEK_DUCK;
                    gameMode = GameMode.GAME_TIME_LIMIT;
                    NewGame();

                    return;
                }

                if (menuFreeModelItem.Hit(shootposition))
                {
                    // free model

                    phase = GAME_PHASE.SEEK_DUCK;
                    gameMode = GameMode.GAME_FREE_MODE;
                    NewGame();

                    return;
                }

                return;
            }

            if (phase == GAME_PHASE.OVER)
            {
                if (menuRestartItem.Hit(shootposition))
                {
                    // free model

                    phase = GAME_PHASE.GAME_SELECT;
                    gameMode = GameMode.GAME_FREE_MODE;
                    NewGame();
                    return;
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
            BulletModel bullet = new BulletModel(shootposition);
            foreach (DuckModel duck in duckList)
            {
                duck.Shoot(bullet);
            }
            bulletsList.Add(bullet);

            if (bullet.GetShootDucks() != null)
            {
                //
                float score = 100;
                for (int i = 0; i < bullet.GetShootDucks().Count; i++)
                {
                    score = 100;
                    score *= (i+1);
                    score /= bullet.GetShootDucks()[i].GetSacle();
                    scoreBoard.AddScore((int)score);

                }

                int ii = 0;
                foreach (DuckModel duck2 in duckList)
                {
                    if (duck2.dead)
                    {
                        hitBoard.SetDuckIconsState(currentduck + ii, DuckIconModel.DuckIconState.Dead);
                    }
                    ii++;
                }

            }

            bulletcount--;
            bulletBoard.RemoveFirstBullet();
        }
    }

}
