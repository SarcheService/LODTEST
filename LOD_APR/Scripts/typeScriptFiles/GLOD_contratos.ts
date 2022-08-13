/// <reference path="CommonHelper.ts"/>

class GLOD_contratos extends GenericController{
    //ELEMENTOS DE INTERFAZ DE USUARIO
    private modalCanvas:string;
    private modalCanvas2:string;
    private formName: string;
    private formName2: string;
    private modalName:string;
    private modalName2:string;
    //private urlGetTable:string;
    private modalForm:ModalHelper;
    private modalForm2:ModalHelper;
    public readonly urlGetTree:string;
    private selectResponsable: SelectHelper;
    private selectSujetoEconomico: SelectHelper;
    private selectAdmContratoCont: SelectHelper;
    private btnEval:any;
    private btnSubmit:any;

    constructor(){
        super();
        this.alert = new AlertHelper()
        this.modalCanvas="#modalCanvas";
        this.modalCanvas2="#modalCanvas2";
        this.modalName="#modalContratos";
        this.modalName2="#modalPermisos";
        this.formName="#formContratos";
        this.formName2="#formPermisos";
        this.urlGetTree = "/GLOD/Home/getTree";
        this.modalForm = new ModalHelper(this.modalName);
        this.modalForm2 = new ModalHelper(this.modalName2);
        this.selectSujetoEconomico= new SelectHelper("#IdSujEcon","/Admin/SujetoEconomico/getSujetoEconomicoJson/","< Seleccione Empresa >", true,false,true);
        this.selectAdmContratoCont= new SelectHelper("#IdAdminContrato", "","< Seleccione Adm. De Contrato >", true,false,true);
        //Para  DocumentosEP
      

    }
    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
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

       // var adm=$('#IdAdminContrato').val();
        
       $("#MontoInicialstr").mask("#.##0", {reverse: true});
        
        $('#IdDireccionContrato').select2({
            allowClear: true,
            placeholder: '<Seleccione una Dirección>',
            theme: "bootstrap"
        });

        $('#IdEmpresaFiscalizadora').select2({
            allowClear: true,
            placeholder: '<Seleccione Empresa Fiscalizadora>',
            theme: "bootstrap"
        });
        $('#IdEmpresaContratista').select2({
            allowClear: true,
            placeholder: '<Seleccione Empresa Contratista>',
            theme: "bootstrap"
        });

        $('#FechaInicioContrato').datepicker({
            language: 'es',
            format: "dd-mm-yyyy",
            today: true,
            autocomplete:'off',
            autoclose: true
        });      

       

        if( $( "#IdSujEcon" ).val()!= null){  
            $('#IdAdminContrato').select2({
            ajax: {
                url:  "/Admin/Contactos/getContactosASP2/",
                dataType: "json",
                type: "GET",
                data:  {IdContratista: $( "#IdSujEcon" ).val() },
                processResults: function (data) {
                    return {
                        results: $.map(data, function (item) {
                            return {
                                    text: item.name,
                                    id: item.id
                                    }
                                })
                            };
                        }
                    },
                placeholder:"< Seleccione Adm. De Contrato >" 
            });  
        }
      
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
        //Funcion de Guardado Oficial
        
        
        $("#guardar").click(()=> {  
           
            var IdResponsable=$("#IdRespMandante").val();
            if(IdResponsable==0){
               
              //  this.alert.toastWarningData("Debe Seleccionar ITO");
                return;
            }
            //MontoPresupContrato
            //var currency = $("#modalContratos #MontoPresupContrato").val();
            //currency=currency.replace(/\./g, "");
            //$("#modalContratos #MontoPresupContrato").val(currency);
            $(this.formName).submit();
        });
        //fin Función de Guardado
        //Función Modelo EP************ */
        $("#modalContratos #IdModeloCtto").change(()=>{
            var valor=$("#IdModeloCtto").val();
            if(valor==2)//Precio Unitario
            {
                $("#modalContratos #CostoDirecto").val($('#MontoPresupContrato').val());
                this.PorcGG();
                this.PorcUtil();
                this.PorcAnticipo();
                this.CalculoMontoContrato();
            }
            else if(valor==1)
            {
                $("#modalContratos #CostoDirecto").val(0);
                this.PorcGG();
                this.PorcUtil();
                this.PorcAnticipo();
                this.CalculoMontoContrato();
            }
        });

