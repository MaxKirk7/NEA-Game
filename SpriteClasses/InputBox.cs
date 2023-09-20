using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _Sprites;
class InputBox : TextBox
{
    private readonly List<char> Input;
    private bool BoxSelected;
    private readonly int MaxLength; //set the max length of this input
    private static readonly List<InputBox> AllInputBoxes = new();
    private Rectangle rect;
    private readonly Queue Q = new(10);
    private float LastKeyPressTime = 0F;
    private readonly float debounceDelay = 0.14f; //delay in seconds between recording keys
    public InputBox(string FontLocation, string text, ContentManager con, SpriteBatch sp, int X, int Y, Color StringColor, double scale, int width, int height, int MaxLength) : base(FontLocation, text, con, sp, X, Y, StringColor, scale, width, height)
    {
        AllInputBoxes.Add(this);
        Input = new List<char>();
        BoxSelected = false;
        rect = new Rectangle(X - width / 2, Y - height / 2, width, height); //create the rectangle of the InputBox
        this.MaxLength = MaxLength;
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
            Vector2 NewPosition = new(Position.X + rect.Width / 3, Position.Y);
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
    private static void DeselectOtherBoxes()
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

    public override void ChangeText(string NewText)
    {
        Input.Clear();
        base.ChangeText(NewText);
    }

    private void GetUserInput(float delta)
    {
        KeyboardState CurrentInput = Keyboard.GetState();
        var keyInputs = CurrentInput.GetPressedKeys();

        foreach (Keys key in keyInputs)
        {
            // Check if enough time has passed since the last key press
            if (LastKeyPressTime >= debounceDelay)
            {
                if (key != Keys.LeftShift || key != Keys.RightShift)
                {
                    Q.enQueue(key); // Add the key pressed to the queue
                    LastKeyPressTime = 0f; // Reset timeSinceLastKeyPress
                }
            }

        }

        // Update timeSinceLastKeyPress
        LastKeyPressTime += delta;
    }
    private void AppendWithInput()
    {
        var key = Q.deQueue();
        if (key == Keys.Back){
            if (Input.Count > 0){
                Input.RemoveAt(Input.Count-1);
            }
        }
        if (key == Keys.None)
        {
            return; // only workds with valid input
        }
        var CharKey = KeyToChar(key);
        if (CharKey != ' ')
        {
            Input.Add(CharKey);
            Log.Information($"Input appended: {Input.Count()}");
            Log.Information($"Key value = {CharKey}");
        }
    }
    private static char KeyToChar(Keys key)
    {
        var keyboardstate = Keyboard.GetState();
        bool isShiftPressed = keyboardstate.IsKeyDown(Keys.RightShift);
        if (keyboardstate.CapsLock && isShiftPressed)
        {
            isShiftPressed = false;
        }
        else if (keyboardstate.CapsLock && !isShiftPressed)
        {
            isShiftPressed = true;
        }
        if (keyboardstate.IsKeyDown(Keys.OemQuotes) && isShiftPressed){
            return '@';
        }
        if (keyboardstate.IsKeyDown(Keys.OemMinus) && isShiftPressed){
            return '_';
        }
        // Determine the character based on key and shift pressed
        switch (key)
        {
            case Keys.A:
                return isShiftPressed ? 'A' : 'a';
            case Keys.B:
                return isShiftPressed ? 'B' : 'b';
            case Keys.C:
                return isShiftPressed ? 'C' : 'c';
            case Keys.D:
                return isShiftPressed ? 'D' : 'd';
            case Keys.E:
                return isShiftPressed ? 'E' : 'e';
            case Keys.F:
                return isShiftPressed ? 'F' : 'f';
            case Keys.G:
                return isShiftPressed ? 'G' : 'g';
            case Keys.H:
                return isShiftPressed ? 'H' : 'h';
            case Keys.I:
                return isShiftPressed ? 'I' : 'i';
            case Keys.J:
                return isShiftPressed ? 'J' : 'j';
            case Keys.K:
                return isShiftPressed ? 'K' : 'k';
            case Keys.L:
                return isShiftPressed ? 'L' : 'l';
            case Keys.M:
                return isShiftPressed ? 'M' : 'm';
            case Keys.N:
                return isShiftPressed ? 'N' : 'n';
            case Keys.O:
                return isShiftPressed ? 'O' : 'o';
            case Keys.P:
                return isShiftPressed ? 'P' : 'p';
            case Keys.Q:
                return isShiftPressed ? 'Q' : 'q';
            case Keys.R:
                return isShiftPressed ? 'R' : 'r';
            case Keys.S:
                return isShiftPressed ? 'S' : 's';
            case Keys.T:
                return isShiftPressed ? 'T' : 't';
            case Keys.U:
                return isShiftPressed ? 'U' : 'u';
            case Keys.V:
                return isShiftPressed ? 'V' : 'v';
            case Keys.W:
                return isShiftPressed ? 'W' : 'w';
            case Keys.X:
                return isShiftPressed ? 'X' : 'x';
            case Keys.Y:
                return isShiftPressed ? 'Y' : 'y';
            case Keys.Z:
                return isShiftPressed ? 'Z' : 'z';
            case Keys.D0:
                return isShiftPressed ? ')' : '0';
            case Keys.D1:
                return isShiftPressed ? '!' : '1';
            case Keys.D2:
                return isShiftPressed ? '"' : '2';
            case Keys.D3:
                return isShiftPressed ? '£' : '3';
            case Keys.D4:
                return isShiftPressed ? '$' : '4';
            case Keys.D5:
                return isShiftPressed ? '%' : '5';
            case Keys.D6:
                return isShiftPressed ? '^' : '6';
            case Keys.D7:
                return isShiftPressed ? '&' : '7';
            case Keys.D8:
                return isShiftPressed ? '*' : '8';
            case Keys.D9:
                return isShiftPressed ? '(' : '9';
            case Keys.OemTilde:
                return isShiftPressed ? '~' : '#';
            case Keys.OemSemicolon:
                return isShiftPressed ? ':' : ';';
            case Keys.OemQuotes:
                return isShiftPressed ? '@' : '\'';
            case Keys.OemPipe:
                return isShiftPressed ? '|' : '\\';
            case Keys.OemOpenBrackets:
                return isShiftPressed ? '{' : '[';
            case Keys.OemCloseBrackets:
                return isShiftPressed ? '}' : ']';
            case Keys.OemComma:
                return isShiftPressed ? '<' : ',';
            case Keys.OemPeriod:
                return isShiftPressed ? '>' : '.';
            case Keys.OemMinus:
                return isShiftPressed ? '_' : '-';
            case Keys.OemPlus:
                return isShiftPressed ? '+' : '=';
            case Keys.OemQuestion:
                return isShiftPressed ? '?' : '/';
            default:
                return ' ';
        }
    }

    public void Update(float delta)
    {
        CheckSelected();
        if (BoxSelected && Input.Count < MaxLength)
        {
            GetUserInput(delta);
            AppendWithInput();
        }
    }
}