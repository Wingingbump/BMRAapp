using Microsoft.Graph.ExternalConnectors;
using Microsoft.Office.Interop.Outlook;
using System.Reflection;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace cert_mailer;

public class EmailBuilder
{
    public string Recipient { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string CertificatePath { get; set; }
    public string Grade { get; set; }

    public EmailBuilder(string recipient, string courseName, string courseId, string certificatePath, string grade)
    {
        Recipient = recipient;
        if (double.TryParse(grade, out double numericGrade))
        {
            if (numericGrade >= 0 && numericGrade <= 1)
            {
                // Assuming the grade is a fraction of 1 and converting it to a percentage
                Grade = (numericGrade * 100).ToString("0.##") + "%";
            }
            else if (numericGrade >= 0 && numericGrade <= 100)
            {
                // Assuming the grade is already in percentage format
                Grade = numericGrade.ToString("0.##") + "%";
            }
            else
            {
                // Handle cases where the grade is out of expected range
                Grade = "Invalid grade";
            }
        }
        else
        {
            Grade = grade;
        }
        Subject = SubjectBuilder(courseName, courseId);
        Body = BodyBuilder(courseName, courseId);
        CertificatePath = certificatePath;
    }

    public void CreateDraft()
    {
        Outlook.Application outlook = GetOutlookApplication();
        MailItem mail = (MailItem)outlook.CreateItem(OlItemType.olMailItem);
        mail.To = Recipient;
        mail.Subject = Subject;
        mail.HTMLBody = Body;
        if (Grade == "Certificates")
        {
            mail.Attachments.Add(CertificatePath, OlAttachmentType.olByValue, Type.Missing, Type.Missing); // Will throw exception if file path is invalid
        }
        mail.Save();
        // mail.SentOnBehalfOfName = "certs@bmra.com";

        // Check if draft has been created
        if (mail.EntryID == null)
        {
            throw new System.Exception($"Failed to create draft for recipient: {Recipient}");
        }
    }

    private string BodyBuilder(string course, string BMRARef)
    {
        string? defaultMessage;
        if (Grade == "Certificates")
        {
            defaultMessage = "Hello, <br><br>" +
                 "Congratulations. Attached to this email is your “Certificate of Training” for your " +
                 course +
                 " course; BMRA reference number " +
                 BMRARef +
                 ". If you require any further assistance, please feel free to contact us." +
                 "<br><br>Thank you," +
                 "<br><br>" + SignatureBuilder();
        }
        else
        {
            defaultMessage = "Hello, <br><br>" +
                 "The score for your " + course +
                 "; BMRA reference number " +
                 BMRARef +
                 " is " +
                 Grade +
                 " if you require any further assistance, please feel free to contact us." +
                 "<br><br>Thank you," +
                 "<br><br>" + SignatureBuilder();
        }

        return defaultMessage;
    }

    private string SubjectBuilder(string course, string BMRARef)
    {
        string? defaultMessage;
        if (Grade == "Certificates")
        {
            defaultMessage = "BMRA Ref: " +
                BMRARef +
                " /// Certificate of Training - " +
                course;
        }
        else
        {
            defaultMessage = "Grade For - " +
               course +
               " /// BMRA Ref: " +
               BMRARef;
        }

        return defaultMessage;
    }

    private string GetUsername()
    {
        Outlook.Application outlook = GetOutlookApplication();
        NameSpace outlookNamespace = outlook.GetNamespace("MAPI");
        var username = outlookNamespace.CurrentUser.Name;
        return username;
    }

    private string GetAddress()
    {
        Outlook.Application outlook = GetOutlookApplication();
        NameSpace outlookNamespace = outlook.GetNamespace("MAPI");
        AddressEntry currentUser = outlookNamespace.CurrentUser.AddressEntry;
        var smtpAddress = string.Empty;

        if (currentUser != null && currentUser.AddressEntryUserType == OlAddressEntryUserType.olExchangeUserAddressEntry)
        {
            ExchangeUser exchUser = currentUser.GetExchangeUser();
            if (exchUser != null)
            {
                smtpAddress = exchUser.PrimarySmtpAddress;
            }
        }

        return smtpAddress;
    }

    private string SignatureBuilder()
    {
        var signatureTemplate = GetTemplateContent("cert_mailer.template.htm");
        signatureTemplate = signatureTemplate.Replace("{{Username}}", "<b>" + GetUsername() + "</b>");
        signatureTemplate = signatureTemplate.Replace("{{Email}}", GetAddress());
        return signatureTemplate;
    }


    private string GetTemplateContent(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceStream = assembly.GetManifestResourceStream(resourceName);

        if (resourceStream == null)
        {
            throw new ArgumentException($"Resource {resourceName} not found in assembly.");
        }

        using var reader = new StreamReader(resourceStream);
        return reader.ReadToEnd();
    }


    private Outlook.Application GetOutlookApplication()
    {
        Type? outlookType;
        try
        {
            outlookType = Type.GetTypeFromProgID("Outlook.Application", throwOnError: true);
        }
        catch (System.Exception ex)
        {
            throw new System.Exception("Failed to retrieve Outlook application type.", ex);
        }

        var outlook = Activator.CreateInstance(outlookType);
        Outlook.Application outlookApp;

        try
        {
            outlookApp = (Outlook.Application)outlook;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("Failed to cast to Outlook application.", ex);
        }

        return outlookApp;
    }

}
