using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using _Sprites;
using Microsoft.Xna.Framework;
using NEAGame;
using System.Collections.Generic;
using SQLQuery;

namespace NEAScreen;
class LeaderBoardHomeScreen : IScreen
{
    private bool First;
    private bool ScreenOver = false;
    private BlankBox LeaderBoardBackGround;
    private List<string> SavedFile = HomeScreen.saveFile();
    //Background
    //HighScores top 3 + own personal high score + top 3 weakly high
    //BackSymbol
    private Button Back;
    private List<List<string>> Scores = new();
    private List<TextBox> HighScores = new(6);
    public void LoadContent(ContentManager con, SpriteBatch sp)
    {
        Scores.Clear();
        Scores = Sql.HighScores(SavedFile[0].Replace("PlayerID,", ""));
        LeaderBoardBackGround = new(new Color(10, 20, 5, 180), con, sp, 500, 500, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
        Back = new("Fonts/TitleFont", "Back", con, sp, 100, 980, Color.Black, 1.3, 100, 75, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
        var Ypos = 320;
        if (!First)
        {
            for (int i = 0; i < Scores.Count; i++)
            {
                if (i == 0)
                {
                    HighScores.Add(new TextBox("Fonts/TitleFont", "High Scores :", con, sp, Game1.ScreenWidth / 2 + 2, Ypos + i * 30, Color.LightCyan, 2));
                    Ypos += 60;
                }
                if (i == 5)
                {
                    Ypos += 60;
                    HighScores.Add(new TextBox("Fonts/TitleFont", "Personal High:", con, sp, Game1.ScreenWidth / 2 + 2, Ypos + i * 30, Color.LightCyan, 2));
                }
                if (Scores[i] != null)
                {
                    HighScores.Add(new TextBox("Fonts/TitleFont", i + 1 + ": " + Scores[i][0] + "......" + Scores[i][1], con, sp, Game1.ScreenWidth / 2 + 2, Ypos + i * 30, Color.LightCyan, 1.5));
                }
                if (i == 1)
                {
                    Ypos += 60;
                    HighScores.Add(new TextBox("Fonts/TitleFont", "Weekly High Scores :", con, sp, Game1.ScreenWidth / 2 + 2, Ypos + i * 30, Color.LightCyan, 2));
                }
            }
        }
        if (First)
        {
            for (int i = 0; i < Scores.Count; i++)
            {
                if (i != 0 || i != 1 || i != 5)
                {
                    HighScores[i].ChangeText(i + 1 + ": " + Scores[i][0] + "......" + Scores[i][1]);
                }
            }
            First = false;
        }
    }

    public void Update(float delta)
    {
        Button.Update();
        if (Back.ButtonPressed())
        {
            ScreenOver = true;
        }
    }
    public void Draw(SpriteBatch sp)
    {
        sp.Begin();
        LeaderBoardBackGround.Draw();
        foreach (var score in HighScores)
        {
            score.Draw();
        }
        Button.ButtonDraw();
        sp.End();
    }

    public bool EndScreen()
    {
        var Over = ScreenOver;
        ScreenOver = false;
        if (Over)
        {
            Button.EndButtons();
            Scores.Clear();
            HighScores.Clear();
            First = true;
        }
        return Over;
    }

}