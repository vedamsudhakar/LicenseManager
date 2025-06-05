using LicenseManager.Models;
using LicenseManager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Xml;
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
        public IActionResult Create()
        {
            ViewBag.Clients = new SelectList(_context.Clients, "Id", "Name");
            ViewBag.Applications = new SelectList(_context.Applications, "Id", "Name");
            ViewBag.LicenseId = GenerateUniqueLicenseId();

            var viewModel = new ClientApplicationLicenseViewModel
            {
                LicensedFeatures = CreateDefaultFeatureConfiguration()
            };

            return View(viewModel);
        }

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

                if (viewModel.LicensedFeatures.Count == 0)
                    viewModel.LicensedFeatures = CreateDefaultFeatureConfiguration();

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

        // GET: License/Create
        public IActionResult Download(Guid? id)
        {
            var clientApplicationMapping = _context.ClientApplicationMappings.FirstOrDefault(cam => cam.Id == id);
            var licensedFeatures = _context.ClientApplicationLicensedFeatures.FirstOrDefault(
                lf => lf.FkClientApplicationMappingId == clientApplicationMapping.Id);

            List<LicenseFeatureItemData> features =
                 JsonSerializer.Deserialize<List<LicenseFeatureItemData>>(licensedFeatures.Features) ?? new List<LicenseFeatureItemData>();

            LicenseData licenseData = new()
            {
                TotalActivationCount = clientApplicationMapping.ActivationsCount,
                CreationDate = clientApplicationMapping.StartDate,
                TrialExpirationDate = clientApplicationMapping.EndDate,
                ActivationExpirationDate = clientApplicationMapping.EndDate,
                FeatureItems = features,
                LicenseId = clientApplicationMapping.LicenseId
            };

            // 1. Serialize to XML
            //var xmlSerializer = new XmlSerializer(typeof(LicenseData));
            //string xmlString;

            //using (var stringWriter = new StringWriter())
            //{
            //    xmlSerializer.Serialize(stringWriter, licenseData);
            //    xmlString = stringWriter.ToString();
            //}

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("i", "http://www.w3.org/2001/XMLSchema-instance");
            namespaces.Add("z", "http://schemas.microsoft.com/2003/10/Serialization/");
            namespaces.Add("",  "http://schemas.datacontract.org/2004/07/TiS.License.Data.License");
            namespaces.Add("a", "http://schemas.datacontract.org/2004/07/TiS.License.Data.Feature");


            var serializer = new DataContractSerializer(typeof(LicenseData), new DataContractSerializerSettings
            {
                PreserveObjectReferences = true
            });

            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = true }))
            {
                serializer.WriteObject(writer, licenseData);
            }

            // 2. Convert XML string to Base64
            var xmlBytes = Encoding.UTF8.GetBytes(sb.ToString());
            var base64String = Convert.ToBase64String(xmlBytes);

            // 3. Convert Base64 string back to bytes for download
            var base64Bytes = Encoding.UTF8.GetBytes(base64String);

            var fileName = $"eFlow + {clientApplicationMapping.ActivationsCount} Activations + {clientApplicationMapping.LicenseId} + LIMITED {clientApplicationMapping.EndDate.ToString("yyyy-MM-dd")}.TIS_LIC";

            // 4. Return file with content type and suggested filename
            return File(base64Bytes, "text/plain", fileName);
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

        private List<LicenseFeatureItemData> CreateDefaultFeatureConfiguration()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "default-feature-configuration.json");

            if (!System.IO.File.Exists(path))
                return [];

            var json = System.IO.File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<LicenseFeatureItemData>>(json) ?? new List<LicenseFeatureItemData>();

        }
    }
}
