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
var MAE_TipoComunicacion = (function (_super) {
    __extends(MAE_TipoComunicacion, _super);
    function MAE_TipoComunicacion() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalTipo";
        _this.formName = "#formTipo";
        _this.urlGetTable = "/Admin/TipoComunicacion/getTable";
        _this.modalForm = new ModalHelper(_this.modalName);
        return _this;
    }
    MAE_TipoComunicacion.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });
        $("#IdLOD").select2({
            placeholder: 'Seleccione el Tipo de Libro',
            allowClear: true,
            theme: "bootstrap"
        });
        this.modalForm.open();
    };
    MAE_TipoComunicacion.prototype.InitModalTipo = function (data, status, xhr) {
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
    MAE_TipoComunicacion.prototype.InitModalEdit = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });
        this.modalForm.initModal();
        this.modalForm.open();
    };
    MAE_TipoComunicacion.prototype.saveResult = function (data, status, xhr) {
        if (data == 'true') {
            this.Events.bind("OnGetPartial", function () {
                var tableHelper = new TableHelper('#tablaDatos', true);
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
    return MAE_TipoComunicacion;
}(GenericController));
