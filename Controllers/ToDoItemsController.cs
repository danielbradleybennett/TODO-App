using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TODO.Data;
using TODO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace TODO.Controllers
{
    [Authorize]
    public class ToDoItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public ToDoItemsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ToDoItems
        public async Task<IActionResult> Index(string filterButton)
        {
            var user = await GetCurrentUserAsync();

            if (filterButton == "To Do")
            {
                var items = await _context.ToDoItems
                  .Where(ti => ti.ApplicationUserId == user.Id)
                  .Where(ti => ti.ToDoStatusId == 1)
                  .Include(ti => ti.ToDoStatus)
                  .ToListAsync();


                return View(items);
            }
            else if (filterButton == "Progress")
            {
                var items = await _context.ToDoItems
                  .Where(ti => ti.ApplicationUserId == user.Id)
                  .Where(ti => ti.ToDoStatusId == 2)
                  .Include(ti => ti.ToDoStatus)
                  .ToListAsync();

                return View(items);
            }
            else if (filterButton == "Done")
            {
                var items = await _context.ToDoItems
                  .Where(ti => ti.ApplicationUserId == user.Id)
                  .Where(ti => ti.ToDoStatusId == 3)
                  .Include(ti => ti.ToDoStatus)
                  .ToListAsync();

                return View(items);
            }
            else
            {
                var items = await _context.ToDoItems
                 .Where(si => si.ApplicationUserId == user.Id)
                 .Include(t => t.ToDoStatus)
                 .ToListAsync();

                return View(items);
            }
        }

        // GET: ToDoItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDoItem = await _context.ToDoItems
                .Include(t => t.ApplicationUser)
                .Include(t => t.ToDoStatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoItem == null)
            {
                return NotFound();
            }

            return View(toDoItem);
        }

        // GET: ToDoItems/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUser, "Id", "Id");
            ViewData["ToDoStatusId"] = new SelectList(_context.ToDoStatuses, "Id", "Title");
            return View();
        }

        // POST: ToDoItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ToDoStatusId,ApplicationUserId")] ToDoItem toDoItem)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                toDoItem.ApplicationUserId = user.Id;

                _context.ToDoItems.Add(toDoItem);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ToDoItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUser, "Id", "Id", toDoItem.ApplicationUserId);
            ViewData["ToDoStatusId"] = new SelectList(_context.ToDoStatuses, "Id", "Title", toDoItem.ToDoStatusId);
            return View(toDoItem);
        }

        // POST: ToDoItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ToDoStatusId,ApplicationUserId")] ToDoItem toDoItem)
        {
            if (id != toDoItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(toDoItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoItemExists(toDoItem.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUser, "Id", "Id", toDoItem.ApplicationUserId);
            ViewData["ToDoStatusId"] = new SelectList(_context.ToDoStatuses, "Id", "Id", toDoItem.ToDoStatusId);
            return View(toDoItem);
        }

        // GET: ToDoItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDoItem = await _context.ToDoItems
                .Include(i => i.ToDoStatus)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (toDoItem == null)
            {
                return NotFound();
            }

            return View(toDoItem);
        }

        // POST: ToDoItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var toDoItem = await _context.ToDoItems.FindAsync(id);
            _context.ToDoItems.Remove(toDoItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoItemExists(int id)
        {
            return _context.ToDoItems.Any(e => e.Id == id);
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}
