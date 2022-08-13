class Log_Home extends GenericController{
    //ELEMENTOS DE INTERFAZ DE USUARIO
    private modalCanvas:string;
    private formName: string;
    private modalName:string;
    private urlGetTable:string;
    private modalForm:ModalHelper
    private btnSubmit:any;
    private IdContrato:number;
    constructor(_IdContrato:number){
        super();
        this.alert = new AlertHelper();
        this.modalCanvas="#modalCanvas";
        this.modalName ="#modalPassword";
        this.formName="#formPassword";
        this.urlGetTable="/GLOD/Log/getTable";
        this.modalForm = new ModalHelper(this.modalName);
        this.IdContrato = _IdContrato;
        var tableHelper = new TableHelper('#tableDatos', false,true,true);



        $("#IdLod").select2({
            placeholder: {
                id: '0', // the value of the option
                text: 'Seleccione un Libro de Obra'
              },
                allowClear: true,
                theme: "bootstrap"
        }).val('0').trigger('change');
       
        var $option = $("<option selected></option>").val('0').text("Seleccione un Libro de Obra");
        $("#IdLod").append($option).trigger('change');
        $("#IdLod").val('0');

        $("#IdAnotacion").select2({
            placeholder: {
                id: '0', // the value of the option
                text: 'Seleccione una Anotación'
              },
                allowClear: true,
                theme: "bootstrap"
        }).val('').trigger('change');

        var $option = $("<option selected></option>").val('0').text("Seleccione un Anotación");
        $("#IdAnotacion").append($option).trigger('change');
        $("#IdAnotacion").val(0);

        $("#UserId").select2({
            placeholder: {
                id: '0', // the value of the option
                text: 'Seleccione un Usuario'
              },
                allowClear: true,
                theme: "bootstrap"
        }).val('').trigger('change');

        var $option = $("<option selected></option>").val('0').text("Seleccione un Usuario");
        $("#UserId").append($option).trigger('change');
        $("#UserId").val(0);

       
        $("#IdLod").on('change', (e)=> {
            let id:number = $("#IdLod").val();
            console.log("entró al change ");
            console.log(id);
            this.GetAnotaciones(id);
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

    private Filtro():void
    {
        
        var filtro = {

            IdLod:$("#IdLod").val(),
            IdAnotacion:$("#IdAnotacion").val(),
            UserId:$("#UserId").val(),
            IdContrato:this.IdContrato,
            FechaLog:$("#FechaLog").val()
        }

        this.Events.bind("OnGetPartial",()=>{
            var tableHelper = new TableHelper('#tableDatos', false);
        });
        this.getPartial("#divTableDatos","/GLOD/Log/GetFiltro",filtro);
    }

    private saveResult(data,status,xhr):void{
        var btns = $(".ladda-button").ladda();
        btns.ladda('stop');
            if(data == 'true'){
                this.Events.bind("OnGetPartial",()=>{
                    var tableHelper = new TableHelper('#tableDatos',false,true,true);
                });
                this.getPartial("#divTableDatos",this.urlGetTable,{});
                this.alert.toastOk();
                this.modalForm.close();
                return;
            } else {
                 this.alert.toastErrorData(data);
            }
    }

    private GetAnotaciones(IdLod:number):void{
        this.Events.bind("OnGetPartial",()=>{
            $(".panel-load").removeClass("sk-loading");  
            $("#IdAnotacion").select2({
                allowClear: true,
                placeholder: 'Seleccione una Anotación',
                theme: "bootstrap"
            });

        });
        $(".panel-load").addClass("sk-loading");
        this.getPartial("#DivAnotacion","/GLOD/Log/GetAnotaciones",{"IdLod":IdLod,"IdContrato":this.IdContrato});  
    }

   

    
}
