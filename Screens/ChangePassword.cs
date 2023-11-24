using _Sprites;
using GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NEAGame;
using NEAScreen;
class ChangePassword : IScreen
{
  bool NewGame = false;
  private Button Enter;
  private Thing BackGround;
  private Button Back;
  private BlankBox UsernameBox;
  private BlankBox PasswordBox;
  private BlankBox HeaderBox;
  private InputBox Username;
  private InputBox Password;
  private TextBox Header;
  private BlankBox EmailBox;
  private InputBox Email;
  private bool ScreenOver;
  public void LoadContent(ContentManager con, SpriteBatch sp)
  {

    if (!NewGame)
    {
      BackGround = new Thing("LoadingScreen/Sprites/BackGroundSpace", con, sp, Game1.ScreenWidth, Game1.ScreenHeight, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
      Back = new Button("Fonts/TitleFont", "Back", con, sp, 100, 980, Color.Black, 1.3, 100, 75, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
      Enter = new Button("Fonts/TitleFont", "Enter", con, sp, Game1.ScreenWidth / 2, (int)((Game1.ScreenHeight / 3) * 2.25), Color.Black, 2, 300, 100, new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
      Header = new TextBox("Fonts/TitleFont", "Change Password", con, sp, Game1.ScreenWidth / 2, 200, Color.Black, 1.8);
      HeaderBox = new BlankBox(Color.BlueViolet, con, sp, 300, 50, Game1.ScreenWidth / 2, 200);
      UsernameBox = new BlankBox(Color.Coral, con, sp, 300, 100, Game1.ScreenWidth / 2, 500);
      PasswordBox = new BlankBox(Color.Coral, con, sp, 300, 100, Game1.ScreenWidth / 2, 650);
      EmailBox = new BlankBox(Color.Coral, con, sp, 300, 100, Game1.ScreenWidth / 2, 350);
      Username = new InputBox("Fonts/TitleFont", "Enter Your Username...", con, sp, Game1.ScreenWidth / 2 , 500, Color.Black, 0.8, 100, 100, 15);
      Password = new InputBox("Fonts/TitleFont", "Enter Your Desired Password...", con, sp, Game1.ScreenWidth / 2 , 650, Color.Black, 0.8, 100, 100, 25);
      Email = new InputBox("Fonts/TitleFont", "Enter Your Email...", con, sp, Game1.ScreenWidth / 2 , 350, Color.Black, 0.8, 100, 100, 50);
    }
    else
    {
      Back.AddButton();
      Enter.AddButton();
      Email.AddBox();
      Password.AddBox();
      Username.AddBox();
    }
    NewGame = false;
  }

  public void Update(float delta)
  {
    Button.Update();
    Username.Update(delta);
    Email.Update(delta);
    Password.Update(delta);
    if (Enter.ButtonPressed())
    {
      if (IsValidInput())
      {
        if (Logic.UpdatePassword(Username.GetInput(), Password.GetInput(), Email.GetInput()))
        {
          ScreenOver = true;
        }
        else
        {
          Email.ChangeText("Not Found");
          Username.ChangeText("Not Found");
        }
      }
    }
    if(Back.ButtonPressed()){
      Back.ManualUnpress();
      ScreenOver = true;
    }

  }
  public void Draw(SpriteBatch sp)
  {
    sp.Begin();
    BackGround.Draw();
    UsernameBox.Draw();
    PasswordBox.Draw();
    HeaderBox.Draw();
    EmailBox.Draw();
    Button.ButtonDraw();
    Username.Draw();
    Email.Draw();
    Password.Draw();
    Header.Draw();
    sp.End();
  }

  public bool EndScreen()
  {
    var over = ScreenOver;
    if (ScreenOver)
    {
      ScreenOver = false;
      NewGame = true;
      Game1.LogIn = true;
      Button.EndButtons();
      InputBox.RemoveBoxes();
    }
    return over;
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
    if (string.IsNullOrEmpty(email) || !email.Contains('@'))
    {
      Email.ChangeText("Invalid Email Address");
      valid = false;
    }
    return valid;
  }
}