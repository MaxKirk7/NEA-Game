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
    private bool ScreenOver = false;
    private BlankBox LeaderBoardBackGround;
    private List<string> SavedFile = HomeScreen.SaveFile();
    //Background
    //HighScores top 3 + own personal high score + top 3 weakly high
    //BackSymbol
    private Button Back;
    private List<List<string>> Scores = new();
    private List<TextBox> HighScores = new(6);
    public void LoadContent(ContentManager con, SpriteBatch sp)
    {
        HighScores.Clear();
        Scores.Clear();
        Scores = Sql.HighScores(SavedFile[0].Replace("PlayerID,", ""));
        LeaderBoardBackGround = new(new Color(10, 20, 5, 180), con, sp, 500, 500, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
        Back = new("Fonts/TitleFont", "Back", con, sp, 100, 980, Color.Black, 1.3, 100, 75, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
        var Ypos = 320;
        var TextGap = 20;
        for (int i = 0; i < Scores.Count; i++)
        {
            if (i == 0)
            {
                HighScores.Add(new TextBox("Fonts/TitleFont", "High Scores :", con, sp, Game1.ScreenWidth / 2 + 2, Ypos + i * TextGap, Color.LightCyan, 2));
                Ypos += 60;
            }
            else if (i == 5)
            {
                Ypos += 60;
                HighScores.Add(new TextBox("Fonts/TitleFont", "Personal High:", con, sp, Game1.ScreenWidth / 2 + 2, Ypos + i * TextGap, Color.LightCyan, 2));
            }
            else if (i == 1)
            {
                Ypos += 60;
                HighScores.Add(new TextBox("Fonts/TitleFont", "Weekly High Scores :", con, sp, Game1.ScreenWidth / 2 + 2, Ypos + i * TextGap, Color.LightCyan, 2));
            }
            if (Scores[i].Count > 1)
            {
                HighScores.Add(new TextBox("Fonts/TitleFont", i + 1 + ": " + Scores[i][0] + "......" + Scores[i][1], con, sp, Game1.ScreenWidth / 2 + 2, Ypos + i * TextGap, Color.LightCyan, 1.5));
            }
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
        }
        return Over;
    }

}