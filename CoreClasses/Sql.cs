using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Azure.Core.Pipeline;
using Microsoft.Data.SqlClient;
namespace SQLQuery;
class Sql
{
    private static string connection = "Data Source=MAX\\SQLEXPRESS,1433;Initial Catalog=NEA;User Id=NEAGame;Password=Chelsea_1;Encrypt = True;TrustServerCertificate=true;";
    public static bool HasReset { get; private set; }
    public static bool IsVerified { get; private set; }
    private const int IndexJump = 3;
    public static List<int> GetAvailableSkin(string PlayerID)
    {
        List<int> AchievementIDs = new List<int>();
        var PID = int.Parse(PlayerID);

        using (SqlConnection con = new SqlConnection(connection))
        {
            con.Open();
            var query = "USE NEA SELECT AchievmentID FROM [PlayerAchievment Tbl] WHERE PlayerID = @player";
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
    public static List<List<string>> HighScores(string PlayerID)
    {
        var Index = Int32.Parse(PlayerID);
        List<List<string>> Scores = new(6) { };
        using (SqlConnection con = new SqlConnection(connection))
        {
            con.Open();
            using (SqlTransaction transaction = con.BeginTransaction())
            {
                var HighScoreQuery = "use NEA select top 3 HighScore, NickName from [Player Info Tbl] where HighScore is not null order by HighScore DESC;";
                using (SqlCommand HighScoreCommand = new(HighScoreQuery, con, transaction))
                {
                    using (SqlDataReader reader = HighScoreCommand.ExecuteReader())
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            var tempscore = new List<string>(2);
                            if (reader.Read())
                            {
                                tempscore.Add(reader["HighScore"].ToString());
                                tempscore.Add(reader["NickName"].ToString());
                            }
                            else
                            {
                                tempscore.Add(null);
                            }
                            Scores.Add(tempscore);
                        }
                    }
                }
                var WeeklyQuery = "USE NEA; SELECT TOP 2 WeeklyHighScore, NickName FROM [Player Info Tbl] WHERE WeeklyHighScore IS NOT NULL ORDER BY WeeklyHighScore DESC;";
                using (SqlCommand WeeklyCommand = new(WeeklyQuery, con, transaction))
                {
                    using (SqlDataReader reader = WeeklyCommand.ExecuteReader())
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            var tempscore = new List<string>(2);
                            if (reader.Read())
                            {
                                tempscore.Add(reader["WeeklyHighScore"].ToString());
                                tempscore.Add(reader["NickName"].ToString());
                            }
                            else { tempscore.Add(null); }
                            Scores.Add(tempscore);
                        }
                    }
                }
                var Personal = "SELECT HighScore, NickName FROM [Player Info Tbl] WHERE PlayerID = @PlayerID AND HighScore IS NOT NULL;";
                using (SqlCommand PersonalCommand = new(Personal, con, transaction))
                {
                    PersonalCommand.Parameters.AddWithValue("@PlayerID", Index);
                    using (SqlDataReader reader = PersonalCommand.ExecuteReader())
                    {
                        var tempscore = new List<string>();
                        if (reader.Read())
                        {
                            tempscore.Add(reader["HighScore"].ToString());
                            tempscore.Add(reader["NickName"].ToString());
                        }
                        else { tempscore.Add(null); }
                        Scores.Add(tempscore);
                    }
                }
                transaction.Commit();
            }
            con.Close();
        }
        return Scores;
    }
    public static int UpdateScore(List<string> file, int LastScore)
    {
        double AverageScore = 0;
        var Index = int.Parse(file[0].Replace("PlayerID,", ""));
        int GamesPlayed = int.Parse(file[2].Replace("GamesPlayed,", ""));
        //get the last average score 
        using (SqlConnection con = new(connection))
        {
            con.Open();
            using (SqlTransaction transaction = con.BeginTransaction())
            {
                var AverageScoreQuery = "use NEA select AverageScore from [Player Info Tbl] where PlayerID = @PlayerID and AverageScore is not null";
                using (SqlCommand AverageScoreCommand = new(AverageScoreQuery, con, transaction))
                {
                    AverageScoreCommand.Parameters.AddWithValue("@PlayerID", Index);
                    using (SqlDataReader reader = AverageScoreCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            AverageScore = reader.GetDouble(0);
                        }
                    }
                }
                //update the average score
                var AverageUpdateQuery = "update [Player Info Tbl] set AverageScore = @Score where PlayerID = @PlayerID";
                using (SqlCommand AverageScoreUpdate = new(AverageUpdateQuery, con, transaction))
                {
                    // calcualte new average score
                    AverageScore = (AverageScore * (GamesPlayed - 1) + LastScore) / GamesPlayed;
                    AverageScoreUpdate.Parameters.AddWithValue("@Score", AverageScore);
                    AverageScoreUpdate.Parameters.AddWithValue("@PlayerID", Index);
                    AverageScoreUpdate.ExecuteNonQuery();
                }
                //update the MaxScore
                var UpdateHighScoreQuery = "update [Player Info Tbl] set HighScore = @Score where PlayerID = @PlayerID and (HighScore < @Score or HighScore is null)";
                using (SqlCommand HighScoreUpdateCommand = new(UpdateHighScoreQuery, con, transaction))
                {
                    HighScoreUpdateCommand.Parameters.AddWithValue("@Score", LastScore);
                    HighScoreUpdateCommand.Parameters.AddWithValue("@PlayerID", Index);
                    HighScoreUpdateCommand.ExecuteNonQuery();
                }
                var UpdateWeeklyScoreQuery = "update [Player Info Tbl] set WeeklyHighScore = @Score where PlayerID = @PlayerID and (WeeklyHighScore < @Score or WeeklyHighScore is null)";
                using (SqlCommand WeeklyScoreUpdateCommand = new(UpdateWeeklyScoreQuery, con, transaction))
                {
                    WeeklyScoreUpdateCommand.Parameters.AddWithValue("@Score", LastScore);
                    WeeklyScoreUpdateCommand.Parameters.AddWithValue("@PlayerID", Index);
                    WeeklyScoreUpdateCommand.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            con.Close();
        }
        return GetAverageScore(Index.ToString());
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
    public static void UpdateWeeklyLeaderboard()
    {
        HasReset = true;
        //get the last time the Leaderboard was reset
        DateTime LastDate;
        var TodaysDate = DateTime.UtcNow;
        var MondayDate = 0;
        {
            using (SqlConnection con = new(connection))
            {
                var DateQuery = "select LastWeeklyReset from [GameInfo Tbl]";
                con.Open();
                using (SqlCommand CheckDate = new(DateQuery, con))
                {
                    using (SqlDataReader reader = CheckDate.ExecuteReader())
                    {
                        reader.Read();
                        LastDate = reader.GetDateTime(0);
                    }
                }
                con.Close();
            }
            // want to reset every Monday, find the first occurence of a monday
            for (int i = 1; i < 7; i++)
            {
                var Day = new DateTime(TodaysDate.Year, 1, i);
                if (Day.DayOfWeek.ToString() == "Monday")
                {
                    MondayDate = i;
                    break;
                }
            }
            // current day of year subtract monday date when modulous calculated will equal 0
            //or if the last date erased is over 7 days
            if (((TodaysDate.DayOfYear - MondayDate) % 7 == 0 && TodaysDate.Date != LastDate.Date) || (TodaysDate.DayOfYear - LastDate.DayOfYear) >= 7)
            {
                //reset weekly leaderboard
                var Query = "Update [Player Info Tbl] set WeeklyHighScore = null";
                using (SqlConnection con = new(connection))
                {
                    con.Open();
                    using (SqlCommand command = new(Query, con))
                    {
                        command.ExecuteNonQuery();
                    }
                    var UpdateDateQuery = "update [GameInfo Tbl] set LastWeeklyReset = @Date";
                    using (SqlCommand UpdateDate = new(UpdateDateQuery, con))
                    {
                        UpdateDate.Parameters.AddWithValue("@Date", TodaysDate.Date);
                        UpdateDate.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
        }
    }
    public static void AddDevice(string MAC, string PlayerID)
    {
        CheckDevice(MAC, PlayerID);
        if (!IsVerified)
        {
            if (int.TryParse(PlayerID, out var PID))
            {

                using (SqlConnection con = new(connection))
                {
                    con.Open();
                    var query = "insert into [VerifiedMachines Tbl] values (@Player,@Adr)";
                    using (SqlCommand command = new(query, con))
                    {
                        command.Parameters.AddWithValue("@Player", PID);
                        command.Parameters.AddWithValue("@Adr", MAC);
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
        }
    }
    public static void CheckDevice(string MAC, string PlayerID)
    {
        IsVerified = false;
        if (int.TryParse(PlayerID, out var PID))
        {
            var query = "select VerifiedMAC from [VerifiedMachines Tbl] where PlayerID = @PLayer";
            using (SqlConnection con = new(connection))
            {
                con.Open();
                using (SqlCommand command = new(query, con))
                {
                    command.Parameters.AddWithValue("@Player", PID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        int Address = 0;
                        while (reader.Read())
                        {
                            if (reader.GetString(Address) == MAC)
                            {
                                IsVerified = true;
                                break;
                            }
                            Address++;
                        }
                    }
                }
                con.Close();
            }
        }
    }
    public static (bool, int) CheckForPlayerID(int PlayerID, string Username)
    {
        var data = (false, PlayerID);
        var query = "select Username from [Player Tbl] where PlayerID = @Player";
        using (SqlConnection con = new(connection))
        {
            con.Open();
            using (SqlCommand command = new(query, con))
            {
                command.Parameters.AddWithValue("@Player", PlayerID);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        data.Item1 = true;
                        if (reader.Read())
                        {
                            if (reader.GetString(0) != null && reader.GetString(0) != Username)
                            {
                                data = CheckForPlayerID(PlayerID + IndexJump, Username);
                            }
                        }
                    }
                }
            }
            con.Close();
        }
        return data;
    }
    public static void CreateAccount(int index, string username, string password, string email)
    {
        var PlayerTblQuery = "insert into [Player Tbl] values (@PlayerID,@User,@Pass)";
        var PlayerInfoQuery = "insert into [Player Info Tbl] (Email,PlayerID,NickName,DateCreated) values (@Email,@PlayerID,@NickName,@DateCreated)";
        using (SqlConnection con = new(connection))
        {
            con.Open();
            using (SqlTransaction transaction = con.BeginTransaction())
            {
                using (SqlCommand PlayerTblCommand = new(PlayerTblQuery, con, transaction))
                {
                    PlayerTblCommand.Parameters.AddWithValue("@PlayerID", index);
                    PlayerTblCommand.Parameters.AddWithValue("@User", username);
                    PlayerTblCommand.Parameters.AddWithValue("@Pass", password);
                    PlayerTblCommand.ExecuteNonQuery();
                }
                using (SqlCommand PlayerInfoCommand = new(PlayerInfoQuery, con, transaction))
                {
                    var date = DateTime.Now.Date;
                    var NickName = username.Substring(0, 4); // nickanme is first 4 leters of username
                    PlayerInfoCommand.Parameters.AddWithValue("@Email", email);
                    PlayerInfoCommand.Parameters.AddWithValue("@PlayerID", index);
                    PlayerInfoCommand.Parameters.AddWithValue("@NickName", NickName);
                    PlayerInfoCommand.Parameters.AddWithValue("@DateCreated", date);
                    PlayerInfoCommand.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            con.Close();
        }
        AddAchievment(index, 1);
    }
    public static void AddAchievment(int index, int achievmentID)
    {
        var AddValue = false;
        using (SqlConnection con = new SqlConnection(connection))
        {
            con.Open();
            var query = "Use NEA select * from [PlayerAchievment Tbl] where PlayerID = @Player and AchievmentID =@Achievment";
            using (SqlCommand command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@Player", index);
                command.Parameters.AddWithValue("@Achievment", achievmentID);
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
                var query2 = "use NEA insert into [PlayerAchievment Tbl] (PlayerID,AchievmentID,DateObtained) values (@Player,@Achievment,@DateObtained);";
                using (SqlCommand command = new(query2, con))
                {
                    var date = DateTime.Now.Date; //remove the prefix of time from it
                    command.Parameters.AddWithValue("@Player", index);
                    command.Parameters.AddWithValue("@Achievment", achievmentID);
                    command.Parameters.AddWithValue("@DateObtained", date);
                    command.ExecuteNonQuery();
                }
            }
            con.Close();
        }
    }
    public static bool CheckPassword(int index, string password)
    {
        bool PasswordMatch = false;
        using (SqlConnection con = new(connection))
        {
            var Query = "select * from [Player Tbl] where PlayerID = @Player and Password = @Password";
            con.Open();
            using (SqlCommand command = new(Query, con))
            {
                command.Parameters.AddWithValue("@Player", index);
                command.Parameters.AddWithValue("@Password", password);
                using SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    PasswordMatch = true;
                }
            }
            con.Close();
        }
        return PasswordMatch;
    }
    public static bool ChangePassword(int index, string username, string email, string password)
    {
        bool success = false;
        using (SqlConnection con = new(connection))
        {
            con.Open();
            using (SqlTransaction transaction = con.BeginTransaction())
            {
                var CheckPassword = "Select Email from [Player Info Tbl] where PlayerID = @Player";
                using (SqlCommand CheckPasswordCommand = new(CheckPassword, con, transaction))
                {
                    CheckPasswordCommand.Parameters.AddWithValue("@Player", index);
                    using (SqlDataReader reader = CheckPasswordCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            success = reader.GetString(0) == email;
                        }
                    }
                }
                if (success)
                {
                    var UpdatePasswordQuery = "Update [Player Tbl] set Password = @Pass where PlayerID = @Player";
                    using (SqlCommand UpdatePassword = new(UpdatePasswordQuery, con, transaction))
                    {
                        UpdatePassword.Parameters.AddWithValue("@Pass", password);
                        UpdatePassword.Parameters.AddWithValue("@Player",index);
                        UpdatePassword.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
            con.Close();
        }
        return success;
    }
}