class GLOD_Liquidacion extends GenericController{
    //ELEMENTOS DE INTERFAZ DE USUARIO
    private modalCanvas:string;
    private formName: string;
    private modalName:string;
    private urlGetTable:string;
    private selectTipo: SelectHelper;
    private modalForm:ModalHelper;

    constructor(){
        super();
        this.alert = new AlertHelper()
        this.modalCanvas="#modalCanvas";
        this.modalName="#modalLiquidacion";
        this.formName="#formLiquidacion";
        //this.urlGetTable="/GLOD/ClassOnee/getTable";   
        this.modalForm = new ModalHelper(this.modalName);
        //var tableHelper = new TableHelper('#tablaDatos', false,true,true);

        $("#IdContrato").select2({
            placeholder: {
                id: '0', // the value of the option
                text: 'Seleccione un Contrato'
              },
                allowClear: true,
                theme: "bootstrap"
        });

        $("#IdContrato").on('change', (e)=> {
            this.Events.bind("OnGetPartial",()=>{
                $("#tablaDatos").dataTable().fnDestroy();
                $("#tablaLibros").dataTable().fnDestroy();
                var tableHelper = new TableHelper('#tablaDatos',false,true,true);
                var tableHelper = new TableHelper('#tablaLibros',false,true,true);
            });
            let idContrato:number = $("#IdContrato").val();
            this.getPartialAsync("#getInfoContrato", "/GLOD/LiquidacionContrato/GetInfoContrato", { id: idContrato });
            this.getPartialAsync("#getDocumentos", "/GLOD/LiquidacionContrato/GetDocumentos", { id: idContrato });
            this.getPartialAsync("#getLibros", "/GLOD/LiquidacionContrato/GetLibros", { id: idContrato });
        });

    }

    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        this.modalForm.open();
    }


    private LoadLadda(){
        var btns = $(".ladda-button").ladda();
        btns.ladda('start');
        this.btnSubmit = $('#btnSubmit').ladda();
        this.btnSubmit.submit();
        $("#btnCancel").prop("disabled",true);
      
    }

    private saveResult(data,status,xhr):void{
        var data1 = data.split(";");
        if(data1[0] == 'true'){
            //this.Events.bind("OnGetPartial",()=>{
              //  var tableHelper = new TableHelper('#tablaDatos',true);
            //});
            this.getPartialAsync("#getInfoContrato", "/GLOD/LiquidacionContrato/GetInfoContrato", { id: data1[1] });
            this.getPartialAsync("#getDocumentos", "/GLOD/LiquidacionContrato/GetDocumentos", { id: data1[1] });
            this.getPartialAsync("#getLibros", "/GLOD/LiquidacionContrato/GetLibros", { id: data1[1] });
            this.alert.toastOk();            
            this.modalForm.close();
            $(".ladda-button").ladda('stop');
            return;
        } else {
             this.alert.toastErrorData(data);
        }
}

}