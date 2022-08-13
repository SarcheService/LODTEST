/// <reference path="Interfaces.ts"/>
//ARGUMENTOS DE RETORNO DE LOS FORMS AJAX
// OnBegin – xhr
// OnComplete – xhr, status
// OnSuccess – data, status, xhr
// OnFailure – xhr, status, error
class GenericController{
    public alert: AlertHelper;
    //EVENTOS
    public Events: EventsHelper;
    //OnGetPartial
    //OnGetAppendPartial
    //OnPostForm
    //OnPostFormData
    //OnPostCustomData

    constructor(){
        this.alert = new AlertHelper();
        this.Events = new EventsHelper();

        $(document).ajaxError(function (e, xhr) {
            var pathname = window.location.pathname;
            if (xhr.status == 403) {
                var response = $.parseJSON(xhr.responseText);
                console.log(response);
                window.location = response.LogOnUrl + '?ReturnUrl=' + pathname;
            }
            if (xhr.status == 401) {
                var response = $.parseJSON(xhr.responseText);
                console.log(response);
                window.location = response.LogOnUrl + '?ReturnUrl=' + pathname;
            }
        });
    }

    
   public ValidatePermisos(_permisos:string[],_buttons:string[],_idEmpresa)
   {
    this.postCustomData({permisos:_permisos,IdEmpresa:_idEmpresa},"/ASP/Home/ValidaPermisos_");
    this.Events.bind("OnPostCustomData", (response) => {
        for (let index = 0; index < response.length; index++) {
         $("#"+_buttons[response[index]]).remove();
        }
    });
   }
    public getAppendPartial(_modalCanvas:string,_url:string,_data:any):void{

        $.ajax({
            url: _url,
            type: 'GET',
            data: _data,
             beforeSend: ()=> {
                $(".panel-load").addClass("sk-loading");
            },
            success: (result)=> {
                $(".panel-load").removeClass("sk-loading");
                $(_modalCanvas).append(result);
                this.Events.call("OnGetAppendPartial");
            }
        });

    }
    
    public getSelectListItem(_select2:string,_url:string,_data:any):void{
        $.ajax({
            type: 'GET',
            url: _url,
            data:_data
        }).then(function (data) {
            $(_select2).empty().trigger("change");
            console.log(data)
            for (let index = 0; index < data.length; index++) {
                const element = data[index];
                var option = new Option(element.Text, element.Value, true, true);
                $(_select2).append(option).trigger('change');
            }
        });
    }

    public AddParamHref(_Objet:string,params:string)
    {
        if($(_Objet).length > 0)
        {
            var href = $(_Objet).attr("href");
            if(href.includes("?"))
                {
                    href+="&";
                }else
                {
                    href+="?";
                }
            $(_Objet).attr("href", href+params)
       }
    }

    public AddParamHrefArray(_Objets:string[],params:string)
    {
       for (let index = 0; index < _Objets.length; index++) {
            const _Objet = _Objets[index];
            if($(_Objet).length > 0)
            {
                var href = $(_Objet).attr("href");
                if(href.includes("?"))
                {
                    href+="&";
                }else
                {
                    href+="?";
                }
                $(_Objet).attr("href", href+params)
            }
        }
        
   
    }


    public getPartial(_panelName:string,_url:string,_data:any):void{

            $.ajax({
            url: _url,
            type: 'GET',
            data: _data,
            async: false,
            beforeSend:()=> {
                $(".panel-load").addClass("sk-loading");
            },
            success: (result)=> {
                $(_panelName).html(result);
                this.Events.call("OnGetPartial");
                $(".panel-load").removeClass("sk-loading");
            }
        });

    }

    public getPartialContentTypeJson(_panelName:string,_url:string,_data:any, FireEvent = true):void{

        $.ajax({
        url: _url,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(_data),
        async: false,
        beforeSend: ()=> {
            $(".panel-load").addClass("sk-loading");
        },
        success: (result)=> {
            $(".panel-load").removeClass("sk-loading");
            $(_panelName).html(result);
            if (FireEvent)
            {
                this.Events.call("OnGetPartial");                    
            }
        }
    });

}


