/// <reference path="CommonHelper.ts"/>
//sumary
//Versión 1.0 para Galena Suite experta by MDTech Ltda.
//Fecha 17-04-2017
class LOD_RepContrato extends GenericController{
    //ELEMENTOS DE INTERFAZ DE USUARIO
    private readonly modalFormName:string;
    private readonly modalDeleteName:string;
    private readonly filesFormName:string;
    private readonly filesDeleteName:string;
    private readonly inputFileName:string;
    public readonly  modalCanvas:string;
    public readonly modalCanvas2:string;
    private readonly tableName:string;
    private readonly tableDivName:string; 
    private readonly urlAddModel:string; 
    private readonly urlGetModel:string; 
    private formValidator:any;
    private modalForm:ModalHelper;
    private modalFormAdd:ModalHelper;
    private modalFormInfo:ModalHelper;
    //VARIABLES DE DATOS PARA USO DE LA CLASE
    private listFiles:FormData[];
    private IdDoc:number;
    private PrimaryKey:number;
    private btnSubmit:any;
    private IdPadre:number;   
    private language:any;  

    constructor(controller = "/GLOD/RepContrato"){
        super();
        this.modalCanvas="#modalCanvas";
        this.modalCanvas2="#modalCanvas2";
        this.modalFormName="#modalFormDocs";
        this.modalDeleteName="#modalFormDelete";
        this.filesFormName = "#formDocs";
        this.filesDeleteName = "#formDeleteDocs";
        //this.inputFileName="#fileName";
        this.tableDivName = "#lstDocs_Repositorio";
        this.tableName = "#tablaDocs_Repositorio";
        this.urlAddModel = "/GLOD/RepContrato/AddFile";
        this.urlGetModel = controller + '/GetDocumentos';
        this.listFiles= [];
        this.IdPadre=0;
        var tableHelper = new TableHelper('#tablaDocs_Repositorio', false,true,true);
        
        this.language = {
            "sProcessing":     "Procesando...",
            "sLengthMenu":     "Mostrar _MENU_ registros",
            "sZeroRecords":    "No se encontraron resultados",
            "sEmptyTable":     "No se encontraron datos para mostrar",
            "sInfo":           "Mostrando _START_ al _END_ de  _TOTAL_ registros",
            "sInfoEmpty":      "Mostrando 0 al 0 de 0 registros",
            "sInfoFiltered":   "(filtrado de un total de _MAX_ registros)",
            "sInfoPostFix":    "",
            "sSearch":         "Buscar:",
            "sUrl":            "",
            "sInfoThousands":  ",",
            "sLoadingRecords": "Cargando...",
            "oPaginate": {
                "sFirst":    "Primero",
                "sLast":     "Último",
                "sNext":     "Siguiente",
                "sPrevious": "Anterior"
            }
        };
        
        
    }

