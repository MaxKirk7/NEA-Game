using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NEAScreen;
using Serilog;

namespace NEAGame
{
    public class Game1 : Game
    {
        //by using dictionary screens are only generated when needed/ used
        private readonly Dictionary<Type , Lazy<IScreen>> MainScreens = new Dictionary<Type, Lazy<IScreen>>(){//https://learn.microsoft.com/en-us/dotnet/api/system.lazy-1?view=net-7.0
            {typeof(LoadingScreen),new Lazy<IScreen>(() => new LoadingScreen())},
            {typeof(HomeScreen),new Lazy<IScreen>(() => new HomeScreen())},
            {typeof(LoginScreen),new Lazy<IScreen>(() => new LoginScreen())},
            {typeof(MainGame),new Lazy<IScreen>(() => new MainGame())}
        };
        private ScreenManager screens;
        private readonly GraphicsDeviceManager _graphics;
        private static SpriteBatch _spriteBatch;
        private static ContentManager Con;
        public const int ScreenWidth = 1920;
        public const int ScreenHeight = 1080;
        public const int Frames = 60;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.IsBorderless = false;
            Window.AllowUserResizing = false;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / Frames);
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        protected override void Initialize()
        {
            Window.Title = "NEA Game";
            screens = new ScreenManager();
            Con = Content;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // Load the initial screen lazily
            var initialScreenType = typeof(LoginScreen); // initial screen
            screens.setScreen(MainScreens[initialScreenType].Value, Con, _spriteBatch);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000F;
            screens.Update(delta);

            if (screens.IsScreenOver())
            {
                Log.Information($"{screens.currentScreen()}: Screen Over");
                var currentScreenType = screens.currentScreen().GetType();

                // Determine the next screen type based on your game logic
                var nextScreenType = GetNextScreenType(currentScreenType); //logic for next screen

                if (MainScreens.TryGetValue(nextScreenType, out var nextScreen))
                {
                    screens.setScreen(nextScreen.Value, Con, _spriteBatch);
                }
                else
                {
                    Exit();
                }
            }

            // TODO: Add your update logic here
            Content = Con;
            base.Update(gameTime);
        }

        // logic to switch screens
        private Type GetNextScreenType(Type current){
            if (current == typeof(LoadingScreen)){
                return typeof(LoginScreen);
            }
            else if (current == typeof(LoginScreen)){
                return typeof(HomeScreen);
            }
            else if (current == typeof(HomeScreen)){
                return typeof(MainGame);
            }
            else{
                return typeof(LoginScreen);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            screens.Draw(_spriteBatch);
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public static SpriteBatch GetSpriteBatch()
        {
            return _spriteBatch;
        }

        public static ContentManager GetContentManager()
        {
            return Con;
        }
    }
}
