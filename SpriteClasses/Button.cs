using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace _Sprites;
class Button : TextBox
{
    private bool IsActive = true;
    private Color ActiveColour;
    private Color Normal;
    private Color Highlighted;
    private Rectangle rect;
    private Texture2D ButtonBackground;
    private static List<Button> AllButtons = new List<Button>();
    private bool IsPressed = false;
    public Button(string FontLocation, string text, ContentManager con, SpriteBatch sp, int X, int Y, Color StringColor, double scale, int width, int height, Color NormalColour, Color HighlightedColour, string ButtonLook = "Buttons/Rounded Square Button") : base(FontLocation, text, con, sp, X, Y, StringColor, scale, width, height)
    {
        if (AllButtons == null){
            AllButtons = new List<Button>();
        }
        AllButtons.Add(this);
        ButtonBackground = con.Load<Texture2D>(ButtonLook);
        Normal = NormalColour;
        Highlighted = HighlightedColour;
        rect = new Rectangle(X - width / 2, Y - height / 2, width, height);
    }
    public static void Update(){
        foreach (var button in AllButtons){
            if(button.IsActive){
                button.UpdateAllButton();
            }
        }
    }
    public static void ButtonDraw(){
        foreach (var button in AllButtons){
            if (button.IsActive){
                button.DrawAllButtons();
            }
        }    
    }
    private void UpdateAllButton(){
        //change colour when button highlighted
        MouseState mouse = Mouse.GetState();
        if (rect.Contains(mouse.Position)){
            ActiveColour = Highlighted;
            if (mouse.LeftButton == ButtonState.Pressed){
                IsPressed = true;
            }
        }
        else{ActiveColour = Normal;}
    }
    private void DrawAllButtons()
    {
        Sp.Draw(ButtonBackground,rect,ActiveColour);
        base.Draw();
    }
    public bool ButtonPressed(){
        return IsPressed;
    }
    public void RemoveButton(){
        this.IsActive =false;
    }
    public void AddButton(){
        this.IsActive = true;
    }
    public void UnPress(){
        IsPressed = false;
    }
    public static void EndButtons(){
        AllButtons.Clear();
    }
}
