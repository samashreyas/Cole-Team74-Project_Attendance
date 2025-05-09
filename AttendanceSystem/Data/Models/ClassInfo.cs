namespace AttendanceSystem.Data.Models
{
    // Ensure your ClassInfo class looks like this:
    public class ClassInfo
    {
        public int id { get; set; } // Matches the alias in your SQL
        public string name { get; set; }
    }

    public class ClassStudent
    {
        public int ClassID { get; set; }
        public long StudentID { get; set; }
    }

}