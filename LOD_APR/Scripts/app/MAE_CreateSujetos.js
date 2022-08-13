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
var MAE_CreateSujetos = (function (_super) {
    __extends(MAE_CreateSujetos, _super);
    function MAE_CreateSujetos() {
        var _this = _super.call(this) || this;
        $("#IdCiudad").select2({
            theme: "bootstrap"
        });
        $("#RazonSocial").keyup(function () {
            var value = $(this).val();
            $("#lblRazon").text(value);
        }).keyup();
        $("#Rut").keyup(function () {
            var value = $(this).val();
            $("#lblRut").text(value);
        }).keyup();
        $("input#Run").rut({
            formatOn: 'blur',
            minimumLength: 7,
            validateOn: 'change'
        });
        $("input#Rut").rut({ validateOn: 'change' });
        $("input#Rut").rut().on('rutInvalido', function (e) {
            var spanRut = $("span[data-valmsg-for='Rut']");
            spanRut.html("<span id='Rut-error' class=''>El RUT " + $(this).val() + " es inválido</span>");
            $("input#Rut").val("");
        });
        $("input#Rut").rut().on('rutValido', function (e) {
            var RUT = $("input#Rut").val();
            var spanRut = $("span[data-valmsg-for='Rut']");
            _this.Events.bind("OnPostCustomData", function (response) {
                if (response != "true") {
                    spanRut.html("<span id='Rut-error' class=''></span>");
                    return;
                }
                else {
                    spanRut.html("<span id='Rut-error' class=''>EL RUT ya se encuentra ingresado en nuestros registros</span>");
                    $("input#Rut").val("");
                    $("input#Rut").focus();
                    return;
                }
            });
            _this.postCustomData({ RUT: RUT }, "/ADMIN/SujetoEconomico/ExisteRut/");
        });
        $("input#Rut").focus();
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
        return _this;
    }
    return MAE_CreateSujetos;
}(GenericController));
