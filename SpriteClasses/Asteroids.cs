using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NEAGame;
namespace NEAGameObjects;
partial class Asteroids
{
    private static List<Asteroids> asteroids = new();
    private double SpeedScale = 1 / 3;
    private double UnitCircleValue = 0;
    private int XPos, YPos;
    private Rectangle rect;
    public Asteroids()
    {
        asteroids ??= new List<Asteroids>();
        asteroids.Add(this);
        Random random = new Random();
        UnitCircleValue = random.NextDouble() * (Math.PI - -Math.PI) + Math.PI; // this will provide a radnom value between pos or neg Pi
        YPos = random.Next(0, Game1.ScreenHeight);
        //randomly sets the asteroids sarting position 
        if (random.Next() % 2 == 0)
        {
            XPos = Game1.ScreenWidth;
        }
        else
        {
            XPos = 0;
        }
    }
    public static void Update(float delta, int score)
    {
        foreach (var ast in asteroids)
        {
            ast.Move(delta);
        }
        if (SpawnAsteroids())
        {
            CreateAsteroids(score);
        }
    }
}