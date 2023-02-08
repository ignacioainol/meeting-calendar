using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TCS.EF;
using TCS.EF.Entidades;
using TCS.WebUI.ViewComponents;

namespace TCS.WebUI.Controllers
{
    public class DesignController : Controller
    {
        private readonly TCSContext _context;

        public DesignController(TCSContext context)
        {
            _context = context;
        }

        // GET: Design
        public async Task<IActionResult> Index()
        {
            string abc = "ñ,A,B,C,D,E,F,G,H,I,J,K,L,M,N,Ñ,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            string[] aABC = abc.Split(",");

            string arreglo = "3(1A)4A1D3(1B1A1C1A)8A1E4A1D1B1A1C1D5A1E8A1D4A";

            //string arreglo = "4(4A4B)2A2D2A4B4(4A4B)2A2C2A4B";
            var varchar = @"[0-9]{1,3}[*]?[A-Z(]{1}|[)]";
            MatchCollection matches = Regex.Matches(arreglo, varchar);

           
            List<string> vector = new List<string>();
            string cadena = "";
            for (int i = 0; i < matches.Count; i++)
            {
                bool findetectado = false;

                var output = Regex.Replace(matches[i].ToString(), @"[\d-]", string.Empty);
                var nextoutput = "";
                var outputoriginal = matches[i].ToString();
                output = output.Replace("*", string.Empty);
             
                if (i + 1 < matches.Count)
                {
                    nextoutput = Regex.Replace(matches[i + 1].ToString(), @"[\d-]", string.Empty);
                    nextoutput = nextoutput.Replace("*", string.Empty);
             
                }
                cadena += $"{outputoriginal}";
                if (nextoutput != "(" && nextoutput != ")" && nextoutput.Length > 0)
                {
                    if (char.Parse(nextoutput) < char.Parse(output))
                    {
                
                        findetectado = true;
                    }
                }
            
                if (nextoutput == "(" || output == ")" || nextoutput.Length == 0 || findetectado)
                {
                    vector.Add(cadena);
                    cadena = "";
                }

            }

            string arregloFinal = "";
            int contador = 1;
            string repeticion = "";
            string repeticionFinal = "";
            var numero = "";
            foreach (var item in vector)
            {

                
                string ultimo = item.Last().ToString();

                if (item[1].ToString() == "(")
                {
                    numero = item[0].ToString();

                    repeticion = contador + ";" ;

                }
                if (ultimo == ")")
                {
                    repeticion = repeticion + contador + ";" + numero;
                    repeticionFinal = repeticionFinal + repeticion + "|";
                }
                MatchCollection matches2 = Regex.Matches(item, varchar);
                foreach (var item2 in matches2)
                {
                    var arreglo2 = item2.ToString().Insert(1, ";");
                    string[] coordenadas = arreglo2.Split(";");
                    for (int i = 1; i < 10; i++)
                    {
                        if (coordenadas[1] == aABC[i])
                        {
                            arregloFinal = arregloFinal + + i + ";" + contador + ";" + coordenadas[0] + "|";
                            break;
                        }
                    }
                }
                contador++;
            }



            var tCSContext = _context.Design.Include(d => d.Draft)
                                            .Include(d => d.PegPlan)
                                            .Include(x => x.DesignYarns)
                                                .ThenInclude(x => x.Yarn);
            return View(await tCSContext.ToListAsync());
        }

        // GET: Design/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var design = await _context.Design
                .Include(d => d.Draft)
                .Include(d => d.PegPlan)
                .FirstOrDefaultAsync(m => m.DesignId == id);
            if (design == null)
            {
                return NotFound();
            }

            return View(design);
        }

        // GET: Design/Create
        public IActionResult Create()
        {
            ViewData["DraftId"] = new SelectList(_context.Draft, "DraftId", "Code");
            ViewData["PegPlanId"] = new SelectList(_context.PegPlan, "PegPlanId", "Code");
            ViewData["YarnId"] = new SelectList(_context.Yarn, "YarnId", "YarnCount");

            return View();
        }

