using LicenseManager.Models;

namespace LicenseManager.ViewModels
{
    public class ClientApplicationLicenseViewModel
    {
        public ClientApplicationMapping ClientApplicationMapping { get; set; } = null!;
        public List<LicenseFeatureItemData> LicensedFeatures { get; set; } = [];
    }
}
