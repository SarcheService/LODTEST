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
var MAE_archivos = (function (_super) {
    __extends(MAE_archivos, _super);
    function MAE_archivos(_ControllerName, _MaxFileSize) {
        var _this = _super.call(this) || this;
        _this.modalCanvas = "#modalCanvas";
        _this.modalFormName = "#modalFormDocs";
        _this.filesFormName = "#formDocs";
        _this.inputFileName = "#fileName";
        _this.tableDivName = "#lstDocs_" + _ControllerName;
        _this.tableName = "#tablaDocs_" + _ControllerName;
        _this.urlAddModel = "/Admin/Documentos/AddFile";
        _this.ControllerName = _ControllerName;
        _this.MaxFileSize = _MaxFileSize;
        _this.listFiles = [];
        _this.selectTipoDoc = new SelectHelper("#IdTipoDocumento", "/ASP/TipoDocumento/getTipoDocJson/", "< Seleccione tipo >", true, false, true);
        return _this;
    }
    MAE_archivos.prototype.getListDocs = function (_id, _path, _tipoEp) {
        var _this = this;
        if (_tipoEp === void 0) { _tipoEp = null; }
        this.PrimaryKey = _id;
        this.IdPath = _path;
        this.tipoEp = _tipoEp;
        this.Events.bind("OnGetPartial", function () {
            $(".panel-load").removeClass("sk-loading");
            _this.DOMcheckboxes();
        });
        $(".panel-load").addClass("sk-loading");
        if (this.tipoEp == null) {
            _super.prototype.getPartial.call(this, this.tableDivName, '/Admin/Documentos/getFiles_' + this.ControllerName, { id: _id, path: _path });
        }
        else {
            _super.prototype.getPartial.call(this, this.tableDivName, '/Admin/Documentos/getFiles_' + this.ControllerName, { id: _id, tipoEp: this.tipoEp });
        }
    };
    MAE_archivos.prototype.addFile = function (_id, _path) {
        this.PrimaryKey = _id;
        this.IdPath = _path;
        this.modalFormAdd = new ModalHelper(this.modalFormName);
        this.modalFormAdd.initModal();
        this.modalFormAdd.open();
        this.btnSubmit = $('#btnSubmit').ladda();
        this.initForm();
    };
    MAE_archivos.prototype.addFileSinTipo = function (_id) {
        this.PrimaryKey = _id;
        this.modalFormAdd = new ModalHelper(this.modalFormName);
        this.modalFormAdd.initModal();
        this.modalFormAdd.open();
        this.btnSubmit = $('#btnSubmit').ladda();
        this.initForm();
    };
    MAE_archivos.prototype.addFileDocEP = function (_idCaratula, _idTipoEP) {
        this.PrimaryKey = _idCaratula;
        this.tipoEp = _idTipoEP;
        this.modalFormAdd = new ModalHelper(this.modalFormName);
        this.modalFormAdd.initModal();
        this.modalFormAdd.open();
        this.btnSubmit2 = $('#btnSubmitDelete').ladda();
        this.btnSubmit = $('#btnSubmit').ladda();
        this.initForm();
    };
    MAE_archivos.prototype.addFileAnot = function (_id, _tipo) {
        this.PrimaryKey = _id;
        this.modalFormAdd = new ModalHelper(this.modalFormName);
        if (_tipo == "Lod") {
            files_anotacionLod.selectTipoDoc.initSelect();
            files_anotacionLod.selectTipoDoc.clearSelection();
        }
        else if (_tipo = "Bit") {
            files_anotacionBit.selectTipoDoc.initSelect();
            files_anotacionBit.selectTipoDoc.clearSelection();
        }
        this.modalFormAdd.initModal();
        this.modalFormAdd.open();
        this.btnSubmit = $('#btnSubmit').ladda();
        this.initForm();
    };
    MAE_archivos.prototype.InitForm = function (_id, _path) {
        this.PrimaryKey = _id;
        this.IdPath = _path;
        this.modalFormAdd = new ModalHelper(this.modalFormName);
        $.validator.unobtrusive.parse($(this.filesFormName));
        this.modalFormAdd.initModal();
        this.modalFormAdd.open();
        $(".panel-load").removeClass("sk-loading");
    };
    MAE_archivos.prototype.initForm = function () {
        var _this = this;
        $(this.filesFormName).ajaxForm({
            beforeSend: function () {
                _this.onBegin();
            },
            uploadProgress: function (event, position, total, percentComplete) {
                $("#progressBar").css("width", percentComplete + "%");
                $("#progressBar").text(percentComplete + "%");
            },
            success: function (data, status, xhr) {
                _this.saveResult(data, status, xhr);
            },
            complete: function () {
                $(".panel-load").removeClass("sk-loading");
                var btns = $(".ladda-button").ladda();
                btns.ladda('stop');
                $("#progressBar").css("width", "0%");
                $("#progressBar").text("0%");
            }
        });
    };
    MAE_archivos.prototype.saveResult = function (data, status, xhr) {
        if (data == 'true') {
            this.getListDocs(this.PrimaryKey, this.IdPath, this.tipoEp);
            this.alert.toastOk();
            this.modalFormAdd.close();
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    MAE_archivos.prototype.onBegin = function () {
        this.btnSubmit.ladda('start');
        $('.panel-load').addClass('sk-loading');
    };
    MAE_archivos.prototype.descargarZip = function () {
        var _this = this;
        var lstIds = this.selectedDocs();
        if (lstIds == null || lstIds.length == 0) {
            this.alert.toastWarningData("Seleccione almenos 1 documento");
            return;
        }
        this.Events.bind("OnPostCustomData", function (result) {
            var resultados = result.split(";");
            if (resultados[0] == "true") {
                window.location.href = resultados[1];
                _this.alert.toastOkData("Descarga realizada correctamente!");
            }
            else {
                _this.alert.toastErrorData(resultados[1]);
            }
        });
        _super.prototype.postCustomData.call(this, { lstDocs: lstIds }, '/Admin/Documentos/zipDocs_' + this.ControllerName);
    };
    MAE_archivos.prototype.selectedDocs = function () {
        var docs = [];
        $(this.tableName + ' input[type=checkbox]').each(function (index, item) {
            var elementoCheck = item;
            if ($(elementoCheck).prop("checked") == true && $(elementoCheck).prop("id") != "chkTodos") {
                docs.push($(elementoCheck).val());
            }
        });
        return docs;
    };
    MAE_archivos.prototype.selectCheckBoxes = function (isChecked) {
        $(this.tableName + ' input[type=checkbox]').each(function (index, item) {
            var elementoCheck = item;
            if ($(elementoCheck).prop("disabled") == false) {
                if (isChecked) {
                    $(elementoCheck).prop("checked", true);
                }
                else {
                    $(elementoCheck).prop("checked", false);
                }
            }
        });
    };
    ;
    MAE_archivos.prototype.DOMcheckboxes = function () {
        var _this = this;
        $(this.tableName).on('change', 'input[type=checkbox]', function (e) {
            var $element = e.target;
            if ($element.id == 'chkTodos') {
                _this.selectCheckBoxes($($element).prop("checked"));
            }
        });
    };
    MAE_archivos.prototype.descargarZipAnot = function (IdAnot) {
        var _this = this;
        this.Events.bind("OnPostCustomData", function (result) {
            var resultados = result.split(";");
            if (resultados[0] == "true") {
                window.location.href = resultados[1];
                _this.alert.toastOkData("Descarga realizada correctamente!");
            }
            else {
                _this.alert.toastErrorData(resultados[1]);
            }
        });
        _super.prototype.postCustomData.call(this, { IdAnot: IdAnot }, '/Admin/Documentos/zipDocs_' + this.ControllerName);
    };
    return MAE_archivos;
}(GenericController));
