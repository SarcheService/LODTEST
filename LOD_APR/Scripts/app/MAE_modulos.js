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
var MAE_modulos = (function (_super) {
    __extends(MAE_modulos, _super);
    function MAE_modulos(_Area, _Controller, IdSistema) {
        var _this = _super.call(this) || this;
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalModulo";
        _this.formName = "#formDatos";
        _this.urlGetTable = "/" + _Area + "/" + _Controller + "/getTable";
        _this.IdSistema = IdSistema;
        var tablaDatos = new TableHelper('#tablaDatos', false);
        _this.modalForm = new ModalHelper(_this.modalName);
        return _this;
    }
    MAE_modulos.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        this.modalForm.open();
    };
    MAE_modulos.prototype.AddParametro = function () {
        var IndiceParametroNew = Number($("#CountParametros").val()) + 1;
        $("#CountParametros").val(IndiceParametroNew);
        var TrCopy = $("#TrCopy").clone().removeClass("hidden");
        $(TrCopy).attr("id", "");
        $(TrCopy).find("#IdModuloParametro").attr("name", "MAE_ParametrosModulo[" + IndiceParametroNew + "].IdModulo");
        $(TrCopy).find("#NombreParametro").attr("name", "MAE_ParametrosModulo[" + IndiceParametroNew + "].Parametro");
        $(TrCopy).find("#ValorParametro").attr("name", "MAE_ParametrosModulo[" + IndiceParametroNew + "].Valor");
        $(TrCopy).find("a").click(function () {
            TrCopy.remove();
        });
        $("#divParametros").append(TrCopy);
    };
    MAE_modulos.prototype.DeleteParametro = function (id) {
        var TrCopy = $("#Parametro-" + id).remove();
    };
    MAE_modulos.prototype.saveResult = function (data, status, xhr) {
        if (data == 'true') {
            this.alert.toastOk();
            this.modalForm.close();
            this.getTable();
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    MAE_modulos.prototype.getTable = function () {
        this.getPartial("#divTableDatos", this.urlGetTable, { Id: this.IdSistema });
        var tablaDatos = new TableHelper('#tablaDatos', false);
    };
    return MAE_modulos;
}(GenericController));
