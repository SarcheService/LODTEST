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
var FilesHelperV2 = (function (_super) {
    __extends(FilesHelperV2, _super);
    function FilesHelperV2() {
        var _this = _super.call(this) || this;
        _this.modalCanvas = "#modalCanvas";
        _this.modalFormName = "#modalFormDocs";
        _this.filesFormName = "#formDocs";
        _this.inputFileName = "#fileName";
        _this.tableDivName = "#lstDocs_";
        _this.tableName = "#tablaDocs_";
        _this.urlAddModel = "/Admin/Documentos/AddFile";
        _this.listFiles = [];
        return _this;
    }
    FilesHelperV2.prototype.AddFile = function () {
        this.modalFormAdd = new ModalHelper(this.modalFormName);
        this.modalFormAdd.initModal();
        $("#IdTipo").select2({
            allowClear: false,
            placeholder: 'Seleccione un Tipo',
            theme: "bootstrap"
        });
        this.modalFormAdd.open();
        this.InitDocAjaxForm();
    };
    FilesHelperV2.prototype.InitDocAjaxForm = function () {
        var _this = this;
        $(this.filesFormName).ajaxForm({
            beforeSend: function () {
                _this.OnBegin();
            },
            uploadProgress: function (event, position, total, percentComplete) {
                $("#pbDocumentHelperV2").css("width", percentComplete + "%");
                $("#pbDocumentHelperV2").text(percentComplete + "%");
            },
            success: function (data, status, xhr) {
                _this.SaveResult(data, status, xhr);
            },
            complete: function () {
                $(".panel-load").removeClass("sk-loading");
                var btns = $("#btnSubmitDocHelperV2").ladda();
                btns.ladda('stop');
                $("#pbDocumentHelperV2").css("width", "0%");
                $("#pbDocumentHelperV2").text("0%");
            }
        });
    };
    FilesHelperV2.prototype.EditDeleteFile = function () {
        this.modalFormAdd = new ModalHelper(this.modalFormName);
        $.validator.unobtrusive.parse($(this.filesFormName));
        this.modalFormAdd.initModal();
        $("#IdTipo").select2({
            allowClear: false,
            placeholder: 'Seleccione un Tipo',
            theme: "bootstrap"
        });
        this.modalFormAdd.open();
        $(".panel-load").removeClass("sk-loading");
    };
    FilesHelperV2.prototype.SaveResult = function (data, status, xhr) {
        $(".panel-load").removeClass("sk-loading");
        var btns = $("#btnSubmitDocHelperV2").ladda();
        btns.ladda('start');
        var r = data.split(";");
        if (r[0] == 'true') {
            this.getPartial("#lstDocs_" + r[4], "/Admin/Documentos/GetFilesV2", { primarykey: r[1], origen: r[2], path: r[3], randomID: r[4], vista: r[5] });
            this.alert.toastOk();
            this.modalFormAdd.close();
            return;
        }
        else {
            this.alert.toastErrorData(r[1]);
        }
    };
    FilesHelperV2.prototype.OnBegin = function () {
        var btns = $("#btnSubmitDocHelperV2").ladda();
        btns.ladda('start');
        $('.panel-load').addClass('sk-loading');
    };
    return FilesHelperV2;
}(GenericController));
