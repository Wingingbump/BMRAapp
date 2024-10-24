using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Layout.Properties;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using Windows.Graphics.Printing3D;
using static System.Runtime.InteropServices.JavaScript.JSType;
using BottomBorder = DocumentFormat.OpenXml.Wordprocessing.BottomBorder;
using InsideHorizontalBorder = DocumentFormat.OpenXml.Wordprocessing.InsideHorizontalBorder;
using InsideVerticalBorder = DocumentFormat.OpenXml.Wordprocessing.InsideVerticalBorder;
using LeftBorder = DocumentFormat.OpenXml.Wordprocessing.LeftBorder;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using RightBorder = DocumentFormat.OpenXml.Wordprocessing.RightBorder;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using TopBorder = DocumentFormat.OpenXml.Wordprocessing.TopBorder;

namespace cert_mailer;
public class Evaluations
{
    private string path
    {
        get; set;
    }
    private string courseCode
    {
        get; set;
    }
    private string type
    {
        get; set;
    }
    private DateTime startDate
    {
        get; set;
    }
    private DateTime endDate
    {
        get; set;
    }
    private string instructor
    {
        get; set;
    }
    private string agency
    {
        get; set;
    }
    private string courseName
    {
        get; set;
    }
    private string attendance
    {
        get; set;
    }
    private string courseABV
    {
        get; set;
    }


    // Create a HashSet to store the responses to omit with case-insensitive matching
    readonly HashSet<string> omittedResponses = new(StringComparer.OrdinalIgnoreCase)
    {
        "None", "NA", "N/A", "No", "No Response", "Not Applicable", "Not at this time",
        "Not Relevant", "No Answer", "Nil", "None Provided", "Unavailable",
        "No Comment", "Not Provided", "Decline to Answer", "Declined", "No Input"
    };


    public Evaluations(FileInfo evaluation, string EOCpath, string type, string courseCode, DateTime startDate, DateTime endDate, string instructor, string agency, string courseName, string attendance, string courseABV)
    {
        // Load the evaluation file and get the first worksheet.
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var evaluationExcel = new ExcelPackage(evaluation);
        var evalSheet = evaluationExcel.Workbook.Worksheets.FirstOrDefault();
        path = EOCpath;
        this.courseCode = courseCode;
        this.type = type;
        this.startDate = startDate;
        this.endDate = endDate;
        this.instructor = instructor;
        this.agency = agency;
        this.courseName = courseName;
        this.attendance = attendance;
        this.courseABV = courseABV;
        // If the worksheet exists, read the evaluations.
        if (evalSheet != null)
        {
            if (type.Equals("SalesForce"))
            {
                SFevalReader(evaluationExcel);
            }
            else
            {
                evalReader(evalSheet);
            }
        }

    }

