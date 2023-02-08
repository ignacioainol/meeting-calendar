using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCS.EF;
using TCS.WebUI.Models;

namespace TCS.WebUI.ViewComponents
{
    public class DesignDraw2ViewComponent : ViewComponent
    {
        private readonly TCSContext _context;

        public DesignDraw2ViewComponent(TCSContext context)
        {
            _context = context;
        }

        #region Matriz draft
        public static bool IsRepeat(List<Repeats> repeats, int fila)
        {
            foreach (var item in repeats)
            {
                if (item.inicio == fila)
                {
                    return true;
                }
            }
            return false;   
        }
        public static int RepeatFin(List<Repeats> repeats, int fila)
        {
            foreach (var item in repeats)
            {
                if (item.inicio == fila)
                {
                    return item.fin;
                }
            }
            return 0;
        }
        public static int RepeatCant(List<Repeats> repeats, int fila)
        {
            foreach (var item in repeats)
            {
                if (item.inicio == fila)
                {
                    return item.cantidad;
                }
            }
            return 0;
        }
        public static int GetMiLetra(List<PointData> pointDatas, int fila, int columna)
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
        public static List<Columna> GetCols(DesignDraw draw, int fila)
        {
            int go = 0;
            List<Columna> columnas = new List<Columna>();
            foreach (var item in draw.Points)
            {
                if (item.row == fila && go == 0)
                {
                    for (int j = 1; j < draw.Cols + 1; j++)
                    {
                        Columna columna = new Columna();
                        columna.col = j;
                        columna.Dato = GetMiLetra(draw.Points, item.row, j) == 1 ? "SieKipo" : "No tiene";
                        columnas.Add(columna);
                    }
                    go++;
                }
            }
            return columnas;
        }
        #endregion

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




