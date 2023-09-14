using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml;
using System.Text.RegularExpressions;
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
            evalReader(evalSheet);
        }

    }
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
            questionRatingCounts.Add("Question16", new int[5]);
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

        for (var row = 2; row <= rowCount; row++)
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

            // Create a HashSet to store the responses to omit with case-insensitive matching
            HashSet<string> omittedResponses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "None",
                "NA",
                "N/A",
                "No",
                "No Response",
                "Not Applicable",
                "Not at this time"
            };

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

        outputSheet.HeaderFooter.OddHeader.RightAlignedText = $"BMRA Ref: {courseCode} \r\n DATE: {dateRange}";

        // Footer for Evaluation Summary
        outputSheet.HeaderFooter.OddFooter.LeftAlignedText = $"Name of course: {courseABV}";
        outputSheet.HeaderFooter.OddFooter.CenteredText = $"{agency} - Virtual";
        outputSheet.HeaderFooter.OddFooter.RightAlignedText = $"Instructor: {instructor}";


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

}
