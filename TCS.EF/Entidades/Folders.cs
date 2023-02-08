using System;
using System.Collections.Generic;
using System.Text;

namespace TCS.EF.Entidades
{
    public partial class Folders
    {
        public int FolderId { get; set; }
        public bool Root { get; set; }
        public string Name { get; set; }
        public bool HasChildrens { get; set; }
        public int FolderIndex { get; set; }
        public int Parent { get; set; }
        public int DepthLevel { get; set; }
        public virtual ICollection<Files> Files { get; set; }


    }
}
