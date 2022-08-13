/// <reference path="CommonHelper.ts"/>

class FORM_InformesEjecutivo extends GenericController{
    //ELEMENTOS DE INTERFAZ DE USUARIO
    private selectEmail: SelectHelper;
    private modalCanvas:string;
    private formName: string;
    private modalName:string;
    private urlGetTable:string;
    private modalForm:ModalHelper
    public readonly urlGetTree:string;
    private btnSubmit:any;
    private IdFormulario:number;
    private Token:string;
    private FormExecute:any;

    constructor(){
        super();
        this.alert = new AlertHelper()
        this.modalCanvas="#modalCanvas";
        this.modalName="#modalFormReporte";
        this.formName="#formFormEval";
        this.urlGetTable = "/GLOD/FormInformes/GetTableEjecutivo";
        var tableHelper = new TableHelper('#tablaReportes', true);
        var tableHelper = new TableHelper('#tablaReportesIncidentes',true);
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
                window.location.href = "/GLOD/FormInformes/ViewReport/" + result[1];
                this.alert.toastOk();
                this.modalForm.close();
                return;
            } 
            else {
                 this.alert.toastErrorData(result[1]);
            }
    }

    private saveResultDelete(data,status,xhr):void{
        var result = data.split(';');
        if(result[0] == 'true')
        {   
            this.Events.bind("OnGetPartial",()=>{
                var tableHelper = new TableHelper('#tablaReportes',true);
            });
            this.getPartial("#DivTablaDatos",this.urlGetTable,{id : result[1]});
            this.alert.toastOk();
            this.modalForm.close();
            return;
        } 
        else {
             this.alert.toastErrorData(result[1]);
        }
    }


    private InitFormModal(data,status,xhr):void{

        this.IdFormulario=$('#IdInforme').val();
        
        var form = $('#formEncuesta');
        this.Token = $('input[name="__RequestVerificationToken"]', form).val();
        this.FormExecute = {
            FormID: this.IdFormulario,
            ItemsData: [],
            FormsData: []
        };

        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });
        
        $("#formEncuesta").validate({
            errorPlacement: function(error, element) {
                error.appendTo($("#error_" + element[0].name));
            }
        });

        $(".multi-line").each((item,value)=> {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength:5000,
                messages: {
                    required: "Dato Obligatorio",
                    maxlength: 'El largo Máximo permitido es de {0}'
                } 
            });
        });

        $(".text-box").each((item,value)=> {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength:200,
                messages: {
                    required: "Dato Obligatorio",
                    maxlength: 'El largo Máximo permitido es de {0}'
                } 
            });
        });

        $(".combo").each((item,value)=> {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                messages: {
                    required: "Dato Obligatorio",
                } 
            });
        });

            $(".select-unica").each((item,value)=> {
                $(value).rules("add", {
                    required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                    messages: {
                        required: "Debe seleccionar una de las opciones",
                      }
                });
            });

            $(".select-multiple").each((item,value)=> {
                $(value).rules("add", {
                    required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                    messages: {
                        required: "Debe seleccionar una o más opciones",
                      }
                });
            });

            $(".entero").mask("#.##0", {reverse: true});
            $(".entero").each((item,value)=> {
                $(value).rules("add", {
                    required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                    maxlength:15,
                    messages: {
                        required: "Dato Obligatorio",
                        maxlength: 'El largo Máximo permitido es de {0}'
                    } 
                });
            });

            $(".decimal").mask("#.##0,99", {reverse: true});
            $(".decimal").each((item,value)=> {
                $(value).rules("add", {
                    required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                    maxlength:18,
                    messages: {
                        required: "Dato Obligatorio",
                        maxlength: 'El largo Máximo permitido es de {0}',
                    } 
                });
            });

            $('.fecha').datepicker({
                language: 'es',
                format: "dd-mm-yyyy",
                today: true,
                todayHighlight: true,
                autoclose: true
            });
            $(".fecha").each((item,value)=> {
                $(value).rules("add", {
                    required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                    maxlength:10,
                    minlength:10,
                    messages: {
                        required: "Dato Obligatorio",
                        maxlength: 'El largo Máximo permitido es de {0}',
                        minlength: 'El largo mínimo permitido es de {0}'
                    } 
                });
            });

            $('.fechahora').datetimepicker({
                locale: 'es',
                format: "DD-MM-YYYY HH:mm",
                sideBySide:true,
            });
            $(".fechahora").each((item,value)=> {
                $(value).rules("add", {
                    required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                    maxlength:16,
                    minlength:16,
                    messages: {
                        required: "Dato Obligatorio",
                        maxlength: 'El largo Máximo permitido es de {0}',
                        minlength: 'El largo mínimo permitido es de {0}'
                    } 
                });
            });

            $('.hora').datetimepicker({
                locale: 'es',
                format: "HH:mm",
            });
            $(".hora").each((item,value)=> {
                $(value).rules("add", {
                    required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                    maxlength:5,
                    minlength:5,
                    messages: {
                        required: "Dato Obligatorio",
                        maxlength: 'El largo Máximo permitido es de {0}',
                        minlength: 'El largo mínimo permitido es de {0}'
                    } 
                });
            });

            $(".telefono").mask("+56-000000000");
            $(".telefono").each((item,value)=> {
                $(value).rules("add", {
                    required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                    maxlength:13,
                    minlength:13,
                    messages: {
                        required: "Dato Obligatorio",
                        maxlength: 'El largo Máximo permitido es de {0}',
                        minlength: 'El largo mínimo permitido es de {0}'
                    } 
                });
            });

            $(".rut").rut({
                formatOn: 'blur',
                minimumLength: 8,
                validateOn: 'change'
            });
            $(".rut").each((item,value)=> {
                $(value).rules("add", {
                    required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                    maxlength:13,
                    minlength:8,
                    messages: {
                        required: "Dato Obligatorio",
                        maxlength: 'El largo Máximo permitido es de {0}',
                        minlength: 'El largo mínimo permitido es de {0}'
                    } 
                });
            });

            $('.direccionip').mask('0ZZ.0ZZ.0ZZ.0ZZ', {
                translation: {
                  'Z': {
                    pattern: /[0-9]/, optional: true
                  }
                },
                placeholder: "0.0.0.0"
              });
              $(".direccionip").each((item,value)=> {
                $(value).rules("add", {
                    required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                    maxlength:15,
                    minlength:7,
                    messages: {
                        required: "Dato Obligatorio",
                        maxlength: 'El largo Máximo permitido es de {0}',
                        minlength: 'El largo mínimo permitido es de {0}'
                    } 
                });
            });

            $(".email").each((item,value)=> {
                $(value).rules("add", {
                    required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                    email: true,
                    messages: {
                        required: "Dato Obligatorio",
                        email: 'Ingrese un email válido'
                    } 
                });
            });

            $(".moneda").mask("#.##0", {reverse: true});
            $(".moneda").each((item,value)=> {
                $(value).rules("add", {
                    required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                    maxlength:17,
                    messages: {
                        required: "Dato Obligatorio",
                        maxlength: 'El largo Máximo permitido es de {0}'
                    } 
                });
            });

            $("#formEncuesta").on("submit",(event)=>{
               // $.validator.unobtrusive.parse($("#formEncuesta"));
                var form = $("#formEncuesta");
                form.validate();

                if (form.valid() === false) {
                    return false;
                }else{
                    this.btnSubmit = $('#btnSubmitForm').ladda();
                    this.btnSubmit.ladda('start');
                    this.CheckRespuestas();
                    event.preventDefault();
                    $(form).ajaxSubmit({
                        data: { 
                            __Ejecucion: JSON.stringify(this.FormExecute),
                            IdFormulario:this.IdFormulario},
                        uploadProgress: (event,position,total, percentComplete)=>{
                            $("#pbDocumentHelperV2").css("width", percentComplete + "%");
                            $("#pbDocumentHelperV2").text(percentComplete + "%")
                        },
                        success: (data,status,xhr)=>{
                            this.SaveResultForm(data,status,xhr);
                        },        
                    })
                }
            })

        this.modalForm = new ModalHelper(this.modalName);
        
        this.modalForm.initModal();
        this.modalForm.open();
        console.log(this.IdFormulario);
    }
    private SaveResultForm(data,status,xhr):void{
        var r = data.split(";");
        if(r[0]  == 'true')
        {      
            this.alert.toastOk();
            if(r[3]){
                this.Events.bind("OnGetPartial",()=>{
                    var tableHelper = new TableHelper('#tablaReportesIncidentes',true);
                });
                this.getPartial("#DivTablaDatos_Incidentes",'/GLOD/FormInformes/GetTableIncidentes',{id:r[3]});
            }else{
                this.getPartial("#DivTablaDatos_" + r[2],'/GLOD/FormInformes/GetTableFormItem',{id:r[1], tipo: r[2]});
            }
            this.modalForm.close();
            return;
        } 
        else 
        {
            this.alert.toastErrorData(r[1]);
            return;
        }
    }
    public CheckRespuestas():void{
     
        $('.respuesta[data-id-item]').each((item,value)=>{
            var idItem = $(value).attr('data-id-item');
            var existe =  $.grep(this.FormExecute.ItemsData, function(e){ return e.ItemID == idItem; });
            if(existe.length==0){
                var newItem = {
                    ItemID: idItem,
                    ItemName: '',
                    Fields: []
                };     
                this.FormExecute.ItemsData.push(newItem); 
            }

        });

        $('.respuesta[data-id-preg]').each((item,value)=>{
            var aplica = false;
            var idItem = $(value).attr('data-id-item');
            var idPregunta = $(value).attr('data-id-preg');
            var idAlterna = $(value).attr('data-id-alterna');
            var idTipo = $(value).attr('data-idtipo');
            var NewRespuesta;
            var item = $.grep(this.FormExecute.ItemsData, function(e){ return e.ItemID == idItem; });
   
            if(idTipo<913){
                NewRespuesta = {
                    FieldID: idPregunta,
                    FieldName: '',
                    Type: {
                        Type:idTipo,
                        TypeName:'',
                        TypeWidth:10
                    },
                    FieldValue: $(value).val(),
                    Options:[]
                };
                item[0].Fields.push(NewRespuesta); 
            }else if(idTipo==913){//COMBO
                NewRespuesta = {
                    FieldID: idPregunta,
                    FieldName: '',
                    Type: {
                        Type:idTipo,
                        TypeName:'',
                        TypeWidth:10
                    },
                    FieldValue: $(value).children("option").filter(":selected").text(),
                    Options:[]
                };
                item[0].Fields.push(NewRespuesta); 
            }else if(idTipo==914 || idTipo==915){//SELECCIÓN MULTIPLE Y SELECCIÓN ÚNICA
               
                var selected = false
                if( $(value).is(':checked')){selected = true;};

                var existe = $.grep(item[0].Fields, function(e){ return e.FieldID == idPregunta; });
                if(existe.length==0){
                    NewRespuesta = {
                        FieldID: idPregunta,
                        FieldName: '',
                        Type: {
                            Type:idTipo,
                            TypeName:'',
                            TypeWidth:10
                        },
                        FieldValue: '',
                        Options:[{
                            OptionID: idAlterna,
                            OptionName:'',
                            Selected: selected
                        }]
                    };
                    item[0].Fields.push(NewRespuesta);
                }else{
                    existe[0].Options.push({
                        OptionID: idAlterna,
                        OptionName:'',
                        Selected: selected
                    });
                }
            
            };
        
        });

    }

}