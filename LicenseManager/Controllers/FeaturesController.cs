using LicenseManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Controllers
{
    public class FeaturesController : Controller
    {
        private readonly LicenseManagerContext _context;

        public FeaturesController(LicenseManagerContext context)
        {
            _context = context;
        }

        // GET: Features
        public async Task<IActionResult> Index()
        {
            var licenseManagerContext = _context.Features.Include(f => f.FkFeatureGroup)
            .OrderBy(f => f.FkFeatureGroup.Name)
            .ThenBy(f => f.Name);
            return View(await licenseManagerContext.ToListAsync());
        }

        // GET: Features/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feature = await _context.Features
                .Include(f => f.FkFeatureGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feature == null)
            {
                return NotFound();
            }

            return View(feature);
        }

        // GET: Features/Create
        public IActionResult Create()
        {
            ViewData["FkFeatureGroupId"] = new SelectList(_context.FeatureGroups, "Id", "Name");
            return View();
        }

        // POST: Features/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FkFeatureGroupId,Name")] Feature feature)
        {
            if (ModelState.IsValid)
            {
                feature.Id = Guid.NewGuid();
                _context.Add(feature);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkFeatureGroupId"] = new SelectList(_context.FeatureGroups, "Id", "Name", feature.FkFeatureGroupId);
            return View(feature);
        }

        // GET: Features/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feature = await _context.Features.FindAsync(id);
            if (feature == null)
            {
                return NotFound();
            }
            ViewData["FkFeatureGroupId"] = new SelectList(_context.FeatureGroups, "Id", "Name", feature.FkFeatureGroupId);
            return View(feature);
        }

        // POST: Features/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FkFeatureGroupId,Name")] Feature feature)
        {
            if (id != feature.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feature);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeatureExists(feature.Id))
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
            ViewData["FkFeatureGroupId"] = new SelectList(_context.FeatureGroups, "Id", "Name", feature.FkFeatureGroupId);
            return View(feature);
        }

        // GET: Features/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feature = await _context.Features
                .Include(f => f.FkFeatureGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feature == null)
            {
                return NotFound();
            }

            return View(feature);
        }

        // POST: Features/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var feature = await _context.Features.FindAsync(id);
            if (feature != null)
            {
                _context.Features.Remove(feature);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeatureExists(Guid id)
        {
            return _context.Features.Any(e => e.Id == id);
        }
    }
}
