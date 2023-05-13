using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using User_Story.Data;
using User_Story.Models;

namespace User_Story.Controllers
{
    public class FilesController : Controller
    {
        private readonly Context _context;
        Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;

        public FilesController(Context context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _context = context;
            Environment = environment;
        }

        // GET: Files
        public async Task<IActionResult> Index(string? sortByMod, string? sortByGame, string? sortByDate, string? gameVersion)
        {

            var context = _context.Files.Include(f => f.User).ToList();

            if (sortByMod != null)
            {
                await Console.Out.WriteLineAsync("OKOKOKOKKOK");
                context = context.OrderByDescending(f => f.ModeVesrion).ToList();
            }
            if (sortByGame != null)
            {
                context = context.OrderByDescending(f => f.GameVersion).ToList();
            }
            if (sortByDate != null)
            {
                context = context.OrderByDescending(f => f.UploadDate).ToList();
            }
            if(gameVersion != null)
            {
                context = context.Where(f => f.GameVersion.Contains(gameVersion)).ToList();
            }

            return View(context.ToList());

        }

        // GET: Files/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var files = await _context.Files
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (files == null)
            {
                return NotFound();
            }

            return View(files);
        }

        // GET: Files/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID");
            return View();
        }

        public IActionResult Download(int? id)
        {
            Files files = _context.Files.First(x => x.ID == id);
            var filepath = Path.Combine(this.Environment.WebRootPath, "Files/"+files.Title, files.Title);
            return File(System.IO.File.ReadAllBytes(filepath), "file/" + System.IO.Path.GetExtension(filepath), System.IO.Path.GetFileName(filepath));
        }

        // POST: Files/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ModeVesrion,GameVersion")] Files files, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {

                    string wwwPath = this.Environment.WebRootPath;
                    string contentPath = this.Environment.ContentRootPath;

                    string path = Path.Combine(this.Environment.WebRootPath, "Files/" + Path.GetFileName(file.FileName));
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fileName = Path.GetFileName(file.FileName);
                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    files.Title = fileName;
                    files.Path = path.Replace("/","\\")+ "\\" + fileName;
                    files.User = _context.Users.First(x => x.Login.Equals(HttpContext.User.Identity.Name));
                    files.UploadDate = DateTime.Now;
                  
                }
                _context.Add(files);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", files.UserID);
            return View(files);
        }

        // GET: Files/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var files = await _context.Files.FindAsync(id);
            if (files == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", files.UserID);
            return View(files);
        }

        // POST: Files/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ModeVesrion,GameVersion")] Files files, IFormFile file)
        {
            if (id != files.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (file != null)
                    {

                        string wwwPath = this.Environment.WebRootPath;
                        string contentPath = this.Environment.ContentRootPath;

                        string path = Path.Combine(this.Environment.WebRootPath, "Files/" + Path.GetFileName(file.FileName));
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        string fileName = Path.GetFileName(file.FileName);
                        using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        files.Title = fileName;
                        files.Path = path.Replace("/", "\\") + "\\" + fileName;
                        files.User = _context.Users.First(x => x.Login.Equals(HttpContext.User.Identity.Name));
                        files.UploadDate = DateTime.Now;

                    }

                    _context.Update(files);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilesExists(files.ID))
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
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", files.UserID);
            return View(files);
        }

        // GET: Files/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var files = await _context.Files
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (files == null)
            {
                return NotFound();
            }

            return View(files);
        }

        // POST: Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var files = await _context.Files.FindAsync(id);
            _context.Files.Remove(files);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilesExists(int id)
        {
            return _context.Files.Any(e => e.ID == id);
        }
    }
}