    public getPartialCustomEvent(_panelName:string,_url:string,_data:any,_Event:string):void{

        $.ajax({
        url: _url,
        type: 'GET',
        data: _data,
        async: false,
        beforeSend: ()=> {
            $(".panel-load").addClass("sk-loading");
        },
        success: (result)=> {
            $(".panel-load").removeClass("sk-loading");
            $(_panelName).html(result);
            this.Events.call(_Event);
        }
    });

    public getPartialValidatePermisos(_panelName:string,_url:string,_data:any,_permisos:string[],_buttons:string[],_idEmpresa):void{
        $(_panelName).hide();
        $.ajax({
        url: _url,
        type: 'GET',
        data: _data,
        async: true,
        beforeSend:()=> {
            $(".panel-load").addClass("sk-loading");
        },
        success: (result)=> {
            $(_panelName).html(result);
            this.Events.bind("OnPostCustomData", (response) => {
                for (let index = 0; index < response.length; index++) {
                 $("#"+_buttons[response[index]]).remove();
                }
                $(_panelName).show();
               
            });
            this.postCustomData({permisos:_permisos,IdEmpresa:_idEmpresa},"/ASP/Home/ValidaPermisos_");
            this.Events.call("OnGetPartial");
            $(".panel-load").removeClass("sk-loading");
        }
    });
    }


    public ValidaPermisosBtns(_permisos:string[],_buttons:string[],_idEmpresa,_Load?:boolean)
    { 
            if(_Load || _Load== undefined){
                $(".panel-load").addClass("sk-loading");
            }
          
          
            this.Events.bind("OnPostCustomData", (response) => {
                for (let i = 0; i < response.length; i++) {
                 $("#"+_buttons[response[i]]).addClass("hide");
                }
                 
                for (let j = 0; j < _buttons.length; j++) {
                    if (!response.includes(j)) {
                        $("#"+_buttons[j]).removeClass("hide");
                    }
                }
            
                if(_Load || _Load== undefined){
                    $(".panel-load").removeClass("sk-loading");
                }
               
                this.Events.call("OnValidate");
            });





            this.postCustomDataAsync({permisos:_permisos,IdEmpresa:_idEmpresa},"/ASP/Home/ValidaPermisos_");
            
          
    }



    

    // JT 23/04/2018
    public getPartialAsync(_panelName:string,_url:string,_data:any,_Load?:boolean):void{

        $.ajax({
        url: _url,
        type: 'GET',
        data: _data,
        async: true,
        beforeSend: ()=> {
            if (_Load || _Load == undefined) {
                $(".panel-load").addClass("sk-loading");
            }
           
        },
        success: (result)=> {
          
            $(_panelName).html(result);
            if (_Load || _Load == undefined) {
                $(".panel-load").removeClass("sk-loading");
            }
            this.Events.call("OnGetPartial");
        }
    });

   }
   //FIN 
    public postForm(form:any,_url):void {

        var postData = form.serialize();
        $.post(_url, postData, (response)=> {
            this.Events.call("OnPostForm",response);
        });

    }
    public postFormData(data:FormData,_url):void {

        $.ajax({
                url: _url,
                type: 'POST',
                data: data,
                contentType: false,
                processData: false,
                success: (result)=> {
                    this.Events.call("OnPostFormData",result);
                }
            });

    }
    public postCustomData(data:any,_url):void {

        $.ajax({
                url: _url,
                type: 'POST',
                data: JSON.stringify(data),
                contentType: 'application/json',
                //contentType: false,
                //processData: false,
                success: (result)=> {
                    this.Events.call("OnPostCustomData",result);
                }
            });

    }

    public postCustomDataAsync(data:any,_url):void {

        $.ajax({
                url: _url,
                type: 'POST',
                data: JSON.stringify(data),
                contentType: 'application/json',
                async:true,
                //contentType: false,
                //processData: false,
                success: (result)=> {
                    this.Events.call("OnPostCustomData",result);
                }
            });

    }
    public postFileToDB(file:FormData,_url:string):void{

            $.ajax({
                url: _url,
                type: 'POST',
                data: file,
                contentType: false,
                processData: false,
                beforeSend: ()=> {
                    this.Events.call('OnIniSaveFile');
                },
                success: (result)=> {
                    this.Events.call('OnSuccessSaveFile',result);
                },complete:(result)=>{
                    this.Events.call('OnCompleteSaveFileaveFile',result);
                }
            });

    }
    public hideLink(name:string){
        $(name).addClass("hide");
        $(name).attr("href", "#");                    
    }
    public showLink(name:string){
        $(name).removeClass("hide");          
    }
    public disableLink(name:string){
        $(name).addClass("disabled");
        $(name).attr("href", "#");                 
    }
    public enableLink(name:string){
        $(name).removeClass("disabled");
    }
    public setLink(name:string,link:string){
        $(name).attr("href", link);
    }
    public updateTreeMenu(estado:string,clase:string)
    {
        $(`.${clase}`).each((i,obj)=>{
            var showON = $(obj).data("showon").split("-");
            var hide = Boolean($(obj).data("hide"));
            var id = $(obj).prop('id');
            var isOFF = true; 
            $(showON).each((index,item)=>{
                if(String(item)==estado){
                    isOFF = false;
                    if(hide){
                        this.showLink(`#${id}`);
                    }else{
                        this.enableLink(`#${id}`);
                    }
                }
            });
            if(isOFF){
                if(hide){
                    this.hideLink(`#${id}`);
                }else{
                    this.disableLink(`#${id}`);
                }
            }

        });

    }

}
class GenericSelectFaena extends GenericController{

