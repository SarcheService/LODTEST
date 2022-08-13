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
var MAE_sucursal = (function (_super) {
    __extends(MAE_sucursal, _super);
    function MAE_sucursal() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalSucursal";
        _this.formName = "#formSucursal";
        _this.urlGetTable = "/Admin/Sucursal/getTable";
        _this.modalForm = new ModalHelper(_this.modalName);
        var tableHelper = new TableHelper('#tablaDatos', true);
        var tableHelper = new TableHelper('#tablaSucursal', false, false, false);
        return _this;
    }
    MAE_sucursal.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        $(".panel-load").removeClass("sk-loading");
        $('#modalSucursal #Telefono').mask('+56 000000000', { placeholder: "+56 _________" });
        $(".select_IdCiudad").select2({
            allowClear: false,
            placeholder: 'Seleccione una ciudad',
            theme: "bootstrap"
        });
        $("#IdDireccion").select2({
            allowClear: false,
            placeholder: 'Seleccione una Dirección',
            theme: "bootstrap"
        });
        this.Events.bind("OnOpenModal", function () {
        });
        this.modalForm.initModal();
        this.modalForm.open();
    };
    MAE_sucursal.prototype.saveResult = function (data, status, xhr) {
        var datos = JSON.parse(data);
        if (!datos.error) {
            this.getPartial("#divTableDatos", this.urlGetTable, { IdSujeto: datos.idSujeto });
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    return MAE_sucursal;
}(GenericController));
