/// <reference path="CommonHelper.ts"/>

class MAE_CreateUser extends GenericController{

    private modalCanvas:string;
    private formName: string;
    private modalName:string;
    private urlGetTable:string;
    private selectTipo: SelectHelper;
    private modalForm:ModalHelper;

    constructor() {
        super();
        this.alert = new AlertHelper()
        this.modalCanvas="#modalCanvas";
        this.modalName="#modalUser";
        this.formName="#formUser";
        this.urlGetTable="";   
        this.modalForm = new ModalHelper(this.modalName);


        $("#IdSucursal").select2({
            theme: "bootstrap"
        });


       //Suficiente para el Rut
       $("input#Run").rut({
        formatOn: 'blur',
        minimumLength: 7, // validar largo mínimo; default: 2
        validateOn: 'change' // si no se quiere validar, pasar null
    });

    $("input#Run").rut({ validateOn: 'change' });


    $("input#Run").rut().on('rutInvalido', function (e) {
        var spanRun = $("span[data-valmsg-for='Run']");
        spanRun.html("<span id='Run-error' class=''>El RUN " + $(this).val() + " es inválido</span>")
        $("input#Run").val("");
    });

    $("input#Run").rut().on('rutValido', (e)=> {
        var RUN = $("input#Run").val();
        var Sujeto = $("input#IdSujeto").val();
        var spanRun = $("span[data-valmsg-for='Run']");
        var numEvent = 1;
        this.Events.bind("OnPostCustomData", (response) => {
            
            if (response != "true") {
                spanRun.html("<span id='Run-error' class=''></span>")
                return;
            } 
            else 
            {
                spanRun.html("<span id='Run-error' class=''>Existe un Usuario <b>Activo</b> registrado con este RUN</span>")
                $("input#Run").val("");
                if(numEvent == 1){
                    numEvent = numEvent + 1;
                    this.initModal(RUN);
                }
                //this.getPartialCustomEvent("modalCanvas","/Admin/Usuarios/DesactivarUsuario/",{RUN: RUN},"OnGetPartial");                  
                return;
            }
        });
        this.postCustomData({ RUN: RUN, IdSujeto:Sujeto }, "/Admin/Usuarios/ExisteRut/");
        //this.Events.unbind("OnPostCustomData");
    });
    


    $("input#Email").on('change', (e)=> {
        var Correo = $("input#Email").val();
        var spanCorreo = $("span[data-valmsg-for='Email']");
        this.Events.bind("OnPostCustomData", (response) => {
            if (response != "true") {
                spanCorreo.html("<span id='Email-error' class=''></span>")
                return;
            } 
            else 
            {
                spanCorreo.html("<span id='Email-error' class=''>El Email ya se encuentra ingresado en el sistema</span>")
                $("input#Email").val("");
                return;
            }
        });
        this.postCustomData({ Correo: Correo }, "/Admin/Usuarios/ExisteCorreo/");
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
                this.getPartial("#divImagenPersonal","/Admin/SujetoEconomico/getImagenPreviewPersona/",data);
                return;
            } else {
                this.alert.toastErrorData(response);
            }
        });
        this.postFileToDB(data,"/Admin/SujetoEconomico/AddImagenPreview/");
    });
      
       
            
    }

    private initModal(RUN:string):void{
        this.getPartialCustomEvent("#modalCanvas","/Admin/Usuarios/DesactivarUsuario/",{ RUN: RUN }, "OnGetPartial");
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        //this.getPartial("modalCanvas","/Admin/Usuarios/DesactivarUsuario/",{RUN: RUN});          
        this.modalForm.open();
    }
   
    private clearImage(_image:string):void{
        $("#"+ _image).replaceWith('<h2 href="#" data-letters="Foto" class="data-letters-galena" style="vertical-align: middle;"></h2>');
        $(".ruta_imagen").val('');
    }

    private saveResult(data,status,xhr):void{
        if(data == 'true'){
            this.alert.toastOk();
            this.modalForm.close();
            return;
        } else {
             this.alert.toastErrorData(data);
        }
}
}