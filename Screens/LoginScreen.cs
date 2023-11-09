using System.Collections.Generic;
using System.IO;
using _Sprites;
using NEAGame;
using SQLQuery;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace NEAScreen
{
    class LoginScreen : IScreen
    {
        private bool FirstLoop;
        private bool NewScreen = false;
        private bool ScreenOver = false;
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
        private BlankBox EmailBox;
        private InputBox Email;
        private bool SigningUp = false;
        private readonly List<string> SavedFile = new();

        public void LoadContent(ContentManager con, SpriteBatch sp)
        {
            SavedFile.Clear();
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
                if (SavedFile.Count > 0 && !String.IsNullOrWhiteSpace(SavedFile[0].Split(",")[1]))
                {
                    ScreenOver = true;
                }
            }
            if (!NewScreen)
            {
                //Common Things that aren't going to change
                BackGround = new Thing("LoadingScreen/Sprites/BackGroundSpace", con, sp, Game1.ScreenWidth, Game1.ScreenHeight, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
                LoginBox = new BlankBox(new Color(10, 20, 5, 180), con, sp, 1000, 800, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
                //MainHeader Parts
                Header = new TextBox("Fonts/TitleFont", "Salam Armada", con, sp, Game1.ScreenWidth / 2, 200, Color.Black, 1.8);
                HeaderBox = new BlankBox(Color.BlueViolet, con, sp, 300, 50, Game1.ScreenWidth / 2, 200);
                //stuff that is drawn when any button is pressed
                UsernameBox = new BlankBox(Color.Coral, con, sp, 300, 100, Game1.ScreenWidth / 2, 500);
                PasswordBox = new BlankBox(Color.Coral, con, sp, 300, 100, Game1.ScreenWidth / 2, 650);
                // only when signing up
                EmailBox = new BlankBox(Color.Coral, con, sp, 300, 100, Game1.ScreenWidth / 2, 350);

                SignIn = new Button("Fonts/TitleFont", "Log In", con, sp, Game1.ScreenWidth / 3, Game1.ScreenHeight / 2, Color.Black, 2, 300, 150, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
                SignUp = new Button("Fonts/TitleFont", "Sign Up", con, sp, Game1.ScreenWidth / 3 * 2, Game1.ScreenHeight / 2, Color.Black, 2, 300, 150, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
                Enter = new Button("Fonts/TitleFont", "Enter", con, sp, Game1.ScreenWidth / 2, (int)((Game1.ScreenHeight / 3) * 2.25), Color.Black, 2, 300, 100, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
            }
            else
            {
                SignIn.AddButton();
                SignUp.AddButton();
            }
            FirstLoop = true;
            Username = new InputBox("Fonts/TitleFont", "Enter Your Username...", con, sp, Game1.ScreenWidth / 2, 500, Color.Black, 1, 300, 100, 15);
            Password = new InputBox("Fonts/TitleFont", "Enter Your Password...", con, sp, Game1.ScreenWidth / 2, 650, Color.Black, 1, 300, 100, 25);
            Email = new InputBox("Fonts/TitleFont", "Enter Your Email...", con, sp, Game1.ScreenWidth / 2 - 50, 350, Color.Black, 0.8, 300, 100, 50);
            //hide Enter Button as the game starts
            Enter.RemoveButton();
            NewScreen = false;

        }

        public void Update(float delta)
        {
            Button.Update();
            if (SignIn.ButtonPressed() || SignUp.ButtonPressed())
            {
                if (SignUp.ButtonPressed())
                {
                    SigningUp = true;
                    Email.Update(delta);
                }
                if (FirstLoop)
                {
                    SignIn.RemoveButton();
                    SignUp.RemoveButton();
                    Enter.AddButton();
                    FirstLoop = false;
                }
                Username.Update(delta);
                Password.Update(delta);
                if (Enter.ButtonPressed())
                {

                    if (IsValidInput())
                    {
                        var result = Sql.PlayerIDQuery(Username.GetInput(), Password.GetInput()).ToString();
                        if (SignIn.ButtonPressed())
                        {
                            // Handle sign-in logic
                            if (result == "null")
                            {
                                Username.ChangeText("Invalid Username Or password");
                                Password.ChangeText("Re-Enter Details");
                            }
                            else
                            {
                                // Handle successful sign-in logic
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
                        else if (SigningUp)
                        {
                            // Handle sign-up logic
                            if (result == "null")
                            {
                                //log error here
                                var NewAccount = Sql.CreateAccount(Username.GetInput(), Password.GetInput(), Email.GetInput());
                                //end error
                                if (NewAccount) // if the account details are new
                                {
                                    string PlayerID = Sql.PlayerIDQuery(Username.GetInput(), Password.GetInput());
                                    SavedFile[0] = $"PlayerID,{PlayerID}";
                                    Sql.AddAchievment(PlayerID, "1");
                                    ScreenOver = true;
                                }
                                else
                                {
                                    Username.ChangeText("Username May already Exists");
                                    Email.ChangeText("Email May already Exist");
                                }
                            }
                            else
                            {
                                Username.ChangeText("Username already Exists");
                            }
                        }
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
                //only draws email if signingup
                if (SigningUp)
                {
                    EmailBox.Draw();
                    Email.Draw();
                }
            }
            sp.End();
        }

        public bool EndScreen()
        {
            var Over = ScreenOver;
            if (ScreenOver)
            {
                ScreenOver = false;
                NewScreen = true;
                SigningUp = false;
                InputBox.RemoveBoxes();
                File.WriteAllLines("SavedInfo.txt", SavedFile);
                Button.EndButtons();
            }
            return Over;
        }

        private bool IsValidInput()
        {
            string username = Username.GetInput();
            string password = Password.GetInput();
            string email = Email.GetInput();
            bool valid = true;

            if (string.IsNullOrEmpty(username) || username.Length < 5)
            {
                Username.ChangeText("Username should be at least 5 characters");
                valid = false;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                Password.ChangeText("Password should be at least 8 characters");
                valid = false;
            }

            if (SigningUp) // only checks if signingup
            {
                if (string.IsNullOrEmpty(email) || !email.Contains('@'))
                {
                    Email.ChangeText("Invalid Email Address");
                    valid = false;
                }
            }

            return valid;
        }

    }
}
