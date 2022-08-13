/// <reference path="CommonHelper.ts"/>

class GLOD_inicioRapido extends GenericController{
    //ELEMENTOS DE INTERFAZ DE USUARIO
    private modalCanvas:string;
    private formName: string;
    private modalName:string;
    private urlGetTable:string;
    private modalForm:ModalHelper
    public readonly urlGetTree:string;

    constructor(){
        super();
        this.alert = new AlertHelper();
       
        $('#ContratosDisponible').select2({
            allowClear: true,
            placeholder: '<Seleccione Contrato>',
            theme: "bootstrap"
        });
         
        $( "#ContratosDisponible").change(()=> { 
           
            this.Events.bind("OnGetPartial",()=>{
            });
            var estado = $("#ContratosDisponible").val();
            console.log(estado);
            if(estado !== "NULL" && estado !== null && estado !== ''){
                //this.getPartialAsync("#getLibros","/GLOD/Contratos/InicioRapidoBuscar", {IdContrato:$("#ContratosDisponible").val()});
                this.getPartialAsync("#getInfoAvance", "/GLOD/Contratos/GetInfoAvance", { IdContrato: $("#ContratosDisponible").val() });
                this.getPartialAsync("#getInfoDoc", "/GLOD/Contratos/GetInfoDoc", { IdContrato: $("#ContratosDisponible").val() });
                this.getPartialAsync("#getContrato", "/GLOD/Contratos/GetContrato", { IdContrato: $("#ContratosDisponible").val() });
                
                //this.getPartialAsync("#getRoles", "/GLOD/Contratos/GetRolesContrato", { IdContrato: $("#ContratosDisponible").val() });
                //this.getPartialAsync("#getInfoAnot", "/GLOD/Contratos/GetInfoAnot", { IdContrato: $("#ContratosDisponible").val() });
                //this.getPartialAsync("#getNotiAnot", "/GLOD/Contratos/GetNotiAnot", { IdContrato: $("#ContratosDisponible").val() });
            }
        });


        
    }

    private showSection(){
        document.getElementById('showSection').style.display = "none";
        document.getElementById('btnShow').style.display = "none";
    }
    
    private GetDetailsLibro(idLibro){
        console.log("Entre a la función");
        console.log(idLibro);
        document.getElementById('showSection').style.display = "block";        

        if(idLibro !== "NULL" && idLibro !== null && idLibro !== ''){
            console.log("Entre a la función");
            this.getPartial("#getDetailsLibroActivo", "/GLOD/LibroObras/Details", { id: idLibro, TipoVista: 2});
        }

        document.getElementById('btnShow').style.display = "block";
       
    }

   
     private Buscar():void{
       
        var buscar = $('#text_buscar').val();
        if (buscar.length > 0) {
            this.DesactiveTabs();
            this.Events.bind("OnGetPartial",()=>{
                this.ActiveTabs();
            });
            this.ReOrganizeTab();
            this.getPartialAsync("#TabContent","/ASP/LibroObras/LibroIndexBuscar", {buscar:$("#text_buscar").val(),IdEmpresa:$("#IdEmpresa").val(),TipoVista:$("#TipoVista").val()});
        }
        else
        {
            $('#text_buscar').focus();
        }
    }
  
    private Recargar():void { 
        this.DesactiveTabs();
        this.Events.bind("OnGetPartial",()=>{
            this.ActiveTabs();
        });

        this.ReOrganizeTab();
        this.getPartialAsync("#TabContent","/ASP/LibroObras/LibroIndexRecargar", {IdEmpresa:$("#IdEmpresa").val(),TipoVista:$("#TipoVista").val()});
    }
   
 
    private getLibBit(_id):void{
        this.DesactiveTabs();
        this.Events.bind("OnGetPartial",()=>{
            this.ActiveTabs();
        });
        this.ReOrganizeTab();
        $("#IdEmpresa").val(_id);
        this.getPartialAsync("#TabContent","/ASP/LibroObras/LibroIndexRecargar", {IdEmpresa:_id,TipoVista:$("#TipoVista").val()});
    }

    
    private ChangeTipoVista():void
    {
        this.DesactiveTabs();
        this.Events.bind("OnGetPartial",()=>{
            this.ActiveTabs();
        });
        this.ReOrganizeTab();
        var TipoVista;
        if($("#TipoVista").val()=="IndexUsr")
        {
            $("#Icon").addClass('fa-th-large');
            $("#Icon").removeClass('fa-align-justify');
            TipoVista="IndexUsrRows";
        }else{
            $("#Icon").removeClass('fa-th-large');
            $("#Icon").addClass('fa-align-justify');
            TipoVista="IndexUsr";
        }   
        $("#TipoVista").val(TipoVista);

        this.getPartialAsync("#TabContent","/ASP/LibroObras/LibroIndexRecargar", {IdEmpresa:$("#IdEmpresa").val(),TipoVista:$("#TipoVista").val()});
    }

    private ReOrganizeTab():void
    {
        $("#tab1").addClass("active");
        $("#tab2").removeClass("active");
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
