using _Sprites;
using NEAGame;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using GameLogic;
using System;

namespace NEAScreen;
class HomeScreen : IScreen
{
    private bool NewScreen = false;
    private bool ScreenOver = false;
    private Thing Background;
    private readonly Dictionary<Type, Lazy<IScreen>> HomeScreens = new Dictionary<Type, Lazy<IScreen>>(){
            {typeof(MainHomeScreen),new Lazy<IScreen>(() => new MainHomeScreen())},
            {typeof(LeaderBoardHomeScreen),new Lazy<IScreen>(() => new LeaderBoardHomeScreen())},
            {typeof(SetttingsHomeScreen),new Lazy<IScreen>(() => new SetttingsHomeScreen())}
        };
    private ScreenManager HomeScreenManager;
    private static List<string> SavedFile = new();
    private static Skin ActiveSkin;
    public void LoadContent(ContentManager con, SpriteBatch sp)
    {
        HomeScreenManager = new ScreenManager();
        SavedFile.Clear();
        SavedFile = Logic.PullFile();
        var initialScreenType = typeof(MainHomeScreen);
        HomeScreenManager.setScreen(HomeScreens[initialScreenType].Value, con, sp);
        if (!NewScreen)
        {
            //Set backgrounds to use
            Background = new Thing("LoadingScreen/Sprites/BackGroundSpace", con, sp, Game1.ScreenWidth, Game1.ScreenHeight, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
        }
        NewScreen = false;
    }

    public void Update(float delta)
    {
        HomeScreenManager.Update(delta);
        if (HomeScreenManager.IsScreenOver())
        {
            ScreenOver = true;
            var currentScreenType = HomeScreenManager.currentScreen().GetType();
            var nextScreenType = GetNextScreenType(currentScreenType);
            if (nextScreenType != null)
            {
                ScreenOver = false;
                var nextScreen = HomeScreens[nextScreenType].Value;
                HomeScreenManager.setScreen(nextScreen, Game1.GetContentManager(), Game1.GetSpriteBatch());
            }
        }
    }

    private Type GetNextScreenType(Type current)
    {
        if (current == typeof(MainHomeScreen))
        {
            if (MainHomeScreen.IsLeaderBoardSelected())
            {
                return typeof(LeaderBoardHomeScreen);
            }
            else if (MainHomeScreen.IsSettingsSelected())
            {
                return typeof(SetttingsHomeScreen);
            }
            else
            {
                return null;
            }
        }
        else
        {
            //always return to the original screen
            if (current == typeof(SetttingsHomeScreen))
            {
                SavedFile = SetttingsHomeScreen.ReturnFile();
            }
            return typeof(MainHomeScreen);
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
        var Over = ScreenOver;
        if (ScreenOver || Game1.LogIn)
        {
            ScreenOver = false;
            ActiveSkin = MainHomeScreen.GetActiveSkin();
            if (!Game1.LogIn)
            {

                if (SavedFile.Count > 1)
                {
                    SavedFile[1] = $"Skin,{ActiveSkin.BaseSkin}";
                }
                else
                {
                    SavedFile.Add($"Skin,{ActiveSkin.BaseSkin}");
                }
            }
            Logic.PushFile(SavedFile);
            Button.EndButtons();
            SavedFile.Clear();
            NewScreen = true;
        }
        return Over;
    }
    public static List<string> SaveFile()
    {
        return SavedFile;
    }
}