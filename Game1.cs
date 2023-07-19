using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NEAGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public const int ScreenWidth = 1920;
    public const int ScreenHeight = 1080;

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
        TargetElapsedTime = System.TimeSpan.FromSeconds(1.0 / 60);
    }

    protected override void Initialize()
    {
        Window.Title = "NEA Game";
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        var delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000F;

        // TODO: Add your update logic here

        base.Update(gameTime);
    }
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}
