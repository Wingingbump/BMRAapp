using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml;

namespace cert_mailer;

public class DataRead
{
    private readonly Dictionary<int, string> rosterType = new()
    {
        {1, "BMRA"},
        {2, "VA PM"},
        {3, "DISA"},
        {4, "VA CPS"}
    };
    public const int GRADESPACE = 1;
    const int BMRAROSTERSPACE = 11;
    private readonly Dictionary<string, string> certMap = new();
    public string certPath;
    private readonly int minimumPassingGrade;
    public Course Course { get; }

    public DataRead(FileInfo roster, FileInfo grades, string certPath, bool createCerts, EnumCertificateType.CertificateType certificateType, int minimumPassingGrade)
    {
        // Set CertPath
        this.certPath = certPath;

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var rosterExcel = new ExcelPackage(roster);
        // Get the Worksheet for the Roster
        // If it's a BMRA roster
        ExcelWorksheet? rosterSheet = null;
        var rosterType = IsBMRARoster(rosterExcel);
        if (rosterType == 1 || rosterType == 3 || rosterType == 4)
        {
            rosterSheet = rosterExcel.Workbook.Worksheets.FirstOrDefault(sheet => sheet.Name.Contains("EOC"));
            // Ensure that there's an EOC page
            if (rosterSheet == null)
            {
                throw new Exception("The worksheet with name containing 'EOC' was not found.");
            }
        }
        // VA
        else if (rosterType == 2)
        {
            foreach (ExcelWorksheet worksheet in rosterExcel.Workbook.Worksheets)
            {
                if (worksheet.Hidden == eWorkSheetHidden.Visible)
                {
                    rosterSheet = worksheet;
                }
            }
        }
        else
        {
            throw new Exception("Roster Type Unsupported");
        }

        if (rosterSheet == null)
        {
            throw new Exception("The worksheet was not found.");
        }

        // min passing grade
        this.minimumPassingGrade = minimumPassingGrade;

        // Get the Worksheet for the Grades
        using var gradesExcel = new ExcelPackage(grades);
        using var gradesSheet = gradesExcel.Workbook.Worksheets[0];

        // Set and create Course
        var fileName = grades.Name;
        Course = CourseReader(gradesSheet, fileName);

        // Add students to course
        StudentReader(rosterSheet, gradesSheet, rosterType, createCerts);

        // If the certs aren't created create them
        if (createCerts == true)
        {
            CertificateCreator creator = new CertificateCreator(gradesSheet, certPath, Course, certificateType);
            // Add the cert to each student
            var numStudents = Course.Students.Count;
            for (var i = 0; i < numStudents; i++)
            {
                var firstName = Course.Students[i].FirstName;
                var lastName = Course.Students[i].LastName;
                var fullName = $"{firstName} {lastName}";
                var cert = MatchCert(fullName);
                Course.Students[i].Certification = cert;
            }
        }
    }

    public void StudentReader(ExcelWorksheet rosterSheet, ExcelWorksheet gradesSheet, int rosterType, bool createCerts)
    {
        // Compare the data in each row
        var skip = 0; // If we need to skip when using the VA roster
        var gradeSkip = 0; // If we need to skip in the grade sheet
        var rowCount = Math.Max(rosterSheet.Dimension.End.Row, gradesSheet.Dimension.End.Row);
        for (var row = 1; row <= rowCount; row++)
        {
            // Scan grade sheet
            var gradeSpacing = row + GRADESPACE;
            var gradesGrade = gradesSheet.Cells[gradeSpacing + gradeSkip, 6].Value?.ToString();
            if (certPath.Equals("SD") && gradesGrade != null) {
                gradesGrade = gradesSheet.Cells[gradeSpacing + gradeSkip, 6].Value?.ToString();
            }
            else if (gradesGrade != null && !GradeCheck(gradesGrade))
            {
                gradeSkip++;
                gradesGrade = gradesSheet.Cells[gradeSpacing + gradeSkip, 6].Value?.ToString();
            }

            var gradesFirstName = gradesSheet.Cells[gradeSpacing + gradeSkip, 3].Value?.ToString();
            var gradesLastName = gradesSheet.Cells[gradeSpacing + gradeSkip, 5].Value?.ToString();

            // Declare roster varibles
            var rosterFirstName = "";
            var rosterLastName = "";
            var rosterEmail = "";

            // BMRA roster
            if (rosterType == 1)
            {
                var rosterSpacing = row + BMRAROSTERSPACE;
                rosterFirstName = rosterSheet.Cells[rosterSpacing, 2].Value?.ToString();
                rosterLastName = rosterSheet.Cells[rosterSpacing, 3].Value?.ToString();
                rosterEmail = rosterSheet.Cells[rosterSpacing, 4].Value?.ToString();
            }
            // VA roster
            else if (rosterType == 2)
            {
                var rosterSpacing = row + GRADESPACE; // Use GRADESPACE since it's only skipping the header
                var rosterPass = rosterSheet.Cells[rosterSpacing + skip, 7].Value?.ToString();
                while (rosterPass == "N")
                {
                    skip++;
                    rosterPass = rosterSheet.Cells[rosterSpacing + skip, 7].Value?.ToString();
                }
                rosterFirstName = rosterSheet.Cells[rosterSpacing + skip, 2].Value?.ToString();
                rosterLastName = rosterSheet.Cells[rosterSpacing + skip, 1].Value?.ToString();
                rosterEmail = rosterSheet.Cells[rosterSpacing + skip, 3].Value?.ToString();
            }
            else if (rosterType == 3)
            {
                var rosterSpacing = row + BMRAROSTERSPACE + 2;
                rosterFirstName = rosterSheet.Cells[rosterSpacing, 2].Value?.ToString();
                rosterLastName = rosterSheet.Cells[rosterSpacing, 3].Value?.ToString();
                rosterEmail = rosterSheet.Cells[rosterSpacing, 4].Value?.ToString();
            }
            if (rosterType == 4)
            {
                var rosterSpacing = row + GRADESPACE + 1; // Use GRADESPACE + 1 since it's only skipping the header
                rosterFirstName = rosterSheet.Cells[rosterSpacing + skip, 3].Value?.ToString();
                while (rosterFirstName != gradesFirstName)
                {
                    skip++;
                    rosterFirstName = rosterSheet.Cells[rosterSpacing + skip, 3].Value?.ToString();
                }
                rosterLastName = rosterSheet.Cells[rosterSpacing + skip, 2].Value?.ToString();
                rosterEmail = rosterSheet.Cells[rosterSpacing + skip, 4].Value?.ToString();
            }


            var firstName = rosterFirstName ?? "";
            var lastName = rosterLastName ?? "";
            var email = rosterEmail ?? "";
            var grade = gradesGrade ?? "";

            // Case where certs need to be created
            var cert = "No Certificate Found";
            // If Certs are already created
            if (createCerts == false && rosterFirstName != null && certPath != "SD")
            {
                var fullName = $"{firstName} {lastName}";
                cert = MatchCert(fullName);
            }

            if (rosterFirstName == gradesFirstName && rosterLastName == gradesLastName && rosterFirstName != null)
            {
                Student student = new Student(firstName, lastName, email, cert, GradeCheck(grade), grade);
                Course.AddStudent(student);
            }
        }
    }
    private bool GradeCheck(string grade)
    {
        if (grade.ToLower() == "pass")
        {
            return true;
        }
        double result;
        var isDouble = double.TryParse(grade, out result);
        if (isDouble && result >= (double)minimumPassingGrade/100)
        {
            return true;
        }
        return false;
    }

