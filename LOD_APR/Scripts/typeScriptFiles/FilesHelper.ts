/// <reference path="CommonHelper.ts"/>
//sumary
//Versión 1.0 para Galena Suite experta by MDTech Ltda.
//Fecha 17-04-2017
class MAE_archivos extends GenericController{
    //ELEMENTOS DE INTERFAZ DE USUARIO
    private readonly modalFormName:string;
    private readonly filesFormName:string;
    private readonly inputFileName:string;
    public readonly  modalCanvas:string;
    private readonly tableName:string;
    private readonly tableDivName:string; 
    private readonly urlAddModel:string; 
    private formValidator:any;
    private modalFormAdd:ModalHelper;
    //VARIABLES DE DATOS PARA USO DE LA CLASE
    private listFiles:FormData[];
    private ControllerName:string;
    private MaxFileSize:number;
    private IdDoc:number;
    private PrimaryKey:number;
    private IdPath:number;
    private tipoEp:number;
    //Para anotaciones 11-04-2018
    private selectTipoDoc: SelectHelper;

    private btnSubmit:any;
    private btnSubmit2:any;
   


    constructor(_ControllerName:string,_MaxFileSize:number){
        super();
        this.modalCanvas="#modalCanvas";
        this.modalFormName="#modalFormDocs";
        this.filesFormName = "#formDocs";
        this.inputFileName="#fileName";
        this.tableDivName = "#lstDocs_" + _ControllerName;
        this.tableName = "#tablaDocs_" + _ControllerName;
        this.urlAddModel = "/Admin/Documentos/AddFile";
        this.ControllerName = _ControllerName;
        this.MaxFileSize = _MaxFileSize;
        this.listFiles= [];
        this.selectTipoDoc= new SelectHelper("#IdTipoDocumento","/ASP/TipoDocumento/getTipoDocJson/","< Seleccione tipo >", true,false,true);
     
    }

    private getListDocs(_id,_path,_tipoEp = null){
        this.PrimaryKey = _id;
        this.IdPath = _path;
        this.tipoEp = _tipoEp;

        
        this.Events.bind("OnGetPartial", () => {
            $(".panel-load").removeClass("sk-loading");
            this.DOMcheckboxes();
        });
        $(".panel-load").addClass("sk-loading");

        if( this.tipoEp == null)
        {
            super.getPartial(this.tableDivName, '/Admin/Documentos/getFiles_'+this.ControllerName, {id:_id,path:_path});
        }
        else
        {
            super.getPartial(this.tableDivName, '/Admin/Documentos/getFiles_'+this.ControllerName, {id:_id,tipoEp:this.tipoEp});
        }
    }

    public addFile(_id,_path):void {
        this.PrimaryKey = _id;
        this.IdPath = _path;
        this.modalFormAdd = new ModalHelper(this.modalFormName);
        this.modalFormAdd.initModal();
        this.modalFormAdd.open();
        this.btnSubmit = $('#btnSubmit').ladda();
        this.initForm();
    }

      //Para Otros
      public addFileSinTipo(_id):void {
        this.PrimaryKey = _id;
        
       //this.IdPath = _path;
        //Agregado Jt para diferenciar entre bit y lod 
       this.modalFormAdd = new ModalHelper(this.modalFormName);
       this.modalFormAdd.initModal();
       this.modalFormAdd.open();
       this.btnSubmit = $('#btnSubmit').ladda();
       this.initForm();
   }

   public addFileDocEP(_idCaratula,_idTipoEP):void {
    this.PrimaryKey = _idCaratula;    
    this.tipoEp = _idTipoEP;    
    this.modalFormAdd = new ModalHelper(this.modalFormName);
    this.modalFormAdd.initModal();
    this.modalFormAdd.open();
    this.btnSubmit2 = $('#btnSubmitDelete').ladda();
    this.btnSubmit = $('#btnSubmit').ladda();
    this.initForm();
    }

    //Para Anotaciones
    public addFileAnot(_id,_tipo):void {
        this.PrimaryKey = _id;
        
        //this.IdPath = _path;
      
        //Agregado Jt para diferenciar entre bit y lod 
        this.modalFormAdd = new ModalHelper(this.modalFormName);
       
        if (_tipo=="Lod") {
            files_anotacionLod.selectTipoDoc.initSelect();
            files_anotacionLod.selectTipoDoc.clearSelection();
        }else if(_tipo="Bit")
        {
            files_anotacionBit.selectTipoDoc.initSelect();
            files_anotacionBit.selectTipoDoc.clearSelection();
        }  
       
        this.modalFormAdd.initModal();
        this.modalFormAdd.open();
        this.btnSubmit = $('#btnSubmit').ladda();
        this.initForm();
    }

