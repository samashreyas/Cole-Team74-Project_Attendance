using AttendanceSystem.Data.Models;
using AttendanceSystem.Pages;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

public class DatabaseService
{
    private readonly string _connectionString = "Server=127.0.0.1;Database=attendance_db;User=root;Password=Misty2003!!!;";

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
            insertedCount += affected; // 1 if inserted, 0 if ignored
        }

        return insertedCount;
    }



// In DatabaseService.cs

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





    /*
    public async Task<List<Student>> GetStudentsAsync()
    {
        var students = new List<Student>();
        using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();

        string query = "SELECT ID, FirstName, LastName FROM Students";
        using var cmd = new MySqlCommand(query, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            students.Add(new Student
            {
                ID = reader.GetInt32("ID"),
                FirstName = reader.GetString("FirstName"),
                LastName = reader.GetString("LastName")
            });
        }

        return students;
    }

    public async Task<List<Question>> GetQuizQuestions(int bankId)
    {
        var questions = new List<Question>();

        using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new MySqlCommand("SELECT * FROM Questions WHERE BankID = @bankId", conn);
        cmd.Parameters.AddWithValue("@bankId", bankId);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            questions.Add(new Question
            {
                QuestionID = reader.GetInt32("QuestionID"),
                Text = reader.GetString("Text"),
                CorrectOption = reader.GetString("CorrectOption"),
                BankID = reader.GetInt32("BankID")
            });
        }

        foreach (var question in questions)
        {
            using var optionConn = new MySqlConnection(_connectionString);
            await optionConn.OpenAsync();

            var optionCmd = new MySqlCommand("SELECT * FROM QuestionOptions WHERE QuestionID = @qid", optionConn);
            optionCmd.Parameters.AddWithValue("@qid", question.QuestionID);

            using var optionReader = await optionCmd.ExecuteReaderAsync();
            while (await optionReader.ReadAsync())
            {
                question.Options.Add(new QuestionOption
                {
                    OptionID = optionReader.GetInt32("OptionID"),
                    QuestionID = question.QuestionID,
                    OptionLabel = optionReader.GetString("OptionLabel"),
                    OptionText = optionReader.GetString("OptionText")
                });
            }

            while (question.Options.Count < 4)
            {
                question.Options.Add(new QuestionOption
                {
                    OptionLabel = ((char)('A' + question.Options.Count)).ToString(),
                    OptionText = null
                });
            }
        }

        return questions;
    }

    public async Task<List<QuestionRow>> GetQuestionRowsAsync()
    {
        var questionRows = new List<QuestionRow>();

        using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();

        var query = @"
        SELECT c.ClassID, m.MeetingDate, qz.QuizID, qt.QuestionID, qt.QuestionText, qt.CorrectOption
        FROM Classes c
        JOIN Meetings m ON m.ClassID = c.ClassID
        JOIN Quizzes qz ON qz.MeetingID = m.MeetingID
        JOIN Questions qt ON qt.QuizID = qz.QuizID
    ";

        var tempRows = new List<(int QuestionID, QuestionRow Row)>();

        using (var cmd = new MySqlCommand(query, conn))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var row = new QuestionRow
                {
                    ClassID = reader.GetInt32("ClassID"),
                    MeetingDate = reader.GetDateTime("MeetingDate"),
                    QuizID = reader.GetInt32("QuizID"),
                    QuestionID = reader.GetInt32("QuestionID"),
                    QuestionText = reader.GetString("QuestionText"),
                    CorrectOption = reader.GetString("CorrectOption"),
                    Options = new List<AnswerOption>()
                };

                questionRows.Add(row);
                tempRows.Add((row.QuestionID, row));
            }
        }

        foreach (var (questionId, row) in tempRows)
        {
            using var optionConn = new MySqlConnection(_connectionString);
            await optionConn.OpenAsync();

            var optionCmd = new MySqlCommand("SELECT OptionID, OptionLabel, OptionText FROM AnswerOptions WHERE QuestionID = @qid", optionConn);

            optionCmd.Parameters.AddWithValue("@qid", questionId);

            using var optionReader = await optionCmd.ExecuteReaderAsync();
            while (await optionReader.ReadAsync())
            {
                row.Options.Add(new AnswerOption
                {
                    OptionID = optionReader.GetInt32("OptionID"),
                    OptionLabel = optionReader.GetString("OptionLabel"),
                    OptionText = optionReader.GetString("OptionText")
                });
            }

            while (row.Options.Count < 4)
            {
                row.Options.Add(new AnswerOption
                {
                    OptionLabel = ((char)('A' + row.Options.Count)).ToString(),
                    OptionText = null
                });
            }
        }

        return questionRows;
    }

    public async Task<List<int>> GetAllClassIDsAsync()
    {
        var classIds = new List<int>();
        using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new MySqlCommand("SELECT DISTINCT ClassID FROM Classes", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            classIds.Add(reader.GetInt32("ClassID"));
        }

        return classIds;
    }

    public async Task<List<DateTime>> GetMeetingDatesForClassAsync(int classId)
    {
        var meetingDates = new List<DateTime>();
        using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new MySqlCommand("SELECT DISTINCT MeetingDate FROM Meetings WHERE ClassID = @ClassID", conn);
        cmd.Parameters.AddWithValue("@ClassID", classId);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            meetingDates.Add(reader.GetDateTime("MeetingDate"));
        }

        return meetingDates;
    }

    public async Task<List<int>> GetQuizIDsForMeetingAsync(int classId, DateTime meetingDate)
    {
        var quizIds = new List<int>();
        using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new MySqlCommand(@"
        SELECT DISTINCT QuizID FROM Quizzes 
        WHERE ClassID = @ClassID AND MeetingDate = @MeetingDate", conn);

        cmd.Parameters.AddWithValue("@ClassID", classId);
        cmd.Parameters.AddWithValue("@MeetingDate", meetingDate);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            quizIds.Add(reader.GetInt32("QuizID"));
        }

        return quizIds;
    }

    public async Task SaveQuestionRowsAsync(List<QuestionRow> questions)
    {
        using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();

        foreach (var question in questions)
        {
            var updateQuestionCmd = new MySqlCommand(
                "UPDATE Questions SET QuestionText = @QuestionText, CorrectOption = @CorrectOption WHERE QuestionID = @QuestionID",
                conn);
            updateQuestionCmd.Parameters.AddWithValue("@QuestionText", question.QuestionText);
            updateQuestionCmd.Parameters.AddWithValue("@CorrectOption", question.CorrectOption);
            updateQuestionCmd.Parameters.AddWithValue("@QuestionID", question.QuestionID);

            await updateQuestionCmd.ExecuteNonQueryAsync();

            foreach (var option in question.Options)
            {
                if (option.OptionID > 0)
                {
                    var updateOptionCmd = new MySqlCommand("UPDATE AnswerOptions SET OptionText = @OptionText WHERE OptionID = @OptionID", conn);

                    updateOptionCmd.Parameters.AddWithValue("@OptionText", option.OptionText);
                    updateOptionCmd.Parameters.AddWithValue("@OptionID", option.OptionID);

                    await updateOptionCmd.ExecuteNonQueryAsync();
                }
                else
                {
                    var insertOptionCmd = new MySqlCommand("INSERT INTO AnswerOptions (QuestionID, OptionLabel, OptionText) VALUES (@QuestionID, @OptionLabel, @OptionText)", conn);

                    insertOptionCmd.Parameters.AddWithValue("@QuestionID", question.QuestionID);
                    insertOptionCmd.Parameters.AddWithValue("@OptionLabel", option.OptionLabel);
                    insertOptionCmd.Parameters.AddWithValue("@OptionText", option.OptionText);

                    await insertOptionCmd.ExecuteNonQueryAsync();
                }
            }
        }
    }*/

}