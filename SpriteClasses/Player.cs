using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NEAGame;
using NEAScreen;

namespace NEAGameObjects;
class Player
{
    private static float TeleportDelay = 0;
    private static float TeleportElapsedTime = 10;
    private static Rectangle rect;
    private static Texture2D texture = Game1.GetContentManager().Load<Texture2D>(MainGame.saveFile()[1].Replace("Skin,", ""));
    private static Vector2 Size = new Vector2(100, 125);
    private static Vector2 Position = new Vector2(Game1.ScreenWidth / 2 - Size.X / 2, Game1.ScreenHeight / 2 - Size.X / 2); //centre screen start
    private static double RadianTurnSpeed = 2.5 * (Math.PI / 180); //speed the player will rotate
    private static double UnitCircleValue = 0; // direction the player will move
    public Player()
    {
    }
    private static void Move(float delta)
    {
        var KeyBoard = Keyboard.GetState();

        if (KeyBoard.IsKeyDown(Keys.D))
        {
            UnitCircleValue += RadianTurnSpeed;
        }
        if (KeyBoard.IsKeyDown(Keys.A))
        {
            UnitCircleValue -= RadianTurnSpeed;
        }
        UnitCircleValue = UnitCircleValue % (2 * Math.PI);// 2 pi is maximum angle of circle vefore restarting
        // Update the object's position based on its current direction
        if (KeyBoard.IsKeyDown(Keys.W))
        {
            Position.X += (float)Math.Cos(UnitCircleValue) * delta * 350;
            Position.Y += (float)Math.Sin(UnitCircleValue) * delta * 350;
            // Check if the X value is out of bounds
            if (Position.X < 0)
            {
                Position.X = Game1.ScreenWidth; //Wrap around to the right side of the screen
            }
            else if (Position.X > Game1.ScreenWidth)
            {
                Position.X = 0; // Wrap around to the left side of the screen
            }

            // Check if the Y value is out of bounds
            if (Position.Y < 0)
            {
                Position.Y = Game1.ScreenHeight; // Wrap around to the bottom of the screen
            }
            else if (Position.Y > Game1.ScreenHeight)
            {
                Position.Y = 0; // Wrap around to the top of the screen
            }
        }
        if (KeyBoard.IsKeyDown(Keys.S))
        {
            if (TeleportElapsedTime > TeleportDelay)
            {
                Position.Y = new Random().Next(Game1.ScreenHeight * 3/ 12, Game1.ScreenHeight * 9 / 12);
                Position.X = new Random().Next(Game1.ScreenWidth * 3 / 12, Game1.ScreenWidth * 9 / 12);
                TeleportElapsedTime = 0;
            }
        }
        TeleportElapsedTime += delta;
    }

    public static void Update(float delta)
    {
        Move(delta);
    }
    public static void Draw(SpriteBatch sp)
    {
        rect = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
        var origin = rect.Center.ToVector2() / 3;
        sp.Draw(texture, rect, null, Color.White, (float)UnitCircleValue, origin, SpriteEffects.None, 0f);
    }
}