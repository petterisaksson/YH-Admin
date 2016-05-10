﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YH_Admin.Model;
using static YH_Admin.View.ConsoleOutput;

namespace YH_Admin.View
{
    class ConsoleController
    {
        School Model { get; set; }

        ConsoleOutput View { get; set; }

        delegate void DelMenu();

        Stack<DelMenu> PreviousMenus { get; set; }

        User CurrentUser { get; set; }

        List<Education> CurrentEducations { get; set; }

        List<SchoolClass> CurrentClasses { get; set; }

        List<Student> CurrentStudents { get; set; }

        List<string> CurrentCourses { get; set; }

        List<ClassCourse> CurrentClassCourses { get; set; }

        Student CurrentStudent { get; set; }

        ClassCourse CurrentClassCourse { get; set; }

        //List<StaffingCourse> CurrentStaffingCourses { get; set; } 

        

        /// <summary>
        /// Constructor to set up Model and View.
        /// </summary>
        /// <param name="school"></param>
        /// <param name="output"></param>
        public ConsoleController(School school, ConsoleOutput output)
        {
            Model = school;
            View = output;
            PreviousMenus = new Stack<DelMenu>();
        }

        public void ShowWelcomeScreen()
        {
            View.Titles.Push("Inloggning till Yh-Admin");
            View.ChoiceHandler = HandleLogIn;
            View.ShowLogIn();
        }

        public void HandleLogIn(string choice)
        {
            var splits = choice.Split('\n');
            if (splits.Length == 2)
            {
                var user = Model.Users.Find(u => u.Username.Equals(splits[0]) && u.PassWord.Equals(splits[1]));
                if (user != null)
                {
                    CurrentUser = user;
                    ShowMainMenu();
                    return;
                }
            }
            ShowWelcomeScreen();

        }

        private void GoBack()
        {
            View.Message = "";
            View.Titles.Pop();
            var p = PreviousMenus.Pop();
            p();
        }

        /// <summary>
        /// Show the main menu.
        /// </summary>
        public void ShowMainMenu()
        {
            PreviousMenus.Clear();
            View.Message = "";
            View.Titles.Clear();
            View.Titles.Push($"Huvudmeny - {CurrentUser.Name}");

            var table = new string[6, 1];
            table[0, 0] = "Kategorier";
            table[1, 0] = "Utbildning";
            table[2, 0] = "Klasser";
            table[3, 0] = "Kurser";
            table[4, 0] = "Studerande";
            table[5, 0] = "Bemanning";

            View.ChoiceHandler = HandleMainMenuChoice;
            View.ShowTableAndWaitForChoice(table, isMainMenu: true);
        }

        /// <summary>
        /// Handle the choices from the main menu.
        /// </summary>
        /// <param name="choice"></param>
        private void HandleMainMenuChoice(string choice)
        {
            switch (choice)
            {
                case "1":
                    PreviousMenus.Push(ShowMainMenu);
                    CurrentEducations = Model.GetEducations(CurrentUser);
                    ShowCurrentEducation();
                    break;
                case "2":
                    PreviousMenus.Push(ShowMainMenu);
                    ShowClassMenu();
                    break;
                case "3":
                    PreviousMenus.Push(ShowMainMenu);
                    ShowCourseMenu();
                    break;
                case "4":
                    PreviousMenus.Push(ShowMainMenu);
                    ShowStudentGrade();
                    break;
                case "5":
                    PreviousMenus.Push(ShowMainMenu);
                    ShowRecruitmentMenu();
                    break;
                case "x":
                    Model.SaveToFiles();
                    return;
                default:
                    ShowMainMenu();
                    break;
            }
        }

        private void ShowStudentGrade()
        {
            View.Titles.Push($"Studentmeny");

            var table = new string[4, 1];
            table[0, 0] = "Kategorier";
            table[1, 0] = "Visa alla studerande";
            table[2, 0] = "Visa studerande i en viss klass";
            table[3, 0] = "Visa ej godkända studenter";

            View.ChoiceHandler = HandleStudentGradeChoice;
            View.ShowTableAndWaitForChoice(table);
        }

