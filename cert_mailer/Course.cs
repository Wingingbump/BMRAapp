namespace cert_mailer
{
    /// <summary>
    /// Represents a course
    /// </summary>
    public class Course
    {
        /// <summary>
        /// Course Name
        /// </summary>
        public string CourseName { get; set; }
        /// <summary>
        /// Prefered Course Name Between ID and Name
        /// </summary>
        public string CourseNamingScheme { get; set; }
        /// <summary>
        /// Course ID
        /// </summary>
        public string CourseId { get; set; }
        /// <summary>
        /// Agency Name
        /// </summary>
        public string AgencyName { get; set; }
        /// <summary>
        /// Instructor for the course
        /// NOT USED, left blank for all instances
        /// </summary>
        public string Instructor { get; set; }
        /// <summary>
        /// Course Location
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// Course Start Date
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Course End Date
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// List of all students who participated in the course.
        /// Includes students who pass or fail, but not students who did not complete the course 
        /// </summary>
        public List<Student> Students { get; set; }

        /// <summary>
        /// Default Course Constructor
        /// </summary>
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

        /// <summary>
        /// Paramaterized Constructor
        /// </summary>
        /// <param name="courseName"> Course Name </param>
        /// <param name="courseNamingScheme"> Prefered Course Name </param>
        /// <param name="courseId"> Course ID </param>
        /// <param name="agencyName"> Agency Name </param>
        /// <param name="instructor"> Unused Instructor Name </param>
        /// <param name="location"> Location </param>
        /// <param name="startDate"> Start Date </param>
        /// <param name="endDate"> End Date </param>
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

        /// <summary>
        /// Adds a student to the student list
        /// </summary>
        /// <param name="student"> The student to add </param>
        public void AddStudent(Student student)
        {
            Students.Add(student);
        }

        /// <summary>
        /// Removes a student from the student list
        /// </summary>
        /// <param name="student"> The student to remove </param>
        public void RemoveStudent(Student student)
        {
            Students.Remove(student);
        }

        /// <summary>
        /// Returns a string that represents the course.
        /// </summary>
        /// <returns>A string that represents the course.</returns>
        public override string ToString()
        {
            return $"Course: {CourseName} ({CourseId}), Agency: {AgencyName}, Instructor: {Instructor}, Location: {Location}, Start Date: {StartDate.ToString()}, End Date: {EndDate.ToString()}, Students: {Students.Count}";
        }

        /// <summary>
        /// Equals Method
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Course))
            {
                return false;
            }

            Course other = (Course)obj;
            return CourseName == other.CourseName && CourseId == other.CourseId && AgencyName == other.AgencyName && Instructor == other.Instructor && Location == other.Location && StartDate == other.StartDate && EndDate == other.EndDate && Students.Equals(other.Students);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(CourseName, CourseId, AgencyName, Instructor, Location, StartDate, EndDate, Students);
        }
    }
}
