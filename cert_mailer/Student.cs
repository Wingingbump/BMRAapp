namespace cert_mailer
{
    public class Student
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string Certification { get; set; }
        public bool Pass { get; set; }
        public string Grade
        { get; set; }

        public Student(string firstName, string lastName, string email, string certification, bool pass, string grade)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Certification = certification;
            Pass = pass;
            Grade = grade;
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName} ({Email}): {Certification} - {Pass}";
        }

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

        public override int GetHashCode()
        {
            return HashCode.Combine(FirstName, LastName, Email, Certification, Pass);
        }

    }


}
