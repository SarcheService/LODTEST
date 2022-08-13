class LOD_CierreLibro extends GenericController{
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
        this.modalName="#modalCierre";
        this.formName="#formTipo";
        //this.urlGetTable="/GLOD/ClassOnee/getTable";   
        this.modalForm = new ModalHelper(this.modalName);
        var tableHelper = new TableHelper('#tablaDatos', true);
        var tableHelper = new TableHelper('#tablaDatos2', true);
    }

    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        this.modalForm.open();
    }

    private InitModalTipo(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
    

        this.selectTipo= new SelectHelper("#Tipo","/Admin/TipoDoc/getJsonTipos","< Seleccione Tipo >", false,false,true);
        this.selectTipo.initSelectAjax(); 
 
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



    private saveResultCierre(data,status,xhr):void{
        var data1 = data.split(";");
            if(data1[0]=="true")
            {
                this.getPartial("#divInfoLibros","/GLOD/LibroObras/getLibro",{id:data1[1]});
                this.alert.toastOk();
                this.modalForm.close();
                //$(".ladda-button").ladda('stop');
                return;
            }else if(data1[0] == "trueInicio"){
                this.getPartial("#getDetailsLibroActivo", "/GLOD/LibroObras/Details", { id: data1[1] });
                this.alert.toastOk();
                this.modalForm.close();
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