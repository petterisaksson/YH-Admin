﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YH_Admin.Model;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace YH_AdminTest
{
    [TestClass]
    public class TestSchool
    {
        [TestMethod]
        public void TestGetClasses()
        {
            //Arrangeare
            School sc = new School();
            string soluPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            sc.LoadData(Path.Combine(soluPath, "YH-Admin"));

            List<SchoolClass> schoolClassListExpected = new List<SchoolClass>();
            SchoolClass scc = new SchoolClass(0, "SU13", 0, new DateTime(2013, 09, 01), new DateTime(2015, 05, 30));
            schoolClassListExpected.Add(new SchoolClass(0, "SU13", 0, new DateTime(2013, 09, 01), new DateTime(2015, 05, 30)));
            schoolClassListExpected.Add(new SchoolClass(1, "SU14", 0, new DateTime(2014, 09, 01), new DateTime(2016, 05, 30)));
            schoolClassListExpected.Add(new SchoolClass(2, "SU15", 0, new DateTime(2015, 09, 01), new DateTime(2017, 05, 30)));

            //Act
            List<SchoolClass> schoolClassListActual = sc.GetClasses(0);

            //Assert
            CollectionAssert.AreEqual(schoolClassListExpected, schoolClassListActual);
        }

        [TestMethod]
        public void TestGetClassesEdu()
        {
            School sc = new School();
            string soluPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            sc.LoadData(Path.Combine(soluPath, "YH-Admin"));

            List<SchoolClass> schoolClassListExpected = new List<SchoolClass>();
            SchoolClass scc = new SchoolClass(0, "SU13", 0, new DateTime(2013, 09, 01), new DateTime(2015, 05, 30));
            schoolClassListExpected.Add(new SchoolClass(0, "SU13", 0, new DateTime(2013, 09, 01), new DateTime(2015, 05, 30)));
            schoolClassListExpected.Add(new SchoolClass(1, "SU14", 0, new DateTime(2014, 09, 01), new DateTime(2016, 05, 30)));
            schoolClassListExpected.Add(new SchoolClass(2, "SU15", 0, new DateTime(2015, 09, 01), new DateTime(2017, 05, 30)));

            //Act
            List<SchoolClass> schoolClassListActual = sc.GetClasses(new Education(0, "Fantasiutb", 1));

            //Assert
            CollectionAssert.AreEqual(schoolClassListExpected, schoolClassListActual);
        }

        [TestMethod]
        public void TestGetClassCourses()
        {
            School sc = GetTestSchool();

            List<ClassCourse> expectedList = new List<ClassCourse>();
            expectedList.Add(new ClassCourse(0, 0, 0, new DateTime(2014, 09, 01), new DateTime(2014, 09, 30)));
            expectedList.Add(new ClassCourse(1, 1, 1, new DateTime(2014, 08, 01), new DateTime(2014, 08, 31)));

            List<ClassCourse> actualList = sc.GetClassCourses(new SchoolClass(1, "SU14", 0, new DateTime(2014, 09, 01), new DateTime(2016, 05, 30)));

            CollectionAssert.AreNotEqual(expectedList, actualList);
        }

        [TestMethod]
        public void TestGetClassCourses1()
        {
            School sc = GetTestSchool();

            List<ClassCourse> expectedList = new List<ClassCourse>();
            expectedList.Add(new ClassCourse(1, 1, 1, new DateTime(2014, 08, 01), new DateTime(2014, 08, 31)));

            List<ClassCourse> actualList = sc.GetClassCourses(new SchoolClass(1, "SU14", 0, new DateTime(2014, 08, 01), new DateTime(2016, 08, 31)));

            CollectionAssert.AreEqual(expectedList, actualList);
        }

        [TestMethod]
        public void TestGetEducations()
        {
            var expectedList = new List<Education>() { new Education(1, "Webutveckling Agila Webprogrammering", 1) };

            School school = new School();
            string soluPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            school.LoadData(Path.Combine(soluPath, "YH-Admin"));

            var actualList = school.GetEducations(1);

            CollectionAssert.AreEqual(expectedList, actualList);
        }


        [TestMethod]
        public void TestGetEducations2()
        {
            var expectedList = new List<Education>() { new Education(1, "Webutveckling Agila Webprogrammering", 1) };

            School school = new School();
            string soluPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            school.LoadData(Path.Combine(soluPath, "YH-Admin"));

            var actualList = school.GetEducations(1);

            CollectionAssert.AreEqual(expectedList, actualList);
        }

        [TestMethod]
        public void TestGetStudents()
        {
            //Arrenge
            School school = GetTestSchool();
            var expectedList = new List<Student>() { new Student(0, "Allan", "Allansson", 0), new Student(1, "Billy", "Butt", 0) };

            //Act
            var actualList = school.GetStudents(0);

            //Assert
            CollectionAssert.AreEqual(expectedList, actualList);
        }

        [TestMethod]
        public void TestGetCourses()
        {
            //Arrenge
            School gc = GetTestSchool();
            List<string> expectedList = new List<string>();
            expectedList.Add("CourseId: 0, Name: Första Course | 20140901->20140930 Status: Avslutat");
            //Act
            var actualList = gc.GetCourses(0);
            //Assert
            CollectionAssert.AreEqual(expectedList, actualList);
        }

        School GetTestSchool()
        {
            School school = new School();

            school.Educations.Add(new Education(0, "Första education", 0));
            school.Educations.Add(new Education(1, "Andra education", 1));
            school.Courses.Add(new Course(0, "Första Course"));
            school.Courses.Add(new Course(1, "Andra Course"));
            school.Courses.Add(new Course(2, "Tredje Kursen"));
            school.SchoolClasses.Add(new SchoolClass(0, "Första schoolclasses", 0, new DateTime(2014, 09, 01), new DateTime(2014, 09, 30)));
            school.SchoolClasses.Add(new SchoolClass(1, "Andra schoolclasses", 0, new DateTime(2014, 08, 01), new DateTime(2014, 08, 31)));
            school.Students.Add(new Student(0, "Allan", "Allansson", 0));
            school.Students.Add(new Student(1, "Billy", "Butt", 0));
            school.Staffs.Add(new Staffing(0, "Magister", "Svensson"));
            school.Staffs.Add(new Staffing(1, "Fröken", "Ur"));
            school.EducationCourses.Add(new EducationCourse(0, 0, 0));
            school.EducationCourses.Add(new EducationCourse(1, 1, 1));
            school.ClassCourseTable.Add(new ClassCourse(0, 0, 0, new DateTime(2014, 09, 01), new DateTime(2014, 09, 30), 0));
            school.ClassCourseTable.Add(new ClassCourse(1, 1, 1, new DateTime(2014, 08, 01), new DateTime(2014, 08, 31), 1));
            school.ClassCourseTable.Add(new ClassCourse(2, 2, 0, new DateTime(2015, 08, 01), new DateTime(2015, 08, 31), 0));
            school.ClassCourseTable.Add(new ClassCourse(3, 2, 1, new DateTime(2015, 08, 01), new DateTime(2015, 08, 31), 1));
            school.ClassCourseTable.Add(new ClassCourse(4, 2, 2, new DateTime(2015, 09, 01), new DateTime(2015, 09, 30)));
            school.Grades.Add(new Grade(0, 0, "IG"));
            school.Grades.Add(new Grade(1, 1, "G"));
            school.Users.Add(new User(0, "Tina", "Tina1", "Tina", "Kraft"));
            

            return school;
        }

        [TestMethod]
        public void TestGetCourses2()
        {

            School gc = GetTestSchool();
            List<string> expectedList = new List<string>();
            expectedList.Add("CourseId: 0, Name: Första Course | 20140901->20140930 Status: Avslutat");

            var actualList = gc.GetCourses(0);

            CollectionAssert.AreEqual(expectedList, actualList);
        }

        [TestMethod]
        public void TestGetClassCoursesStu()
        {
            School sc = GetTestSchool();

            List<ClassCourse> expectedList = new List<ClassCourse>();
            expectedList.Add(new ClassCourse(0, 0, 0, new DateTime(2014, 09, 01), new DateTime(2014, 09, 30)));

            List<ClassCourse> actualList = sc.GetClassCourses(new Student(0, "Allan", "Allansson", 0));

            CollectionAssert.AreEqual(expectedList, actualList);
        }

        [TestMethod]
        public void TestGetGrade()
        {
            School sc = GetTestSchool();
            var expectedGrade = new Grade(101, 0, 0, "G");

            sc.Grades.Add(expectedGrade);
            sc.Grades.Add(new Grade(101, 1, 1, "VG")); // Medvetet att betyg är olikt från expected för att vilkor för lika är studentId och classCourseId


            var actualGrade = sc.GetGrade(new Student(0, "Allan", "Allansson", 0), new ClassCourse(0, 0, 0, new DateTime(2014, 09, 01), new DateTime(2014, 09, 30)));

            Assert.AreEqual(expectedGrade, actualGrade );
        }

        [TestMethod]
        public void TestGetCoursesWithoutTeacher()
        {
            School sc = GetTestSchool();
            var expectedList = new List<ClassCourse>() { new ClassCourse(4, 2, 2, new DateTime(2015, 09, 01), new DateTime(2015, 09, 30), 2) };
            var actualList = sc.GetCoursesWithoutTeacher();

            CollectionAssert.AreEqual(expectedList, actualList);
        }

        [TestMethod]
        public void TestGetFailers()
        {
            School sc = GetTestSchool();
            List<Student> expectedList = new List<Student>();
            expectedList.Add(new Student(0, "Allan", "Allansson", 0));

            List<Student> actualList = sc.GetFailers();

            CollectionAssert.AreEqual(expectedList, actualList);
        }
    }
}