        public IViewComponentResult Invoke(int cols, int rows, string data, string puntos, string repeats, int? x, int? draftId)
        {

            var draft = _context.PegPlan.First(x => x.DraftPlan == data);


            var sley = _context.Draft.Where(x => x.DraftId == draftId).FirstOrDefault();


            int suma = 0;
            if (repeats != null)
            {
                string[] repeticiones = repeats.Split("|");

                foreach (var item3 in repeticiones)
                {
                    string[] rep = item3.Split(",");
                    int inicio = Convert.ToInt32(rep[0]);
                    int fin = Convert.ToInt32(rep[1]);
                    int cantidad = Convert.ToInt32(rep[2]);
                    int cant = cantidad - 1;
                    var G = (fin - inicio + 1) * cant;
                    suma = suma + G;
                }

            }
            DesignDrawSley drawSley = new DesignDrawSley
            {
                Cols = sley.Lenght,
                Rows = sley.Shafts,

            };

            DesignDraw draw = new DesignDraw
            {
                Cols = cols,
                Rows = rows
            };

            Matriz matriz = new Matriz
            {
                Cols = cols,
                Rows = rows
            };
            MatrizSley matrizSley = new MatrizSley
            {
                Cols = sley.Lenght,
                Rows = sley.Shafts,
            };
            MatrizSley2 matrizSley2 = new MatrizSley2
            {
                Cols = sley.Lenght,
                Rows = sley.Shafts,
            };

            ViewBag.delete = x;

            #region Matriz 
            if (data != null)
            {
                string[] _data = data.Split("|");
                if (data.Length > 1)
                {
                    for (int l = 0; l < _data.Length; l++)
                    {
                        string[] datos = _data[l].Split(";");
                        PointData p = new PointData();
                        p.row = int.Parse(datos[0]);
                        p.col = int.Parse(datos[1]);
                        draw.Points.Add(p);
                    }
                }
            }
            if (puntos != null)
            {
                string[] _puntos = puntos.Split("|");
                if (puntos.Length > 1)
                {
                    for (int h = 0; h < _puntos.Length; h++)
                    {
                        string[] datos = _puntos[h].Split(";");
                        PointData p = new PointData();
                        p.row = int.Parse(datos[0]);
                        p.col = int.Parse(datos[1]);
                        draw.Points2.Add(p);
                    }
                }

                ViewBag.puntos = puntos;
            }
            if (repeats != null)
            {

                string[] _repeats = repeats.Split("|");
                if (repeats.Length > 1)
                {
                    for (int a = 0; a < _repeats.Length; a++)
                    {
                        string[] datos = _repeats[a].Split(",");
                        Repeats r = new Repeats();
                        int ini = int.Parse(datos[0]);
                        r.fin = (rows - ini) + 1;
                        int fini = int.Parse(datos[1]);
                        r.inicio = (rows - fini) + 1;
                        r.cantidad = int.Parse(datos[2]);
                        draw.Repeats.Add(r);
                    }
                }

            }
            List<Fila> filas = new List<Fila>();
            int i = 1;
            while (i<= rows)
            {
                if (IsRepeat(draw.Repeats, i))
                {
                    int cursor = i;
                    int inicio = i;
                    int fin = RepeatFin(draw.Repeats, i);
                    int cantidad = RepeatCant(draw.Repeats, i);
                    int repeticion = 0;

                    while (repeticion < cantidad)
                    {
                        while (cursor <= fin)
                        {
                            
                            Fila fila = new Fila();
                            fila.row = cursor;
                            fila.Columnas = GetCols(draw,cursor);
                            filas.Add(fila);
                            matriz.Filas = filas;
                            cursor++;
                        }
                        repeticion++;
                        cursor = i;
                    }
                    i = i + ((fin - inicio) + 1);
                }
                else
                {
                    Fila fila = new Fila();
                    fila.row = i;
                    fila.Columnas = GetCols(draw, i);
                    filas.Add(fila);
                    matriz.Filas = filas;
                    i++;
                }
            }
            ViewBag.matriz = matriz;
            #endregion




            #region Matriz Sley
            if (sley.PlanCode != null)
            {
                string[] _data = sley.PlanCode.Split("|");
                if (sley.PlanCode.Length > 1)
                {
                    for (int l = 0; l < _data.Length; l++)
                    {
                        string[] datos = _data[l].Split(";");
                        PointDataSley p = new PointDataSley();
                        p.row = int.Parse(datos[0]);
                        p.col = int.Parse(datos[1]);
                        drawSley.PointDataSley.Add(p);
                    }
                }
            }
            if (sley.Repeats != null)
            {

                string[] _repeats = sley.Repeats.Split("|");
                if (sley.Repeats.Length > 1)
                {
                    for (int a = 0; a < _repeats.Length; a++)
                    {
                        string[] datos = _repeats[a].Split(",");
                        Repeats r = new Repeats();
                        int ini = int.Parse(datos[0]);
                        r.inicio = int.Parse(datos[0]);
                        int fini = int.Parse(datos[1]);
                        r.fin = int.Parse(datos[1]);
                        r.cantidad = int.Parse(datos[2]);
                        drawSley.Repeats.Add(r);
                    }
                }

            }
            List<ColumnaSley> columnaSleys = new List<ColumnaSley>();
            int j = 1;
            while (j <= sley.Lenght)
            {
                if (IsRepeatSley(drawSley.Repeats, j))
                {
                    int cursor = j;
                    int inicio = j;
                    int fin = RepeatFinSley(drawSley.Repeats, j);
                    int cantidad = RepeatCantSley(drawSley.Repeats, j);
                    int repeticion = 0;

                    while (repeticion < cantidad)
                    {
                        while (cursor <= fin)
                        {
                            ColumnaSley columnaSley = new ColumnaSley();
                            columnaSley.col = cursor;
                            columnaSley.filaSleys = GetColsSley(drawSley, cursor);
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
                    columnaSley.filaSleys = GetColsSley(drawSley, j);
                    columnaSleys.Add(columnaSley);
                    matrizSley.ColumnaSleys = columnaSleys;
                    j++;
                }
            }
             ViewBag.matrizSley = matrizSley;
            #endregion


            #region Ultima matriz
            List<ColumnaSley2> columnaSleys2 = new List<ColumnaSley2>();
            int z = 1;
            int zx = 1;
            foreach (var item in matrizSley.ColumnaSleys)
            {
                foreach (var item2 in item.filaSleys)
                {
                    ColumnaSley2 columnaSley2 = new ColumnaSley2();
                    if (item2.Dato == "SieKipo")
                    {
                        
                        List<FilaSley2> filaSleys2 = new List<FilaSley2>();
                        foreach (var item3 in matriz.Filas)
                        {
                           columnaSley2.col = z;
                            foreach (var item4 in item3.Columnas)
                            {
                                if (item4.col == (sley.Shafts -item2.fila) + 1)
                                {
                                    FilaSley2 filaSley2 = new FilaSley2();
                                    filaSley2.fila = zx;
                                    filaSley2.Dato = item4.Dato;
                                    filaSleys2.Add(filaSley2);
                                    zx++;
                                }
                                
                            }
                            
                            columnaSley2.filaSleys2 = filaSleys2;
                        }
                        zx = 1;
                        columnaSleys2.Add(columnaSley2);
                        z++;
                        matrizSley2.ColumnaSleys2 = columnaSleys2;
                    }
                }
            }
            ViewBag.matrizSley2 = matrizSley2;
            #endregion


            List<Fila> filas2 = new List<Fila>();

            return View(draw);
        }
    }
}
