using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.Models
{
    [Table("LOD_UserAnotacion")]
    public class LOD_UserAnotacion
    {
        [Key]
        [Column(Order = 1)]
        [DisplayName("Anotación")]
        [ForeignKey("LOD_UsuarioLod")]
        public int IdUsLod { get; set; }
        public virtual LOD_UsuariosLod LOD_UsuarioLod { get; set; }

        [Key]
        [Column(Order = 2)]
        [DisplayName("Anotación")]
        [ForeignKey("LOD_Anotaciones")]
        public int IdAnotacion { get; set; }
        public virtual LOD_Anotaciones LOD_Anotaciones { get; set; }

        public bool Destacado { get; set; }
        public string TempCode { get; set; }
        public bool Leido { get; set; }

        public bool EsPrincipal { get; set; }

        public bool EsRespRespuesta { get; set; }

        public bool RespVB { get; set; }

        public bool VistoBueno { get; set; }

        public DateTime? FechaVB { get; set; }

        public int? TipoVB { get; set; }

        [NotMapped]
        public string RutaImg { get {
                if (LOD_UsuarioLod.ApplicationUser.RutaImagen == null)
                {
                    return string.Empty;
                }
                else
                {
                    return "/Images/Contactos/" + LOD_UsuarioLod.ApplicationUser.RutaImagen;
                }
            } }

        [NotMapped]
        public string Nombre { get {
                return LOD_UsuarioLod.ApplicationUser.Nombres.Split(' ')[0] + " " + LOD_UsuarioLod.ApplicationUser.Apellidos;
            } }
    }
}