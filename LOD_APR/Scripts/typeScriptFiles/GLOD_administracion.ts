/// <reference path="CommonHelper.ts"/>

class GLOD_Administracion extends GenericController{
    
        public treeView:TreeHelperAdmin;
        public nodeID:number;
        public nodeParent:number;
        public readonly urlGetTree:string;
        public readonly urlEdit:string;
        public readonly urlAdd:string;
        public readonly urlDelete:string;
        public readonly modalCanvas:string;
        public readonly modalEdit:string;
        public readonly modalDelete:string;
        public  formEdit: string;
        
        private modalForm:ModalHelper;
    
        constructor(){
            super();
            this.modalCanvas="#modalCanvas";
            this.modalEdit = "#modalForm";
            this.modalDelete = "#modalDelete";
            this.formEdit="#formEdit";
            this.urlGetTree = "/GLOD/Home/getTree";
            this.urlAdd = "/GLOD/Home/Create";
            this.urlEdit = "/GLOD/Home/Edit";
            this.urlDelete = "/GLOD/Home/Delete";
            this.nodeParent=0;
            this.modalForm = new ModalHelper("#modalAdministracion");
            $(".panel-load").removeClass("sk-loading");
            var tableHelper = new TableHelper('#tablaContratos', false);  
        }
    
