
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NEAScreen{
    interface IScreen{
        void LoadContent(ContentManager con, SpriteBatch sp);

        void Update(float delta);

        void Draw(SpriteBatch sp);

        public bool EndScreen();
    }
}