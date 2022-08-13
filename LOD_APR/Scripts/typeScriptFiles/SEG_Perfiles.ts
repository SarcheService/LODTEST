/// <reference path="CommonHelper.ts"/>


class SEG_Perfiles extends GenericController {    
    private modalForm:ModalHelper
    private modalName:string;
    private formName: string;
    private modalCanvas:string;
    private urlGetTable:string;
    constructor(_Area:string,_Controller:string) {
        super();
        this.modalCanvas="#modalCanvas";
        this.modalName ="#modalDelete";
        this.formName="#formDatos";   
        this.urlGetTable="/"+_Area+"/"+_Controller+"/getPerfiles";
        
        var tablaDatos= new TableHelper('#tablaDatos', false);
        this.modalForm = new ModalHelper(this.modalName);
    

        $('.iCheck-helper').click(function(e) {
            if (this.previousSibling.value == 1) {
                    $('#TipoPerfil').val(1);
            }
            else {
                $('#TipoPerfil').val(2);
            }
        });
    
    
    
    }

    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));       
        this.modalForm.initModal();
        this.modalForm.open();
    }


    private saveResult(data,status,xhr):void{
        if(data == 'true'){           
            this.alert.toastOk();
            this.modalForm.close();
            
            
            this.getTable();
            return;
        } else {
             this.alert.toastErrorData(data);
        }
    }

    
    private GetPerfiles(_id:number)
    { 
        this.setLink("#BtnCreate","/Admin/Perfil/Create?id="+_id);
        $("#IdEmpresa").val(_id);
        this.getPartial("#divTableDatos",this.urlGetTable,{id:_id});
        var tablaDatos= new TableHelper('#tablaDatos', false);
    }




    private getTable(){
        
        this.getPartial("#divTableDatos",this.urlGetTable,{id:$("#IdEmpresa").val()});
        var tablaDatos= new TableHelper('#tablaDatos', false);
    }

}


class OpcionesPerfiles extends GenericController{
    private urlGetTable:string;
    private urlGetTableTipo:string;
    private btnSubmit:any;
    private Estandar:boolean;
    
    constructor(_idPerfil:string,_Area:string,_Controller:string){
        super();
        this.urlGetTable="/"+_Area+"/"+_Controller+"/getOpciones";
        this.urlGetTableTipo ="/"+_Area+"/"+_Controller+"/getTipo";
        this.btnSubmit = $('#btnSubmit').ladda();
        

        $("#IdSistema").select2({            
            allowClear: false,
            theme: "bootstrap"
        }).on('change', (e) => {

            
            let _IdSistema:number = $("#IdSistema").val();
            let _idPerfil:number = $("#IdPerfil").val();
            if(String(_IdSistema) != ""){
                this.urlGetTable="/"+_Controller+"/getOpciones";
                // console.log(this.urlGetTable);
                
                this.getOptionChanage(_idPerfil,_IdSistema); 
           }
        });  
        let _IdSistema:number = $("#IdSistema").val();


        
        this.getOpciones(_idPerfil,_Area,_Controller);
        
    }

    
    private getOpciones(_idPerfil:string,_Area:string,_Controller:string):void{
   
        this.Events.bind("OnGetPartial",()=>{
            //JS PARA LA ACTUALIZACIÓN DINAMICA DE LOS PERMISOS
            $('#tabPermisos').on('change', 'input[type=checkbox]', (e)=> {
                // console.log(e.currentTarget.dataset.perfil);
                var permiso = $("#" + e.currentTarget.id).data("opcion");
                var perfil = $("#" + e.currentTarget.id).data("perfil");
                var isChecked = e.currentTarget.checked;
               
                // console.log(permiso,perfil,isChecked);
                this.savePermiso(perfil, permiso, isChecked,_Area,_Controller);              
            });      
        });
        this.getPartial("#tabPermisos",this.urlGetTable,{idPerfil:_idPerfil});
    }

