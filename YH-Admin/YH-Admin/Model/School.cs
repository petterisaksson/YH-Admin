﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YH_Admin.Utils;

namespace YH_Admin.Model
{
    public class School
    {
        public SchoolIO SchoolDatabase { get; set; }

        public List<User> Users { get; set; }

        public List<Education> Educations { get; set; }

        public List<SchoolClass> SchoolClasses { get; private set; }

        public List<Student> Students { get; private set; }

        public List<Course> Courses { get; private set; }

        public List<EducationCourse> EducationCourses { get; private set; }

        public List<ClassCourse> ClassCourseTable { get; private set; }

        public List<Grade> Grades { get; set; }

        public School()
        {
            Users = new List<User>();
            Educations = new List<Education>();
            SchoolClasses = new List<SchoolClass>();
            Students = new List<Student>();
            Courses = new List<Course>();
            EducationCourses = new List<EducationCourse>();
            ClassCourseTable = new List<ClassCourse>();
            Grades = new List<Grade>();
        }

        /// <summary>
        /// Read all the datafiles in a specific folder.
        /// </summary>
        public void LoadData(string soluPath)
        {
            SchoolDatabase = new SchoolIO(soluPath);
            
            // Read user file
            Users = SchoolDatabase.ReadUserFile();
            // Read student file
            Educations = SchoolDatabase.ReadEducationFile();
            // Read class file
            Students = SchoolDatabase.ReadStudentFile();
            // Read course file
            SchoolClasses = SchoolDatabase.ReadClassFile();
            // Read class-course file
            Courses = SchoolDatabase.ReadCourseFile();
            // Read education file
            ClassCourseTable = SchoolDatabase.ReadClassCourseFile();
            // Read education-course file
            EducationCourses = SchoolDatabase.ReadEducationCourseFile();
            // Read grade file
            Grades = SchoolDatabase.ReadGradeFile();
            
        }
        

        public void SaveToFiles()
        {
            SchoolDatabase.SaveGradeFile(Grades);
        }
        
        /// <summary>
        /// Read the school classes data
        /// </summary>
        /// <param name="path"></param>
        public void ReadClassFile(string path)
        {
            try
            {
                SchoolClasses = new List<SchoolClass>();
                string[] lines = File.ReadAllLines(path);
                foreach (var line in lines)
                {
                    var splits = line.Split(' ');
                    var startDate = DateTime.ParseExact(splits[3], "yyyyMMdd", null);
                    var endDate = DateTime.ParseExact(splits[4], "yyyyMMdd", null);
                    var cl = new SchoolClass(int.Parse(splits[0]), splits[1], int.Parse(splits[2]), startDate, endDate);
                    SchoolClasses.Add(cl);

                    //Test code: 
                    //Console.WriteLine(cl);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in creating SchoolClass: " + ex);
                Console.ReadLine();

            }

        }

        public Grade GetGrade(Student student, ClassCourse classCourse)
        {
            return Grades.Find(g => g.StudentId == student.StudentId && g.ClassCourseId == classCourse.ClassCourseId);
        }

        public List<Student> GetFailers()
        {
            var failList = Grades.Where(g => g.GradeString == "IG");
            var failedStudent = new List<Student>();

            foreach (var grade in failList)
            {
                var student = Students.Find(s => s.StudentId == grade.StudentId);
                if (student != null)
                    failedStudent.Add(student);
            }

            return failedStudent;
        }

        public void SetGrade(Student student, ClassCourse classCourse, string gradeString)
        {
            var grade = GetGrade(student, classCourse);
            if (grade != null)
                grade.GradeString = gradeString;
            else
            {
                Grades.Add(new Grade(Guid.NewGuid(), student.StudentId, classCourse.ClassCourseId, gradeString));
            }
        }

        public List<Education> GetEducations(int userId)
        {
            var educations = Educations.Where(e => e.UserId == userId).ToList();
            return educations;
        }

        public List<Education> GetEducations(User user)
        {
            return GetEducations(user.UserId);
        }

        public List<ClassCourse> GetClassCourses(Student student)
        {
            var schoolClass = SchoolClasses.Find(c => c.SchoolClassId == student.ClassId);
            return GetClassCourses(schoolClass);
        }

        public List<string> GetCourses(int classId)
        {
            var ccs = ClassCourseTable.Where(c => c.ClassId == classId);
            var sorted = ccs.OrderBy(c => c.StartDate);
            var output = new List<string>();
            foreach (var cc in sorted)
            {
                var course = Courses.Find(c => c.CourseId == cc.CourseId);
                output.Add(course.ToString() + " | " + cc.ShowCourseStatus());
            }
            return output;
        }

        public List<string> GetCourses(SchoolClass schoolClass)
        {
            return GetCourses(schoolClass.SchoolClassId);
        }

        public List<ClassCourse> GetClassCourses(SchoolClass schoolClass)
        {
            return ClassCourseTable.Where(c => c.ClassId == schoolClass.SchoolClassId).OrderBy(c => c.StartDate).ToList();
        }

        /// <summary>
        /// Get the classes within an education with educationId.
        /// </summary>
        /// <param name="educationId"></param>
        /// <returns></returns>
        public List<SchoolClass> GetClasses(int educationId)
        {
            var classes = SchoolClasses.Where(s => s.EducationId == educationId).ToList();
            return classes;
        }

        public List<SchoolClass> GetClasses(Education education)
        {
            return GetClasses(education.EducationId);
        }
        
        /// <summary>
        /// Get students from a class with classId.
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public List<Student> GetStudents(int classId)
        {
            var students = Students.Where(s => s.ClassId == classId).ToList();
            return students;
        }

        /// <summary>
        /// Get students from a class with schoolClass.
        /// </summary>
        /// <param name="schoolClass"></param>
        /// <returns></returns>
        public List<Student> GetStudents(SchoolClass schoolClass)
        {
            return GetStudents(schoolClass.SchoolClassId);
        }

    }
}
