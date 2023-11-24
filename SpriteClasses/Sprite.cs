using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _Sprites
{
    class Sprite
    {
        protected ContentManager Con;
        protected SpriteBatch Sp;
        protected Vector2 Position;
        protected Vector2 Size;
        protected Texture2D Texture;
        public Rectangle Rect;
        public Sprite(string Location, ContentManager con, SpriteBatch sp, int Width, int Height, int X, int Y)
        {
            if (Location != null){
                Texture = con.Load<Texture2D>(Location);
            }
            this.Con = con;
            this.Sp = sp;
            Position.X = X - Width / 2; //Centre of the Sprite
            Position.Y = Y - Height / 2; //
            Size.X = Width;
            Size.Y = Height;
        }
        public Sprite(Color BoxColor,ContentManager con, SpriteBatch sp, int Width, int Height, int X, int Y)
        {
            this.Con = con;
            this.Sp = sp;
            Position.X = X - Width / 2; //Centre of the Sprite
            Position.Y = Y - Height / 2; //
            Size.X = Width;
            Size.Y = Height;
        }

        public virtual void Draw()
        {
            this.Rect = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
            Sp.Draw(Texture, Rect, Color.White);
        }
        public void ChangeTexture(string Location)
        {
            Texture = Con.Load<Texture2D>(Location);
        }
    }
}