using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string User_Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
    }
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Group_Id { get; set; }
        public string Group_Number { get; set; }
        public string Teacher_Id { get; set; }
        public int Year { get; set; }
        public string Specialty { get; set; }
    }
    public class Role
    {
        [Key]
        public int Role_Id { get; set; }
        public string Role_Name { get; set; }
    }
    public class Prof
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Teacher_Id { get; set; }
        public string User_Id { get; set; }
    }
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Student_Id { get; set; }
        public string User_Id { get; set; }
        public string Group_Id { get; set; }
    }
    public class UserRoleClaim
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Claim_Id { get; set; }
        public string User_Id { get; set; }
        public int Role_Id { get; set; }
    }
    public class Lab
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Lab_Id { get; set; }
        public string Lab_Name { get; set; }
        public int Steps { get; set; }
        public string Description { get; set; }
    }
    public class Labwork
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Work_Id { get; set; }
        public string Lab_Id { get; set; }
        public string Student_Id { get; set; }
        public int Done_Steps { get; set; }
        public int Mark { get; set; }
        public bool Finished { get; set; }
    }
}
