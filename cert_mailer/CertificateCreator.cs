using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf;
using OfficeOpenXml;
using Spire.Doc;
using System.IO.Compression;
using System.Reflection;

namespace cert_mailer
{
    public class CertificateCreator
    {

        private Course course;
        private string certificateName;
        private string certPath;
        private ExcelWorksheet gradeSheet;

        public CertificateCreator(ExcelWorksheet gradeSheet, string certPath, Course course, EnumCertificateType.CertificateType enumCertType)
        {
            this.course = course;
            this.certPath = certPath;
            certificateName = course.CourseNamingScheme;

            // Get the Grades sheet
            this.gradeSheet = gradeSheet;
            string certType = GetCertType(enumCertType);

            CertCreator(gradeSheet, certType);
            CreateCompressed(certPath);
            MergePdfFiles(certPath);
        }

        // Create the certs docs -> pdf
        public void CertCreator(ExcelWorksheet gradesSheet, string certTempPath)
        {
            // Collect basic info from the course
            var courseCode = certificateName;
            var courseName = course.CourseName;
            var startDate = course.StartDate;
            var endDate = course.EndDate;
            var location = course.Location;
            var clp = gradesSheet.Cells[2, 12].Value;

            // For all rows with data
            var rowCount = gradesSheet.Cells
            .Select(cell => cell.Start.Row)
            .Distinct()
            .Count(row => gradesSheet.Cells[row, 1].Value != null);

            // create doc cert for each student
            Parallel.For(1, rowCount, (row) =>
            {
                int gradeSpacing = row + DataRead.GRADESPACE;
                string? firstName = gradesSheet.Cells[gradeSpacing, 3].Value.ToString();
                string? lastName = gradesSheet.Cells[gradeSpacing, 5].Value.ToString();

                // string templatePath = @"C:\Users\Tommy\source\repos\Wingingbump\cert_mailer\Certificate of Training - Edit.docx"; // TEMP
                // Create the output for the docs
                string fullName = $"{firstName} {lastName}";
                string outputPath = Path.Combine(certPath, $"{row:00} - {fullName} - Certificate of Training - {course.CourseNamingScheme}.docx");

                // Copy the template file to the output path
                System.IO.File.Copy(certTempPath, outputPath, true);

                // Open the copy of the template file for editing
                using (WordprocessingDocument document = WordprocessingDocument.Open(outputPath, true))
                {
                    // Access the main document part
                    var mainPart = document.MainDocumentPart;
                    if (mainPart?.Document?.Body == null)
                    {
                        throw new Exception("Unable to find the main part of the document.");
                    }

                    // Define dictionary to store placeholder names and values
                    Dictionary<string, string> placeholders;
                    if (startDate == endDate)
                    {
                        placeholders = new Dictionary<string, string>()
                        {
                            { "FNAME", firstName ?? "null firstname"},
                            { "LNAME", lastName ?? "null lastname"},
                            { "COURSE", courseName },
                            { "CLPS", clp.ToString() ?? "null clp"},
                            { "LOCATION", location },
                            { "START_DATE", startDate.ToString("M/d/yyyy") },
                            { "END_DATE", "" }
                        };
                    }
                    else
                    {
                        placeholders = new Dictionary<string, string>()
                        {
                            { "FNAME", firstName ?? "null firstname"},
                            { "LNAME", lastName ?? "null lastname"},
                            { "COURSE", courseName },
                            { "CLPS", clp.ToString() ?? "null clp"},
                            { "LOCATION", location },
                            { "START_DATE", startDate.ToString("M/d/yyyy") },
                            { "END_DATE", " - " + endDate.ToString("M/d/yyyy") }
                        };
                    }

                    // Find and replace text in the document
                    foreach (var paragraph in mainPart.Document.Body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
                    {
                        foreach (var run in paragraph.Descendants<DocumentFormat.OpenXml.Wordprocessing.Run>())
                        {
                            var textElements = run.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>();
                            if (textElements.Any())
                            {
                                var text = string.Concat(textElements.Select(t => t.Text));
                                foreach (var placeholder in placeholders)
                                {
                                    if (text.Contains(placeholder.Key))
                                    {
                                        run.RemoveAllChildren<DocumentFormat.OpenXml.Wordprocessing.Text>();
                                        var newText = text.Replace(placeholder.Key, placeholder.Value);
                                        run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text(newText));
                                    }
                                }
                            }
                        }
                    }
                    // Save the modified document
                    document.Save();
                }
                string PDFOutputPath = Path.ChangeExtension(outputPath, ".pdf");
                ConvertToPdf(outputPath, PDFOutputPath);
                DeleteDocx(outputPath);
            });
        }

