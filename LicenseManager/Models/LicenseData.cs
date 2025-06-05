namespace LicenseManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TiS.License.Data.License")]
    public class LicenseData
    {
        [DataMember(Name = "m_ActivationExpirationDate")]
        public DateTime ActivationExpirationDate { get; set; }

        [DataMember(Name = "m_CreationDate")]
        public DateTime CreationDate { get; set; }

        [DataMember(Name = "m_FeatureItems")]
        public List<LicenseFeatureItemData> FeatureItems { get; set; }

        [DataMember(Name = "m_LicenseId")]
        public string LicenseId { get; set; }

        [DataMember(Name = "m_TotalActivationCount")]
        public int TotalActivationCount { get; set; }

        [DataMember(Name = "m_TrialExpirationDate")]
        public DateTime TrialExpirationDate { get; set; }
    }

}
