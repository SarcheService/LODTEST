/// <reference path="CommonHelper.ts"/>

class FORM_InformesIncidentes extends GenericController{
    //ELEMENTOS DE INTERFAZ DE USUARIO
    private selectEmail: SelectHelper;
    private modalCanvas:string;
    private formName: string;
    private modalName:string;
    private urlGetTable:string;
    private modalForm:ModalHelper
    public readonly urlGetTree:string;
    private btnSubmit:any;
    constructor(){
        super();
        this.alert = new AlertHelper()
        this.modalCanvas="#modalCanvas";
        this.modalName="#modalFormReporte";
        this.formName="#formFormEval";
        this.urlGetTable = "/GLOD/FormInformes/GetTableIncidentes";
        var tableHelper = new TableHelper('#tablaReportes', true);
    }

    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm = new ModalHelper(this.modalName);
        this.modalForm.initModal();
        this.modalForm.open();
    }
    private OnBeginActivate():void{
        this.btnSubmit = $('#BtnSubmitActivate').ladda();
        this.btnSubmit.ladda('start');
    }

    private saveResult(data,status,xhr):void{
        var result = data.split(';');
            if(result[0] == 'true')
            {   
                window.location.href = "/GLOD/FormInformes/View/" + result[1];
                this.alert.toastOk();
                this.modalForm.close();
                return;
            } 
            else {
                 this.alert.toastErrorData(result[1]);
            }
    }
    
}