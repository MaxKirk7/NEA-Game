using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using Microsoft.IdentityModel.Tokens;
using SQLQuery;

namespace GameLogic
{
    public static class Logic
    {
        public static List<string> PullFile()
        {
            //open or create the file and then read from it
            using (FileStream stream = new("SavedInfo.txt", FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
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
    }
}