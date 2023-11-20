using System;
using System.Collections.Generic;
using System.IO;
using GameLogic;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using NEAScreen;
using Serilog;
using SQLQuery;
namespace NEAGame
{
    public class Game1 : Game
    {
        //by using dictionary screens are only generated when needed/ used
        private readonly Dictionary<Type, Lazy<IScreen>> MainScreens = new Dictionary<Type, Lazy<IScreen>>(){
            {typeof(LoadingScreen),new Lazy<IScreen>(() => new LoadingScreen())},
            {typeof(HomeScreen),new Lazy<IScreen>(() => new HomeScreen())},
            {typeof(LoginScreen),new Lazy<IScreen>(() => new LoginScreen())},
            {typeof(MainGame),new Lazy<IScreen>(() => new MainGame())}
        };
        public static bool LogIn { get; set; }
        private ScreenManager screens;
        private static bool IsMuted = false;
        private readonly GraphicsDeviceManager _graphics;
        private static SpriteBatch _spriteBatch;
        private static ContentManager Con;
        public const int ScreenWidth = 1920;
        public const int ScreenHeight = 1080;
        public const int Frames = 60;
        private List<string> file = new List<string>();
        public static Song GameMusic;

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
            file = Logic.PullFile();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // Load the initial screen lazily
            var initialScreenType = typeof(LoadingScreen); // initial screen
            screens.setScreen(MainScreens[initialScreenType].Value, Con, _spriteBatch);
            //Sound For Game Qued
            GameMusic = Con.Load<Song>("Key Media/GameMusic");

        }

        protected override void Update(GameTime gameTime)
        {
            file = Logic.PullFile();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            var delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000F;
            screens.Update(delta);
            MediaPlayer.IsMuted = IsMuted;
            if (screens.IsScreenOver() || LogIn)
            {
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
        private Type GetNextScreenType(Type current)
        {
            if (!LogIn)
            {
                if (current == typeof(LoadingScreen))
                {
                    if (file.Count == 0 || file[0].Split(",")[1].IsNullOrEmpty() || !Sql.IsVerified)
                    { // if new game
                        var newfile = "PlayerID,\nSkin,\nGamesPlayed,0\nMusic,true\nSoundEFX,true";
                        Logic.PushFile(newfile);
                    }
                    MediaPlayer.Volume = 0.6F;
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Play(GameMusic);
                    file = Logic.PullFile();
                    if (!bool.Parse(file[3].Split(",")[1]))
                    {
                        MediaPlayer.IsMuted = true;
                    }
                    if (!file[0].Split(",")[1].IsNullOrEmpty())
                    {
                        return typeof(HomeScreen);
                    }
                    return typeof(LoginScreen);
                }
                else if (current == typeof(LoginScreen))
                {
                    return typeof(HomeScreen);
                }
                else if (current == typeof(HomeScreen))
                {
                    return typeof(MainGame);
                }
                else if (current == typeof(MainGame))
                {
                    if (MainGame.EndGame())
                    {
                        Exit();
                    }
                    MainGame.NewGame = true;

                    return typeof(HomeScreen);
                }
                else
                {
                    return typeof(LoginScreen); // if any errors
                }
            }
            else
            {
                LogIn = false;
                return typeof(LoginScreen); // if opting to re do the log in screen
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            screens.Draw(_spriteBatch);
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        public static SpriteBatch GetSpriteBatch()
        {
            return _spriteBatch;
        }

        public static ContentManager GetContentManager()
        {
            return Con;
        }
        public static void Mute()
        {
            IsMuted = !IsMuted;
        }
    }
}