        $("#modalContratos #MontoPresupContrato").change(()=>{
            var valor=$("#IdModeloCtto").val();
            if(valor==2)//Precio Unitario
            {
                $("#modalContratos #CostoDirecto").val($('#MontoPresupContrato').val());
                this.PorcGG();
                this.PorcUtil();
                this.PorcAnticipo();
                this.CalculoMontoContrato();
            }
        });


        //***************************** */
        //Para validar Notas******************************************
        var Min=$("#RefEval").attr("data-min");
        var Max=$("#RefEval").attr("data-max");
        console.log(Min,Max);
        
        $(".ED").change(function() {
            console.log(this);
            var obj=this.id;
            var valor = $('#'+obj).val();
            console.log(valor);
            Max2=Max+".0";
            if (valor < Min || valor > Max2) 
            {
                $('#'+obj).val("");
                $('#'+obj).focus();
                glod_contratos.alert.toastErrorData("La nota debe ser entre " + Min + " y " + Max);
            }
        });

       /***********************************************/
        
        $( "#IdSujEcon" ).change(()=> { 
            $('#IdAdminContrato').select2("destroy"); 
            $('#IdAdminContrato').removeAttr("disabled");
            this.selectAdmContratoCont.initSelect();
            $("#IdAdminContrato").select2('val', {id: null, text: null});
            $("#IdAdminContrato").select2({placeholder:"< Seleccione Adm. De Contrato >"}).trigger('change');
            $('#IdAdminContrato').select2({
            //Dentro del ajax mandar Id de Bitacora para descartar contactos ya agregados en Bit.
            ajax: {
                url:  "/Admin/Contactos/getContactosASP2/",
                dataType: "json",
                type: "GET",
                data:  {IdContratista: $( "#IdSujEcon" ).val() }     
                ,
                processResults: function (data) {
                    return {
                        results: $.map(data, function (item) {
                            return {
                                text: item.name,
                                id: item.id
                                    }
                             })
                         };
                     }
                 },
                 placeholder:"< Seleccione Adm. De Contrato >" 
            });
        });
      

