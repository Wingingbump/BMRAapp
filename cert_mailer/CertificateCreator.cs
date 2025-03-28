using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf;
using iText.Layout.Properties;
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
        private bool addPDU;
        private bool addCPE;
        private bool addCEU;

        // construct and intialize
        public CertificateCreator(ExcelWorksheet gradeSheet, string certPath, Course course, EnumCertificateType.CertificateType enumCertType, bool addPDU, bool addCPE, bool addCEU)
        {
            this.course = course;
            this.certPath = certPath;
            this.addPDU = addPDU;
            this.addCPE = addCPE;
            this.addCEU = addCEU;
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
            var pdu = "";
            var cpe = "";
            var ceu = "";
            if (addPDU)
            {
                pdu = ", " + clp + " PDUs";
            }
            if (addCPE)
            {
                cpe = ", " + clp + " CPEs";
            }
            if (addCEU)
            {
                ceu = ", " + clp + " CEUs";
            }

            // Combine them into a single string if needed
            var certificationText = $" {pdu}{cpe}{ceu}".TrimStart(',', ' ');
            certificationText = $" {pdu}{cpe}{ceu}";

            // Make a DNS folder
            string DNSpath = certPath + "\\DNS";
            Directory.CreateDirectory(DNSpath);
            var DNScounter = 0;

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
                            { "CLUS", certificationText.ToString()},
                            { "LOCATION", location},
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
                            { "CLUS", certificationText .ToString()},
                            { "LOCATION", location},
                            { "START_DATE", startDate.ToString("M/d/yyyy") + " " },
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
                // if student fail place their cert in DNS
                string PDFOutputPath = Path.ChangeExtension(outputPath, ".pdf");
                if (course.Students[row-1].Pass == false)
                {
                    DNScounter++;
                    PDFOutputPath = Path.Combine(certPath, "DNS", Path.GetFileName(outputPath) + ".pdf");
                }
                ConvertToPdf(outputPath, PDFOutputPath);
                DeleteDocx(outputPath);
            });
            if (DNScounter == 0)
            {
                Directory.Delete(DNSpath, true);
            }
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