        private void HandleStudentGradeChoice(string choice)
        {
            switch (choice)
            {
                case "1":
                    PreviousMenus.Push(ShowStudentGrade);
                    CurrentStudents = Model.GetStudents();
                    ShowStudentMenu();
                    break;
                case "2":
                    PreviousMenus.Push(ShowStudentGrade);
                    ShowStudentInClassMenu();
                    break;
                //case "3":
                //    //TODO
                //    break;
                case "x":
                    GoBack();
                    return;
                case "h":
                    ShowMainMenu();
                    return;
                default:
                    ShowStudentGrade();
                    break;
            }
        }

        private void ShowStudentMenu()
        {
            View.Titles.Push($"Alla studerande");
            var table = new string[CurrentStudents.Count + 2, 3];
            table[0, 0] = "Förnamn";
            table[0, 1] = "Efternamn";
            table[0, 2] = "Klass";
            for (int i = 0; i < CurrentStudents.Count; i++)
            {
                table[i + 1, 0] = CurrentStudents[i].FirstName;
                table[i + 1, 1] = CurrentStudents[i].LastName;
                table[i + 1, 2] = Model.SchoolClasses.Find(sc => sc.SchoolClassId == CurrentStudents[i].ClassId).Name;
            }
            View.Message = $"Välj en student för att se dennes studieresultat\nTryck {CurrentStudents.Count + 1} för att lägga till en ny student.";
            View.ChoiceHandler = HandleStudentMenuChoice;
            View.ShowTableAndWaitForChoice(table);
        }

        private void HandleStudentMenuChoice(string choice)
        {
            if (choice.Equals("x"))
            {
                GoBack();
                return;
            }
            if (choice.Equals("h"))
            {
                ShowMainMenu();
                return;
            }
            int index;
            if (int.TryParse(choice, out index))
            {
                if (index > 0 && index <= CurrentStudents.Count)
                {
                    PreviousMenus.Push(ShowStudentMenu);
                    CurrentStudent = CurrentStudents[index - 1];
                    View.Titles.Push($"Kurser som läses av {CurrentStudent.Name}");
                    CurrentClassCourses = Model.GetClassCourses(CurrentStudent);
                    ShowCurrentClassCoursesStudent();
                    return;
                }
                else if (index == CurrentStudents.Count + 1)
                {
                    PreviousMenus.Push(ShowStudentMenu);
                    View.Titles.Push($"Lägg till en ny student");
                    View.ChoiceHandler = HandleAddStudent;
                    View.ShowAddStudent(Model.SchoolClasses.Select(c => c.Name).ToArray());
                    return;
                }
            }
            ShowStudentMenu();
        }

        private void HandleAddStudent(string choice)
        {
            var splits = choice.Split('\n');
            if (splits.Length == 3)
            {
                var classId = Model.GetClassId(splits[2]);
                if (classId != null)
                {
                    Model.AddStudents(new Student(splits[0], splits[1], (int)classId));
                    CurrentStudents = Model.GetStudents();
                    GoBack();
                }
            }
        }

        private void ShowRecruitmentMenu()
        {
           // var tabel = new string[CurrentStraffingCourse.count + 1, 3];
            // TODO: Implement här ska det vara.
            /*Psuedo kod:
                Lista alla kurser
                kolla om kursen har en lärare i Staffingcourses
                formatera informationen till en 2-dimensionell matris
                Anropa ConsoleOutput.ShowTableAndWaitForChoice (...) för att visa det på skärmen.

                tror det räcker så :)


            */

            Console.Clear();
            Console.WriteLine("ShowRecruitmentMenu not implemented\nPress any key to return.");
            Console.ReadLine();
            ShowMainMenu();
        }

        private void ShowCurrentEducation()
        {
            View.Titles.Push($"Utbildningar ansvariga av {CurrentUser.Name}");
            var table = new string[CurrentEducations.Count + 1, 1];
            table[0, 0] = "Namn";
            for (int i = 0; i < CurrentEducations.Count; i++)
            {
                table[i + 1, 0] = CurrentEducations[i].Name;
            }
            View.ChoiceHandler = HandleCurrentEducations;
            View.ShowTableAndWaitForChoice(table);
        }

