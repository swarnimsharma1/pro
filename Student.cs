using System;
using System.Collections.Generic;
using System.Text;

namespace FTPDataQuery.Models
{
    class Student
    {
        public static string HeaderRow = $"{nameof(Student.StudentID)},{nameof(Student.FirstName)},{nameof(Student.LastName)},{nameof(Student.DateOfBirth)},{nameof(Student.ImageData)}";
        public string StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool MyRecord { get; set; }
        public string Directory { get; set; }
        public string AbsoluteUrl { get; set; }
        public string FullPathUrl
        {
            get => AbsoluteUrl + "/" + Directory;
        }
        private string _DateOfBirth;
        public string DateOfBirth
        {
            get { return _DateOfBirth; }
            set
            {
                _DateOfBirth = value;
                //Convert DateOfBirth to DateTime
                DateTime dtOut;
                DateTime.TryParse(value, out dtOut);
                DateOfBirthDT = dtOut;
                calculateAge();
            }
        }
        public DateTime DateOfBirthDT { get; internal set; }
        public int Age { get; internal set; }
        public string ImageData { get; set; }

        public List<string> Exceptions { get; set; } = new List<string>();

        public Student()
        {

        }
        public Student(string csvData)
        {
            FromCSV(csvData);
        }

        private void calculateAge()
        {
            Age = DateTime.Now.Year - DateOfBirthDT.Year;
        }


        public void FromDirectory(string directory)
        {
            Directory = directory;

            if (String.IsNullOrEmpty(directory.Trim()))
            {
                return;
            }

            string[] data = directory.Trim().Split(' ');

            StudentID = data[0];
            FirstName = data[1];
            LastName = data[2];
        }

        public void FromCSV(string csvdata)
        {
            string[] data = csvdata.Split(',', StringSplitOptions.RemoveEmptyEntries);

            try
            {
                StudentID = data[0];
                FirstName = data[1];
                LastName = data[2];
                DateOfBirth = data[3];
                if(data.Length > 4)
                {
                    ImageData = data[4];
                }
                else
                {
                    ImageData = "";
                }
                
            }
            catch (Exception e)
            {
                Exceptions.Add(e.Message);
            }
        }

        public string ToCSV()
        {
            string csv_data = $"{StudentID},{FirstName},{LastName},{DateOfBirthDT.ToShortDateString()},{ImageData}";
            return csv_data;
        }

        public override string ToString()
        {
            //return ToCSV();
            return $"{StudentID} {FirstName} {LastName}";
        }

        public string ToJSON()
        {
            StringBuilder json_data = new StringBuilder("{");
            json_data.Append($"\"StudentID\":\"{StudentID}\",");
            json_data.Append($"\"FirstName\":\"{FirstName}\",");
            json_data.Append($"\"LastName\":\"{LastName}\",");
            json_data.Append($"\"DateOfBirth\":\"{DateOfBirth}\",");
            json_data.Append($"\"ImageData\":\"{ImageData}\"");
            json_data.Append("}");

            return json_data.ToString();
        }

        public string ToXML()
        {
            StringBuilder xml_data = new StringBuilder("<Student>");
            xml_data.Append($"<StudentID>{StudentID}</StudentID>");
            xml_data.Append($"<FirstName>{FirstName}</FirstName>");
            xml_data.Append($"<LastName>{LastName}</LastName>");
            xml_data.Append($"<DateOfBirth>{DateOfBirth}</DateOfBirth>");
            xml_data.Append($"<ImageData>{ImageData}</ImageData>");
            xml_data.Append("</Student>");
            return xml_data.ToString();
        }
    }
}
