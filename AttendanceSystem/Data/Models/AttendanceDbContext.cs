using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceSystem.Data.Models
{
    public class AttendanceDbContext : DbContext
    {
        public DbSet<Class> Classes { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionResponse> QuestionResponses { get; set; }

        public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // QuestionResponses
            modelBuilder.Entity<QuestionResponse>()
                .ToTable("question_responses")
                .HasKey(qr => new { qr.QuestionText, qr.StudentID, qr.ClassID });

            modelBuilder.Entity<QuestionResponse>().Property(qr => qr.QuestionText).HasColumnName("question_text");
            modelBuilder.Entity<QuestionResponse>().Property(qr => qr.StudentID).HasColumnName("student_id");
            modelBuilder.Entity<QuestionResponse>().Property(qr => qr.ClassID).HasColumnName("class_id");
            modelBuilder.Entity<QuestionResponse>().Property(qr => qr.Answer).HasColumnName("answer");

            // Students
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.StudentID);
                entity.Property(s => s.StudentID).HasColumnName("student_id");
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

            // StudentCourses
            modelBuilder.Entity<StudentCourse>()
                .ToTable("enrollment")
                .HasKey(sc => new { sc.StudentID, sc.ClassID });

            modelBuilder.Entity<StudentCourse>().Property(sc => sc.StudentID).HasColumnName("student_id");
            modelBuilder.Entity<StudentCourse>().Property(sc => sc.ClassID).HasColumnName("class_id");

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentID);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Class)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.ClassID);

            // Attendance
            modelBuilder.Entity<AttendanceRecord>()
                .ToTable("attendance")
                .HasKey(ar => new { ar.StudentID, ar.ClassID, ar.Date });

            modelBuilder.Entity<AttendanceRecord>().Property(ar => ar.StudentID).HasColumnName("studentID");
            modelBuilder.Entity<AttendanceRecord>().Property(ar => ar.ClassID).HasColumnName("classID");
            modelBuilder.Entity<AttendanceRecord>().Property(ar => ar.Date).HasColumnName("sessionDate");
            modelBuilder.Entity<AttendanceRecord>().Property(ar => ar.Status).HasColumnName("status");

            // Classes
            modelBuilder.Entity<Class>()
                .ToTable("course")
                .HasKey(c => c.ClassID);

            modelBuilder.Entity<Class>().Property(c => c.ClassID).HasColumnName("courseID");
            modelBuilder.Entity<Class>().Property(c => c.Name).HasColumnName("course_name");
            modelBuilder.Entity<Class>().Property(c => c.Time).HasColumnName("time");
            modelBuilder.Entity<Class>().Property(c => c.Room).HasColumnName("room");
            modelBuilder.Entity<Class>().Property(c => c.InstructorID).HasColumnName("instructorID");

            // Questions
            modelBuilder.Entity<Question>()
                .ToTable("quiz questions")
                .HasKey(q => q.QuestionText);

            modelBuilder.Entity<Question>().Property(q => q.QuestionText).HasColumnName("question_text");
            modelBuilder.Entity<Question>().Property(q => q.ClassID).HasColumnName("bankID");

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

        [NotMapped]
        public int TotalStudents { get; set; }

        public int InstructorID { get; set; }
        public Instructor Instructor { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; }
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; }
        public ICollection<Question> Questions { get; set; }
    }

    public class AttendanceRecord
    {
        public int StudentID { get; set; }
        public int ClassID { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }

        public Class Class { get; set; }
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

    public class Student
    {
        public int StudentID { get; set; }
        public string Name { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; }
    }

    [NotMapped]
    public class Instructor
    {
        public int InstructorID { get; set; }
        public string Name { get; set; }
    }

    public class StudentCourse
    {
        public int StudentID { get; set; }
        public Student Student { get; set; }

        public int ClassID { get; set; }
        public Class Class { get; set; }
    }

    public class Question
    {
        public string QuestionText { get; set; }
        public int ClassID { get; set; }

        [NotMapped]
        public bool Selected { get; set; }

        public Class Class { get; set; }
    }

    public class QuestionResponse
    {
        public string QuestionText { get; set; }
        public int StudentID { get; set; }
        public int ClassID { get; set; }
        public string Answer { get; set; }
    }
}