        private void HandleCurrentEducations(string choice)
        {
            if (choice.Equals("x"))
            {
                GoBack();
                return;
            }
            if (choice.Equals("h"))
            {
                ShowMainMenu();
                return;
            }
            int index;
            if (int.TryParse(choice, out index))
            {
                if (index > 0 && index <= CurrentEducations.Count)
                {
                    PreviousMenus.Push(ShowCurrentEducation);
                    var chosen = CurrentEducations[index - 1];
                    View.Titles.Push($"Klasser i {chosen.Name}");
                    CurrentClasses = Model.GetClasses(chosen);
                    ShowCurrentClasses();
                    return;
                }
            }
            ShowCurrentEducation();

        }

        private void ShowClassMenu()
        {
            View.Titles.Push($"Visa klasser i en viss utbildning");
            CurrentEducations = Model.Educations;
            var table = new string[CurrentEducations.Count + 1, 1];
            table[0, 0] = "Namn";
            for (int i = 0; i < CurrentEducations.Count; i++)
            {
                table[i + 1, 0] = CurrentEducations[i].Name;
            }

            View.ChoiceHandler = HandleClassMenu;
            View.ShowTableAndWaitForChoice(table);
        }

        private void HandleClassMenu(string choice)
        {
            if (choice.Equals("x"))
            {
                GoBack();
                return;
            }
            if (choice.Equals("h"))
            {
                ShowMainMenu();
                return;
            }
            int index;
            if (int.TryParse(choice, out index))
            {
                if (index > 0 && index <= CurrentEducations.Count)
                {
                    PreviousMenus.Push(ShowClassMenu);
                    var chosen = CurrentEducations[index - 1];
                    View.Titles.Push($"Klasser i {chosen.Name}");
                    CurrentClasses = Model.GetClasses(chosen);
                    ShowCurrentClasses();
                    return;
                }
            }
            ShowClassMenu();
        }

        private void ShowCourseMenu()
        {
            View.Titles.Push($"Visa kurser som läses av en viss klass");
            CurrentClasses = Model.SchoolClasses;
            var table = new string[CurrentClasses.Count + 1, 1];
            table[0, 0] = "Namn";
            for (int i = 0; i < CurrentClasses.Count; i++)
            {
                table[i + 1, 0] = CurrentClasses[i].Name;
            }

            View.ChoiceHandler = HandleCourseMenu;
            View.ShowTableAndWaitForChoice(table);
        }

        private void HandleCourseMenu(string choice)
        {
            if (choice.Equals("x"))
            {
                GoBack();
                return;
            }
            if (choice.Equals("h"))
            {
                ShowMainMenu();
                return;
            }
            int index;
            if (int.TryParse(choice, out index))
            {
                if (index > 0 && index <= CurrentClasses.Count)
                {
                    PreviousMenus.Push(ShowCourseMenu);
                    var chosen = CurrentClasses[index - 1];
                    View.Titles.Push($"Kurser som läses av {chosen.Name}");
                    CurrentClassCourses = Model.GetClassCourses(chosen);
                    ShowCurrentClassCourses();
                    return;
                }
            }
            ShowCourseMenu();
        }

        private void ShowCurrentClassCourses()
        {
            var table = new string[CurrentClassCourses.Count + 1, 4];
            table[0, 0] = "Namn";
            table[0, 1] = "Startdatum";
            table[0, 2] = "Slutdatum";
            table[0, 3] = "Status";
            for (int i = 0; i < CurrentClassCourses.Count; i++)
            {
                table[i + 1, 0] = Model.Courses.Find(c => c.CourseId == CurrentClassCourses[i].CourseId).Name;
                table[i + 1, 1] = CurrentClassCourses[i].StartDateString;
                table[i + 1, 2] = CurrentClassCourses[i].EndDateString;
                table[i + 1, 3] = CurrentClassCourses[i].Status;
            }
            View.ChoiceHandler = HandleShowCurrentClassCourses;
            View.ShowTableAndWaitForChoice(table, choosable: false);
        }

