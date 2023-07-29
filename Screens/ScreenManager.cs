using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NEAScreen
{
    class ScreenManager
    {
        private IScreen activeScreen;
        private IScreen NextScreen;
        public void setScreen(IScreen NScreen, ContentManager Con, SpriteBatch sp)
        {
            NextScreen = NScreen;
            switchScreen(Con,sp);
        }
        private void switchScreen(ContentManager con, SpriteBatch sp)
        {
            if (NextScreen != null)
            {
                activeScreen = NextScreen;
                activeScreen.LoadContent(con,sp);
            }
            NextScreen = null;
        }
        public void Draw(SpriteBatch sp)
        {
            activeScreen?.Draw(sp);
        }
        public void Update(float delta)
        {
            activeScreen?.Update(delta);
        }
        public IScreen currentScreen()
        {
            return activeScreen;
        }
        public bool IsScreenOver(){
            return activeScreen.EndScreen();
        }
    }
}