using FTPDataQuery.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace FTPDataQuery
{
    class Program
    {
        private static int choice;

        static void Main(string[] args)
        {
            List<string> directoryResults = FTP.GetDirectory(Constants.FTP.BaseUrl, Constants.FTP.UserName, Constants.FTP.Password);
            List<Student> students = new List<Student>();
            string myStudentID = ""; //200448232
            Student student = new Student();
            StringBuilder csvData = new StringBuilder("StudentID,FirstName,LastName,DateOfBirth,ImageData");
            StringBuilder jsonData = new StringBuilder("[");
            StringBuilder xmlData = new StringBuilder("<Students>");

            Console.WriteLine("Please Enter your StudentID:");
            myStudentID = Console.ReadLine();
            Console.WriteLine("Importing data...\n");

            foreach (var directory in directoryResults)
            {
                student = new Student() { AbsoluteUrl = Constants.FTP.BaseUrl };
                student.FromDirectory(directory);

                string infoFilePath = student.FullPathUrl + "/" + Constants.Locations.InfoFile;
                bool fileExists = FTP.FileExists(infoFilePath);

                // Check if the directory is your directory
                student.MyRecord = student.StudentID.Equals(myStudentID) ? true : false; // The ?: condition is for clarity, not necessary here

                Console.WriteLine($"{student}:");

                if (fileExists == true)
                {
                    try
                    {
                        setStudentInfo(student, directory, infoFilePath);
                        students.Add(student);
                    }
                    catch (Exception)
                    {

                        Console.WriteLine("Error in the csv file");
                    }
                }
                else
                {
                    Console.WriteLine("Couldn't find info file");
                }
                Console.WriteLine($"    {infoFilePath}");

                string imageFilePath = student.FullPathUrl + "/" + Constants.Locations.ImageFile;
                //FTP.DownloadFile(imageFilePath, csvPath);
                fileExists = FTP.FileExists(imageFilePath);

                if (fileExists == true)
                {
                    Console.WriteLine("Found image file");
                    //If this is your record
                    if (student.MyRecord)
                    {
                        try
                        {
                            if (student.ImageData.Length == 0)
                            {
                                setImageData(student);
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Couldn't find image file");
                }
                Console.WriteLine(imageFilePath);
            }

            Console.WriteLine("Import Complete!");

            Console.WriteLine("Students found:");
            foreach (Student stud in students)
            {
                //Console.WriteLine(stud);
                csvData.AppendLine($"{stud.ToCSV()},");
                jsonData.Append($"{stud.ToJSON()},");
                xmlData.Append($"{stud.ToXML()}");
            }
            jsonData.Append("]");
            xmlData.Append("</Students>");

            //Upload the JSON and XML files
            Student MyStudRecord = students.Find(s => s.MyRecord == true);
            uploadFile(MyStudRecord, "students.csv", Encoding.UTF8.GetBytes(csvData.ToString()));
            uploadFile(MyStudRecord, "students.json", Encoding.UTF8.GetBytes(jsonData.ToString()));
            uploadFile(MyStudRecord, "students.xml", Encoding.UTF8.GetBytes(xmlData.ToString()));

            do
            {
                int stud_count = 0;
                int ch = 0;
                List<int> valid_choice = new List<int> { 1, 2, 3 };

                printMenu();

                try
                {
                    choice = Convert.ToInt32(Console.ReadLine());
                    switch (choice)
                    {
                        //Total number of students
                        case 1:
                            Console.WriteLine($"Total number of students is {students.Count}");
                            break;
                        //Starts with
                        case 2:
                            do
                            {
                                Console.WriteLine("What do you want to filter students by ?\n 1> Student ID\n 2> First Name \n 3> Last Name");

                                try
                                {
                                    ch = Convert.ToInt32(Console.ReadLine());
                                    Console.WriteLine("Enter the character you want to filter by:");
                                    char character = Convert.ToChar(Console.ReadLine());
                                    switch (ch)
                                    {
                                        case 1:
                                            stud_count = students.FindAll(studs => studs.StudentID.ToLower().StartsWith(character.ToString().ToLower())).Count;
                                            break;
                                        case 2:
                                            stud_count = students.FindAll(studs => studs.FirstName.ToLower().StartsWith(character.ToString().ToLower())).Count;
                                            break;
                                        case 3:
                                            stud_count = students.FindAll(studs => studs.LastName.ToLower().StartsWith(character.ToString().ToLower())).Count;
                                            break;
                                        default:
                                            Console.WriteLine("Enter a valid choice");
                                            break;
                                    }
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("Enter a valid choice");
                                    ch = 0;
                                }

                            } while ( !valid_choice.Contains(ch) );
                            
                            Console.WriteLine($"Total number of students after filtering are: {stud_count}");
                            break;
                        //Contains
                        case 3:
                            do
                            {
                                Console.WriteLine("What do you want to filter students by ?\n 1> Student ID\n 2> First Name \n 3> Last Name");

                                try
                                {
                                    ch = Convert.ToInt32(Console.ReadLine());
                                    Console.WriteLine("Enter the string you want to filter by:");
                                    string str_contain = Console.ReadLine();
                                    switch (ch)
                                    {
                                        case 1:
                                            stud_count = students.FindAll(studs => studs.StudentID.ToLower().Contains(str_contain.ToLower())).Count;
                                            break;
                                        case 2:
                                            stud_count = students.FindAll(studs => studs.FirstName.ToLower().Contains(str_contain.ToLower())).Count;
                                            break;
                                        case 3:
                                            stud_count = students.FindAll(studs => studs.LastName.ToLower().Contains(str_contain.ToLower())).Count;
                                            break;
                                        default:
                                            Console.WriteLine("Enter a valid choice");
                                            break;
                                    }
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("Enter a valid choice");
                                    ch = 0;
                                }

                            } while (!valid_choice.Contains(ch));

                            Console.WriteLine($"Total number of students after filtering are: {stud_count}");
                            break;
                        //Your record
                        case 4:
                            Console.WriteLine($"Your record is :\n{MyStudRecord}");
                            break;
                        //Average Age
                        case 5:
                            double avg_age = students.Select(studs => studs.Age).Average();
                            Console.WriteLine($"Average Age of students is: {avg_age}");
                            break;
                        //Highest Age
                        case 6:
                            int high_age = students.Select(studs => studs.Age).Max();
                            Console.WriteLine($"Highest Age of students is: {high_age}");
                            break;
                        //Lowest Age
                        case 7:
                            int low_age = students.Select(studs => studs.Age).Min();
                            Console.WriteLine($"Lowest Age of students is: {low_age}");
                            break;
                        //Display CSV
                        case 8:
                            Console.WriteLine(csvData);
                            break;
                        //Display JSON
                        case 9:
                            Console.WriteLine(jsonData);
                            break;
                        //Display XML
                        case 10:
                            Console.WriteLine(xmlData);
                            break;
                        //Exit
                        case 11:
                            Console.WriteLine("GoodBye!");
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Enter a valid choice");
                }
            } while (choice != 11);

        }

        private static void printMenu()
        {
            Console.WriteLine("\n\nWhat would you like to know?");
            Console.WriteLine("1>Display count of Total students:");
            Console.WriteLine("2> Count of students \x1B[4m starting with \x1B[0m a specific string:");
            Console.WriteLine("3> Count of students \x1B[4m Containing \x1B[0m a specific string:");
            Console.WriteLine("4> Your Student Record:");
            Console.WriteLine("5> Average age of Student:");
            Console.WriteLine("6> Highest age of Student:");
            Console.WriteLine("7> Lowest age of Student:");
            Console.WriteLine("8> Show CSV format of students file:");
            Console.WriteLine("9> Show JSON format of students file:");
            Console.WriteLine("10> Show XML format of students file:");
            Console.WriteLine("11> Exit:");
        }

        private static void setStudentInfo(Student student,string directory,string infoFilePath)
        {
            string csvPath = $@"C:\Users\kashi\Desktop\Student_Files\{directory}.csv";
            //FTP.DownloadFile(infoFilePath, csvPath);
            byte[] bytes = FTP.DownloadFileBytes(infoFilePath);
            string csvData = Encoding.Default.GetString(bytes);

            string[] csvlines = csvData.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            if (csvlines.Length != 2)
            {
                Console.WriteLine($"Error in CSV format");
            }
            else
            {
                Console.WriteLine("Found info file");
                student.FromCSV(csvlines[1]);
            }

        }

        private static void setImageData(Student student)
        {
            string imageFilePath = student.FullPathUrl + "/" + Constants.Locations.ImageFile;
            byte[] bytes = FTP.DownloadFileBytes(imageFilePath);
            student.ImageData = Convert.ToBase64String(bytes);

            //updateCSV
            byte[] fileContents;

            // Encode the encrypted file's data
            fileContents = Encoding.UTF8.GetBytes(student.ToCSV());
            uploadFile(student,Constants.Locations.InfoFile,fileContents);
        }

        private static void uploadFile(Student student,string FileName,byte[] fileContents)
        {
            // Connect to FTP to upload the file and configure request, Also add the file you want to update
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(student.FullPathUrl + "/" + FileName);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(Constants.FTP.UserName, Constants.FTP.Password);

            request.ContentLength = fileContents.Length;
            //Upload the data via FTP
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"FTP Transaction for {FileName} Complete, status {response.StatusDescription}");
            }
        }
    }
}
