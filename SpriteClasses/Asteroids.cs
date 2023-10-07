using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NEAGame;

namespace NEAGameObjects;
class Asteroids
{
    private const int BaseScore = 50;
    private static float RoundElapsedTime = 0;
    private static readonly float RoundWaitTime = 3.5f;
    private static Texture2D Texture;
    private static List<Asteroids> CurrentAsteroids = new();
    private Vector2 Size;
    private Vector2 Position;
    private readonly double UnitCircleValue;
    private int SpeedScale = 100;
    private static List<Asteroids> asteroidsToRemove = new List<Asteroids>();
    public Rectangle rect
    {
        get; private set;
    }
    public int Score
    {
        private set; get;
    }
    public static void LoadTexture(Texture2D texture)
    {
        Texture = texture;
    }
    public Asteroids(int startSize = 1, Vector2? startPosition = null)
    {
        if (startSize < 4)
        {
            var Random = new Random();
            UnitCircleValue = Random.NextDouble() * 2 * Math.PI; // puts the number between 2 pi and 0
            Score = BaseScore * startSize;
            SpeedScale *= startSize;
            if (startPosition == null)
            {
                Position.Y = Random.Next(Game1.ScreenHeight * 2 / 12, Game1.ScreenHeight * 10 / 12);
                if (Random.Next() % 2 == 0)
                {
                    Position.X = Game1.ScreenWidth * 2 / 12;
                }
                else
                {
                    Position.X = Game1.ScreenWidth * 10 / 12;
                }
            }
            else
            {
                Position = (Vector2)startPosition;
            }
            Size.X = (float)(Texture.Width / (3.5 * startSize));
            Size.Y = (float)(Texture.Height / (3.5 * startSize));
        }
        CurrentAsteroids.Add(this);
    }
    public static void Update(float delta, int Score)
    {
        SpawnAsteroids(delta, Score);
        foreach (var asteroid in CurrentAsteroids)
        {
            asteroid.Move(delta);
        }
    }

    private static void SpawnAsteroids(float delta, int score)
    {
        if (CurrentAsteroids.Count == 0) //if "round" over give a little break
        {
            if (RoundElapsedTime > RoundWaitTime)
            {
                if (score < 500)
                {
                    _ = new Asteroids();
                }
                else
                {
                    for (int i = 0; i < score / 800; i++)
                    {
                        _ = new Asteroids();
                    }
                }
                RoundElapsedTime = 0;
            }
            RoundElapsedTime += delta;
        }
    }

    private void Move(float delta)
    {
        Position.X += (float)Math.Cos(UnitCircleValue) * delta * SpeedScale;
        Position.Y += (float)Math.Sin(UnitCircleValue) * delta * SpeedScale;
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
    private void DrawIndividual(SpriteBatch sp)
    {
        rect = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
        sp.Draw(Texture, rect, Color.White);
    }
    public static void Draw(SpriteBatch sp)
    {
        foreach (var asteroid in CurrentAsteroids)
        {
            asteroid.DrawIndividual(sp);
        }
    }
    public static bool IsHit(Rectangle Obj)
    {
        var ObjHitbox = new Rectangle(Obj.X,Obj.Y,Obj.Width + 3,Obj.Height+2);
        foreach (var asteroid in CurrentAsteroids)
        {
            if (asteroid.rect.Intersects(ObjHitbox))
            {
                asteroidsToRemove.Add(asteroid);
            }
        }

        foreach (var asteroidToRemove in asteroidsToRemove)
        {
            CurrentAsteroids.Remove(asteroidToRemove);
            // Check if the start size of the new asteroids is less than 4 to avoid infinite spawning
            if (asteroidToRemove.Score / BaseScore < 3)
            {
                // Create two new smaller asteroids discard the refernace to increase efficiency
                _ = new Asteroids((asteroidToRemove.Score / BaseScore) + 1, asteroidToRemove.Position);
                _ = new Asteroids((asteroidToRemove.Score / BaseScore) + 1, asteroidToRemove.Position);
            }
        }

        return asteroidsToRemove.Count > 0;
    }

    public static List<Asteroids> GetAsteroids()
    {
        return CurrentAsteroids;
    }
    public static List<Asteroids> AsteroidsDestroyed()
    {
        var Copy = new List<Asteroids>(asteroidsToRemove); // Create a copy of the list
        asteroidsToRemove.Clear(); // Clear the original list
        return Copy; // Return the copy containing destroyed asteroids

    }
}