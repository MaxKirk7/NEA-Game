using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using SQLQuery;

namespace GameLogic
{
    public static class Logic
    {
        public static List<string> PullFile()
        {
            //open or create the file and then read from it
            using (FileStream stream = new("SavedInfo.txt", FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (StreamReader reader = new(stream))
                {
                    string line;
                    var file = new List<string>(5);
                    while ((line = reader.ReadLine()) != null)
                    { //read all the lines that are not blank/empty
                        var PlainLine = Decode(line);
                        file.Add(PlainLine);
                    }
                    //return the file to be used in the program
                    return file;
                }
            }
        }
        public static void PushFile(List<string> file)
        {
            //open file for truncate and ability to write so that writing to a empty file
            using (FileStream stream = new("SavedInfo.txt", FileMode.Truncate, FileAccess.Write))
            {
                using (StreamWriter writer = new(stream))
                {
                    foreach (string line in file)
                    {
                        if (!line.IsNullOrEmpty())
                        {//only write if the line is not empty
                            var EncodedLine = Encode(line);
                            writer.WriteLine(EncodedLine);
                        }
                    }
                    writer.Flush();
                }
            }
        }
        public static void PushFile(string text) // logic for if uploading text not a file
        {
            using (FileStream stream = new("SavedInfo.txt", FileMode.Truncate, FileAccess.Write))
            {
                using (StreamWriter writer = new(stream))
                {
                    var Text = text.Split("\n");
                    foreach (var line in Text)
                    {
                        var EncodedLine = Encode(line);
                        writer.WriteLine(EncodedLine);
                    }
                    writer.Flush();
                }
            }
        }
        private static string Encode(string BaseText)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(BaseText);
            return Convert.ToBase64String(bytes);
        }
        private static string Decode(string line)
        {
            byte[] encrypted = Convert.FromBase64String(line);
            return System.Text.Encoding.UTF8.GetString(encrypted);
        }

        private static object GetMACAddress()
        {
            string macAddresses = "";

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return macAddresses;
        }
        public static bool CheckMachineAuthentication(string PlayerID)
        {
            Sql.CheckDevice(GetMACAddress().ToString(), PlayerID);
            return false;
        }
        public static void AuthenticateMachine(string PlayerID)
        {
            Sql.AddDevice(GetMACAddress().ToString(), PlayerID);
        }
        private static string GetHashValue(string input)
        {
            var InputToBytes = System.Text.Encoding.UTF8.GetBytes(input);
            var InputToHash = SHA256.HashData(InputToBytes);
            var HexValue = Convert.ToHexString(InputToHash);
            return HexValue;
        }
        public static (bool, int) CreateNewUser(string username, string password, string email)
        {
            var result = (false, -1);
            var PotentialIndex = BigInteger.Parse(GetHashValue(username), System.Globalization.NumberStyles.HexNumber);
            PotentialIndex %= 1000; // index within 1000
            if (PotentialIndex < 0) { PotentialIndex *= -1; }
            else if (PotentialIndex == 0) { PotentialIndex = 1000; }
            var data = Sql.CheckForPlayerID((int)PotentialIndex, username);
            result.Item2 = data.Item2;
            if (!data.Item1)
            {
                Sql.CreateAccount(data.Item2, username, password, email);
                result.Item1 = true;
            }
            return result;
        }
        public static (bool, int) FindUser(string username, string password)
        {
            var PotentialIndex = BigInteger.Parse(GetHashValue(username), System.Globalization.NumberStyles.HexNumber);
            PotentialIndex %= 1000; // index within 1000
            if (PotentialIndex < 0) { PotentialIndex *= -1; }
            else if (PotentialIndex == 0) { PotentialIndex = 1000; }
            var data = Sql.CheckForPlayerID((int)PotentialIndex, username); // states whether account was found and the index at which it was found
            //check the passwords match
            if (data.Item1)
            {
                data.Item1 = Sql.CheckPassword(data.Item2, password);
            }
            else { data.Item1 = false; }
            return data;
        }
        public static bool UpdatePassword(string username, string password, string email)
        {
            var PotentialIndex = BigInteger.Parse(GetHashValue(username), System.Globalization.NumberStyles.HexNumber);
            PotentialIndex %= 1000; // index within 1000
            if (PotentialIndex < 0) { PotentialIndex *= -1; }
            else if (PotentialIndex == 0) { PotentialIndex = 1000; }
            var data = Sql.CheckForPlayerID((int)PotentialIndex, username);
            if (data.Item1)
            {
                data.Item1 = Sql.ChangePassword(data.Item2, username, email, password);
            }
            return data.Item1;
        }
    }
}