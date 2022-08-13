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
var FORM_InformesIncidentes = (function (_super) {
    __extends(FORM_InformesIncidentes, _super);
    function FORM_InformesIncidentes() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalFormReporte";
        _this.formName = "#formFormEval";
        _this.urlGetTable = "/GLOD/FormInformes/GetTableIncidentes";
        var tableHelper = new TableHelper('#tablaReportes', true);
        return _this;
    }
    FORM_InformesIncidentes.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm = new ModalHelper(this.modalName);
        this.modalForm.initModal();
        this.modalForm.open();
    };
    FORM_InformesIncidentes.prototype.OnBeginActivate = function () {
        this.btnSubmit = $('#BtnSubmitActivate').ladda();
        this.btnSubmit.ladda('start');
    };
    FORM_InformesIncidentes.prototype.saveResult = function (data, status, xhr) {
        var result = data.split(';');
        if (result[0] == 'true') {
            window.location.href = "/GLOD/FormInformes/View/" + result[1];
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(result[1]);
        }
    };
    return FORM_InformesIncidentes;
}(GenericController));
