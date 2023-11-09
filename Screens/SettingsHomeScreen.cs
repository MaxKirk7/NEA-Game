using _Sprites;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NEAGame;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace NEAScreen;
class SetttingsHomeScreen : IScreen
{
    private MouseState OldMouse;
    private static bool NewScreen = false;
    private bool ScreenOver = false;
    private static List<string> SaveFile = new(5);
    private BlankBox Backdrop;
    private Button Music;
    private Button SoundEFX;
    private TextBox MusicTxt;
    private TextBox SoundEFXTxt;
    private Button SignOut;
    private Button Back;
    public void LoadContent(ContentManager con, SpriteBatch sp)
    {
        if (!NewScreen)
        {
            Backdrop = new BlankBox(new Color(100, 55, 70, 85), con, sp, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2 + 200, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
            Music = new Button("Fonts/TitleFont", "", con, sp, Game1.ScreenWidth / 2 - 100, Game1.ScreenHeight / 2 - 150, Color.Pink, 9999, 200, 200, Color.GhostWhite, Color.Coral);
            SoundEFX = new Button("Fonts/TitleFont", "", con, sp, Game1.ScreenWidth / 2 - 100, Game1.ScreenHeight / 2 + 150, Color.Pink, 9999, 200, 200, Color.GhostWhite, Color.Coral);
            MusicTxt = new TextBox("Fonts/TitleFont", "Music", con, sp, 600, Game1.ScreenHeight / 2 - 150, Color.White, 3);
            SoundEFXTxt = new TextBox("Fonts/TitleFont", "Sound EFX", con, sp, 600, Game1.ScreenHeight / 2 + 150, Color.White, 3);
            SignOut = new Button("Fonts/TitleFont", "Log Out", con, sp, 100, 200, Color.Black, 1.3, 100, 75, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
            Back = new("Fonts/TitleFont", "Back", con, sp, 100, 980, Color.Black, 1.3, 100, 75, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
        }
        SaveFile.Clear();
        using (FileStream stream = new FileStream("SavedInfo.txt", FileMode.Open, FileAccess.ReadWrite))
        {
            using (StreamReader reader = new(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    SaveFile.Add(line);
                }
            }
        }
        if (SaveFile.Count < 5)
        {
            while (SaveFile.Count != 5)
            {
                if (SaveFile.Count < 3)
                {
                    SaveFile.Add("Temp");
                }
                else if (SaveFile.Count < 4)
                {
                    SaveFile.Add("Music,false");
                }
                SaveFile.Add("SoundEfX,false");
            }
        }
        if (NewScreen)
        {
            Back.AddButton();
            SoundEFX.AddButton();
            Music.AddButton();
            SignOut.AddButton();
        }
        NewScreen = false;
    }

    public void Update(float delta)
    {
        Button.Update();
        if (Music.ButtonPressed())
        {
            if (OldMouse != Mouse.GetState() || OldMouse.LeftButton == ButtonState.Released)
            {
                if (SaveFile[3] == "Music,true")
                {
                    SaveFile[3] = "Music,false";
                }
                else { SaveFile[3] = "Music,true"; }
                Game1.Mute();
            }
        }
        else if (SoundEFX.ButtonPressed())
        {
            if (OldMouse != Mouse.GetState() || OldMouse.LeftButton == ButtonState.Released)
            {
                if (SaveFile[4] == "SoundEFX,false")
                {
                    SaveFile[4] = "SoundEFX,true";
                }
                else
                {
                    SaveFile[4] = "SoundEFX,false";
                }
            }
        }
        if (Back.ButtonPressed())
        {
            ScreenOver = true;
        }
        else if (SignOut.ButtonPressed())
        {
            ScreenOver = true;
            Game1.LogIn = true;
            SaveFile[0] = "PlayerID,";
            SaveFile[1] = "Skin,";
            SaveFile[2] = "GamesPlayed,0";
        }
        else
        {
            OldMouse = Mouse.GetState();
        }
    }
    public void Draw(SpriteBatch sp)
    {
        sp.Begin();
        Backdrop.Draw();
        MusicTxt.Draw();
        SoundEFXTxt.Draw();
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
            using FileStream stream = new("SavedInfo.txt", FileMode.Truncate, FileAccess.Write);
            using StreamWriter writer = new(stream);
            foreach (string s in SaveFile)
            {
                if (s != "")
                {
                    writer.WriteLine(s);
                }
            }
            writer.Flush();
            NewScreen = true;
        }
        return Over;
    }
    public static List<string> ReturnFile()
    {
        return SaveFile;
    }

}