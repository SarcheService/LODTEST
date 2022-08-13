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
var MAE_sujetos = (function (_super) {
    __extends(MAE_sujetos, _super);
    function MAE_sujetos() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalSujeto";
        _this.formName = "#formSujeto";
        _this.modalNameUser = "#modalUsuario";
        _this.modalFormUser = new ModalHelper(_this.modalNameUser);
        _this.urlGetTable = "/Admin/SujetoEconomico/getTable";
        _this.urlGetTableUser = "/Admin/SujetoEconomico/getTableUser";
        _this.modalForm = new ModalHelper(_this.modalName);
        var tableHelper = new TableHelper('#tablaDatos', true, false, true);
        if ($("input#Rut").length > 0) {
            $("input#Rut").rut({
                formatOn: 'blur',
                minimumLength: 8,
                validateOn: 'change'
            });
        }
        $("input[name=flexRadioDefault]").click(function () {
            console.log("#EsMandante", $("#EsGubernamental", $("#EsContratista")));
            if ($("#EsMandante1").is(':checked')) {
                $("#EsMandante").val(true);
                $("#EsGubernamental").val(false);
                $("#EsContratista").val(false);
            }
            if ($("#EsGubernamental2").is(':checked')) {
                $("#EsMandante").val(false);
                $("#EsGubernamental").val(true);
                $("#EsContratista").val(false);
            }
            if ($("#EsContratista3").is(':checked')) {
                $("#EsMandante").val(false);
                $("#EsGubernamental").val(false);
                $("#EsContratista").val(true);
            }
        });
        $('#fileImage').on('change', function () {
            $(".panel-load").addClass("sk-loading");
            var fileUpload = $("#fileImage").get(0);
            var files = fileUpload.files;
            var data = new FormData();
            if (files.length == 0) {
                $(".panel-load").removeClass("sk-loading");
                _this.alert.toastErrorData("Debe ingresar una Foto!");
                return;
            }
            data.append("fileImage", files[0]);
            _this.Events.bind("OnSuccessSaveFile", function (response) {
                var r = response.split(';');
                var data = { url: r[1] };
                if (r[0] == "true") {
                    _this.getPartial("#divImagenSujeto", "/Admin/SujetoEconomico/getImagenPreview/", data);
                    return;
                }
                else {
                    _this.alert.toastErrorData(response);
                }
            });
            _this.postFileToDB(data, "/Admin/SujetoEconomico/AddImagenPreview/");
        });
        return _this;
    }
    ;
    MAE_sujetos.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        $(".panel-load").removeClass("sk-loading");
        this.modalForm.initModal();
        this.modalForm.open();
    };
    MAE_sujetos.prototype.saveResult = function (data, status, xhr) {
        if (data == 'true') {
            this.Events.bind("OnGetPartial", function () {
                var tableHelper = new TableHelper('#tablaDatos', true, false, true);
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
    MAE_sujetos.prototype.saveResultUser = function (data, status, xhr) {
        if (data == 'true') {
            this.Events.bind("OnGetPartial", function () {
                var tableHelper = new TableHelper('#tablaDatosUser', true, false, true);
            });
            this.getPartial("#divTableDatosUser", this.urlGetTableUser, {});
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    MAE_sujetos.prototype.initModalUser = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalFormUser.initModal();
        this.modalFormUser.open();
    };
    MAE_sujetos.prototype.submit = function () {
        this.btnSubmit.ladda('start');
    };
    MAE_sujetos.prototype.clearImage = function (_image) {
        $("#" + _image).replaceWith('<h2 href="#" data-letters="Foto" class="data-letters-galena" style="vertical-align: middle;"></h2>');
        $("#RutaImagen").val('');
    };
    MAE_sujetos.prototype.Filtro = function () {
        var filtro = {
            Texto: $("#searchInput").val(),
            Activo: $('#chkActivo:checkbox:checked').length > 0,
            Inactivo: $('#chkInactivo:checkbox:checked').length > 0,
            Mandante: $('#chkMandante:checkbox:checked').length > 0,
            Gubernamental: $('#chkGob:checkbox:checked').length > 0,
            Contratista: $('#chkContra:checkbox:checked').length > 0,
        };
        this.Events.bind("OnGetPartial", function () {
            var table = $('#tablaDatos').DataTable();
            table.destroy();
            var tableHelper = new TableHelper('#tablaDatos', true, false, true);
        });
        _super.prototype.getPartial.call(this, "#divTableDatos", "/Admin/SujetoEconomico/Filtro", filtro);
        $("#divFicha").load("/Admin/SujetoEconomico/DetailsEmpty");
    };
    return MAE_sujetos;
}(GenericController));
