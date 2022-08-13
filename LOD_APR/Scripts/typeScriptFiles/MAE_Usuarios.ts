/// <reference path="CommonHelper.ts"/>

class MAE_Usuarios extends GenericController{
    //ELEMENTOS DE INTERFAZ DE USUARIO
    private modalCanvas:string;
    private formName: string;
    private modalName:string;
    private urlGetTable:string;
    private modalForm:ModalHelper
    private btnSubmit:any;
    constructor(){
        super();
        this.alert = new AlertHelper();
        this.modalCanvas="#modalCanvas";
        this.modalName ="#modalPassword";
        this.formName="#formPassword";
        this.urlGetTable="/Admin/Usuarios/getTableUsers";
        this.modalForm = new ModalHelper(this.modalName);
        var tableHelper = new TableHelper('#tableUsers', true,true,true);
        
        $("#IdSujEcon").select2({
            placeholder: 'Seleccione el sujeto economico',
                allowClear: true,
                theme: "bootstrap"
        });

        $("#IdSucursal").select2({
            theme: "bootstrap"
        });
      

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
       
        $('#IdSujEcon').val('').trigger('change');

        $("input[type=checkbox]").on('click', (e)=> {
            // console.log(e.currentTarget.dataset.perfil);
            var run = $("input#Run").val();
            var isChecked = $("#Activo").val();
            //var isChecked = e.currentTarget.checked;
            var IdUser = $("#Id").val();
            console.log(isChecked);
            // console.log(permiso,perfil,isChecked);
            this.ExisteUsuarioActivo(run,IdUser,isChecked);
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
                    this.getPartial("#divImagenPersonal","/Admin/Usuarios/getImagenPreviewPersona/",data);
                    return;
                } else {
                    this.alert.toastErrorData(response);
                }
            });
            this.postFileToDB(data,"/Admin/Usuarios/AddImagenPreview/");
        });

    };
    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        this.btnSubmit = $('#btnSubmit').ladda();
        this.modalForm.initModal();
        this.modalForm.open();
    }
    private submit():void{
        this.btnSubmit.ladda('start');
    }

    private clearImage(_image:string):void{
        $("#"+ _image).replaceWith('<h2 href="#" data-letters="Foto" class="data-letters-galena" style="vertical-align: middle;"></h2>');
        $("#RutaImagen").val('');
    }

    private ExisteUsuarioActivo(RUN:string,IdUser:string,checked):void{
        var numEvent = 1;
        this.Events.bind("OnPostCustomData",(data)=>{
            if(data == 'true'){
                if(numEvent == 1){
                    numEvent = numEvent + 1;
                    this.ActivarUsuario(RUN, IdUser);
                }      
            }else if(data == 'false'){
                this.DesactivarUsuario(IdUser);
            }else{
                this.alert.toastOk();
            }
        });
        this.postCustomData({ RUN: RUN,IdUser:IdUser,check: checked},'/Admin/Usuarios/ExisteUsuarioActivo/');
    }

    private ActivarUsuario(RUN:string, IdUser:string):void{
        this.getPartialCustomEvent("#modalCanvas","/Admin/Usuarios/ActivarUsuario/",{IdUser: IdUser}, "OnGetPartial");
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        //this.getPartial("modalCanvas","/Admin/Usuarios/DesactivarUsuario/",{RUN: RUN});          
        this.modalForm.open();
    }

    private DesactivarUsuario(IdUser:string):void{
        var numEvent = 1;
        this.Events.bind("OnPostCustomData",(data)=>{
            if(data == 'true'){
                if(numEvent == 1){
                    this.alert.toastOk();
                }      
            }else{
                this.alert.toastErrorData(data);
            }
        });
        this.postCustomData({ IdUser:IdUser},'/Admin/Usuarios/DesUsuario/');
    }
  
    private Filtro():void
    {
        
        var filtro = {
            Texto:$("#searchInput").val(),
            Activo:$('#chkActivo:checkbox:checked').length > 0,
            Inactivo:$('#chkInactivo:checkbox:checked').length > 0,
            Mandante:$('#chkMandante:checkbox:checked').length > 0,
            Gubernamental:$('#chkGob:checkbox:checked').length > 0,
            Contratista:$('#chkContra:checkbox:checked').length > 0,
           IdSujetoEconomico:$("#IdSujEcon").val()
        }

        this.Events.bind("OnGetPartial",()=>{
            var tableHelper = new TableHelper('#tableUsers', false);
        });
        this.getPartial("#divTableDatos","/Admin/Usuarios/Filtro",filtro);
    }




    private saveResult(data,status,xhr):void{
        var btns = $(".ladda-button").ladda();
        btns.ladda('stop');
            if(data == 'true'){
                this.Events.bind("OnGetPartial",()=>{
                    var tableHelper = new TableHelper('#tableUsers',false);
                });
                this.getPartial("#divTableDatos",this.urlGetTable,{});
                this.alert.toastOk();
                this.modalForm.close();
                return;
            } else {
                 this.alert.toastErrorData(data);
            }
    }

    private saveResult2(data,status,xhr):void{
        var btns = $(".ladda-button").ladda();
        btns.ladda('stop');
            if(data == 'true'){
                this.alert.toastOk();
                this.modalForm.close();
                return;
            } else {
                 this.alert.toastErrorData(data);
            }
    }


    private clearImage(_image:string):void{
        $("#"+ _image).replaceWith('<h2 href="#" data-letters="Foto" class="data-letters-galena" style="vertical-align: middle;"></h2>');
        $(".ruta_imagen").val('');
    }
}
