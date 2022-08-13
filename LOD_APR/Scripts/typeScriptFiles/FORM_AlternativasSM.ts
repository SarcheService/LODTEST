/// <reference path="CommonHelper.ts"/>

class FORM_AlternativasSM extends GenericController{
    //ELEMENTOS DE INTERFAZ DE USUARIO
    private modalCanvas:string;
    private formName: string;
    private modalName:string;
    private modalForm:ModalHelper

    constructor(){
        super();
        this.alert = new AlertHelper()
        this.modalCanvas="#modalCanvas";
        this.modalName="#modalAlternativa";
        this.formName="#formAlternativa";
        this.modalForm = new ModalHelper(this.modalName);
    }
    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        this.modalForm.open();
    }
    private saveResult(data,status,xhr):void{
        var data1=data.split(";");
        if(data1[0] == 'true')
         {   
            var params = new FORM_ParametrosFormulario();
            params.UpdatePanelParams(data1[1]);
            this.modalForm.close();
            return;
         }
        else {
             this.alert.toastErrorData(data);
         }
    }
}