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
    public static string PlayerIDQuery(string Username = "null", string Password = "null")
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
    public static void AddAchievment(string PlayerID, string AchievmentID)
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
    public static List<int> GetAvailableSkin(string PlayerID)
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


    public static List<string> GetSkinLocation(string AchievementID)
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
    public static bool CreateAccount(string Username, string Password, string Email)
    {
        bool newAccount = false;
        int newPlayerID = -1;

        using (SqlConnection con = new SqlConnection(connection))
        {
            con.Open();
            // Check if the username already exists
            string usernameQuery = "SELECT PlayerID FROM [Player Tbl] WHERE Username = @User";
            using (SqlCommand command = new SqlCommand(usernameQuery, con))
            {
                command.Parameters.AddWithValue("@User", Username);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        // Username is not found, proceed to check email
                        reader.Close(); // Close the previous reader
                    }
                }
                string emailQuery = "SELECT PlayerID FROM [Player Info Tbl] WHERE Email = @Email";
                using (SqlCommand emailCommand = new SqlCommand(emailQuery, con))
                {
                    emailCommand.Parameters.AddWithValue("@Email", Email);
                    using (SqlDataReader emailReader = emailCommand.ExecuteReader())
                    {
                        if (!emailReader.HasRows)
                        {
                            // Both username and email are new, create the account
                            newAccount = true;
                            // Insert into [Player Tbl] and get the new PlayerID
                        }
                    }
                }
                if (newAccount)
                {
                    string createUserQuery = "insert into [Player Tbl] (Username, Password) Values (@User, @Pass);";
                    using (SqlCommand createUserCommand = new SqlCommand(createUserQuery, con))
                    {
                        createUserCommand.Parameters.AddWithValue("@User", Username);
                        createUserCommand.Parameters.AddWithValue("@Pass", Password);
                        createUserCommand.ExecuteNonQuery();
                    }
                    var UserIDQuery = "select PlayerID from [Player Tbl] where Username = @User and Password = @Pass";
                    using (SqlCommand GetUserID = new(UserIDQuery, con))
                    {
                        GetUserID.Parameters.AddWithValue("@User", Username);
                        GetUserID.Parameters.AddWithValue("@Pass", Password);
                        using (SqlDataReader PlayerIDReader = GetUserID.ExecuteReader())
                        {
                            PlayerIDReader.Read();
                            newPlayerID = PlayerIDReader.GetInt32(0);
                        }

                        // Insert into [Player Info Tbl] with the new PlayerID
                        string createEmailQuery = "INSERT INTO [Player Info Tbl] (Email, PlayerID,NickName,DateCreated) VALUES (@Email, @PlayerID,@NickName,@DateCreated)";
                        using (SqlCommand createEmailCommand = new SqlCommand(createEmailQuery, con))
                        {
                            var date = DateTime.Now.Date;
                            var NickName = Username.Substring(0, 4); // nickanme is first 4 leters of username
                            createEmailCommand.Parameters.AddWithValue("@Email", Email);
                            createEmailCommand.Parameters.AddWithValue("@PlayerID", newPlayerID);
                            createEmailCommand.Parameters.AddWithValue("@NickName", NickName);
                            createEmailCommand.Parameters.AddWithValue("@DateCreated", date);
                            createEmailCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            con.Close();
        }
        return newAccount;
    }
    public static List<List<string>> HighScores(string PlayerID)
    {
        var PID = Int32.Parse(PlayerID);
        List<List<string>> Scores = new(6) { };
        var HighScoreQuery = "use NEA select top 3 HighScore, NickName from [Player Info Tbl] where HighScore is not null order by HighScore DESC;";
        using (SqlConnection con = new SqlConnection(connection))
        {
            con.Open();
            using (SqlCommand command = new(HighScoreQuery, con))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            List<string> tempscore = new() { };
                            string HighScore = reader["HighScore"].ToString();
                            string NickName = reader["NickName"].ToString();
                            tempscore.Add(NickName);
                            tempscore.Add(HighScore);
                            Scores.Add(tempscore);
                        }
                    }
                }
            }
            con.Close();
        }
        //make sure the list has full amount of values
        if (Scores.Count < 3)
        {
            for (int i = Scores.Count - 1; i < 3; i++)
            {
                Scores.Add(null);
            }
        }
        //weekly scores
        var WeeklyQuery = "USE NEA; SELECT TOP 2 WeeklyHighScore, Nickname FROM [Player Info Tbl] WHERE WeeklyHighScore IS NOT NULL ORDER BY WeeklyHighScore DESC;";

        using (SqlConnection con = new(connection))
        {
            con.Open();
            using (SqlCommand command = new(WeeklyQuery, con))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            List<string> tempscore = new() { };
                            string Weekly = reader["WeeklyHighScore"].ToString();
                            string NickName = reader["NickName"].ToString();
                            tempscore.Add(NickName);
                            tempscore.Add(Weekly);
                            Scores.Add(tempscore);
                        }
                    }
                }
            }
            con.Close();
        }
        if (Scores.Count < 5)
        {
            for (int i = Scores.Count - 1; i < 5; i++)
            {
                Scores.Add(null);
            }
        }
        var Personal = "USE NEA; SELECT HighScore, Nickname FROM [Player Info Tbl] WHERE PlayerID = @PlayerID AND HighScore IS NOT NULL;";
        using (SqlConnection con = new(connection))
        {
            con.Open();
            using (SqlCommand command = new(Personal,con))
            {
                command.Parameters.AddWithValue("@PlayerID", PID);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            List<string> tempscore = new() { };
                            string HighScore = reader["HighScore"].ToString();
                            string NickName = reader["NickName"].ToString();
                            tempscore.Add(NickName);
                            tempscore.Add(HighScore);
                            Scores.Add(tempscore);
                        }
                    }
                }
            }
            con.Close();
        }
        if (Scores.Count < 6)
        {
            Scores.Add(null);
        }
        return Scores;
    }
    public static int GetScore(string PlayerID)
    {
        int Value = 0;
        int PID = Int32.Parse(PlayerID);
        var Query = "use NEA select HighScore from [Player Info Tbl] where PlayerID = @PlayerID and HighScore is not null";
        using (SqlConnection con = new SqlConnection(connection))
        {
            con.Open();
            using (SqlCommand command = new SqlCommand(Query, con))
            {
                command.Parameters.AddWithValue("@PlayerID", PID);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Value = reader.GetInt32(0);
                    }
                }
            }
            con.Close();
        }
        return Value;
    }
    public static int UpdateScore(List<string> file, int LastScore)
    {
        double AverageScore = 0;
        var PID = int.Parse(file[0].Replace("PlayerID,", ""));
        int GamesPlayed = int.Parse(file[2].Replace("GamesPlayed,", ""));
        //get the last average score 
        using (SqlConnection con = new(connection))
        {
            var query = "use NEA select AverageScore from [Player Info Tbl] where PlayerID = @PlayerID and AverageScore is not null";
            con.Open();
            using (SqlCommand command = new(query, con))
            {
                command.Parameters.AddWithValue("@PlayerID", PID);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows && reader.Read())
                    {
                        AverageScore = reader.GetDouble(0);
                    }
                }
            }
            //update the average score
            var UpdateQuery = "use NEA update [Player Info Tbl] set AverageScore = @Score where PlayerID = @PlayerID";
            using (SqlCommand AverageScoreUpdate = new SqlCommand(UpdateQuery, con))
            {
                // calcualte new average score
                AverageScore = (AverageScore * (GamesPlayed - 1) + LastScore) / GamesPlayed;
                AverageScoreUpdate.Parameters.AddWithValue("@Score", AverageScore);
                AverageScoreUpdate.Parameters.AddWithValue("@PlayerID", PID);
                AverageScoreUpdate.ExecuteNonQuery();
            }
            //update the MaxScore
            var UpdateHighScore = "Use NEA update [Player Info Tbl] set HighScore = @Score where PlayerID = @PlayerID and (HighScore < @Score or HighScore is null)";
            using (SqlCommand HighScoreCommand = new(UpdateHighScore, con))
            {
                HighScoreCommand.Parameters.AddWithValue("@Score", LastScore);
                HighScoreCommand.Parameters.AddWithValue("@PlayerID", PID);
                HighScoreCommand.ExecuteNonQuery();
            }
            con.Close();
        }
        return GetAverageScore(PID.ToString());
    }
    public static int GetAverageScore(string PlayerID)
    {
        double Value = 0;
        int PID = Int32.Parse(PlayerID);
        var Query = "use NEA select AverageScore from [Player Info Tbl] where PlayerID = @PlayerID and AverageScore is not null";
        using (SqlConnection con = new SqlConnection(connection))
        {
            con.Open();
            using (SqlCommand command = new SqlCommand(Query, con))
            {
                command.Parameters.AddWithValue("@PlayerID", PID);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows && reader.Read())
                    {
                        Value = reader.GetDouble(0);
                    }
                }
            }
            con.Close();
        }
        return (int)Value;
    }
    public static List<string> ResetAverageScore(List<String> File)
    {
        var PID = int.Parse(File[0].Replace("PlayerID,", ""));
        var query = "use NEA update [Player Info Tbl] set AverageScore = 0 where PlayerID = @PlayerID";
        using (SqlConnection con = new(connection))
        {
            con.Open();
            using SqlCommand command = new SqlCommand(query, con);
            command.Parameters.AddWithValue("@PlayerID", PID);
            command.ExecuteNonQuery();
            con.Close();
        }
        if (File.Count < 3)
        {
            File.Add("GamesPlayed,0");
        }
        else { File[2] = "GamesPlayed,0"; }
        return File;
    }
}