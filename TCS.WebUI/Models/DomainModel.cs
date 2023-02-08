using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCS.WebUI.Models
{
    public class DomainModel
    {
        public string DomainName { get; set; }
        public string DomainRute { get; set; }
    }

    public static class Domains
    {
        private static List<DomainModel> domainList;
        public static List<DomainModel> DomainList
        {
            get
            {
                domainList = new List<DomainModel>();
                domainList.Add(new DomainModel() { DomainName = "CROSSVILLE", DomainRute = "crossville.crossvillefabric.cl" });
                domainList.Add(new DomainModel() { DomainName = "TOM JAMES", DomainRute = "tomjames.tomjames.com" });
                return domainList;
            }
            set
            {
                domainList = value;
            }
        }

    }
}
