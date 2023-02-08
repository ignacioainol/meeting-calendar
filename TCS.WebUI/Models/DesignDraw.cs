using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TCS.WebUI.Models
{
    public class DesignDraw
    {
        public int Rows { get; set; } = 0;
        public int Cols { get; set; } = 0;
        public List<PointData> Points { get; set; }
        public List<PointData> Points2 { get; set; }
        public List<PointDataWarping> PointDataWarpings { get; set; }
        public List<PointDataWarping> PointDataWarpings2 { get; set; }
        public List<Repeats> Repeats { get; set; }

        public DesignDraw()
        {
            Points = new List<PointData>();
            Points2 = new List<PointData>();
            PointDataWarpings = new List<PointDataWarping>();
            PointDataWarpings2 = new List<PointDataWarping>();
            Repeats = new List<Repeats>();

        }
    }
  

    public class DesignDrawSley
    {
        public int Rows { get; set; } = 0;
        public int Cols { get; set; } = 0;
        
        public List<PointDataSley> PointDataSley { get; set; }
        public List<PointDataSley> PointDataSley2 { get; set; }
        public List<PointDataSley> PointDataSley3 { get; set; }
        public List<PointDataSley> PointDataSley4 { get; set; }

        public List<Repeats> Repeats { get; set; }

        public DesignDrawSley()
        {
            PointDataSley = new List<PointDataSley>();
            PointDataSley2 = new List<PointDataSley>();
            PointDataSley3 = new List<PointDataSley>();
            PointDataSley4 = new List<PointDataSley>();
            Repeats = new List<Repeats>();


        }
    }


    public class Matriz
    {
        public int Rows { get; set; } = 0;
        public int Cols { get; set; } = 0;
        public List<Fila> Filas { get; set; }

    }

    public class Fila
    {
        public int row { get; set; }
        public List<Columna> Columnas { get; set; }

    }
    public class Columna
    {
        public int col { get; set; }
        public string Dato { get; set; }


    }

    public class MatrizSley
    {
        public int Rows { get; set; } = 0;
        public int Cols { get; set; } = 0;
        public List<ColumnaSley> ColumnaSleys { get; set; }

    }
    public class ColumnaSley
    {
        public int col { get; set; }
        public List<FilaSley> filaSleys { get; set; }

    }
    public class FilaSley
    {
        public int fila { get; set; }
        public string Dato { get; set; }


    }

    public class MatrizSley2
    {
        public int Rows { get; set; } = 0;
        public int Cols { get; set; } = 0;
        public List<ColumnaSley2> ColumnaSleys2 { get; set; }

    }
    public class ColumnaSley2
    {
        public int col { get; set; }
        public List<FilaSley2> filaSleys2 { get; set; }

    }
    public class FilaSley2
    {
        public int fila { get; set; }
        public string Dato { get; set; }


    }
    public class PointData
    {
        public int row { get; set; }
        public int col { get; set; }
        public PointData()
        {
        }
    }
    public class PointDataWarping
    {
        public int row { get; set; }
        public int col { get; set; }
        public int val { get; set; }
        public PointDataWarping()
        {
        }
    }
    public class PointDataSley
    {
        public int col { get; set; }
        public int row { get; set; }
        public int color { get; set; }


        public PointDataSley()
        {
        }
    }
    public class Repeats
    {
        [Required(ErrorMessage = "El codigo es requerido.")]
        public int inicio { get; set; }
        public int fin { get; set; }
        public int cantidad { get; set; }
        public Repeats()
        {
        }
    }
}
