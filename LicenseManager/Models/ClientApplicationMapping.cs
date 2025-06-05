using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicenseManager.Models;

public partial class ClientApplicationMapping
{
    [Display(Name = "Id")]
    public Guid Id { get; set; }

    [Display(Name = "License Id")]
    public string LicenseId { get; set; } = null!;

    [Display(Name = "Client Id")]
    public Guid FkClientId { get; set; }

    [Display(Name = "Application Id")]
    public Guid FkApplicationId { get; set; }

    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; }

    [Display(Name = "End Date")]
    public DateTime EndDate { get; set; }

    [Display(Name = "Activations Count")]
    public int ActivationsCount { get; set; }

    public virtual ICollection<ClientApplicationLicensedFeature> ClientApplicationLicensedFeatures { get; set; } = new List<ClientApplicationLicensedFeature>();

    [Display(Name = "Application")]
    [NotMapped]
    public virtual Application? FkApplication { get; set; }

    [Display(Name = "Client")]
    [NotMapped]
    public virtual Client? FkClient { get; set; }
}
