/// <reference path="CommonHelper.ts"/>

class FORM_ItemsFormulario extends GenericController{
    //ELEMENTOS DE INTERFAZ DE USUARIO
    private modalCanvas:string;
    private formName: string;
    private modalName:string;
    private modalForm:ModalHelper
    public readonly urlGetTree:string;

    constructor(){
        super();
        this.alert = new AlertHelper()
        this.modalCanvas="#modalCanvas";
        this.modalName="#modalItemsFormEval";
        this.formName="#formItemsFormEval";
        this.modalForm = new ModalHelper(this.modalName);

        $("#DivDetails").sortable({
            items: ".dragable-panel",
            stop: (event, ui)=> {
                $('.dragable-panel').each((index, element)=> {
                  this.UpdateItemIndex(element.dataset.id,index,element.dataset.idform);
                });
            }
        });

        $('.panel-body').on('shown.bs.collapse', (item)=> {
            $("#Icon_" + item.target.id).removeClass("fa-chevron-down").addClass("fa-chevron-up");
         });
         
         $('.panel-body').on('hidden.bs.collapse',(item)=> {
            $("#Icon_" + item.target.id).removeClass("fa-chevron-up").addClass("fa-chevron-down");
         });

    }
    
    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();

        $(document).on("click", "[type='checkbox']", function (e) {
            if (this.id == "Activo_1" && this.checked) {
                $("#Activo").attr("value", "True");
            } else if (this.id == "Activo_1" && !this.checked) {
                $("#Activo").attr("value", "False");
            }
        });

        this.modalForm.open();
    }
    private saveResult(data,status,xhr):void{
        var data1=data.split(";");
            if(data1[0] == 'true')
            {   
                var loadUrl = "/Admin/Formularios/Details";
                this.Events.bind("OnGetPartial",()=>{ });
                var id={id:data1[1]};
                this.getPartial("#DivDetails",loadUrl,id);      
                this.alert.toastOk();
                this.modalForm.close();
                return;
            }
            else {
                 this.modalForm.close();
                 this.alert.toastErrorData(data);
            }
    }
    private UpdateItemIndex(_Id:number,_Index:number, _IdForm:number){
        this.Events.bind("OnPostCustomData",(_Indice)=>{});
        this.postCustomData({id:_Id, index:_Index},"/Admin/FormItems/UpdateIndex");
    }

}