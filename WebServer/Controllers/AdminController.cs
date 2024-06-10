using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebServer;
using WebServer.Models;

namespace WebServer.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            var claims = await _context.UserRoleClaims.ToListAsync();
            List<UserData> data = new List<UserData>();
            foreach (var user in users)
            {
                data.Add(new UserData(user, claims.Find(x => x.User_Id == user.User_Id)));
            }
            return View(data);
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.User_Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Admin/Create
        public async Task<IActionResult> CreateAsync()
        {
            var roles = await _context.Roles.ToListAsync();
            ViewBag.RoleList = new SelectList(roles, "Role_Id", "Role_Name");
            var groups = await _context.Groups.ToListAsync();
            ViewBag.GroupList = new SelectList(groups, "Group_Id", "Group_Number", null, "Year");
            ViewBag.Claim = new UserRoleClaim();
            ViewBag.Student = new Student();
            if (roles == null)
            {
                return NotFound();
            }
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Login,Password,Email,Active")] User user, UserRoleClaim claim, Student student)
        {
            if (ModelState.IsValid)
            {
                user.Active = false;
                _context.Add(user);
                await _context.SaveChangesAsync();
                claim.User_Id = user.User_Id;
                _context.Add(claim);
                await _context.SaveChangesAsync();
                if (claim.Role_Id == 1)
                {
                    await _context.SaveChangesAsync();
                    student.User_Id = user.User_Id;
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                }
                else if (claim.Role_Id == 2)
                {
                    // Создаем запись в таблице Profs только для преподавателей
                    Prof prof = new Prof
                    {
                        User_Id = user.User_Id,
                        // Другие свойства, если есть
                    };
                    _context.Add(prof);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/Groups
        public async Task<IActionResult> Groups()
        {
            var groups = await _context.Groups.ToListAsync();
            var teaches = await _context.Profs.ToListAsync();
            var users = await _context.Users.ToListAsync();
            List<GroupData> data = new List<GroupData>();
            foreach (var group in groups)
            {
                data.Add(new GroupData(group, users.Find(y => y.User_Id == teaches.Find(x => x.Teacher_Id == group.Teacher_Id).User_Id).Name));
            }
            return View(data);
        }

        // GET: Admin/CreateGroup
        public async Task<IActionResult> CreateGroupAsync()
        {
            var teachers = await _context.Profs.ToListAsync();
            List<ProfData> TeacherList = new List<ProfData>();
            foreach (var teacher in teachers)
            {
                TeacherList.Add(new ProfData(teacher.Teacher_Id, _context.Users.FirstOrDefaultAsync(m => m.User_Id == teacher.User_Id).Result.Name));
            }
            ViewBag.TeacherList = new SelectList(TeacherList, "Teacher_Id", "Teacher_Name");
            return View();
        }

        // POST: Admin/CreateGroup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGroup([Bind("Group_Number,Teacher_Id,Year,Specialty")] Group group)
        {
            if (ModelState.IsValid)
            {
                _context.Add(group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Groups));
            }
            return View();
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("User_Id,Name,Login,Password,Email,Active,Role_Id")] User user)
        {
            if (id != user.User_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.User_Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.User_Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.User_Id == id);
        }
    }
}
