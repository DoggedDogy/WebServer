using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebServer;
using WebServer.Models;
using System.Security.Claims;
using System.Diagnostics;

namespace WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabworksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LabworksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Labworks
        [HttpGet]
        public async Task<List<Lab>> GetLabwork(string userId)
        {
            Debug.WriteLine("Prikol");
            var labs = await _context.Labs.ToListAsync();
            var labwork = await _context.Labworks.ToListAsync();
            var students = await _context.Students.ToListAsync();
            bool state = false;
            bool proxy = true;
            List<Lab> infos = new List<Lab>();
            foreach (var lab in labs)
            {
                var prevIndex = labwork.FindIndex(x => x.Lab_Id == lab.Lab_Id && students.FirstOrDefault(y => y.Student_Id == x.Student_Id).User_Id == userId);
                if (prevIndex > -1)
                    if (labwork[prevIndex].Done_Steps / lab.Steps > 0.5F)
                        state = true;
                    else
                        state = false;
                else
                {
                    state = proxy;
                    if (!proxy)
                        break;
                }
                proxy = false;
                if (state)
                    infos.Add(lab);
            }

            return infos;
        }
        [HttpPost]
        public async Task<IActionResult> LoginIn([FromBody] LoginModel loginModel)
        {
            Debug.WriteLine(loginModel.Login);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == loginModel.Login && u.Password == loginModel.Password);

            Debug.WriteLine("Prikol");
            if (user != null)
            {
                
                var userClaim = await _context.UserRoleClaims.FirstOrDefaultAsync(u => u.User_Id == user.User_Id);
                var userRole = await _context.Roles.FirstOrDefaultAsync(u => u.Role_Id == userClaim.Role_Id);
                // Аутентификация прошла успешно, устанавливаем cookie с идентификатором пользователя
                // и перенаправляем на защищенную страницу
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.PrimarySid, user.User_Id),
                    new Claim(ClaimTypes.Role, userRole.Role_Name)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10) // Например, устанавливаем срок действия cookie в 1 час
                    
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                var student = await _context.Students.FirstOrDefaultAsync(u => u.User_Id == user.User_Id);

                Debug.WriteLine("Prikol");
                return Ok(new { User_Id = user.User_Id, Student_Id = student.Student_Id, Name = user.Name});
            }
            else
            {
                return Unauthorized();
            }
        }
        // GET: api/Labworks/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Labwork>> GetLabwork(string id)
        //{
        //    var labwork = await _context.Labworks.FindAsync(id);

        //    if (labwork == null)
        //    {
        //        return NotFound();
        //    }

        //    return labwork;
        //}

        // PUT: api/Labworks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLabwork(string id, [FromBody] WorkData data)
        {
            Labwork labwork = await _context.Labworks.FirstOrDefaultAsync(x => x.Work_Id == id);
            labwork.Student_Id = data.Student_Id;
            labwork.Lab_Id = data.Lab_Id;
            labwork.Done_Steps = data.Done_Steps;
            labwork.Finished = data.Finished;
            _context.Entry(labwork).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LabworkExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { User_Id = labwork.Work_Id });
        }

        // POST: api/Labworks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutLabwork(WorkData data)
        {
            var prevWork = await _context.Labworks.FirstOrDefaultAsync(u => u.Lab_Id == data.Lab_Id && u.Student_Id == data.Student_Id);
            if (prevWork != null)
            {
                _context.Labworks.Remove(prevWork);
                await _context.SaveChangesAsync();
            }
            var lab = await _context.Labs.FirstOrDefaultAsync(u => u.Lab_Id == data.Lab_Id);
            Labwork labwork = new Labwork();
            labwork.Student_Id = data.Student_Id;
            labwork.Lab_Id = data.Lab_Id;
            labwork.Done_Steps = data.Done_Steps;
            labwork.Finished = lab.Steps <= data.Done_Steps;

            _context.Labworks.Add(labwork);
            await _context.SaveChangesAsync();

            return Ok(new { User_Id = labwork.Work_Id });
        }

        private bool LabworkExists(string id)
        {
            return _context.Labworks.Any(e => e.Work_Id == id);
        }
    }
}
