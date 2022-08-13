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
var MAE_opcionesMenu = (function (_super) {
    __extends(MAE_opcionesMenu, _super);
    function MAE_opcionesMenu(_Area, _Controller, IdModulo) {
        var _this = _super.call(this) || this;
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalOpcionesMenu";
        _this.formName = "#formDatos";
        _this.urlGetTable = "/" + _Area + "/" + _Controller + "/getTable";
        _this.IdModulo = IdModulo;
        var tablaDatos = new TableHelper('#tablaDatos', false);
        _this.modalForm = new ModalHelper(_this.modalName);
        return _this;
    }
    MAE_opcionesMenu.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        this.modalForm.open();
        if ($("#IdAcc").val() != "") {
            $("#IdOpcion").val($("#IdAcc").val());
            $("#IdOpcionVisual").val($("#IdOpcion").val());
        }
    };
    MAE_opcionesMenu.prototype.changeValueInput = function () {
        $("#IdOpcionVisual").val($("#IdOpcion").val());
    };
    MAE_opcionesMenu.prototype.saveResult = function (data, status, xhr) {
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
    MAE_opcionesMenu.prototype.getTable = function () {
        this.getPartial("#divTableDatos", this.urlGetTable, { Id: this.IdModulo });
        var tablaDatos = new TableHelper('#tablaDatos', false);
    };
    return MAE_opcionesMenu;
}(GenericController));
