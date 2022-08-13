using LOD_APR.Areas.Admin.Models;
using LOD_APR.Areas.GLOD.Models;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LOD_APR.Helpers
{
    public static class JsTrees
    {
        public static List<TreeNode> getTreeAdminASP()        {            LOD_DB db = new LOD_DB();

            List<TreeNode> nodos = new List<TreeNode>();            nodos.Add(new TreeNode() { id = "t_0", text = "Archivo", parent = "#", state = new TreeState() { opened = true }, data = new TreeData() { type = "ro", db_id = 0 }, icon = "fa fa-archive" });           
            List<CON_Contratos> contratos = db.CON_Contratos.ToList();            foreach (CON_Contratos con in contratos)            {                string parent = "t_0";                int IdParent = 0;                if (con.IdCarpeta != null)                {                    parent = "f_" + con.IdCarpeta.ToString();                    IdParent = Convert.ToInt32(con.IdCarpeta);                }                TreeNode nodo = new TreeNode()                {                    id = "c_" + con.IdContrato.ToString(),                    text = con.CodigoContrato,/*(con.CodigoContrato != null) ? con.CodigoContrato + "-" + con.NombreContrato : con.NombreContrato,*/
                    parent = parent,                    icon = "fa fa-file-text-o",                    data = new TreeData()                    {                        db_id = con.IdContrato,                        parent_id = IdParent,                        leaft = true,                        type = "con"                    }                };                nodos.Add(nodo);            }            List<LOD_Carpetas> carpetas = db.LOD_Carpetas.ToList();            foreach (LOD_Carpetas carp in carpetas)            {                string parent = "t_0";                int IdParent = 0;                if (carp.IdCarpPadre != null)                {                    parent = "f_" + carp.IdCarpPadre.ToString();                    IdParent = Convert.ToInt32(carp.IdCarpPadre);                }                else if (carp.IdContrato != null)                {                    parent = "c_" + carp.IdContrato.ToString();                    IdParent = Convert.ToInt32(carp.IdContrato);                }                string iconCarp = "fa fa-folder-o";                if (carp.EsPortafolio)                {                    iconCarp = "fa fa-briefcase";                }                TreeNode nodo = new TreeNode()                {                    id = "f_" + carp.IdCarpeta.ToString(),                    text = carp.NombreCarpeta,                    parent = parent,                    icon = iconCarp,                    data = new TreeData()                    {                        db_id = carp.IdCarpeta,                        parent_id = IdParent,                        leaft = false,                        type = "carp"                    }                };                nodos.Add(nodo);            }
            //FALTARIA AGREGAR LOS Libros ********
            List<LOD_LibroObras> Libros = db.LOD_LibroObras.ToList();            foreach (LOD_LibroObras Libro in Libros)            {                string parent = "t_0";                int IdParent = 0;

                if (Libro.IdCarpeta != null)                {                    parent = "f_" + Libro.IdCarpeta.ToString();                    IdParent = Convert.ToInt32(Libro.IdCarpeta);

                }                else                {                    parent = "c_" + Libro.IdContrato.ToString();                    IdParent = Convert.ToInt32(Libro.IdContrato);                }                TreeNode nodo = new TreeNode()                {                    id = "l_" + Libro.IdLod.ToString(),                    text = Libro.NombreLibroObra,                    parent = parent,                    icon = "fa fa-book",                    data = new TreeData()                    {                        db_id = Libro.IdLod,                        parent_id = IdParent,                        leaft = true,                        type = "lib"                    }                };                nodos.Add(nodo);            }
            /*****************************************/
            nodos.OrderBy(n => n.text);            return nodos;        }


        public static List<TreeNode> getTreeContratos(string userid)        {            LOD_DB db = new LOD_DB();

            List<TreeNode> nodos = new List<TreeNode>();            List<CON_Contratos> contratos = new List<CON_Contratos>();            nodos.Add(new TreeNode() { id = "t_0", text = "Archivo", parent = "#", state = new TreeState() { opened = true }, data = new TreeData() { type = "ro", db_id = 0 }, icon = "fa fa-archive" });

            List<int> contratosId = db.LOD_UsuariosLod.Where(x => x.UserId == userid || x.LOD_LibroObras.CON_Contratos.UserId == userid).Select(x => x.LOD_LibroObras.IdContrato).Distinct().ToList();
            List<ApplicationRole> roles = db.Roles.Where(x => x.Users.Select(a => a.UserId).Contains(userid)).ToList();
            if (/*roles.Select(x => x.Name).Contains("Administrador-MOP") || roles.Select(x => x.Name).Contains("Control de Contratos") ||*/ roles.Select(x => x.Name).Contains("Administrador Plataforma") || roles.Select(x => x.Name).Contains("Administrador General")) //Roles para ver todos los contratos para el Control de Contratos, Administrador MOP y administrador de plataforma
            {
                contratos = db.CON_Contratos.ToList();
            }
            else
            {
                contratos = db.CON_Contratos.Where(x => contratosId.Contains(x.IdContrato) || x.UserId == userid).ToList();
            }

                        foreach (CON_Contratos con in contratos)            {                string parent = "t_0";                int IdParent = 0;                if (con.IdCarpeta != null)                {                    parent = "f_" + con.IdCarpeta.ToString();                    IdParent = Convert.ToInt32(con.IdCarpeta);                }                TreeNode nodo = new TreeNode()                {                    id = "c_" + con.IdContrato.ToString(),                    text =/* con.CodigoContrato,*/(con.CodigoContrato != null) ? con.CodigoContrato + "-" + con.NombreContrato : con.NombreContrato,
                    parent = parent,                    icon = "fa fa-file-text-o",                    data = new TreeData()                    {                        db_id = con.IdContrato,                        parent_id = IdParent,                        leaft = true,                        type = "con"                    }                };                nodos.Add(nodo);            }

            List<LOD_Carpetas> carpetas = new List<LOD_Carpetas>();

            if (/*roles.Select(x => x.Name).Contains("Administrador-MOP") || roles.Select(x => x.Name).Contains("Control de Contratos") ||*/ roles.Select(x => x.Name).Contains("Administrador Plataforma") || roles.Select(x => x.Name).Contains("Administrador General")) //Roles para ver todos los contratos para el Control de Contratos, Administrador MOP y administrador de plataforma
            {
                carpetas = db.LOD_Carpetas.ToList();
            }
            else
            {
                List<int> IdsCarpetasPadre = new List<int>();
                if (contratos.Where(x => x.IdCarpeta != null).Select(x => x.IdCarpeta.Value).Count() > 0)
                {
                    IdsCarpetasPadre = contratos.Where(x => x.IdCarpeta != null).Select(x => x.IdCarpeta.Value).Distinct().ToList();
                }

                foreach (var item in IdsCarpetasPadre)
                {
                    carpetas.Add(db.LOD_Carpetas.Find(item));
                    if (db.LOD_Carpetas.Find(item).IdCarpPadre != null)
                    {
                        carpetas.AddRange(getPadre(db.LOD_Carpetas.Find(item).IdCarpPadre.Value));
                    }

                }


                //Con esta query se trae las carpeta que estan dentro del contrato
                List<int> IdsCarpetasExistentes = carpetas.Select(x => x.IdCarpeta).ToList();
                IdsCarpetasExistentes = IdsCarpetasExistentes.Distinct().ToList();
                carpetas.AddRange(db.LOD_Carpetas.Where(x => (contratosId.Contains(x.IdContrato.Value) || x.UserId == userid) && !IdsCarpetasExistentes.Contains(x.IdCarpeta) /*|| x.EsPortafolio*/ ).Distinct().ToList());
                List<int> IdsCarpetasLimpio = carpetas.Select(x => x.IdCarpeta).Distinct().ToList();
                carpetas = db.LOD_Carpetas.Where(x => IdsCarpetasLimpio.Contains(x.IdCarpeta)).ToList();
            }

           
                        foreach (LOD_Carpetas carp in carpetas)            {                string parent = "t_0";                int IdParent = 0;                if (carp.IdCarpPadre != null)                {                    parent = "f_" + carp.IdCarpPadre.ToString();                    IdParent = Convert.ToInt32(carp.IdCarpPadre);                }                else if (carp.IdContrato != null)                {                    parent = "c_" + carp.IdContrato.ToString();                    IdParent = Convert.ToInt32(carp.IdContrato);                }                string iconCarp = "fa fa-folder-o";                if (carp.EsPortafolio)                {                    iconCarp = "fa fa-briefcase";                }                TreeNode nodo = new TreeNode()                {                    id = "f_" + carp.IdCarpeta.ToString(),                    text = carp.NombreCarpeta,                    parent = parent,                    icon = iconCarp,                    data = new TreeData()                    {                        db_id = carp.IdCarpeta,                        parent_id = IdParent,                        leaft = false,                        type = "carp"                    }                };                nodos.Add(nodo);            }



            List<LOD_LibroObras> Libros = new List<LOD_LibroObras>();
            if (/*roles.Select(x => x.Name).Contains("Administrador-MOP") || roles.Select(x => x.Name).Contains("Control de Contratos") ||*/ roles.Select(x => x.Name).Contains("Administrador Plataforma") || roles.Select(x => x.Name).Contains("Administrador General")) //Roles para ver todos los contratos para el Control de Contratos, Administrador MOP y administrador de plataforma
            {
                Libros = db.LOD_LibroObras.ToList();
            }
            else
            {
                Libros = db.LOD_LibroObras.Where(x => contratosId.Contains(x.IdContrato) || x.UserId == userid).ToList();
            }
            foreach (LOD_LibroObras Libro in Libros)            {                string parent = "t_0";                int IdParent = 0;

                if (Libro.IdCarpeta != null)                {                    parent = "f_" + Libro.IdCarpeta.ToString();                    IdParent = Convert.ToInt32(Libro.IdCarpeta);

                }                else                {                    parent = "c_" + Libro.IdContrato.ToString();                    IdParent = Convert.ToInt32(Libro.IdContrato);                }                TreeNode nodo = new TreeNode()                {                    id = "l_" + Libro.IdLod.ToString(),                    text = Libro.NombreLibroObra,                    parent = parent,                    icon = "fa fa-book",                    data = new TreeData()                    {                        db_id = Libro.IdLod,                        parent_id = IdParent,                        leaft = true,                        type = "lib"                    }                };                nodos.Add(nodo);            }
            /*****************************************/
            nodos.OrderBy(n => n.text);            return nodos;        }

        public static List<LOD_Carpetas> getPadre(int id)
        {
            LOD_DB db = new LOD_DB();

            LOD_Carpetas padre = db.LOD_Carpetas.Find(id);
            List<LOD_Carpetas> carpetasPadre = new List<LOD_Carpetas>();
            carpetasPadre.Add(padre);
            if (padre.IdCarpPadre != null)
            {
                carpetasPadre.AddRange(getPadre(padre.IdCarpPadre.Value));
                //if (db.LOD_Carpetas.Find(padre.IdCarpPadre).IdCarpPadre != null)
                //{
                //    getPadre(db.LOD_Carpetas.Find(padre.IdCarpPadre).IdCarpPadre.Value);
                //}
            }

            return carpetasPadre;
        }
       
        public static TreeNode getTreeCarpetasDoc()
        {
            LOD_DB db = new LOD_DB();

            //List<TreeNode> nodos = new List<TreeNode>();
            TreeNode nodoRoot = new TreeNode() { id = "t_0", text = "Repositorio de Documentos", parent = "#", state = new TreeState() { opened = true }, data = new TreeData() { type = "ro", db_id = 0, parent_id = -1 }, icon = "fa fa-archive", children = new List<TreeNode>() };

            //SELECCION DE LAS CARPETAS ROOT POR TIPO DE TAG
            List<MAE_ClassOne> one = db.MAE_ClassOne.Where(x => x.Activo).ToList();
            foreach (MAE_ClassOne o in one)
            {
                TreeNode nodo = new TreeNode()
                {
                    id = "o_" + o.IdClassOne,
                    text = o.Nombre,
                    parent = "t_0",
                    state = new TreeState()
                    {
                        opened = true
                    },
                    data = new TreeData()
                    {
                        type = "one",
                        db_id = o.IdClassOne,
                        parent_id = 0,
                    },
                    icon = "fa fa-folder-open",
                    children = new List<TreeNode>()
                };

                List<MAE_ClassTwo> two = db.MAE_ClassTwo.Where(x => x.IdClassOne==o.IdClassOne && x.Activo).ToList();
                foreach (MAE_ClassTwo t in two)
                {
                    TreeNode nodotwo = new TreeNode()
                    {
                        id = "t_" + t.IdClassTwo,
                        text = t.Nombre,
                        parent = "o_" + t.IdClassOne,
                        icon = "fa fa-folder-o",
                        data = new TreeData()
                        {
                            db_id = t.IdClassTwo,
                            parent_id = t.IdClassOne,
                            leaft = false,
                            type = "two"
                        },
                        children = new List<TreeNode>()
                    };
                    nodo.children.Add(nodotwo);
                }

                nodoRoot.children.Add(nodo);

            }
            
            return nodoRoot;

        }

        public static List<TreeNode> getTreePaths()
        {
            LOD_DB db = new LOD_DB();

            List<TreeNode> nodos = new List<TreeNode>();
            nodos.Add(new TreeNode() { id = "t_0", text = "Listado de Paths", parent = "#", state = new TreeState() { opened = true }, data = new TreeData() { type = "ro", db_id = 0 }, icon = "fa fa-archive", children = new List<TreeNode>() });

            //SELECCION DE LAS CARPETAS ROOT POR TIPO DE TAG
            List<MAE_TipoPath> tiposPaths = db.MAE_TipoPath.ToList();
            foreach (MAE_TipoPath tipo in tiposPaths)
            {
                nodos.Add(new TreeNode()
                {
                    id = "t_" + tipo.IdTipo,
                    text = tipo.TipoPath,
                    parent = "t_0",
                    state = new TreeState()
                    {
                        opened = true
                    },
                    data = new TreeData()
                    {
                        type = "ti",
                        db_id = tipo.IdTipo
                    },
                    icon = "fa fa-folder-open",
                    children = new List<TreeNode>()
                });

            }

            List<MAE_Path> Paths = db.MAE_Path.ToList();
            foreach (MAE_Path path in Paths)
            {
                TreeNode nodo = new TreeNode()
                {
                    id = "t_" + path.IdPath,
                    text = path.Path,
                    parent = "t_" + path.Padre,
                    icon = "fa fa-folder-o",
                    data = new TreeData()
                    {
                        db_id = path.IdPath,
                        parent_id = path.Padre,
                        leaft = false,
                        type = "ua"
                    },
                    children = new List<TreeNode>()
                };

                nodos.Add(nodo);

            }

            return nodos;

        }
        
        public static List<TreeNode> FlatToHierarchy(List<TreeNode> list, int parentId = 0)
        {
            try
            {
                var lista = (from i in list
                             where i.data.parent_id == parentId
                             select new TreeNode
                             {
                                 id = i.id,
                                 parent = i.parent,
                                 text = i.text,
                                 icon = i.icon,
                                 data = new TreeData()
                                 {
                                     db_id = i.data.db_id,
                                     parent_id = i.data.parent_id,
                                     type = i.data.type,
                                     leaft = i.data.leaft
                                 },
                                 children = FlatToHierarchy(list, i.data.db_id)
                             }).ToList();

                return lista;
            }
            catch(StackOverflowException ex)
            {
                return new List<TreeNode>();
            }
           
        }

        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> childSelector)
        {
            var stack = new Stack<T>(items);
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                foreach (var child in childSelector(next))
                    stack.Push(child);
            }
        }

        public static List<TreeNode> GetHijos(List<TreeNode> Original, List<TreeNode> resultado, int parentId = 0)
        {
            resultado.AddRange((from i in Original
                                where i.data.parent_id == parentId
                                select new TreeNode
                                {
                                    id = i.id,
                                    parent = i.parent,
                                    text = i.text,
                                    data = new TreeData()
                                    {
                                        db_id = i.data.db_id,
                                        parent_id = i.data.parent_id,
                                        type = i.data.type,
                                        leaft = i.data.leaft
                                    }
                                }).ToList());
            foreach (TreeNode n in resultado)
                resultado.AddRange(GetHijos(Original, resultado, n.data.db_id));

            return resultado;
        }


    }
    public class TreeNode
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public TreeState state { get; set; }
        public TreeData data { get; set; }
        public IList<TreeNode> children { get; set; }
        public int position { get; set; }
    }
    public class TreeData
    {
        public int db_id { get; set; }
        public int parent_id { get; set; }
        public string type { get; set; }
        public bool leaft { get; set; }
        //Cambio
        public int IdNivelJerarquico { get; set; }
    }
    public class TreeState
    {
        public bool opened { get; set; }
        public bool disabled { get; set; }
        public bool selected { get; set; }
    }
}