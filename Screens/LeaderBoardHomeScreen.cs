using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using _Sprites;
using Microsoft.Xna.Framework;
using NEAGame;

namespace NEAScreen;
class LeaderBoardHomeScreen : IScreen
{
    private bool ScreenOver = false;
    private BlankBox LeaderBoardBackGround;
    //Background
    //HighScores top 3 + own personal high score + top 3 weakly high
    //BackSymbol
    private Button Back;
    public void LoadContent(ContentManager con, SpriteBatch sp)
    {
        LeaderBoardBackGround = new(new Color(10, 20, 5, 180),con,sp,500,500,Game1.ScreenWidth/2,Game1.ScreenHeight/2);
        Back = new("Fonts/TitleFont","Back",con,sp,100,980,Color.Black,1.3,100,75,new Color(50, 80, 12), Color.BlueViolet, "Buttons/Rounded Square Button");
    }

    public void Update(float delta)
    {
        Button.Update();
        if (Back.ButtonPressed()){
            ScreenOver = true;
        }
    }
    public void Draw(SpriteBatch sp)
    {
        sp.Begin();
        LeaderBoardBackGround.Draw();
        Button.ButtonDraw();
        sp.End();
    }

    public bool EndScreen()
    {
        var Over = ScreenOver;
        ScreenOver = false;
        if (Over){
            Button.EndButtons();
        }
        return Over;
    }

}