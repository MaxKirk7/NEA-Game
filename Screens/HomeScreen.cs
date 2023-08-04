using _Sprites;
using NEAGame;
using SQLQuery;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;

namespace NEAScreen;
class HomeScreen : IScreen
{
    //Skins need to be loaded from database
    private Sql Query = new Sql();
    private List<Skin> AvailableSkins = new List<Skin>();
    private List<string> SavedFile = new List<string>();
    private Thing Background;
    private Sprite CurrentSkin;
    private BlankBox SkinBackGround;
    private TextBox test;
    private Button LeftArrow;
    private Button RightArrow;
    private int ActiveSkinIndex;
    public void LoadContent(ContentManager con, SpriteBatch sp)
    {
        //Set backgrounds to use
        if (File.Exists("SavedInfo.txt"))
        {
            foreach (string s in File.ReadLines("SavedInfo.txt"))
            {
                SavedFile.Add(s);
            }
        }
        else
        {
            SavedFile.Add("PlayerID,1");
        }

        //Set backgrounds to use
        Background = new Thing("LoadingScreen/Sprites/BackGroundSpace", con, sp, Game1.ScreenWidth, Game1.ScreenHeight, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
        //Create Buttons
        test = new TextBox("Fonts/TitleFont", "Test", con, sp, 200, 400, Color.Red, 2);

        //Set Swapping feature
        LeftArrow = new Button("Fonts/TitleFont", "", con, sp, Game1.ScreenWidth / 2 - 220, 200, Color.Red, 2, 300, 300, Color.White, Color.Yellow, "Buttons/Left Arrow");
        RightArrow = new Button("Fonts/TitleFont", "", con, sp, Game1.ScreenWidth / 2 + 220, 200, Color.Red, 2, 300, 300, Color.White, Color.Yellow, "Buttons/Right Arrow");
        SkinBackGround = new BlankBox(new Color(9, 20, 9, 100), con, sp, 200, 200, Game1.ScreenWidth / 2, 200);
        //set up skins to lookthrough
        var SkinIds = Query.GetAvailableSkin(SavedFile[0].Replace("PlayerID,", ""));
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
            LeftArrow.UnPress();
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
            RightArrow.UnPress();
        }
    }
    public void Draw(SpriteBatch sp)
    {
        sp.Begin();
        Background.Draw();
        SkinBackGround.Draw();
        Button.ButtonDraw();
        test.Draw();
        CurrentSkin.Draw();
        sp.End();
    }
    public bool EndScreen()
    {
        var LastUsedSkin = "Skin," + AvailableSkins[ActiveSkinIndex].BaseSkin;
        SavedFile[1] = LastUsedSkin;
        using StreamWriter writer = new StreamWriter("SavedInfo.txt"){
            foreach (string s in SavedFile){
                writer.Write(s + "\n");
            }
            writer.Flush();
        }


        return false;
    }
}