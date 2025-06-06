﻿namespace LicenseManager.Models;

public partial class Client
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ClientApplicationMapping> ClientApplicationMappings { get; set; } = new List<ClientApplicationMapping>();
}
