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
var LOD_RepContrato = (function (_super) {
    __extends(LOD_RepContrato, _super);
    function LOD_RepContrato(controller) {
        if (controller === void 0) { controller = "/GLOD/RepContrato"; }
        var _this = _super.call(this) || this;
        _this.modalCanvas = "#modalCanvas";
        _this.modalCanvas2 = "#modalCanvas2";
        _this.modalFormName = "#modalFormDocs";
        _this.modalDeleteName = "#modalFormDelete";
        _this.filesFormName = "#formDocs";
        _this.filesDeleteName = "#formDeleteDocs";
        _this.tableDivName = "#lstDocs_Repositorio";
        _this.tableName = "#tablaDocs_Repositorio";
        _this.urlAddModel = "/GLOD/RepContrato/AddFile";
        _this.urlGetModel = controller + '/GetDocumentos';
        _this.listFiles = [];
        _this.IdPadre = 0;
        var tableHelper = new TableHelper('#tablaDocs_Repositorio', false, true, true);
        _this.language = {
            "sProcessing": "Procesando...",
            "sLengthMenu": "Mostrar _MENU_ registros",
            "sZeroRecords": "No se encontraron resultados",
            "sEmptyTable": "No se encontraron datos para mostrar",
            "sInfo": "Mostrando _START_ al _END_ de  _TOTAL_ registros",
            "sInfoEmpty": "Mostrando 0 al 0 de 0 registros",
            "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
            "sInfoPostFix": "",
            "sSearch": "Buscar:",
            "sUrl": "",
            "sInfoThousands": ",",
            "sLoadingRecords": "Cargando...",
            "oPaginate": {
                "sFirst": "Primero",
                "sLast": "Último",
                "sNext": "Siguiente",
                "sPrevious": "Anterior"
            }
        };
        return _this;
    }
    LOD_RepContrato.prototype.onBegin = function () {
        this.btnSubmit.ladda('start');
        $('.panel-load').addClass('sk-loading');
    };
    LOD_RepContrato.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.filesFormName));
        this.modalForm = new ModalHelper(this.modalFormName);
        var tableHelper = new TableHelper('#tableDoc', false, true, true);
        this.modalForm.initModal();
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });
        this.modalForm.open();
    };
    LOD_RepContrato.prototype.initModalDelete = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.filesDeleteName));
        this.modalForm = new ModalHelper(this.modalDeleteName);
        this.modalForm.initModal();
        this.modalForm.open();
    };
    LOD_RepContrato.prototype.InitFormFile = function (_idPadre) {
        var _this = this;
        this.IdPadre = _idPadre;
        this.modalFormAdd = new ModalHelper(this.modalFormName);
        $.validator.unobtrusive.parse($(this.filesFormName));
        this.modalFormAdd.initModal();
        this.modalFormAdd.open();
        this.btnSubmit = $("#btnSubmit").ladda();
        $(".panel-load").removeClass("sk-loading");
        $('#Fecha_Documento').datepicker({
            language: 'es',
            format: "dd-mm-yyyy",
            today: true,
            todayHighlight: true,
            autoclose: true
        });
        $('#FechaVto').datepicker({
            language: 'es',
            format: "dd-mm-yyyy",
            today: true,
            todayHighlight: true,
            autoclose: true
        });
        $(this.filesFormName).ajaxForm({
            beforeSend: function () {
                _this.onBegin();
            },
            uploadProgress: function (event, position, total, percentComplete) {
                $("#progressBar").css("width", percentComplete + "%");
                $("#progressBar").text(percentComplete + "%");
            },
            success: function (data, status, xhr) {
                _this.SaveResult(data, status, xhr);
            },
            complete: function () {
                $(".panel-load").removeClass("sk-loading");
                var btns = $(".ladda-button").ladda();
                btns.ladda('stop');
                $("#progressBar").css("width", "0%");
                $("#progressBar").text("0%");
            }
        });
        $('#Fecha_Documento').datepicker({
            language: 'es',
            format: "dd-mm-yyyy",
            today: true,
            todayHighlight: true,
            autoclose: true
        });
        $('#FechaVto').datepicker({
            language: 'es',
            format: "dd-mm-yyyy",
            today: true,
            todayHighlight: true,
            autoclose: true
        });
        $("#fileName").change(function () {
            $("#btnSubmit").prop("disabled", this.files.length == 0);
        });
    };
    LOD_RepContrato.prototype.SaveResult = function (data, status, xhr) {
        console.log(data);
        var res = data.split("|");
        console.log(res);
        if (res[0] == 'true') {
            if (res[1] != undefined) {
                this.IdPadre = res[1];
            }
            this.GetListDocs(this.IdPadre);
            this.InitDatatablejs("/GLOD/RepContrato/GetDocumentosPage?Padre=" + this.IdPadre);
            this.alert.toastOk();
            if ($("#modalFormDocs").length > 0) {
                this.modalFormAdd.close();
            }
            if ($("#modalFormDocsInfo").length > 0) {
                this.modalFormInfo.close();
            }
            return;
        }
        else {
            $(".panel-load").removeClass("sk-loading");
            this.alert.toastErrorData(data);
            this.btnSubmit.ladda('stop');
        }
    };
    LOD_RepContrato.prototype.GetListDocs = function (_idPadre) {
        console.log(_idPadre, "padre");
        $(".panel-load").addClass("sk-loading");
        _super.prototype.getPartial.call(this, this.tableDivName, this.urlGetModel, { Padre: _idPadre });
    };
    LOD_RepContrato.prototype.PartialViewGet = function (_idPadre, controller) {
        if (controller === void 0) { controller = "/GLOD/RepContrato"; }
        $('.tooltip').not(this).hide();
        this.InitDatatablejs(controller + "/GetDocumentosPage?Padre=" + _idPadre);
    };
    LOD_RepContrato.prototype.ErrorMsg = function (data) {
        this.alert.toastErrorData(data);
    };
    LOD_RepContrato.prototype.check = function (e) {
        var tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 8) {
            return true;
        }
        var patron = /[A-Za-z0-9 ]/;
        var tecla_final = String.fromCharCode(tecla);
        return patron.test(tecla_final);
    };
    LOD_RepContrato.prototype.ValidaSpecial = function (event) {
        var Speciales = event.target.value.match(/[^A-Za-z0-9 ]/g);
        if (Speciales != null) {
            $("#FolderName").val(event.target.value.replace(/[^A-Za-z0-9 ]/g, ""));
            this.alert.toastWarningData("Los nombres de carpeta no puede contener caracteres especiales");
        }
    };
    LOD_RepContrato.prototype.InitDatatablejs = function (url) {
        $.fn.DataTable.ext.pager.numbers_length = 5;
        var NumRegistro = $("#NumRegistro").val();
        $('#tablaDocs_Repositorio').DataTable({
            "processing": true,
            "deferLoading": NumRegistro,
            "lengthMenu": [20],
            language: this.language
        });
        $('[data-toggle="tooltip"]').tooltip();
        $('#tablaDocs_Repositorio').on('draw.dt', function () {
            $('[data-toggle="tooltip"]').tooltip();
        });
    };
    return LOD_RepContrato;
}(GenericController));
