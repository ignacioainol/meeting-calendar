﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCS.WebUI.Models;

namespace TCS.WebUI.ViewComponents
{
    public class WarpingDrawViewComponent : ViewComponent
    {

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


        public IViewComponentResult Invoke(int cols, int rows, string data, string puntos, string repeats, int? x)
        {
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

            ViewBag.delete = x;

            if (data != null)
            {
                string[] _data = data.Split("|");
                if (data.Length > 1)
                {
                    for (int i = 0; i < _data.Length; i++)
                    {
                        string[] datos = _data[i].Split(";");
                        PointDataWarping p = new PointDataWarping();
                        p.row = int.Parse(datos[0]);
                        p.col = int.Parse(datos[1]);
                        p.val = int.Parse(datos[2]);
                        draw.PointDataWarpings.Add(p);
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
                        PointDataWarping p = new PointDataWarping();
                        p.row = int.Parse(datos[0]);
                        p.col = int.Parse(datos[1]);
                        draw.PointDataWarpings2.Add(p);
                    }
                }

                ViewBag.puntos = puntos;
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

            return View(draw);
        }
    }
}
