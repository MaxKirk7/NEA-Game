using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
namespace SQLQuery;
class Sql
{
    private static string connection = "Data Source=MAX\\SQLEXPRESS,1433;Initial Catalog=NEA;User Id=NEAGame;Password=Chelsea_1;Encrypt = False;";
    public Sql()
    {
    }
    public string PlayerIDQuery(string Username = "null", string Password = "null")
    {
        var result = "null";
        var query = $"Select PlayerID From [Player Tbl] where Username = @User and Password = @Pass";
        using (SqlConnection con = new SqlConnection(connection))
        {
            con.Open();
            using (SqlCommand command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@User", Username);
                command.Parameters.AddWithValue("@Pass", Password);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    result = reader.GetInt32(0).ToString();
                    reader.Close();
                }
                reader.Close();
            }
            con.Close();
        }
        return result;
    }
    public bool NewPlayerTbl(string Username, string Password)
    {
        var ExistingUsername = false;
        var PlayerID = "";
        var query = "Select PlayerID From [Player Tbl] where Username = @User";
        using (SqlConnection con = new SqlConnection(connection))
        {
            con.Open();
            using (SqlCommand command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@user", Username);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        ExistingUsername = true;
                        reader.Read();
                        PlayerID = reader.GetInt32(0).ToString();
                    }
                }
            }
            con.Close();
        }
        if (!ExistingUsername)
        {
            query = "insert into [Player Tbl] (Username, Password) Values (@user, @pass);";
            using (SqlConnection con = new SqlConnection(connection))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@user", Username);
                    command.Parameters.AddWithValue("@pass", Password);
                    command.ExecuteNonQuery();
                }
                con.Close();
            }
        }
        return ExistingUsername;
    }
    public void AddAchievment(string PlayerID, string AchievmentID)
    {
        //add a new entry for a achievment completed by a player.
        //if not already exists!
        //check its not already achieved
        var PID = Int32.Parse(PlayerID);
        var AID = Int32.Parse(AchievmentID);
        var AddValue = false;
        using (SqlConnection con = new SqlConnection(connection))
        {
            con.Open();
            var query = "Use NEA select PlayerAchievmentID from [Player Achievment Tbl] where PlayerID = @player and AchievmentID =@achievment";
            using (SqlCommand command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@player", PID);
                command.Parameters.AddWithValue("@achievment", AID);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {//the query retruns null
                        AddValue = true;
                    }
                }
            }
            if (AddValue)
            //if its not achieved add the achievment
            {
                var query2 = "use NEA insert into [Player Achievment Tbl] (PlayerID,AchievmentID,DateObtained) values (@player,@achievment,@dateObtained);";
                using (SqlCommand command = new SqlCommand(query2, con))
                {
                    var date = DateTime.Now.Date; //remove the prefix of time from it
                    command.Parameters.AddWithValue("@player", PID);
                    command.Parameters.AddWithValue("@achievment", AID);
                    command.Parameters.AddWithValue("@dateObtained", date);
                    command.ExecuteNonQuery();
                }
            }
            con.Close();
        }

    }
    public List<int> GetAvailableSkin(string PlayerID)
    {
        List<int> AchievementIDs = new List<int>();
        var PID = Int32.Parse(PlayerID);

        using (SqlConnection con = new SqlConnection(connection))
        {
            con.Open();
            var query = "USE NEA SELECT AchievmentID FROM [Player Achievment Tbl] WHERE PlayerID = @player";
            using (SqlCommand command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@player", PID);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int achievementID = reader.GetInt32(0);
                        AchievementIDs.Add(achievementID);
                    }
                }
            }
        }

        return AchievementIDs;
    }


    public List<string> GetSkinLocation(string AchievementID)
    {
        var AID = Int32.Parse(AchievementID);
        var Locations = new List<string>();
        using (SqlConnection con = new SqlConnection(connection))
        {
            con.Open();
            var Query = "Use NEA select BaseSkinLocation,FlySkinLocation from [Achievment Tbl] where AchievmentID = @achievment";
            using (SqlCommand command = new SqlCommand(Query, con))
            {
                command.Parameters.AddWithValue("@achievment", AID);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Locations.Add(reader.GetString(i));
                    }
                }
            }
            con.Close();
        }
        return Locations;
    }
}