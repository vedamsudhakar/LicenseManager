using System;
using System.Collections.Generic;

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

    public virtual Application FkApplication { get; set; } = null!;

    public virtual Client FkClient { get; set; } = null!;
}
