namespace AttendanceSystem.Models
{
    public class AttendanceRecord
    {
        public int Id { get; set; }
        public string? StudentName { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
    }

}
