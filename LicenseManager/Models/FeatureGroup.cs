namespace LicenseManager.Models;

public partial class FeatureGroup
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Feature> Features { get; set; } = new List<Feature>();
}