        // POST: Design/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Design design, string yarnWarp, string yarnWeft)
        {
            //string abc = "ñ,A,B,C,D,E,F,G,H,I,J,K,L,M,N,Ñ,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            //string[] aABC = abc.Split(",");

            //string arreglo = "A4C5A2B5A4";
            //var varchar = @"[0-9]{1,3}[*]?[A-Z(]{1}|[)]";
            //MatchCollection matches = Regex.Matches(arreglo, varchar);


        
            string[] coordenadas = design.WarpPlan.Split("|");
            string dato = "";
            for (int i = 1; i < 10; i++)
            {
                for (int j = 1; j < design.ShadesperWarp +1; j++)
                {
                     dato = dato + TieneDatos(j, i, coordenadas);
                }
            }

                
            dato = dato.Remove(dato.Length - 1);

            //string[] coordenadas2 = dato.Split("|");
            //string dato2 = "";

            //foreach (var item in coordenadas2)
            //{
            //    string[] item2 = item.Split(";");
            //    for (int i = 1; i < 10; i++)
            //    {
            //        if (i.ToString() == item2[0])
            //        {
            //            dato2 = dato2 + item2[2]  + aABC[i];
            //        }
            //    }
            //}


            if (ModelState.IsValid)
            {

                _context.Entry(design).State = EntityState.Added;
                _context.Design.Add(design);
                await _context.SaveChangesAsync();

                if (yarnWarp != null)
                {
                    yarnWarp = yarnWarp.Remove(yarnWarp.Length - 1);

                    string[] yarnWarp2 = yarnWarp.Split("|");

                    for (int i = 0; i < yarnWarp2.Count(); i++)
                    {
                        DesignYarn designYarn = new DesignYarn();
                        designYarn.DesignId = design.DesignId;
                        designYarn.YarnId = Convert.ToInt32(yarnWarp2[i]);
                        designYarn.Tipo = 99;
                        designYarn.Linea = 98;
                        _context.Entry(designYarn).State = EntityState.Added;
                        _context.DesignYarn.Add(designYarn);

                    }
                    await _context.SaveChangesAsync();
                }
                if (yarnWeft != null)
                {
                    yarnWeft = yarnWeft.Remove(yarnWeft.Length - 1);

                    string[] yarnWeft2 = yarnWeft.Split("|");

                    for (int i = 0; i < yarnWeft2.Count(); i++)
                    {
                        DesignYarn designYarn = new DesignYarn();
                        designYarn.DesignId = design.DesignId;
                        designYarn.YarnId = Convert.ToInt32(yarnWeft2[i]);
                        designYarn.Tipo = 99;
                        designYarn.Linea = 98;
                        _context.Entry(designYarn).State = EntityState.Added;
                        _context.DesignYarn.Add(designYarn);

                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["DesignId"] = new SelectList(_context.Draft, "DraftId", "Code", design.DesignId);
            ViewData["DesignId"] = new SelectList(_context.PegPlan, "PegPlanId", "Code", design.DesignId);
            return View(design);
        }
        public static string TieneDatos(int i, int j, string[] coordenadas)
        {
            string a = "";
            foreach (var item in coordenadas)
            {
               
                if (i.ToString() == item.Split(";")[0] && j.ToString() == item.Split(";")[1])
                {
                    
                    a = a + $"{item}|";
                }
            }
            return a;
        }
        // GET: Design/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var design = await _context.Design.FindAsync(id);
            if (design == null)
            {
                return NotFound();
            }
            ViewData["DraftId"] = new SelectList(_context.Draft, "DraftId", "Code", design.DesignId);
            ViewData["PegPlanId"] = new SelectList(_context.PegPlan, "PegPlanId", "Code", design.DesignId);
            return View(design);
        }

        // POST: Design/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DesignId,QualityId,DraftId,PegPlanId,Article,Healds,NoOfWarps,ShadesperWarp,EndsPerRepeat,Extras,Castouts,NoOfWefts,ShadesperWeft,PicksPerRepeat,WarpPlan,WeftPlan")] Design design)
        {
            if (id != design.DesignId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(design);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DesignExists(design.DesignId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DesignId"] = new SelectList(_context.Draft, "DraftId", "Code", design.DesignId);
            ViewData["DesignId"] = new SelectList(_context.PegPlan, "PegPlanId", "Code", design.DesignId);
            return View(design);
        }

        // GET: Design/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var design = await _context.Design
                .Include(d => d.Draft)
                .Include(d => d.PegPlan)
                .FirstOrDefaultAsync(m => m.DesignId == id);
            if (design == null)
            {
                return NotFound();
            }

            return View(design);
        }

        // POST: Design/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var design = await _context.Design.FindAsync(id);
            _context.Design.Remove(design);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DesignExists(int id)
        {
            return _context.Design.Any(e => e.DesignId == id);
        }


        public IActionResult GetColsDraws(int? Shafts, int? Ends, string data, string repeats)
        {
            return ViewComponent(typeof(WarpingDrawViewComponent), new { cols = Shafts, rows = Ends, data, repeats });
        }

        public IActionResult GetColsDrawsWeft(int? Shafts, int? Ends, string data, string repeats)
        {
            return ViewComponent(typeof(WeftingDrawViewComponent), new { cols = Shafts, rows = Ends, data, repeats });
        }
        public async Task<ActionResult<object>> CrearWapping(Design design)
        {
            _context.Design.Add(design);
            await _context.SaveChangesAsync();
            return View();
        }

        public  ActionResult GetTable (int input, int navwarp)
        {


            ViewBag.navwarp = navwarp;
            ViewBag.numero = input;
            return PartialView("_Warp");
        }

        public ActionResult<object> CheckWarp(string warpPlan, string repeatsWarp)
        {

            string[] _plan = warpPlan.Split("|");
            string[] _repeat = { };
            if (repeatsWarp != null)
            {
                _repeat = repeatsWarp.Split("|");

            }
            int count = 0;
            for (int i = 0; i < _plan.Length; i++)
            {
                string[] plan = _plan[i].Split(";");

                int columna = int.Parse(plan[1]);
                int valor = int.Parse(plan[2]);

                if (repeatsWarp != null)
                {
                    for (int j = 0; j < _repeat.Length; j++)
                    {
                        string[] repeat = _repeat[j].Split(",");
                        int inicio = int.Parse(repeat[0]);
                        int fin = int.Parse(repeat[1]);
                        int cantidad = int.Parse(repeat[2]);

                        if (columna >= inicio && columna <= fin)
                        {
                            count = (valor * cantidad) + count;
                        }
                        else
                        {
                            count = count + valor;
                        }
                    }
                }
                else
                {
                    count = count + valor;
                }
                    

            }

            return count;
        }
        //public async Task<ActionResult<object>> GuardarWarp(Yarn Persona)
        //{
        //    var personaIngresada = _context.Personas.Where(x => x.Rut == Persona.Rut).FirstOrDefault();

        //    if (personaIngresada != null)
        //    {
        //        return Json(new { success = false, personaIngresada });
        //    }

        //    Persona.AddUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.Actor).Value);
        //    Persona.Activo = true;
        //    Persona.AddDate = DateTime.Now;
        //    _context.Entry(Persona).State = EntityState.Added;
        //    _context.Personas.Add(Persona);
        //    await _context.SaveChangesAsync();

        //    //LISTADO DE PERSONAS
        //    var personas = _context.Personas.Where(x => x.Activo);


        //    var response = JsonConvert.SerializeObject((personas, Persona), Formatting.Indented,
        //        new JsonSerializerSettings()
        //        {
        //            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //        });

        //    return (personas, Persona);
        //}
    }
}
