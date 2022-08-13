/// <reference path="CommonHelper.ts"/>

class GLOD_LibroObras extends GenericController{
    //ELEMENTOS DE INTERFAZ DE USUARIO
    private modalCanvas:string;
    private formName: string;
    private modalName:string;
    public readonly urlGetTree:string;
    private modalForm:ModalHelper
    private selectContratista: SelectHelper;
    private selectContacto: SelectHelper;
    private btnSubmitLadda:any;

    constructor(){
        super();
        this.alert = new AlertHelper()
        this.modalCanvas="#modalCanvas";
        this.modalName="#modalLibros";
        this.formName="#formLibros";
        this.urlGetTree = "/GLOD/Home/getTree";
        this.modalForm = new ModalHelper(this.modalName);
        //this.selectContratista= new SelectHelper("#IdContratista","/Admin/SujetoEconomico/getSujetoEconomicoJson/","< Seleccione Contratista >", true,false,true);
        //this.selectContacto= new SelectHelper("#IdContacto","","< Seleccione Usuario >", true,false,true);
        //this.InitBit(); 
        $('[data-toggle="tooltip"]').tooltip();

        /*
        $('.input-daterange').datepicker({
            language: 'es',
            format: "dd-mm-yyyy",
            today: true,
            todayHighlight: true,
            autoclose: true
            
        });
        $("#IdEstado").select2({
            allowClear: true,
            placeholder: 'Seleccione un estado',
            theme: "bootstrap"
        });
        $("#UserId").select2({
            allowClear: true,
            placeholder: 'Remitentes',
            theme: "bootstrap"
        });*/

        $('.clickable').click(function() {
            $('#home').html('<embed src="./files/'+this.id+'_4b/'+this.id+'_ejemplo.pdf" type="application/pdf" width="100%" height="400px" class="margenfilemodal" />')
        });
  
       
    }

    private initModalDelete(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        this.modalForm.open();
    }
   
    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        $("#IdTipoLod").select2({
            allowClear: true,
            placeholder: '< Seleccione un Tipo Libro >',
            theme: "bootstrap"
        });

        $(this.formName).ajaxForm({
            beforeSend: ()=>{
                $('.panel-load').addClass('sk-loading');
            },
            uploadProgress: (event,position,total, percentComplete)=>{
            },
            success: (data)=>{
                this.saveResult(data,null,null);

            },            
            complete: ()=>{
                $(".panel-load").removeClass("sk-loading");
            }
        
        });        
        $('#fileImage').on('change', () => {
            $(".panel-load").addClass("sk-loading");
            var fileUpload = $("#fileImage").get(0);
            var files = fileUpload.files;
            var data = new FormData();
            if (files.length == 0) { 
                $(".panel-load").removeClass("sk-loading");    
                this.alert.toastErrorData("Debe ingresar una Foto!");
                return;
            }         
            data.append("fileImage", files[0]);
            this.Events.bind("OnSuccessSaveFile", (response) => {
                var r = response.split(';');
                var data = { url: r[1] };
                if (r[0] == "true") {
                    this.getPartial("#divImagenContrato","/Admin/SujetoEconomico/getImagenPreview/",data);
                    return;
                } else {
                    this.alert.toastErrorData(response);
                }
            });

            

            this.postFileToDB(data,"/Admin/SujetoEconomico/AddImagenPreview/");
        });

       


