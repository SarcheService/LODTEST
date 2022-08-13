class MAE_ClassDoc extends GenericController{
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
        this.urlGetTable="/Admin/ClassDoc/getTable";   
        this.modalForm = new ModalHelper(this.modalName);
        //var tableHelper = new TableHelper('#tablaDatos', true);
    }
    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();

        $("#IdClassTwo").select2({
            placeholder: 'Seleccione el tipo de Subclasificación',
            allowClear: true,
            theme: "bootstrap"
        });

        $("#IdTipo").select2({
            placeholder: 'Seleccione el tipo de documento',
            allowClear: true,
            theme: "bootstrap"
        });

        $("#IdTipoSub").select2({
            placeholder: 'Seleccione el tipo de Subtipo de Comunicación',
            allowClear: true,
            theme: "bootstrap"
        });

         
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });
        this.modalForm.open();
    }

    private InitModalTipo(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
    

        //this.selectTipo= new SelectHelper("#Tipo","/Admin/TipoDoc/getJsonTipos","< Seleccione Tipo >", false,false,true);
        //this.selectTipo.initSelectAjax(); 
 
        $("#IdClassTwo").select2({
            placeholder: 'Seleccione el tipo de Subclasificación',
            allowClear: true,
            theme: "bootstrap"
        });

        $("#IdTipo").select2({
            placeholder: 'Seleccione el tipo de documento',
            allowClear: true,
            theme: "bootstrap"
        });

        $("#IdTipoSub").select2({
            placeholder: 'Seleccione el tipo de Subtipo de Comunicación',
            allowClear: true,
            theme: "bootstrap"
        });



        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });

        this.modalForm.initModal();
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