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
    private bool ScreenOver = false;
    private Thing Background;
    private readonly List<IScreen> HomeScreens = new() { new MainHomeScreen(), new LeaderBoardHomeScreen() };
    private ScreenManager HomeScreenManager;
    private static readonly List<string> SavedFile = new();
    private static Skin activeSkin;
    public void LoadContent(ContentManager con, SpriteBatch sp)
    {
        HomeScreenManager = new ScreenManager();
        //Set backgrounds to use
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
                    HomeScreenManager.setScreen(HomeScreens[0], Game1.GetContentManager(), Game1.GetSpriteBatch());
                    Log.Information("Setttings");
                }
            }
            else if (IndexCurrentScreen != 0)
            { // if the user selects back button ever go back to the main screen
                Log.Information("Finsihed LeaderBoard");
                ScreenOver = false;
                HomeScreenManager.setScreen(HomeScreens[0], Game1.GetContentManager(), Game1.GetSpriteBatch());
            }
        }
        HomeScreenManager.Update(delta);
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
        if (ScreenOver)
        {
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
                writer.WriteLine(s);
            }
            writer.Flush();
            Button.EndButtons();
        }
        return ScreenOver;
    }
    public static List<string> saveFile()
    {
        return SavedFile;
    }
}