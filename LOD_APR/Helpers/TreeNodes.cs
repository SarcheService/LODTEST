﻿using LOD_APR.Areas.Admin.Models;
using LOD_APR.Areas.GLOD.Models;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LOD_APR.Helpers
{
    public static class JsTrees
    {
        public static List<TreeNode> getTreeAdminASP()

            List<TreeNode> nodos = new List<TreeNode>();
            List<CON_Contratos> contratos = db.CON_Contratos.ToList();
                    parent = parent,
            //FALTARIA AGREGAR LOS Libros ********
            List<LOD_LibroObras> Libros = db.LOD_LibroObras.ToList();

                if (Libro.IdCarpeta != null)

                }
            /*****************************************/
            nodos.OrderBy(n => n.text);


        public static List<TreeNode> getTreeContratos(string userid)

            List<TreeNode> nodos = new List<TreeNode>();

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

            
                    parent = parent,

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

           
            



            List<LOD_LibroObras> Libros = new List<LOD_LibroObras>();
            if (/*roles.Select(x => x.Name).Contains("Administrador-MOP") || roles.Select(x => x.Name).Contains("Control de Contratos") ||*/ roles.Select(x => x.Name).Contains("Administrador Plataforma") || roles.Select(x => x.Name).Contains("Administrador General")) //Roles para ver todos los contratos para el Control de Contratos, Administrador MOP y administrador de plataforma
            {
                Libros = db.LOD_LibroObras.ToList();
            }
            else
            {
                Libros = db.LOD_LibroObras.Where(x => contratosId.Contains(x.IdContrato) || x.UserId == userid).ToList();
            }


                if (Libro.IdCarpeta != null)

                }
            /*****************************************/
            nodos.OrderBy(n => n.text);

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