        private void HandleShowCurrentClassCourses(string choice)
        {
            if (choice.Equals("x"))
            {
                GoBack();
                return;
            }
            if (choice.Equals("h"))
            {
                ShowMainMenu();
                return;
            }
            ShowCurrentClassCourses();
        }

        private void ShowStudentInClassMenu()
        {
            View.Titles.Push($"Visa studerande i en viss klass");
            CurrentClasses = Model.SchoolClasses;
            var table = new string[CurrentClasses.Count + 1, 1];
            table[0, 0] = "Namn";
            for (int i = 0; i < CurrentClasses.Count; i++)
            {
                table[i + 1, 0] = CurrentClasses[i].Name;
            }

            View.ChoiceHandler = HandleStudentInClassMenuChoice;
            View.ShowTableAndWaitForChoice(table);
        }

        private void HandleStudentInClassMenuChoice(string choice)
        {
            if (choice.Equals("x"))
            {
                GoBack();
                return;
            }
            if (choice.Equals("h"))
            {
                ShowMainMenu();
                return;
            }
            int index;
            if (int.TryParse(choice, out index))
            {
                if (index > 0 && index <= CurrentClasses.Count)
                {
                    PreviousMenus.Push(ShowStudentInClassMenu);
                    var chosen = CurrentClasses[index - 1];
                    View.Titles.Push($"Studerande i {chosen.Name}");
                    CurrentStudents = Model.GetStudents(chosen);
                    ShowCurrentStudents();
                    return;
                }
            }
            ShowStudentInClassMenu();
        }

        private void ShowCurrentClasses()
        {
            var table = new string[CurrentClasses.Count + 1, 3];
            table[0, 0] = "Namn";
            table[0, 1] = "Startdatum";
            table[0, 2] = "Status";

            for (int i = 0; i < CurrentClasses.Count; i++)
            {
                table[i + 1, 0] = CurrentClasses[i].Name;
                table[i + 1, 1] = CurrentClasses[i].StartDateString;
                table[i + 1, 2] = CurrentClasses[i].Status;

            }
            View.ChoiceHandler = HandleShowCurrentClasses;
            View.ShowTableAndWaitForChoice(table);
        }

        private void HandleShowCurrentClasses(string choice)
        {
            if (choice.Equals("x"))
            {
                GoBack();
                return;
            }
            if (choice.Equals("h"))
            {
                ShowMainMenu();
                return;
            }
            int index;
            if (int.TryParse(choice, out index))
            {
                if (index > 0 && index <= CurrentClasses.Count)
                {
                    PreviousMenus.Push(ShowCurrentClasses);
                    var chosen = CurrentClasses[index - 1];
                    View.Titles.Push($"Studerande i {chosen.Name}");
                    CurrentStudents = Model.GetStudents(chosen);
                    ShowCurrentStudents();
                    return;
                }
            }
            ShowCurrentClasses();
        }

        private void ShowCurrentClassCoursesStudent()
        {
            var table = new string[CurrentClassCourses.Count + 1, 5];
            table[0, 0] = "Namn";
            table[0, 1] = "Startdatum";
            table[0, 2] = "Slutdatum";
            table[0, 3] = "Status";
            table[0, 4] = "Betyg";
            for (int i = 0; i < CurrentClassCourses.Count; i++)
            {
                table[i + 1, 0] = Model.Courses.Find(c => c.CourseId == CurrentClassCourses[i].CourseId).Name;
                table[i + 1, 1] = CurrentClassCourses[i].StartDateString;
                table[i + 1, 2] = CurrentClassCourses[i].EndDateString;
                table[i + 1, 3] = CurrentClassCourses[i].Status;
                var grade = Model.GetGrade(CurrentStudent, CurrentClassCourses[i]);
                if (grade != null)
                    table[i + 1, 4] = Model.GetGrade(CurrentStudent, CurrentClassCourses[i]).GradeString;
                else
                    table[i + 1, 4] = "";
            }
            View.Message = "Välj en kurs för att sätta/ändra betyg, om den är avslutad.";
            View.ChoiceHandler = HandleShowCurrentClassCoursesStudent;
            View.ShowTableAndWaitForChoice(table);
            
        }

