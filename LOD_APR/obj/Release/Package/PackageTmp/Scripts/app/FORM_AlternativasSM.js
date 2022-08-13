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
var FORM_AlternativasSM = (function (_super) {
    __extends(FORM_AlternativasSM, _super);
    function FORM_AlternativasSM() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalAlternativa";
        _this.formName = "#formAlternativa";
        _this.modalForm = new ModalHelper(_this.modalName);
        return _this;
    }
    FORM_AlternativasSM.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        this.modalForm.open();
    };
    FORM_AlternativasSM.prototype.saveResult = function (data, status, xhr) {
        var data1 = data.split(";");
        if (data1[0] == 'true') {
            var params = new FORM_ParametrosFormulario();
            params.UpdatePanelParams(data1[1]);
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    return FORM_AlternativasSM;
}(GenericController));