    //SE UTILIZA PARA EL FORM DE ELIMINACIÓN Y EDICIÓN DE ARCHIVOS
    public InitForm(_id,_path):void {
        this.PrimaryKey = _id;
        this.IdPath = _path;
        this.modalFormAdd = new ModalHelper(this.modalFormName);
        $.validator.unobtrusive.parse($(this.filesFormName));
        this.modalFormAdd.initModal();
        this.modalFormAdd.open();
        $(".panel-load").removeClass("sk-loading");
    }

    private initForm():void{

        $(this.filesFormName).ajaxForm({
            beforeSend: ()=>{
                this.onBegin();
            },
            uploadProgress: (event,position,total, percentComplete)=>{
                $("#progressBar").css("width", percentComplete + "%");
                $("#progressBar").text(percentComplete + "%")
            },
            success: (data,status,xhr)=>{
                this.saveResult(data,status,xhr);
            },            
            complete: ()=>{
                $(".panel-load").removeClass("sk-loading");
                var btns = $(".ladda-button").ladda();
                btns.ladda('stop');
                $("#progressBar").css("width", "0%");
                $("#progressBar").text("0%")
            }  
        });
    }

    private saveResult(data,status,xhr):void{
        if(data == 'true')
        {
            this.getListDocs(this.PrimaryKey,this.IdPath, this.tipoEp);
            this.alert.toastOk();
            this.modalFormAdd.close();
            return;
        } 
        else 
        {
            this.alert.toastErrorData(data);
        }
    }

    private onBegin():void{
        this.btnSubmit.ladda('start');
        $('.panel-load').addClass('sk-loading');
    }
    public descargarZip() {
        var lstIds = this.selectedDocs();
        if (lstIds == null || lstIds.length == 0) {
            this.alert.toastWarningData("Seleccione almenos 1 documento");
            return;
        }

        this.Events.bind("OnPostCustomData", (result) => {
            var resultados = result.split(";");
            if (resultados[0] == "true") {
                window.location.href = resultados[1];
                this.alert.toastOkData("Descarga realizada correctamente!");
            } else {
                this.alert.toastErrorData(resultados[1]);
            }
        });
        super.postCustomData({ lstDocs: lstIds }, '/Admin/Documentos/zipDocs_' + this.ControllerName)
    }
     public selectedDocs(): any {
        var docs = [];
        $(this.tableName + ' input[type=checkbox]').each((index, item) => {

            var elementoCheck = item;
            if ($(elementoCheck).prop("checked") == true && $(elementoCheck).prop("id") != "chkTodos") {
                docs.push($(elementoCheck).val());
            }

        });
        return docs;
    }

    private selectCheckBoxes(isChecked: boolean) {
        $(this.tableName + ' input[type=checkbox]').each((index, item) => {
            var elementoCheck = item;
            if ($(elementoCheck).prop("disabled") == false) {
                if (isChecked) {
                    $(elementoCheck).prop("checked", true);
                } else {
                    $(elementoCheck).prop("checked", false);
                }
            }
        });
    };

    public DOMcheckboxes() {
        $(this.tableName).on('change', 'input[type=checkbox]', (e) => {
            var $element = e.target;
            if ($element.id == 'chkTodos') {
                this.selectCheckBoxes($($element).prop("checked"));
            }
        });
    }

    //Agregado Jt 17-04-2018 
    //Tipo es para poder usarla en anotaciones BIT y LOD
    public descargarZipAnot(IdAnot) {
    
        this.Events.bind("OnPostCustomData", (result) => {
            var resultados = result.split(";");
            if (resultados[0] == "true") {
                window.location.href = resultados[1];
                this.alert.toastOkData("Descarga realizada correctamente!");
            } else {
                this.alert.toastErrorData(resultados[1]);
            }
        });
        
     
        super.postCustomData({ IdAnot: IdAnot }, '/Admin/Documentos/zipDocs_' + this.ControllerName)
       

        
    }

}