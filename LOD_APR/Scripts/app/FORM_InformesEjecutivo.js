var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var FORM_InformesEjecutivo = (function (_super) {
    __extends(FORM_InformesEjecutivo, _super);
    function FORM_InformesEjecutivo() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalFormReporte";
        _this.formName = "#formFormEval";
        _this.urlGetTable = "/GLOD/FormInformes/GetTableEjecutivo";
        var tableHelper = new TableHelper('#tablaReportes', true);
        var tableHelper = new TableHelper('#tablaReportesIncidentes', true);
        return _this;
    }
    FORM_InformesEjecutivo.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm = new ModalHelper(this.modalName);
        this.modalForm.initModal();
        this.modalForm.open();
    };
    FORM_InformesEjecutivo.prototype.OnBeginActivate = function () {
        this.btnSubmit = $('#BtnSubmitActivate').ladda();
        this.btnSubmit.ladda('start');
    };
    FORM_InformesEjecutivo.prototype.saveResult = function (data, status, xhr) {
        var result = data.split(';');
        if (result[0] == 'true') {
            window.location.href = "/GLOD/FormInformes/ViewReport/" + result[1];
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(result[1]);
        }
    };
    FORM_InformesEjecutivo.prototype.saveResultDelete = function (data, status, xhr) {
        var result = data.split(';');
        if (result[0] == 'true') {
            this.Events.bind("OnGetPartial", function () {
                var tableHelper = new TableHelper('#tablaReportes', true);
            });
            this.getPartial("#DivTablaDatos", this.urlGetTable, { id: result[1] });
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(result[1]);
        }
    };
    FORM_InformesEjecutivo.prototype.InitFormModal = function (data, status, xhr) {
        var _this = this;
        this.IdFormulario = $('#IdInforme').val();
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
            errorPlacement: function (error, element) {
                error.appendTo($("#error_" + element[0].name));
            }
        });
        $(".multi-line").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength: 5000,
                messages: {
                    required: "Dato Obligatorio",
                    maxlength: 'El largo Máximo permitido es de {0}'
                }
            });
        });
        $(".text-box").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength: 200,
                messages: {
                    required: "Dato Obligatorio",
                    maxlength: 'El largo Máximo permitido es de {0}'
                }
            });
        });
        $(".combo").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                messages: {
                    required: "Dato Obligatorio",
                }
            });
        });
        $(".select-unica").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                messages: {
                    required: "Debe seleccionar una de las opciones",
                }
            });
        });
        $(".select-multiple").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                messages: {
                    required: "Debe seleccionar una o más opciones",
                }
            });
        });
        $(".entero").mask("#.##0", { reverse: true });
        $(".entero").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength: 15,
                messages: {
                    required: "Dato Obligatorio",
                    maxlength: 'El largo Máximo permitido es de {0}'
                }
            });
        });
        $(".decimal").mask("#.##0,99", { reverse: true });
        $(".decimal").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength: 18,
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
        $(".fecha").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength: 10,
                minlength: 10,
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
            sideBySide: true,
        });
        $(".fechahora").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength: 16,
                minlength: 16,
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
        $(".hora").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength: 5,
                minlength: 5,
                messages: {
                    required: "Dato Obligatorio",
                    maxlength: 'El largo Máximo permitido es de {0}',
                    minlength: 'El largo mínimo permitido es de {0}'
                }
            });
        });
        $(".telefono").mask("+56-000000000");
        $(".telefono").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength: 13,
                minlength: 13,
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
        $(".rut").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength: 13,
                minlength: 8,
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
        $(".direccionip").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength: 15,
                minlength: 7,
                messages: {
                    required: "Dato Obligatorio",
                    maxlength: 'El largo Máximo permitido es de {0}',
                    minlength: 'El largo mínimo permitido es de {0}'
                }
            });
        });
        $(".email").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                email: true,
                messages: {
                    required: "Dato Obligatorio",
                    email: 'Ingrese un email válido'
                }
            });
        });
        $(".moneda").mask("#.##0", { reverse: true });
        $(".moneda").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength: 17,
                messages: {
                    required: "Dato Obligatorio",
                    maxlength: 'El largo Máximo permitido es de {0}'
                }
            });
        });
        $("#formEncuesta").on("submit", function (event) {
            var form = $("#formEncuesta");
            form.validate();
            if (form.valid() === false) {
                return false;
            }
            else {
                _this.btnSubmit = $('#btnSubmitForm').ladda();
                _this.btnSubmit.ladda('start');
                _this.CheckRespuestas();
                event.preventDefault();
                $(form).ajaxSubmit({
                    data: {
                        __Ejecucion: JSON.stringify(_this.FormExecute),
                        IdFormulario: _this.IdFormulario
                    },
                    uploadProgress: function (event, position, total, percentComplete) {
                        $("#pbDocumentHelperV2").css("width", percentComplete + "%");
                        $("#pbDocumentHelperV2").text(percentComplete + "%");
                    },
                    success: function (data, status, xhr) {
                        _this.SaveResultForm(data, status, xhr);
                    },
                });
            }
        });
        this.modalForm = new ModalHelper(this.modalName);
        this.modalForm.initModal();
        this.modalForm.open();
        console.log(this.IdFormulario);
    };
    FORM_InformesEjecutivo.prototype.SaveResultForm = function (data, status, xhr) {
        var r = data.split(";");
        if (r[0] == 'true') {
            this.alert.toastOk();
            if (r[3]) {
                this.Events.bind("OnGetPartial", function () {
                    var tableHelper = new TableHelper('#tablaReportesIncidentes', true);
                });
                this.getPartial("#DivTablaDatos_Incidentes", '/GLOD/FormInformes/GetTableIncidentes', { id: r[3] });
            }
            else {
                this.getPartial("#DivTablaDatos_" + r[2], '/GLOD/FormInformes/GetTableFormItem', { id: r[1], tipo: r[2] });
            }
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(r[1]);
            return;
        }
    };
    FORM_InformesEjecutivo.prototype.CheckRespuestas = function () {
        var _this = this;
        $('.respuesta[data-id-item]').each(function (item, value) {
            var idItem = $(value).attr('data-id-item');
            var existe = $.grep(_this.FormExecute.ItemsData, function (e) { return e.ItemID == idItem; });
            if (existe.length == 0) {
                var newItem = {
                    ItemID: idItem,
                    ItemName: '',
                    Fields: []
                };
                _this.FormExecute.ItemsData.push(newItem);
            }
        });
        $('.respuesta[data-id-preg]').each(function (item, value) {
            var aplica = false;
            var idItem = $(value).attr('data-id-item');
            var idPregunta = $(value).attr('data-id-preg');
            var idAlterna = $(value).attr('data-id-alterna');
            var idTipo = $(value).attr('data-idtipo');
            var NewRespuesta;
            var item = $.grep(_this.FormExecute.ItemsData, function (e) { return e.ItemID == idItem; });
            if (idTipo < 913) {
                NewRespuesta = {
                    FieldID: idPregunta,
                    FieldName: '',
                    Type: {
                        Type: idTipo,
                        TypeName: '',
                        TypeWidth: 10
                    },
                    FieldValue: $(value).val(),
                    Options: []
                };
                item[0].Fields.push(NewRespuesta);
            }
            else if (idTipo == 913) {
                NewRespuesta = {
                    FieldID: idPregunta,
                    FieldName: '',
                    Type: {
                        Type: idTipo,
                        TypeName: '',
                        TypeWidth: 10
                    },
                    FieldValue: $(value).children("option").filter(":selected").text(),
                    Options: []
                };
                item[0].Fields.push(NewRespuesta);
            }
            else if (idTipo == 914 || idTipo == 915) {
                var selected = false;
                if ($(value).is(':checked')) {
                    selected = true;
                }
                ;
                var existe = $.grep(item[0].Fields, function (e) { return e.FieldID == idPregunta; });
                if (existe.length == 0) {
                    NewRespuesta = {
                        FieldID: idPregunta,
                        FieldName: '',
                        Type: {
                            Type: idTipo,
                            TypeName: '',
                            TypeWidth: 10
                        },
                        FieldValue: '',
                        Options: [{
                                OptionID: idAlterna,
                                OptionName: '',
                                Selected: selected
                            }]
                    };
                    item[0].Fields.push(NewRespuesta);
                }
                else {
                    existe[0].Options.push({
                        OptionID: idAlterna,
                        OptionName: '',
                        Selected: selected
                    });
                }
            }
            ;
        });
    };
    return FORM_InformesEjecutivo;
}(GenericController));
