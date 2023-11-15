using System.Collections.Generic;
using System.IO;
using _Sprites;
using GameLogic;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NEAGame;
using NEAGameObjects;
using SQLQuery;

namespace NEAScreen;
class MainGame : IScreen
{
    public static bool NewGame;
    private static bool Quitting;
    private const float ImmunityTime = 2;
    private bool SEFX;
    private float ImmunityElapsedTime = 10;
    private int Lives = 3;
    private bool RoundOver = false;
    private static bool GameOver;
    private Texture2D BulletTexture;
    private Texture2D AsteroidTexture;
    private bool IsDead;
    private int CurrentScore;
    private List<string> SavedFile = new();
    private TextBox Score;
    private Button PlayAgain;
    private Button HomeScreen;
    private Button Quit;
    public void LoadContent(ContentManager con, SpriteBatch sp)
    {
        RoundOver = false;
        IsDead = false;
        CurrentScore = -1;
        SavedFile.Clear();
        SavedFile = Logic.PullFile();
        if (SavedFile[3].Split(",")[1].IsNullOrEmpty()){
            SavedFile[3] = "GamesPlayed,0";
        }
        CurrentScore = Sql.GetAverageScore(SavedFile[0].Replace("PlayerID,", ""));
        if (!NewGame)
        {
            BulletTexture = con.Load<Texture2D>("Player/Bullet/Bullet");
            Bullets.LoadTexture(BulletTexture);
            AsteroidTexture = con.Load<Texture2D>("Player/Asteroid/Asteroid");
            Asteroids.LoadTexture(AsteroidTexture);
            Score = new("Fonts/TitleFont", CurrentScore.ToString(), con, sp, 30, 30, Color.Red, 3f);
            var SFX = con.Load<SoundEffect>("Key Media/Shot");
            Bullets.SetSoundEffect(SFX, SEFX);
            PlayAgain = new("Fonts/TitleFont", "Play Again", con, sp, Game1.ScreenWidth / 3, Game1.ScreenHeight / 2, Color.Black, 2, 300, 150, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
            HomeScreen = new("Fonts/TitleFont", "Home Screen", con, sp, Game1.ScreenWidth * 2 / 3, Game1.ScreenHeight / 2, Color.Black, 2, 300, 150, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
            Quit = new("Fonts/TitleFont", "Quit", con, sp, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2, Color.Black, 2, 300, 150, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
        }
        else
        {
            Score.ChangeText(CurrentScore.ToString());
        }
        Player.GetFile(SavedFile);
        SEFX = bool.Parse(SavedFile[4].Split(",")[1]);
        PlayAgain.RemoveButton();
        HomeScreen.RemoveButton();
        Quit.RemoveButton();
        ImmunityElapsedTime = 10;
        Lives = 3;
        Player.Reset();
        Asteroids.Reset();
        Bullets.Reset();
        NewGame = false;
    }

    public void Update(float delta)
    {
        Game(delta);
        DeathScreen(delta);
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
        Button.ButtonDraw();
        sp.End();
    }

    public bool EndScreen()
    {
        var ScreenOver = false;
        if (GameOver)
        {
            ScreenOver = true;
            NewGame = true;
            GameOver = false;
            Logic.PushFile(SavedFile);
            Button.EndButtons();
            SavedFile.Clear();
        }
        return ScreenOver;
    }
    private void Game(float delta)
    {
        if (!RoundOver)
        {
            Asteroids.Update(delta, CurrentScore);
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
            bulletDestroyed?.Remove();
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
        if (IsDead)
        {
            ImmunityElapsedTime += delta;
            if (ImmunityElapsedTime >= ImmunityTime)
            {
                IsDead = false;
                Lives--;
                ImmunityElapsedTime = 0;
            }
        }
        if (Lives < 0)
        {
            RoundOver = true;
        }
    }
    private void DeathScreen(float delta)
    {
        if (RoundOver)
        {
            PlayAgain.AddButton();
            HomeScreen.AddButton();
            Quit.AddButton();
            Button.Update();
            if (PlayAgain.ButtonPressed() || HomeScreen.ButtonPressed())
            {
                var Games = SavedFile[2].Split(",");
                SavedFile[2] = SavedFile[2].Replace(Games[1], (int.Parse(Games[1]) + 1).ToString());
                Sql.UpdateScore(SavedFile, CurrentScore);
                if (CurrentScore > 1000)
                {
                    var PlayerID = SavedFile[0].Replace("PlayerID,", "");
                    Sql.AddAchievment(PlayerID, "2");
                    if (CurrentScore > 100000)
                    {
                        Sql.AddAchievment(PlayerID, "3");
                    }
                }
                if (PlayAgain.ButtonPressed())
                {
                    Player.Reset();
                    Bullets.Reset();
                    Asteroids.Reset();
                    RoundOver = false;
                    Lives = 3;
                    CurrentScore = Sql.GetAverageScore(SavedFile[0].Replace("PlayerID,", ""));
                }
                if (HomeScreen.ButtonPressed())
                {
                    GameOver = true;
                }
            }
            if (Quit.ButtonPressed())
            {
                Quitting = true;
                GameOver = true;
            }
        }
        else
        {
            PlayAgain.RemoveButton();
            HomeScreen.RemoveButton();
            Quit.RemoveButton();
        }
    }
    public static bool EndGame()
    {
        return Quitting;
    }

}