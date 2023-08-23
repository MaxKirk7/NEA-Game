using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;
using _Sprites;
using NEAGame;
using SQLQuery;
namespace NEAScreen
{
    class Login : IScreen
    {
        
        private bool ScreenOver = false;
        private Sql LoginScreenQuery;
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
            //see if this is the first time this sytem has been used for the game
            if (File.Exists("SavedInfo.txt"))
            {
                foreach (string s in File.ReadAllLines("SavedInfo.txt"))
                {
                    SavedFile.Add(s);
                }
                if (SavedFile.Count > 0 && SavedFile[0].Length != 0)
                {
                    ScreenOver = true;
                }
            }
            else
            {
                File.Create("SavedInfo.txt");
            }
            LoginScreenQuery = new Sql();
            //Common Thigs that arent going to change
            BackGround = new Thing("LoadingScreen/Sprites/BackGroundSpace", con, sp, Game1.ScreenWidth, Game1.ScreenHeight, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
            LoginBox = new BlankBox(new Color(10, 20, 5, 180), con, sp, 1000, 800, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
            //MainHeader Parts
            Header = new TextBox("Fonts/TitleFont", "Salam Armada", con, sp, Game1.ScreenWidth / 2, 200, Color.Black, 1.8);
            HeaderBox = new BlankBox(Color.BlueViolet, con, sp, 300, 50, Game1.ScreenWidth / 2, 200);
            //Buttons
            SignIn = new Button("Fonts/TitleFont", "Log In", con, sp, Game1.ScreenWidth / 3, Game1.ScreenHeight / 2, Color.Black, 2, 300, 150, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
            //SignIn is drawn the first third of the scree
            SignUp = new Button("Fonts/TitleFont", "Sign Up", con, sp, Game1.ScreenWidth / 3 * 2, Game1.ScreenHeight / 2, Color.Black, 2, 300, 150, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
            //SignUp is drawn on the 2nd third 
            Enter = new Button("Fonts/TitleFont", "Enter", con, sp, (Game1.ScreenWidth / 2), (int)((Game1.ScreenHeight / 3) * 2.25), Color.Black, 2, 300, 100, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
            Enter.RemoveButton();
            //Set the enter button to hide

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
                        var result = LoginScreenQuery.PlayerIDQuery(Username.GetInput(), Password.GetInput()).ToString();
                        if (SignIn.ButtonPressed())
                        {
                            // Sign in logic
                            if (result == "null")
                            {
                                Username.ChangeText("Invalid Username Or password");
                                Password.ChangeText("ReEnter Details");
                            }
                            else
                            {
                                if (SavedFile.Count == 0)
                                {
                                    SavedFile.Add($"PlayerID,{result}");
                                }
                                else { SavedFile[0] = ($"PlayerID,{result}"); }
                                ScreenOver = true;
                            }
                        }
                        else
                        {
                            // Sign up logic
                            if (result == "null") // Possibly new person
                            {
                                // Check if the username already exists
                                var isPlayer = LoginScreenQuery.NewPlayerTbl(Username.GetInput(), Password.GetInput());
                                if (!isPlayer)
                                {
                                    // Get the PlayerID of the new account
                                    result = LoginScreenQuery.PlayerIDQuery(Username.GetInput(), Password.GetInput()).ToString();
                                    // Save the PlayerID to skip this in the future
                                    SavedFile.Add($"PlayerID,{result}");
                                    //add the achievment to the player
                                    LoginScreenQuery.AddAchievment(result, "1");
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
            //these things are always going to be drawn in this scene
            BackGround.Draw();
            LoginBox.Draw();
            HeaderBox.Draw();
            Header.Draw();
            Button.ButtonDraw();//draws any active buttons
            if (SignIn.ButtonPressed() || SignUp.ButtonPressed())
            {//when one of the buttons is pressed
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
            }
            Button.EndButtons();
            return ScreenOver;
        }

    }
}