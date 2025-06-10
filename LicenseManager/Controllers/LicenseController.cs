using LicenseManager.Models;
using LicenseManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using TiS.License.Data.Feature;
using TiS.License.Data.License;

namespace LicenseManager.Controllers
{
    public class LicenseController : Controller
    {
        private readonly LicenseManagerContext _context;

        public LicenseController(LicenseManagerContext context)
        {
            _context = context;
        }

        // GET: ClientApplication licenses
        [Authorize(Roles = "Admin, User")]        
        public async Task<IActionResult> Index()
        {
            // return View(await _context.ClientApplicationMappings.ToListAsync());
            var mappings = await _context.ClientApplicationMappings
                                        .Include(cam => cam.FkClient)
                                        .Include(cam => cam.FkApplication)
                                        .ToListAsync();

            return View(mappings);
        }


        // GET: License/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Clients = new SelectList(_context.Clients, "Id", "Name");
            ViewBag.Applications = new SelectList(_context.Applications, "Id", "Name");
            ViewBag.LicenseId = GenerateUniqueLicenseId();

            var featureGroups = _context.FeatureGroups
                .Include(fg => fg.Features)
                .OrderBy(x=>x.Name).ThenBy(y=>y.Name).ToList();

            ViewBag.FeatureGroups = featureGroups;
            ViewBag.DefaultFeatureConfiguration = CreateDefaultFeatureConfiguration();

            var viewModel = new ClientApplicationLicenseViewModel();

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientApplicationLicenseViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _context.ClientApplicationMappings.Add(viewModel.ClientApplicationMapping);
                await _context.SaveChangesAsync();

                // Get the last inserted ID
                var insertedId = viewModel.ClientApplicationMapping.Id;

                var mapping = new ClientApplicationLicensedFeature
                {
                    FkClientApplicationMappingId = insertedId,
                    Features = JsonSerializer.Serialize(viewModel.LicensedFeatures)
                };

                _context.ClientApplicationLicensedFeatures.Add(mapping);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Reload Clients in case of error
            ViewBag.Clients = new SelectList(_context.Clients, "Id", "Name");
            ViewBag.Applications = new SelectList(_context.Applications, "Id", "Name");
            ViewBag.LicenseId = GenerateUniqueLicenseId();

            return View(viewModel);
        }

        [Authorize(Roles = "Admin, User")]
        // GET: License/Create
        public IActionResult Download(Guid? id)
        {
            var clientApplicationMapping = _context.ClientApplicationMappings.First(cam => cam.Id == id);
            var licensedFeatures = _context.ClientApplicationLicensedFeatures.First(
                    lf => lf.FkClientApplicationMappingId == clientApplicationMapping.Id);

            List<LicenseFeatureItemData> features =
                 JsonSerializer.Deserialize<List<LicenseFeatureItemData>>(licensedFeatures.Features) ?? [];

            features.ForEach(f =>
            {
                f.Attribute ??= string.Empty;
                f.Attribute2 ??= string.Empty;
            });

            LicenseData licenseData = new()
            {
                TotalActivationCount = clientApplicationMapping.ActivationsCount,
                CreationDate = clientApplicationMapping.StartDate,
                TrialExpirationDate = clientApplicationMapping.EndDate,
                ActivationExpirationDate = clientApplicationMapping.EndDate,
                FeatureItems = features,
                LicenseId = clientApplicationMapping.LicenseId
            };


            //var namespaces = new XmlSerializerNamespaces();
            //namespaces.Add("i", "http://www.w3.org/2001/XMLSchema-instance");
            //namespaces.Add("z", "http://schemas.microsoft.com/2003/10/Serialization/");
            //namespaces.Add("", "http://schemas.datacontract.org/2004/07/TiS.License.Data.License");
            //namespaces.Add("a", "http://schemas.datacontract.org/2004/07/TiS.License.Data.Feature");

            // 1. Serialize to XML
            var xmlSerializer = new XmlSerializer(typeof(LicenseData));
            string xmlString = string.Empty;

            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, licenseData);
                xmlString = stringWriter.ToString();
            }

            //var serializer = new DataContractSerializer(typeof(LicenseData), new DataContractSerializerSettings
            //{
            //    PreserveObjectReferences = true
            //});

            //var sb = new StringBuilder();
            //using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = true }))
            //{
            //    serializer.WriteObject(writer, licenseData);
            //}

            
            var xmlBytes = Encoding.UTF8.GetBytes(xmlString);

            //// 2. Convert XML string to Base64
            //var base64String = Convert.ToBase64String(xmlBytes);

            //// 3. Convert Base64 string back to bytes for download
            //var base64Bytes = Encoding.UTF8.GetBytes(base64String);

            var fileName = $"eFlow + {clientApplicationMapping.ActivationsCount} Activations + {clientApplicationMapping.LicenseId} + LIMITED {clientApplicationMapping.EndDate.ToString("yyyy-MM-dd")}.TIS_LIC";

            // 4. Return file with content type and suggested filename
            return File(xmlBytes, "text/plain", "LicenseData.xml");
        }


        public string GenerateUniqueLicenseId()
        {
            var uniqueId = string.Empty;

            do
            {
                // Generate a standard Guid and remove hyphens
                string raw = Guid.NewGuid().ToString("N").ToUpper(); // 32 hex characters

                // Split into 5 blocks of 5 characters: total 25 characters used
                uniqueId = string.Join("-", Enumerable.Range(0, 5).Select(i => raw.Substring(i * 5, 5)));

            }
            while (_context.ClientApplicationMappings.Any(cam => cam.LicenseId == uniqueId) == true);

            return uniqueId;
        }

        private Dictionary<string, LicenseFeatureItemData> CreateDefaultFeatureConfiguration()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "default-feature-configuration.json");

            if (!System.IO.File.Exists(path))
                return [];

            var json = System.IO.File.ReadAllText(path);
            List<LicenseFeatureItemData> featureDataItems = JsonSerializer.Deserialize<List<LicenseFeatureItemData>>(json) ?? new List<LicenseFeatureItemData>();

            return  featureDataItems.ToDictionary(fd => $"{fd.GroupName}_{fd.Name}", fd => fd);
        }

        // DELETE: License/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientApplicationMapping = _context.ClientApplicationMappings
                .Include(c => c.FkApplication)
                .Include(c => c.FkClient)
                .FirstOrDefault(cam => cam.Id == id);

            ClientApplicationLicenseViewModel clientApplicationLicenseViewModel = new ClientApplicationLicenseViewModel()
            {
                ClientApplicationMapping = clientApplicationMapping
            };

            return View(clientApplicationLicenseViewModel);
        }

        // POST: License/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var clientApplicationMapping = await _context.ClientApplicationMappings.FindAsync(id);

            if (clientApplicationMapping != null)
            {
                var licensedFeatures = await _context.ClientApplicationLicensedFeatures
                    .FirstOrDefaultAsync(lf => lf.FkClientApplicationMappingId == clientApplicationMapping.Id);

                if (licensedFeatures != null)
                {
                    _context.ClientApplicationLicensedFeatures.Remove(licensedFeatures);
                }

                _context.ClientApplicationMappings.Remove(clientApplicationMapping);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
