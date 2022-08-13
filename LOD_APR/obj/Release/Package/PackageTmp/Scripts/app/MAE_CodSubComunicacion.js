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
var MAE_CodSubComunicacion = (function (_super) {
    __extends(MAE_CodSubComunicacion, _super);
    function MAE_CodSubComunicacion() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalTipo";
        _this.formName = "#formTipo";
        _this.urlGetTable = "/Admin/CodSubCom/getTable";
        _this.modalForm = new ModalHelper(_this.modalName);
        return _this;
    }
    MAE_CodSubComunicacion.prototype.initModal = function (data, status, xhr) {
        var _this = this;
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });
        $("#IdTipo").select2({
            placeholder: 'Seleccione el Tipo de Documento',
            allowClear: true,
            theme: "bootstrap"
        });
        $("#IdTipoCom").on('change', function (e) {
            var id = $("#IdTipoCom").val();
            console.log("entró al change tipo com");
            _this.GetTipoSub(id);
        });
        $("#IdTipoSub").on('change', function (e) {
            var id = $("#IdTipoSub").val();
            console.log("entró al change tipo sub");
            _this.GetTipoDoc(id);
        });
        this.modalForm.open();
    };
    MAE_CodSubComunicacion.prototype.InitModalTipo = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.selectTipo = new SelectHelper("#Tipo", "/Admin/TipoDoc/getJsonTipos", "< Seleccione Tipo >", false, false, true);
        this.selectTipo.initSelectAjax();
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });
        this.modalForm.initModal();
        this.modalForm.open();
    };
    MAE_CodSubComunicacion.prototype.InitModalEdit = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });
        this.modalForm.initModal();
        this.modalForm.open();
    };
    MAE_CodSubComunicacion.prototype.saveResult = function (data, status, xhr) {
        if (data == 'true') {
            this.Events.bind("OnGetPartial", function () {
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
    MAE_CodSubComunicacion.prototype.GetTipoSub = function (IdCom) {
        this.Events.bind("OnGetPartial", function () {
            $(".panel-load").removeClass("sk-loading");
            $("#IdTipoSub").select2({
                allowClear: false,
                placeholder: '<Seleccione un Subtipo>',
                theme: "bootstrap"
            });
        });
        $(".panel-load").addClass("sk-loading");
        this.getPartial("#DivSub", "/Admin/CodSubCom/GetTipoSub", { "IdCom": IdCom });
    };
    MAE_CodSubComunicacion.prototype.GetTipoDoc = function (IdSub) {
        this.Events.bind("OnGetPartial", function () {
            $(".panel-load").removeClass("sk-loading");
            $("#IdTipo").select2({
                allowClear: false,
                placeholder: '<Seleccione un tipo>',
                theme: "bootstrap"
            });
        });
        $(".panel-load").addClass("sk-loading");
        this.getPartial("#DivDoc", "/Admin/CodSubCom/GetTipoDoc", { "IdSub": IdSub });
    };
    return MAE_CodSubComunicacion;
}(GenericController));
