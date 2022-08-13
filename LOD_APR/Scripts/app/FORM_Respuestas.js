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
var FORM_Respuestas = (function (_super) {
    __extends(FORM_Respuestas, _super);
    function FORM_Respuestas(_id, _tipo, _primarykey) {
        var _this = _super.call(this) || this;
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalForm";
        _this.formName = "#form-";
        _this.urlGetForm = "/Admin/Ejecucion/ModalFormItems";
        _this.modalForm = new ModalHelper(_this.modalName);
        _this.TipoEjecucion = _tipo;
        _this.PrimaryKey = _primarykey;
        if (_tipo == 0) {
            _this.urlGetResultOk = "/Admin/Ejecucion/FormEnd";
            _this.urlGetResultNotOk = "/Admin/Ejecucion/FormError";
        }
        else if (_tipo == 1) {
            _this.urlGetResultOk = "/Admin/Ejecucion/ExecFormResultHelpDesk";
            _this.urlGetResultNotOk = "/Admin/Ejecucion/ExecFormResultHelpDesk";
        }
        _this.alert = new AlertHelper();
        _this.IdFormulario = _id;
        var form = $('#formEncuesta');
        _this.Token = $('input[name="__RequestVerificationToken"]', form).val();
        _this.FormExecute = {
            FormID: _id,
            ItemsData: [],
            FormsData: []
        };
        $('.form-table').each(function (item, value) {
            var newForm = {
                FormID: value.dataset.idform,
                FormName: '',
                Fields: [],
                Rows: []
            };
            _this.FormExecute.FormsData.push(newForm);
        });
        $('.form-table tbody').on('click', '[name=btnDelete]', function (element) {
            var idform = $(element.target)[0].dataset.idform;
            var t = $('#form-table-' + idform).DataTable();
            var indice = $(element.target).closest('tr').index();
            t
                .row($(element.target).parents('tr'))
                .remove()
                .draw();
            var formData = $.grep(_this.FormExecute.FormsData, function (e) { return e.FormID == idform; });
            formData[0].Rows.splice(indice, 1);
        });
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
                        _this.SaveResult(data, status, xhr);
                    },
                });
            }
        });
        return _this;
    }
    FORM_Respuestas.prototype.SaveResult = function (data, status, xhr) {
        var r = data.split(";");
        if (r[0] == 'true') {
            this.getPartial("#divEjecutionForm", this.urlGetResultOk, { id: r[1], primarykey: this.PrimaryKey });
            return;
        }
        else {
            this.getPartial("#divEjecutionForm", this.urlGetResultNotOk, { error: r[1], primarykey: this.PrimaryKey });
            return;
        }
    };
    FORM_Respuestas.prototype.CheckRespuestas = function () {
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
    FORM_Respuestas.prototype.initModal = function (data, status, xhr) {
        $(this.formName).validate({
            errorPlacement: function (error, element) {
                error.appendTo($("#error_" + element[0].name));
            }
        });
        $(".form-multi-line").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength: 5000,
                messages: {
                    required: "Dato Obligatorio",
                    maxlength: 'El largo Máximo permitido es de {0}'
                }
            });
        });
        $(".form-text-box").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength: 200,
                messages: {
                    required: "Dato Obligatorio",
                    maxlength: 'El largo Máximo permitido es de {0}'
                }
            });
        });
        $(".form-combo").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                messages: {
                    required: "Dato Obligatorio",
                }
            });
        });
        $(".form-entero").mask("#.##0", { reverse: true });
        $(".form-entero").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength: 15,
                messages: {
                    required: "Dato Obligatorio",
                    maxlength: 'El largo Máximo permitido es de {0}'
                }
            });
        });
        $(".form-decimal").mask("#.##0,99", { reverse: true });
        $(".form-decimal").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength: 18,
                messages: {
                    required: "Dato Obligatorio",
                    maxlength: 'El largo Máximo permitido es de {0}',
                }
            });
        });
        $('.form-fecha').datepicker({
            language: 'es',
            format: "dd-mm-yyyy",
            today: true,
            todayHighlight: true,
            autoclose: true
        });
        $(".form-fecha").each(function (item, value) {
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
        $('.form-fechahora').datetimepicker({
            locale: 'es',
            format: "DD-MM-YYYY HH:mm",
            sideBySide: true,
        });
        $(".form-fechahora").each(function (item, value) {
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
        $('.form-hora').datetimepicker({
            locale: 'es',
            format: "HH:mm",
        });
        $(".form-hora").each(function (item, value) {
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
        $(".form-telefono").mask("+56-000000000");
        $(".form-telefono").each(function (item, value) {
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
        $(".form-rut").rut({
            formatOn: 'blur',
            minimumLength: 8,
            validateOn: 'change'
        });
        $(".form-rut").each(function (item, value) {
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
        $('.form-direccionip').mask('0ZZ.0ZZ.0ZZ.0ZZ', {
            translation: {
                'Z': {
                    pattern: /[0-9]/, optional: true
                }
            },
            placeholder: "0.0.0.0"
        });
        $(".form-direccionip").each(function (item, value) {
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
        $(".form-email").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                email: true,
                messages: {
                    required: "Dato Obligatorio",
                    email: 'Ingrese un email válido'
                }
            });
        });
        $(".form-moneda").mask("#.##0", { reverse: true });
        $(".form-moneda").each(function (item, value) {
            $(value).rules("add", {
                required: JSON.parse(value.dataset.obligatoria.toLowerCase()),
                maxlength: 17,
                messages: {
                    required: "Dato Obligatorio",
                    maxlength: 'El largo Máximo permitido es de {0}'
                }
            });
        });
        this.modalForm = new ModalHelper(this.modalName);
        this.modalForm.initModal();
        this.modalForm.open();
    };
    FORM_Respuestas.prototype.OnBeginModal = function (idForm) {
        this.formName = "#form-" + idForm;
    };
    FORM_Respuestas.prototype.serializeForm = function (IdForm) {
        var newRow = [];
        var tableRow = [];
        $('#form-' + IdForm + ' .form-field[data-id-preg]').each(function (item, value) {
            var idTipo = $(value).attr('data-idtipo');
            var valor = "";
            if (idTipo == 913) {
                valor = $(value).children("option").filter(":selected").text();
            }
            else {
                valor = $(value).val();
            }
            newRow.push(valor);
            tableRow.push(valor);
        });
        var form = $(this.formName);
        form.validate();
        if (form.valid() === false) {
            return false;
        }
        else {
            var formData = $.grep(this.FormExecute.FormsData, function (e) { return e.FormID == IdForm; });
            formData[0].Rows.push(newRow);
            var btnBorrar = '<a class="btn btn-xs btn-danger" name="btnDelete" data-idform="' + IdForm + '"><i class="fa fa-trash"></i> Borrar</a>';
            tableRow.push(btnBorrar);
            var t = $('#form-table-' + IdForm).DataTable();
            t.row.add(tableRow).draw(false);
            this.modalForm.close();
        }
    };
    return FORM_Respuestas;
}(GenericController));
