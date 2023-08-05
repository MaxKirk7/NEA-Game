using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NEAScreen;

namespace NEAGame;

public class Game1 : Game
{
    private readonly List<IScreen> AllScreens = new() { new LoadingScreen(), new Login(), new HomeScreen() };//more screens to be added
    private ScreenManager screens;
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public const int ScreenWidth = 1920;
    public const int ScreenHeight = 1080;
    public const int Frames = 60;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.IsBorderless = true;
        Window.AllowUserResizing = false;
        _graphics.PreferredBackBufferHeight = ScreenHeight;
        _graphics.PreferredBackBufferWidth = ScreenWidth;
        IsFixedTimeStep = true;
        TargetElapsedTime = System.TimeSpan.FromSeconds(1.0 / Frames);
    }

    protected override void Initialize()
    {
        Window.Title = "NEA Game";
        screens = new ScreenManager();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        screens.setScreen(AllScreens[1], Content, _spriteBatch);
        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        if (screens.IsScreenOver())
        {
            var IndexCurrentScreen = AllScreens.IndexOf(screens.currentScreen());
            if (IndexCurrentScreen + 1 <= AllScreens.Count) //stop the List from going out of its limit
            {
                screens.setScreen((IScreen)AllScreens[IndexCurrentScreen + 1], Content, _spriteBatch);
            }
            else{
                //End Of game logic to be added
            }
        }


        var delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000F;
        screens.Update(delta);

        // TODO: Add your update logic here

        base.Update(gameTime);
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
}
