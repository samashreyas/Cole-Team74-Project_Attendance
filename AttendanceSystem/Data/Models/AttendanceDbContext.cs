// Database Context Class 
// Connects to DB and Creates Classes to Load Data
// Made by Steven Maddela & Nathan Kim


using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;

namespace AttendanceSystem.Data.Models
{
    public class AttendanceDbContext : DbContext
    {
        public DbSet<Class> Classes { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<IPAddressLog> IPAddressLogs { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionResponse> QuestionResponses { get; set; }

        public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Configure QuestionResponse entity - Map to question_responses table
            modelBuilder.Entity<QuestionResponse>()
                .ToTable("question_responses")
                .HasKey(qr => new { qr.QuestionText, qr.StudentID, qr.ClassID });  // Composite key

            modelBuilder.Entity<QuestionResponse>()
                .Property(qr => qr.QuestionText)
                .HasColumnName("question_text");

            modelBuilder.Entity<QuestionResponse>()
                .Property(qr => qr.StudentID)
                .HasColumnName("student_id");

            modelBuilder.Entity<QuestionResponse>()
                .Property(qr => qr.ClassID)
                .HasColumnName("class_id");

            modelBuilder.Entity<QuestionResponse>()
                .Property(qr => qr.Answer)
                .HasColumnName("answer");


            // Student configuration
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.StudentID);
                entity.Property(s => s.StudentID)
                    .HasColumnName("student_id");
            });

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");
                entity.HasKey(e => e.UserID);
                entity.Property(e => e.UserID).HasColumnName("userID");
                entity.Property(e => e.FirstName).HasColumnName("first_name");
                entity.Property(e => e.LastName).HasColumnName("last_name");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.RoleID).HasColumnName("role_id");
                entity.Property(e => e.UTD_ID).HasColumnName("UTD_ID");
            });

            modelBuilder.Entity<IPAddressLog>(entity =>
            {
                entity.ToTable("ip_address_log");

                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasColumnName("id");
                entity.Property(e => e.UserID).HasColumnName("user_id");
                entity.Property(e => e.IPAddress).HasColumnName("ip_address");
                entity.Property(e => e.Timestamp).HasColumnName("timestamp");

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserID);
            });

            // StudentCourse configuration
            modelBuilder.Entity<StudentCourse>(entity =>
            {
                entity.HasKey(sc => new { sc.StudentID, sc.ClassID });

                entity.Property(sc => sc.StudentID)
                    .HasColumnName("student_id");

                entity.Property(sc => sc.ClassID)
                    .HasColumnName("course_id");  // Map to course_id column

                entity.HasOne(sc => sc.Student)
                    .WithMany(s => s.StudentCourses)
                    .HasForeignKey(sc => sc.StudentID);

                entity.HasOne(sc => sc.Class)
                    .WithMany(c => c.StudentCourses)
                    .HasForeignKey(sc => sc.ClassID);
            });

            // Table mappings for AttendanceRecord
            modelBuilder.Entity<AttendanceRecord>()
                .ToTable("attendance")
                .HasKey(ar => ar.AttendanceID); // Define the primary key

            modelBuilder.Entity<AttendanceRecord>()
                .Property(ar => ar.AttendanceID)
                .HasColumnName("attendance_id");

            modelBuilder.Entity<AttendanceRecord>()
                .Property(ar => ar.StudentID)
                .HasColumnName("student_id");

            modelBuilder.Entity<AttendanceRecord>()
                .Property(ar => ar.Date)
                .HasColumnName("attendance_date");

            modelBuilder.Entity<AttendanceRecord>()
                .Property(ar => ar.Status)
                .HasColumnName("status");

            modelBuilder.Entity<AttendanceRecord>()
                .Property(ar => ar.ClassID)
                .HasColumnName("class_id");

            // Configure Class entity
            modelBuilder.Entity<Class>()
                .HasKey(c => c.ClassID);

            modelBuilder.Entity<Class>()
                .Property(c => c.ClassID)
                .HasColumnName("id");

            modelBuilder.Entity<Class>()
                .Property(c => c.TotalStudents)
                .HasColumnName("total_students");

            // Configure StudentCourse (many-to-many)
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentID, sc.ClassID });

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentID);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Class)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.ClassID);

            // Configure Question entity - Map to question_banks table
            modelBuilder.Entity<Question>()
                .ToTable("question_banks")
                .HasKey(q => q.QuestionText);  // Using question_text as primary key

            modelBuilder.Entity<Question>()
                .Property(q => q.QuestionText)
                .HasColumnName("question_text");

            modelBuilder.Entity<Question>()
                .Property(q => q.ClassID)
                .HasColumnName("class_id");

            modelBuilder.Entity<Question>()
                .Property(q => q.Selected)
                .HasColumnName("selected");


            modelBuilder.Entity<Question>()
                .Property(q => q.AnswerChoices)
                .HasColumnName("answer_choices");


            modelBuilder.Entity<Question>()
                .Property(q => q.CorrectAnswer)
                .HasColumnName("correct_answer");


            modelBuilder.Entity<Question>()
                .HasOne(q => q.Class)
                .WithMany(c => c.Questions)
                .HasForeignKey(q => q.ClassID);
        }
    }

    public class Class
    {
        public int ClassID { get; set; }
        public string Name { get; set; }
        public string Time { get; set; }
        public string Room { get; set; }
        public int TotalStudents { get; set; }

        public int InstructorID { get; set; }
        public Instructor Instructor { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; }
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; }
        public ICollection<Question> Questions { get; set; }
    }


    public class QuestionResponse
    {
        public string QuestionText { get; set; }
        public int StudentID { get; set; }
        public int ClassID { get; set; }
        public string Answer { get; set; }
    }

    public class Instructor
    {
        public int InstructorID { get; set; }
        public string Name { get; set; }
    }

    public class StudentCourse
    {
        public long StudentID { get; set; }
        public Student Student { get; set; }

        public int ClassID { get; set; }
        public Class Class { get; set; }
    }

    public class AttendanceRecord
    {
        public int AttendanceID { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }

        public int ClassID { get; set; }
        public Class Class { get; set; }

        public long? StudentID { get; set; }
    }

    public class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public int UTD_ID { get; set; }
        public string PasswordHash { get; set; } = "";
        public int RoleID { get; set; }
    }

    public class IPAddressLog
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string IPAddress { get; set; }
        public DateTime Timestamp { get; set; }
        public User User { get; set; }
    }

    public class Student
    {
        public long? StudentID { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Username { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; }
    }



    public class Question
    {
        public string QuestionText { get; set; } = "";
        public int ClassID { get; set; }
        public bool Selected { get; set; }

        // New properties for multiple choice
        public string? AnswerChoices { get; set; }
        public string? CorrectAnswer { get; set; }

        // Navigation properties
        public Class Class { get; set; }
    }
}
