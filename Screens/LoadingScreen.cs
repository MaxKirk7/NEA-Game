using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using _Sprites;
using NEAGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using SQLQuery;
using GameLogic;

namespace NEAScreen
{
    class LoadingScreen : IScreen
    {
        private bool LoadingScreenOver = false;
        private int LoadingTime = 4 * Game1.Frames; // time before the game loads
        private Thing Icon;
        private Thing BackGround;
        private Song LoadingSound;
        private BlankBox TitleBox;
        private TextBox Title;
        public void LoadContent(ContentManager con, SpriteBatch sp)
        {
            BackGround = new Thing("LoadingScreen/Sprites/BackGroundSpace", con, sp, 1920, 1080, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);//sets a space background
            Icon = new Thing("LoadingScreen/Sprites/Moon", con, sp, 200, 200, Game1.ScreenWidth / 2, Game1.ScreenHeight / 2); // centre of the screen
            TitleBox = new BlankBox(new Color(58, 90, 62, 150), con, sp, 400, 120, Game1.ScreenWidth / 2, 150);
            Title = new TextBox("Fonts/TitleFont", "Salam Armada", con, sp, Game1.ScreenWidth / 2, 150, new Color(212, 152, 177), 3.5);
            LoadingSound = con.Load<Song>("LoadingScreen/Sounds/IntroSong");
            MediaPlayer.Volume = 1F;
            MediaPlayer.Play(LoadingSound);
        }
        public void Draw(SpriteBatch sp)
        //spinning animation as the game loads
        {
            sp.Begin();
            BackGround.Draw();
            TitleBox.Draw();
            Title.Draw();
            Icon.SpinDraw();
            sp.End();
        }
        public void Update(float delta)
        {
            if (!Sql.HasReset)
            {
                var SavedFile = Logic.PullFile();
                Sql.UpdateWeeklyLeaderboard();
                if (SavedFile.Count > 0)
                {
                    Logic.CheckMachineAuthentication(SavedFile[0].Split(",")[1]);
                }
            }
            LoadingTime--;
            if (LoadingTime <= 0)
            {
                MediaPlayer.Stop();
                LoadingScreenOver = true;
            }
            Icon?.Update(delta);
        }
        public bool EndScreen()
        {
            return LoadingScreenOver;
        }

    }
}