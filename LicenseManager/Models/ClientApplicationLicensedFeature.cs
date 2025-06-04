using System;
using System.Collections.Generic;

namespace LicenseManager.Models;

public partial class ClientApplicationLicensedFeature
{
    public Guid Id { get; set; }

    public Guid FkClientApplicationMappingId { get; set; }

    public string Features { get; set; } = null!;

    public virtual ClientApplicationMapping FkClientApplicationMapping { get; set; } = null!;
}
