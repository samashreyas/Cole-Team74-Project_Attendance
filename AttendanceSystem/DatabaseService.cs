// Database connector for upload and assign students functions.. (reminder: Delete commented out code)
// Made by Nathan Kim

using AttendanceSystem.Data.Models;
using AttendanceSystem.Pages;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

public class DatabaseService
{
    private readonly string _connectionString = "Server=127.0.0.1;Database=attendance_db;User=root;Password=Misty2003!!!;"; // Replace DB name and password when necessary
    // Adding a new student. Upload student uses this. Parse info from csv, and insert that information as a query to the database to add the new student entry.
    public async Task<int> InsertStudentsAsync(List<Student> students)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"INSERT IGNORE INTO Students2 (LastName, FirstName, Username, StudentID)
                VALUES (@LastName, @FirstName, @Username, @StudentID)";

        int insertedCount = 0;

        foreach (var student in students)
        {
            int affected = await connection.ExecuteAsync(sql, student);
            insertedCount += affected; // 1 if inserted, 0 if ignored. Count kept to show duplication prevention
        }

        return insertedCount;
    }



// In DatabaseService.cs
// Some retrieval functions for numeric operations in Upload and Assign...
public async Task UpdateClassTotalStudentsAsync(int classId, int studentCount)
{
    using var connection = new MySqlConnection(_connectionString);
    string sql = "UPDATE Classes SET total_students = @StudentCount WHERE id = @ClassId";
    await connection.ExecuteAsync(sql, new { ClassId = classId, StudentCount = studentCount });
}

    public async Task<List<ClassInfo>> GetAllClassesAsync(int professorId)
{
    using var connection = new MySqlConnection(_connectionString);
string sql = "SELECT id AS id, name AS name FROM Classes WHERE InstructorID = @ProfessorId";
    var classes = await connection.QueryAsync<ClassInfo>(sql, new { ProfessorId = professorId });
    return classes.ToList();
}
    public async Task<List<Student>> GetAllStudentsAsync()
    {
        using var connection = new MySqlConnection(_connectionString);
        string sql = "SELECT * FROM Students2 ORDER BY LastName, FirstName";
        var students = await connection.QueryAsync<Student>(sql);
        return students.ToList();
    }
// Add student function: Need to create entries in join table that contain the student's ID, and which class they will be enrolled to
    public async Task AddStudentToClassAsync(int classId, long studentId)
{
   using var connection = new MySqlConnection(_connectionString);
await connection.OpenAsync();
using var transaction = connection.BeginTransaction();

try {
    string sql = @"INSERT IGNORE INTO studentcourses (student_id, course_id)
               VALUES (@studentId, @classId)";
    await connection.ExecuteAsync(sql, new { studentId, classId }, transaction);
    transaction.Commit();
}
catch (Exception ex) {
    Console.WriteLine($"Error: {ex.Message}");
    transaction.Rollback();
    throw;
}

}

// Query to retrieve/remove students in a certain class. From join table StudentCourses
    public async Task<List<Student>> GetStudentsInClassAsync(int classId)
    {
        using var connection = new MySqlConnection(_connectionString);
        string sql = @"
        SELECT s.*
        FROM Students2 s
        JOIN studentcourses cs ON s.StudentID = cs.student_id
        WHERE cs.course_id = @ClassID";
        var students = await connection.QueryAsync<Student>(sql, new { ClassID = classId });
        return students.ToList();
    }

    public async Task RemoveStudentFromClassAsync(int classId, long studentId)
    {
        using var connection = new MySqlConnection(_connectionString);
        string sql = "DELETE FROM studentcourses WHERE course_id = @ClassID AND student_id = @StudentID";
        await connection.ExecuteAsync(sql, new { ClassID = classId, StudentID = studentId });
    }


}
