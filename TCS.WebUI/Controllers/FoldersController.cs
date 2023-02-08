using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TCS.EF;
using TCS.EF.Entidades;

namespace TCS.WebUI.Controllers
{
    public class FoldersController : Controller
    {
        public readonly TCSContext _context;

        public FoldersController(TCSContext context)
        {
            _context = context;
        }
        public class TreeViewNode
        {
            public string id { get; set; }
            public string parent { get; set; }
            public string text { get; set; }
            public string icon { get; set; }
            public string state { get; set; }
        }
        // GET: Folders
        public async Task<IActionResult> Index()
        {
            List<Folders> roots = _context.Folders.Where(x => x.Root).ToList();

            List<Folders> childs = _context.Folders.Where(x => x.Root == false).ToList();

            ViewBag.roots = roots;
            ViewBag.childs = childs;

            List<TreeViewNode> nodes = new List<TreeViewNode>();
            Folders entities = new Folders();
            foreach (var item in roots)
            {
                nodes.Add(new TreeViewNode { id = item.FolderId.ToString(), parent = "#", text = item.Name });

                mapChildrens(item, nodes);
            }


            ViewBag.tree = JsonConvert.SerializeObject(nodes);
            return View();
        }

        public async Task<ActionResult<object>> GetTree()
        {
            List<Folders> roots = _context.Folders.Where(x => x.Root).ToList();
            List<TreeViewNode> nodes = new List<TreeViewNode>();
            Folders entities = new Folders();

            foreach (var item in roots)
            {
                nodes.Add(new TreeViewNode { id = item.FolderId.ToString(), parent = "#", text = item.Name });

                mapChildrens(item, nodes);
            }
            return (nodes);

        }
        public void mapChildrens(Folders folder, List<TreeViewNode> nodes)
        {
            List<Folders> childs = _context.Folders.Where(x => x.Root == false && x.Parent == folder.FolderId).OrderBy(x => x.FolderIndex).ToList();
            if (folder.HasChildrens)
            {
                foreach (var child in childs)
                {
                    nodes.Add(new TreeViewNode { id = child.FolderId.ToString(), parent = child.Parent.ToString(), text = child.Name, icon = null  });
                    mapChildrens(child, nodes);
                    mapFiles(child.FolderId, nodes);
                }
            }
        }

        //public void mapChildrens2(int parent, List<int> nodes)
        //{
        //    var padre = _context.Folders.Where(x => x.FolderId == parent).FirstOrDefault();

        //    nodes.Add(padre.FolderId);
        //    if (padre.FolderId != padre.Parent)
        //    {
        //        mapChildrens2(padre.Parent, nodes);
        //    }
            
        //}


        public void  mapFiles(int id, List<TreeViewNode> nodes)
        {
            List<Files> files = _context.Files.Where(x => x.FolderId == id).OrderBy(x => x.FileIndex).ToList();
            foreach (var item in files)
            {
                nodes.Add(new TreeViewNode { id = "asd", parent = item.FolderId.ToString(), text = item.FileName , icon = "fal fa-file" });
            }
        }

