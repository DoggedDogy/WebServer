using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebServer;
using WebServer.Models;

namespace WebServer.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Student
        public async Task<IActionResult> Index()
        {
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Name == User.Identity.Name);
            var stud = await _context.Students.FirstOrDefaultAsync(m => m.User_Id == user.User_Id);
            List<LabworkData> labworks = new List<LabworkData>();
            foreach (var item in await _context.Labworks.Where(l => l.Student_Id == stud.Student_Id).ToListAsync())
            {
                labworks.Add(new LabworkData(await _context.Labs.FirstOrDefaultAsync(x => x.Lab_Id == item.Lab_Id), item));
            }
            return View(labworks);
        }

        // GET: Student/Labs
        public async Task<IActionResult> Labs()
        {
            return View(await _context.Labs.OrderBy(m => m.Lab_Name).ToListAsync());
        }
    }
}
