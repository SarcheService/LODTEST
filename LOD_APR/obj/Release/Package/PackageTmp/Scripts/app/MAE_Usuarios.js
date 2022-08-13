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
var MAE_Usuarios = (function (_super) {
    __extends(MAE_Usuarios, _super);
    function MAE_Usuarios() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalPassword";
        _this.formName = "#formPassword";
        _this.urlGetTable = "/Admin/Usuarios/getTableUsers";
        _this.modalForm = new ModalHelper(_this.modalName);
        var tableHelper = new TableHelper('#tableUsers', true, true, true);
        $("#IdSujEcon").select2({
            placeholder: 'Seleccione el sujeto economico',
            allowClear: true,
            theme: "bootstrap"
        });
        $("#IdSucursal").select2({
            theme: "bootstrap"
        });
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
        $('#IdSujEcon').val('').trigger('change');
        $("input[type=checkbox]").on('click', function (e) {
            var run = $("input#Run").val();
            var isChecked = $("#Activo").val();
            var IdUser = $("#Id").val();
            console.log(isChecked);
            _this.ExisteUsuarioActivo(run, IdUser, isChecked);
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
                    _this.getPartial("#divImagenPersonal", "/Admin/Usuarios/getImagenPreviewPersona/", data);
                    return;
                }
                else {
                    _this.alert.toastErrorData(response);
                }
            });
            _this.postFileToDB(data, "/Admin/Usuarios/AddImagenPreview/");
        });
        return _this;
    }
    ;
    MAE_Usuarios.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.btnSubmit = $('#btnSubmit').ladda();
        this.modalForm.initModal();
        this.modalForm.open();
    };
    MAE_Usuarios.prototype.submit = function () {
        this.btnSubmit.ladda('start');
    };
    MAE_Usuarios.prototype.clearImage = function (_image) {
        $("#" + _image).replaceWith('<h2 href="#" data-letters="Foto" class="data-letters-galena" style="vertical-align: middle;"></h2>');
        $("#RutaImagen").val('');
    };
    MAE_Usuarios.prototype.ExisteUsuarioActivo = function (RUN, IdUser, checked) {
        var _this = this;
        var numEvent = 1;
        this.Events.bind("OnPostCustomData", function (data) {
            if (data == 'true') {
                if (numEvent == 1) {
                    numEvent = numEvent + 1;
                    _this.ActivarUsuario(RUN, IdUser);
                }
            }
            else if (data == 'false') {
                _this.DesactivarUsuario(IdUser);
            }
            else {
                _this.alert.toastOk();
            }
        });
        this.postCustomData({ RUN: RUN, IdUser: IdUser, check: checked }, '/Admin/Usuarios/ExisteUsuarioActivo/');
    };
    MAE_Usuarios.prototype.ActivarUsuario = function (RUN, IdUser) {
        this.getPartialCustomEvent("#modalCanvas", "/Admin/Usuarios/ActivarUsuario/", { IdUser: IdUser }, "OnGetPartial");
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        this.modalForm.open();
    };
    MAE_Usuarios.prototype.DesactivarUsuario = function (IdUser) {
        var _this = this;
        var numEvent = 1;
        this.Events.bind("OnPostCustomData", function (data) {
            if (data == 'true') {
                if (numEvent == 1) {
                    _this.alert.toastOk();
                }
            }
            else {
                _this.alert.toastErrorData(data);
            }
        });
        this.postCustomData({ IdUser: IdUser }, '/Admin/Usuarios/DesUsuario/');
    };
    MAE_Usuarios.prototype.Filtro = function () {
        var filtro = {
            Texto: $("#searchInput").val(),
            Activo: $('#chkActivo:checkbox:checked').length > 0,
            Inactivo: $('#chkInactivo:checkbox:checked').length > 0,
            Mandante: $('#chkMandante:checkbox:checked').length > 0,
            Gubernamental: $('#chkGob:checkbox:checked').length > 0,
            Contratista: $('#chkContra:checkbox:checked').length > 0,
            IdSujetoEconomico: $("#IdSujEcon").val()
        };
        this.Events.bind("OnGetPartial", function () {
            var tableHelper = new TableHelper('#tableUsers', false);
        });
        this.getPartial("#divTableDatos", "/Admin/Usuarios/Filtro", filtro);
    };
    MAE_Usuarios.prototype.saveResult = function (data, status, xhr) {
        var btns = $(".ladda-button").ladda();
        btns.ladda('stop');
        if (data == 'true') {
            this.Events.bind("OnGetPartial", function () {
                var tableHelper = new TableHelper('#tableUsers', false);
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
    MAE_Usuarios.prototype.saveResult2 = function (data, status, xhr) {
        var btns = $(".ladda-button").ladda();
        btns.ladda('stop');
        if (data == 'true') {
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    MAE_Usuarios.prototype.clearImage = function (_image) {
        $("#" + _image).replaceWith('<h2 href="#" data-letters="Foto" class="data-letters-galena" style="vertical-align: middle;"></h2>');
        $(".ruta_imagen").val('');
    };
    return MAE_Usuarios;
}(GenericController));
