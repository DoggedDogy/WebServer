using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Models
{
    public class LoginModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
    public class RoleData
    {
        public RoleData(string user_Id, string role_Name)
        {
            User_Id = user_Id;
            Role_Name = role_Name;
        }

        public string User_Id { get; set; }
        public string Role_Name { get; set; }
    }
    public class GroupData
    {
        public GroupData(Group group, string name)
        {
            Group_Id = group.Group_Id;
            Group_Number = group.Group_Number;
            Teacher_Name = name;
            Year = group.Year;
            Specialty = group.Specialty;
        }

        public string Group_Id { get; set; }
        public string Group_Number { get; set; }
        public string Teacher_Name { get; set; }
        public int Year { get; set; }
        public string Specialty { get; set; }
    }
    public class UserData
    {
        public UserData(User user, UserRoleClaim claim)
        {
            User_Id = user.User_Id;
            if (claim.Role_Id == 1) Role_Name = "Студент";
            else if (claim.Role_Id == 2) Role_Name = "Преподаватель";
            else if (claim.Role_Id == 3) Role_Name = "Администратор";
            Name = user.Name;
            Login = user.Login;
            Password = user.Password;
            Email = user.Email;
            Active = user.Active;
        }

        public string User_Id { get; set; }
        public string Role_Name { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
    }
    public class ProfData
    {
        public ProfData(string id, string name)
        {
            Teacher_Id = id;
            Teacher_Name = name;
        }
        public string Teacher_Id { get; set; }
        public string Teacher_Name { get; set; }
    }
    public class StudData
    {
        public StudData(string id, string name)
        {
            Student_Id = id;
            Name = name;
        }
        public string Student_Id { get; set; }
        public string Name { get; set; }
    }
    public class StudentInfo
    {
        public string User_Id { get; set; }
        public string Student_Id { get; set; }
        public string Name { get; set; }
    }
    public class WorkData
    {
        public string Lab_Id { get; set; }
        public string Student_Id { get; set; }
        public int Done_Steps { get; set; }
        public bool Finished { get; set; }
    }
    public class LabworkData
    {
        public LabworkData(Lab lab, Labwork work)
        {
            Lab_Name = lab.Lab_Name;
            Done_Steps = "шаг " + work.Done_Steps + "/" + lab.Steps;
            if (work.Mark != 0)
                Mark = work.Mark.ToString();
        }

        public string Lab_Name { get; set; }
        public string Done_Steps { get; set; }
        public string Mark { get; set; }
    }
}
