using System.Collections.Generic;
using System.IO;
using _Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NEAGameObjects;
using SQLQuery;

namespace NEAScreen;
class MainGame : IScreen
{
    private Texture2D BulletTexture;
    private Texture2D AsteroidTexture;
    private bool IsDead = false;
    private int CurrentScore = -1;
    private static List<string> SavedFile = new();
    private TextBox Score;
    public void LoadContent(ContentManager con, SpriteBatch sp)
    {
        using (FileStream stream = new("SavedInfo.txt", FileMode.Open, FileAccess.Read))
        {
            using (StreamReader reader = new(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    SavedFile.Add(line);
                }
            }
        }
        CurrentScore = Sql.GetScore(SavedFile[0].Replace("PlayerID,", ""));
        Score = new("Fonts/TitleFont", CurrentScore.ToString(), con, sp, 10, 30, Color.Red, 3f);
        BulletTexture = con.Load<Texture2D>("Player/Bullet/Bullet");
        Bullets.LoadTexture(BulletTexture);
        AsteroidTexture = con.Load<Texture2D>("Player/Asteroid/Asteroid");
        Asteroids.LoadTexture(AsteroidTexture);
    }

    public void Update(float delta)
    {
        Asteroids.Update(delta, CurrentScore);
        if (!IsDead)
        {
            Player.Update(delta);
            Bullets.Update(delta);
            var BulletHitBoxes = Bullets.GetBullets();
            Bullets bulletDestroyed = null;
            foreach (var bullet in BulletHitBoxes)
            {
                if (bullet.LifeElapsed > 1)
                {
                    if (Player.IsHit(bullet.rect))
                    {
                        IsDead = true;
                        break;
                    }
                }
                if (Asteroids.IsHit(bullet.rect))
                {
                    bulletDestroyed = bullet;
                    break;
                }
            }
            if (bulletDestroyed != null){
                bulletDestroyed.Remove();
            }
            var AstHitBoxes = Asteroids.GetAsteroids();
            foreach (var ast in AstHitBoxes)
            {
                if (Player.IsHit(ast.rect))
                {
                    IsDead = true;
                    break;
                }
            }
            var Destroyed = Asteroids.AsteroidsDestroyed();
            foreach (var ast in Destroyed)
            {
                CurrentScore += ast.Score;
            }
        }
        Score.ChangeText(CurrentScore.ToString());
    }
    public void Draw(SpriteBatch sp)
    {
        sp.Begin();
        Score.Draw();
        Asteroids.Draw(sp);
        if (!IsDead)
        {
            Player.Draw(sp);
        }
        Bullets.Draw(sp);
        sp.End();
    }

    public bool EndScreen()
    {
        return false;
    }
    public static List<string> saveFile()
    {
        return SavedFile;
    }

}