        public initTreeAdministracion(_treeName:string,_id:number,_tipo:string){ 
            
            
            this.treeView = new TreeHelperAdmin(_treeName);

            //Funciones Para copiar objetos dentro del arbol
            $('#treeView').on("paste.jstree", (parent, node)=> {
                
                var idOrigen = node.node[0];
                var idDestino = this.treeView.SelectedNode;
                idOrigen.parents = [];
                idOrigen.children = [];
                idOrigen.children_d = [];
                idDestino.parents = [];
                idDestino.children = [];
                idDestino.children_d = [];
                console.log(idOrigen);
                console.log(idDestino);
                this.Events.bind("OnGetPartial",()=>{
                    this.initModal(null,null,null);
                });
                this.getPartial("#modalCanvas","/GLOD/Home/MoveNode",{origin: JSON.stringify(idOrigen), destination: JSON.stringify(idDestino)});

            })

            $('#treeView').on("show_contextmenu.jstree", (e, $node)=> {
                $(_treeName).jstree({
                    "contextmenu":{         
                        "items": ($node)=> {
                            return {
                                "Cut": {
                                    "separator_before": false,
                                    "separator_after": false,
                                    "label": "Cortar",
                                    "_disabled": this.treeView.DisableCutContext(),
                                    "action": (obj)=> { 
                                        this.treeView.CutNode = $node;
                                        $(_treeName).jstree().cut($node);
                                    }
                                },                         
                                "Paste": {
                                    "separator_before": false,
                                    "separator_after": false,
                                    "label":  this.treeView.GetPasteContextName(),
                                    "_disabled": this.treeView.DisablePasteContext(),
                                    "action": (obj)=> { 
                                        $(_treeName).jstree().paste($node);
                                    }
                                }
                            };
                        }
                    }
                });
    
            })
            //******************************************************************** 
            
            this.treeView.Events.bind("OnSelectNode", (node)=>{
                this.treeView.SelectedNode = node;
                //Al seleccionar el nodo
                this.nodeParent=node.data.db_id;//Define el nodo como Padre
                this.nodeID = node.data.db_id;    //Define nodoID como el nodo selccionado
                var hijos = node.children.length;//Obtiene la cantidad los hijos del nodo
                var tipo = node.data.type; //Se mandan cuando se crea el arbol
                var parents = node.parents;//Obtiene El ID padre
                //var existFA = false;
                //3 putas horas Limpia selected
                $('#treeView').on('select_node.jstree', function(e, data){
                    var countSelected = data.selected.length;
                    if (countSelected>1) {
                      data.instance.deselect_node( [ data.selected[0] ] );
                    }
                })
                this.Events.bind("OnGetPartial",()=>{
                    this.AddParamHrefArray(["#CreateCarpetas","#CreateProyectos","#CreateObras","#CreateContratos","#CreateBitacoras"],"IdEmpresa="+$("#IdEmpresa").val());
                    this.ActiveTabs();
                    if (hijos >0) {
                        $("#BtnEliminarCarpeta").addClass("hide")
                        $("#BtnEliminarObra").addClass("hide")
                        $("#BtnEliminarProyecto").addClass("hide")
                    }    
                });

                if(tipo=="ro"){                 
                    var loadUrl = "/GLOD/HOME/Details";
                    this.getPartial("#DivDetails",loadUrl,{});
                    return; 
                }                
                if(tipo=="carp"){
                    this.Events.bind("OnGetPartial",()=>{
                        //this.AddParamHrefArray(["#CreateCarpetas","#CreateProyectos","#CreateObras","#CreateContratos","#CreateBitacoras"],"IdEmpresa="+$("#IdEmpresa").val());
                        var tableHelper = new TableHelper('#tablaLogs', false); 
                        if (hijos >0) {
                            $("#BtnEliminarCarpeta").addClass("hide")
                        }    
                    });
                    var loadUrl = "/GLOD/Carpetas/Details";
                    var data={id:this.nodeID};                   
                    this.getPartial("#DivDetails",loadUrl,data);
                    return;                      
                } 
                if(tipo=="con"){
                    this.Events.bind("OnGetPartial",()=>{
                        //this.AddParamHrefArray(["#CreateCarpetas","#CreateProyectos","#CreateObras","#CreateContratos","#CreateBitacoras"],"IdEmpresa="+$("#IdEmpresa").val());
                        //var tableHelper = new TableHelper('#tablaLogs', false);   
                      
                    });
                    var loadUrl = "/GLOD/Contratos/Details";
                    var data={id:this.nodeID};
                    this.getPartial("#DivDetails",loadUrl,data);
                    return;                         
                } 
                if(tipo=="lib"){
                    this.Events.bind("OnGetPartial",()=>{
                       // var tableHelper = new TableHelper('#tablaLogs', false);  
                       // var tableHelper = new TableHelper('#tablaBitLobrasAsoc', false);  
                        //this.AddParamHrefArray(["#CreateCarpetas","#CreateProyectos","#CreateObras","#CreateContratos","#CreateBitacoras"],"IdEmpresa="+$("#IdEmpresa").val());
                       // asp_bitacoras.selectContratista.initSelectAjax();
                       // asp_bitacoras.selectContacto.initSelect();
                       // $('#IdContacto').attr("disabled", "true");
                        
                        
                       // $( "#IdContratista" ).change(()=> { 
                       //     $('#IdContacto').select2("destroy"); 
                       //     $('#IdContacto').removeAttr("disabled");
                       //     asp_bitacoras.selectContacto.initSelect();
                       //     $("#IdContacto").select2('val', {id: null, text: null});
                       //     $("#IdContacto").select2({placeholder:"< Seleccione Usuario >"}).trigger('change');
                        //    $('#IdContacto').select2({
                        //        //Dentro del ajax mandar Id de Bitacora para descartar contactos ya agregados en Bit.
                        //        ajax: {
                        //            url:  "/Admin/Contactos/getContactosASP/",
                        //            dataType: "json",
                        //            type: "GET",
                        //            data:  { IdContacto:  $( "#IdContratista" ).val(),IdBit:$( "#IdBitacora" ).val(), IdContratista: $( "#IdContratista" ).val() }     
                        //            ,
                        //            processResults: function (data) {
                        //                return {
                        //                    results: $.map(data, function (item) {
                        //                        return {
                        //                            text: item.name,
                        //                            id: item.id
                        //                        }
                        //                    })
                        //                };
                        //            }
                         //       }
                         //   });
                        //});
                        
                        //Pasar al common Helpers
                       // $('#formBitUser').bootstrapDualListbox({
                       //     selectorMinimalHeight: 160,
                       //     filterTextClear: 'Elementos No Seleccionados N°',
                       //     filterPlaceHolder: 'Filtro',
                       //     moveSelectedLabel: 'Mover Seleccionado',
                       //     moveAllLabel: 'Mover Todo',
                       //     removeSelectedLabel:'Remover Seleccionado',
                       //     removeAllLabel:'Remover Todo',
                       //     infoText:'Elementos en Lista N° {0}',
                       //     infoTextFiltered: '<span class="label label-warning">Filtrado</span> {0} from {1}',
                       //     infoTextEmpty: 'Lista Vacia',
                       //     SelectedListId:'formBitUser_Selected',
                       //     nonSelectedListId: 'formBitUser_NonSelected'
                       // });
                    
                       // $('#formBitLobras').bootstrapDualListbox({
                        //    selectorMinimalHeight: 160,
                        //    filterTextClear: 'Elementos No Seleccionados N°',
                         //   filterPlaceHolder: 'Filtro',
                         //   moveSelectedLabel: 'Mover Seleccionado',
                         //   moveAllLabel: 'Mover Todo',
                         //   removeSelectedLabel:'Remover Seleccionado',
                         //   removeAllLabel:'Remover Todo',
                         //   infoText:'Elementos en Lista N° {0}',
                         //   infoTextFiltered: '<span class="label label-warning">Filtrado</span> {0} from {1}',
                         //   infoTextEmpty: 'Lista Vacia',
                         //  SelectedListId:'formBitLobras_Selected',
                         //   nonSelectedListId: 'formBitLobras_NonSelected'
                        //});
                          //tooltip
                          //  $('[data-toggle="tooltip"]').tooltip(); 
                     });
                    var loadUrl = "/GLOD/LibroObras/Details";
                    var data={id:this.nodeID};
                    this.getPartial("#DivDetails",loadUrl,data);
                   
                    return;                      
                }   
                
                return;
            });

            this.treeView.initTree();

            //Esta Función Activa
            this.treeView.Events.bind("OnGetTreeData",()=>{
               setTimeout(() => {
                   $('#treeView').jstree(true).select_node(_tipo + _id);
                },1000);
            });
           
            this.treeView.updateTreeData(this.urlGetTree); 
       
        }

