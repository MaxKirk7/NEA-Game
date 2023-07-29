using System;
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
        try
        {
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
        }
        catch (Exception ex)
        {
            result = ex.ToString();
        }
        return result;
    }
    public bool NewPlayerTbl(string Username, string Password)
    {
        var ExistingUsername = false;
        var query = "Select PlayerID From [Player Tbl] where Username = @User";
        using (SqlConnection con = new SqlConnection(connection))
        {
            con.Open();
            using (SqlCommand command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@user", Username);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) { ExistingUsername = true; }
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
}