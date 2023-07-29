using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _Sprites;
class BlankBox : Sprite{
    public BlankBox(Color BoxColor,ContentManager con, SpriteBatch sp, int Width, int Height, int X, int Y) : base(BoxColor,con,sp,Width,Height,X,Y){
        //Found Online using Monogame Texture2D Tutorial
        this.Texture = new Texture2D(sp.GraphicsDevice,1,1);
        this.Texture.SetData(new[] {BoxColor});
    }
}