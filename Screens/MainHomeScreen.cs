using System;
using System.Collections.Generic;
using _Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NEAGame;
using SQLQuery;

namespace NEAScreen;
class MainHomeScreen : IScreen
{
    private bool NewScreen = false;
    private double ButtonElapsedTime = 0;
    private double ButtonPressDelay = 0.2;
    private bool ScreenOver = false;
    //Skins need to be loaded from database
    private static bool LeaderBoardSelected = false;
    private static bool SettingsSelected = false;
    private static readonly List<Skin> AvailableSkins = new();
    private List<string> SavedFile;
    private Sprite CurrentSkin;
    private BlankBox SkinBackGround;
    private Button LeftArrow;
    private Button RightArrow;
    private static int ActiveSkinIndex;
    private Button PlayGame;
    private Button NewGame;
    private Button LeaderBoard;
    private Thing LeaderBoardSymbol;
    private Button Settings;
    private Thing SettingsSymbol;
    public void LoadContent(ContentManager con, SpriteBatch sp)
    {
        if (!Game1.LogIn)
        {

            AvailableSkins.Clear();
            SavedFile = HomeScreen.SaveFile();
            if (!NewScreen)
            {
                //Create Buttons
                NewGame = new("Fonts/TitleFont", "New Game", con, sp, Game1.ScreenWidth / 2, 750, Color.Black, 2, 300, 150, new Color(212, 152, 177), Color.DarkGoldenrod);
                PlayGame = new("Fonts/TitleFont", "Start Game", con, sp, Game1.ScreenWidth / 2, 600, Color.Black, 2, 300, 150, new Color(212, 152, 177), Color.DarkGoldenrod);
                RightArrow = new Button("Fonts/TitleFont", "", con, sp, Game1.ScreenWidth / 2 + 220, 200, Color.Red, 2, 300, 300, new Color(212, 152, 177), Color.DarkGoldenrod, "Buttons/Right Arrow");
                LeftArrow = new Button("Fonts/TitleFont", "", con, sp, Game1.ScreenWidth / 2 - 220, 200, Color.Red, 2, 300, 300, new Color(212, 152, 177), Color.DarkGoldenrod, "Buttons/Left Arrow");
                LeaderBoard = new("Fonts/TitleFont", "", con, sp, 1850, 980, Color.Black, 0, 75, 75, new Color(212, 152, 177), Color.DarkGoldenrod, "Buttons/Rounded Square Button");
                Settings = new("Fonts/TitleFont", "", con, sp, 300, 980, Color.Black, 0, 75, 75, new Color(212, 152, 177), Color.DarkGoldenrod, "Buttons/Rounded Square Button");
                //UI
                SkinBackGround = new BlankBox(new Color(9, 20, 9, 100), con, sp, 200, 200, Game1.ScreenWidth / 2, 200);
                //Create Other UI features
                LeaderBoardSymbol = new("Buttons/Trophie", con, sp, 75, 75, 1849, 978);
                SettingsSymbol = new("Buttons/Settings Cog", con, sp, 75, 75, 299, 978);
            }
            else
            {
                NewGame.AddButton();
                PlayGame.AddButton();
                RightArrow.AddButton();
                LeftArrow.AddButton();
                LeaderBoard.AddButton();
                Settings.AddButton();
            }
            //set up skins to lookthrough
            var SkinIds = Sql.GetAvailableSkin(SavedFile[0].Replace("PlayerID,", ""));
            //initilaise skin values to the list if they are 'unlocked'
            foreach (int ID in SkinIds)
            {
                AvailableSkins.Add(new Skin(ID));
            }
            //load the last used skin
            if (SavedFile.Count > 1 && !String.IsNullOrWhiteSpace(SavedFile[1].Split(",")[1]))
            {
                if (!NewScreen)
                {
                    var Skin = SavedFile[1].Replace("Skin,", "");//remove the pretext before the skin location
                    ActiveSkinIndex = 0;
                    foreach (var skin in AvailableSkins)
                    {
                        if (skin.BaseSkin == Skin)
                        { //the variableSkins base skin is the same as the searchd for skin
                            ActiveSkinIndex = AvailableSkins.IndexOf(skin);
                            break;
                        }
                    }
                    CurrentSkin = new Sprite(AvailableSkins[ActiveSkinIndex].BaseSkin, con, sp, 150, 150, Game1.ScreenWidth / 2, 200);
                }
            }
            else
            {
                CurrentSkin = new Sprite(AvailableSkins[0].BaseSkin, con, sp, 150, 150, Game1.ScreenWidth / 2, 200);
            }
            NewScreen = false;
        }
    }

    public void Update(float delta)
    {
        Button.Update();
        if (ButtonPressDelay < ButtonElapsedTime)
        {

            if (LeftArrow.ButtonPressed())
            {
                //logic for going left by one in the List AvailableSkins
                //if the current skin is not in position 0
                if (ActiveSkinIndex > 0)
                {
                    ActiveSkinIndex--;
                }
                else
                {
                    ActiveSkinIndex = AvailableSkins.Count - 1;
                }
                CurrentSkin.ChangeTexture(AvailableSkins[ActiveSkinIndex].BaseSkin);
                ButtonElapsedTime = 0;
            }
            if (RightArrow.ButtonPressed())
            {
                //logic for going right by one in the List AvailableSkins
                //if the current skin is not in largest position
                if (ActiveSkinIndex < AvailableSkins.Count - 1)
                {
                    ActiveSkinIndex++;
                }
                else
                {
                    ActiveSkinIndex = 0;
                }
                CurrentSkin.ChangeTexture(AvailableSkins[ActiveSkinIndex].BaseSkin);
                ButtonElapsedTime = 0;
            }
        }
        if (PlayGame.ButtonPressed())
        {
            ScreenOver = true;
        }
        if (NewGame.ButtonPressed())
        {
            SavedFile = Sql.ResetAverageScore(SavedFile);
            SavedFile[2] = "GamesPlayed,0";
            ScreenOver = true;
        }
        if (LeaderBoard.ButtonPressed())
        {
            ScreenOver = true;
            LeaderBoardSelected = true;
        }
        if (Settings.ButtonPressed())
        {
            ScreenOver = true;
            SettingsSelected = true;
        }
        ButtonElapsedTime += delta;
    }
    public void Draw(SpriteBatch sp)
    {
        if (!ScreenOver)
        {
            sp.Begin();
            SkinBackGround.Draw();
            Button.ButtonDraw();
            LeaderBoardSymbol.Draw();
            SettingsSymbol.Draw();
            CurrentSkin.Draw();
            sp.End();
        }
    }

    public bool EndScreen()
    {
        var IsOver = ScreenOver;
        if (IsOver)
        {
            ScreenOver = false;
            Button.EndButtons();
            NewScreen = true;
        }
        return IsOver;
    }
    public static bool IsLeaderBoardSelected()
    {
        var Selected = LeaderBoardSelected;
        LeaderBoardSelected = false;
        return Selected;
    }
    public static bool IsSettingsSelected()
    {
        var Selected = SettingsSelected;
        SettingsSelected = false;
        return Selected;
    }
    public static Skin GetActiveSkin()
    {
        if (AvailableSkins.Count > 0)
        {
            return AvailableSkins[ActiveSkinIndex];
        }
        return null;
    }
}