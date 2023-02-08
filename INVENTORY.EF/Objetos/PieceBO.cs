using System;
using System.Collections.Generic;
using System.Text;

namespace INVENTORY.EF.Objetos
{
    public class PieceBO
    {
        private string _pieceNo;
        public string PieceNo
        {
            get { return _pieceNo; }
            set { _pieceNo = value; }
        }

        private string _warpShade;
        public string WarpShade
        {
            get { return _warpShade; }
            set { _warpShade = value; }
        }

        private string _weftShade;
        public string WeftShade
        {
            get { return _weftShade; }
            set { _weftShade = value; }
        }

        private string _quality;
        public string Quality
        {
            get { return _quality; }
            set { _quality = value; }
        }

        private string _design;
        public string Design
        {
            get { return _design; }
            set { _design = value; }
        }
        public string LoomNo { get; set; }
        public string Location { get; set; }
        public string RackNo { get; set; }
        public string Prog_Status { get; set; }
    }
}
