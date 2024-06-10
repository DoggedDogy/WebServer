using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer.Controllers
{

    [Authorize(Roles = "Преподаватель")]
    public class TeacherController : Controller
    {
        private readonly AppDbContext _context;

        public TeacherController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Teacher/Labs
        public async Task<IActionResult> LabsAsync()
        {
            return View(await _context.Labs.ToListAsync());
        }

        // GET: Teacher/Journal
        public async Task<IActionResult> JournalAsync()
        {
            var students = await _context.Students.ToListAsync();
            var users = await _context.Users.ToListAsync();
            List<StudData> data = new List<StudData>();
            foreach (var stud in students)
            {
                data.Add(new StudData(stud.Group_Id, users.FirstOrDefault(x => x.User_Id == stud.User_Id).Name));
            }
            ViewBag.StudData = data.OrderBy(x => x.Name);
            ViewBag.Labs = await _context.Labs.ToListAsync();
            return View(await _context.Labworks.ToListAsync());
        }

        // GET: Teacher/AddLab
        public async Task<IActionResult> AddLabAsync()
        {
            return View();
        }

        // POST: Teacher/AddLab
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLab([Bind("Lab_Name,Steps,Description")] Lab lab)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lab);
                await _context.SaveChangesAsync();
            }
            return Redirect("/Teacher/Labs");
        }
    }
}
