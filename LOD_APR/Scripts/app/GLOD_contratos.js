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
var GLOD_contratos = (function (_super) {
    __extends(GLOD_contratos, _super);
    function GLOD_contratos() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalCanvas2 = "#modalCanvas2";
        _this.modalName = "#modalContratos";
        _this.modalName2 = "#modalPermisos";
        _this.formName = "#formContratos";
        _this.formName2 = "#formPermisos";
        _this.urlGetTree = "/GLOD/Home/getTree";
        _this.modalForm = new ModalHelper(_this.modalName);
        _this.modalForm2 = new ModalHelper(_this.modalName2);
        _this.selectSujetoEconomico = new SelectHelper("#IdSujEcon", "/Admin/SujetoEconomico/getSujetoEconomicoJson/", "< Seleccione Empresa >", true, false, true);
        _this.selectAdmContratoCont = new SelectHelper("#IdAdminContrato", "", "< Seleccione Adm. De Contrato >", true, false, true);
        return _this;
    }
    GLOD_contratos.prototype.initModal = function (data, status, xhr) {
        var _this = this;
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        $(this.formName).ajaxForm({
            beforeSend: function () {
                $('.panel-load').addClass('sk-loading');
            },
            uploadProgress: function (event, position, total, percentComplete) {
            },
            success: function (data) {
                _this.saveResult(data, null, null);
            },
            complete: function () {
                $(".panel-load").removeClass("sk-loading");
            }
        });
        $("#MontoInicialstr").mask("#.##0", { reverse: true });
        $('#IdDireccionContrato').select2({
            allowClear: true,
            placeholder: '<Seleccione una Dirección>',
            theme: "bootstrap"
        });
        $('#IdEmpresaFiscalizadora').select2({
            allowClear: true,
            placeholder: '<Seleccione Empresa Fiscalizadora>',
            theme: "bootstrap"
        });
        $('#IdEmpresaContratista').select2({
            allowClear: true,
            placeholder: '<Seleccione Empresa Contratista>',
            theme: "bootstrap"
        });
        $('#FechaInicioContrato').datepicker({
            language: 'es',
            format: "dd-mm-yyyy",
            today: true,
            autocomplete: 'off',
            autoclose: true
        });
        if ($("#IdSujEcon").val() != null) {
            $('#IdAdminContrato').select2({
                ajax: {
                    url: "/Admin/Contactos/getContactosASP2/",
                    dataType: "json",
                    type: "GET",
                    data: { IdContratista: $("#IdSujEcon").val() },
                    processResults: function (data) {
                        return {
                            results: $.map(data, function (item) {
                                return {
                                    text: item.name,
                                    id: item.id
                                };
                            })
                        };
                    }
                },
                placeholder: "< Seleccione Adm. De Contrato >"
            });
        }
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
                    _this.getPartial("#divImagenContrato", "/Admin/SujetoEconomico/getImagenPreview/", data);
                    return;
                }
                else {
                    _this.alert.toastErrorData(response);
                }
            });
            _this.postFileToDB(data, "/Admin/SujetoEconomico/AddImagenPreview/");
        });
        $("#guardar").click(function () {
            var IdResponsable = $("#IdRespMandante").val();
            if (IdResponsable == 0) {
                return;
            }
            $(_this.formName).submit();
        });
        $("#modalContratos #IdModeloCtto").change(function () {
            var valor = $("#IdModeloCtto").val();
            if (valor == 2) {
                $("#modalContratos #CostoDirecto").val($('#MontoPresupContrato').val());
                _this.PorcGG();
                _this.PorcUtil();
                _this.PorcAnticipo();
                _this.CalculoMontoContrato();
            }
            else if (valor == 1) {
                $("#modalContratos #CostoDirecto").val(0);
                _this.PorcGG();
                _this.PorcUtil();
                _this.PorcAnticipo();
                _this.CalculoMontoContrato();
            }
        });
        $("#modalContratos #MontoPresupContrato").change(function () {
            var valor = $("#IdModeloCtto").val();
            if (valor == 2) {
                $("#modalContratos #CostoDirecto").val($('#MontoPresupContrato').val());
                _this.PorcGG();
                _this.PorcUtil();
                _this.PorcAnticipo();
                _this.CalculoMontoContrato();
            }
        });
        var Min = $("#RefEval").attr("data-min");
        var Max = $("#RefEval").attr("data-max");
        console.log(Min, Max);
        $(".ED").change(function () {
            console.log(this);
            var obj = this.id;
            var valor = $('#' + obj).val();
            console.log(valor);
            Max2 = Max + ".0";
            if (valor < Min || valor > Max2) {
                $('#' + obj).val("");
                $('#' + obj).focus();
                glod_contratos.alert.toastErrorData("La nota debe ser entre " + Min + " y " + Max);
            }
        });
        $("#IdSujEcon").change(function () {
            $('#IdAdminContrato').select2("destroy");
            $('#IdAdminContrato').removeAttr("disabled");
            _this.selectAdmContratoCont.initSelect();
            $("#IdAdminContrato").select2('val', { id: null, text: null });
            $("#IdAdminContrato").select2({ placeholder: "< Seleccione Adm. De Contrato >" }).trigger('change');
            $('#IdAdminContrato').select2({
                ajax: {
                    url: "/Admin/Contactos/getContactosASP2/",
                    dataType: "json",
                    type: "GET",
                    data: { IdContratista: $("#IdSujEcon").val() },
                    processResults: function (data) {
                        return {
                            results: $.map(data, function (item) {
                                return {
                                    text: item.name,
                                    id: item.id
                                };
                            })
                        };
                    }
                },
                placeholder: "< Seleccione Adm. De Contrato >"
            });
        });
        this.modalForm.open();
    };
    GLOD_contratos.prototype.saveResult = function (data, status, xhr) {
        var data1 = data.split(";");
        if (data1[0] == 'delete') {
            glod_administracion.treeView.Events.bind("OnGetTreeData", function () {
                setTimeout(function () { $('#treeView').jstree(true).select_node("f_" + data1[1]); }, 1000);
            });
            glod_administracion.treeView.updateTreeData(this.urlGetTree);
            this.alert.toastOk();
            this.modalForm.close();
            $(".ladda-button").ladda('stop');
            return;
        }
        else if (data1[0] == 'true') {
            glod_administracion.treeView.Events.bind("OnGetTreeData", function () {
                setTimeout(function () { $('#treeView').jstree(true).select_node("c_" + data1[1]); }, 1000);
            });
            glod_administracion.treeView.updateTreeData(this.urlGetTree);
            this.alert.toastOk();
            this.modalForm.close();
            $(".ladda-button").ladda('stop');
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    GLOD_contratos.prototype.Reconvertir = function (valor) {
        var resultado = numeral(valor._value).format('0,0.00');
        resultado = resultado.replace(".", "d");
        resultado = resultado.replace(/\,/g, ".");
        resultado = resultado.replace("d", ",");
        return (resultado);
    };
    GLOD_contratos.prototype.ReconvertirPorcentajes = function (valor) {
        var resultado = numeral(valor._value).format('0,0.000000');
        resultado = resultado.replace(".", "d");
        resultado = resultado.replace(/\,/g, ".");
        resultado = resultado.replace("d", ",");
        return (resultado);
    };
    GLOD_contratos.prototype.processDate = function (date) {
        var aux = date.split("-");
        var fecha = new Date();
        fecha = new Date(parseInt(aux[2]), parseInt(aux[1]) - 1, parseInt(aux[0]));
        return fecha;
    };
    GLOD_contratos.prototype.unmaskDinero = function (dinero) {
        return Number(dinero.replace(/\./g, ""));
    };
    GLOD_contratos.prototype.LoadLadda = function () {
        var btns = $(".ladda-button").ladda();
        btns.ladda('start');
        this.btnSubmit = $('#btnSubmit').ladda();
        this.btnSubmit.submit();
        $("#btnCancel").prop("disabled", true);
    };
    GLOD_contratos.prototype.Logs = function (IdContrato) {
        this.getPartial("#divTableLogs", "/GLOD/Contratos/Logs", { id: IdContrato });
        return;
    };
    GLOD_contratos.prototype.initModalPermisos = function (data, status, xhr) {
        var _this = this;
        $.validator.unobtrusive.parse($("#formAnticipo"));
        this.modalForm.initModal();
        $("#formAnticipo").ajaxForm({
            beforeSend: function () {
                $('.panel-load').addClass('sk-loading');
            },
            success: function (data) {
                _this.saveResultAsinarARol(data, null, null);
            },
            complete: function () {
                $(".panel-load").removeClass("sk-loading");
            }
        });
        $('#UserId').select2({
            allowClear: true,
            placeholder: '<Seleccione Usuario>',
            theme: "bootstrap"
        });
        $('#FechaEP').datepicker({
            language: 'es',
            format: "dd-mm-yyyy",
            today: true,
            todayHighlight: true,
            autoclose: true
        });
        $("#guardarAnticipo").click(function () {
            $(_this.formName).submit();
        });
        this.btnSubmitAprobarDocEp = $('#btnSubmit').ladda();
        this.modalForm.open();
    };
    GLOD_contratos.prototype.saveResultAsinarARol = function (data, status, xhr) {
        var data1 = data.split(";");
        if (data1[0] == "true") {
            this.Events.bind("OnGetPartial", function () {
                var tableHelper = new TableHelper('#tablaEPs', true);
            });
            this.getPartial("#divTableRoles", "/GLOD/Contratos/getTableRoles", { id: data1[1] });
            this.alert.toastOk();
            this.modalForm.close();
            $(".ladda-button").ladda('stop');
            return;
        }
        else {
            $('#btnSubmit').stop();
            this.alert.toastErrorData(data);
            $(".ladda-button").ladda('stop');
        }
    };
    GLOD_contratos.prototype.initModalActivacion = function (data, status, xhr) {
        $.validator.unobtrusive.parse($("#formAnticipo"));
        this.modalForm.initModal();
        this.modalForm.open();
    };
    GLOD_contratos.prototype.initActivacionFEA = function (id) {
        var _this = this;
        var startTime = new Date().getTime();
        var interval = setInterval(function () {
            if (new Date().getTime() - startTime > 120000) {
                clearInterval(interval);
                return;
            }
            console.log("Nuevo Intervalo..");
            axios.get('/GLOD/LibroObras/GetActivateState/' + id).then(function (response) {
                var val = response.data;
                if (val.Status) {
                    _this.getPartial("#divInfoLibros", "/GLOD/LibroObras/getLibro", { id: val.Parametros });
                    _this.alert.toastOk();
                    _this.modalForm.close();
                    clearInterval(interval);
                    return;
                }
            })
                .catch(function (error) {
                console.error(error);
                this.ProgressBarValue = 0;
                this.IsSending = false;
            });
        }, 5000);
    };
    GLOD_contratos.prototype.saveResultActivacion = function (data, status, xhr) {
        var data1 = data.split(";");
        if (data1[0] == "true") {
            this.getPartial("#divInfoLibros", "/GLOD/LibroObras/getLibro", { id: data1[1] });
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            $('#btnSubmit').stop();
            this.alert.toastErrorData(data);
            $(".ladda-button").ladda('stop');
        }
    };
    GLOD_contratos.prototype.initModalPermisosRol = function (data, status, xhr) {
        $.validator.unobtrusive.parse($("#formPermisos"));
        $('#IdLod').select2({
            allowClear: true,
            placeholder: '<Seleccione Libro>',
            theme: "bootstrap"
        });
        this.modalForm2.initModal();
        this.modalForm2.open();
    };
    GLOD_contratos.prototype.initModalEditPermisosRol = function (data, status, xhr) {
        $.validator.unobtrusive.parse($("#formPermisos"));
        this.modalForm2.initModal();
        this.modalForm2.open();
    };
    GLOD_contratos.prototype.saveResultPermisos = function (data, status, xhr) {
        var data1 = data.split(";");
        if (data1[0] == "true") {
            this.getPartial("#divPermisosRol", "/GLOD/Contratos/GetPermisosRol", { id: data1[1] });
            this.alert.toastOk();
            this.modalForm2.close();
            $(".ladda-button").ladda('stop');
            return;
        }
        else {
            $('#btnSubmit').stop();
            this.alert.toastErrorData(data);
            $(".ladda-button").ladda('stop');
        }
    };
    return GLOD_contratos;
}(GenericController));
