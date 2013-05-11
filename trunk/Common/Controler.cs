using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using GameCommon;

namespace DuckHuntCommon 
{
    class ObjectTexturesItem
    {
        public ModelType objType;
        public List<Texture2D> textureList;
    }

    class ViewObjectFactory
    {
        public static ViewObject CreateViewObject(ModelObject model)
        {
            ViewObject viewObject = null;
            switch (model.Type())
            {
                case ModelType.SKY:
                case ModelType.GRASS:
                case ModelType.DOG:
                case ModelType.DUCK:
                case ModelType.BULLET:
                    {
                        viewObject = new CommonViewObject(model);
                    }
                    break;
                case ModelType.HITBOARD:
                    {
                        viewObject = new CommonViewObject(model);
                    }
                    break;
                case ModelType.DUCKICON:
                    {
                        viewObject = new CommonViewObject(model);
                    }
                    break;
                case ModelType.BULLETBOARD:
                    {
                        viewObject = new CommonViewObject(model);
                    }
                    break;
                case ModelType.BULLETICON:
                    {
                        viewObject = new CommonViewObject(model);
                    }
                    break;
                    
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

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public void LoadContent(ContentManager Content1)
        {
            // TODO: use this.Content to load your game content here
            Content = Content1;
            LoadTextures();

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


        public void LoadTextures()
        {
            List<ObjectTexturesPathsItem> texturesPaths;
            game.GetTexturesPaths(out texturesPaths);
            if (texturesPaths != null)
            {
                foreach(ObjectTexturesPathsItem texturePathItm in texturesPaths)
                {
                    ObjectTexturesItem textureItm;
                    textureItm = new ObjectTexturesItem();
                    textureItm.objType = texturePathItm.objType;
                    textureItm.textureList = new List<Texture2D>();
                    foreach (string path in texturePathItm.texturePathList)
                    {
                        textureItm.textureList.Add(Content.Load<Texture2D>(path));
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
                    viewObject = ViewObjectFactory.CreateViewObject(obj);
                    viewObject.Init(obj, objTextureLst, obj.GetSpace());
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
                    viewObject = ViewObjectFactory.CreateViewObject(obj);
                    viewObject.Init(obj, objTextureLst, obj.GetSpace());
                    obj.SetViewObject(viewObject);
                }
                viewObject.Draw(spriteBatch);
            }
        }


        public void HuntDuck(Vector2 shootPosition)
        {
            game.ShootDuck(shootPosition);
        }

    }


    class ObjectTexturesPathsItem
    {
        public ModelType objType;
        public List<string> texturePathList;
    }

    class GameRound
    {
        public int deadDuck = 0;
        public string shootername;
    }
    class DuckHuntGame
    {
        enum GAME_PHASE { SEEK_DUCK, DUCK_FLY, DOG_SHOW, OVER };

        GAME_PHASE phase = GAME_PHASE.SEEK_DUCK;


        Rectangle rectBackground;
        SkyModel blueSky;
        GrassModel grass;

        List<BulletModel> bulletsList;
        List<DuckModel> duckList;
        Rectangle duckFlySpace;

        DogModel dog;
        Rectangle dogRunSpace;

        HitBoardModel hitBoard;
        Rectangle hitBoardSpace;

        BulletBoardModel bulletBoard;
        Rectangle bulletBoardSpace;

        int round = 1;
        List<GameRound> gameRounds;

        int currentduck = 0;
        int concurrentduckcnt = 2;

        int bulletcount = 3;

        public DuckHuntGame()
        {
            gameRounds = new List<GameRound>();
            bulletsList = new List<BulletModel>();
        }

        public void GetTexturesPaths(out List<ObjectTexturesPathsItem> texturesPaths)
        {
            texturesPaths = new List<ObjectTexturesPathsItem>();
       
            List<ModelObject> objlst = new List<ModelObject>();
            // sky
            objlst.Add(new SkyModel());
            objlst.Add(new GrassModel());
            objlst.Add(new DuckModel());
            objlst.Add(new DogModel());
            objlst.Add(new BulletModel());
            objlst.Add(new HitBoardModel());
            objlst.Add(new DuckIconModel());
            objlst.Add(new BulletBoardModel());
            objlst.Add(new BulletIconModel());

            foreach (ModelObject obj in objlst)
            {
                ObjectTexturesPathsItem texturesPathsItem = new ObjectTexturesPathsItem();
                texturesPathsItem.texturePathList = new List<string>();
                texturesPathsItem.objType = obj.Type();
                List<AnimationInfo> animationLst = obj.GetAnimationInfoList();
                foreach(AnimationInfo animationInfo in animationLst)
                {
                    texturesPathsItem.texturePathList.Add(animationInfo.texturesPath);
                }
                texturesPaths.Add(texturesPathsItem);
            }

        }

        public void GetObjects(out List<ModelObject> objlst)
        {
            objlst = new List<ModelObject>();
            if (phase == GAME_PHASE.SEEK_DUCK)
            {
                objlst.Add(blueSky);
                objlst.Add(grass);
                objlst.Add(dog);
                objlst.Add(this.hitBoard);
                objlst.Add(bulletBoard);
            }
            else if (phase == GAME_PHASE.DUCK_FLY)
            {
                objlst.Add(blueSky);
                objlst.Add(grass);
                objlst.Add(bulletBoard);

                foreach (DuckModel duck in duckList)
                {
                    objlst.Add(duck);
                }
                foreach (BulletModel bullet in bulletsList)
                {
                    objlst.Add(bullet);
                }
                objlst.Add(this.hitBoard);
            }
            else if (phase == GAME_PHASE.DOG_SHOW)
            {
                objlst.Add(blueSky);
                objlst.Add(grass);
                objlst.Add(bulletBoard);

                objlst.Add(dog);
                objlst.Add(this.hitBoard);

            }
            else if (phase == GAME_PHASE.OVER)
            {
                objlst.Add(blueSky);
                objlst.Add(grass);
                objlst.Add(bulletBoard);

                objlst.Add(this.hitBoard);

            }
        }

        void NewBackground()
        {
            // dog seek duck
            blueSky = new SkyModel();
            blueSky.Initialize(null, rectBackground, 0);
            grass = new GrassModel();
            grass.Initialize(null, rectBackground, 0);
        }

        void NewDog()
        {
            // dog seek duck
            dog = new DogModel();
            dog.Initialize(null, dogRunSpace, 0);
            dog.StartPilot(dogRunSpace);
        }

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
                duckList.Clear();
                bulletcount = 3;
                bulletBoard.LoadBullet(bulletcount);
            }
            DuckModel duck = new DuckModel();
            duckList.Add(duck);
            duck = new DuckModel();
            duckList.Add(duck);
            
            int i = 0;
            DateTime now = System.DateTime.Now;
            int s = now.Hour * 60 * 60 + now.Minute * 60 + now.Second;
            foreach (DuckModel duck1 in duckList)
            {
                duck1.Initialize(null, duckFlySpace, s + (i++) * 7);
                duck1.StartPilot(duckFlySpace);
            }

            for (i = 0; i < concurrentduckcnt; i++)
            {
                hitBoard.SetDuckIconsState(this.currentduck+i, DuckIconModel.DuckIconState.Ongoing);
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

        public void StartGame(Rectangle screenRect)
        {
            // load textures

            rectBackground = screenRect;
            if (rectBackground.Width < rectBackground.Height)
            {
                duckFlySpace.Width = rectBackground.Height - 50;
                duckFlySpace.Height = rectBackground.Width;
            }
            else
            {
                duckFlySpace.Width = rectBackground.Width;
                duckFlySpace.Height = rectBackground.Height - 50;
            }

            if (rectBackground.Width < rectBackground.Height)
            {
                dogRunSpace.Width = rectBackground.Height;
                dogRunSpace.Y = rectBackground.Width - 220;
                dogRunSpace.Height = 120;
            }
            else
            {
                dogRunSpace.Width = rectBackground.Width;
                dogRunSpace.Y = rectBackground.Height - 220;
                dogRunSpace.Height = 120;
            }

            HitBoardModel hitBoard1 = new HitBoardModel();
            if (rectBackground.Width < rectBackground.Height)
            {
                hitBoardSpace.X = (rectBackground.Height - hitBoard1.GetSpace().Width) / 2 + 20;
                hitBoardSpace.Y = dogRunSpace.Left + 150;
                hitBoardSpace.Width = hitBoard1.GetSpace().Width;
                hitBoardSpace.Height = hitBoard1.GetSpace().Height;
            }
            else
            {
                hitBoardSpace.X = (rectBackground.Width - hitBoard1.GetSpace().Width) / 2 + 20;
                hitBoardSpace.Y = dogRunSpace.Top + 150;
                hitBoardSpace.Width = hitBoard1.GetSpace().Width;
                hitBoardSpace.Height = hitBoard1.GetSpace().Height;
            }


            BulletBoardModel bulletBoard1 = new BulletBoardModel();
            if (rectBackground.Width < rectBackground.Height)
            {
                bulletBoardSpace.X = 20;
                bulletBoardSpace.Y = dogRunSpace.Left + 150;
                bulletBoardSpace.Width = bulletBoard1.GetSpace().Width;
                bulletBoardSpace.Height = bulletBoard1.GetSpace().Height;
            }
            else
            {
                bulletBoardSpace.X = 20;
                bulletBoardSpace.Y = dogRunSpace.Top + 150;
                bulletBoardSpace.Width = bulletBoard1.GetSpace().Width;
                bulletBoardSpace.Height = bulletBoard1.GetSpace().Height;
            }

            NewBackground();
            NewDog();
            NewHitBoard();
            NewBulletBoard();
        }


        public void Update(GameTime gametime)
        {
            //
            if (phase == GAME_PHASE.SEEK_DUCK)
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
                GameRound gameround = new GameRound();
                gameround.deadDuck = deadcount;
                gameround.shootername = "pennerz";
                gameRounds.Add(gameround);

                if (finished)
                {
                    phase = GAME_PHASE.DOG_SHOW;
                    dog.ShowDog(deadcount);
                }
            }
            else if (phase == GAME_PHASE.DOG_SHOW)
            {
                dog.Update(gametime);
                if (dog.Gone)
                {
                    if (round < 10)
                    {
                        phase = GAME_PHASE.DUCK_FLY;

                        // create two new duck
                        NewDuck();
                    }
                    else
                    {
                        phase = GAME_PHASE.OVER;
                    }
                    round++;
                }
            }
        }

        Vector2 oldshootposition = Vector2.Zero;
        public void ShootDuck(Vector2 shootposition)
        {
            if (oldshootposition == shootposition)
            {
                return;
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
