namespace AttendanceSystem.Data.Models
{
    public class ClassInfo
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; }
    }

    public class ClassStudent
    {
        public int ClassID { get; set; }
        public long StudentID { get; set; }
    }

}