    private getTipo(_idPerfil:string,_Area:string,_Controller:string):void{
   
        this.Events.bind("OnGetPartial",()=>{
            //JS PARA LA ACTUALIZACIÓN DINAMICA DE LOS PERMISOS
            $('#tabTipo').on('change', 'input[type=checkbox]', (e)=> {
                // console.log(e.currentTarget.dataset.perfil);
                var tipo = $("#" + e.currentTarget.id).data("tipo");
                var perfil = $("#" + e.currentTarget.id).data("perfil");
                var isChecked = e.currentTarget.checked;
               
                // console.log(permiso,perfil,isChecked);
                this.savePermisoTipo(perfil, tipo, isChecked,_Area,_Controller);
            });            
        });       
        this.getPartial("#tabTipo",this.urlGetTableTipo,{idPerfil:_idPerfil});
    }
    
    private getPerfilEmpresa(id:number)
    {
        
        console.log("id");
        if(this.Estandar){//Si el perfil cambio de estandar a normal

            this.Estandar=false;
            this.getSelectListItem("#IdSistema","/Admin/Perfil/GetOptionsSelect",{IdPerfil: $("#IdPerfil").val(),IdEmpresa:id});
            this.Events.bind("OnGetLatestSelect",()=>{
            });
        }
        if(id ==-1){//SI EL PERFIL CAMBIO DE NORMAL A ESTANDAR
            this.Estandar =true;
            this.getSelectListItem("#IdSistema","/Admin/Perfil/GetOptionsSelect",{IdPerfil: $("#IdPerfil").val(),IdEmpresa:id});
            this.Events.bind("OnGetLatestSelect",()=>{
            });
        }
      $("#IdEmpresa").val(id);
      var idSis =$("#IdSistema").val();
      $("#IdSistema").val(idSis).trigger('change')
      // this.getSelectListItem("#TipoPerfil","/Perfil/getSelectTipoPerfil", {idPerfil:$("#IdPerfil").val()});
    }

    private savePermiso(idPerfil,idPerm,checked,_Area,_Controller):void{
        this.Events.bind("OnPostCustomData",(data)=>{
            if(data == 'true'){
                this.alert.toastOk();          
            }else{
                 this.alert.toastErrorData(data);
            }
        });
        this.postCustomData({ idPerfil: idPerfil, idPermiso: idPerm, isChecked: checked, IdEmpresa:$("#IdEmpresa").val() },'/'+_Area+'/'+_Controller+'/savePermiso/');
    }

    private savePermisoTipo(idPerfil,Tipo,checked,_Area,_Controller):void{
        this.Events.bind("OnPostCustomData",(data)=>{
            if(data == 'true'){
                this.alert.toastOk();          
            }else{
                 this.alert.toastErrorData(data);
            }
        });
        this.postCustomData({ idPerfil: idPerfil, tipo: Tipo, isChecked: checked},'/'+_Area+'/'+_Controller+'/savePermisoTipo/');
    }

    public getOptionChanage(_IdPerfil,_IdSistema):void{
    
         this.DesactiveTabs();
        this.Events.bind("OnGetPartial",()=>{
            
            this.ActiveTabs();
            
        });
        this.getPartial("#tabPermisos",this.urlGetTable,{IdSistema:_IdSistema,idPerfil:_IdPerfil,IdEmpresa:$("#IdEmpresa").val()});

    } 



    private submit():void{
        this.btnSubmit.ladda('start');
    }

    private saveResult(data,status,xhr):void{
        if(data == 'true'){           
            this.alert.toastOk();
            this.btnSubmit.ladda('stop');         
            return;
        } else {
             this.alert.toastErrorData(data);
        }
    }



    private DesactiveTabs()
    {
        $(".LiTab").css("pointer-events","none");
        $("a.Tab-Admin").css("cursor","not-allowed");
    }

    private ActiveTabs()
    {
        $(".LiTab").css("pointer-events","all");
        $("a.Tab-Admin").css("cursor","default");
    }

}