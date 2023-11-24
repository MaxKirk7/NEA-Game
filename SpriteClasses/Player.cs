using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NEAGame;
using NEAScreen;
using Serilog;

namespace NEAGameObjects;
class Player
{
    private static readonly float ShotDelay = 0.4f;
    private static float ShotElapsed;
    private static readonly float TeleportDelay = 10;
    private static float TeleportElapsedTime = 10;
    private static Rectangle rect;
    private static Texture2D texture;
    private static Vector2 Size = new Vector2(75,75);
    private static Vector2 Position = new(Game1.ScreenWidth / 2 - Size.X / 2, Game1.ScreenHeight / 2 - Size.X / 2); //centre screen start
    private static readonly double RadianTurnSpeed = 2.5 * (Math.PI / 180); //speed the player will rotate
    private static double UnitCircleValue = 0; // direction the player will move/ face
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
                Position.Y = new Random().Next(Game1.ScreenHeight * 3 / 12, Game1.ScreenHeight * 9 / 12);
                Position.X = new Random().Next(Game1.ScreenWidth * 3 / 12, Game1.ScreenWidth * 9 / 12);
                TeleportElapsedTime = 0;
            }
        }
        if (KeyBoard.IsKeyDown(Keys.Space))
        {
            if (ShotDelay < ShotElapsed)
            {
                Shoot();
                Log.Information("Shoot");
                ShotElapsed = 0;
            }
        }
        TeleportElapsedTime += delta;
        ShotElapsed += delta;
    }

    public static void Update(float delta)
    {
        Move(delta);
    }
    public static void Draw(SpriteBatch sp)
    {

        // Create the rect based on the rotated position
        rect = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

        // Define the source rect (if needed)
        var rect2 = new Rectangle(0, 0, texture.Width, texture.Height);

        // Calculate the origin for rotation
        var origin = new Vector2(rect2.Width / 2, rect2.Height / 2);

        // Draw the texture with rotation
        sp.Draw(texture, rect, rect2, Color.White, (float)UnitCircleValue, origin, SpriteEffects.None, 0f);
    }
    public static bool IsHit(Rectangle obj)
    {
        Rectangle Hitbox = new Rectangle(rect.X, rect.Y, rect.Width - 4, rect.Height - 4);
        Rectangle ObjHitbox = new(obj.X, obj.Y, obj.Width - 2, obj.Height - 2);
        //ObjHitbox.Offset(2, 2);
        return Hitbox.Intersects(ObjHitbox);
    }
    public static void Shoot()
    {
        Vector2 bulletStartPosition = Position;
        Bullets.Shoot(bulletStartPosition, UnitCircleValue);
    }
    public static void Reset()
    {
        Position = new(Game1.ScreenWidth / 2 - Size.X / 2, Game1.ScreenHeight / 2 - Size.X / 2);
        UnitCircleValue = 0;
        var newSkin= MainHomeScreen.GetActiveSkin();
        texture = Game1.GetContentManager().Load<Texture2D>(newSkin.BaseSkin);
    }
    public static void GetFile(List<string> file){
        texture = Game1.GetContentManager().Load<Texture2D>(file[1].Replace("Skin,", ""));
    }
}