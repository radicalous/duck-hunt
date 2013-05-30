using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

using System.Collections.Generic;

using DuckHuntCommon;
using GameCommon;

namespace DuckHunt
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        //DuckHuntGameControler controler;
        DuckHuntGameControler controler;

        // input

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;

        //Mouse states used to track Mouse button press
        MouseState currentMouseState;
        MouseState previousMouseState;

        // A movement speed for the player
        float playerMoveSpeed;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            controler = new DuckHuntGameControler();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Rectangle viewScene = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            //controler.Initialize(viewScene);
            controler.Initialize(viewScene);

            TouchPanel.EnabledGestures = GestureType.Tap/*|GestureType.DoubleTap|GestureType.Pinch|GestureType.Hold*/;

            base.Initialize();

            IsMouseVisible = true;
            //IsMouseVisible = false;

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            controler.LoadContent(Content);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            controler.Update(gameTime);
            HuntDuck(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            // Start drawing
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            controler.Draw(_spriteBatch, gameTime);

            // Stop drawing
            _spriteBatch.End(); 
        }

        List<Vector2> pointpositions = new List<Vector2>();
        HashSet<Vector2> pointslist = new HashSet<Vector2>();

        ButtonState lastButtonState = new ButtonState();
        private void HuntDuck(GameTime gameTime)
        {
            // Get Thumbstick Controls
            /*
            player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

            // Use the Keyboard / Dpad
            if (currentKeyboardState.IsKeyDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                player.Position.X -= playerMoveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                player.Position.X += playerMoveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Up) || currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                player.Position.Y -= playerMoveSpeed;
            }


            if (currentKeyboardState.IsKeyDown(Keys.Down) || currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                player.Position.Y += playerMoveSpeed;
            }

            // Make sure that the player does not go out of bounds
            player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height);
            */

            pointpositions.Clear();
            // Windows 8 Touch Gestures for MonoGame
            while (TouchPanel.IsGestureAvailable)
            {
                IsMouseVisible = false;

                GestureSample gesture = TouchPanel.ReadGesture();

                if (gesture.GestureType == GestureType.Tap)
                {
                    /*
                    Vector2 off = gesture.Position-lastpos;
                    if(gameTime.ElapsedGameTime.Milliseconds - lastinputtime.ElapsedGameTime.Milliseconds < 20 || (off.Length() < 1))
                    {
                        return ;
                    }
                    lastinputtime = gameTime;
                    lastpos = gesture.Position;
                     */
                    pointpositions.Add(gesture.Position);
                }
                /*
            else if (gesture.GestureType == GestureType.Pinch)
            {
                if (gesture.Delta.X * gesture.Delta.X + gesture.Delta.Y * gesture.Delta.Y <= 0.00001)
                {
                    pointslist.Add(gesture.Position);
                }
                else
                {
                    pointpositions.Add(gesture.Position); 
                }
                if (gesture.Delta2.X * gesture.Delta2.X + gesture.Delta2.Y * gesture.Delta2.Y <= 0.00001)
                {
                    pointslist.Add(gesture.Position2);
                }
                else
                {
                    pointpositions.Add(gesture.Position2);
                }
            }
                 */
            }
            /*
            pointpositions.Clear();
            foreach (Vector2 pos in pointslist)
            {
                pointpositions.Add(pos);
            }
             */
            if (pointpositions.Count > 0)
            {
                controler.Click(pointpositions);
                pointpositions.Clear();

                return;
            }

            //Get Mouse State then Capture the Button type and Respond Button Press

            currentMouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
            if (currentMouseState.LeftButton == ButtonState.Released && lastButtonState == ButtonState.Pressed)
            {
                IsMouseVisible = true;

                pointpositions.Add(mousePosition);
                controler.Click(pointpositions);
                pointpositions.Clear();
            }
            lastButtonState = currentMouseState.LeftButton;
        } 

    }
}
