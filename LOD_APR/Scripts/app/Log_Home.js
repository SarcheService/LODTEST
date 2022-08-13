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
var Log_Home = (function (_super) {
    __extends(Log_Home, _super);
    function Log_Home(_IdContrato) {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalPassword";
        _this.formName = "#formPassword";
        _this.urlGetTable = "/GLOD/Log/getTable";
        _this.modalForm = new ModalHelper(_this.modalName);
        _this.IdContrato = _IdContrato;
        var tableHelper = new TableHelper('#tableDatos', false, true, true);
        $("#IdLod").select2({
            placeholder: {
                id: '0',
                text: 'Seleccione un Libro de Obra'
            },
            allowClear: true,
            theme: "bootstrap"
        }).val('0').trigger('change');
        var $option = $("<option selected></option>").val('0').text("Seleccione un Libro de Obra");
        $("#IdLod").append($option).trigger('change');
        $("#IdLod").val('0');
        $("#IdAnotacion").select2({
            placeholder: {
                id: '0',
                text: 'Seleccione una Anotación'
            },
            allowClear: true,
            theme: "bootstrap"
        }).val('').trigger('change');
        var $option = $("<option selected></option>").val('0').text("Seleccione un Anotación");
        $("#IdAnotacion").append($option).trigger('change');
        $("#IdAnotacion").val(0);
        $("#UserId").select2({
            placeholder: {
                id: '0',
                text: 'Seleccione un Usuario'
            },
            allowClear: true,
            theme: "bootstrap"
        }).val('').trigger('change');
        var $option = $("<option selected></option>").val('0').text("Seleccione un Usuario");
        $("#UserId").append($option).trigger('change');
        $("#UserId").val(0);
        $("#IdLod").on('change', function (e) {
            var id = $("#IdLod").val();
            console.log("entró al change ");
            console.log(id);
            _this.GetAnotaciones(id);
        });
        return _this;
    }
    ;
    Log_Home.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.btnSubmit = $('#btnSubmit').ladda();
        this.modalForm.initModal();
        this.modalForm.open();
    };
    Log_Home.prototype.submit = function () {
        this.btnSubmit.ladda('start');
    };
    Log_Home.prototype.Filtro = function () {
        var filtro = {
            IdLod: $("#IdLod").val(),
            IdAnotacion: $("#IdAnotacion").val(),
            UserId: $("#UserId").val(),
            IdContrato: this.IdContrato,
            FechaLog: $("#FechaLog").val()
        };
        this.Events.bind("OnGetPartial", function () {
            var tableHelper = new TableHelper('#tableDatos', false);
        });
        this.getPartial("#divTableDatos", "/GLOD/Log/GetFiltro", filtro);
    };
    Log_Home.prototype.saveResult = function (data, status, xhr) {
        var btns = $(".ladda-button").ladda();
        btns.ladda('stop');
        if (data == 'true') {
            this.Events.bind("OnGetPartial", function () {
                var tableHelper = new TableHelper('#tableDatos', false, true, true);
            });
            this.getPartial("#divTableDatos", this.urlGetTable, {});
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    Log_Home.prototype.GetAnotaciones = function (IdLod) {
        this.Events.bind("OnGetPartial", function () {
            $(".panel-load").removeClass("sk-loading");
            $("#IdAnotacion").select2({
                allowClear: true,
                placeholder: 'Seleccione una Anotación',
                theme: "bootstrap"
            });
        });
        $(".panel-load").addClass("sk-loading");
        this.getPartial("#DivAnotacion", "/GLOD/Log/GetAnotaciones", { "IdLod": IdLod, "IdContrato": this.IdContrato });
    };
    return Log_Home;
}(GenericController));
