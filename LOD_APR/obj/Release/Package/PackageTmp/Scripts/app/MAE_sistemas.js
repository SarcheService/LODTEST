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
var MAE_sistemas = (function (_super) {
    __extends(MAE_sistemas, _super);
    function MAE_sistemas(_Area, _Controller) {
        var _this = _super.call(this) || this;
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalSistema";
        _this.formName = "#formDatos";
        _this.urlGetTable = "/" + _Area + "/" + _Controller + "/getTable";
        var tablaDatos = new TableHelper('#tablaDatos', false);
        _this.modalForm = new ModalHelper(_this.modalName);
        return _this;
    }
    MAE_sistemas.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        $(document).on("click", "[type='checkbox']", function (e) {
            if (this.id == "Activo_1" && this.checked) {
                $("#Activo").attr("value", "True");
            }
            else if (this.id == "Activo_1" && !this.checked) {
                $("#Activo").attr("value", "False");
            }
        });
        if ($("#action").val() == "Create") {
            $(".modal").find("select").val(null);
        }
        $("#SR1").select2({
            allowClear: true,
            placeholder: 'Seleccione un sistema',
            theme: "bootstrap"
        });
        $("#SR2").select2({
            allowClear: true,
            placeholder: 'Seleccione un sistema',
            theme: "bootstrap"
        });
        $("#SR3").select2({
            allowClear: true,
            placeholder: 'Seleccione un sistema',
            theme: "bootstrap"
        });
        var before;
        $('select').on('select2:selecting', function (evt) {
            var Id = $(this).attr("id");
            before = $("#" + Id).val();
        });
        $('select').on('select2:unselecting', function (e) {
            var Id = $(this).attr("id");
            before = $("#" + Id).val();
        });
        $("select").change(function () {
            if ($("#" + $(this).attr("id") + " option:selected").val() != undefined) {
                var SistemaSeleccionado = $("#" + $(this).attr("id") + " option:selected").val();
                var SelectException = $(this).attr("id");
                var Selects = $(".modal").find("select[name!='" + SelectException + "']");
                for (var index = 0; index < Selects.length; index++) {
                    var element = Selects[index];
                    var Option = $(element).find("option[value=" + SistemaSeleccionado + "]");
                    $(Option).prop("disabled", true);
                    $(Selects[index]).select2({
                        allowClear: true,
                        placeholder: 'Seleccione un sistema',
                        theme: "bootstrap"
                    });
                }
                if (before != undefined && before != 0) {
                    var SelectException = $(this).attr("id");
                    var Selects = $(".modal").find("select[name!='" + SelectException + "']");
                    for (var index = 0; index < Selects.length; index++) {
                        var element = Selects[index];
                        var Option = $(element).find("option[value=" + before + "]");
                        $(Option).prop("disabled", false);
                        $(Selects[index]).select2({
                            allowClear: true,
                            placeholder: 'Seleccione un sistema',
                            theme: "bootstrap"
                        });
                    }
                    before = 0;
                }
            }
            else {
                var SelectException = $(this).attr("id");
                var Selects = $(".modal").find("select[name!='" + SelectException + "']");
                for (var index = 0; index < Selects.length; index++) {
                    var element = Selects[index];
                    var Option = $(element).find("option[value=" + before + "]");
                    $(Option).prop("disabled", false);
                    $(Selects[index]).select2({
                        allowClear: true,
                        placeholder: 'Seleccione un sistema',
                        theme: "bootstrap"
                    });
                }
                before = 0;
            }
        });
        this.modalForm.open();
    };
    MAE_sistemas.prototype.saveResult = function (data, status, xhr) {
        if (data == 'true') {
            this.alert.toastOk();
            this.modalForm.close();
            this.getTable();
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    MAE_sistemas.prototype.getTable = function () {
        this.getPartial("#divTableDatos", this.urlGetTable, {});
        var tablaDatos = new TableHelper('#tablaDatos', false);
    };
    return MAE_sistemas;
}(GenericController));
