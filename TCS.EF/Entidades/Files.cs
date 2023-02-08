using System;
using System.Collections.Generic;
using System.Text;

namespace TCS.EF.Entidades
{
    public partial class Files
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public int FileIndex { get; set; }
        public int FolderId { get; set; }
        public Folders Folders { get; set; }
    }
}
