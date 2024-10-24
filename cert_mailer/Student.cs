namespace cert_mailer;

/// <summary>
/// Represents a student with information such as first name, last name, email, certification, pass status, and grade.
/// </summary>
public class Student
{
    /// <summary>
    /// Gets the first name of the student.
    /// </summary>
    public string FirstName
    {
        get;
    }
    /// <summary>
    /// Gets the last name of the student.
    /// </summary>
    public string LastName
    {
        get;
    }
    /// <summary>
    /// Gets the email of the student.
    /// </summary>
    public string Email
    {
        get;
    }
    /// <summary>
    /// Gets or sets the certification of the student.
    /// </summary>
    public string Certification
    {
        get; set;
    }
    /// <summary>
    /// Gets or sets a value indicating whether the student passed the certification.
    /// </summary>
    public bool Pass
    {
        get; set;
    }
    /// <summary>
    /// Gets or sets the grade of the student.
    /// </summary>
    public string Grade
    {
        get; set;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Student"/> class with the specified details.
    /// </summary>
    /// <param name="firstName">The first name of the student.</param>
    /// <param name="lastName">The last name of the student.</param>
    /// <param name="email">The email of the student.</param>
    /// <param name="certification">The certification of the student.</param>
    /// <param name="pass">A value indicating whether the student passed the certification.</param>
    /// <param name="grade">The grade of the student.</param>
    public Student(string firstName, string lastName, string email, string certification, bool pass, string grade)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Certification = certification;
        Pass = pass;
        Grade = grade;
    }

    /// <summary>
    /// Returns a string that represents the current student.
    /// </summary>
    /// <returns>A string that represents the current student.</returns>
    public override string ToString()
    {
        return $"{FirstName} {LastName} ({Email}): {Certification} - {Pass}";
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (Student)obj;
        return FirstName == other.FirstName
            && LastName == other.LastName
            && Email == other.Email
            && Certification == other.Certification
            && Pass == other.Pass;
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(FirstName, LastName, Email, Certification, Pass);
    }
}
