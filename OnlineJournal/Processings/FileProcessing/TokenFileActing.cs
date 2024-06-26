using System;
using System.Configuration;
using System.IO;

namespace OnlineJournal.Processings.FileProcessing
{
    class TokenFileActing
    {
        private const string TOKENFILENAME = "data.txt";

        public string GetToken()
        {
            string file = GetPath(TOKENFILENAME);

            if (!File.Exists(file))
                return null;

            try
            {
                using (StreamReader reader = File.OpenText(file))
                {
                    return reader.ReadLine();
                }
            }
            catch
            {
                return null;
            }
        }

        public void SaveToken(string token)
        {
            File.WriteAllText(GetPath(TOKENFILENAME), token);
        }

        public void DeleteTokenFile()
        {
            string file = GetPath(TOKENFILENAME);

            if (File.Exists(file))
                File.Delete(file);
        }

        public bool IsTokenExist()
        {
            string file = GetPath(TOKENFILENAME);

            return File.Exists(file);
        }

        private static string GetPath(string fileName)
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appDataFolder = Path.Combine(localAppData, ConfigurationManager.AppSettings["directoryName"]);

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            return Path.Combine(appDataFolder, fileName);
        }
    }
}
