using DocumentFormat.OpenXml.Office2013.Excel;
using iText.StyledXmlParser.Jsoup.Select;
using Microsoft.Graph;
using OfficeOpenXml;
using System.Diagnostics;

namespace cert_mailer
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //string evalpath = @"F:\Leadership Skills - End of Course Survey.xlsx";
            //string evalpath = @"F:\BMRA_code\work_project\TestEnv\11023.0001 SBA ICG-EL Virtual - NN\Production\Internal Controls for Grants Course Evaluation - 11023.0001.xlsx";
/*            string evalpath = @"F:\BMRA_code\work_project\TestEnv\SF Eval Example 2.xlsx";
            string EOCpath = @"F:\BMRA_code\work_project\TestEnv\test";
            string type = "SalesForce";
            DateTime startDate = new DateTime(2023, 6, 20);
            DateTime endDate = new DateTime(2023, 6, 23);
            FileInfo evalInfo = new FileInfo(evalpath);
            var attendance = "10";
            var courseABV = "TOM";
            Evaluations eval = new Evaluations(evalInfo, EOCpath, type, "11023.0001", startDate, endDate, "J. Beverage", "CDC", "FPM 334", attendance, courseABV);*/

/*            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            string rosterPath = @"F:\tests\60023.0003 USDA MDCP-EL Virtual - ZU\60023.0003 USDA MDCP-EL Virtual - ZU\Attendance Roster - 60023.0003.xlsx";
            //string rosterPath = @"F:\work_project\TestEnv\11023.0001 SBA ICG-EL Virtual - NN\Attendance Roster - 11023.0001.xlsx";
            FileInfo rosterInfo = new FileInfo(rosterPath);
            string gradesPath = @"F:\tests\60023.0003 USDA MDCP-EL Virtual - ZU\60023.0003 USDA MDCP-EL Virtual - ZU\BMRA Roster and Grades - 60023.0003.xlsx";
            //string gradesPath = @"F:\work_project\TestEnv\11023.0001 SBA ICG-EL Virtual - NN\BMRA Roster and Grades - 11023.0001.xlsx";
            FileInfo gradesInfo = new FileInfo(gradesPath);
            //string certPath = @"F:\tests\96523.0024 VAAA PM FPM 334-VA Virtual - JB\96523.0024 VAAA PM FPM 334-VA Virtual - JB\EOC\Certs";
            //string certPath = @"F:\work_project\TestEnv\11023.0001 SBA ICG-EL Virtual - NN\EOC\Certs";
            string certPath = @"F:\tests\";
            var certType = EnumCertificateType.CertificateType.NOAA;
            DataRead test = new DataRead(rosterInfo, gradesInfo, certPath, true, certType, 80);


            string courseName = test.Course.CourseName;
            string courseId = test.Course.CourseId;

            foreach (Student student in test.Course.Students)
            {
                EmailBuilder message = new EmailBuilder(student.Email, courseName, courseId, student.Certification);
                message.CreateDraft();
            }

            Console.WriteLine("Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds);
            stopwatch.Stop();*/

        }
    }
}
