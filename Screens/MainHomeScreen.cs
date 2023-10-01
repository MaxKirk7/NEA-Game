using System;
using System.Collections.Generic;
using System.Diagnostics;
using _Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NEAGame;
using SQLQuery;

namespace NEAScreen;
class MainHomeScreen : IScreen
{
    private bool ScreenOver = false;
    //Skins need to be loaded from database
    private bool LeaderBoardSelected = false;
    private bool SettingsSelected = false;
    private readonly Sql Query = new();
    private static readonly List<Skin> AvailableSkins = new();
    private readonly List<string> SavedFile = HomeScreen.saveFile();
    private Sprite CurrentSkin;
    private BlankBox SkinBackGround;
    private TextBox test;
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
        //Create Buttons
        test = new TextBox("Fonts/TitleFont", "Test", con, sp, 200, 400, Color.Red, 2);
        PlayGame = new("Fonts/TitleFont", "Start Game", con, sp, Game1.ScreenWidth / 2, 600, Color.Black, 2, 300, 150, new Color(212, 152, 177), Color.DarkGoldenrod);
        NewGame = new("Fonts/TitleFont", "New Game", con, sp, Game1.ScreenWidth / 2, 750, Color.Black, 2, 300, 150, new Color(212, 152, 177), Color.DarkGoldenrod);
        //Set Swapping feature
        LeftArrow = new Button("Fonts/TitleFont", "", con, sp, Game1.ScreenWidth / 2 - 220, 200, Color.Red, 2, 300, 300, new Color(212, 152, 177), Color.DarkGoldenrod, "Buttons/Left Arrow");
        RightArrow = new Button("Fonts/TitleFont", "", con, sp, Game1.ScreenWidth / 2 + 220, 200, Color.Red, 2, 300, 300, new Color(212, 152, 177), Color.DarkGoldenrod, "Buttons/Right Arrow");
        SkinBackGround = new BlankBox(new Color(9, 20, 9, 100), con, sp, 200, 200, Game1.ScreenWidth / 2, 200);
        //set up skins to lookthrough
        var SkinIds = Sql.GetAvailableSkin(SavedFile[0].Replace("PlayerID,", ""));
        //Create Other UI features
        LeaderBoard = new("Fonts/TitleFont", "", con, sp, 1850, 980, Color.Black, 0, 75, 75, new Color(212, 152, 177), Color.DarkGoldenrod, "Buttons/Rounded Square Button");
        LeaderBoardSymbol = new("Buttons/Trophie", con, sp, 75, 75, 1849, 978);
        Settings = new("Fonts/TitleFont", "", con, sp, 300, 980, Color.Black, 0, 75, 75, new Color(212, 152, 177), Color.DarkGoldenrod, "Buttons/Rounded Square Button");
        SettingsSymbol = new("Buttons/Settings Cog", con, sp, 75, 75, 299, 978);
        //initilaise skin values to the list if they are 'unlocked'
        foreach (int ID in SkinIds)
        {
            AvailableSkins.Add(new Skin(ID));
        }
        //load the last used skin
        if (SavedFile.Count > 1 && !String.IsNullOrWhiteSpace(SavedFile[1]))
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
        else
        {
            CurrentSkin = new Sprite(AvailableSkins[0].BaseSkin, con, sp, 150, 150, Game1.ScreenWidth / 2, 200);
        }
    }

    public void Update(float delta)
    {
        Button.Update();
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
        }
        if (PlayGame.ButtonPressed())
        {
            ScreenOver = true;
        }
        if (NewGame.ButtonPressed())
        {
            //sql statement to reset the average score
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
            test.Draw();
            CurrentSkin.Draw();
            sp.End();
        }
    }

    public bool EndScreen()
    {
        var IsOver = ScreenOver;
        ScreenOver = false;
        if (IsOver)
        {
            Button.EndButtons();
        }
        return IsOver;
    }
    public bool IsLeaderBoardSelected()
    {
        var Selected = LeaderBoardSelected;
        LeaderBoardSelected = false;
        return Selected;
    }
    public bool IsSettingsSelected()
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