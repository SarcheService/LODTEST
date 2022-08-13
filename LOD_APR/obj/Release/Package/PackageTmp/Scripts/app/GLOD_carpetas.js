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
var GLOD_carpetas = (function (_super) {
    __extends(GLOD_carpetas, _super);
    function GLOD_carpetas() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalCarpetas";
        _this.formName = "#formCarpetas";
        _this.urlGetTree = "/GLOD/Home/getTree";
        _this.modalForm = new ModalHelper(_this.modalName);
        return _this;
    }
    GLOD_carpetas.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        this.modalForm.open();
    };
    GLOD_carpetas.prototype.saveResult = function (data, status, xhr) {
        var data1 = data.split(";");
        if (data1[0] == 'true') {
            glod_administracion.treeView.Events.bind("OnGetTreeData", function () {
                setTimeout(function () { $('#treeView').jstree(true).select_node("f_" + data1[1]); }, 1000);
            });
            glod_administracion.treeView.updateTreeData(this.urlGetTree);
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else if (data1[0] == 'delete') {
            glod_administracion.treeView.Events.bind("OnGetTreeData", function () {
                setTimeout(function () { $('#treeView').jstree(true).select_node(data1[1]); }, 1000);
            });
            glod_administracion.treeView.updateTreeData(this.urlGetTree);
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    return GLOD_carpetas;
}(GenericController));
