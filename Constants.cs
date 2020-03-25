using System;
using System.Collections.Generic;
using System.Text;

namespace FTPDataQuery.Models
{
    class Constants
    {
        public readonly Student Student = new Student { StudentID = "200448232", FirstName = "Swarnim", LastName = "Sharma" };

        public class Locations
        {
            public readonly static string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            public readonly static string ExePath = Environment.CurrentDirectory;

            public readonly static string ContentsFolder = $"{ExePath}\\..\\..\\..\\Content";
            public readonly static string ImagesFolder = $"{ContentsFolder}\\Images";
            public readonly static string DataFolder = $"{ContentsFolder}\\Data";
            public const string InfoFile = "info.csv";
            public const string ImageFile = "myimage.jpg";

            //string filePath = $@"{desktopPath}\info.csv";
            public readonly static string FilePath = $@"{DataFolder}\info.csv";
        }
        public class FTP
        {
            public const string UserName = @"bdat100119f\bdat1001";
            public const string Password = "bdat1001";

            public const string BaseUrl = "ftp://waws-prod-dm1-127.ftp.azurewebsites.windows.net/bdat1001-20914";

            public const int OperationPauseTime = 0000;
        }
    }
}
