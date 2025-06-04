using System;
using System.Collections.Generic;

namespace LicenseManager.Models;

public partial class Feature
{
    public Guid Id { get; set; }

    public Guid FkFeatureGroupId { get; set; }

    public string Name { get; set; } = null!;

    public virtual FeatureGroup FkFeatureGroup { get; set; } = null!;
}
