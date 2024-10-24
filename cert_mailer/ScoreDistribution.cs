using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using OfficeOpenXml;

namespace cert_mailer;
public class ScoreDistribution
{
    private readonly FileInfo gradeFile;
    private FileInfo attendanceFile;

    public ScoreDistribution(FileInfo gradeFile, FileInfo attendanceFile)
    {
        this.gradeFile = gradeFile;
        this.attendanceFile = attendanceFile;
        // Create a dataReader so we can use it's data
        DataRead reader = new DataRead(attendanceFile, gradeFile, "SD", false, EnumCertificateType.CertificateType.None, 0, false);
        var courseName = reader.Course.CourseName;
        var courseId = reader.Course.CourseId;
        foreach (Student student in reader.Course.Students)
        {
            EmailBuilder message = new EmailBuilder(student.Email, courseName, courseId, "SD", student.Grade);
            message.CreateDraft();
        }
    }



}