    // I suck at coding lmao this should return a struct with both dicts then pass it to the next function but i'm lazy
    public void evalReader(ExcelWorksheet evalSheet)
    {
        int rowCount = evalSheet.Dimension.Rows;

        Dictionary<string, int[]> questionRatingCounts = new Dictionary<string, int[]>();
        Dictionary<string, string[]> comments = new Dictionary<string, string[]>();

        // Initialize rating counts for each question
        if (type.Equals("DISA")) {
            questionRatingCounts.Add("Question1", new int[2]); // Index 0 for "No", Index 1 for "Yes"
            questionRatingCounts.Add("Question2", new int[2]); // Index 0 for "No", Index 1 for "Yes"
            questionRatingCounts.Add("Question3", new int[10]); // Index 0 for "Poor", Index 4 for "Excellent"
            questionRatingCounts.Add("Question4", new int[10]);
            questionRatingCounts.Add("Question5", new int[10]);
            questionRatingCounts.Add("Question7", new int[2]); // Index 0 for "Not Recommend", Index 1 for "Recommend"
            questionRatingCounts.Add("Question8", new int[10]);
            questionRatingCounts.Add("Question9", new int[10]);
            questionRatingCounts.Add("Question10", new int[10]);
            questionRatingCounts.Add("Question11", new int[10]);
            questionRatingCounts.Add("Question14", new int[10]);
            questionRatingCounts.Add("Question15", new int[10]);
            questionRatingCounts.Add("Question16", new int[10]);
        }
        else {
            questionRatingCounts.Add("Question1", new int[2]); // Index 0 for "No", Index 1 for "Yes"
            questionRatingCounts.Add("Question2", new int[2]); // Index 0 for "No", Index 1 for "Yes"
            questionRatingCounts.Add("Question3", new int[5]); // Index 0 for "Poor", Index 4 for "Excellent"
            questionRatingCounts.Add("Question4", new int[5]);
            questionRatingCounts.Add("Question5", new int[5]);
            questionRatingCounts.Add("Question7", new int[2]); // Index 0 for "Not Recommend", Index 1 for "Recommend"
            questionRatingCounts.Add("Question8", new int[5]);
            questionRatingCounts.Add("Question9", new int[5]);
            questionRatingCounts.Add("Question10", new int[5]);
            questionRatingCounts.Add("Question11", new int[5]);
            questionRatingCounts.Add("Question14", new int[5]);
            questionRatingCounts.Add("Question15", new int[5]);
            questionRatingCounts.Add("Question16", new int[5]); // 13 questions
        }

        // Initialize comment count
        int commentCount = 1;
        // Defualt buffer for non-LMS evaluations
        int buffer = 0;
        int buffer2 = 0;
        if (type.Equals("Default"))
        {
            buffer = 4;
            buffer2 = 6;
        }
        if (type.Equals("DISA"))
        {
            buffer = 5;
            buffer2 = 5;
        }

        for (int row = 2; row <= rowCount; row++)
        {
            // Yes/No questions
            // Count the "Yes" and "No" responses separately
            string? question1Response = evalSheet.Cells[row, 2 + buffer].Value.ToString();
            string? question2Response = evalSheet.Cells[row, 3 + buffer].Value.ToString();

            UpdateYesNoCount(questionRatingCounts["Question1"], question1Response);
            UpdateYesNoCount(questionRatingCounts["Question2"], question2Response);

            // Stars
            // Convert the stars to ints.
            int question3 = ConvertStarsToInt(evalSheet.Cells[row, 4 + buffer].Value);
            int question4 = ConvertStarsToInt(evalSheet.Cells[row, 5 + buffer].Value);
            int question5 = ConvertStarsToInt(evalSheet.Cells[row, 6 + buffer].Value);
            int question14 = ConvertStarsToInt(evalSheet.Cells[row, 15 + buffer].Value);
            int question15 = ConvertStarsToInt(evalSheet.Cells[row, 16 + buffer].Value);
            int question16 = ConvertStarsToInt(evalSheet.Cells[row, 17 + buffer].Value);

            // Strings
            // Convert the strings to ints.
            int question8 = ConvertStringToInt(evalSheet.Cells[row, 9 + buffer].Value.ToString());
            int question9 = ConvertStringToInt(evalSheet.Cells[row, 10 + buffer].Value.ToString());
            int question10 = ConvertStringToInt(evalSheet.Cells[row, 11 + buffer].Value.ToString());
            int question11 = ConvertStringToInt(evalSheet.Cells[row, 12 + buffer].Value.ToString());

            // Question 7 Recommend Count
            string? question7Response = evalSheet.Cells[row, 8 + buffer].Value.ToString();
            int question7Rating = ExtractRatingFromFormat(question7Response);
            UpdateRecommendCount(questionRatingCounts["Question7"], question7Rating);

            // Update rating counts for each question
            UpdateRatingCount(questionRatingCounts["Question3"], question3);
            UpdateRatingCount(questionRatingCounts["Question4"], question4);
            UpdateRatingCount(questionRatingCounts["Question5"], question5);
            UpdateRatingCount(questionRatingCounts["Question8"], question8);
            UpdateRatingCount(questionRatingCounts["Question9"], question9);
            UpdateRatingCount(questionRatingCounts["Question10"], question10);
            UpdateRatingCount(questionRatingCounts["Question11"], question11);
            UpdateRatingCount(questionRatingCounts["Question14"], question14);
            UpdateRatingCount(questionRatingCounts["Question15"], question15);
            UpdateRatingCount(questionRatingCounts["Question16"], question16);

            // Comments
            string[] userResponse = new string[4];
            userResponse[0] = evalSheet.Cells[row, 7 + buffer].Value?.ToString() ?? "";
            userResponse[1] = evalSheet.Cells[row, 13 + buffer].Value?.ToString() ?? "";
            userResponse[2] = evalSheet.Cells[row, 18 + buffer].Value?.ToString() ?? "";
            userResponse[3] = evalSheet.Cells[row, 19 + buffer2].Value?.ToString() ?? ""; // Additional buffer for Default evaluations

            // Update the omitted responses to blanks
            for (int i = 0; i < userResponse.Length; i++)
            {
                if (omittedResponses.Contains(userResponse[i]))
                {
                    userResponse[i] = ""; // Update omitted responses to blank
                }
            }

            // Check if all the comments are blank
            bool allBlank = userResponse.All(string.IsNullOrEmpty);

            // If the comments are not all blank, add them to the dictionary
            if (!allBlank)
            {
                comments.Add("Comment" + commentCount, userResponse);
                commentCount++;
            }

        }
        CreateEvaluationTemplate(questionRatingCounts, comments);
    }