    private onBegin():void{
        this.btnSubmit.ladda('start');
        $('.panel-load').addClass('sk-loading');
    }



    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.filesFormName));
        this.modalForm = new ModalHelper(this.modalFormName);
        var tableHelper = new TableHelper('#tableDoc',false, true, true);
        this.modalForm.initModal();
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });
        this.modalForm.open();
    }

    private initModalDelete(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.filesDeleteName));
        this.modalForm = new ModalHelper(this.modalDeleteName);
        this.modalForm.initModal();
        this.modalForm.open();
    }

    private InitFormFile(_idPadre:number):void{

        this.IdPadre= _idPadre;
        
        this.modalFormAdd = new ModalHelper(this.modalFormName);
        $.validator.unobtrusive.parse($(this.filesFormName));
        this.modalFormAdd.initModal();
        this.modalFormAdd.open();
        this.btnSubmit = $("#btnSubmit").ladda();
        $(".panel-load").removeClass("sk-loading");

        $('#Fecha_Documento').datepicker({
            language: 'es',
            format: "dd-mm-yyyy",
            today: true,
            todayHighlight: true,            
            autoclose: true
        });

        $('#FechaVto').datepicker({
            language: 'es',
            format: "dd-mm-yyyy",
            today: true,
            todayHighlight: true,
            autoclose: true
        });

        $(this.filesFormName).ajaxForm({
            beforeSend: ()=>{
                this.onBegin();
            },
            uploadProgress: (event,position,total, percentComplete)=>{
                $("#progressBar").css("width", percentComplete + "%");
                $("#progressBar").text(percentComplete + "%")
            },
            success: (data,status,xhr)=>{
                this.SaveResult(data,status,xhr);
            },            
            complete: ()=>{
                $(".panel-load").removeClass("sk-loading");
                var btns = $(".ladda-button").ladda();
                btns.ladda('stop');
                $("#progressBar").css("width", "0%");
                $("#progressBar").text("0%")
            }  
        });

        $('#Fecha_Documento').datepicker({
            language: 'es',
            format: "dd-mm-yyyy",
            today: true,
            todayHighlight: true,            
            autoclose: true
        });

        $('#FechaVto').datepicker({
            language: 'es',
            format: "dd-mm-yyyy",
            today: true,
            todayHighlight: true,
            autoclose: true
        });

        $("#fileName").change(function(){
            $("#btnSubmit").prop("disabled", this.files.length == 0);
        });        
    }

    private SaveResult(data,status,xhr):void{
        
        console.log(data);
        
        var res = data.split("|");

        console.log(res);
        
        
        
        if(res[0] == 'true')
        {
            
            if(res[1] != undefined)
            {
                this.IdPadre = res[1];
            }
            
            this.GetListDocs(this.IdPadre);
         
            this.InitDatatablejs("/GLOD/RepContrato/GetDocumentosPage?Padre="+this.IdPadre);
            this.alert.toastOk();
            

            if($("#modalFormDocs").length > 0)
            {
                this.modalFormAdd.close();
            }

            if($("#modalFormDocsInfo").length > 0)
            {
                this.modalFormInfo.close();
            }

            return;
        } 
        else 
        {
            $(".panel-load").removeClass("sk-loading");
            this.alert.toastErrorData(data);
            this.btnSubmit.ladda('stop');   
        }
    }

    private GetListDocs(_idPadre:number){
        console.log(_idPadre,"padre");
       
        $(".panel-load").addClass("sk-loading");
        super.getPartial(this.tableDivName, this.urlGetModel, {Padre:_idPadre})
    }

    /*
    private Filtro(controller = "/BIB/Home"){


        // return false;

        var IdTipo = $("#IdTipo").val();
        if(IdTipo != null)
        {
            IdTipo = JSON.stringify(IdTipo)
        }

        var CatDoc = $("#CatDoc").val();
        if(CatDoc != null)
        {
            CatDoc = JSON.stringify(CatDoc)
        }


        let valido:boolean = true;
        let FechaFilter:number = $("#TFechaFilter").val();
        if(String(FechaFilter) != "" && FechaFilter != null){ 
           
            if($("#FDesdeFilter").val() == "" || $("#FHastaFilter").val() == ""){                
                $("span[data-valmsg-for='daterange']").html("<span id='daterange-error' >Dato obligatorio</span>")
                valido = false;
            }            
        }

        if(!valido){
            return false;
        }
        
        $('#daterange-error').remove();
        
        var filtro = {
            Texto:$("#searchInput").val(),
            UA:$("#radio-Carpeta:checked").length > 0,
            TDoc:IdTipo,
            CatDoc:CatDoc,
            Publico:$('#checkbox-0:checkbox:checked').length > 0,
            Corporativo:$('#checkbox-1:checkbox:checked').length > 0,
            Interno:$('#checkbox-2:checkbox:checked').length > 0,
            Privado:$('#checkbox-3:checkbox:checked').length > 0,
            TFecha:$('#TFechaFilter').val(),
            FDesde:$('#FDesdeFilter').val(),
            FHasta:$('#FHastaFilter').val(),
            IdJerarquia:$("#IdJerarquiaFilter").val(),
            Padre: $("#PadreId").val()
        }

        //console.log(filtro);
        //return false;
        
        
        
        //VERIFICAR QUE SELECCIONÓ POR LO MENOS 1 FILTRO        
        // if(filtro.Texto == "" && filtro.Publico == false && filtro.Corporativo == false
        // && filtro.Interno == false && filtro.Privado == false && filtro.IdJerarquia == "")
        // {
        //     return false;
        // }

        this.Events.bind("OnGetPartial",()=>{

            $(".btn-creates").addClass("hidden");

            var NumRegistro = $("#NumRegistro").val();
            
            
            this.InitDatatablejs(controller+"/GetDocumentosPage?Padre=0&FiltrosJson="+$("#FiltrosUsados").val());
        });
        this.getPartial("#lstDocs_biblioteca",controller+"/Filtro",filtro);
        
    }
    

    private DeleteFiltro(controller = "/BIB/Home"){
        
        if(controller == "/BIB/Home")
        {
            $("#checkbox-0, #checkbox-1, #checkbox-2, #checkbox-3").prop('checked', false);
        }
        
        $("#IdJerarquiaFilter,#lblJerarquiaFilter").val("");  
        $('#searchInput').val('');


        $('#CatDoc').val(null).trigger('change');
        $('#IdTipo').val(null).trigger('change');
        $('#TFechaFilter').val(null).trigger('change');
        $('.FRange').val(null);
        $('#daterange-error').remove();
        


        this.Events.bind("OnGetPartial",()=>{
            var table= $('#tablaPersonal').DataTable();
            table.destroy();

            // var NumRegistro = $("#NumRegistro").val();
            // $('#tablaDocs_biblioteca').DataTable( {
            //     "sDom": this.sDomDt,
            //     "processing": true,
            //     "serverSide": true,
            //     "ajax": controller+"/GetDocumentosPage?Padre=0",
            //     "deferLoading": NumRegistro,
            //     "lengthMenu": [this.NFilas],
            //     language: this.language
            // } );

            // $('[data-toggle="tooltip"]').tooltip();
            // //genera tooltiop en paginacion (2da pagina)
            // $('#tablaDocs_biblioteca').on('draw.dt', function() {
            //     $('[data-toggle="tooltip"]').tooltip();
            // });

            this.InitDatatablejs(controller+"/GetDocumentosPage?Padre=0");
        });
        this.getPartial("#lstDocs_biblioteca",controller+"/GetDocumentos",{Padre:0});
        
    }*/

    private PartialViewGet(_idPadre:number,controller = "/GLOD/RepContrato"):void{
        /*this.Events.bind("OnGetPartial",()=>{
            var tableHelper = new TableHelper('#tablaDocs_Repositorio', true,true,true);
        });*/
        //var tableHelper = new TableHelper('#tablaDocs_Repositorio', true,true,true);
        $('.tooltip').not(this).hide();
        this.InitDatatablejs(controller + "/GetDocumentosPage?Padre="+_idPadre);
    }

    

    public ErrorMsg(data)
    {
        this.alert.toastErrorData(data);
    }

    public check(e) {
        var tecla = (document.all) ? e.keyCode : e.which;
    
        //Tecla de retroceso para borrar, siempre la permite
        if (tecla == 8) {
            return true;
        }
    
        // Patron de entrada, en este caso solo acepta numeros y letras
        var patron = /[A-Za-z0-9 ]/;
        var tecla_final = String.fromCharCode(tecla);
        return patron.test(tecla_final);
    }

    public ValidaSpecial(event)
    {
        //var newString = event.target.value.replace(/[^A-Za-z0-9 ]/g, "");
        //console.log(newString,"replace");    
        var Speciales = event.target.value.match(/[^A-Za-z0-9 ]/g);         
        if(Speciales != null)
        {
            $("#FolderName").val(event.target.value.replace(/[^A-Za-z0-9 ]/g, ""));
            this.alert.toastWarningData("Los nombres de carpeta no puede contener caracteres especiales");
        }
    }


    private InitDatatablejs(url)
    {
        
        $.fn.DataTable.ext.pager.numbers_length = 5;
        var NumRegistro = $("#NumRegistro").val();
        $('#tablaDocs_Repositorio').DataTable( {
            "processing": true,
            "deferLoading": NumRegistro,
            "lengthMenu": [20],
            language: this.language
        } );
        
       /* $('#searchInput').keyup(function(){
            tableD.search($(this).val()).draw() ;
        })*/

        $('[data-toggle="tooltip"]').tooltip();
        //genera tooltiop en paginacion (2da pagina)
        $('#tablaDocs_Repositorio').on('draw.dt', function() {
            $('[data-toggle="tooltip"]').tooltip();
        });
    }

}