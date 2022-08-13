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
var MAE_ClassDoc = (function (_super) {
    __extends(MAE_ClassDoc, _super);
    function MAE_ClassDoc() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalTipo";
        _this.formName = "#formTipo";
        _this.urlGetTable = "/Admin/ClassDoc/getTable";
        _this.modalForm = new ModalHelper(_this.modalName);
        return _this;
    }
    MAE_ClassDoc.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        $("#IdClassTwo").select2({
            placeholder: 'Seleccione el tipo de Subclasificación',
            allowClear: true,
            theme: "bootstrap"
        });
        $("#IdTipo").select2({
            placeholder: 'Seleccione el tipo de documento',
            allowClear: true,
            theme: "bootstrap"
        });
        $("#IdTipoSub").select2({
            placeholder: 'Seleccione el tipo de Subtipo de Comunicación',
            allowClear: true,
            theme: "bootstrap"
        });
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });
        this.modalForm.open();
    };
    MAE_ClassDoc.prototype.InitModalTipo = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        $("#IdClassTwo").select2({
            placeholder: 'Seleccione el tipo de Subclasificación',
            allowClear: true,
            theme: "bootstrap"
        });
        $("#IdTipo").select2({
            placeholder: 'Seleccione el tipo de documento',
            allowClear: true,
            theme: "bootstrap"
        });
        $("#IdTipoSub").select2({
            placeholder: 'Seleccione el tipo de Subtipo de Comunicación',
            allowClear: true,
            theme: "bootstrap"
        });
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });
        this.modalForm.initModal();
        this.modalForm.open();
    };
    MAE_ClassDoc.prototype.InitModalEdit = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });
        this.modalForm.initModal();
        this.modalForm.open();
    };
    MAE_ClassDoc.prototype.saveResult = function (data, status, xhr) {
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
    return MAE_ClassDoc;
}(GenericController));
