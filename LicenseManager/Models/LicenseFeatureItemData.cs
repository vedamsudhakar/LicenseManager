using System.Runtime.Serialization;

namespace LicenseManager.Models;

[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TiS.License.Data.Feature")]
public class LicenseFeatureItemData
{
    [DataMember(Name = "m_Attribute")]
    public string Attribute { get; set; }

    [DataMember(Name = "m_Attribute2")]
    public string Attribute2 { get; set; }

    [DataMember(Name = "m_GroupName")]
    public string GroupName { get; set; }

    [DataMember(Name = "m_Name")]
    public string Name { get; set; }

    [DataMember(Name = "m_TotalUsageCount")]
    public int TotalUsageCount { get; set; }
}

