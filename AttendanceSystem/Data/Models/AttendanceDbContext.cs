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
        public DbSet<StudentCourse> StudentCourses { get; set; } // Many-to-many relation table
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }

        public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the primary key and column mappings
            modelBuilder.Entity<Class>()
                .HasKey(c => c.ClassID); // Use ClassID as primary key

           

modelBuilder.Entity<Class>()
    .Property(c => c.ClassID)
    .HasColumnName("id"); // Map the ClassID property to 'id' column in the database

             modelBuilder.Entity<Class>()
        .Property(c => c.TotalStudents)
        .HasColumnName("total_students"); // 👈 Fix for the error you hit

            // Configure the primary key for the many-to-many join table StudentCourse
           modelBuilder.Entity<StudentCourse>()
    .HasKey(sc => new { sc.StudentID, sc.ClassID }); // ✅ use ClassID

            // Configure relationships
            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentID);
                
            modelBuilder.Entity<AttendanceRecord>()
    .HasOne(ar => ar.Class)
    .WithMany(c => c.AttendanceRecords)
    .HasForeignKey(ar => ar.ClassID); // Ensure this matches your schema


            modelBuilder.Entity<StudentCourse>()
    .HasOne(sc => sc.Class)
    .WithMany(c => c.StudentCourses)
    .HasForeignKey(sc => sc.ClassID); // ✅ not CourseID anymore
        }
        
    }
    

    public class Class
{
    public int ClassID { get; set; }
    public string Name { get; set; }
    public string Time { get; set; }
    public string Room { get; set; }

    public int TotalStudents { get; set; }

    public int InstructorID { get; set; } // FK to Instructor
    public Instructor Instructor { get; set; } // Navigation property

    public ICollection<StudentCourse> StudentCourses { get; set; } // Many-to-many relation

    // Add this navigation property for AttendanceRecords
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } // Navigation property to AttendanceRecords
}


    public class Instructor
    {
        public int InstructorID { get; set; }
        public string Name { get; set; }
    }

   public class StudentCourse
{
    public int StudentID { get; set; }
    public Student Student { get; set; }

    public int ClassID { get; set; }  //  renamed from CourseID to ClassID
    public Class Class { get; set; }
}


public class AttendanceRecord
{
    public int AttendanceRecordID { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; }

    public int ClassID { get; set; } // Make sure ClassID exists here
    public Class Class { get; set; } // Navigation property to Class
}


    public class Student
    {
        public int StudentID { get; set; }
        public string Name { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; }
    }
}
