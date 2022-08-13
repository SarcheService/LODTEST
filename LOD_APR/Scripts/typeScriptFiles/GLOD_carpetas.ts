/// <reference path="CommonHelper.ts"/>

class GLOD_carpetas extends GenericController{
    //ELEMENTOS DE INTERFAZ DE USUARIO
    private modalCanvas:string;
    private formName: string;
    private modalName:string;
    private urlGetTable:string;
    private modalForm:ModalHelper
    public readonly urlGetTree:string;

    constructor(){
        super();
        this.alert = new AlertHelper()
        this.modalCanvas="#modalCanvas";
        this.modalName="#modalCarpetas";
        this.formName="#formCarpetas";
        this.urlGetTree = "/GLOD/Home/getTree";
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
                glod_administracion.treeView.Events.bind("OnGetTreeData",()=>{
                    setTimeout(() => {$('#treeView').jstree(true).select_node("f_"+data1[1]);},1000);//EL mismo nodo  
                 });
                 glod_administracion.treeView.updateTreeData(this.urlGetTree);
               
                this.alert.toastOk();
                this.modalForm.close();
                return;
            }else if(data1[0] == 'delete'){
                glod_administracion.treeView.Events.bind("OnGetTreeData",()=>{
                    setTimeout(() => {$('#treeView').jstree(true).select_node(data1[1]);},1000);//Se recibe nodo Padre  
                 });
                 glod_administracion.treeView.updateTreeData(this.urlGetTree);
                this.alert.toastOk();
                this.modalForm.close();
                return;
            } 
            else {
                 this.alert.toastErrorData(data);
            }
    }
}