    private searchTree: SearchTreeHelper;
    public nodeSearchID:number;
    public nodeSearchName:string;
    private urlGetTree:string;
    private urlGetModal:string;
    private readonly modalCanvas:string;
    private readonly modalName:string;
    private readonly treeName:string;
    private readonly InputName:string;
    private readonly LabelName:string;
    public modal:ModalHelper;

    constructor(_InputName:string,_LabelName:string){
        super();
        this.modalCanvas="#modalCanvas";
        this.modalName="#modalSelectTree";
        this.treeName="#treeSelectFaena";
        this.urlGetTree = "/Admin/Faenas/getTree";
        this.urlGetModal = "/Admin/Faenas/getSelFaena";
        this.InputName = _InputName;
        this.LabelName = _LabelName;
        this.modal = new ModalHelper(this.modalName);
        
    }
    public seleccionarFaena(_input:string,_label:string){
        this.getModal({}, {InputName: this.InputName, LabelName:this.LabelName});
    }
   
    public deleteFaena() {
        $(this.InputName).val(null);
        $(this.LabelName).val('-');
    }
    private initSearchTree(_params:any){
        
        this.searchTree = new SearchTreeHelper(this.treeName,"#btnExpSearch_Faena","#btnColSearch_Faena","#txtSearch_Faena");
        this.searchTree.Events.bind("OnSelectNode", (node)=>{
            
            var tipo = node.data.type;
            var parents = node.parents;
            var rootNodeId = node.data.db_id;
  
            if(tipo=="ua" || tipo=='ro') {
                $("#btnSelectFaena").prop("disabled", true);
                this.nodeSearchID=null;
                this.nodeSearchName = '-';
            }else{
                  $("#btnSelectFaena").prop("disabled", false);
                  this.nodeSearchID=node.data.db_id;
                  this.nodeSearchName = node.text;

                  for (var i = 0; i < parents.length; i++) {
                        if (parents[i] != "#") {
                            var n = $(this.treeName).jstree(true).get_node(parents[i]);
                            if (n["data"].type == "fa") {
                                this.nodeSearchName = n.text + "/" + node.text;
                                break;
                            };
                        }
                    }

            }

            $(this.InputName).val(this.nodeSearchID);
            $(this.LabelName).val(this.nodeSearchName);

        });
        this.searchTree.initTree();
        //{idModelo:this.nodeID}
        this.searchTree.updateTreeData(this.urlGetTree+"?IdEmpresa="+$("#IdEmpresa").val(),_params);
    };
     public getModal(_paramsTree:any,_paramsModal:any){

        this.modal.Events.bind("OnGetModal",()=>{
            this.modal.initModal();
            this.initSearchTree(_paramsTree);
            this.modal.open();
        });

        this.modal.getModal(this.modalCanvas,this.urlGetModal,_paramsModal);

    }


}
class GenericSelectJerarquia extends GenericController{

    private searchTree: SearchTreeHelper;
    public nodeSearchID:number;
    public nodeSearchName:string;
    private urlGetTree:string;
    private urlGetModal:string;
    private readonly modalCanvas:string;
    private readonly modalName:string;
    private readonly treeName:string;
    public modal:ModalHelper;

    constructor(){
        super();
        this.modalCanvas="#modalCanvas";
        this.modalName="#modalSelectTree";
        this.treeName="#treeSelectJerarquia";
        this.urlGetTree = "/PER/Jerarquia/getSelectTree";
        this.urlGetModal = "/PER/Jerarquia/getSelJerarquia";
        this.modal = new ModalHelper(this.modalName);
        //this.urlGetTree = "/PER/Faenas/getTree";
        //this.urlGetModal = "/PER/Faenas/getSelFaena";

    }

