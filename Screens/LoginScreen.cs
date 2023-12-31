using System.Collections.Generic;
using _Sprites;
using NEAGame;
using SQLQuery;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using GameLogic;

namespace NEAScreen
{
    class LoginScreen : IScreen
    {
        private static bool HasForgotPassword = false;
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
        private Button Back;
        private Button ButtonForgotPassword;
        private bool SigningUp = false;
        private List<string> SavedFile = new();

        public void LoadContent(ContentManager con, SpriteBatch sp)
        {
            HasForgotPassword = false;
            SavedFile.Clear();
            SavedFile = Logic.PullFile();
            if (SavedFile.Count > 0 && !String.IsNullOrWhiteSpace(SavedFile[0].Split(",")[1]) && Sql.IsVerified)
            {
                ScreenOver = true;
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
                //other buttons
                Back = new Button("Fonts/TitleFont", "Back", con, sp, 100, 980, Color.Black, 1.3, 100, 75, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
                ButtonForgotPassword = new Button("Fonts/TitleFont", "ForgotPassword", con, sp, Game1.ScreenWidth - 100, 980, Color.Black, 1.3, 150, 75, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
                SignIn = new Button("Fonts/TitleFont", "Log In", con, sp, Game1.ScreenWidth / 3, Game1.ScreenHeight / 2, Color.Black, 2, 300, 175, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
                SignUp = new Button("Fonts/TitleFont", "Sign Up", con, sp, Game1.ScreenWidth / 3 * 2, Game1.ScreenHeight / 2, Color.Black, 2, 300, 150, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
                Enter = new Button("Fonts/TitleFont", "Enter", con, sp, Game1.ScreenWidth / 2, (int)((Game1.ScreenHeight / 3) * 2.25), Color.Black, 2, 300, 100, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
            }
            else
            {
                SignIn.AddButton();
                SignUp.AddButton();
            }
            FirstLoop = true;
            Username = new InputBox("Fonts/TitleFont", "Enter Your Username...", con, sp, Game1.ScreenWidth / 2, 500, Color.Black, 0.8, 100, 100, 15);
            Password = new InputBox("Fonts/TitleFont", "Enter Your Password...", con, sp, Game1.ScreenWidth / 2, 650, Color.Black, 0.8, 100, 100, 25);
            Email = new InputBox("Fonts/TitleFont", "Enter Your Email...", con, sp, Game1.ScreenWidth / 2 , 350, Color.Black, 0.8, 100, 100, 50);
            //hide Enter Button as the game starts
            Enter.RemoveButton();
            Back.RemoveButton();
            ButtonForgotPassword.RemoveButton();
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
                    Back.AddButton();
                    FirstLoop = false;
                    ButtonForgotPassword.AddButton();
                }
                Username.Update(delta);
                Password.Update(delta);
                if (Enter.ButtonPressed())
                {
                    if (IsValidInput())
                    {
                        if (SignIn.ButtonPressed())
                        {
                            var result = Logic.FindUser(Username.GetInput(), Password.GetInput()); // find the PlayerID spot or if it exists
                            // Handle sign-in logic
                            if (!result.Item1)
                            {
                                Username.ChangeText("Invalid Username Or password");
                                Password.ChangeText("Re-Enter Details");
                            }
                            else
                            {
                                //store player ID
                                SavedFile[0] = $"PlayerID,{result.Item2}";
                                ScreenOver = true;
                            }
                        }
                        else if (SigningUp)
                        {
                            var NewAccount = Logic.CreateNewUser(Username.GetInput(), Password.GetInput(), Email.GetInput());
                            if (NewAccount.Item1) // if the account details are new
                            {
                                SavedFile[0] = $"PlayerID,{NewAccount.Item2}";
                                ScreenOver = true;
                            }
                            else
                            {
                                Username.ChangeText("Username Already Exists");
                            }
                        }
                    }
                }
            }
            if (Back.ButtonPressed())
            {
                ButtonForgotPassword.RemoveButton();
                Back.ManualUnpress();
                ScreenOver = true;
                Game1.LogIn = true;
            }
            if (ButtonForgotPassword.ButtonPressed())
            {
                ButtonForgotPassword.ManualUnpress();
                ScreenOver = true;
                HasForgotPassword = true;
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
                Logic.AuthenticateMachine(SavedFile[0].Split(",")[1]);
                Logic.PushFile(SavedFile);
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
        public static bool ForgotPassword()
        {
            return HasForgotPassword;
        }
    }
}
