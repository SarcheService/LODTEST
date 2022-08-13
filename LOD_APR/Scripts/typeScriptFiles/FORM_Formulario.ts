/// <reference path="CommonHelper.ts"/>

class FORM_Formulario extends GenericController{
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
        this.modalName="#modalFormEval";
        this.formName="#formFormEval";
        this.urlGetTable = "/Admin/Formularios/GetTable";
        this.modalForm = new ModalHelper(this.modalName);
        var tableHelper = new TableHelper('#tablaFormularios', true);
    }

    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        this.modalForm.open();
    }
    private OnBeginActivate():void{
        this.btnSubmit = $('#BtnSubmitActivate').ladda();
        this.btnSubmit.ladda('start');
    }

    private saveResult(data,status,xhr):void{
            if(data == 'true')
            {   
                this.Events.bind("OnGetPartial",()=>{
                    var tableHelper = new TableHelper('#tablaFormularios',true);
                });
                this.getPartial("#DivTablaDatos",this.urlGetTable,{});
                this.alert.toastOk();
                this.modalForm.close();
                return;
            } 
            else {
                 this.alert.toastErrorData(data);
            }
    }

    private saveResultEmbebido(data,status,xhr):void{
        if(data == 'true')
        {   
            this.Events.bind("OnGetPartial",()=>{
                var tableHelper = new TableHelper('#tablaFormularios',true);
            });
            this.getPartial("#DivTablaDatos", "/Admin/FormsEmbebidos/GetTable",{});
            this.alert.toastOk();
            this.modalForm.close();
            return;
        } 
        else {
             this.alert.toastErrorData(data);
        }
}

    private SaveResultActivate(data,status,xhr):void{
        var btns = $(".ladda-button").ladda();
        btns.ladda('stop');
        var data1=data.split(";");
            if(data1[0] == 'true')
            {   
                window.location.href = "/Admin/Formularios/Edit/" + data1[1];
                return;
            }
            else {
                 this.alert.toastErrorData(data);
            }
    }
    
}