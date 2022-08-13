class MAE_RolesContrato extends GenericController{
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
        this.modalName="#modalTipo";
        this.formName="#formTipo";
        this.urlGetTable="/Admin/RolesContrato/getTable";   
        this.modalForm = new ModalHelper(this.modalName);
        var tableHelper = new TableHelper('#tablaDatos', false,false,false);
    }
    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });
        this.modalForm.open();
    }


    private InitModalEdit(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));    

        //this.selectTipo= new SelectHelper("#Tipo","/Admin/TipoDoc/getJsonTipos","<Seleccione Tipo>", false,false,true);
        //this.selectTipo.initSelectAjax(); 
 
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });

        this.modalForm.initModal();
        this.modalForm.open();
 
    }


    private saveResult(data,status,xhr):void{
            if(data == 'true'){
                this.Events.bind("OnGetPartial",()=>{
                    var tableHelper = new TableHelper('#tablaDatos',true);
                });
                this.getPartial("#divTableDatos",this.urlGetTable,{});
                this.alert.toastOk();
                this.modalForm.close();
                return;
            } else {
                 this.alert.toastErrorData(data);
            }
    }
}