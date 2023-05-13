using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using User_Story.Data;
using User_Story.Models;

namespace User_Story.Controllers
{
    public class MessagesController : Controller
    {
        private readonly Context _context;

        public MessagesController(Context context)
        {
            _context = context;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            var context = _context.Messages.Include(m => m.Topic).Include(m => m.User);
            return View(await context.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Topic)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            ViewData["TopicID"] = new SelectList(_context.Topics, "ID", "ID");
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string? messageText, int? topicId)
        {
            Message message = new Message();
            message.MessageText = messageText;
            message.CreationDate = DateTime.Now;
            message.Topic = _context.Topics.First(x=>x.ID == topicId);
            message.User = _context.Users.First(x => x.Login.Equals(HttpContext.User.Identity.Name));
            _context.Add(message);
            await _context.SaveChangesAsync();
            return Redirect("~/Topics/Details/" + topicId); 
            
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["TopicID"] = new SelectList(_context.Topics, "ID", "ID", message.TopicID);
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", message.UserID);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,MessageText,CreationDate,TopicID,UserID")] Message message)
        {
            if (id != message.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.ID))
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
            ViewData["TopicID"] = new SelectList(_context.Topics, "ID", "ID", message.TopicID);
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", message.UserID);
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Topic)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            int topicId = message.TopicID;
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return Redirect("~/Topics/Details/" + topicId);
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.ID == id);
        }
    }
}
