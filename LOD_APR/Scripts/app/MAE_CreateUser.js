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
var MAE_CreateUser = (function (_super) {
    __extends(MAE_CreateUser, _super);
    function MAE_CreateUser() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalUser";
        _this.formName = "#formUser";
        _this.urlGetTable = "";
        _this.modalForm = new ModalHelper(_this.modalName);
        $("#IdSucursal").select2({
            theme: "bootstrap"
        });
        $("input#Run").rut({
            formatOn: 'blur',
            minimumLength: 7,
            validateOn: 'change'
        });
        $("input#Run").rut({ validateOn: 'change' });
        $("input#Run").rut().on('rutInvalido', function (e) {
            var spanRun = $("span[data-valmsg-for='Run']");
            spanRun.html("<span id='Run-error' class=''>El RUN " + $(this).val() + " es inválido</span>");
            $("input#Run").val("");
        });
        $("input#Run").rut().on('rutValido', function (e) {
            var RUN = $("input#Run").val();
            var Sujeto = $("input#IdSujeto").val();
            var spanRun = $("span[data-valmsg-for='Run']");
            var numEvent = 1;
            _this.Events.bind("OnPostCustomData", function (response) {
                if (response != "true") {
                    spanRun.html("<span id='Run-error' class=''></span>");
                    return;
                }
                else {
                    spanRun.html("<span id='Run-error' class=''>Existe un Usuario <b>Activo</b> registrado con este RUN</span>");
                    $("input#Run").val("");
                    if (numEvent == 1) {
                        numEvent = numEvent + 1;
                        _this.initModal(RUN);
                    }
                    return;
                }
            });
            _this.postCustomData({ RUN: RUN, IdSujeto: Sujeto }, "/Admin/Usuarios/ExisteRut/");
        });
        $("input#Email").on('change', function (e) {
            var Correo = $("input#Email").val();
            var spanCorreo = $("span[data-valmsg-for='Email']");
            _this.Events.bind("OnPostCustomData", function (response) {
                if (response != "true") {
                    spanCorreo.html("<span id='Email-error' class=''></span>");
                    return;
                }
                else {
                    spanCorreo.html("<span id='Email-error' class=''>El Email ya se encuentra ingresado en el sistema</span>");
                    $("input#Email").val("");
                    return;
                }
            });
            _this.postCustomData({ Correo: Correo }, "/Admin/Usuarios/ExisteCorreo/");
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
                    _this.getPartial("#divImagenPersonal", "/Admin/SujetoEconomico/getImagenPreviewPersona/", data);
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
    MAE_CreateUser.prototype.initModal = function (RUN) {
        this.getPartialCustomEvent("#modalCanvas", "/Admin/Usuarios/DesactivarUsuario/", { RUN: RUN }, "OnGetPartial");
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        this.modalForm.open();
    };
    MAE_CreateUser.prototype.clearImage = function (_image) {
        $("#" + _image).replaceWith('<h2 href="#" data-letters="Foto" class="data-letters-galena" style="vertical-align: middle;"></h2>');
        $(".ruta_imagen").val('');
    };
    MAE_CreateUser.prototype.saveResult = function (data, status, xhr) {
        if (data == 'true') {
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    return MAE_CreateUser;
}(GenericController));
