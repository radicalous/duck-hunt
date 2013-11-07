using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

using System.Collections.Generic;
using Windows.ApplicationModel.Core;

using DuckHuntCommon;
using GameCommon;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

using StorageSampleREST;

namespace DuckHunt
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        public static GraphicsDevice graphicDevice;

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

        public enum WindowState { Full = 0, Snap};

        WindowState windState = WindowState.Full;
        Windows.Foundation.Rect _fullWindowBounds;

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
            graphicDevice = GraphicsDevice;

            Rectangle viewScene = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            //controler.Initialize(viewScene);
            controler.Initialize(viewScene);

            TouchPanel.EnabledGestures = GestureType.Tap /*| GestureType.FreeDrag | GestureType.DragComplete /*|GestureType.DoubleTap|GestureType.Pinch|GestureType.Hold*/;

            base.Initialize();

            IsMouseVisible = true;
            //IsMouseVisible = false;

            /*
            Window.Current.SizeChanged += OnWindowSizeChanged;

            SettingsPane.GetForCurrentView().CommandsRequested += SettingCharmManager_CommandsRequested;
            */
            _windowBounds = Windows.UI.Xaml.Window.Current.Bounds;
            _fullWindowBounds = _windowBounds;

            Windows.UI.Xaml.Window.Current.SizeChanged += OnWindowSizeChanged;
            Windows.UI.ApplicationSettings.SettingsPane.GetForCurrentView().CommandsRequested += SettingCharmManager_CommandsRequested;
        
            CoreApplication.Suspending += CoreApplication_Suspending;
            CoreApplication.Resuming += CoreApplication_Resuming;
        
        }

        Windows.Foundation.Rect _windowBounds;

        double _settingsWidth = 346;
        Windows.UI.Xaml.Controls.Primitives.Popup _settingsPopup;

        void OnWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            _windowBounds = Windows.UI.Xaml.Window.Current.Bounds;

            if (_windowBounds.Width == _fullWindowBounds.Width && _windowBounds.Height == _fullWindowBounds.Height)
            {
                windState = WindowState.Full;
                controler.Pause(windState == WindowState.Snap);
            }
            else
            {
                windState = WindowState.Snap;
                controler.Pause(windState == WindowState.Snap);
            }
        }

        //
        private void SettingCharmManager_CommandsRequested(
            Windows.UI.ApplicationSettings.SettingsPane sender, 
            Windows.UI.ApplicationSettings.SettingsPaneCommandsRequestedEventArgs args)
        {
            // UICommandInvokedHandler handler = new UICommandInvokedHandler(onPrivacyPolicyCommand);
            //  args.Request.ApplicationCommands.Add(new SettingsCommand("privacypolicy", "Privacy policy", handler));


            Windows.UI.ApplicationSettings.SettingsCommand cmd = new Windows.UI.ApplicationSettings.SettingsCommand("Settings", "Privacy Policy", (x) =>
            {
                _settingsPopup = new Windows.UI.Xaml.Controls.Primitives.Popup();
                _settingsPopup.Closed += OnPopupClosed;
                Windows.UI.Xaml.Window.Current.Activated += OnWindowActivated;
                _settingsPopup.IsLightDismissEnabled = true;
                _settingsPopup.Width = _settingsWidth;
                _settingsPopup.Height = _windowBounds.Height;

                PrivacyPolicy mypane = new PrivacyPolicy();
                mypane.Width = _settingsWidth;
                mypane.Height = _windowBounds.Height;

                _settingsPopup.Child = mypane;
                _settingsPopup.SetValue(Windows.UI.Xaml.Controls.Canvas.LeftProperty, _windowBounds.Width - _settingsWidth);
                _settingsPopup.SetValue(Windows.UI.Xaml.Controls.Canvas.TopProperty, 0);
                _settingsPopup.IsOpen = true;

               // Windows.UI.Xaml.Window.Current.Content = mypane;
            });

            args.Request.ApplicationCommands.Add(cmd);

        }

        private void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                _settingsPopup.IsOpen = false;
            }
        }

        void OnPopupClosed(object sender, object e)
        {
            Windows.UI.Xaml.Window.Current.Activated -= OnWindowActivated;
        }




        static void CoreApplication_Resuming(object sender, object e)
        {
            // coming back from suspend, probably don't need to do anything as current state is in memory
            
        }

        static void CoreApplication_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            // suspending, save appropriate game and user state
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
            CollectInput(gameTime);
            controler.Update(gameTime);
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

            //controler.Draw(_spriteBatch, gameTime);
            controler.CurrentLayer().visitForDraw(_spriteBatch);

            // Stop drawing
            _spriteBatch.End(); 
        }

        Vector2 lastpos = new Vector2();
        GameTime lastinputtime = new GameTime();
        List<Vector2> pointpositions = new List<Vector2>();
        HashSet<Vector2> pointslist = new HashSet<Vector2>();

        ButtonState lastButtonState = new ButtonState();
        private void CollectInput(GameTime gameTime)
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
                    if (pointpositions.Count > 0)
                    {
                        controler.Click(pointpositions);
                        pointpositions.Clear();
                    }

                }
                else if (gesture.GestureType == GestureType.FreeDrag)
                {
                    Vector2 off = gesture.Position-lastpos;
                    if(/*gameTime.TotalGameTime.Milliseconds - lastinputtime.TotalGameTime.Milliseconds < 20 || */(off.Length() < 1))
                    {
                        continue ;
                    }
                    lastinputtime.TotalGameTime = gameTime.TotalGameTime;
                    lastpos = gesture.Position;

                    pointpositions.Add(gesture.Position);
                    if (pointpositions.Count > 0)
                    {
                        controler.Press(pointpositions);
                        pointpositions.Clear();
                    }
                }
                else if (gesture.GestureType == GestureType.DragComplete)
                {
                    pointpositions.Add(lastpos);
                    if (pointpositions.Count > 0)
                    {
                        controler.Click(pointpositions);
                        pointpositions.Clear();
                    }
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

            //Get Mouse State then Capture the Button type and Respond Button Press

            currentMouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                IsMouseVisible = true;

                pointpositions.Add(mousePosition);
                controler.Press(pointpositions);
                pointpositions.Clear();
            }
            else if (currentMouseState.LeftButton == ButtonState.Released && lastButtonState == ButtonState.Pressed)
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