        private void HandleShowCurrentClassCoursesStudent(string choice)
        {
            if (choice.Equals("x"))
            {
                GoBack();
                return;
            }
            if (choice.Equals("h"))
            {
                ShowMainMenu();
                return;
            }
            int index;
            if (int.TryParse(choice, out index))
            {
                if (index > 0 && index <= CurrentClassCourses.Count)
                {
                    CurrentClassCourse = CurrentClassCourses[index - 1];
                    if (CurrentClassCourse.IsFinished)
                    {
                        PreviousMenus.Push(ShowCurrentClassCoursesStudent);
                        View.Titles.Push("Sätta/ ändra betyg" + "\n" + $"{CurrentStudent.Name}");
                        View.Message = "";
                        ShowCurrentClassCourseMenu();

                        return;
                    }
                }
            }
            ShowCurrentClassCoursesStudent();
        }

        private void ShowCurrentClassCourseMenu()
        {
            var table = new string[2, 6];
            table[0, 0] = "Namn";
            table[0, 1] = "Startdatum";
            table[0, 2] = "Slutdatum";
            table[0, 3] = "Status";
            table[0, 4] = "Betyg";
            table[0, 5] = "Lärare";

            table[1, 0] = Model.Courses.Find(c => c.CourseId == CurrentClassCourse.CourseId).Name;
            table[1, 1] = CurrentClassCourse.StartDateString;
            table[1, 2] = CurrentClassCourse.EndDateString;
            table[1, 3] = CurrentClassCourse.Status;
            table[1, 4] = CurrentClassCourse.StaffingId.ToString(); // inte bara id, utan vi ska lägga in namnet här. men för att det ska kompilera kan vi göra en ToString() först
            var grade = Model.GetGrade(CurrentStudent, CurrentClassCourse);
            if (grade != null)
                table[1, 4] = Model.GetGrade(CurrentStudent, CurrentClassCourse).GradeString + "?";
            else
                table[1, 4] = "?";

            View.Message = "Betyg: 'IG' = icke godkänd, 'G' = godkänd, 'VG' = väl godkänd.";
            View.ChoiceHandler = HandleShowCurrentClassCourseMenu;
            View.ShowTableAndWaitForChoice(table, choosable:false, cursorStr:"Betyg ");
        }

        private void HandleShowCurrentClassCourseMenu(string choice)
        {
            choice = choice.ToUpperInvariant();
            switch (choice)
            {
                case "IG":
                case "G":
                case "VG":
                    Model.SetGrade(CurrentStudent, CurrentClassCourse, choice);
                    CurrentClassCourse = null;
                    GoBack();
                    break;
                case "X":
                    GoBack();
                    return;
                case "H":
                    ShowMainMenu();
                    return;
                default:
                    View.Message = "Tillåtna val: 'X', 'H', 'IG' 'G' 'VG'";
                    ShowCurrentClassCourseMenu();
                    break;
            }
        }

        private void ShowCurrentStudents()
        {
            var table = new string[CurrentStudents.Count + 1, 1];
            table[0, 0] = "Namn";
            for (int i = 0; i < CurrentStudents.Count; i++)
            {
                table[i + 1, 0] = CurrentStudents[i].Name;
            }
            View.ChoiceHandler = HandleShowCurrentStudents;
            View.ShowTableAndWaitForChoice(table, choosable: false);
        }

        private void HandleShowCurrentStudents(string choice)
        {
            if (choice.Equals("x"))
            {
                GoBack();
                return;
            }
            if (choice.Equals("h"))
            {
                ShowMainMenu();
                return;
            }
            int index;
            if (int.TryParse(choice, out index))
            {
                if (index > 0 && index <= CurrentStudents.Count)
                {
                    PreviousMenus.Push(ShowClassMenu);
                    CurrentStudent = CurrentStudents[index];
                    // Visar betyg ?
                    {
                        ShowMainMenu();
                    }
                    return;
                }
            }
            ShowCurrentStudents();
        }
       
        private void ShowFailedStudents()
        {

        }
    }
}
