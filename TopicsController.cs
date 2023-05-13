using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using User_Story.Data;
using User_Story.Models;

namespace User_Story.Controllers
{
    public class TopicsController : Controller
    {
        private readonly Context _context;

        public TopicsController(Context context)
        {
            _context = context;
        }

        // GET: Topics
        public async Task<IActionResult> Index(string? topic)
        {
            var context = _context.Topics.Include(t => t.User).ToList();

            if (topic != null)
            {
                context = context.Where(f => f.Title.Contains(topic)).ToList();
            }

            return View(context.ToList());
        }

        // GET: Topics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.ID == id);

            topic.Messages = _context.Messages.Include(x=>x.User).Where(x => x.TopicID == topic.ID).OrderByDescending(x=>x.CreationDate).ToList();

            if (topic == null)
            {
                return NotFound();
            }

            return View(topic);
        }

        // GET: Topics/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID");
            return View();
        }

        // POST: Topics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title")] Topic topic)
        {
            if (ModelState.IsValid)
            {
                topic.User = _context.Users.First(x => x.Login.Equals(HttpContext.User.Identity.Name));
                topic.CreationDate = DateTime.Now;
                _context.Add(topic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", topic.UserID);
            return View(topic);
        }

        // GET: Topics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", topic.UserID);
            return View(topic);
        }

        // POST: Topics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title")] Topic topic)
        {
            if (id != topic.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    topic.CreationDate = DateTime.Now;
                    _context.Update(topic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TopicExists(topic.ID))
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
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", topic.UserID);
            return View(topic);
        }

        // GET: Topics/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (topic == null)
            {
                return NotFound();
            }

            return View(topic);
        }

        // POST: Topics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TopicExists(int id)
        {
            return _context.Topics.Any(e => e.ID == id);
        }
    }
}
