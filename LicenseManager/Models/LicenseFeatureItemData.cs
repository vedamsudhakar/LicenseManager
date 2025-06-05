namespace TiS.License.Data.Feature
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [DataContract(Name = "LicenseFeatureItemData", Namespace = "http://schemas.datacontract.org/2004/07/TiS.License.Data.Feature")]
    public class LicenseFeatureItemData
    {

        [DataMember(Name = "m_Attribute")]
        [Display(Name = "Attribute")]
        public string? Attribute { get; set; }

        [DataMember(Name = "m_Attribute2")]
        [Display(Name = "Attribute2")]
        public string? Attribute2 { get; set; }

        [DataMember(Name = "m_GroupName")]
        [Display(Name = "Group Name")]
        public string GroupName { get; set; }

        [DataMember(Name = "m_Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "m_TotalUsageCount")]
        [Display(Name = "Total Usage Count")]
        public int TotalUsageCount { get; set; }
    }
}