        private getTree(id)
        {   
            this.DesactiveTabs();
            $("#IdEmpresa").val(id);
            this.treeView.Events.bind("OnGetTreeData",()=>{
                setTimeout(() => {
                    $('#treeView').jstree(true).select_node("t_0_anchor");
                 },1000);
            });
            this.treeView.updateTreeData(this.urlGetTree)
        }

        private DesactiveTabs()
        {
            $(".LiTab").css("pointer-events","none");
            $("a.Tab-Admin").css("cursor","not-allowed");
        }

        private ActiveTabs()
        {
            $(".LiTab").css("pointer-events","all");
            $("a.Tab-Admin").css("cursor","default");
        }

        private initModal(data,status,xhr):void{
            this.modalForm = new ModalHelper("#modalJerarquia");
            $.validator.unobtrusive.parse($("#formJerarquia"));
            this.modalForm.initModal();
            this.modalForm.open();
        }
        private saveResult(data,status,xhr):void{
            if(data == 'true'){
                this.alert.toastOk();
                this.modalForm.close();
                return;
            } else {
                this.alert.toastErrorData(data);
            }
            this.ActiveTabs();
        }

     
        //Candidata a Borrar
        private sleep(milliseconds) {
            var start = new Date().getTime();
            for (var i = 0; i < 1e7; i++) {
              if ((new Date().getTime() - start) > milliseconds){
                break;
              }
            }
          }
    
        //Candidata a Borrar
       
    
    }