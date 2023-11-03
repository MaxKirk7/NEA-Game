using _Sprites;
using NEAGame;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Serilog;

namespace NEAScreen;
class HomeScreen : IScreen
{
    private bool NewScreen = false;
    private bool ScreenOver = false;
    private Thing Background;
    private readonly List<IScreen> HomeScreens = new() { new MainHomeScreen(), new LeaderBoardHomeScreen(), new SetttingsHomeScreen() };
    private ScreenManager HomeScreenManager;
    private static List<string> SavedFile = new();
    private static Skin activeSkin;
    public void LoadContent(ContentManager con, SpriteBatch sp)
    {
        HomeScreenManager = new ScreenManager();
        SavedFile.Clear();
        //Set b
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
        HomeScreenManager.setScreen(HomeScreens[0], con, sp);


        //Set backgrounds to use
        Background = new Thing("LoadingScreen/Sprites/BackGroundSpace", con, sp, Game1.ScreenWidth, Game1.ScreenHeight, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
    }

    public void Update(float delta)
    {
        if (NewScreen)
        {
            LoadContent(Game1.GetContentManager(), Game1.GetSpriteBatch());
            NewScreen = false;
        }
        HomeScreenManager.Update(delta);
        var IndexCurrentScreen = HomeScreens.IndexOf(HomeScreenManager.currentScreen());
        if (HomeScreenManager.IsScreenOver())
        {
            Log.Information("SubScreen finished");
            ScreenOver = true;
            if (IndexCurrentScreen == 0)
            {
                var InstanceHomeScreen = (MainHomeScreen)HomeScreens[0];
                if (InstanceHomeScreen.IsLeaderBoardSelected())
                {
                    ScreenOver = false;
                    HomeScreenManager.setScreen(HomeScreens[1], Game1.GetContentManager(), Game1.GetSpriteBatch());
                    Log.Information("LeaderboardScreen");
                    //End Of game logic to be added
                }
                else if (InstanceHomeScreen.IsSettingsSelected())
                {
                    ScreenOver = false;
                    HomeScreenManager.setScreen(HomeScreens[2], Game1.GetContentManager(), Game1.GetSpriteBatch());
                    Log.Information("Setttings");
                }
            }
            else if (IndexCurrentScreen != 0)
            { // if the user selects back button ever go back to the main screen
                if (IndexCurrentScreen == 1)
                {
                    SavedFile = SetttingsHomeScreen.ReturnFile();
                }
                Log.Information("Finsihed LeaderBoard");
                ScreenOver = false;
                HomeScreenManager.setScreen(HomeScreens[0], Game1.GetContentManager(), Game1.GetSpriteBatch());
            }
        }
    }
    public void Draw(SpriteBatch sp)
    {
        sp.Begin();
        Background.Draw();
        sp.End();
        HomeScreenManager.Draw(sp);
    }
    public bool EndScreen()
    {
        var Over = false;
        if (ScreenOver)
        {
            ScreenOver = false;
            Over = true;
            activeSkin = MainHomeScreen.GetActiveSkin();
            if (SavedFile.Count > 1)
            {
                SavedFile[1] = $"Skin,{activeSkin.BaseSkin}";
            }
            else
            {
                SavedFile.Add($"Skin,{activeSkin.BaseSkin}");
            }
            using FileStream stream = new("SavedInfo.txt", FileMode.Open, FileAccess.Write);
            using StreamWriter writer = new(stream);
            foreach (string s in SavedFile)
            {
                if (s != "")
                {
                    writer.WriteLine(s);
                }
            }
            Button.EndButtons();
            SavedFile.Clear();
            NewScreen = true;
        }
        return Over;
    }
    public static List<string> saveFile()
    {
        return SavedFile;
    }
}