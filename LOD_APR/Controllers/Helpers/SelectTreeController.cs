using LOD_APR.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TreeNode = LOD_APR.Helpers.TreeNode;

namespace LOD_APR.Controllers
{
    [CustomAuthorize]
    public class SelectTreeController : Controller
    {
        //private LOD_DB db = new LOD_DB();

        //public async Task<JsonResult> GetTreeFaenas()
        //{
        //    List<TreeNode> mAE_faenas = JsTrees.getTreeFaenas();
        //    List<TreeNode> dependencias = JsTrees.FlatToHierarchy(mAE_faenas,-1);
        //    return Json(dependencias.FirstOrDefault(), JsonRequestBehavior.AllowGet);
        //}

        //public async Task<JsonResult> GetTreeMarcas()
        //{
        //    List<TreeNode> mAE_faenas = JsTrees.getTreeMarcaModelo();
        //    List<TreeNode> dependencias = JsTrees.FlatToHierarchy(mAE_faenas, -1);
        //    return Json(dependencias.FirstOrDefault(), JsonRequestBehavior.AllowGet);
        //}

        //public async Task<JsonResult> GetTreeClases()
        //{
        //    List<TreeNode> mAE_faenas = JsTrees.getTreeClaseActivo();
        //    List<TreeNode> dependencias = JsTrees.FlatToHierarchy(mAE_faenas, -1);
        //    return Json(dependencias.FirstOrDefault(), JsonRequestBehavior.AllowGet);
        //}

        //public async Task<JsonResult> GetTreeJerarquia()
        //{
        //    List<TreeNode> mAE_faenas = JsTrees.getTreeJerarquico(true);
        //    List<TreeNode> dependencias = JsTrees.FlatToHierarchy(mAE_faenas, -1);
        //    return Json(dependencias.FirstOrDefault(), JsonRequestBehavior.AllowGet);
        //}

        public async Task<JsonResult> GetTreePaths()
        {
            TreeNode mAE_repo = JsTrees.getTreeCarpetasDoc();
            //List<TreeNode> dependencias = JsTrees.FlatToHierarchy(mAE_repo, -1);
            return Json(mAE_repo, JsonRequestBehavior.AllowGet);
        }

        //public async Task<JsonResult> GetTreeContratos()
        //{
        //    List<TreeNode> mAE_faenas = JsTrees.getTreeAdminASP(1);
        //    List<TreeNode> dependencias = JsTrees.FlatToHierarchy(mAE_faenas, -1);
        //    return Json(dependencias.FirstOrDefault(), JsonRequestBehavior.AllowGet);
        //}
    }
}