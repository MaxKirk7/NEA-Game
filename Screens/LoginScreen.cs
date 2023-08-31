using System.Collections.Generic;
using System.IO;
using _Sprites;
using NEAGame;
using SQLQuery;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;
using System;

namespace NEAScreen;
class LoginScreen : IScreen
{
    private bool ScreenOver = false;
    private readonly Sql LoginScreenQuery = new();
    private Button SignUp;
    private Button SignIn;
    private Button Enter;
    private Thing BackGround;
    private BlankBox UsernameBox;
    private BlankBox PasswordBox;
    private BlankBox LoginBox;
    private BlankBox HeaderBox;
    private InputBox Username;
    private InputBox Password;
    private TextBox Header;
    private readonly List<string> SavedFile = new();
    public void LoadContent(ContentManager con, SpriteBatch sp)
    {
        using (FileStream stream = new("SavedInfo.txt", FileMode.OpenOrCreate))
        {
            using (StreamReader reader = new(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    SavedFile.Add(line);
                }
            }
            if (SavedFile.Count > 0 && !String.IsNullOrWhiteSpace(SavedFile[0]))
            {
                ScreenOver = true;
            }
        }
        //Common Thigs that arent going to change
        BackGround = new Thing("LoadingScreen/Sprites/BackGroundSpace", con, sp, Game1.ScreenWidth, Game1.ScreenHeight, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
        LoginBox = new BlankBox(new Color(10, 20, 5, 180), con, sp, 1000, 800, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
        //MainHeader Parts
        Header = new TextBox("Fonts/TitleFont", "Salam Armada", con, sp, Game1.ScreenWidth / 2, 200, Color.Black, 1.8);
        HeaderBox = new BlankBox(Color.BlueViolet, con, sp, 300, 50, Game1.ScreenWidth / 2, 200);
        SignIn = new Button("Fonts/TitleFont", "Log In", con, sp, Game1.ScreenWidth / 3, Game1.ScreenHeight / 2, Color.Black, 2, 300, 150, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
        SignUp = new Button("Fonts/TitleFont", "Sign Up", con, sp, Game1.ScreenWidth / 3 * 2, Game1.ScreenHeight / 2, Color.Black, 2, 300, 150, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
        Enter = new Button("Fonts/TitleFont", "Enter", con, sp, Game1.ScreenWidth / 2, (int)((Game1.ScreenHeight / 3) * 2.25), Color.Black, 2, 300, 100, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
        //hide Enter Button as game starts
        Enter.RemoveButton();
        //stuff that is drawn when any button is pressed
        UsernameBox = new BlankBox(Color.Coral, con, sp, 300, 100, Game1.ScreenWidth / 2, 500);
        Username = new InputBox("Fonts/TitleFont", "Enter Your Username...", con, sp, Game1.ScreenWidth / 2, 500, Color.Black, 1, 300, 100, 15);
        PasswordBox = new BlankBox(Color.Coral, con, sp, 300, 100, Game1.ScreenWidth / 2, 650);
        Password = new InputBox("Fonts/TitleFont", "Enter Your Password...", con, sp, Game1.ScreenWidth / 2, 650, Color.Black, 1, 300, 100, 25);
    }

    public void Update(float delta)
    {
        Button.Update();
        if (SignIn.ButtonPressed() || SignUp.ButtonPressed())
        {
            SignIn.RemoveButton();
            SignUp.RemoveButton();
            Enter.AddButton();
            Username.Update();
            Password.Update();
            if (Enter.ButtonPressed())
            {
                if ((Username.GetInput().Length >= 5 && Password.GetInput().Length >= 8) || Username.GetInput() == "TestUser" && Password.GetInput() == "TestPass")
                {
                    //execute query to see if the table has a result for the inputed username and password
                    var result = LoginScreenQuery.PlayerIDQuery(Username.GetInput(), Password.GetInput()).ToString();
                    //sign in logic
                    if (SignIn.ButtonPressed())
                    {
                        //failed login
                        if (result == "null")
                        {
                            Username.ChangeText("Invalid Username Or password");
                            Password.ChangeText("Re-Enter Details");
                        }
                        //succesful login
                        else
                        {
                            if (SavedFile.Count == 0)
                            {
                                SavedFile.Add($"PlayerID,{result}");
                            }
                            else
                            {
                                SavedFile[0] = $"PlayerID,{result}";
                            }
                            ScreenOver = true;
                        }
                    }
                    else
                    {
                        //Sign up logic
                        //if the details are not found in the table (password and username)
                        if (result == "null")
                        {
                            //creates the account if the name doesnt already exist
                            var ExisitngName = LoginScreenQuery.NewPlayerTbl(Username.GetInput(), Password.GetInput());
                            if (!ExisitngName)
                            {
                                //get PlayerID and save it for later use
                                string PlayerID = LoginScreenQuery.PlayerIDQuery(Username.GetInput(), Password.GetInput());
                                SavedFile.Add($"PlayerID,{PlayerID}");
                                //add the first achievment for the player
                                LoginScreenQuery.AddAchievment(PlayerID,"1");
                                //end the screen
                                ScreenOver = true;
                            }
                            else
                            {
                                // If the username already exists, display a message
                                Username.ChangeText("Username already Exists");
                            }
                        }
                        else
                        {
                            // If the username already exists, display a message
                            Username.ChangeText("Username already Exists");
                        }
                    }
                }
                else
                {
                    Username.ChangeText("Username or Password not long enough");
                    Password.ChangeText("Min length 5 and 8 charecters");
                }
            }

        }
    }
    public void Draw(SpriteBatch sp)
    {
        sp.Begin();
        BackGround.Draw();
        LoginBox.Draw();
        HeaderBox.Draw();
        Header.Draw();
        Button.ButtonDraw();
        if (Enter.IsButtonActive())
        {
            //draws these once a button pressed
            UsernameBox.Draw();
            Username.Draw();
            PasswordBox.Draw();
            Password.Draw();
        }
        sp.End();
    }
    public bool EndScreen()
    {
        if (ScreenOver)
        {
            File.WriteAllLines("SavedInfo.txt", SavedFile);
            Button.EndButtons();
        }
        return ScreenOver;
    }

}