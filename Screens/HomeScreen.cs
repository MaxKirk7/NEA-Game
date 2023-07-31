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
    private TextBox test;
    public void LoadContent(ContentManager con, SpriteBatch sp)
    {
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
            var IndexSkin = 0;
            foreach (var skin in AvailableSkins)
            {
                if (skin.BaseSkin == Skin)
                { //the variableSkins base skin is the same as the searchd for skin
                    IndexSkin = AvailableSkins.IndexOf(skin);
                    break;
                }
            }
            CurrentSkin = new Sprite(AvailableSkins[IndexSkin].BaseSkin, con, sp, 150, 150, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
        }
        else
        {
            CurrentSkin = new Sprite(AvailableSkins[0].BaseSkin, con, sp, 150, 150, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
        }

    }

    public void Update(float delta)
    {
    }
    public void Draw(SpriteBatch sp)
    {
        sp.Begin();
        Background.Draw();
        test.Draw();
        try
        {
            CurrentSkin.Draw();
        }
        catch (Exception ex)
        {
            test.ChangeText(ex.ToString());
        }
        sp.End();
    }
    public bool EndScreen()
    {
        return false;
    }
}