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
var FORM_Formulario = (function (_super) {
    __extends(FORM_Formulario, _super);
    function FORM_Formulario() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalFormEval";
        _this.formName = "#formFormEval";
        _this.urlGetTable = "/Admin/Formularios/GetTable";
        _this.modalForm = new ModalHelper(_this.modalName);
        var tableHelper = new TableHelper('#tablaFormularios', true);
        return _this;
    }
    FORM_Formulario.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        this.modalForm.open();
    };
    FORM_Formulario.prototype.OnBeginActivate = function () {
        this.btnSubmit = $('#BtnSubmitActivate').ladda();
        this.btnSubmit.ladda('start');
    };
    FORM_Formulario.prototype.saveResult = function (data, status, xhr) {
        if (data == 'true') {
            this.Events.bind("OnGetPartial", function () {
                var tableHelper = new TableHelper('#tablaFormularios', true);
            });
            this.getPartial("#DivTablaDatos", this.urlGetTable, {});
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    FORM_Formulario.prototype.saveResultEmbebido = function (data, status, xhr) {
        if (data == 'true') {
            this.Events.bind("OnGetPartial", function () {
                var tableHelper = new TableHelper('#tablaFormularios', true);
            });
            this.getPartial("#DivTablaDatos", "/Admin/FormsEmbebidos/GetTable", {});
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    FORM_Formulario.prototype.SaveResultActivate = function (data, status, xhr) {
        var btns = $(".ladda-button").ladda();
        btns.ladda('stop');
        var data1 = data.split(";");
        if (data1[0] == 'true') {
            window.location.href = "/Admin/Formularios/Edit/" + data1[1];
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    return FORM_Formulario;
}(GenericController));