    public Course CourseReader(ExcelWorksheet gradesSheet, string fileName)
    {
        // Varibles for data gathered from the course sheet
        var instructor = ""; // instructor not used

        var courseName = gradesSheet.Cells[2, 7].Value?.ToString();
        var courseId = gradesSheet.Cells[2, 1].Value?.ToString();
        var agency = gradesSheet.Cells[2, 10].Value?.ToString();
        var location = gradesSheet.Cells[2, 11].Value?.ToString();
        var startDateString = gradesSheet.Cells[2, 8].Value?.ToString();
        var endDateString = gradesSheet.Cells[2, 9].Value?.ToString();
        DateTime startDate = StringDateTime(startDateString ?? "");
        DateTime endDate = StringDateTime(endDateString ?? "");

        // Get a possible courseId override
        var courseNamingScheme = fileName;
        courseNamingScheme = courseNamingScheme.Replace("BMRA Roster and Grades - ", "");
        courseNamingScheme = courseNamingScheme.Replace(".xlsx", "");

        if (courseId != courseNamingScheme)
        {
            courseId = courseNamingScheme;
        }

        return new Course(courseName ?? "", courseNamingScheme ?? "", courseId ?? "", agency ?? "", instructor ?? "", location ?? "", startDate, endDate);
    }


    private static DateTime StringDateTime(string date)
    {
        return DateTime.Parse(date);
    }

    private static string ColonSplit(string input)
    {
        var parts = input.Split(": ");
        _ = parts[0];
        var b = parts[1];
        return b;
    }

    private void BuildCertMap()
    {
        if (certPath == "SD") 
        {
            return;
        }
        // Get all the PDF files in the specified directory
        var files = System.IO.Directory.GetFiles(certPath, "*.pdf");

        // Loop through each file
        foreach (var file in files)
        {
            // Get the file name without the extension and split it by the delimiter
            var fileName = Path.GetFileNameWithoutExtension(file);
            var nameParts = fileName.Split(" - ");

            // Make sure the file name has at least two parts (full name and certificate type)
            if (nameParts.Length >= 2)
            {
                // Get the full name from the file name and add it to the certMap if it's not already there
                var fullName = nameParts[1];
                if (!certMap.ContainsKey(fullName))
                {
                    certMap.Add(fullName, file);
                }
            }
        }
    }

    private string MatchCert(string searchString)
    {
        if (certMap.Count == 0)
        {
            BuildCertMap();
        }

        try
        {
            return certMap[searchString];
        }
        catch (KeyNotFoundException)
        {
            return "Could not find certificate for" + searchString;
        }
    }

    private static int IsBMRARoster(ExcelPackage rosterExcel)
    {
        var worksheet = rosterExcel.Workbook.Worksheets[0]; // get the first worksheet
        var cell = worksheet.Cells["A1"]; // get the cell A1
        var value = cell.Value; // get the value of the cell A1
        var a1Data = value?.ToString() ?? "null";

        // Hardcoded the default Roster
        if (a1Data != null && rosterExcel != null)
        {
            // disa/bmra
            if (a1Data.Contains("BUSINESS MANAGEMENT RESEARCH ASSOCIATES, INC"))
            {
                switch (worksheet.Cells["E11"].Value)
                {
                    case "Lunch: ":
                        return 3;
                    default:
                        return 1;
                }
            }
            // pm
            if (a1Data.Contains("Last Name"))
            {
                return 2;
            }
            //cps
            if (a1Data.Contains("null"))
            {
                return 4;
            }
        }
        return 0;
    }

}