using SQLQuery;

namespace _Sprites
{
    class Skin
    {

        public string BaseSkin { get; private set; }
        public string FlyingSkin { get; private set; }
        public int AchievementID { get; private set; }

        public Skin(int ID)
        {
            AchievementID = ID;
            var Locations = Sql.GetSkinLocation(AchievementID.ToString());
            BaseSkin = Locations[0];
            FlyingSkin = Locations[1];
        }
    }
}