        this.modalForm.open();
        //*************************************************************************************
     
    }


    private saveResult(data,status,xhr):void{
        var data1=data.split(";");
        if(data1[0] == 'delete')
        {
            glod_administracion.treeView.Events.bind("OnGetTreeData",()=>{
                setTimeout(() => {$('#treeView').jstree(true).select_node("f_"+data1[1]);},1000);//Se recibe nodo Padre   
            });
            glod_administracion.treeView.updateTreeData(this.urlGetTree);
            this.alert.toastOk();
            this.modalForm.close();
            $(".ladda-button").ladda('stop');
            return;
        }
        else if(data1[0] == 'true')
        { 
            glod_administracion.treeView.Events.bind("OnGetTreeData",()=>{
                setTimeout(() => {$('#treeView').jstree(true).select_node("c_"+data1[1]);},1000);//EL mismo nodo  
             });
            glod_administracion.treeView.updateTreeData(this.urlGetTree);
            this.alert.toastOk();
            this.modalForm.close();
            $(".ladda-button").ladda('stop');
            return;
        } 
        else 
        {
            this.alert.toastErrorData(data);
        }
    }


    private Reconvertir(valor){
        var resultado= numeral(valor._value).format('0,0.00');
        resultado=resultado.replace(".","d");
        resultado=resultado.replace(/\,/g,".");
        resultado=resultado.replace("d",",");
        return(resultado);
    }

    private ReconvertirPorcentajes(valor){
        var resultado= numeral(valor._value).format('0,0.000000');
        resultado=resultado.replace(".","d");
        resultado=resultado.replace(/\,/g,".");
        resultado=resultado.replace("d",",");
        return(resultado);
    }

    public processDate(date){
        var aux = date.split("-");
        var fecha=new Date(); 
        fecha=new Date(parseInt(aux[2]),parseInt(aux[1])-1,parseInt(aux[0]));
        return fecha;
    }

   public unmaskDinero(dinero) {
        return Number(dinero.replace(/\./g, ""));
    }

    //4-01-2019
    private LoadLadda(){
        var btns = $(".ladda-button").ladda();
        btns.ladda('start');
        this.btnSubmit = $('#btnSubmit').ladda();
        this.btnSubmit.submit();
        $("#btnCancel").prop("disabled",true);
      
    }

    private  Logs(IdContrato)
    {
        this.getPartial("#divTableLogs","/GLOD/Contratos/Logs",{id:IdContrato});
        return;
        
    }


    private initModalPermisos(data,status,xhr):void{
        $.validator.unobtrusive.parse($("#formAnticipo"));
        this.modalForm.initModal(); 
        $("#formAnticipo").ajaxForm({
            beforeSend: ()=>{
                $('.panel-load').addClass('sk-loading');
            },
            success: (data)=>{
                this.saveResultAsinarARol(data,null,null);
            },            
            complete: ()=>{
                $(".panel-load").removeClass("sk-loading");  
            }   
        }); 

        $('#UserId').select2({
            allowClear: true,
            placeholder: '<Seleccione Usuario>',
            theme: "bootstrap"
        });
        //**Para Anticipo */
        $('#FechaEP').datepicker({
            language: 'es',
            format: "dd-mm-yyyy",
            today: true,
            todayHighlight: true,
            autoclose: true
        });
        $("#guardarAnticipo").click(()=> {  
            //MontoPresupContrato
            //var currency = $("#modalContratos #PagoAnticipo").val();
            //currency=currency.replace(/\./g, "");
            //$("#modalContratos #PagoAnticipo").val(currency);
            $(this.formName).submit();
        });
        this.btnSubmitAprobarDocEp = $('#btnSubmit').ladda();
        this.modalForm.open();
    }

    private saveResultAsinarARol(data,status,xhr):void{
        var data1 = data.split(";");
            if(data1[0]=="true")
            {
                this.Events.bind("OnGetPartial",()=>{
                    var tableHelper = new TableHelper('#tablaEPs',true);
                });
             
                this.getPartial("#divTableRoles","/GLOD/Contratos/getTableRoles",{id:data1[1]});
                this.alert.toastOk();
                this.modalForm.close();
                $(".ladda-button").ladda('stop');
                return;
            }
            else
            {
                $('#btnSubmit').stop();
                this.alert.toastErrorData(data);
                $(".ladda-button").ladda('stop');
            }
    }

    private initModalActivacion(data,status,xhr):void{
        $.validator.unobtrusive.parse($("#formAnticipo"));
        this.modalForm.initModal();
        this.modalForm.open();
    }

    private initActivacionFEA(id):void{
        var startTime = new Date().getTime();
        var interval = setInterval(() => {
            if(new Date().getTime() - startTime > 120000){
                clearInterval(interval);
                return;
            }
            console.log("Nuevo Intervalo..");
            axios.get('/GLOD/LibroObras/GetActivateState/' + id).then((response) => {
                    let val = response.data;
                    if (val.Status) {
                        this.getPartial("#divInfoLibros", "/GLOD/LibroObras/getLibro", { id: val.Parametros });
                        this.alert.toastOk();
                        this.modalForm.close();
                        clearInterval(interval);
                        return;
                    }
                })
                .catch(function (error) {
                    console.error(error);
                    this.ProgressBarValue = 0;
                    this.IsSending = false;
                });

            }, 5000);
    }

    private saveResultActivacion(data,status,xhr):void{
        var data1 = data.split(";");
            if(data1[0]=="true")
            {
                this.getPartial("#divInfoLibros","/GLOD/LibroObras/getLibro",{id:data1[1]});
                this.alert.toastOk();
                this.modalForm.close();
                //$(".ladda-button").ladda('stop');
                return;
            }
            else
            {
                $('#btnSubmit').stop();
                this.alert.toastErrorData(data);
                $(".ladda-button").ladda('stop');
            }
    }


    private initModalPermisosRol(data,status,xhr):void{
        $.validator.unobtrusive.parse($("#formPermisos"));
        $('#IdLod').select2({
            allowClear: true,
            placeholder: '<Seleccione Libro>',
            theme: "bootstrap"
        });

        this.modalForm2.initModal();
        this.modalForm2.open();
    }

    private initModalEditPermisosRol(data,status,xhr):void{
        $.validator.unobtrusive.parse($("#formPermisos"));
        this.modalForm2.initModal();
        this.modalForm2.open();
    }

    private saveResultPermisos(data,status,xhr):void{
        var data1 = data.split(";");
            if(data1[0]=="true")
            {
                
                this.getPartial("#divPermisosRol","/GLOD/Contratos/GetPermisosRol",{id:data1[1]});
                
                //this.getPartialContentTypeJson("#divPermisosRol","/GLOD/Contratos/GetPermisosRol",{id:data1[1]});
                
                this.alert.toastOk();
                this.modalForm2.close();
                $(".ladda-button").ladda('stop');
                return;
            }
            else
            {
                $('#btnSubmit').stop();
                this.alert.toastErrorData(data);
                $(".ladda-button").ladda('stop');
            }
    }
}