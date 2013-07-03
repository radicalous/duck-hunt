using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;


using GameCommon;

namespace DuckHuntCommon
{
    class ViewItem
    {
        public Animation animation;
        public bool backGroundAnimation;
        public StaticBackground2 staticBackground;
        public Animation bganimation;
    }


    abstract class ViewObject
    {
        public abstract void Init(Vector2 orgpoint1, float defscale1, ModelObject model, Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space);
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void PlaySound();
    }

    abstract class DefViewObject : ViewObject
    {
        public override void PlaySound()
        {
        }

        private void DrawRectangle(SpriteBatch spriteBatch, Rectangle coords, Color color)
        {
            var rect = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            rect.SetData(new[] { color });

            // draw left
            Rectangle rcline = new Rectangle();
            rcline.X = coords.Left;
            rcline.Y = coords.Top;
            rcline.Width = 1;
            rcline.Height = coords.Height;
            spriteBatch.Draw(rect, rcline, color);
            rcline.X += coords.Width;
            spriteBatch.Draw(rect, rcline, color);
            rcline.X = coords.Left;
            rcline.Width = coords.Width;
            rcline.Height = 1;
            spriteBatch.Draw(rect, rcline, color);
            rcline.Y += coords.Height;
            spriteBatch.Draw(rect, rcline, color);
        }


        private void DrawRectangle2(SpriteBatch spriteBatch, Rectangle coords, Color color)
        {
            var rect = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            rect.SetData(new[] { color });

            // draw left

            spriteBatch.Draw(rect, coords, color);
        }
    }



    class CommonViewObject : DefViewObject
    {
        List<AnimationInfo> animationList;
        List<ViewItem> viewItmList;

        ModelObject model;
        List<ViewObject> childViewObjectList;

        Vector2 _orgpointinscreen;
        float _defscaleinscreen;

        public Vector2 OrgPointInScreen
        {
            get
            {
                return _orgpointinscreen;
            }
        }
        public float DefScaleInScreen
        {
            get
            {
                return _defscaleinscreen;
            }
        }

        public List<SpriteFont> ObjFontList
        {
            get
            {
                return _resLst[model.Type()].fontList;
            }
        }
        List<SpriteFont> fontlist;

        // screen rect
        public Rectangle screenRc = new Rectangle();

        public CommonViewObject(ModelObject model1, Vector2 orgpointinscreen, float defscaleinscreen)
        {

        }

        public CommonViewObject()
        {

        }

        Dictionary<ModelType, ObjectTexturesItem> _resLst;

        public override void Init(Vector2 orgpointinscreen, float defscaleinscreen, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle spaceInLogic)
        {
            if (model == null || (model.Type() != ModelType.KEYBORD && model.Type() != ModelType.KEYITEM))
            {
                _orgpointinscreen = orgpointinscreen;
                _defscaleinscreen = defscaleinscreen;
            }

            model = model1;
            List<ModelObject> childobjlst = model.GetChildrenObjects();
            if (childobjlst != null)
            {
                childViewObjectList = new List<ViewObject>();
                foreach (ModelObject obj in childobjlst)
                {
                    ViewObject viewobj = ViewObjectFactory.CreateViewObject(obj);

                    childViewObjectList.Add(viewobj);
                }
            }

            _orgpointinscreen = orgpointinscreen;
            _defscaleinscreen = defscaleinscreen;

            _resLst = objTextureLst;
            // try to calculate how may textures are needed by children

            // create view items for this object
            List<Texture2D> texturesList = objTextureLst[model.Type()].textureList;
            animationList = model.GetAnimationInfoList();
            viewItmList = new List<ViewItem>();
            for (int i = 0; i < texturesList.Count; i++)
            {
                AnimationInfo animationInfo = model.GetAnimationInfoList()[i];
                ViewItem viewItm = new ViewItem();
                if (animationInfo.animation)
                {
                    viewItm.animation = new Animation();
                    viewItm.animation.Initialize(
                        texturesList[i],
                        Vector2.Zero, (int)(animationInfo.frameWidth),
                        (int)(animationInfo.frameHeight),
                        animationInfo.frameCount, animationInfo.frameTime, animationInfo.backColor,
                        model.GetSacle(), true);
                }
                else
                {
                    viewItm.backGroundAnimation = true;
                    viewItm.bganimation = new Animation();
                    if (animationInfo.frameHeight == 0)
                    {
                        viewItm.bganimation.Initialize(
                            texturesList[i],
                            Vector2.Zero, (int)(texturesList[i].Width/*animationInfo.frameWidth*/),
                            (int)(texturesList[i].Height/*animationInfo.frameHeight*/),
                            1/*animationInfo.frameCount*/, 1/*animationInfo.frameTime*/, animationInfo.backColor,
                            model.GetSacle(), true);


                        float scale = 1.0f;
                        if (texturesList[i].Width * 1.0f / texturesList[i].Height > screenRc.Width * 1.0 / screenRc.Height)
                        {
                            // the text wider, should extend according height
                            scale = screenRc.Height * 1.0f / texturesList[i].Height;

                            int offx = (int)((texturesList[i].Width * scale - screenRc.Width) / 2 / scale);
                            offx = (int)(offx * scale);
                            offx = -offx;
                            int centerx = (int)(offx + texturesList[i].Width * scale / 2);
                            int centery = screenRc.Height / 2;
                            viewItm.bganimation.Position.X = centerx;
                            viewItm.bganimation.Position.Y = centery;
                            viewItm.bganimation.scale = scale;

                        }
                        else
                        {
                            // the texture is higher, should extend according width
                            scale = screenRc.Width * 1.0f / texturesList[i].Width;

                            int offy = (int)((texturesList[i].Height * scale - screenRc.Height) / scale);
                            offy = (int)(offy * scale);
                            offy = -offy;
                            int centerx = screenRc.Width / 2;
                            int centery = (int)(offy + texturesList[i].Height * scale / 2);
                            viewItm.bganimation.Position.X = centerx;
                            viewItm.bganimation.Position.Y = centery;
                            viewItm.bganimation.scale = scale;

                        }
                    }
                    else
                    {
                        viewItm.bganimation.Initialize(
                            texturesList[i],
                            Vector2.Zero, animationInfo.frameWidth,
                            animationInfo.frameHeight,
                            animationInfo.frameCount, animationInfo.frameTime, animationInfo.backColor,
                            model.GetSacle(), true);


                        float scale = 1.0f;
                        if (animationInfo.frameWidth * 1.0f / animationInfo.frameHeight > screenRc.Width * 1.0 / screenRc.Height)
                        {
                            // the text wider, should extend according height
                            scale = screenRc.Height * 1.0f / animationInfo.frameHeight;

                            int offx = (int)((animationInfo.frameWidth * scale - screenRc.Width) / 2 / scale);
                            offx = (int)(offx * scale);
                            offx = -offx;
                            int centerx = (int)(offx + animationInfo.frameWidth * scale / 2);
                            int centery = screenRc.Height / 2;
                            viewItm.bganimation.Position.X = centerx;
                            viewItm.bganimation.Position.Y = centery;
                            viewItm.bganimation.scale = scale;

                        }
                        else
                        {
                            // the texture is higher, should extend according width
                            scale = screenRc.Width * 1.0f / animationInfo.frameWidth;

                            int offy = (int)((animationInfo.frameHeight * scale - screenRc.Height) / scale);
                            offy = (int)(offy * scale);
                            offy = -offy;
                            int centerx = screenRc.Width / 2;
                            int centery = (int)(offy + animationInfo.frameHeight * scale / 2);
                            viewItm.bganimation.Position.X = centerx;
                            viewItm.bganimation.Position.Y = centery;
                            viewItm.bganimation.scale = scale;

                        }

                    }

                }
                viewItmList.Add(viewItm);
            }

            // left textures are for children
            if (childViewObjectList != null)
            {
                foreach (ViewObject childviewobj in childViewObjectList)
                {
                    Rectangle rc = new Rectangle();
                    childviewobj.Init(_orgpointinscreen, _defscaleinscreen, null, objTextureLst, rc);
                }
            }
        }

        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //

