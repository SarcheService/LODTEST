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
var BIB_Home = (function (_super) {
    __extends(BIB_Home, _super);
    function BIB_Home() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalPassword";
        _this.formName = "#formPassword";
        _this.urlGetTable = "/GLOD/Biblioteca//getTableUsers";
        _this.modalForm = new ModalHelper(_this.modalName);
        var tableHelper = new TableHelper('#tableDatos', false, true, true);
        $("input[name=flexRadioDefault]").click(function () {
            console.log("#Formulario", $("#DocAdmin", $("#DocTecnico", $("#Otros"))));
            if ($("#Formulario").is(':checked')) {
                $("#Formulario").val(true);
                $("#DocAdmin").val(false);
                $("#DocTecnico").val(false);
                $("#Otros").val(false);
            }
            if ($("#DocAdmin").is(':checked')) {
                $("#Formulario").val(false);
                $("#DocAdmin").val(true);
                $("#DocTecnico").val(false);
                $("#Otros").val(false);
            }
            if ($("#DocTecnico").is(':checked')) {
                $("#Formulario").val(false);
                $("#DocAdmin").val(false);
                $("#DocTecnico").val(true);
                $("#Otros").val(false);
            }
            if ($("#Otros").is(':checked')) {
                $("#Formulario").val(false);
                $("#DocAdmin").val(false);
                $("#DocTecnico").val(false);
                $("#Otros").val(true);
            }
        });
        $("#IdDireccion").select2({
            placeholder: {
                id: '0',
                text: 'Seleccione una Dirección'
            },
            allowClear: true,
            theme: "bootstrap"
        }).val('0').trigger('change');
        var $option = $("<option selected></option>").val('0').text("Seleccione una Dirección");
        $("#IdDireccion").append($option).trigger('change');
        $("#IdDireccion").val('0');
        $("#IdFiscalizador").select2({
            placeholder: {
                id: '0',
                text: 'Seleccione un Fiscalizador'
            },
            allowClear: true,
            theme: "bootstrap"
        }).val('').trigger('change');
        var $option = $("<option selected></option>").val('0').text("Seleccione un Fiscalizador");
        $("#IdFiscalizador").append($option).trigger('change');
        $("#IdFiscalizador").val('0');
        $("#IdSujEcon").select2({
            placeholder: {
                id: '0',
                text: 'Seleccione un Contratista'
            },
            allowClear: true,
            theme: "bootstrap"
        }).val('').trigger('change');
        var $option = $("<option selected></option>").val('0').text("Seleccione un Contratista");
        $("#IdSujEcon").append($option).trigger('change');
        $("#IdSujEcon").val(0);
        $("#IdContrato").select2({
            placeholder: {
                id: '0',
                text: 'Seleccione un Contrato'
            },
            allowClear: true,
            theme: "bootstrap"
        }).val('').trigger('change');
        var $option = $("<option selected></option>").val('0').text("Seleccione un Contrato");
        $("#IdContrato").append($option).trigger('change');
        $("#IdContrato").val(0);
        $("#IdTipoLibro").select2({
            placeholder: {
                id: '0',
                text: 'Seleccione un Tipo Libro'
            },
            allowClear: true,
            theme: "bootstrap"
        }).val('').trigger('change');
        var $option = $("<option selected></option>").val('0').text("Seleccione un Tipo Libro");
        $("#IdTipoLibro").append($option).trigger('change');
        $("#IdTipoLibro").val(0);
        $("#IdTipoComunicacion").select2({
            placeholder: {
                id: '0',
                text: 'Seleccione un Tipo de Comunicación'
            },
            allowClear: true,
            theme: "bootstrap"
        }).val('').trigger('change');
        var $option = $("<option selected></option>").val('0').text("Seleccione un Tipo de Comunicación");
        $("#IdTipoComunicacion").append($option).trigger('change');
        $("#IdTipoComunicacion").val(0);
        $("#IdSubtipoComunicacion").select2({
            placeholder: {
                id: '0',
                text: 'Seleccione un Tipo de Subtipo de Comunicación'
            },
            allowClear: true,
            theme: "bootstrap"
        }).val('').trigger('change');
        var $option = $("<option selected></option>").val('0').text("Seleccione un Subtipo de comunicación");
        $("#IdSubtipoComunicacion").append($option).trigger('change');
        $("#IdSubtipoComunicacion").val(0);
        $("#IdTipoDoc").select2({
            placeholder: {
                id: '0',
                text: 'Seleccione un Tipo de Documento'
            },
            allowClear: true,
            theme: "bootstrap",
        }).val('').trigger('change');
        var $option = $("<option selected></option>").val('0').text("Seleccione un Tipo de Documento");
        $("#IdTipoDoc").append($option).trigger('change');
        $("#IdTipoDoc").val(0);
        $("#IdDireccion").on('change', function (e) {
            var IdSujEcon = $("#IdSujEcon").val();
            var idDireccion = $("#IdDireccion").val();
            var idFiscalizador = $("#IdFiscalizador").val();
            if (idFiscalizador == null) {
                idFiscalizador = 0;
            }
            _this.GetSujetos(idDireccion);
            _this.GetFiscalizador(idDireccion);
            _this.GetContratos(IdSujEcon, idFiscalizador, idDireccion);
        });
        $("#IdSujEcon").on('change', function (e) {
            var IdSujEcon = $("#IdSujEcon").val();
            var idDireccion = $("#IdDireccion").val();
            var idFiscalizador = $("#IdFiscalizador").val();
            if (idFiscalizador == null) {
                idFiscalizador = 0;
            }
            _this.GetContratos(IdSujEcon, idFiscalizador, idDireccion);
        });
        $("#IdFiscalizador").on('change', function (e) {
            var IdSujEcon = $("#IdSujEcon").val();
            var idDireccion = $("#IdDireccion").val();
            var idFiscalizador = $("#IdFiscalizador").val();
            if (idFiscalizador == null) {
                idFiscalizador = 0;
            }
            _this.GetContratos(IdSujEcon, idFiscalizador, idDireccion);
        });
        $("#IdContrato").on('change', function (e) {
            var id = $("#IdContrato").val();
            console.log("entró al change");
            _this.GetTipoLOD(id);
        });
        $("#IdTipoLibro").on('change', function (e) {
            var id = $("#IdTipoLibro").val();
            console.log("entró al change tipo tipo libro");
            _this.GetTipoComunicacion(id);
        });
        $("#IdTipoComunicacion").on('change', function (e) {
            var id = $("#IdTipoComunicacion").val();
            console.log("entró al change tipo comunicacion");
            _this.GetSubtipo(id);
        });
        $("#IdSubtipoComunicacion").on('change', function (e) {
            var id = $("#IdSubtipoComunicacion").val();
            console.log("entró al change subtipo");
            _this.GetTipoDoc(id);
        });
        return _this;
    }
    ;
    BIB_Home.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.btnSubmit = $('#btnSubmit').ladda();
        this.modalForm.initModal();
        this.modalForm.open();
    };
    BIB_Home.prototype.submit = function () {
        this.btnSubmit.ladda('start');
    };
    BIB_Home.prototype.Filtro = function () {
        var filtro = {
            Formulario: $('#chkFormulario:checkbox:checked').length > 0,
            DocTecnicos: $('#chkDocTecnico:checkbox:checked').length > 0,
            DocAdmin: $('#chkDocAdmin:checkbox:checked').length > 0,
            Otros: $('#chkDocOtros:checkbox:checked').length > 0,
            IdDireccion: $("#IdDireccion").val(),
            IdSujEcon: $("#IdSujEcon").val(),
            IdContrato: $("#IdContrato").val(),
            IdTipoLibroObra: $("#IdTipoLibro").val(),
            IdTipoComunicacion: $("#IdTipoComunicacion").val(),
            IdSubtipoComunicacion: $("#IdSubtipoComunicacion").val(),
            IdTipoDoc: $("#IdTipoDoc").val(),
            FechaCreacion: $("#FechaCreacion").val()
        };
        this.Events.bind("OnGetPartial", function () {
            var tableHelper = new TableHelper('#tableDatos', false);
        });
        this.getPartial("#divTableDatos", "/GLOD/Biblioteca/GetFiltro", filtro);
    };
    BIB_Home.prototype.saveResult = function (data, status, xhr) {
        var btns = $(".ladda-button").ladda();
        btns.ladda('stop');
        if (data == 'true') {
            this.Events.bind("OnGetPartial", function () {
                var tableHelper = new TableHelper('#tableDatos', false, true, true);
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
    BIB_Home.prototype.GetLibrosObra = function (IdSub) {
        this.Events.bind("OnGetPartial", function () {
            $(".panel-load").removeClass("sk-loading");
            $("#IdLibroObra").select2({
                allowClear: false,
                placeholder: 'Seleccione un Libro',
                theme: "bootstrap"
            });
        });
        this.getPartial("#DivLibros", "/GLOD/Biblioteca/GetLibroObra", { "IdContrato": IdSub });
    };
    BIB_Home.prototype.GetAux = function (IdAlgo) {
    };
    BIB_Home.prototype.GetTiposLOD = function (IdSub) {
        this.Events.bind("OnGetPartial", function () {
            $("#IdTipoLibro").select2({
                placeholder: 'Seleccione un Tipo de Libro',
                allowClear: true,
                theme: "bootstrap"
            });
        });
        this.getPartial("#DivTipoLod", "/GLOD/Biblioteca/GetTipoLOD", { "IdContrato": IdSub });
    };
    BIB_Home.prototype.GetTipoComunicacion = function (IdSub) {
        var _this = this;
        this.Events.bind("OnGetPartial", function () {
            $("#IdTipoComunicacion").select2({
                placeholder: {
                    id: '0',
                    text: 'Seleccione un Tipo de Comunicación'
                },
                allowClear: true,
                theme: "bootstrap"
            }).on('change', function (e) {
                var id = $("#IdTipoComunicacion").val();
                console.log("entró al change tipo comunicacion");
                _this.GetSubtipo(id);
            });
            var $option = $("<option selected></option>").val('0').text("Seleccione un Tipo de Comunicación");
            $("#IdTipoComunicacion").append($option).trigger('change');
            $("#IdTipoComunicacion").val(0);
        });
        this.getPartial("#DivTipoCom", "/GLOD/Biblioteca/GetTipoCom", { "IdTipoLod": IdSub });
    };
    BIB_Home.prototype.GetSubtipo = function (IdSub) {
        var _this = this;
        this.Events.bind("OnGetPartial", function () {
            $("#IdSubtipoComunicacion").select2({
                placeholder: {
                    id: '0',
                    text: 'Seleccione un Tipo de Subtipo de Comunicación'
                },
                allowClear: true,
                theme: "bootstrap"
            }).on('change', function (e) {
                var id = $("#IdSubtipoComunicacion").val();
                console.log("entró al change subtipo");
                _this.GetTipoDoc(id);
            });
            var $option = $("<option selected></option>").val('0').text("Seleccione un Subtipo de comunicación");
            $("#IdSubtipoComunicacion").append($option).trigger('change');
            $("#IdSubtipoComunicacion").val(0);
        });
        this.getPartial("#DivSubtipo", "/GLOD/Biblioteca/GetSubtipo", { "IdTipoCom": IdSub });
    };
    BIB_Home.prototype.GetTipoDoc = function (IdSub) {
        this.Events.bind("OnGetPartial", function () {
            $("#IdTipoDoc").select2({
                placeholder: {
                    id: '0',
                    text: 'Seleccione un Tipo de Documento'
                },
                allowClear: true,
                theme: "bootstrap",
            });
            var $option = $("<option selected></option>").val('0').text("Seleccione un Tipo de Documento");
            $("#IdTipoDoc").append($option).trigger('change');
            $("#IdTipoDoc").val(0);
        });
        this.getPartial("#DivTipoDoc", "/GLOD/Biblioteca/GetTipoDoc", { "IdTipoSub": IdSub });
    };
    BIB_Home.prototype.GetFiscalizador = function (IdSuj) {
        var _this = this;
        console.log(IdSuj);
        this.Events.bind("OnGetPartial", function () {
            $("#IdFiscalizador").select2({
                placeholder: {
                    id: '0',
                    text: 'Seleccione un Fiscalizador'
                },
                allowClear: true,
                theme: "bootstrap"
            }).on('change', function (e) {
                var IdSujEcon = $("#IdSujEcon").val();
                var idDireccion = $("#IdDireccion").val();
                var idFiscalizador = $("#IdFiscalizador").val();
                console.log("entró al change ");
                _this.GetContratos(IdSujEcon, idFiscalizador, idDireccion);
            });
            var $option = $("<option selected></option>").val('0').text("Seleccione un Fiscalizador");
            $("#IdFiscalizador").append($option).trigger('change');
            $("#IdFiscalizador").val('0');
        });
        this.getPartial("#DivFiscalizador", "/GLOD/Biblioteca/GetFiscalizador", { "IdDireccion": IdSuj });
    };
    BIB_Home.prototype.GetSujetos = function (IdSuj) {
        var _this = this;
        console.log(IdSuj);
        this.Events.bind("OnGetPartial", function () {
            $("#IdSujEcon").select2({
                placeholder: {
                    id: '0',
                    text: 'Seleccione un Contratista'
                },
                allowClear: true,
                theme: "bootstrap"
            }).on('change', function (e) {
                var IdSujEcon = $("#IdSujEcon").val();
                var idDireccion = $("#IdDireccion").val();
                var idFiscalizador = $("#IdFiscalizador").val();
                console.log("entró al change ");
                _this.GetContratos(IdSujEcon, idFiscalizador, idDireccion);
            });
            var $option = $("<option selected></option>").val('0').text("Seleccione un Contratista");
            $("#IdSujEcon").append($option).trigger('change');
            $("#IdSujEcon").val(0);
        });
        this.getPartial("#DivSujetos", "/GLOD/Biblioteca/GetSujEcon", { "IdDireccion": IdSuj });
    };
    BIB_Home.prototype.GetContratos = function (IdSujEcon, IdFiscalizador, IdDireccion) {
        var _this = this;
        console.log("Entro al GetContratos");
        console.log(IdSujEcon);
        console.log(IdDireccion);
        console.log(IdFiscalizador);
        this.Events.bind("OnGetPartial", function () {
            $("#IdContrato").select2({
                placeholder: {
                    id: '0',
                    text: 'Seleccione un Contrato'
                },
                allowClear: true,
                theme: "bootstrap"
            }).on('change', function (e) {
                var id = $("#IdContrato").val();
                console.log("entró al change");
                _this.GetTipoLOD(id);
            });
            var $option = $("<option selected></option>").val('0').text("Seleccione un Contrato");
            $("#IdContrato").append($option).trigger('change');
            $("#IdContrato").val(0);
        });
        this.getPartial("#DivContrato", "/GLOD/Biblioteca/GetContratos", { "IdSujEcon": IdSujEcon, "IdFiscalizador": IdFiscalizador, "IdDireccion": IdDireccion });
    };
    BIB_Home.prototype.GetTipoLOD = function (IdSub) {
        var _this = this;
        this.Events.bind("OnGetPartial", function () {
            $("#IdTipoLibro").select2({
                placeholder: {
                    id: '0',
                    text: 'Seleccione un Tipo Libro'
                },
                allowClear: true,
                theme: "bootstrap"
            }).on('change', function (e) {
                var id = $("#IdTipoLibro").val();
                console.log("entró al change tipo tipo libro");
                _this.GetTipoComunicacion(id);
            });
            var $option = $("<option selected></option>").val('0').text("Seleccione un Tipo Libro");
            $("#IdTipoLibro").append($option).trigger('change');
            $("#IdTipoLibro").val(0);
        });
        this.getPartial("#DivTipoLod", "/GLOD/Biblioteca/GetTipoLOD", { "IdContrato": IdSub });
    };
    return BIB_Home;
}(GenericController));
