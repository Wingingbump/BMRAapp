namespace cert_mailer
{
    public class Course
    {
        public string CourseName { get; set; }
        public string CourseNamingScheme { get; set; }
        public string CourseId { get; set; }
        public string AgencyName { get; set; }
        public string Instructor { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<Student> Students { get; set; }

        // Default constructor
        public Course()
        {
            CourseName = "";
            CourseNamingScheme = "";
            CourseId = "";
            AgencyName = "";
            Instructor = "";
            Location = "";
            StartDate = DateTime.MinValue;
            EndDate = DateTime.MaxValue;
            Students = new List<Student>();
        }

        public Course(string courseName, string courseNamingScheme, string courseId, string agencyName, string instructor, string location, DateTime startDate, DateTime endDate)
        {
            CourseName = courseName;
            CourseNamingScheme = courseNamingScheme;
            CourseId = courseId;
            AgencyName = agencyName;
            Instructor = instructor;
            Location = location;
            StartDate = startDate;
            EndDate = endDate;
            Students = new List<Student>();
        }

        public void AddStudent(Student student)
        {
            Students.Add(student);
        }

        public void RemoveStudent(Student student)
        {
            Students.Remove(student);
        }

        public override string ToString()
        {
            return $"Course: {CourseName} ({CourseId}), Agency: {AgencyName}, Instructor: {Instructor}, Location: {Location}, Start Date: {StartDate.ToString()}, End Date: {EndDate.ToString()}, Students: {Students.Count}";
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Course))
            {
                return false;
            }

            Course other = (Course)obj;
            return CourseName == other.CourseName && CourseId == other.CourseId && AgencyName == other.AgencyName && Instructor == other.Instructor && Location == other.Location && StartDate == other.StartDate && EndDate == other.EndDate && Students.Equals(other.Students);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CourseName, CourseId, AgencyName, Instructor, Location, StartDate, EndDate, Students);
        }
    }
}
