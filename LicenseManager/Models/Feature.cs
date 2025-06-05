using System.ComponentModel.DataAnnotations;

namespace LicenseManager.Models;

public partial class Feature
{
    [Display(Name = "Id")]

    public Guid Id { get; set; }

    [Display(Name = "Feature Group Id")]
    public Guid FkFeatureGroupId { get; set; }

    [Display(Name = "Name")]
    public string Name { get; set; } = null!;

    [Display(Name ="Feature Group")]
    public virtual FeatureGroup? FkFeatureGroup { get; set; } = null!;
}
