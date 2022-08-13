/// <reference path="CommonHelper.ts"/>

class MAE_sucursal extends GenericController{
    //ELEMENTOS DE INTERFAZ DE USUARIO
    private modalCanvas:string;
    private formName: string;
    private modalName:string;
   
    private urlGetTable:string;
    private modalForm:ModalHelper;
    
    private selectFaena: GenericSelectFaena;

    constructor(){
        super();
        this.alert = new AlertHelper()
        this.modalCanvas="#modalCanvas";
        this.modalName="#modalSucursal";
        this.formName="#formSucursal";
        this.urlGetTable="/Admin/Sucursal/getTable";
        this.modalForm = new ModalHelper(this.modalName);
       
        var tableHelper = new TableHelper('#tablaDatos', true);
        var tableHelper = new TableHelper('#tablaSucursal', false,false,false);
    }
    
    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        $(".panel-load").removeClass("sk-loading");
        $('#modalSucursal #Telefono').mask('+56 000000000', { placeholder: "+56 _________" });
        $(".select_IdCiudad").select2({
                allowClear: false,
                placeholder: 'Seleccione una ciudad',
                theme: "bootstrap"
            });
            $("#IdDireccion").select2({
                allowClear: false,
                placeholder: 'Seleccione una Dirección',
                theme: "bootstrap"
            });
           
        this.Events.bind("OnOpenModal",()=>{
        });
        
        this.modalForm.initModal();
        this.modalForm.open();
    }

   

    private saveResult(data,status,xhr):void{
        
            var datos = JSON.parse(data);
            if(!datos.error){
                this.getPartial("#divTableDatos",this.urlGetTable,{IdSujeto:datos.idSujeto});
                this.alert.toastOk();
                this.modalForm.close();
                return;
            } else {
                 this.alert.toastErrorData(data);
            }
    }
}