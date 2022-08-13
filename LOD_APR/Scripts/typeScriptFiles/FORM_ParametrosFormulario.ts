/// <reference path="CommonHelper.ts"/>

class FORM_ParametrosFormulario extends GenericController{
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
        this.modalName="#modalparametrosEvaluacion";
        this.formName="#formparametrosEvaluacion";
        this.modalForm = new ModalHelper(this.modalName);

        this.UpdateSorteable();

    }
    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
     
        $(document).on("click", "[type='checkbox']", function (e) {
            if (this.id == "TextoLargo_1" && this.checked) {
                $("#Obligatoria").attr("value", "True");
            } else if (this.id == "TextoLargo_1" && !this.checked) {
                $("#Obligatoria").attr("value", "False");
            }
        });

        this.modalForm.open();
    }

    private saveResult(data,status,xhr):void{
        var data1=data.split(";");
            if(data1[0] == 'true')
            {   
                this.UpdatePanelParams(data1[1]);
                return;
            }
            else {
                this.modalForm.close();
                 this.alert.toastErrorData(data);
            }
    }

    public UpdatePanelParams(_idPanel:number){
        var loadUrl = "/Admin/Formularios/ListParams";
        this.Events.bind("OnGetPartial",()=>{
            this.UpdateSorteable();
        });
        var id={id:_idPanel};
        this.getPartial("#Panel_" + _idPanel,loadUrl,id);      
        this.alert.toastOk();
        this.modalForm.close();
        return;
    }

    private UpdateSorteable(){
        $(".dragable-List").sortable({
            items:".list-group-item",
            stop: (event, ui)=> {
                var IdItem = ui.item[0].dataset.iditem;
                var indexReal = 0;
                console.log(ui);
                console.log("Cambio de Index UI: " + IdItem);
                $('.dragable-List').find('.list-group-item').each((index,item)=> {
                    console.log("Cambio de Index: " + index + ' ' + item.dataset.id);
                    var itemIdItem = item.dataset.iditem;
                    var itemId = item.dataset.id;
                    if(itemIdItem==IdItem){
                        indexReal++;
                        this.UpdateParamIndex(itemId,index);
                        $("#ParamIndic_" + itemId).text(indexReal + ")");
                    }
                });
            }
        });
    }
    
    private UpdateParamIndex(_Id:String,_Index:number){
        this.Events.bind("OnPostCustomData",(_Indice)=>{ });
        this.postCustomData({id:_Id, index:_Index},"/Admin/FormPreguntas/UpdateIndex");
    }

}