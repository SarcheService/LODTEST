/// <reference path="CommonHelper.ts"/>

class MAE_sujetos extends GenericController{
    //ELEMENTOS DE INTERFAZ DE USUARIO
    private modalCanvas:string;
    private formName: string;
    private modalName:string;
    private urlGetTable:string;
    private urlGetTableUser:string;
    private modalNameUser:string;
    private modalFormUser:ModalHelper;
    private modalForm:ModalHelper;
    constructor(){
        super();
        this.alert = new AlertHelper();
        this.modalCanvas="#modalCanvas";
        this.modalName ="#modalSujeto";
        this.formName="#formSujeto";
        this.modalNameUser="#modalUsuario";
        this.modalFormUser = new ModalHelper(this.modalNameUser);
        this.urlGetTable="/Admin/SujetoEconomico/getTable";
        this.urlGetTableUser = "/Admin/SujetoEconomico/getTableUser";
        this.modalForm = new ModalHelper(this.modalName);
        var tableHelper = new TableHelper('#tablaDatos',true,false,true);
       // var tableEvaluaciones = new TableHelper('#tablaEvaluaciones',false, false);
        
        if($("input#Rut").length > 0)
        {
            $("input#Rut").rut({
                formatOn: 'blur',
                minimumLength: 8, // validar largo mínimo; default: 2
                validateOn: 'change' // si no se quiere validar, pasar null
            });
        }


        $("input[name=flexRadioDefault]").click(()=> {    
          
            console.log("#EsMandante",$("#EsGubernamental",$("#EsContratista");
            if($("#EsMandante1").is(':checked'))
            {
                $("#EsMandante").val(true);
                $("#EsGubernamental").val(false);
                $("#EsContratista").val(false);
            }
           if($("#EsGubernamental2").is(':checked'))
            {
                $("#EsMandante").val(false);
                $("#EsGubernamental").val(true);
                $("#EsContratista").val(false);
            } 
            if($("#EsContratista3").is(':checked'))
            {
                $("#EsMandante").val(false);
                $("#EsGubernamental").val(false);
                $("#EsContratista").val(true);
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
                this.getPartial("#divImagenSujeto","/Admin/SujetoEconomico/getImagenPreview/",data);
                return;
            } else {
                this.alert.toastErrorData(response);
            }
        });
        this.postFileToDB(data,"/Admin/SujetoEconomico/AddImagenPreview/");
    });

    };
    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        $(".panel-load").removeClass("sk-loading");
        this.modalForm.initModal();
        this.modalForm.open();
    }

    private saveResult(data,status,xhr):void{
            if(data == 'true'){
                this.Events.bind("OnGetPartial",()=>{
                    var tableHelper = new TableHelper('#tablaDatos',true,false,true);
                });
                this.getPartial("#divTableDatos",this.urlGetTable,{});
                this.alert.toastOk();
                this.modalForm.close();
                return;
            } else {
                 this.alert.toastErrorData(data);
            }
    }

    private saveResultUser(data,status,xhr):void{
        if(data == 'true'){
            this.Events.bind("OnGetPartial",()=>{
                var tableHelper = new TableHelper('#tablaDatosUser',true,false,true);
            });
            this.getPartial("#divTableDatosUser",this.urlGetTableUser,{});
            this.alert.toastOk();
            this.modalForm.close();
            return;
        } else {
             this.alert.toastErrorData(data);
        }
    }

    private initModalUser(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        this.modalFormUser.initModal();
        this.modalFormUser.open();
    }

    private submit():void{
        this.btnSubmit.ladda('start');
    }
     private clearImage(_image:string):void{
        $("#"+ _image).replaceWith('<h2 href="#" data-letters="Foto" class="data-letters-galena" style="vertical-align: middle;"></h2>');
        $("#RutaImagen").val('');
    }
     private Filtro(){
        var filtro = {
            Texto:$("#searchInput").val(),
            Activo:$('#chkActivo:checkbox:checked').length > 0,
            Inactivo:$('#chkInactivo:checkbox:checked').length > 0,
            Mandante:$('#chkMandante:checkbox:checked').length > 0,
            Gubernamental:$('#chkGob:checkbox:checked').length > 0,
            Contratista:$('#chkContra:checkbox:checked').length > 0,
           
            //Tags:JSON.stringify($("#IdTags").val())
        }

        this.Events.bind("OnGetPartial",()=>{
            var table= $('#tablaDatos').DataTable();
            table.destroy();
            var tableHelper = new TableHelper('#tablaDatos',true,false,true);
        });
        super.getPartial("#divTableDatos","/Admin/SujetoEconomico/Filtro",filtro);
        $("#divFicha").load("/Admin/SujetoEconomico/DetailsEmpty");
    }
    
}