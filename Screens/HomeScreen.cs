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
    private bool ScreenOver;
    //Skins need to be loaded from database
    private readonly Sql Query = new();
    private readonly List<Skin> AvailableSkins = new();
    private readonly List<string> SavedFile = new();
    private Thing Background;
    private Sprite CurrentSkin;
    private BlankBox SkinBackGround;
    private TextBox test;
    private Button LeftArrow;
    private Button RightArrow;
    private int ActiveSkinIndex;
    private Button PlayGame;
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

        //Set backgrounds to use
        Background = new Thing("LoadingScreen/Sprites/BackGroundSpace", con, sp, Game1.ScreenWidth, Game1.ScreenHeight, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
        //Create Buttons
        test = new TextBox("Fonts/TitleFont", "Test", con, sp, 200, 400, Color.Red, 2);
        PlayGame = new("Fonts/TitleFont","Start Game",con,sp,Game1.ScreenWidth/2,600,Color.Black,2,300,150,new Color(212, 152, 177),Color.DarkGoldenrod);

        //Set Swapping feature
        LeftArrow = new Button("Fonts/TitleFont", "", con, sp, Game1.ScreenWidth / 2 - 220, 200, Color.Red, 2, 300, 300, new Color(212, 152, 177),Color.DarkGoldenrod, "Buttons/Left Arrow");
        RightArrow = new Button("Fonts/TitleFont", "", con, sp, Game1.ScreenWidth / 2 + 220, 200, Color.Red, 2, 300, 300, new Color(212, 152, 177),Color.DarkGoldenrod, "Buttons/Right Arrow");
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
        if (PlayGame.ButtonPressed()){
            ScreenOver = true;
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
        using StreamWriter writer = new("SavedInfo.txt");
            foreach (string s in SavedFile){
                writer.WriteLine(s);
            }
            writer.Flush();
        return ScreenOver;
    }
}