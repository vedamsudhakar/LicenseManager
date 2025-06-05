using System.ComponentModel.DataAnnotations.Schema;

namespace LicenseManager.Models;

public partial class ClientApplicationMapping
{
    public Guid Id { get; set; }

    public string LicenseId { get; set; } = null!;

    public Guid FkClientId { get; set; }

    public Guid FkApplicationId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int ActivationsCount { get; set; }

    public virtual ICollection<ClientApplicationLicensedFeature> ClientApplicationLicensedFeatures { get; set; } = new List<ClientApplicationLicensedFeature>();

    [NotMapped]
    public virtual Application? FkApplication { get; set; }

    [NotMapped]
    public virtual Client? FkClient { get; set; }
}
