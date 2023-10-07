using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NEAGame;

namespace NEAGameObjects
{
    class Bullets
    {
        public Rectangle rect;
        private static readonly double LifeTime = 3.5;
        public double LifeElapsed = 0;
        private readonly double UnitCircleValue;
        private Vector2 Position;
        private Vector2 Size = new(25, 25);
        private static Texture2D Texture;
        private static List<Bullets> FiredBullets = new(5); // Max 5 Bullets

        public Bullets(Vector2 startPosition, double direction)
        {
            UnitCircleValue = direction;
            Position = startPosition;
        }

        private void Move(float delta)
        {
            Position.X += (float)Math.Cos(UnitCircleValue) * delta * 600;
            Position.Y += (float)Math.Sin(UnitCircleValue) * delta * 600;
            if (Position.X < 0)
            {
                Position.X = Game1.ScreenWidth; // Wrap around to the right side of the screen
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

        public static void Update(float delta)
        {
            // Create a list to hold bullets that need to be removed
            List<Bullets> bulletsToRemove = new List<Bullets>();

            foreach (var bullet in FiredBullets)
            {
                bullet.Move(delta);
                if (bullet.LifeElapsed > LifeTime)
                {
                    // Mark bullets for removal
                    bulletsToRemove.Add(bullet);
                }
                else
                {
                    bullet.LifeElapsed += delta;
                }
            }

            // Remove bullets that need to be removed
            foreach (var bulletToRemove in bulletsToRemove)
            {
                FiredBullets.Remove(bulletToRemove);
            }
        }

        private void DrawIndividual(SpriteBatch sp)
        {
            rect = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
            var origin = rect.Size.ToVector2() / 2;
            sp.Draw(Texture, rect, null, Color.White, (float)UnitCircleValue, origin, SpriteEffects.None, 0f);
        }

        public static void Draw(SpriteBatch sp)
        {
            foreach (var bullet in FiredBullets)
            {
                bullet.DrawIndividual(sp);
            }
        }

        public static void Shoot(Vector2 start, double direction)
        {
            // Only create a new bullet if the max limit is not reached
            if (FiredBullets.Count < 5)
            {
                FiredBullets.Add(new Bullets(start, direction));
            }
        }

        public static void LoadTexture(Texture2D texture)
        {
            Texture = texture;
        }

        public static List<Bullets> GetBullets()
        {
            return FiredBullets;
        }
        public void Remove(){
            FiredBullets.Remove(this);
        }
    }
}
