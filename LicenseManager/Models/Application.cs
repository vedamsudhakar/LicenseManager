using System;
using System.Collections.Generic;

namespace LicenseManager.Models;

public partial class Application
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ClientApplicationMapping> ClientApplicationMappings { get; set; } = new List<ClientApplicationMapping>();
}
