class MAE_CodSubComunicacion extends GenericController{
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
        this.urlGetTable="/Admin/CodSubCom/getTable";   
        this.modalForm = new ModalHelper(this.modalName);
        //var tableHelper = new TableHelper('#tablaDatos', true);
    }
    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });

        $("#IdTipo").select2({
            placeholder: 'Seleccione el Tipo de Documento',
            allowClear: true,
            theme: "bootstrap"
        });

        /*
        $("#IdTipoCom").select2({
            placeholder: 'Seleccione el Tipo de Comunicación',
            allowClear: true,
            theme: "bootstrap"
        }).on('change', (e)=> {
            let id:number = $("#IdTipoCom").val();
            this.GetTipoSub(id);
        });*/

        /*
        $("#IdTipoSub").select2({
            placeholder: 'Seleccione el Tipo de Comunicación',
            allowClear: true,
            theme: "bootstrap"
        }).on('change', (e)=> {
            let id:number = $("#IdTipoSub").val();
            this.GetTipoDoc(id);
        });*/


        $("#IdTipoCom").on('change', (e)=> {
            let id:number = $("#IdTipoCom").val();
            console.log("entró al change tipo com");
            this.GetTipoSub(id);
        });

        $("#IdTipoSub").on('change', (e)=> {
            let id:number = $("#IdTipoSub").val();
            console.log("entró al change tipo sub");
            this.GetTipoDoc(id);
        });

        

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

    private saveResult(data,status,xhr):void{
            if(data == 'true'){
                this.Events.bind("OnGetPartial",()=>{
                    //var tableHelper = new TableHelper('#tablaDatos',true);
                });
                this.getPartial("#divTableDatos",this.urlGetTable,{});
                this.alert.toastOk();
                this.modalForm.close();
                return;
            } else {
                 this.alert.toastErrorData(data);
            }
    }

    private GetTipoSub(IdCom:number):void{
        this.Events.bind("OnGetPartial",()=>{
            $(".panel-load").removeClass("sk-loading");  
            $("#IdTipoSub").select2({
                allowClear: false,
                placeholder: '<Seleccione un Subtipo>',
                theme: "bootstrap"
            });

        });
        $(".panel-load").addClass("sk-loading");
        this.getPartial("#DivSub","/Admin/CodSubCom/GetTipoSub",{"IdCom":IdCom});  
    }

    private GetTipoDoc(IdSub:number):void{
        this.Events.bind("OnGetPartial",()=>{
            $(".panel-load").removeClass("sk-loading");  
            $("#IdTipo").select2({
                allowClear: false,
                placeholder: '<Seleccione un tipo>',
                theme: "bootstrap"
            });

        });
        $(".panel-load").addClass("sk-loading");
        this.getPartial("#DivDoc","/Admin/CodSubCom/GetTipoDoc",{"IdSub":IdSub});  
    }
}