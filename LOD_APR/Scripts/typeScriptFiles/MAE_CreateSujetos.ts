/// <reference path="CommonHelper.ts"/>

class MAE_CreateSujetos extends GenericController {

    constructor() {
        super();
        
        $("#IdCiudad").select2({
            theme: "bootstrap"
        });

        $("#RazonSocial").keyup(function () {
            var value = $(this).val();
            $("#lblRazon").text(value);
        }).keyup();

        $("#Rut").keyup(function () {
            var value = $(this).val();
            $("#lblRut").text(value);
        }).keyup();

        //Suficiente para el Rut
        $("input#Run").rut({
            formatOn: 'blur',
            minimumLength: 7, // validar largo mínimo; default: 2
            validateOn: 'change' // si no se quiere validar, pasar null
        });

        $("input#Rut").rut({ validateOn: 'change' });


        $("input#Rut").rut().on('rutInvalido', function (e) {
            var spanRut = $("span[data-valmsg-for='Rut']");
            spanRut.html("<span id='Rut-error' class=''>El RUT " + $(this).val() + " es inválido</span>")
            $("input#Rut").val("");
        });

        $("input#Rut").rut().on('rutValido', (e)=> {
            var RUT = $("input#Rut").val();
            var spanRut = $("span[data-valmsg-for='Rut']");
            this.Events.bind("OnPostCustomData", (response) => {
                if (response != "true") {
                    spanRut.html("<span id='Rut-error' class=''></span>")
                    return;
                } 
                else 
                {
                    spanRut.html("<span id='Rut-error' class=''>EL RUT ya se encuentra ingresado en nuestros registros</span>")
                    $("input#Rut").val("");
                    $("input#Rut").focus();
                    return;
                }
            });
            this.postCustomData({ RUT: RUT }, "/ADMIN/SujetoEconomico/ExisteRut/");
        });
        $("input#Rut").focus();

       

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
            
    }
}