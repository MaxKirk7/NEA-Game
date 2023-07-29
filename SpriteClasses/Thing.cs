using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _Sprites
{
    class Thing : Sprite
    {
        private Vector2 Origin;
        private float rotationAngle = 0;
        private static float rotationsPerSec = 0.25F;
        float rotationSpeed = (MathHelper.TwoPi) * rotationsPerSec;
        public Thing(string Location, ContentManager con, SpriteBatch sp, int Width, int Height, int X, int Y) : base(Location, con, sp, Width, Height, X, Y)
        {
            Origin = new Vector2(Texture.Width/2,Texture.Height/2);
        }

        public void SpinDraw()
        {
            Vector2 adjustedPosition = Position + Origin /2;
            this.Rect = new Rectangle((int)adjustedPosition.X, (int)adjustedPosition.Y, (int)Size.X, (int)Size.Y);
            Sp.Draw(Texture, Rect, null, Color.White, rotationAngle, Origin, SpriteEffects.None, 0f);
        }
        public void Update(float delta)
        {
            rotationAngle += rotationSpeed * delta;
        }
    }
}