    // Create Excel file
    public void CreateEvaluationTemplate(Dictionary<string, int[]> scores, Dictionary<string, string[]> comments)
    {
        var DISABuffer = 12;
        // Combine path with file name
        string output = System.IO.Path.Combine(path, "Course Evaluation Summary - " + courseCode + ".xlsx");
        // Get template file path
        string templatePath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath + "\\Assets", "Course Evaluation Summary - Template.xlsx");
        if (type.Equals("DISA")) {
            templatePath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath + "\\Assets", "Course Evaluation Summary - DISA Template.xlsx");
            DISABuffer = 17;
        }
        // Copy the template file to the output path
        System.IO.File.Copy(templatePath, output, true);
        // Open the copied file for editing
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var outputPath = new ExcelPackage(new FileInfo(output));

        // Get the first sheet
        ExcelWorksheet outputSheet = outputPath.Workbook.Worksheets.FirstOrDefault();
        int[] questionOrder = new int[] { 11, 8, 10, 9, 3, 14, 15, 16, 4, 5 };
        int skip = 0;
        for (int items = 6; items <= 17; items++)
        {
            if (items == 11 || items == 16)
            {
                skip++;
                items++;
            }
            for (int col = 8; col <= DISABuffer; col++)
            {
                int reversedIndex = DISABuffer - col; // Calculate the reversed index
                if (scores["Question" + questionOrder[items - 6 - skip]][reversedIndex] == 0)
                {
                    outputSheet.Cells[items, col].Value = "";
                }
                else
                {
                    outputSheet.Cells[items, col].Value = scores["Question" + questionOrder[items - 6 - skip]][reversedIndex];
                }
            }
        }

        // Yes/No questions
        int[] yesNo = new int[] { 9, 11 };
        if (type.Equals("DISA"))
        {
            yesNo = new int[] { 12, 14 };
        }
        int[] yesNoQuestions = new int[] { 2, 3, 19 };

        for (int rowIndex = 0; rowIndex < yesNoQuestions.Length; rowIndex++)
        {
            for (int colIndex = 0; colIndex < yesNo.Length; colIndex++)
            {
                int reversedColIndex = yesNo.Length - 1 - colIndex; // Calculate the reversed column index
                if (rowIndex < 2)
                {
                    if (scores["Question" + (rowIndex + 1)][reversedColIndex] == 0)
                    {
                        outputSheet.Cells[yesNoQuestions[rowIndex], yesNo[colIndex]].Value = "";
                    }
                    else
                    {
                        outputSheet.Cells[yesNoQuestions[rowIndex], yesNo[colIndex]].Value = scores["Question" + (rowIndex + 1)][reversedColIndex];
                    }
                }
                else
                {
                    if (scores["Question" + (7)][reversedColIndex] == 0) {
                        outputSheet.Cells[yesNoQuestions[rowIndex], yesNo[colIndex]].Value = "";
                    }
                    else
                    {
                        outputSheet.Cells[yesNoQuestions[rowIndex], yesNo[colIndex]].Value = scores["Question" + (7)][reversedColIndex];
                    }
                }
            }
        }

        // Fill out the response amount
        if (type.Equals("DISA"))
        {
            outputSheet.Cells[21, 17].Value = attendance;
            outputSheet.Cells[22, 17].Value = scores["Question1"].Sum();
            outputSheet.Cells[23, 17].Value = comments.Count;
        }
        else
        {
            outputSheet.Cells[21, 13].Value = attendance;
            outputSheet.Cells[22, 13].Value = scores["Question1"].Sum();
            outputSheet.Cells[23, 13].Value = comments.Count;
        }

        // Header for Evaluation Summary
        string dateRange;
        if (startDate.Date == endDate.Date)
        {
            // Same day
            dateRange = startDate.ToString("MMM. d, yyyy");
        }
        else if (startDate.Month == endDate.Month && startDate.Year == endDate.Year)
        {
            // Same month and year
            dateRange = startDate.ToString("MMM. d") + " - " + endDate.ToString("d, yyyy");
        }
        else if (startDate.Year == endDate.Year)
        {
            // Different months, same year
            dateRange = startDate.ToString("MMM. d") + " - " + endDate.ToString("MMM. d, yyyy");
        }
        else
        {
            // Different months and years
            dateRange = startDate.ToString("MMM. d, yyyy") + " - " + endDate.ToString("MMM. d, yyyy");
        }

        // Replace "Sep." with "Sept."
        dateRange = dateRange.Replace("Sep.", "Sept.");

        // Replace "May." with "May"
        dateRange = dateRange.Replace("May.", "May");

        outputSheet.HeaderFooter.OddHeader.RightAlignedText = $"BMRA Ref: {courseCode} \r\n DATE: {dateRange}";

        // Footer for Evaluation Summary
        outputSheet.HeaderFooter.OddFooter.LeftAlignedText = $"Name of course: {courseABV}";
        outputSheet.HeaderFooter.OddFooter.CenteredText = $"{agency} - Virtual";
        outputSheet.HeaderFooter.OddFooter.RightAlignedText = $"Instructor: {instructor}";

        // Update basic info on the calcs sheet
        outputSheet = outputPath.Workbook.Worksheets["calcs"];

        outputSheet.Cells["A3"].Value = courseCode; // Course ID

        outputSheet.Cells["C3"].Value = courseABV + " " + agency; // Course Name + Agency

        outputSheet.Cells["D3"].Value = dateRange; // Date

        outputSheet.Cells["A9"].Value = courseCode; // Course ID

        outputSheet.Cells["C9"].Value = courseABV + " " + agency; // Course Name + Agency

        outputSheet.Cells["D9"].Value = dateRange; // Date


        // Save the file
        outputPath.Save();

        // Open Comment template
        string commentTemplatePath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath + "\\Assets", "Student Comments - Template.docx");
        // Get output path for comment
        string commentOutput = System.IO.Path.Combine(path, "Student Comments - " + courseCode + ".docx");
        // Copy the template file to the output path
        System.IO.File.Copy(commentTemplatePath, commentOutput, true);
        // Open the copied file for editing
        using var commentOutputPath = WordprocessingDocument.Open(commentOutput, true);

        // Retrieve the main document part
        var commentMainPart = commentOutputPath.MainDocumentPart;
        // Create a new table
        var table = new Table();

        // Create table borders
        var tableBorders = new TableBorders(
            new TopBorder() { Val = BorderValues.Single, Size = 6, Color = "000000" }, // Top border
            new BottomBorder() { Val = BorderValues.Single, Size = 6, Color = "000000" }, // Bottom border
            new LeftBorder() { Val = BorderValues.Single, Size = 6, Color = "000000" }, // Left border
            new RightBorder() { Val = BorderValues.Single, Size = 6, Color = "000000" }, // Right border
            new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 6, Color = "000000" }, // Inside horizontal border
            new InsideVerticalBorder() { Val = BorderValues.Single, Size = 6, Color = "000000" } // Inside vertical border
        );
        // Apply table borders to table
        table.AppendChild(tableBorders);

        foreach (var comment in comments)
        {
            // Create new row
            var tr = new TableRow();

            // Edit the content of the new cell 
            var tc = new TableCell();

            // Create a list to store the generated paragraphs
            var paragraphs = new List<Paragraph>();

            // Generate the paragraphs for each question and comment
            string[] questionList = new string[] { "Do you have any feedback about the virtual platform?", "Any comments about the course materials, presentation, or exercises?", "Do you have any comments about the Instructor?", "Anything else you'd like to tell us?" };
            for (var question = 0; question < 4; question++)
            {
                if (!comment.Value[question].Equals(""))
                {
                    paragraphs.Add(CreateParagraphWithBullet(questionList[question], comment.Value[question]));
                }
            }

            // Add the paragraphs to the cell
            tc.Append(paragraphs);
            // Add the cell to the row
            tr.Append(tc);
            // Add the row to the table
            table.Append(tr);
        }

        // Append the table to the document
        commentMainPart.Document.Body.Append(table);

        // Add the End of student comments bold text
        commentMainPart.Document.Body.Append(new Paragraph(
            new Run(new Text("End of student comments."))
            {
                RunProperties = new RunProperties(new Bold())
            }
        ));

        // Modify the document margins to remove the space at the top
        var sectionProperties = commentMainPart.Document.Body.Elements<SectionProperties>().FirstOrDefault();
        if (sectionProperties != null)
        {
            var pageMargin = sectionProperties.Elements<PageMargin>().FirstOrDefault();
            if (pageMargin != null)
            {
                pageMargin.Top = 0; // Set the top margin to 0
            }
        }
        var headerPart = commentMainPart.HeaderParts.FirstOrDefault();

        // Check if a header part exists
        if (headerPart != null)
        {
            // Iterate through the paragraphs in the header
            foreach (var paragraph in headerPart.Header.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
            {
                // Iterate through the runs in each paragraph
                foreach (var run in paragraph.Elements<DocumentFormat.OpenXml.Wordprocessing.Run>())
                {
                    // Iterate through the text elements in each run
                    foreach (var textElement in run.Elements<DocumentFormat.OpenXml.Wordprocessing.Text>())
                    {
                        // Replace the specific text with the updated values
                        textElement.Text = textElement.Text.Replace("CODE", courseCode)
                                                           .Replace("INSTRUCTOR", instructor)
                                                           .Replace("COURSE", courseName)
                                                           .Replace("DATE", dateRange);
                    }
                }
            }
        }

        // Save the file
        commentOutputPath.Save();
    }

    public void SFevalReader(ExcelPackage evaluationExcel)
    {
        var currSheet = evaluationExcel.Workbook.Worksheets.First(); // Current sheet
        const int startingCol = 4; // First question

        // Data structure to hold question data

        /**
         * Index 0 for "No", Index 1 for "Yes"
         * Index 0 for "Poor", Index 4 for "Excellent"
         * Index 0 for "Not Recommend", Index 1 for "Recommend"
         * 13 questions
         */

        // Define a dictionary with question names and their respective array sizes
        var questions = new Dictionary<string, int[]>
        {
            { "Question1", new int[2] }, // 1. Did this course meet its stated learning objectives?
            { "Question2", new int[2] }, // 2. Did this course meet your personal reasons or objectives for taking this course?
            { "Question3", new int[5] }, // 3. What would you rate this course overall?
            { "Question4", new int[5] }, // 4. What would you rate this instructor overall?
            { "Question5", new int[5] }, // 5. How would you rate the virtual platform?
            { "Question7", new int[2] }, // 7. How likely are you to recommend a BMRA course to a friend or colleague?
            { "Question8", new int[5] }, // 8. How well structured/organized was the course material?
            { "Question9", new int[5] }, // 9. How engaging was the course overall?
            { "Question10", new int[5] }, // 10. How engaging was the course presentation?
            { "Question11", new int[5] }, // 11. How helpful were the course exercises?
            { "Question14", new int[5] }, // 14. How well did the Instructor demonstrate knowledge of the subject?
            { "Question15", new int[5] }, // 15. How successful was the Instructor in making the virtual course engaging?
            { "Question16", new int[5] }  // 16. How successfully did the Instructor answer questions?
        };

        // ==========================
        // Update the Rating Questions
        // ==========================

        // Get the rating table start
        int ratingRow = SearchRating(currSheet); // rating range (poor...) row 
        int startingRow = ratingRow + 2; // First question 2 rows down

        // Establishes the x range of the 2d question array (Ratings)
        var ratingPointer = currSheet.Cells[ratingRow, 4].Value.ToString(); // row
        var ratingRange = new List<int>();
        var index = 0;
        while (ratingPointer != "Total")
        {
            var ratingNumber = int.Parse(ratingPointer.Split('-')[0]);
            ratingRange.Add(ratingNumber);
            index++;
            ratingPointer = currSheet.Cells[ratingRow, 4 + index].Value.ToString();
        }

        // Establishes the y range of the 2d question array (Questions)
        var ratingQuestionRows = 10 + startingRow; // all rating questions
        var ratingCountRange = ratingRange.Count(); // Numeric Range Max
        int[] skipQuestions = {6, 7, 12, 13};
        var questionBuffer = 3;
        for (var row = startingRow ; row < ratingQuestionRows; row++)
        {
            while (skipQuestions.Contains(questionBuffer))
            {
                questionBuffer++;
            }
            for (var col = startingCol; col < ratingCountRange + startingCol; col++)
            {
                var questionNumber = "Question" + (questionBuffer);
                int intRating = currSheet.Cells[row, col].Value is double doubleValue ? (int)doubleValue : int.Parse(currSheet.Cells[row, col].Value.ToString());
                questions[questionNumber][ratingRange[col - startingCol]-1] = intRating;
            }
            questionBuffer++;
        }

        // ==========================
        // Update the Yes/No Questions
        // ==========================
        currSheet = evaluationExcel.Workbook.Worksheets[1]; // Yes/No

        // Get the y/n table start
        ratingRow = SearchRating(currSheet); // y/n row 
        startingRow = ratingRow + 2; // First question 2 rows down

        // Establishes the x range of the 2d question array (Ratings)
        ratingPointer = currSheet.Cells[ratingRow, 4].Value.ToString(); // row
        ratingRange = new List<int>();
        index = 0;
        while (ratingPointer != "Total")
        {
            var ratingNumber = ratingPointer == "NO" ? 0 : 1;
            ratingRange.Add(ratingNumber);
            index++;
            ratingPointer = currSheet.Cells[ratingRow, 4 + index].Value.ToString();
        }

        // Establishes the y range of the 2d question array (Questions)
        ratingQuestionRows = 2 + startingRow; // all rating questions
        ratingCountRange = ratingRange.Count(); // Numeric Range Max
        questionBuffer = 1;
        for (var row = startingRow; row < ratingQuestionRows; row++)
        {
            for (var col = startingCol; col < ratingCountRange + startingCol; col++)
            {
                var questionNumber = "Question" + (questionBuffer);
                int intRating = currSheet.Cells[row, col].Value is double doubleValue ? (int)doubleValue : int.Parse(currSheet.Cells[row, col].Value.ToString());
                questions[questionNumber][ratingRange[col - startingCol]] = intRating;
            }
            questionBuffer++;
        }

        // ==========================
        // Update the NPS Questions
        // ==========================
        currSheet = evaluationExcel.Workbook.Worksheets[2];

        // Get the NPS table start
        ratingRow = SearchRating(currSheet); // NPS row 
        startingRow = ratingRow + 2; // First question 2 rows down

        // Establishes the x range of the 2d question array (Ratings)
        ratingPointer = currSheet.Cells[ratingRow, 4].Value.ToString(); // row
        ratingRange = new List<int>();
        index = 0;
        while (ratingPointer != "Total")
        {
            var ratingNumber = ratingPointer == "Not at all Likely" ? 0 : 1;
            ratingRange.Add(ratingNumber);
            index++;
            ratingPointer = currSheet.Cells[ratingRow, 4 + index].Value.ToString();
        }

        // Establishes the y range of the 2d question array (Questions)
        ratingCountRange = ratingRange.Count(); // Numeric Range Max
        for (var col = startingCol; col < ratingCountRange + startingCol; col++)
        {
            int intRating = currSheet.Cells[startingRow, col].Value is double doubleValue ? (int)doubleValue : int.Parse(currSheet.Cells[startingRow, col].Value.ToString());
            questions["Question7"][ratingRange[col - startingCol]] = intRating;
        }

        //PrintQuestionDictionary(questions);

        // ==========================
        // Update the Comments
        // ==========================
        currSheet = evaluationExcel.Workbook.Worksheets[3];

        var comments = new Dictionary<string, List<string>>()
        {
            { "Question1", new List<string>() }, // 1. Do you have any feedback about the virtual platform?
            { "Question2", new List<string>() }, // 2. Any comments about the course materials, presentation, or exercises?
            { "Question3", new List<string>() }, // 3. Do you have any comments about the Instructor?
            { "Question4", new List<string>() } // 4. Anything else you'd like to tell us?
        };

        // Get the comments table start
        ratingRow = SearchComment(currSheet); // NPS row 
        startingRow = ratingRow + 1; // First question 1 row down

        var commentQuestionCol = 3;
        var commentAnswerCol = 5;

        questionBuffer = 0;
        var currRow = startingRow;
        // While Comments are in the comment col
        while (currSheet.Cells[currRow, commentAnswerCol].Value != null)
        {
            // If A new question comes up then switch to next question
            if (currSheet.Cells[currRow, commentQuestionCol].Value != null)
            {
                questionBuffer++;
            }
            var questionNumber = "Question" + (questionBuffer);
            comments[questionNumber].Add(currSheet.Cells[currRow, commentAnswerCol].Value.ToString());
            currRow++;
        }

        // Remove ommited Responses from the dict
        foreach (var key in comments.Keys.ToList()) // Use ToList() to avoid modifying the collection while iterating
        {
            comments[key].RemoveAll(comment => omittedResponses.Contains(comment));
        }

        //PrintSFComments(comments);
        CreateEvaluationTemplateSF(questions, comments);


    }

    public void CreateEvaluationTemplateSF(Dictionary<string, int[]> questions, Dictionary<string, List<string>> comments)
    {
        // ==========================
        // Eval Sheet
        // ==========================

        // This section is same as the non SF verion
        var Buffer = 12;
        // Combine path with file name
        string output = System.IO.Path.Combine(path, "Course Evaluation Summary - " + courseCode + ".xlsx");
        // Get template file path
        string templatePath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath + "\\Assets", "Course Evaluation Summary - Template.xlsx");
        // Copy the template file to the output path
        System.IO.File.Copy(templatePath, output, true);
        // Open the copied file for editing
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var outputPath = new ExcelPackage(new FileInfo(output));

        // Get the first sheet
        ExcelWorksheet outputSheet = outputPath.Workbook.Worksheets.FirstOrDefault();
        int[] questionOrder = new int[] { 11, 8, 10, 9, 3, 14, 15, 16, 4, 5 };
        int skip = 0;
        for (int items = 6; items <= 17; items++)
        {
            if (items == 11 || items == 16)
            {
                skip++;
                items++;
            }
            for (int col = 8; col <= Buffer; col++)
            {
                int reversedIndex = Buffer - col; // Calculate the reversed index
                if (questions["Question" + questionOrder[items - 6 - skip]][reversedIndex] == 0)
                {
                    outputSheet.Cells[items, col].Value = "";
                }
                else
                {
                    outputSheet.Cells[items, col].Value = questions["Question" + questionOrder[items - 6 - skip]][reversedIndex];
                }
            }
        }

        // Yes/No questions
        int[] yesNo = new int[] { 9, 11 };
        int[] yesNoQuestions = new int[] { 2, 3, 19 };

        for (int rowIndex = 0; rowIndex < yesNoQuestions.Length; rowIndex++)
        {
            for (int colIndex = 0; colIndex < yesNo.Length; colIndex++)
            {
                int reversedColIndex = yesNo.Length - 1 - colIndex; // Calculate the reversed column index
                if (rowIndex < 2)
                {
                    if (questions["Question" + (rowIndex + 1)][reversedColIndex] == 0)
                    {
                        outputSheet.Cells[yesNoQuestions[rowIndex], yesNo[colIndex]].Value = "";
                    }
                    else
                    {
                        outputSheet.Cells[yesNoQuestions[rowIndex], yesNo[colIndex]].Value = questions["Question" + (rowIndex + 1)][reversedColIndex];
                    }
                }
                else
                {
                    if (questions["Question" + (7)][reversedColIndex] == 0)
                    {
                        outputSheet.Cells[yesNoQuestions[rowIndex], yesNo[colIndex]].Value = "";
                    }
                    else
                    {
                        outputSheet.Cells[yesNoQuestions[rowIndex], yesNo[colIndex]].Value = questions["Question" + (7)][reversedColIndex];
                    }
                }
            }
        }

        // Fill out the response amount
        outputSheet.Cells[21, 13].Value = attendance;
        outputSheet.Cells[22, 13].Value = questions["Question1"].Sum();
        outputSheet.Cells[23, 13].Value = comments.Count;

        // Header for Evaluation Summary
        string dateRange;
        if (startDate.Date == endDate.Date)
        {
            // Same day
            dateRange = startDate.ToString("MMM. d, yyyy");
        }
        else if (startDate.Month == endDate.Month && startDate.Year == endDate.Year)
        {
            // Same month and year
            dateRange = startDate.ToString("MMM. d") + " - " + endDate.ToString("d, yyyy");
        }
        else if (startDate.Year == endDate.Year)
        {
            // Different months, same year
            dateRange = startDate.ToString("MMM. d") + " - " + endDate.ToString("MMM. d, yyyy");
        }
        else
        {
            // Different months and years
            dateRange = startDate.ToString("MMM. d, yyyy") + " - " + endDate.ToString("MMM. d, yyyy");
        }

        // Replace "Sep." with "Sept."
        dateRange = dateRange.Replace("Sep.", "Sept.");

        // Replace "May." with "May"
        dateRange = dateRange.Replace("May.", "May");

        outputSheet.HeaderFooter.OddHeader.RightAlignedText = $"BMRA Ref: {courseCode} \r\n DATE: {dateRange}";

        // Footer for Evaluation Summary
        outputSheet.HeaderFooter.OddFooter.LeftAlignedText = $"Name of course: {courseABV}";
        outputSheet.HeaderFooter.OddFooter.CenteredText = $"{agency} - Virtual";
        outputSheet.HeaderFooter.OddFooter.RightAlignedText = $"Instructor: {instructor}";

        // Update basic info on the calcs sheet
        outputSheet = outputPath.Workbook.Worksheets["calcs"];

        outputSheet.Cells["A3"].Value = courseCode; // Course ID

        outputSheet.Cells["C3"].Value = courseABV + " " + agency; // Course Name + Agency

        outputSheet.Cells["D3"].Value = dateRange; // Date

        outputSheet.Cells["A9"].Value = courseCode; // Course ID

        outputSheet.Cells["C9"].Value = courseABV + " " + agency; // Course Name + Agency

        outputSheet.Cells["D9"].Value = dateRange; // Date


        // Save the file
        outputPath.Save();

        // ==========================
        // Comment Sheet
        // ==========================

        // Open Comment template
        string commentTemplatePath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath + "\\Assets", "Student Comments - Template.docx");
        // Get output path for comment
        string commentOutput = System.IO.Path.Combine(path, "Student Comments - " + courseCode + ".docx");
        // Copy the template file to the output path
        System.IO.File.Copy(commentTemplatePath, commentOutput, true);
        // Open the copied file for editing
        using var commentOutputPath = WordprocessingDocument.Open(commentOutput, true);

        // Retrieve the main document part
        var commentMainPart = commentOutputPath.MainDocumentPart;
        // Create a new table
        var table = new Table();

        // Create table properties
        var tableProperties = new DocumentFormat.OpenXml.Wordprocessing.TableProperties(
            new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct }, // Set table width to 100%
            new TableBorders(
                new TopBorder() { Val = BorderValues.Single, Size = 6, Color = "000000" }, // Top border
                new BottomBorder() { Val = BorderValues.Single, Size = 6, Color = "000000" }, // Bottom border
                new LeftBorder() { Val = BorderValues.Single, Size = 6, Color = "000000" }, // Left border
                new RightBorder() { Val = BorderValues.Single, Size = 6, Color = "000000" }, // Right border
                new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 6, Color = "000000" }, // Inside horizontal border
                new InsideVerticalBorder() { Val = BorderValues.Single, Size = 6, Color = "000000" }  // Inside vertical border
            )
        );

        // Apply the table properties to your table
        table.AppendChild(tableProperties);


        // ==========================
        // Modified section
        // ==========================

        var question = 0;
        string[] questionList = new string[] { "Do you have any feedback about the virtual platform?", "Any comments about the course materials, presentation, or exercises?", "Do you have any comments about the Instructor?", "Anything else you'd like to tell us?" };
        foreach (var comment in comments)
        {
            // Create new row
            var tr = new TableRow();

            // Edit the content of the new cell 
            var tc = new TableCell();

            // Create a list to store the generated paragraphs
            var paragraphs = new List<Paragraph>();

            // Generate the paragraphs for each question and comment
            var commentParagraphs = CreateParagraphsWithBulletSF(questionList[question], comment.Value);

            // Add each generated paragraph to the main paragraphs list
            paragraphs.AddRange(commentParagraphs);


            // Add the paragraphs to the cell
            tc.Append(paragraphs);
            // Add the cell to the row
            tr.Append(tc);
            // Add the row to the table
            table.Append(tr);
            question++;
        }

        // Append the table to the document
        commentMainPart.Document.Body.Append(table);

        // Add the End of student comments bold text
        commentMainPart.Document.Body.Append(new Paragraph(
            new Run(new Text("End of student comments."))
            {
                RunProperties = new RunProperties(new Bold())
            }
        ));

        // Modify the document margins to remove the space at the top
        var sectionProperties = commentMainPart.Document.Body.Elements<SectionProperties>().FirstOrDefault();
        if (sectionProperties != null)
        {
            var pageMargin = sectionProperties.Elements<PageMargin>().FirstOrDefault();
            if (pageMargin != null)
            {
                pageMargin.Top = 0; // Set the top margin to 0
            }
        }
        var headerPart = commentMainPart.HeaderParts.FirstOrDefault();

        // Check if a header part exists
        if (headerPart != null)
        {
            // Iterate through the paragraphs in the header
            foreach (var paragraph in headerPart.Header.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
            {
                // Iterate through the runs in each paragraph
                foreach (var run in paragraph.Elements<DocumentFormat.OpenXml.Wordprocessing.Run>())
                {
                    // Iterate through the text elements in each run
                    foreach (var textElement in run.Elements<DocumentFormat.OpenXml.Wordprocessing.Text>())
                    {
                        // Replace the specific text with the updated values
                        textElement.Text = textElement.Text.Replace("CODE", courseCode)
                                                           .Replace("INSTRUCTOR", instructor)
                                                           .Replace("COURSE", courseName)
                                                           .Replace("DATE", dateRange);
                    }
                }
            }
        }

        // Save the file
        commentOutputPath.Save();
    }

    // Create a helper method to generate a paragraph with bullet formatting and line spacing
    Paragraph CreateParagraphWithBullet(string question, string comment)
    {
        var paragraph = new Paragraph(
            new DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties(
                new NumberingProperties(
                    new NumberingLevelReference() { Val = 0 },
                    new NumberingId() { Val = 1 }
                ),
                new SpacingBetweenLines() { Line = "360", LineRule = LineSpacingRuleValues.Auto } // Set line spacing to 1.5
            )
        );

        // Add the question text with bullet point
        paragraph.AppendChild(new Run(new Text(question)));

        // Add the comment text with italic formatting
        var commentRun = new Run();
        commentRun.AppendChild(new Text("\u00A0"));
        commentRun.AppendChild(new RunProperties(new Italic()));
        commentRun.AppendChild(new Text(comment));
        paragraph.AppendChild(commentRun);

        return paragraph;
    }

    // Create a helper method to generate a paragraph with bullet formatting and line spacing for SF Format
    List<Paragraph> CreateParagraphsWithBulletSF(string question, List<string> comments)
    {
        // Create a list to hold the paragraphs
        var paragraphs = new List<Paragraph>();

        // Create a paragraph for the question without a bullet
        var questionParagraph = new Paragraph(
            new DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties(
                new SpacingBetweenLines() { Line = "360", LineRule = LineSpacingRuleValues.Auto } // Set line spacing to 1.5
            )
        );
        questionParagraph.AppendChild(new Run(new Text(question)));
        paragraphs.Add(questionParagraph);

        // Create paragraphs for each comment with a bullet
        foreach (var comment in comments)
        {
            var commentParagraph = new Paragraph(
                new DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties(
                    new NumberingProperties(
                        new NumberingLevelReference() { Val = 0 }, // Bullet level
                        new NumberingId() { Val = 1 }
                    ),
                    new SpacingBetweenLines() { Line = "360", LineRule = LineSpacingRuleValues.Auto }
                )
            );

            // Add the comment text with italic formatting
            var commentRun = new Run();
            commentRun.AppendChild(new RunProperties(new Italic()));
            commentRun.AppendChild(new Text(comment));
            commentParagraph.AppendChild(commentRun);

            // Add the comment paragraph to the list of paragraphs
            paragraphs.Add(commentParagraph);
        }

        return paragraphs;
    }





    private void UpdateRatingCount(int[] ratingCounts, int rating)
    {
        if (rating >= 1 && rating <= 10)
        {
            ratingCounts[rating - 1]++;
        }
    }

    // Add to the count for the recommend
    private void UpdateRecommendCount(int[] recomnendCounts, int recommend)
    {
        if (recommend < 5)
        {
            recomnendCounts[0]++;
        }
        else
        {
            recomnendCounts[1]++;
        }
    }

    // Add to the count for the yes/no
    private void UpdateYesNoCount(int[] yesNoCounts, string? response)
    {
        if (response?.ToLower() == "yes")
        {
            yesNoCounts[1]++; // Index 1 for "Yes"
        }
        else if (response?.ToLower() == "no")
        {
            yesNoCounts[0]++; // Index 0 for "No"
        }
    }

    // Converts the rating string to an int
    private int ConvertStarsToInt(object value)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
        {
            return 0;
        }

        string? rating = value.ToString();

        // Use regular expression to extract the numeric part
        Match match = Regex.Match(rating, @"\d+");
        if (match.Success)
        {
            if (int.TryParse(match.Value, out int convertedRating))
            {
                return convertedRating;
            }
        }
        // default to 0 if the rating is not found.
        return 0;
    }

    // Dictionary to map the rating string to an int
    private static Dictionary<string, int> ratingMap = new Dictionary<string, int>
    {
        { "excellent", 5 },
        { "very good", 4 },
        { "good", 3 },
        { "satisfactory", 2 },
        { "poor", 1 }
    };

    // Converts the rating string to an int
    private int ConvertStringToInt(string rating)
    {
        if (ratingMap.ContainsKey(rating.ToLower()))
        {
            return ratingMap[rating.ToLower()];
        }

        if (int.TryParse(rating, out int result))
        {
            return result;
        }

        // Default to 0 if the rating is not an integer or not found in the map
        return 0;
    }


    // Remove any non-digit characters from the input
    private int ExtractRatingFromFormat(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return 0;
        }

        // Remove any non-digit characters from the input
        string digitsOnly = new string(input.Where(char.IsDigit).ToArray());

        if (int.TryParse(digitsOnly, out int rating))
        {
            return rating;
        }

        // Default to 0 if the rating is not found or cannot be parsed
        return 0;
    }

    // SF helper to search for the rating table
    private int SearchRating(ExcelWorksheet sheet)
    {
        // Start at the first row
        int row = 5;
        // Maximum number of rows to traverse
        int maxTraverse = 50;

        // Traverse down column B, up to a maximum of 50 rows
        while (row <= maxTraverse)
        {
            var cellValue = sheet.Cells[row, 2].Value?.ToString();

            // Check if the cell contains the word "Rating", case sensitive
            if (!string.IsNullOrEmpty(cellValue) && (cellValue.Contains("Rating") || (cellValue.Contains("Choice") && !cellValue.Contains("Value")) || cellValue.Contains("NPS")))
            {
                return row;
            }

            row++;
        }

        // If "Rating" is not found, return -1 or another appropriate value to indicate failure
        return -1;
    }

    // SF helper to search for the Comment table
    private int SearchComment(ExcelWorksheet sheet)
    {
        // Start at the first row
        int row = 10;
        // Maximum number of rows to traverse
        int maxTraverse = 50;

        // Traverse down column B, up to a maximum of 50 rows
        while (row <= maxTraverse)
        {
            var cellValue = sheet.Cells[row, 2].Value?.ToString();
            var cellValue2 = sheet.Cells[row, 3].Value?.ToString();

            // Check if the cell contains the word "Rating", case sensitive
            if (!string.IsNullOrEmpty(cellValue) && cellValue.Contains("Class: Class ID") &&
                !string.IsNullOrEmpty(cellValue2) && cellValue2.Contains("Question: Name"))
            {
                return row;
            }


            row++;
        }

        // If "Rating" is not found, return -1 or another appropriate value to indicate failure
        return -1;
    }

    // Helper to print the Question Dict
    private void PrintQuestionDictionary(Dictionary<string, int[]> questions)
    {
        foreach (var question in questions)
        {
            Console.Write(question.Key + ": ");
            foreach (var value in question.Value)
            {
                Console.Write(value + " ");
            }
            Console.WriteLine();
        }
    }

    // Helper to print the Comments Dict SF
    void PrintSFComments(Dictionary<string, List<string>> comments)
    {
        foreach (var question in comments)
        {
            Console.WriteLine($"{question.Key}:");
            foreach (var comment in question.Value)
            {
                Console.WriteLine($"- {comment}");
            }
            Console.WriteLine();
        }
    }



}
