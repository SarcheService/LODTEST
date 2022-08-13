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
var LOD_CierreLibro = (function (_super) {
    __extends(LOD_CierreLibro, _super);
    function LOD_CierreLibro() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalCierre";
        _this.formName = "#formTipo";
        _this.modalForm = new ModalHelper(_this.modalName);
        var tableHelper = new TableHelper('#tablaDatos', true);
        var tableHelper = new TableHelper('#tablaDatos2', true);
        return _this;
    }
    LOD_CierreLibro.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        this.modalForm.open();
    };
    LOD_CierreLibro.prototype.InitModalTipo = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.selectTipo = new SelectHelper("#Tipo", "/Admin/TipoDoc/getJsonTipos", "< Seleccione Tipo >", false, false, true);
        this.selectTipo.initSelectAjax();
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });
        this.modalForm.initModal();
        this.modalForm.open();
    };
    LOD_CierreLibro.prototype.InitModalEdit = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });
        this.modalForm.initModal();
        this.modalForm.open();
    };
    LOD_CierreLibro.prototype.saveResultCierre = function (data, status, xhr) {
        var data1 = data.split(";");
        if (data1[0] == "true") {
            this.getPartial("#divInfoLibros", "/GLOD/LibroObras/getLibro", { id: data1[1] });
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            $('#btnSubmit').stop();
            this.alert.toastErrorData(data);
            $(".ladda-button").ladda('stop');
        }
    };
    return LOD_CierreLibro;
}(GenericController));
