using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCS.WebUI.Models;
namespace TCS.WebUI.ViewComponents
{
    public class DesignSleyViewComponent : ViewComponent
    {

        #region Matriz sley
        public static bool IsRepeatSley(List<Repeats> repeats, int columna)
        {
            foreach (var item in repeats)
            {
                if (item.inicio == columna)
                {
                    return true;
                }
            }
            return false;
        }
        public static int RepeatFinSley(List<Repeats> repeats, int columna)
        {
            foreach (var item in repeats)
            {
                if (item.inicio == columna)
                {
                    return item.fin;
                }
            }
            return 0;
        }
        public static int RepeatCantSley(List<Repeats> repeats, int columna)
        {
            foreach (var item in repeats)
            {
                if (item.inicio == columna)
                {
                    return item.cantidad;
                }
            }
            return 0;
        }
        public static int GetMiLetraSley(List<PointDataSley> pointDatas, int fila, int columna)
        {
            foreach (var item in pointDatas)
            {
                if (item.col == columna && item.row == fila)
                {
                    return 1;
                }
            }
            return 0;
        }
        public static List<FilaSley> GetColsSley(DesignDrawSley drawSley, int columna)
        {
            int go = 0;
            List<FilaSley> filaSleys = new List<FilaSley>();
            foreach (var item in drawSley.PointDataSley)
            {
                if (item.col == columna && go == 0)
                {
                    for (int j = 1; j < drawSley.Rows + 1; j++)
                    {
                        FilaSley filaSley = new FilaSley();
                        filaSley.fila = j;
                        filaSley.Dato = GetMiLetraSley(drawSley.PointDataSley, j, item.col) == 1 ? "SieKipo" : "No tiene";
                        filaSleys.Add(filaSley);
                    }
                    go++;
                }
            }
            return filaSleys;
        }

        #endregion

        public IViewComponentResult Invoke(int cols, int rows, int ends, string data, string sley, string puntos, string repeats, int? x ,string report, string coordMalas2)
        {
            DesignDrawSley draw = new DesignDrawSley
            {
                Cols = cols,
                Rows = rows,
                

            };
            MatrizSley matrizSley = new MatrizSley
            {
                Cols = cols,
                Rows = rows,
            };
            ViewBag.delete = x;
            ViewBag.report = report;

            if (data != null)
            {
                string[] _data = data.Split("|");
                if (data.Length > 1)
                {
                    for (int i = 0; i < _data.Length; i++)
                    {
                        string[] datos = _data[i].Split(";");
                        PointDataSley p = new PointDataSley();
                        p.row = int.Parse(datos[0]);
                        p.col = int.Parse(datos[1]);
                        draw.PointDataSley.Add(p);
                    }
                }
            }

            if (sley != null)
            {
                string[] _sley = sley.Split("|");
                if (sley.Length > 1)
                {
                    for (int i = 0; i < _sley.Length; i++)
                    {
                        string[] datos = _sley[i].Split(";");
                        PointDataSley p = new PointDataSley();
                        p.row = int.Parse(datos[0]);
                        p.col = int.Parse(datos[1]);
                        if (datos.Count()>2)
                        {
                            p.color = int.Parse(datos[2]);

                        }
                        else
                        {
                            p.color = 0;
                        }
                        draw.PointDataSley3.Add(p);
                    }
                }
            }

            if (puntos != null)
            {
                string[] _puntos = puntos.Split("|");
                if (puntos.Length > 1)
                {
                    for (int i = 0; i < _puntos.Length; i++)
                    {
                        string[] datos = _puntos[i].Split(";");
                        PointDataSley p = new PointDataSley();
                        p.row = int.Parse(datos[0]);
                        p.col = int.Parse(datos[1]);
                        draw.PointDataSley2.Add(p);
                    }
                }

                ViewBag.puntos = puntos;
            }


            if (coordMalas2 != null)
            {
                string[] _coordMalas2 = coordMalas2.Split("|");
                if (coordMalas2.Length > 1)
                {
                    for (int i = 0; i < _coordMalas2.Length; i++)
                    {
                        string[] datos = _coordMalas2[i].Split(";");
                        PointDataSley p = new PointDataSley();
                        p.row = int.Parse(datos[0]);
                        p.col = int.Parse(datos[1]);
                        draw.PointDataSley4.Add(p);
                    }
                }

                ViewBag.coordMalas = coordMalas2;
            }

            if (repeats != null)
            {
                string[] _repeats = repeats.Split("|");
                if (repeats.Length > 1)
                {
                    for (int i = 0; i < _repeats.Length; i++)
                    {
                        string[] datos = _repeats[i].Split(",");
                        Repeats r = new Repeats();
                        int ini = int.Parse(datos[0]);
                        r.inicio = int.Parse(datos[0]);
                        int fini = int.Parse(datos[1]);
                        r.fin = int.Parse(datos[1]);
                        r.cantidad = int.Parse(datos[2]);
                        draw.Repeats.Add(r);
                    }
                }

            }


            #region Matriz Sley
            
            
            List<ColumnaSley> columnaSleys = new List<ColumnaSley>();
            int j = 1;
            while (j <= cols)
            {
                if (IsRepeatSley(draw.Repeats, j))
                {
                    int cursor = j;
                    int inicio = j;
                    int fin = RepeatFinSley(draw.Repeats, j);
                    int cantidad = RepeatCantSley(draw.Repeats, j);
                    int repeticion = 0;

                    while (repeticion < cantidad)
                    {
                        while (cursor <= fin)
                        {
                            ColumnaSley columnaSley = new ColumnaSley();
                            columnaSley.col = cursor;
                            columnaSley.filaSleys = GetColsSley(draw, cursor);
                            columnaSleys.Add(columnaSley);
                            matrizSley.ColumnaSleys = columnaSleys;
                            cursor++;
                        }
                        repeticion++;
                        cursor = j;
                    }
                    j = j + ((fin - inicio) + 1);
                }
                else
                {
                    ColumnaSley columnaSley = new ColumnaSley();
                    columnaSley.col = j;
                    columnaSley.filaSleys = GetColsSley(draw, j);
                    columnaSleys.Add(columnaSley);
                    matrizSley.ColumnaSleys = columnaSleys;
                    j++;
                }
            }
            ViewBag.matrizSley = matrizSley;
            #endregion


            return View(draw);
        }
    }
}