        public void ConvertToPdf(string docxPath, string pdfPath)
        {
            // Load the Word document
            Document doc = new Document();
            doc.LoadFromFile(docxPath);

            // Save the document as PDF
            doc.SaveToFile(pdfPath, Spire.Doc.FileFormat.PDF);
        }

        public void DeleteDocx(string docxPath)
        {
            System.IO.File.Delete(docxPath);
        }

        public void CreateCompressed(string filePath)
        {
            // Create a unique name for the ZIP file
            string zipName = $"Compressed Certificates of Training - {course.CourseNamingScheme}.zip";
            string zipPath = Path.Combine(filePath, zipName);

            // Collect all the PDF files in the directory
            string[] pdfFiles = System.IO.Directory.GetFiles(filePath, "*.pdf");

            // Create a new ZIP file and add the PDF files to it
            using (ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                foreach (string pdfFile in pdfFiles)
                {
                    // Create a name for the PDF file in the ZIP archive
                    string zipEntryName = Path.GetFileName(pdfFile);

                    // Create a new entry in the ZIP archive and copy the PDF file data into it
                    ZipArchiveEntry entry = zip.CreateEntry(zipEntryName);
                    using (Stream entryStream = entry.Open())
                    using (FileStream pdfStream = System.IO.File.OpenRead(pdfFile))
                    {
                        pdfStream.CopyTo(entryStream);
                    }
                }
            }
        }
        public void MergePdfFiles(string directoryPath)
        {
            // Get all PDF files in the directory
            string[] pdfFiles = System.IO.Directory.GetFiles(directoryPath, "*.pdf");

            // Create a new PDF document for the merged files
            string fileName = $"Certificates of Training - {course.CourseNamingScheme}.pdf";
            string filePath = Path.Combine(directoryPath, fileName);
            PdfDocument mergedPdf = new PdfDocument(new PdfWriter(filePath));

            // Loop through each PDF file and merge it with the merged document
            foreach (string pdfFile in pdfFiles)
            {
                // Open the PDF file
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(pdfFile));

                // Copy the pages to the merged document
                pdfDoc.CopyPagesTo(1, pdfDoc.GetNumberOfPages(), mergedPdf);

                // Close the PDF document
                pdfDoc.Close();
            }

            // Close the merged PDF document
            mergedPdf.Close(); 
        }

        public string GetCertType(EnumCertificateType.CertificateType enumCertType)
        {
            string fileName;
            string filePath = Application.StartupPath + "\\Assets";
            switch (enumCertType)
            {
                case EnumCertificateType.CertificateType.Default:
                    fileName = "Certificate of Training - Edit.docx";
                    filePath = Path.Combine(filePath, fileName);
                    return filePath;
                case EnumCertificateType.CertificateType.SBA:
                    fileName = "Certificate of Training - SBA Edit.docx";
                    filePath = Path.Combine(filePath, fileName);
                    return filePath;
                case EnumCertificateType.CertificateType.NOAA:
                    fileName = "Certificate of Training - NOAA Edit.docx";
                    filePath = Path.Combine(filePath, fileName);
                    return filePath;
                case EnumCertificateType.CertificateType.DOIU:
                    fileName = "Certificate of Training - DOIU Edit.docx";
                    filePath = Path.Combine(filePath, fileName);
                    return filePath;
                default:
                    throw new ArgumentOutOfRangeException(nameof(enumCertType), enumCertType, null);
            }
        }

    }
}
