using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _Sprites;
class TextBox 
//this class is very similar to the Sprite class, however because the fact that this will be using SpriteFont not a Texture2d a new super class is needed
{
    protected ContentManager Con;
    protected SpriteBatch Sp;
    protected Vector2 Position;
    protected SpriteFont Font;
    protected Vector2 Origin;
    protected Color StringColor;
    protected string Text;
    protected double FontScale;
    public TextBox(string FontLocation,string text, ContentManager con, SpriteBatch sp,int X, int Y, Color StringColor, double scale)
    {
        if (FontLocation != null)
        {
            Font = con.Load<SpriteFont>(FontLocation);
        }
        this.Con = con;
        this.Sp = sp;
        Position.X = X ; //Centre of the textbox
        Position.Y = Y;
        Origin = Font.MeasureString(text) * 0.5F; //The centre of the Text box will be half the length of the words
        this.StringColor = StringColor;
        this.Text = text;
        FontScale = scale; //SpriteFonts are very small so this is needed to make it larger
    }
    public TextBox(string FontLocation,string text, ContentManager con, SpriteBatch sp,int X, int Y, Color StringColor, double scale, int width,int height)
    {
        if (FontLocation != null)
        {
            Font = con.Load<SpriteFont>(FontLocation);
        }
        this.Con = con;
        this.Sp = sp;
        Position.X = X ; //Centre of the textbox
        Position.Y = Y;
        Origin = Font.MeasureString(text) * 0.5F; //The centre of the Text box will be half the length of the words
        this.StringColor = StringColor;
        this.Text = text;
        FontScale = scale; //SpriteFonts are very small so this is needed to make it larger
    }
    public virtual void Draw(){
        Sp.DrawString(Font,Text,Position,StringColor,0F,Origin,(float)FontScale,SpriteEffects.None,0f);
        //we dont want any rotation or layerdepth so both are 0F
    }
    public virtual void ChangeText(string NewText){
        Text = NewText;
    }
}