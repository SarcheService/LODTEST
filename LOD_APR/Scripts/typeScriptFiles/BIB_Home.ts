class BIB_Home extends GenericController{
    //ELEMENTOS DE INTERFAZ DE USUARIO
    private modalCanvas:string;
    private formName: string;
    private modalName:string;
    private urlGetTable:string;
    private modalForm:ModalHelper
    private btnSubmit:any;
    constructor(){
        super();
        this.alert = new AlertHelper();
        this.modalCanvas="#modalCanvas";
        this.modalName ="#modalPassword";
        this.formName="#formPassword";
        this.urlGetTable="/GLOD/Biblioteca//getTableUsers";
        this.modalForm = new ModalHelper(this.modalName);
        var tableHelper = new TableHelper('#tableDatos', false,true,true);


        $("input[name=flexRadioDefault]").click(()=> {    
          
            console.log("#Formulario",$("#DocAdmin",$("#DocTecnico",$("#Otros");
            if($("#Formulario").is(':checked'))
            {
                $("#Formulario").val(true);
                $("#DocAdmin").val(false);
                $("#DocTecnico").val(false);
                $("#Otros").val(false);
            }
           if($("#DocAdmin").is(':checked'))
            {
                $("#Formulario").val(false);
                $("#DocAdmin").val(true);
                $("#DocTecnico").val(false);
                $("#Otros").val(false);
            } 
            if($("#DocTecnico").is(':checked'))
            {
                $("#Formulario").val(false);
                $("#DocAdmin").val(false);
                $("#DocTecnico").val(true);
                $("#Otros").val(false);

            }if($("#Otros").is(':checked'))
            {
                $("#Formulario").val(false);
                $("#DocAdmin").val(false);
                $("#DocTecnico").val(false);
                $("#Otros").val(true);
            }

       });



        $("#IdDireccion").select2({
            placeholder: {
                id: '0', // the value of the option
                text: 'Seleccione una Dirección'
              },
                allowClear: true,
                theme: "bootstrap"
        }).val('0').trigger('change');
       
        var $option = $("<option selected></option>").val('0').text("Seleccione una Dirección");
        $("#IdDireccion").append($option).trigger('change');
        $("#IdDireccion").val('0');


        $("#IdFiscalizador").select2({
            placeholder: {
                id: '0', // the value of the option
                text: 'Seleccione un Fiscalizador'
              },
                allowClear: true,
                theme: "bootstrap"
        }).val('').trigger('change');

        var $option = $("<option selected></option>").val('0').text("Seleccione un Fiscalizador");
        $("#IdFiscalizador").append($option).trigger('change');
        $("#IdFiscalizador").val('0');

        $("#IdSujEcon").select2({
            placeholder: {
                id: '0', // the value of the option
                text: 'Seleccione un Contratista'
              },
                allowClear: true,
                theme: "bootstrap"
        }).val('').trigger('change');

        var $option = $("<option selected></option>").val('0').text("Seleccione un Contratista");
        $("#IdSujEcon").append($option).trigger('change');
        $("#IdSujEcon").val(0);

        $("#IdContrato").select2({
            placeholder: {
                id: '0', // the value of the option
                text: 'Seleccione un Contrato'
              },
                allowClear: true,
                theme: "bootstrap"
        }).val('').trigger('change');

        var $option = $("<option selected></option>").val('0').text("Seleccione un Contrato");
        $("#IdContrato").append($option).trigger('change');
        $("#IdContrato").val(0);

        $("#IdTipoLibro").select2({
            placeholder: {
                id: '0', // the value of the option
                text: 'Seleccione un Tipo Libro'
              },
                allowClear: true,
                theme: "bootstrap"
        }).val('').trigger('change');
        
        var $option = $("<option selected></option>").val('0').text("Seleccione un Tipo Libro");
        $("#IdTipoLibro").append($option).trigger('change');
        $("#IdTipoLibro").val(0);

        /*
        $("#IdLibroObra").select2({
            placeholder: 'Seleccione un Libro',
                allowClear: true,
                theme: "bootstrap"
        });*/

        $("#IdTipoComunicacion").select2({
            placeholder: {
                id: '0', // the value of the option
                text: 'Seleccione un Tipo de Comunicación'
              },
                allowClear: true,
                theme: "bootstrap"
        }).val('').trigger('change');

        var $option = $("<option selected></option>").val('0').text("Seleccione un Tipo de Comunicación");
        $("#IdTipoComunicacion").append($option).trigger('change');
        $("#IdTipoComunicacion").val(0);


        $("#IdSubtipoComunicacion").select2({
            placeholder: {
                id: '0', // the value of the option
                text: 'Seleccione un Tipo de Subtipo de Comunicación'
              },
                allowClear: true,
                theme: "bootstrap"
        }).val('').trigger('change');

        var $option = $("<option selected></option>").val('0').text("Seleccione un Subtipo de comunicación");
        $("#IdSubtipoComunicacion").append($option).trigger('change');
        $("#IdSubtipoComunicacion").val(0);

        $("#IdTipoDoc").select2({
            placeholder: {
                id: '0', // the value of the option
                text: 'Seleccione un Tipo de Documento'
              },
                allowClear: true,
                theme: "bootstrap",
        }).val('').trigger('change');
      
        var $option = $("<option selected></option>").val('0').text("Seleccione un Tipo de Documento");
        $("#IdTipoDoc").append($option).trigger('change');
        $("#IdTipoDoc").val(0);

        $("#IdDireccion").on('change', (e)=> {
            let IdSujEcon:number = $("#IdSujEcon").val();
            let idDireccion:number = $("#IdDireccion").val();
            let idFiscalizador:number = $("#IdFiscalizador").val();
            if(idFiscalizador == null){
                idFiscalizador = 0;
            }

            this.GetSujetos(idDireccion);
            this.GetFiscalizador(idDireccion);
            this.GetContratos(IdSujEcon,idFiscalizador,idDireccion)
        });

        $("#IdSujEcon").on('change', (e)=> {
            let IdSujEcon:number = $("#IdSujEcon").val();
            let idDireccion:number = $("#IdDireccion").val();
            let idFiscalizador:number = $("#IdFiscalizador").val();

            if(idFiscalizador == null){
                idFiscalizador = 0;
            }

            this.GetContratos(IdSujEcon,idFiscalizador,idDireccion)
        });

        $("#IdFiscalizador").on('change', (e)=> {
            let IdSujEcon:number = $("#IdSujEcon").val();
            let idDireccion:number = $("#IdDireccion").val();
            let idFiscalizador:number = $("#IdFiscalizador").val();

            if(idFiscalizador == null){
                idFiscalizador = 0;
            }

            this.GetContratos(IdSujEcon,idFiscalizador,idDireccion)
        });



        $("#IdContrato").on('change', (e)=> {
            let id:number = $("#IdContrato").val();
            console.log("entró al change");
            this.GetTipoLOD(id);
        });

        /*
        $("#IdLibroObra").on('change', (e)=> {
            let id:number = $("#IdLibroObra").val();
            console.log("entró al change tipo libro");
            this.GetTipoLOD(id);
        });
        */

        $("#IdTipoLibro").on('change', (e)=> {
            let id:number = $("#IdTipoLibro").val();
            console.log("entró al change tipo tipo libro");
            this.GetTipoComunicacion(id);
        });

        $("#IdTipoComunicacion").on('change', (e)=> {
            let id:number = $("#IdTipoComunicacion").val();
            console.log("entró al change tipo comunicacion");
            this.GetSubtipo(id);
        });

        $("#IdSubtipoComunicacion").on('change', (e)=> {
            let id:number = $("#IdSubtipoComunicacion").val();
            console.log("entró al change subtipo");
            this.GetTipoDoc(id);
        });

    };

    private initModal(data,status,xhr):void{
        $.validator.unobtrusive.parse($(this.formName));
        this.btnSubmit = $('#btnSubmit').ladda();
        this.modalForm.initModal();
        this.modalForm.open();
    }
    private submit():void{
        this.btnSubmit.ladda('start');
    }

    private Filtro():void
    {
        
        var filtro = {
            Formulario:$('#chkFormulario:checkbox:checked').length > 0,
            DocTecnicos:$('#chkDocTecnico:checkbox:checked').length > 0,
            DocAdmin:$('#chkDocAdmin:checkbox:checked').length > 0,
            Otros:$('#chkDocOtros:checkbox:checked').length > 0,
            IdDireccion:$("#IdDireccion").val(),
            IdSujEcon:$("#IdSujEcon").val(),
            IdContrato:$("#IdContrato").val(),
            IdTipoLibroObra:$("#IdTipoLibro").val(),
            IdTipoComunicacion:$("#IdTipoComunicacion").val(),
            IdSubtipoComunicacion:$("#IdSubtipoComunicacion").val(),
            IdTipoDoc:$("#IdTipoDoc").val(),
            FechaCreacion:$("#FechaCreacion").val()
        }

        this.Events.bind("OnGetPartial",()=>{
            var tableHelper = new TableHelper('#tableDatos', false);
        });
        this.getPartial("#divTableDatos","/GLOD/Biblioteca/GetFiltro",filtro);
    }

    private saveResult(data,status,xhr):void{
        var btns = $(".ladda-button").ladda();
        btns.ladda('stop');
            if(data == 'true'){
                this.Events.bind("OnGetPartial",()=>{
                    var tableHelper = new TableHelper('#tableDatos',false,true,true);
                });
                this.getPartial("#divTableDatos",this.urlGetTable,{});
                this.alert.toastOk();
                this.modalForm.close();
                return;
            } else {
                 this.alert.toastErrorData(data);
            }
    }

    private GetLibrosObra(IdSub:number):void{
        this.Events.bind("OnGetPartial",()=>{
            $(".panel-load").removeClass("sk-loading");  
            $("#IdLibroObra").select2({
                allowClear: false,
                placeholder: 'Seleccione un Libro',
                theme: "bootstrap"
            });

        });
        //$(".panel-load").addClass("sk-loading");
        this.getPartial("#DivLibros","/GLOD/Biblioteca/GetLibroObra",{"IdContrato":IdSub});  
    }

    private GetAux(IdAlgo:number):void{

    }

    private GetTiposLOD(IdSub:number):void{
        this.Events.bind("OnGetPartial",()=>{
            //$(".panel-load").removeClass("sk-loading");  
            $("#IdTipoLibro").select2({
                placeholder: 'Seleccione un Tipo de Libro',
                    allowClear: true,
                    theme: "bootstrap"
            });

        });
        //$(".panel-load").addClass("sk-loading");
        this.getPartial("#DivTipoLod","/GLOD/Biblioteca/GetTipoLOD",{"IdContrato":IdSub});  
    }

    private GetTipoComunicacion(IdSub:number):void{
        this.Events.bind("OnGetPartial",()=>{
            //$(".panel-load").removeClass("sk-loading");  
            $("#IdTipoComunicacion").select2({
                placeholder: {
                    id: '0', // the value of the option
                    text: 'Seleccione un Tipo de Comunicación'
                  },
                    allowClear: true,
                    theme: "bootstrap"
            }).on('change', (e)=> {
                let id:number = $("#IdTipoComunicacion").val();
                console.log("entró al change tipo comunicacion");
                this.GetSubtipo(id);
            });

            var $option = $("<option selected></option>").val('0').text("Seleccione un Tipo de Comunicación");
            $("#IdTipoComunicacion").append($option).trigger('change');
            $("#IdTipoComunicacion").val(0);

        });
        //$(".panel-load").addClass("sk-loading");
        this.getPartial("#DivTipoCom","/GLOD/Biblioteca/GetTipoCom",{"IdTipoLod":IdSub});  
    }

    private GetSubtipo(IdSub:number):void{
        this.Events.bind("OnGetPartial",()=>{
            //$(".panel-load").removeClass("sk-loading");  
            $("#IdSubtipoComunicacion").select2({
                placeholder: {
                    id: '0', // the value of the option
                    text: 'Seleccione un Tipo de Subtipo de Comunicación'
                  },
                    allowClear: true,
                    theme: "bootstrap"
            }).on('change', (e)=> {
                let id:number = $("#IdSubtipoComunicacion").val();
                console.log("entró al change subtipo");
                this.GetTipoDoc(id);
            });

            var $option = $("<option selected></option>").val('0').text("Seleccione un Subtipo de comunicación");
            $("#IdSubtipoComunicacion").append($option).trigger('change');
            $("#IdSubtipoComunicacion").val(0);

        });
        //$(".panel-load").addClass("sk-loading");
        this.getPartial("#DivSubtipo","/GLOD/Biblioteca/GetSubtipo",{"IdTipoCom":IdSub});  
    }

    private GetTipoDoc(IdSub:number):void{
        this.Events.bind("OnGetPartial",()=>{
            //$(".panel-load").removeClass("sk-loading");  
            $("#IdTipoDoc").select2({
                placeholder: {
                    id: '0', // the value of the option
                    text: 'Seleccione un Tipo de Documento'
                  },
                    allowClear: true,
                    theme: "bootstrap",
            });

            var $option = $("<option selected></option>").val('0').text("Seleccione un Tipo de Documento");
            $("#IdTipoDoc").append($option).trigger('change');
            $("#IdTipoDoc").val(0);

        });
        //$(".panel-load").addClass("sk-loading");
        this.getPartial("#DivTipoDoc","/GLOD/Biblioteca/GetTipoDoc",{"IdTipoSub":IdSub});  
    }

  
    private GetFiscalizador(IdSuj:number):void{
        console.log(IdSuj);
        this.Events.bind("OnGetPartial",()=>{
            //$(".panel-load").removeClass("sk-loading");  
            $("#IdFiscalizador").select2({
                placeholder: {
                    id: '0', // the value of the option
                    text: 'Seleccione un Fiscalizador'
                  },
                    allowClear: true,
                    theme: "bootstrap"
            }).on('change', (e)=> {
                let IdSujEcon:number = $("#IdSujEcon").val();
                let idDireccion:number = $("#IdDireccion").val();
                let idFiscalizador:number = $("#IdFiscalizador").val();
                console.log("entró al change ");
                this.GetContratos(IdSujEcon,idFiscalizador,idDireccion)
            });

            var $option = $("<option selected></option>").val('0').text("Seleccione un Fiscalizador");
            $("#IdFiscalizador").append($option).trigger('change');
            $("#IdFiscalizador").val('0');

        });
        //$(".panel-load").addClass("sk-loading");
        this.getPartial("#DivFiscalizador","/GLOD/Biblioteca/GetFiscalizador",{"IdDireccion":IdSuj});  
    }

    private GetSujetos(IdSuj:number):void{
        console.log(IdSuj);
        this.Events.bind("OnGetPartial",()=>{
            //$(".panel-load").removeClass("sk-loading");  
            $("#IdSujEcon").select2({
                placeholder: {
                    id: '0', // the value of the option
                    text: 'Seleccione un Contratista'
                  },
                    allowClear: true,
                    theme: "bootstrap"
            }).on('change', (e)=> {
                let IdSujEcon:number = $("#IdSujEcon").val();
                let idDireccion:number = $("#IdDireccion").val();
                let idFiscalizador:number = $("#IdFiscalizador").val();
                console.log("entró al change ");
                this.GetContratos(IdSujEcon,idFiscalizador,idDireccion)
            });

            var $option = $("<option selected></option>").val('0').text("Seleccione un Contratista");
            $("#IdSujEcon").append($option).trigger('change');
            $("#IdSujEcon").val(0);

        });
        //$(".panel-load").addClass("sk-loading");
        this.getPartial("#DivSujetos","/GLOD/Biblioteca/GetSujEcon",{"IdDireccion":IdSuj});  
    }
 
    private GetContratos(IdSujEcon:number, IdFiscalizador:number, IdDireccion:number):void{
        console.log("Entro al GetContratos");
        console.log(IdSujEcon);
        console.log(IdDireccion);
        console.log(IdFiscalizador);
        this.Events.bind("OnGetPartial",()=>{
            //$(".panel-load").removeClass("sk-loading");  
            $("#IdContrato").select2({
                placeholder: {
                    id: '0', // the value of the option
                    text: 'Seleccione un Contrato'
                  },
                    allowClear: true,
                    theme: "bootstrap"
            }).on('change', (e)=> {
                let id:number = $("#IdContrato").val();
                console.log("entró al change");
                this.GetTipoLOD(id);
            });

            var $option = $("<option selected></option>").val('0').text("Seleccione un Contrato");
            $("#IdContrato").append($option).trigger('change');
            $("#IdContrato").val(0);

        });
        //$(".panel-load").addClass("sk-loading");
        this.getPartial("#DivContrato","/GLOD/Biblioteca/GetContratos",{"IdSujEcon":IdSujEcon,"IdFiscalizador":IdFiscalizador,"IdDireccion":IdDireccion});  
    }

    private GetTipoLOD(IdSub:number):void{
        this.Events.bind("OnGetPartial",()=>{
            //$(".panel-load").removeClass("sk-loading");  
            $("#IdTipoLibro").select2({
                placeholder: {
                    id: '0', // the value of the option
                    text: 'Seleccione un Tipo Libro'
                  },
                    allowClear: true,
                    theme: "bootstrap"
            }).on('change', (e)=> {
            let id:number = $("#IdTipoLibro").val();
            console.log("entró al change tipo tipo libro");
            this.GetTipoComunicacion(id);
            });

            var $option = $("<option selected></option>").val('0').text("Seleccione un Tipo Libro");
            $("#IdTipoLibro").append($option).trigger('change');
            $("#IdTipoLibro").val(0);
            
        });
        //$(".panel-load").addClass("sk-loading");
        this.getPartial("#DivTipoLod","/GLOD/Biblioteca/GetTipoLOD",{"IdContrato":IdSub});  
    }
}
