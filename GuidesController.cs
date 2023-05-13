using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using User_Story.Data;
using User_Story.Models;

namespace User_Story.Controllers
{
    public class GuidesController : Controller
    {
        private readonly Context _context;

        public GuidesController(Context context)
        {
            _context = context;
        }

        // GET: Topics
        public async Task<IActionResult> Index()
        {
            return View();
        }

    }
}