        public override void Update(GameTime gameTime)
        {
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            if (animationList[model.GetCurrentAnimationIndex()].animation)
            {
                viewItm.animation.Position = _orgpointinscreen +
                    model.GetAbsolutePosition() * _defscaleinscreen;

                viewItm.animation.Position.X += (viewItm.animation.FrameWidth / 2) * _defscaleinscreen;
                viewItm.animation.Position.Y += (viewItm.animation.FrameHeight / 2) * _defscaleinscreen;
                viewItm.animation.scale = model.GetSacle() * _defscaleinscreen;
                viewItm.animation.Update(gameTime);
            }
            else
            {
                if (viewItm.backGroundAnimation)
                {
                    viewItm.bganimation.Update(gameTime);
                }
                else
                {
                    viewItm.staticBackground.Update(gameTime);
                }
            }

            if (childViewObjectList != null)
            {
                foreach (ViewObject viewObj in childViewObjectList)
                {
                    viewObj.Update(gameTime);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            if (animationList[model.GetCurrentAnimationIndex()].animation)
            {
                // check if the position is zero,
                // when button pause, it switch a animation, but update will never be called
                viewItm.animation.Draw(spriteBatch, model.GetAnimationDepth());
            }
            else
            {
                if (viewItm.backGroundAnimation)
                {
                    viewItm.bganimation.Draw(spriteBatch, model.GetAnimationDepth());
                }
                else
                {
                    viewItm.staticBackground.Draw(spriteBatch, model.GetAnimationDepth());
                }
            }
            if (childViewObjectList != null)
            {
                foreach (ViewObject viewObj in childViewObjectList)
                {
                    viewObj.Draw(spriteBatch);
                }
            }
        }

        public override void PlaySound()
        {
            // check need to play audio
            //play init sound
            if (_resLst[model.Type()].soundList.Count > 0)
            {
                int soundindex = model.GetSoundIndex();
                if (soundindex >= 0 && soundindex < _resLst[model.Type()].soundList.Count)
                {
                    float mastvol = SoundEffect.MasterVolume;
                    _resLst[model.Type()].soundList[0].Play(1, 0, 0);
                }

            }
        }
    }


    class CommonViewObjectEx : DefViewObject
    {
        List<AnimationInfo> animationList;
        AnimationEx viewItm;

        ModelObject model;
        List<ViewObject> childViewObjectList;

        Vector2 _orgpointinscreen;
        float _defscaleinscreen;

        public Vector2 OrgPointInScreen
        {
            get
            {
                return _orgpointinscreen;
            }
        }
        public float DefScaleInScreen
        {
            get
            {
                return _defscaleinscreen;
            }
        }

        public List<SpriteFont> ObjFontList
        {
            get
            {
                return _resLst[model.Type()].fontList;
            }
        }
        List<SpriteFont> fontlist;

        // screen rect
        public Rectangle screenRc = new Rectangle();

        public CommonViewObjectEx(ModelObject model1, Vector2 orgpointinscreen, float defscaleinscreen)
        {

        }

        public CommonViewObjectEx()
        {

        }

        Dictionary<ModelType, ObjectTexturesItem> _resLst;



        public override void Init(Vector2 orgpointinscreen, float defscaleinscreen, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle spaceInLogic)
        {
            if (model == null || (model.Type() != ModelType.KEYBORD && model.Type() != ModelType.KEYITEM))
            {
                _orgpointinscreen = orgpointinscreen;
                _defscaleinscreen = defscaleinscreen;
            }
            //screenRc = spaceInLogic;

            model = model1;
            List<ModelObject> childobjlst = model.GetChildrenObjects();
            if (childobjlst != null)
            {
                childViewObjectList = new List<ViewObject>();
                foreach (ModelObject obj in childobjlst)
                {
                    ViewObject viewobj = ViewObjectFactory.CreateViewObject(obj);

                    childViewObjectList.Add(viewobj);
                }
            }

            _orgpointinscreen = orgpointinscreen;
            _defscaleinscreen = defscaleinscreen;

            _resLst = objTextureLst;
            // try to calculate how may textures are needed by children

            // create view items for this object
            List<Texture2D> texturesList = objTextureLst[model.Type()].textureList;
            animationList = model.GetAnimationInfoList();
            viewItm = new AnimationEx();
            AnimationInfo animationInfo = model.GetAnimationInfoList()[0];

            {
                if (animationInfo.frameHeight == 0)
                {
                    viewItm.Initialize(
                        texturesList,
                        Vector2.Zero, (int)(texturesList[0].Width/*animationInfo.frameWidth*/),
                        (int)(texturesList[0].Height/*animationInfo.frameHeight*/),
                        animationInfo.frameCount, animationInfo.frameTime, animationInfo.backColor,
                        model.GetSacle(), true);


                    float scale = 1.0f;
                    if (texturesList[0].Width * 1.0f / texturesList[0].Height > screenRc.Width * 1.0 / screenRc.Height)
                    {
                        // the text wider, should extend according height
                        scale = screenRc.Height * 1.0f / texturesList[0].Height;

                        int offx = (int)((texturesList[0].Width * scale - screenRc.Width) / 2 / scale);
                        offx = (int)(offx * scale);
                        offx = -offx;
                        int centerx = (int)(offx + texturesList[0].Width * scale / 2);
                        int centery = screenRc.Height / 2;
                        viewItm.Position.X = centerx;
                        viewItm.Position.Y = centery;
                        viewItm.scale = scale;

                    }
                    else
                    {
                        // the texture is higher, should extend according width
                        scale = screenRc.Width * 1.0f / texturesList[0].Width;

                        int offy = (int)((texturesList[0].Height * scale - screenRc.Height) / scale);
                        offy = (int)(offy * scale);
                        offy = -offy;
                        int centerx = screenRc.Width / 2;
                        int centery = (int)(offy + texturesList[0].Height * scale / 2);
                        viewItm.Position.X = centerx;
                        viewItm.Position.Y = centery;
                        viewItm.scale = scale;

                    }
                }
            }
            /*
            viewItm.Initialize(texturesList, Vector2.Zero, animationInfo.frameWidth,
                animationInfo.frameHeight, animationInfo.frameCount, animationInfo.frameTime,
                animationInfo.backColor, model.GetSacle(), true);
            */

            // left textures are for children
            if (childViewObjectList != null)
            {
                foreach (ViewObject childviewobj in childViewObjectList)
                {
                    Rectangle rc = new Rectangle();
                    childviewobj.Init(_orgpointinscreen, _defscaleinscreen, null, objTextureLst, rc);
                }
            }
        }

        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //

        public override void Update(GameTime gameTime)
        {
            viewItm.Update(gameTime);

            if (childViewObjectList != null)
            {
                foreach (ViewObject viewObj in childViewObjectList)
                {
                    viewObj.Update(gameTime);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            viewItm.Draw(spriteBatch, model.GetAnimationDepth());

            if (childViewObjectList != null)
            {
                foreach (ViewObject viewObj in childViewObjectList)
                {
                    viewObj.Draw(spriteBatch);
                }
            }
        }

        public override void PlaySound()
        {
            // check need to play audio
            //play init sound
            if (_resLst[model.Type()].soundList.Count > 0)
            {
                int soundindex = model.GetSoundIndex();
                if (soundindex >= 0 && soundindex < _resLst[model.Type()].soundList.Count)
                {
                    float mastvol = SoundEffect.MasterVolume;
                    _resLst[model.Type()].soundList[0].Play(1, 0, 0);
                }

            }
        }
    }



    class SmokeViewObject : DefViewObject
    {
        List<AnimationInfo> animationList;
        List<ViewItem> viewItmList;

        SmokeModel model;

        Vector2 _orgpointinscreen;
        float _defscaleinscreen;

        public Vector2 OrgPointInScreen
        {
            get
            {
                return _orgpointinscreen;
            }
        }
        public float DefScaleInScreen
        {
            get
            {
                return _defscaleinscreen;
            }
        }

        public List<SpriteFont> ObjFontList
        {
            get
            {
                return _resLst[model.Type()].fontList;
            }
        }
        List<SpriteFont> fontlist;

        // screen rect
        public Rectangle screenRc = new Rectangle();

        public SmokeViewObject(ModelObject model1, Vector2 orgpointinscreen, float defscaleinscreen)
        {

        }

        public SmokeViewObject()
        {

        }

        Dictionary<ModelType, ObjectTexturesItem> _resLst;

        int bgxoff = 0;
        int bgyoff = 0;
        float bgscale = 1.0f;

        public override void Init(Vector2 orgpointinscreen, float defscaleinscreen, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle spaceInLogic)
        {
            if (model == null || (model.Type() != ModelType.KEYBORD && model.Type() != ModelType.KEYITEM))
            {
                _orgpointinscreen = orgpointinscreen;
                _defscaleinscreen = defscaleinscreen;
            }
            screenRc = spaceInLogic;

            model = (SmokeModel)model1;


            _orgpointinscreen = orgpointinscreen;
            _defscaleinscreen = defscaleinscreen;

            _resLst = objTextureLst;
            // try to calculate how may textures are needed by children

            // create view items for this object
            List<Texture2D> texturesList = objTextureLst[model.Type()].textureList;
            animationList = model.GetAnimationInfoList();
            viewItmList = new List<ViewItem>();


            int bgwidth = model.BgRcWidth;
            int bgheight = model.BgRcHight;
            if (bgwidth * 1.0f / bgheight > screenRc.Width * 1.0 / screenRc.Height)
            {
                // the text wider, should extend according height
                bgscale = screenRc.Height * 1.0f / bgheight;
                int offx = (int)((bgwidth * bgscale - screenRc.Width) / 2 / bgscale);
                bgxoff = offx;

                /*
                offx = model.XOffInBg - offx;
                offx = (int)(offx * scale);
                //offx = -offx;
                int centerx = (int)(offx + texturesList[i].Width * scale / 2);
                int centery = (int)(model.YOffInBg*scale + texturesList[i].Height*scale / 2);
                viewItm.bganimation.Position.X = centerx;
                viewItm.bganimation.Position.Y = centery;
                viewItm.bganimation.scale = scale;
                 */
            }
            else
            {
                // the texture is higher, should extend according width
                bgscale = screenRc.Width * 1.0f / bgwidth;

                int offy = (int)((bgheight * bgscale - screenRc.Height) / bgscale);
                bgyoff = offy;
                /*
                offy = model.YOffInBg - offy;
                offy = (int)(offy * scale);

                int centerx = (int)(model.XOffInBg * scale + texturesList[i].Width * scale / 2);
                int centery = (int)(offy + texturesList[i].Height* scale / 2);
                viewItm.bganimation.Position.X = centerx;
                viewItm.bganimation.Position.Y = centery;
                viewItm.bganimation.scale = scale;
                 */
            }

            for (int i = 0; i < texturesList.Count; i++)
            {
                AnimationInfo animationInfo = model.GetAnimationInfoList()[i];
                ViewItem viewItm = new ViewItem();
                {
                    viewItm.backGroundAnimation = true;
                    viewItm.bganimation = new Animation();
                        viewItm.bganimation.Initialize(
                            texturesList[i],
                            Vector2.Zero, (int)(texturesList[i].Width),
                            (int)(texturesList[i].Height),
                            1, 1, animationInfo.backColor,
                            1.0f/*model.GetSacle()*/, true);

                }
                viewItmList.Add(viewItm);
            }

        }

        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //
        float smokedeltay = 0;
        int smokeindex = 0;
        float smokescale = 1.0f;

        public override void Update(GameTime gameTime)
        {
            // prepare this time
            smokedeltay += 1;
            smokescale += 0.01f;
            if (smokedeltay > 10)
            {
                smokedeltay = 0;
                smokescale = 1.0f;
                smokeindex = (smokeindex + 1) % viewItmList.Count;
            }


            ViewItem viewItm = viewItmList[smokeindex];

            Vector2 smokelefttop = Vector2.Zero;
            smokelefttop.X = model.XOffInBg;
            smokelefttop.Y = model.YOffInBg;
            // translate it to screen
            smokelefttop.X -= bgxoff;
            smokelefttop.Y -= bgyoff;

            Vector2 smokecenter = Vector2.Zero;
            smokecenter.X = smokelefttop.X + viewItm.bganimation.FrameWidth / 2;
            smokecenter.Y = smokelefttop.Y + viewItm.bganimation.FrameHeight / 2;

            // adust center
            smokecenter.Y -= smokedeltay;


            // center after scaled
            smokecenter.X *= bgscale;
            smokecenter.Y *= bgscale;

            // animation scale
            viewItm.bganimation.scale = bgscale * smokescale;
            viewItm.bganimation.Position = smokecenter;
            viewItm.bganimation.Update(gameTime);

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            ViewItem viewItm = viewItmList[smokeindex];
            viewItm.bganimation.Draw(spriteBatch, model.GetAnimationDepth());
        }

        public override void PlaySound()
        {
            // check need to play audio
            //play init sound
            if (_resLst[model.Type()].soundList.Count > 0)
            {
                int soundindex = model.GetSoundIndex();
                if (soundindex >= 0 && soundindex < _resLst[model.Type()].soundList.Count)
                {
                    float mastvol = SoundEffect.MasterVolume;
                    _resLst[model.Type()].soundList[0].Play(1, 0, 0);
                }

            }
        }
    }



    class FireworkViewObject : DefViewObject
    {
        List<AnimationInfo> animationList;
        //List<AnimationEx> viewItmList;
        AnimationEx animation;

        FireworkModel model;

        Vector2 _orgpointinscreen;
        float _defscaleinscreen;

        public Vector2 OrgPointInScreen
        {
            get
            {
                return _orgpointinscreen;
            }
        }
        public float DefScaleInScreen
        {
            get
            {
                return _defscaleinscreen;
            }
        }

        public List<SpriteFont> ObjFontList
        {
            get
            {
                return _resLst[model.Type()].fontList;
            }
        }
        List<SpriteFont> fontlist;

        // screen rect
        public Rectangle screenRc = new Rectangle();

        public FireworkViewObject(ModelObject model1, Vector2 orgpointinscreen, float defscaleinscreen)
        {

        }

        public FireworkViewObject()
        {

        }

        Dictionary<ModelType, ObjectTexturesItem> _resLst;

        public override void Init(Vector2 orgpointinscreen, float defscaleinscreen, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle spaceInLogic)
        {
            if (model == null || (model.Type() != ModelType.KEYBORD && model.Type() != ModelType.KEYITEM))
            {
                _orgpointinscreen = orgpointinscreen;
                _defscaleinscreen = defscaleinscreen;
            }

            animation = new AnimationEx();
            model = (FireworkModel)model1;

            _orgpointinscreen = orgpointinscreen;
            _defscaleinscreen = defscaleinscreen;

            _resLst = objTextureLst;
            // try to calculate how may textures are needed by children

            // create view items for this object
            List<Texture2D> texturesList = objTextureLst[model.Type()].textureList;
            animationList = model.GetAnimationInfoList();
            AnimationInfo animationInfo = model.GetAnimationInfoList()[0];
            animation.Initialize(texturesList, Vector2.Zero,
                animationInfo.frameWidth, animationInfo.frameHeight,
                animationInfo.frameCount, animationInfo.frameTime, animationInfo.backColor,
                        model.GetSacle(), true);

            animation.Position = model.GetAbsolutePosition();
            animation.Position.X += (animation.FrameWidth / 2) * _defscaleinscreen;
            animation.Position.Y += (animation.FrameHeight / 2) * _defscaleinscreen;
            animation.scale = model.GetSacle() * _defscaleinscreen;
            //animation.Update(gameTime);
        }

        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //

        public override void Update(GameTime gameTime)
        {
            animation.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
                // check if the position is zero,
                // when button pause, it switch a animation, but update will never be called
               animation.Draw(spriteBatch, model.GetAnimationDepth());
  
        }

        public override void PlaySound()
        {
            // check need to play audio
            //play init sound
            if (_resLst[model.Type()].soundList.Count > 0)
            {
                int soundindex = model.GetSoundIndex();
                if (soundindex >= 0 && soundindex < _resLst[model.Type()].soundList.Count)
                {
                    float mastvol = SoundEffect.MasterVolume;
                    _resLst[model.Type()].soundList[0].Play(1, 0, 0);
                }

            }
        }
    }


    class KeyboardViewObject : DefViewObject
    {
        List<AnimationInfo> animationList;
        List<ViewItem> viewItmList;

        ModelObject model;
        List<ViewObject> childViewObjectList;

        Vector2 _orgpointinscreen;
        float _defscaleinscreen;

        // screen rect
        public Rectangle screenRc = new Rectangle();

        public KeyboardViewObject(ModelObject model1, Vector2 orgpointinscreen, float defscaleinscreen)
        {
            model = model1;
            List<ModelObject> childobjlst = model.GetChildrenObjects();
            if (childobjlst != null)
            {
                childViewObjectList = new List<ViewObject>();
                foreach (ModelObject obj in childobjlst)
                {
                    ViewObject viewobj = ViewObjectFactory.CreateViewObject(obj);

                    childViewObjectList.Add(viewobj);
                }
            }
        }

        Dictionary<ModelType, ObjectTexturesItem> _resLst;
        public override void Init(Vector2 orgpointinscreen, float defscaleinscreen, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle spaceInLogic)
        {
            _orgpointinscreen = orgpointinscreen;
            _defscaleinscreen = defscaleinscreen;

            _resLst = objTextureLst;
            // try to calculate how may textures are needed by children

            // create view items for this object
            List<Texture2D> texturesList = objTextureLst[model.Type()].textureList;
            animationList = model.GetAnimationInfoList();
            viewItmList = new List<ViewItem>();
            for (int i = 0; i < texturesList.Count; i++)
            {
                AnimationInfo animationInfo = model.GetAnimationInfoList()[i];
                ViewItem viewItm = new ViewItem();
                if (animationInfo.animation)
                {
                    viewItm.animation = new Animation();
                    viewItm.animation.Initialize(
                        texturesList[i],
                        Vector2.Zero, (int)(animationInfo.frameWidth),
                        (int)(animationInfo.frameHeight),
                        animationInfo.frameCount, animationInfo.frameTime, animationInfo.backColor,
                        model.GetSacle(), true);
                }
                else
                {
                    viewItm.backGroundAnimation = true;
                    viewItm.bganimation = new Animation();
                    if (animationInfo.frameHeight == 0)
                    {
                        viewItm.bganimation.Initialize(
                            texturesList[i],
                            Vector2.Zero, (int)(texturesList[i].Width/*animationInfo.frameWidth*/),
                            (int)(texturesList[i].Height/*animationInfo.frameHeight*/),
                            1/*animationInfo.frameCount*/, 1/*animationInfo.frameTime*/, animationInfo.backColor,
                            model.GetSacle(), true);


                        float scale = 1.0f;
                        if (texturesList[i].Width * 1.0f / texturesList[i].Height > screenRc.Width * 1.0 / screenRc.Height)
                        {
                            // the text wider, should extend according height
                            scale = screenRc.Height * 1.0f / texturesList[i].Height;

                            int offx = (int)((texturesList[i].Width * scale - screenRc.Width) / 2 / scale);
                            offx = (int)(offx * scale);
                            offx = -offx;
                            int centerx = (int)(offx + texturesList[i].Width * scale / 2);
                            int centery = screenRc.Height / 2;
                            viewItm.bganimation.Position.X = centerx;
                            viewItm.bganimation.Position.Y = centery;
                            viewItm.bganimation.scale = scale;

                        }
                        else
                        {
                            // the texture is higher, should extend according width
                            scale = screenRc.Width * 1.0f / texturesList[i].Width;

                            int offy = (int)((texturesList[i].Height * scale - screenRc.Height) / scale);
                            offy = (int)(offy * scale);
                            offy = -offy;
                            int centerx = screenRc.Width / 2;
                            int centery = (int)(offy + texturesList[i].Height * scale / 2);
                            viewItm.bganimation.Position.X = centerx;
                            viewItm.bganimation.Position.Y = centery;
                            viewItm.bganimation.scale = scale;

                        }
                    }
                    else
                    {
                        viewItm.bganimation.Initialize(
                            texturesList[i],
                            Vector2.Zero, animationInfo.frameWidth,
                            animationInfo.frameHeight,
                            animationInfo.frameCount, animationInfo.frameTime, animationInfo.backColor,
                            model.GetSacle(), true);


                        float scale = 1.0f;
                        if (animationInfo.frameWidth * 1.0f / animationInfo.frameHeight > screenRc.Width * 1.0 / screenRc.Height)
                        {
                            // the text wider, should extend according height
                            scale = screenRc.Height * 1.0f / animationInfo.frameHeight;

                            int offx = (int)((animationInfo.frameWidth * scale - screenRc.Width) / 2 / scale);
                            offx = (int)(offx * scale);
                            offx = -offx;
                            int centerx = (int)(offx + animationInfo.frameWidth * scale / 2);
                            int centery = screenRc.Height / 2;
                            viewItm.bganimation.Position.X = centerx;
                            viewItm.bganimation.Position.Y = centery;
                            viewItm.bganimation.scale = scale;

                        }
                        else
                        {
                            // the texture is higher, should extend according width
                            scale = screenRc.Width * 1.0f / animationInfo.frameWidth;

                            int offy = (int)((animationInfo.frameHeight * scale - screenRc.Height) / scale);
                            offy = (int)(offy * scale);
                            offy = -offy;
                            int centerx = screenRc.Width / 2;
                            int centery = (int)(offy + animationInfo.frameHeight * scale / 2);
                            viewItm.bganimation.Position.X = centerx;
                            viewItm.bganimation.Position.Y = centery;
                            viewItm.bganimation.scale = scale;

                        }

                    }


                }
                viewItmList.Add(viewItm);
            }
            //play init sound

            // left textures are for children
            if (childViewObjectList != null)
            {
                foreach (ViewObject childviewobj in childViewObjectList)
                {
                    Rectangle rc = new Rectangle();
                    childviewobj.Init(_orgpointinscreen, _defscaleinscreen, null, objTextureLst, rc);
                }

            }
        }

        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //

        public override void Update(GameTime gameTime)
        {
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            if (animationList[model.GetCurrentAnimationIndex()].animation)
            {
                viewItm.animation.Position = _orgpointinscreen +
                    model.GetAbsolutePosition();

                viewItm.animation.Position.X += (viewItm.animation.FrameWidth / 2);

                // calculate the center y
                viewItm.animation.Position.Y =
                    model.GetAbsolutePosition().Y + model.GetSpace().Height - (int)(model.GetSpace().Height * model.GetSacle());
                viewItm.animation.Position.Y += viewItm.animation.FrameHeight * 1.0f / 2 * model.GetSacle();
                //viewItm.animation.Position.Y += (viewItm.animation.FrameHeight / 2) ;
                viewItm.animation.scale = model.GetSacle();
                viewItm.animation.Update(gameTime);
            }
            else
            {
                if (viewItm.backGroundAnimation)
                {
                    viewItm.bganimation.Update(gameTime);
                }
                else
                {
                    viewItm.staticBackground.Update(gameTime);
                }
            }

            if (childViewObjectList != null)
            {
                foreach (ViewObject viewObj in childViewObjectList)
                {
                    viewObj.Update(gameTime);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            if (animationList[model.GetCurrentAnimationIndex()].animation)
            {
                viewItm.animation.Draw(spriteBatch, model.GetAnimationDepth());
            }
            else
            {
                if (viewItm.backGroundAnimation)
                {
                    viewItm.bganimation.Draw(spriteBatch, model.GetAnimationDepth());
                }
                else
                {
                    viewItm.staticBackground.Draw(spriteBatch, model.GetAnimationDepth());
                }
            }
            if (childViewObjectList != null)
            {
                foreach (ViewObject viewObj in childViewObjectList)
                {
                    viewObj.Draw(spriteBatch);
                }
            }
        }

        public override void PlaySound()
        {
            // check need to play audio
            //play init sound
            if (_resLst[model.Type()].soundList.Count > 0)
            {//SoundEffect.MasterVolume
                //SoundEffect.
                //SoundEf
                int soundindex = model.GetSoundIndex();
                if (soundindex >= 0 && soundindex < _resLst[model.Type()].soundList.Count)
                {
                    float mastvol = SoundEffect.MasterVolume;
                    _resLst[model.Type()].soundList[0].Play(1, 0, 0);
                }

            }
        }

    }



    abstract class UIControlViewObject : ViewObject
    {
        protected Vector2 _orgpoint;
        protected float _defscale;
        protected List<AnimationInfo> animationList;
        protected List<ViewItem> viewItmList;
        protected List<SpriteFont> fontList;
        protected List<Texture2D> texturesList;

        ModelObject model;
        Dictionary<ModelType, ObjectTexturesItem> _resLst;


        public override void Init(Vector2 orgpoint, float defscale, ModelObject model,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            this.model = model;
            _resLst = objTextureLst;
            _orgpoint = orgpoint;
            _defscale = defscale;

            texturesList = objTextureLst[model.Type()].textureList;
            fontList = objTextureLst[model.Type()].fontList;
            animationList = model.GetAnimationInfoList();

            // background
            viewItmList = new List<ViewItem>();
            for (int i = 0; i < texturesList.Count; i++)
            {
                AnimationInfo animationInfo = animationList[i];
                ViewItem viewItm = new ViewItem();
                viewItm.animation = new Animation();
                viewItm.animation.Initialize(
                    texturesList[i],
                    Vector2.Zero, animationInfo.frameWidth, animationInfo.frameHeight,
                    animationInfo.frameCount, animationInfo.frameTime, animationInfo.backColor,
                    model.GetSacle(), true);

                viewItm.animation.Position = _orgpoint + model.GetAbsolutePosition() * _defscale;
                viewItm.animation.Position.X += (viewItm.animation.FrameWidth * model.GetSacle() * _defscale / 2);
                viewItm.animation.Position.Y += (viewItm.animation.FrameHeight * model.GetSacle() * _defscale / 2);
                viewItm.animation.scale = model.GetSacle() * _defscale;
                GameTime gametime = new GameTime();
                viewItm.animation.Update(gametime);

                viewItmList.Add(viewItm);
            }
        }


        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];

            viewItm.animation.Draw(spriteBatch, model.GetAnimationDepth());

            /*
            spriteBatch.DrawString(fontList[0], value, pos1, Color.Yellow, 0, Vector2.Zero, 1,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
             */
        }

        public override void PlaySound()
        {
            if (_resLst[model.Type()].soundList.Count > 0)
            {//SoundEffect.MasterVolume
                //SoundEffect.
                //SoundEf
                int soundindex = model.GetSoundIndex();
                if (soundindex >= 0 && soundindex < _resLst[model.Type()].soundList.Count)
                {
                    float mastvol = SoundEffect.MasterVolume;
                    _resLst[model.Type()].soundList[0].Play(1, 0, 0);
                }

            }
        }
    }

    class CheckBoxViewObject : UIControlViewObject
    {
        CheckBoxModel model;

        Vector2 contentoff;

        public CheckBoxViewObject(ModelObject model1)
        {
            model = (CheckBoxModel)model1;
        }


        public override void Init(Vector2 orgpoint, float defscale, ModelObject model,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            base.Init(orgpoint, defscale, this.model, objTextureLst, space);

            // check box string offset
            contentoff = model.GetAbsolutePosition() * _defscale + _orgpoint;
            contentoff.X += 80 * _defscale;
            contentoff.Y += 50 * _defscale;

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // this rc is logic rc

            // draw score
            Vector2 pos1 = contentoff;
            string value = this.model.Content;
            //spriteBatch.DrawString(fontList[0], value, pos1, Color.White, 0, Vector2.Zero, 1,
            //    SpriteEffects.None,  model.GetAnimationDepth() - 0.02f);
            spriteBatch.DrawString(fontList[0], value, pos1, Color.Yellow, 0, Vector2.Zero, 1,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
        }
    }


    // draw the score myself
    class ScoreBoardViewObject : CommonViewObject
    {
        ScroeBoardModel model;
        /*
        List<AnimationInfo> animationList;
        List<ViewItem> viewItmList;
        List<SpriteFont> fontList;
         Vector2 _orgpoint;
        float _defscale;
 
        */

        Vector2 scoreposition;


        public ScoreBoardViewObject(ModelObject model1)
        {
            model = (ScroeBoardModel)model1;
        }

        public override void Init(Vector2 orgpoint, float defscale, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            base.Init(orgpoint, defscale, model, objTextureLst, space);

            /*
            model = (ScroeBoardModel)model1;

            _orgpoint = orgpoint;
            _defscale = defscale;

            fontList = objTextureLst[model.Type()].fontList;


            // create view items for this object
            List<Texture2D> texturesList = objTextureLst[model.Type()].textureList;
            animationList = model.GetAnimationInfoList();

            // background
            viewItmList = new List<ViewItem>();
            for (int i = 0; i < texturesList.Count; i++)
            {
                AnimationInfo animationInfo = model.GetAnimationInfoList()[i];
                ViewItem viewItm = new ViewItem();
                if (animationInfo.animation)
                {
                    viewItm.animation = new Animation();
                    viewItm.animation.Initialize(
                        texturesList[i],
                        Vector2.Zero, animationInfo.frameWidth, animationInfo.frameHeight,
                        animationInfo.frameCount, animationInfo.frameTime, animationInfo.backColor,
                        model.GetSacle(), true);
                }
                else
                {
                    viewItm.staticBackground = new StaticBackground2();
                    viewItm.staticBackground.Initialize(
                        texturesList[i],
                        orgpoint,
                        (int)(space.Width*defscale), (int)(space.Height*defscale), 0);
                }
                viewItmList.Add(viewItm);
            }
            */
            scoreposition = model.GetAbsolutePosition() * DefScaleInScreen + OrgPointInScreen;
            scoreposition.X += 20 * DefScaleInScreen;
            scoreposition.Y += 25 * DefScaleInScreen;


        }

        public override void Update(GameTime gameTime)
        {
            return;
            //base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];

            // this rc is logic rc
            Rectangle rc = model.GetSpace();
            rc.Height = 63; // same height with hitboard
            rc.Width = 200;
            rc.Width = (int)(rc.Width * DefScaleInScreen);
            rc.Height = (int)(rc.Height * DefScaleInScreen);
            rc.X += (int)scoreposition.X; // scoreposition is position in local view
            rc.Y += (int)scoreposition.Y;

            Color color = new Color(167, 167, 167);
            color.A = 10;

            color = new Color(253, 253, 253);

            Color color1 = Color.Yellow;
            color1.A = 80;
            //DrawRectangle2(spriteBatch, rc, color1);
            //DrawRectangle(spriteBatch, rc, Color.Blue);


            // draw score
            Vector2 pos1 = scoreposition;
            pos1.Y += 10 * DefScaleInScreen;
            pos1.X += 10 * DefScaleInScreen;
            string value = this.model.TotalScore.ToString();
            //spriteBatch.DrawString(fontList[0], value, pos1, Color.White, 0, Vector2.Zero, 1,
            //    SpriteEffects.None,  model.GetAnimationDepth() - 0.02f);
            spriteBatch.DrawString(ObjFontList[0], "SCORE: " + value, pos1, Color.Yellow, 0, Vector2.Zero, 1,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);

            value = "Level: " + model.GetLevel().ToString();
            //spriteBatch.DrawString(fontList[0], value, pos1, Color.White, 0, Vector2.Zero, 1,
            //    SpriteEffects.None,  model.GetAnimationDepth() - 0.02f);
            pos1.Y += 50 * DefScaleInScreen;
            spriteBatch.DrawString(base.ObjFontList[0], value, pos1, Color.Yellow, 0, Vector2.Zero, 1,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);

        }

        public override void PlaySound()
        {
        }

    }


    // draw the score myself
    class KeyItemViewObject : CommonViewObject
    {
        KeyItemModel model;
        /*
        List<AnimationInfo> animationList;
        List<ViewItem> viewItmList;
        List<SpriteFont> fontList;
                 Vector2 _orgpoint;
        float _defscale;
 
        */

        Vector2 scoreposition;


        public KeyItemViewObject(ModelObject model1/*, Vector2 orgpoint, float defscale*/)
        {
            model = (KeyItemModel)model1;
            /*
            _orgpoint = orgpoint;
            _defscale = defscale;
             */
        }

        public override void Init(Vector2 orgpoint, float defscale, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            // model = (KeyItemModel)model1;
            base.Init(orgpoint, defscale, model, objTextureLst, space);

            /*
            _orgpoint = orgpoint;
            _defscale = defscale;

            fontList = objTextureLst[model.Type()].fontList;


            // create view items for this object
            List<Texture2D> texturesList = objTextureLst[model.Type()].textureList;
            animationList = model.GetAnimationInfoList();

            // background
            viewItmList = new List<ViewItem>();
            for (int i = 0; i < texturesList.Count; i++)
            {
                AnimationInfo animationInfo = model.GetAnimationInfoList()[i];
                ViewItem viewItm = new ViewItem();
                if (animationInfo.animation)
                {
                    viewItm.animation = new Animation();
                    viewItm.animation.Initialize(
                        texturesList[i],
                        Vector2.Zero, animationInfo.frameWidth, animationInfo.frameHeight,
                        animationInfo.frameCount, animationInfo.frameTime, animationInfo.backColor,
                        model.GetSacle(), true);
                }
                else
                {
                    viewItm.staticBackground = new StaticBackground2();
                    viewItm.staticBackground.Initialize(
                        texturesList[i],
                        orgpoint,
                        (int)(space.Width * defscale), (int)(space.Height * defscale), 0);
                }
                viewItmList.Add(viewItm);
            }
            */

            scoreposition = model.GetAbsolutePosition() * DefScaleInScreen + OrgPointInScreen;
            scoreposition.X += 0 * DefScaleInScreen;
            scoreposition.Y += 0 * DefScaleInScreen;

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            /*
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            if (animationList[model.GetCurrentAnimationIndex()].animation)
            {
                viewItm.animation.Position = _orgpoint + model.GetAbsolutePosition() ;

                viewItm.animation.Position.X += (viewItm.animation.FrameWidth *model.GetSacle() / 2) ;
                viewItm.animation.Position.Y += (viewItm.animation.FrameHeight * model.GetSacle() / 2);
                viewItm.animation.scale = model.GetSacle() ;
                viewItm.animation.Update(gameTime);
            }
            else
            {
                viewItm.staticBackground.Update(gameTime);
            }
             */

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            /*
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];

            viewItm.animation.Draw(spriteBatch, model.GetAnimationDepth());
            */

            // this rc is logic rc
            Rectangle rc = model.GetSpace();
            rc.Height = 63; // same height with hitboard
            rc.Width = 200;
            rc.Width = (int)(rc.Width * DefScaleInScreen);
            rc.Height = (int)(rc.Height * DefScaleInScreen);
            rc.X += (int)scoreposition.X; // scoreposition is position in local view
            rc.Y += (int)scoreposition.Y;

            Color color = new Color(167, 167, 167);
            color.A = 10;

            color = new Color(253, 253, 253);

            Color color1 = Color.Yellow;
            color1.A = 80;
            //DrawRectangle2(spriteBatch, rc, color1);
            //DrawRectangle(spriteBatch, rc, Color.Blue);


            // draw score
            Vector2 pos1 = scoreposition;
            pos1.Y += 10 * DefScaleInScreen;
            pos1.X += 10 * DefScaleInScreen;
            string value = this.model.Conent.ToString();
            //spriteBatch.DrawString(fontList[0], value, pos1, Color.White, 0, Vector2.Zero, 1,
            //    SpriteEffects.None,  model.GetAnimationDepth() - 0.02f);
            spriteBatch.DrawString(ObjFontList[0], value, pos1, Color.Yellow, 0, Vector2.Zero, 1,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
        }
    }


    // draw the score myself
    class TimeBoardViewObject : CommonViewObject
    {
        TimeBoardModel model;
        Vector2 scoreposition;

        public TimeBoardViewObject(ModelObject model1)
        {
            model = (TimeBoardModel)model1;
        }

        public override void Init(Vector2 orgpoint, float defscale, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            model = (TimeBoardModel)model1;

            base.Init(orgpoint, defscale, model, objTextureLst, space);
            /*
            _orgpoint = orgpoint;
            _defscale = defscale;

            fontList = objTextureLst[model.Type()].fontList;


            // create view items for this object
            List<Texture2D> texturesList = objTextureLst[model.Type()].textureList;
            // background
            */
            scoreposition = model.GetAbsolutePosition() * DefScaleInScreen + OrgPointInScreen;
            scoreposition.X += 20 * DefScaleInScreen;
            scoreposition.Y += 25 * DefScaleInScreen;
        }

        public override void Update(GameTime gameTime)
        {

        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            // this rc is logic rc
            Rectangle rc = model.GetSpace();
            rc.Height = 63; // same height with hitboard
            rc.Width = 200;
            rc.Width = (int)(rc.Width * DefScaleInScreen);
            rc.Height = (int)(rc.Height * DefScaleInScreen);
            rc.X += (int)scoreposition.X; // scoreposition is position in local view
            rc.Y += (int)scoreposition.Y;

            Color color = new Color(167, 167, 167);
            color.A = 10;

            color = new Color(253, 253, 253);

            Color color1 = Color.Blue;
            color1.A = 10;

            // draw score
            Vector2 pos1 = scoreposition;
            pos1.Y += 10 * DefScaleInScreen;
            pos1.X += 10 * DefScaleInScreen;
            string value = this.model.LeftTime.ToString();

            spriteBatch.DrawString(base.ObjFontList[0], "Left Time: " + value, pos1, Color.Yellow, 0, Vector2.Zero, 1,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
        }
    }



    // draw the score myself
    class LevelUpBoardViewObject : CommonViewObject
    {
        LevelUpBoardModel model;
        Vector2 scoreposition;
        Color color1 = Color.Orange;
        public LevelUpBoardViewObject(ModelObject model1)
        {
            model = (LevelUpBoardModel)model1;
            //color1.A = 95;
        }

        public override void Init(Vector2 orgpoint, float defscale, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            model = (LevelUpBoardModel)model1;

            base.Init(orgpoint, defscale, model, objTextureLst, space);

            scoreposition = model.GetAbsolutePosition() * DefScaleInScreen + OrgPointInScreen;
            scoreposition.X += 20 * DefScaleInScreen;
            scoreposition.Y += 25 * DefScaleInScreen;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!model.Show())
            {
                return;
            }
            // this rc is logic rc
            Rectangle rc = model.GetSpace();
            rc.Height = 63; // same height with hitboard
            rc.Width = 200;
            rc.Width = (int)(rc.Width * DefScaleInScreen);
            rc.Height = (int)(rc.Height * DefScaleInScreen);
            rc.X += (int)scoreposition.X; // scoreposition is position in local view
            rc.Y += (int)scoreposition.Y;


            // draw score
            Vector2 pos1 = scoreposition;
            pos1.Y += 10 * DefScaleInScreen;
            pos1.X += 10 * DefScaleInScreen;

            spriteBatch.DrawString(base.ObjFontList[0], "Level Up", pos1, color1,
                model.Rotate, Vector2.Zero, model.Scale,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
        }
    }


    // draw the score myself
    class LostDuckBoardViewObject : CommonViewObject
    {
        LostDuckBoardModel model;
        Vector2 scoreposition;
        /*
        List<SpriteFont> fontList;


        Vector2 _orgpoint;
        float _defscale;
        */

        public LostDuckBoardViewObject(ModelObject model1)
        {
            model = (LostDuckBoardModel)model1;
        }

        public override void Init(Vector2 orgpoint, float defscale, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            base.Init(orgpoint, defscale, model, objTextureLst, space);
            /*
            model = (LostDuckBoardModel)model1;

            _orgpoint = orgpoint;
            _defscale = defscale;

            fontList = objTextureLst[model.Type()].fontList;


            // create view items for this object
            List<Texture2D> texturesList = objTextureLst[model.Type()].textureList;
            // background
            */
            scoreposition = model.GetAbsolutePosition() * DefScaleInScreen + OrgPointInScreen;
            scoreposition.X += 20 * DefScaleInScreen;
            scoreposition.Y += 25 * DefScaleInScreen;
        }

        public override void Update(GameTime gameTime)
        {

        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            // this rc is logic rc
            Rectangle rc = model.GetSpace();
            rc.Height = 63; // same height with hitboard
            rc.Width = 200;
            rc.Width = (int)(rc.Width * DefScaleInScreen);
            rc.Height = (int)(rc.Height * DefScaleInScreen);
            rc.X += (int)scoreposition.X; // scoreposition is position in local view
            rc.Y += (int)scoreposition.Y;

            Color color = new Color(167, 167, 167);
            color.A = 10;

            color = new Color(253, 253, 253);

            Color color1 = Color.Blue;
            color1.A = 10;

            // draw score
            Vector2 pos1 = scoreposition;
            pos1.Y += 10 * DefScaleInScreen;
            pos1.X += 10 * DefScaleInScreen;
            string value = this.model.LostDuckCount.ToString();

            spriteBatch.DrawString(this.ObjFontList[0], "Lost Duck: " + value, pos1, Color.Yellow, 0, Vector2.Zero, 1,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
        }

    }



    // draw the score myself
    class HitBoardViewObject : CommonViewObject
    {
        HitBoardModel model;
        Vector2 scoreposition;
        /*
        List<AnimationInfo> animationList;
        List<ViewItem> viewItmList;
        List<SpriteFont> fontList;


        Vector2 _orgpoint;
        float _defscale;
        */
        public HitBoardViewObject(ModelObject model1)
        {
            model = (HitBoardModel)model1;
        }

        public override void Init(Vector2 orgpoint, float defscale, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            base.Init(orgpoint, defscale, model, objTextureLst, space);
            /*
            model = (HitBoardModel)model1;

            _orgpoint = orgpoint;
            _defscale = defscale;

            fontList = objTextureLst[model.Type()].fontList;


            // create view items for this object
            List<Texture2D> texturesList = objTextureLst[model.Type()].textureList;
            animationList = model.GetAnimationInfoList();

            // background
            viewItmList = new List<ViewItem>();
            for (int i = 0; i < texturesList.Count; i++)
            {
                AnimationInfo animationInfo = model.GetAnimationInfoList()[i];
                ViewItem viewItm = new ViewItem();
                if (animationInfo.animation)
                {
                    viewItm.animation = new Animation();
                    viewItm.animation.Initialize(
                        texturesList[i],
                        Vector2.Zero, animationInfo.frameWidth, animationInfo.frameHeight,
                        animationInfo.frameCount, animationInfo.frameTime, animationInfo.backColor,
                        model.GetSacle(), true);
                }
                else
                {
                    viewItm.staticBackground = new StaticBackground2();
                    viewItm.staticBackground.Initialize(
                        texturesList[i],
                        orgpoint,
                        (int)(space.Width * defscale), (int)(space.Height * defscale), 0);
                }
                viewItmList.Add(viewItm);
            }
            */
            scoreposition = model.GetAbsolutePosition() * DefScaleInScreen + OrgPointInScreen;
            scoreposition.X += 20 * DefScaleInScreen;
            scoreposition.Y += 25 * DefScaleInScreen;
        }

        public override void Update(GameTime gameTime)
        {
            /*
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            if (animationList[model.GetCurrentAnimationIndex()].animation)
            {
                viewItm.animation.Position = _orgpoint + model.GetAbsolutePosition() * _defscale;

                viewItm.animation.Position.X += (viewItm.animation.FrameWidth / 2) * _defscale;
                viewItm.animation.Position.Y += (viewItm.animation.FrameHeight / 2) * _defscale;
                viewItm.animation.scale = model.GetSacle() * _defscale;
                viewItm.animation.Update(gameTime);
            }
            else
            {
                viewItm.staticBackground.Update(gameTime);
            }

            scoreposition = model.GetAbsolutePosition() * _defscale + _orgpoint;
            scoreposition.X += 20 * _defscale;
            scoreposition.Y += 25 * _defscale;
             */
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle rc = model.GetSpace();
            rc.Width = (int)(rc.Width * DefScaleInScreen);
            rc.Height = (int)(rc.Height * DefScaleInScreen);
            rc.X += (int)scoreposition.X;
            rc.Y += (int)scoreposition.Y;

            Color color = Color.Blue;
            color.A = 10;
            //DrawRectangle2(spriteBatch, rc, color);
            //DrawRectangle(spriteBatch, rc, Color.Blue);

            // draw score
            Vector2 pos1 = scoreposition;
            pos1.Y += 10 * DefScaleInScreen;
            pos1.X += 10 * DefScaleInScreen;
            string value = "Hit Duck: " + model.GetHitCount().ToString();
            //spriteBatch.DrawString(fontList[0], value, pos1, Color.White, 0, Vector2.Zero, 1,
            //    SpriteEffects.None,  model.GetAnimationDepth() - 0.02f);
            spriteBatch.DrawString(base.ObjFontList[0], value, pos1, Color.Yellow, 0, Vector2.Zero, 1,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);

        }

    }


    // draw the score myself
    class MenuItemViewObject : CommonViewObject
    {
        MenuItemModel model;
        Vector2 menuContentPos;

        public MenuItemViewObject(ModelObject model1)
        {
            model = (MenuItemModel)model1;
        }

        int fontindex = 0;
        public override void Init(Vector2 orgpoint, float defscale, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            base.Init(orgpoint, defscale, model, objTextureLst, space);
            menuContentPos = model.GetAbsolutePosition() * DefScaleInScreen + OrgPointInScreen;

            menuContentPos.X += (120 - model.Conent.Length * 10 ) * DefScaleInScreen;
            menuContentPos.Y += 60 * DefScaleInScreen;


            
            float fontindexcheck = 0;
            fontindexcheck += 1/DefScaleInScreen - 1;
            if(fontindexcheck > 0 && fontindexcheck <= 1.0)
            {
                fontindex = 1;
            }
            if(fontindexcheck > 1.0 && fontindexcheck <= 2.0)
            {
                fontindex = 2;
            }
            if (fontindex >= base.ObjFontList.Count)
            {
                fontindex = base.ObjFontList.Count - 1;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // draw score
            Vector2 pos1 = menuContentPos;
            Rectangle space = model.GetSpace();
            //.Y += (space.Height/2 - 60) * _defscale;
            //pos1.X += 0 * _defscale;
            string value = this.model.Conent.ToString();
            spriteBatch.DrawString(base.ObjFontList[fontindex], value, pos1, Color.Blue, 0, Vector2.Zero, 1,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
        }

    }


    // draw the score myself
    class ScoreListBoardViewObject : CommonViewObject
    {
        ScroeListBoardModel model;
        Vector2 scoreposition;
        /*
        List<AnimationInfo> animationList;
        List<ViewItem> viewItmList;
        List<SpriteFont> fontList;


        Vector2 _orgpoint;
        float _defscale;
        */

        public ScoreListBoardViewObject(ModelObject model1)
        {
            model = (ScroeListBoardModel)model1;
        }

        public override void Init(Vector2 orgpoint, float defscale, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            base.Init(orgpoint, defscale, model, objTextureLst, space);

            scoreposition = model.GetAbsolutePosition() * DefScaleInScreen + OrgPointInScreen;
            scoreposition.X += 0 * DefScaleInScreen;
            scoreposition.Y += 0 * DefScaleInScreen;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            //ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            //viewItm.animation.Draw(spriteBatch, model.GetAnimationDepth());
            // draw score
            Vector2 pos1 = scoreposition;
            pos1.Y += 10 * DefScaleInScreen;
            pos1.X += 10 * DefScaleInScreen;
            //string value = this.model.TotalScore.ToString();
            //spriteBatch.DrawString(fontList[0], value, pos1, Color.White, 0, Vector2.Zero, 1,
            //    SpriteEffects.None,  model.GetAnimationDepth() - 0.02f);
            Vector2 pos2 = pos1;
            pos2.X += 150 * DefScaleInScreen;
            spriteBatch.DrawString(base.ObjFontList[0], "Top 10 Score List",
                pos2, Color.Yellow, 0, Vector2.Zero, 1,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);

            pos1.Y += 60 * DefScaleInScreen;

            var result = model.ScoreList.OrderByDescending(c => c.Key);
            foreach (KeyValuePair<int, string> pair in result)
            {
                spriteBatch.DrawString(base.ObjFontList[1], pair.Value,
                    pos1, Color.Yellow, 0, Vector2.Zero, 1,
                    SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
                pos2 = pos1;
                pos2.X += 500 * DefScaleInScreen;
                spriteBatch.DrawString(base.ObjFontList[1], pair.Key.ToString(),
                    pos2, Color.Yellow, 0, Vector2.Zero, 1,
                    SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
                pos1.Y += 30 * DefScaleInScreen;
            }

            pos1.Y += 30 * DefScaleInScreen;
            pos2 = pos1;
            pos2.X += 150 * DefScaleInScreen;
            spriteBatch.DrawString(base.ObjFontList[0], "Top 10 Level List",
                pos2, Color.Yellow, 0, Vector2.Zero, 1,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);

            pos1.Y += 60 * DefScaleInScreen;

            result = model.LevelList.OrderByDescending(c => c.Key);
            foreach (KeyValuePair<int, string> pair in result)
            {
                spriteBatch.DrawString(base.ObjFontList[1], pair.Value,
                    pos1, Color.Yellow, 0, Vector2.Zero, 1,
                    SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
                pos2 = pos1;
                pos2.X += 500 * DefScaleInScreen;
                spriteBatch.DrawString(base.ObjFontList[1], pair.Key.ToString(),
                    pos2, Color.Yellow, 0, Vector2.Zero, 1,
                    SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
                pos1.Y += 30 * DefScaleInScreen;
            }

        }
    }
}