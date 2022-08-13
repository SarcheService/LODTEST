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
var GLOD_Liquidacion = (function (_super) {
    __extends(GLOD_Liquidacion, _super);
    function GLOD_Liquidacion() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalLiquidacion";
        _this.formName = "#formLiquidacion";
        _this.modalForm = new ModalHelper(_this.modalName);
        $("#IdContrato").select2({
            placeholder: {
                id: '0',
                text: 'Seleccione un Contrato'
            },
            allowClear: true,
            theme: "bootstrap"
        });
        $("#IdContrato").on('change', function (e) {
            _this.Events.bind("OnGetPartial", function () {
                $("#tablaDatos").dataTable().fnDestroy();
                $("#tablaLibros").dataTable().fnDestroy();
                var tableHelper = new TableHelper('#tablaDatos', false, true, true);
                var tableHelper = new TableHelper('#tablaLibros', false, true, true);
            });
            var idContrato = $("#IdContrato").val();
            _this.getPartialAsync("#getInfoContrato", "/GLOD/LiquidacionContrato/GetInfoContrato", { id: idContrato });
            _this.getPartialAsync("#getDocumentos", "/GLOD/LiquidacionContrato/GetDocumentos", { id: idContrato });
            _this.getPartialAsync("#getLibros", "/GLOD/LiquidacionContrato/GetLibros", { id: idContrato });
        });
        return _this;
    }
    GLOD_Liquidacion.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        this.modalForm.open();
    };
    GLOD_Liquidacion.prototype.LoadLadda = function () {
        var btns = $(".ladda-button").ladda();
        btns.ladda('start');
        this.btnSubmit = $('#btnSubmit').ladda();
        this.btnSubmit.submit();
        $("#btnCancel").prop("disabled", true);
    };
    GLOD_Liquidacion.prototype.saveResult = function (data, status, xhr) {
        var data1 = data.split(";");
        if (data1[0] == 'true') {
            this.getPartialAsync("#getInfoContrato", "/GLOD/LiquidacionContrato/GetInfoContrato", { id: data1[1] });
            this.getPartialAsync("#getDocumentos", "/GLOD/LiquidacionContrato/GetDocumentos", { id: data1[1] });
            this.getPartialAsync("#getLibros", "/GLOD/LiquidacionContrato/GetLibros", { id: data1[1] });
            this.alert.toastOk();
            this.modalForm.close();
            $(".ladda-button").ladda('stop');
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    return GLOD_Liquidacion;
}(GenericController));
