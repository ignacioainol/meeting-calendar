using System;
using System.Collections.Generic;

namespace REPOSITORY.EF.Entidades
{
    public partial class Query
    {
        public int ID_Query { get; set; }
        public string ID_Owner { get; set; }
        public string Name_Query { get; set; }
        public string QueryText { get; set; }
        public string Server_Name { get; set; }
        public string Server_Type { get; set; }
        public string Database_Name { get; set; }
        public string Database_User { get; set; }
        public string Query_Type { get; set; }
        public string Database_Password { get; set; }
        public bool active { get; set; }
        public string Description { get; set; }
        public DateTime create_date { get; set; }
        public DateTime? update_date { get; set; }
        public string update_user { get; set; }
        public string template_type { get; set; }

        public virtual ICollection<Share> Shares { get; set; }
        public virtual ICollection<Parameters> Parameters { get; set; }

    }
}