        this.modalForm.open();
    }

    private saveResult(data,status,xhr):void{
        var data1=data.split(";");
        if(data1[0] == 'delete')
        {
            glod_administracion.treeView.Events.bind("OnGetTreeData",()=>{
                setTimeout(() => {$('#treeView').jstree(true).select_node("c_"+data1[1]);},1000);//Se recibe nodo Padre  
             });
             glod_administracion.treeView.updateTreeData(this.urlGetTree);
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else if(data1[0] == 'true')
        { 
            glod_administracion.treeView.Events.bind("OnGetTreeData",()=>{
                setTimeout(() => {$('#treeView').jstree(true).select_node("l_"+data1[1]);},1000);//EL mismo nodo
             });
            glod_administracion.treeView.updateTreeData(this.urlGetTree); 
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else 
        {
            this.alert.toastErrorData(data);
        }
    }  

    private Filtro(){
        this.btnSubmitLadda = $('#btnFilter').ladda();
        this.btnSubmitLadda.ladda('start');
    
        var filtro = {
            IdEstado:$('#IdEstado').val(),            
            FDesde:$('#FDesdeFilter').val(),
            FHasta:$('#FHastaFilter').val(),
            UserId:$('#UserId').val(),
            IdBitacora:$('#IdBitacora').val(),
            searchCuerpo: $('#searchCuerpo').val()
        }
        this.Events.bind("OnGetPartial",()=>{
            var tableHelper = new TableHelper('#datatableAnot1',true);
            var btns = $(".ladda-button").ladda();
            btns.ladda('stop');
        });
    
        this.getPartialContentTypeJson("#PanelAnotaciones","/ASP/Bitacoras/Filtro",filtro,true);
    }
    
    private CleanFiltro(){
        this.btnSubmitLadda = $('#btnRemoveFiltro').ladda();
        this.btnSubmitLadda.ladda('start');
    
        $('#IdEstado').val('').trigger('change')           
        $('#FDesdeFilter').val("")
        $('#FHastaFilter').val("")
        $('#UserId').val('').trigger('change')
        $('#searchCuerpo').val("")
    
        this.Events.bind("OnGetPartial",()=>{
            var tableHelper = new TableHelper('#datatableAnot1',true);
            var btns = $(".ladda-button").ladda();
            btns.ladda('stop');
        });
     
        this.getPartialContentTypeJson("#PanelAnotaciones","/ASP/Bitacoras/RemoveFiltro",{IdBitacora: $('#IdBitacora').val()},true);
    }
    
    public getPartialContentTypeJson(_panelName:string,_url:string,_data:any, FireEvent = true):void{
    
        $.ajax({
        url: _url,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(_data),
        async: false,
        beforeSend: ()=> {
            $(".panel-load").addClass("sk-loading");
        },
        success: (result)=> {
            $(".panel-load").removeClass("sk-loading");
            $(_panelName).html(result);
            if (FireEvent)
            {
                this.Events.call("OnGetPartial");                    
            }
        }
    });
    
    }

    private GetFiltroRapido(cs,IdBit):void{
        this.Events.bind("OnGetPartial",()=>{
            this.InitBit();   
        });
     
        this.getPartial("#PanelAnotaciones" , "/ASP/Bitacoras/GetFiltroRapido" , {cs:cs,IdBit:IdBit});  
      
    }

    /*
    private InitBit():void{
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',  
        });
    
        $(function () {
            var checkAll = $('input.chkTodos');
            var checkboxes = $('input.chkInd');
    
            checkAll.on('ifChecked ifUnchecked', function (event) {
                if (event.type == 'ifChecked') {
                    checkboxes.iCheck('check');
                } else {
                    checkboxes.iCheck('uncheck');
                }
            });
    
            checkboxes.on('ifChanged', function (event) {
                if (checkboxes.filter(':checked').length == checkboxes.length) {
                    checkAll.prop('checked', 'checked');
                } else {
                    checkAll.removeProp('checked');
                }
                checkAll.iCheck('update');
            });
        
        });
        
        var tableHelper = new TableHelper('#datatableAnot1',true);

       // $('#datatableAnot1').DataTable({
          //  searching: false,
           // ordering: false,
        //});
    }*/
    
        //Pasar a common helpers
    
    private GetUserBit(IdBit):void{
        this.Events.bind("OnPostCustomData", (response) => {
            if (response =='true') {
               this.alert.toastOk(); 
            }else
            {
             this.alert.toastError(); 
            }
            $(".panel-load").removeClass("sk-loading");
        });
       $(".panel-load").addClass("sk-loading");
       var LstOptionsSelected= $("#formBitUser_Selected option").map(function() 
       {
       return $(this).val();
       }).get();
      
       var LstOptionsNotSelected= $("#formBitUser_NonSelected option").map(function() 
       {
           return $(this).val();
       }).get();
       
       this.postCustomDataAsync({LstOptionsSelected:LstOptionsSelected, LstOptionsNotSelected:LstOptionsNotSelected,IdBit:IdBit},"/ASP/Bitacoras/SetUserBits");
      
    }

    private RestaurarUserBit(IdBit):void{
        this.Events.bind("OnGetPartial",()=>{ 
            $('#formBitUser').bootstrapDualListbox({
                selectorMinimalHeight: 160,
                filterTextClear: 'Elementos No Seleccionados N°',
                filterPlaceHolder: 'Filtro',
                moveSelectedLabel: 'Mover Seleccionado',
                moveAllLabel: 'Mover Todo',
                removeSelectedLabel:'Remover Seleccionado',
                removeAllLabel:'Remover Todo',
                infoText:'Elementos en Lista N° {0}',
                infoTextFiltered: '<span class="label label-warning">Filtrado</span> {0} from {1}',
                infoTextEmpty: 'Lista Vacia',
                SelectedListId:'formBitUser_Selected',
                nonSelectedListId: 'formBitUser_NonSelected'
            });
          });
     this.getPartialAsync("#divDualList","/ASP/Bitacoras/Restaurar/",{IdBit:IdBit});
    
    }

    private InsertContactoABit(IdBit) {
        var IdContacto= $('#IdContacto').val();
        this.Events.bind("OnPostCustomData", (response) => {

            this.getPartial("#divContactosBit","/ASP/Bitacoras/GetContactosBit/",{IdBit:IdBit/*, IdContratista: $( "#IdContratista" ).val()*/});
         
         });
        this.postCustomDataAsync({IdBit:IdBit ,IdContacto:IdContacto},"/ASP/Bitacoras/SetContactoBit/");
       
        
    }

    private GetLobrasBit(IdBit):void{
       
            this.Events.bind("OnPostCustomData",(result)=>{ 
                if ("true") {
                    this.Events.bind("OnGetPartial",(result)=>{ });
                    this.alert.toastOk();
                    this.getPartial("#divTableHistorialBit","/ASP/Bitacoras/GetBitLobrasAsoc", {IdBit:IdBit})
                    var tableHelper = new TableHelper('#tablaBitLobrasAsoc', false); 
                }
            });

            var LstOptionsSelected= $("#formBitLobras_Selected option").map(function() 
            {
            return $(this).val();
            }).get();
           
            var LstOptionsNotSelected= $("#formBitLobras_NonSelected option").map(function() 
            {
                return $(this).val();
            }).get();
            
            this.postCustomDataAsync({LstOptionsSelected:LstOptionsSelected, LstOptionsNotSelected:LstOptionsNotSelected,IdBit:IdBit},"/ASP/Bitacoras/SetLobrasBit","2");
           
    }


    private RestaurarLobrasBit(IdBit):void{
        
    this.Events.bind("OnGetPartial",(result)=>{ 

     $('#formBitLobras').bootstrapDualListbox({
                selectorMinimalHeight: 160,
                filterTextClear: 'Elementos No Seleccionados N°',
                filterPlaceHolder: 'Filtro',
                moveSelectedLabel: 'Mover Seleccionado',
                moveAllLabel: 'Mover Todo',
                removeSelectedLabel:'Remover Seleccionado',
                removeAllLabel:'Remover Todo',
                infoText:'Elementos en Lista N° {0}',
                infoTextFiltered: '<span class="label label-warning">Filtrado</span> {0} from {1}',
                infoTextEmpty: 'Lista Vacia',
                SelectedListId:'formBitLobras_Selected',
                nonSelectedListId: 'formBitLobras_NonSelected'
    });   
    }) ;
      
    this.getPartial("#divDualList2","/ASP/Bitacoras/RestaurarLobrasBit/",{IdBit:IdBit});

    }

        private addFav(id, idlib):void{
            $("#" + id).toggleClass('text-warning');
            $("#" + id).toggleClass('fa-star');
            $("#" + id).toggleClass('fa-star-o');
            this.Events.bind("OnPostCustomData",(result)=>{ 
                if (result == "destacada") {
                       this.alert.toastOk();
                       this.GetPanelOpc(idlib);
                }else if(result =="nodestacada")
                {
                    this.alert.toastOk();
                    this.GetPanelOpc(idlib);
                }
                    else if (result == "error") {
                        this.alert.toastErrorData("Ha ocurrido un error durante la modificación. Intentelo nuevamente"); 
                    }     
            });
            this.postCustomData({id:id},"/AnotacionesBit/addDestacada/")
           
            
        }

        private addFavAsoc(id, idbit):void{
            $("#" + id).toggleClass('text-warning');
            $("#" + id).toggleClass('fa-star');
            $("#" + id).toggleClass('fa-star-o');
            this.Events.bind("OnPostCustomData",(result)=>{ 
                if (result == "destacada") {
                       this.alert.toastOk();
                       this.GetPanelOpc(idbit);
                }else if(result =="nodestacada")
                {
                    this.alert.toastOk();
                    this.GetPanelOpc(idbit);
                }
                    else if (result == "error") {
                        this.alert.toastErrorData("Ha ocurrido un error durante la modificación. Intentelo nuevamente"); 
                    }     
            });
            this.postCustomData({id:id,idbit:idbit},"/AnotacionesBit/addDestacadaAsoc/")
            
            
        }


        private MarcarDest(cs,IdBit):void{
            $(".panel-load").addClass("sk-loading");
            var array = [];
            var count = 0;
            $('input:checkbox[id*="chkInd"]').each(function () {
                if ($(this).is(':checked')) {
                    array[count] = $(this).val();
                    count++;
                }
            });
    
             if (array.length > 0) { 
                this.Events.bind("OnPostCustomData",(result)=>{ 
                if (result == "true") {
                    var Case = $("#Cs").val();
                    this.alert.toastOk();
                    this.GetFiltroRapido(Case,IdBit);
                    this.GetPanelOpc(IdBit)
                }
                else {
                    this.alert.toastErrorData("Ha ocurrido un problema. Por favor, notifique al administrador;"+result);
                }

            });
                this.postCustomData({ cs: cs, sel: array,IdBit:IdBit},'/ASP/AnotacionesBit/MarcarDest/');
        }
        }


        private MarcarLeidaSel(cs,IdBit):void{
            $(".panel-load").addClass("sk-loading");
            var array = [];
            var count = 0;
            $('input:checkbox[id*="chkInd"]').each(function () {
                if ($(this).is(':checked')) {
                    array[count] = $(this).val();
                    count++;
                }
            });
                if (array.length > 0) {
                    this.Events.bind("OnPostCustomData",(result)=>{ 
                        if (result == "true") {
                           
                            this.alert.toastOk();
                            var Case = $("#Cs").val();
                            this.GetFiltroRapido(Case,IdBit);
                            }
                            else 
                            {
                                this.alert.toastErrorData(result); 
                            }    
                    });
                    this.postCustomData({cs:cs,sel:array,IdBit:IdBit},"/AnotacionesBit/MarcarLeidaSel/")
                   
                }
        }

        private QuitarContactoBit(IdBit,IdContacto):void{
      
            this.Events.bind("OnPostCustomData",(result)=>{ 
                if (result == "true") {
                    this.getPartial("#divContactosBit","/ASP/Bitacoras/GetContactosBit/",{IdBit:IdBit});
                }
                else {
                    this.alert.toastError();
                }
    
            });
          this.postCustomData({IdBit:IdBit,IdContacto:IdContacto},"/ASP/Bitacoras/QuitarContactoBit/")
          
          
        }
        

        private GetPanelOpc(IdBit):void{
            this.getPartial("#PanelOpc","/ASP/Bitacoras/GetOpcionesFiltro/", {id:IdBit})
        }


    }
  