    private initSearchTree(_params:any){
        
        this.searchTree = new SearchTreeHelper(this.treeName,"#btnExpSearch_Jer","#btnColSearch_Jer","#txtSearch_Jer");
        this.searchTree.Events.bind("OnSelectNode", (node)=>{
            
            var tipo = node.data.type;
            var parents = node.parents;
            var rootNodeId = node.data.db_id;
  
            if(tipo=="ua" || tipo=='ro') {
                $("#btnSelectFaena").prop("disabled", true);
                this.nodeSearchID=null;
            }else{
                  $("#btnSelectFaena").prop("disabled", false);
                  this.nodeSearchID=node.data.db_id;
                  this.nodeSearchName = node.text;

                  for (var i = 0; i < parents.length; i++) {
                        if (parents[i] != "#") {
                            var n = $(this.treeName).jstree(true).get_node(parents[i]);
                            if (n["data"].type == "fa") {
                                this.nodeSearchName = n.text + "/" + node.text;
                                break;
                            };
                        }
                    }

                  
                 
            }

        });
        this.searchTree.initTree();
        //{idModelo:this.nodeID}
        this.searchTree.updateTreeData(this.urlGetTree+"?id="+$("#IdEmpresa").val(),_params);
    };
     public getModal(_paramsTree:any,_paramsModal:any){

        this.modal.Events.bind("OnGetModal",()=>{
            this.modal.initModal();
            this.initSearchTree(_paramsTree);
            this.modal.open();
        });

        this.modal.getModal(this.modalCanvas,this.urlGetModal,_paramsModal);

    }
}

class GenericSelectCarpetasDoc extends GenericController{

    private searchTree: SearchTreeHelper;
    public nodeSearchID:number;
    public nodeSearchName:string;
    private urlGetTree:string;
    private urlGetModal:string;
    private readonly modalCanvas:string;
    private readonly modalName:string;
    private readonly treeName:string;
    private readonly InputName:string;
    private readonly LabelName:string;
    private readonly TipoObject:string;
    public modal:ModalHelper;

    constructor(_InputName:string,_LabelName:string,_TipoObject){
        super();
        this.modalCanvas="#modalCanvas";
        this.modalName="#modalSelectTree";
        this.treeName="#treeSelectCarpetasDoc";
        this.urlGetTree = "/Admin/Paths/getTreeCarpetasDoc";
        this.urlGetModal = "/Admin/Paths/getSelCarpetasDoc";
        this.InputName = _InputName;
        this.LabelName = _LabelName;
        this.TipoObject=_TipoObject;//Para diferencia entre Bit y Lod
        this.modal = new ModalHelper(this.modalName);
        
    }
  
    public seleccionarCarpetasDoc(/*_input:string,_label:string*/){
        this.getModal({}, {InputName: this.InputName, LabelName:this.LabelName /*, TipoObject:this.TipoObject*/});
    }
   
    public deleteCarpetasDoc() {
        $(this.InputName).val(null);
        $(this.LabelName).val('-');
    }
 
    private initSearchTree(_params:any){
        
         this.searchTree = new SearchTreeHelper(this.treeName,"#btnExpSearch_CarpetasDoc","#btnConSearch_CarpetasDoc","#treeSelectCarpetasDoc");
        
         this.searchTree.Events.bind("OnSelectNode", (node)=>{
            
             var tipo = node.data.type;
             var parents = node.parents;
             var rootNodeId = node.data.db_id;
  
             if(tipo=='ro' || tipo=='ti') {
                 $("#btnSelectCarpetasDoc").prop("disabled", true);
                 this.nodeSearchID=null;
                 this.nodeSearchName = '-';
             }else{
                   $("#btnSelectCarpetasDoc").prop("disabled", false);
                   this.nodeSearchID=node.data.db_id;
                   this.nodeSearchName = node.text;

                   for (var i = 0; i < parents.length; i++) {
                         if (parents[i] != "#") {
                             var n = $(this.treeName).jstree(true).get_node(parents[i]);
                             if (n["data"].type == "fa") {
                                 this.nodeSearchName = n.text + "/" + node.text;
                                 break;
                            };
                         }
                     }

             }

             $(this.InputName).val(this.nodeSearchID);
             $(this.LabelName).val(this.nodeSearchName);

         });
        
        
         this.searchTree.initTree();
         //{idModelo:this.nodeID}
         this.searchTree.updateTreeData(this.urlGetTree,_params);
        console.log("entro");
    };
 
    public getModal(_paramsTree:any,_paramsModal:any){
        _paramsTree={TipoObject: this.TipoObject};
        this.modal.Events.bind("OnGetModal",()=>{
            this.modal.initModal();
            this.initSearchTree(_paramsTree);
            this.modal.open();
        });

        this.modal.getModal(this.modalCanvas,this.urlGetModal,_paramsModal);

    }


}