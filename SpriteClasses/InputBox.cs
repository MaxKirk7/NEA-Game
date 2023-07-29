using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace _Sprites;
class InputBox : TextBox
{
    private List<char> Input;
    private bool BoxSelected;
    private int MaxLength; //set the max length of this input
    private static List<InputBox> AllInputBoxes = new List<InputBox>();
    private bool KeyPress = false; //make sure each charecter is only inputed once
    private Rectangle rect;
    public InputBox(string FontLocation, string text, ContentManager con, SpriteBatch sp, int X, int Y, Color StringColor, double scale, int width, int height, int MaxLength) : base(FontLocation, text, con, sp, X, Y, StringColor, scale, width, height)
    {
        AllInputBoxes.Add(this);
        Input = new List<char>();
        BoxSelected = false;
        rect = new Rectangle(X - width / 2, Y - height / 2, width, height); //create the rectangle of the InputBox
        this.MaxLength = MaxLength;
    }
    public void Update()
    {
        CheckSelected();
        if (BoxSelected && !KeyPress && Input.Count < MaxLength)
        {
            GetKeyboardInput();
        }
        if (Keyboard.GetState().GetPressedKeyCount() == 0)
        {
            KeyPress = false;
        }
    }
    private void GetKeyboardInput()
    {
        var KeyboardState = Keyboard.GetState();
        if (KeyboardState.GetPressedKeys().Length > 0)
        {
            KeyPress = true; // set that a key is being pressed
            var PressedKey = KeyboardState.GetPressedKeys()[0];
            if (PressedKey == Keys.Back || PressedKey == Keys.Delete)
            { // delete back a charecter
                if (Input.Count > 0)
                {
                    Input.RemoveAt(Input.Count - 1);
                }
            }
            else
            {

                var CharKey = PressedKey.ToString()[0];
                List<Keys> AcceptedPuncuation = new List<Keys>{
                    Keys.OemQuotes, //will represent @
                    Keys.OemPeriod,
                    Keys.OemMinus,
                    Keys.OemComma,
                    Keys.OemPlus //will represent underscore
                };
                if (((PressedKey >= Keys.A && PressedKey <= Keys.Z) || //check if its alphebetical
            (PressedKey >= Keys.D0 && PressedKey <= Keys.D9) || AcceptedPuncuation.Contains(PressedKey)))//check if key is numerical or accepted puncutation
                {
                    if (PressedKey >= Keys.D0 && PressedKey <= Keys.D9)
                    {
                        Input.Add(GetNumberChar(PressedKey));
                    }
                    else if (AcceptedPuncuation.Contains(PressedKey)){
                        Input.Add(GetPunctuation(PressedKey));
                    }
                    else
                    {
                        if (char.IsLetter(CharKey)) //only records Letters,Digits,Symbols nad punctuation
                        {

                            if (KeyboardState.CapsLock && char.IsLetterOrDigit(CharKey)) //Checks if capslock is active
                            {
                                CharKey = Char.ToUpper(CharKey);
                            }
                            else
                            {
                                CharKey = Char.ToLower(CharKey);
                            }
                            Input.Add(CharKey);
                        }
                    }
                }
            }
        }
    }
    public override void Draw() //Write the openng message, then once the user types will show what they are typing
    {
        if (Input.ToArray().Length == 0)
        {
            base.Draw();
        }
        else
        {
            string StringInput = "";
            foreach (char c in Input)
            {
                StringInput += c;
            }
            Vector2 NewOrigin = Font.MeasureString(Input.ToString()) * 0.5F;
            Vector2 NewPosition = new Vector2(Position.X + rect.Width / 3, Position.Y);
            Sp.DrawString(Font, StringInput, NewPosition, StringColor, 0F, NewOrigin, (float)FontScale, SpriteEffects.None, 0f);
        }
    }
    public string GetInput()
    {//retrun what the final input is for user
        var StringInput = "";
        foreach (char c in Input)
        {
            StringInput += c;
        }
        return StringInput;
    }
    private void DeselectOtherBoxes()
    {
        foreach (InputBox box in AllInputBoxes)
        {
            if (box.BoxSelected)
            {
                box.BoxSelected = false;
            }
        }
    }
    private void CheckSelected()
    {
        MouseState mouse = Mouse.GetState();
        //if rectangle contains the mouse when leftclciekd then box is selected
        if (rect.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed)
        {
            DeselectOtherBoxes();
            BoxSelected = true;
            // When selected other boxes are deselected
        }
    }
    private char GetNumberChar(Keys key)
    {
        switch (key)
        {
            case Keys.D0: return '0';
            case Keys.D1: return '1';
            case Keys.D2: return '2';
            case Keys.D3: return '3';
            case Keys.D4: return '4';
            case Keys.D5: return '5';
            case Keys.D6: return '6';
            case Keys.D7: return '7';
            case Keys.D8: return '8';
            case Keys.D9: return '9';
            default: return '0';
        }
    }
    private char GetPunctuation(Keys key){
        switch (key){
            case Keys.OemPeriod: return '.';
            case Keys.OemMinus: return '-';
            case Keys.OemComma: return ',';
            case Keys.OemQuotes: return '@';
            case Keys.OemPlus: return '_';
            default: return' ';
        }
    }
    public override void ChangeText(string NewText)
    {
        Input.Clear();
        base.ChangeText(NewText);
    }

}