        public async Task<ActionResult<object>> CreateNode(int parent, string nameFolder)
        {
            int nodos = 0;
            Folders folder = new Folders();
            if (parent != 0)
            {
                
                var padre = await _context.Folders.FindAsync(parent);

                if (padre.HasChildrens)
                {
                    var hijos = await _context.Folders.Where(x => x.Parent == padre.FolderId && x.FolderId != parent).OrderByDescending(x => x.FolderIndex).FirstOrDefaultAsync();
                    folder.Root = false;
                    folder.Name = nameFolder;
                    folder.HasChildrens = false;
                    if (hijos == null)
                    {
                        folder.FolderIndex = 1;
                        folder.DepthLevel = 1;

                    }
                    else
                    {
                        folder.FolderIndex = hijos.FolderIndex + 1;
                        folder.DepthLevel = hijos.DepthLevel;
                    }
                    folder.Parent = padre.FolderId;
                    _context.Entry(folder).State = EntityState.Added;
                    _context.Folders.Add(folder);
                    _context.SaveChanges();
                }
                else
                {
                    //ACTUALIZANDO PADRE
                    padre.HasChildrens = true;
                    _context.Entry(padre).State = EntityState.Modified;
                    _context.Folders.Update(padre);

                    //NUEVO HIJO

                    folder.Root = false;
                    folder.Name = nameFolder;
                    folder.HasChildrens = false;
                    folder.FolderIndex = 1;
                    folder.Parent = padre.FolderId;
                    folder.DepthLevel = 1;
                    _context.Entry(folder).State = EntityState.Added;
                    _context.Folders.Add(folder);
                    _context.SaveChanges();

                }

                nodos = folder.FolderId;
            }
            else
            {
                folder.Root = true;
                folder.Name = nameFolder;
                folder.HasChildrens = false;
                folder.FolderIndex = 1;
                folder.Parent = 0;
                folder.DepthLevel = 0;
                _context.Entry(folder).State = EntityState.Added;
                _context.Folders.Add(folder);
                _context.SaveChanges();
                folder.Parent = folder.FolderId;
                _context.Entry(folder).State = EntityState.Modified;
                _context.Folders.Update(folder);
                _context.SaveChanges();
            }




            List<Folders> roots = _context.Folders.Where(x => x.Root).ToList();

            List<TreeViewNode> nodes = new List<TreeViewNode>();
            Folders entities = new Folders();
            foreach (var item in roots)
            {
                nodes.Add(new TreeViewNode { id = item.FolderId.ToString(), parent = "#", text = item.Name });


                //if (item.FolderId == folder.FolderId)
                //{
                //    nodes.Add(new TreeViewNode { id = item.FolderId.ToString(), parent = "#", text = item.Name, state = true });


                //}
                //else
                //{
                //    nodes.Add(new TreeViewNode { id = item.FolderId.ToString(), parent = "#", text = item.Name });

                //}
                mapChildrens(item, nodes);
            }

      
            return (nodes, nodos);
        }


        public async Task<ActionResult<object>> DeleteNode(int parent)
        {
            var nodo = await _context.Folders.FindAsync(parent);
            var padre = await _context.Folders.Where(x => x.FolderId == nodo.Parent).FirstOrDefaultAsync();

            mapChildrensDelete(nodo);

            _context.Folders.Remove(nodo);

            await _context.SaveChangesAsync();


            if (!nodo.Root)
            {
                List<Folders> childsPadre = _context.Folders.Where(x => x.Parent == padre.FolderId).ToList();
                if (childsPadre.Count() == 0)
                {
                    padre.HasChildrens = false;
                    _context.Folders.Update(padre);
                    await _context.SaveChangesAsync();
                }
            }
            
            List<Folders> roots = _context.Folders.Where(x => x.Root).ToList();
            List<TreeViewNode> nodes = new List<TreeViewNode>();
            Folders entities = new Folders();

            foreach (var item in roots)
            {
                nodes.Add(new TreeViewNode { id = item.FolderId.ToString(), parent = "#", text = item.Name });

                mapChildrens(item, nodes);
            }
            int padreId = padre.FolderId;
            return (nodes,padreId);

        }


        public void mapChildrensDelete(Folders folder)
        {
            List<Folders> childs = _context.Folders.Where(x => x.Root == false && x.Parent == folder.FolderId).ToList();
            if (folder.HasChildrens)
            {
                foreach (var child in childs)
                {
                    mapChildrensDelete(child);
                }
            }
            else
            {
                _context.Folders.Remove(folder);
                
            }
        }
        // GET: Folders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var folders = await _context.Folders
                .FirstOrDefaultAsync(m => m.FolderId == id);
            if (folders == null)
            {
                return NotFound();
            }

            return View(folders);
        }

        // GET: Folders/Create
        public IActionResult Create()
        {
            return View();
        }

       
        
        // POST: Folders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FolderId,Root,Name,FolderIndex,Parent,DepthLevel")] Folders folders)
        {


            if (ModelState.IsValid)
            {



                _context.Add(folders);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(folders);
        }

        // GET: Folders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var folders = await _context.Folders.FindAsync(id);
            if (folders == null)
            {
                return NotFound();
            }
            return View(folders);
        }

        // POST: Folders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FolderId,Root,Name,FolderIndex,Parent,DepthLevel")] Folders folders)
        {
            if (id != folders.FolderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(folders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoldersExists(folders.FolderId))
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
            return View(folders);
        }

        // GET: Folders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var folders = await _context.Folders
                .FirstOrDefaultAsync(m => m.FolderId == id);
            if (folders == null)
            {
                return NotFound();
            }

            return View(folders);
        }

        // POST: Folders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var folders = await _context.Folders.FindAsync(id);
            _context.Folders.Remove(folders);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FoldersExists(int id)
        {
            return _context.Folders.Any(e => e.FolderId == id);
        }
    }
}
