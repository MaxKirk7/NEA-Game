using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NEAGameObjects;

namespace NEAScreen;
class MainGame : IScreen
{
    private bool IsDead = false;
    private int CurrentScore = 0;
    private static List<string> SavedFile = new();
    public void LoadContent(ContentManager con, SpriteBatch sp)
    {
        using (FileStream stream = new("SavedInfo.txt", FileMode.Open, FileAccess.Read))
        {
            using (StreamReader reader = new(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    SavedFile.Add(line);
                }
            }
        }
    }

    public void Update(float delta)
    {
        if (!IsDead){
            Player.Update(delta);
        }
    }
    public void Draw(SpriteBatch sp)
    {
        sp.Begin();
        Player.Draw(sp);
        sp.End();
    }

    public bool EndScreen()
    {
        return false;
    }
    public static List<string> saveFile()
    {
        return SavedFile;
    }

}