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
var GLOD_inicioRapido = (function (_super) {
    __extends(GLOD_inicioRapido, _super);
    function GLOD_inicioRapido() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        $('#ContratosDisponible').select2({
            allowClear: true,
            placeholder: '<Seleccione Contrato>',
            theme: "bootstrap"
        });
        $("#ContratosDisponible").change(function () {
            _this.Events.bind("OnGetPartial", function () {
            });
            var estado = $("#ContratosDisponible").val();
            console.log(estado);
            if (estado !== "NULL" && estado !== null && estado !== '') {
                _this.getPartialAsync("#getInfoAvance", "/GLOD/Contratos/GetInfoAvance", { IdContrato: $("#ContratosDisponible").val() });
                _this.getPartialAsync("#getInfoDoc", "/GLOD/Contratos/GetInfoDoc", { IdContrato: $("#ContratosDisponible").val() });
                _this.getPartialAsync("#getContrato", "/GLOD/Contratos/GetContrato", { IdContrato: $("#ContratosDisponible").val() });
            }
        });
        return _this;
    }
    GLOD_inicioRapido.prototype.showSection = function () {
        document.getElementById('showSection').style.display = "none";
        document.getElementById('btnShow').style.display = "none";
    };
    GLOD_inicioRapido.prototype.GetDetailsLibro = function (idLibro) {
        console.log("Entre a la función");
        console.log(idLibro);
        document.getElementById('showSection').style.display = "block";
        if (idLibro !== "NULL" && idLibro !== null && idLibro !== '') {
            console.log("Entre a la función");
            this.getPartial("#getDetailsLibroActivo", "/GLOD/LibroObras/Details", { id: idLibro, TipoVista: 2 });
        }
        document.getElementById('btnShow').style.display = "block";
    };
    GLOD_inicioRapido.prototype.Buscar = function () {
        var _this = this;
        var buscar = $('#text_buscar').val();
        if (buscar.length > 0) {
            this.DesactiveTabs();
            this.Events.bind("OnGetPartial", function () {
                _this.ActiveTabs();
            });
            this.ReOrganizeTab();
            this.getPartialAsync("#TabContent", "/ASP/LibroObras/LibroIndexBuscar", { buscar: $("#text_buscar").val(), IdEmpresa: $("#IdEmpresa").val(), TipoVista: $("#TipoVista").val() });
        }
        else {
            $('#text_buscar').focus();
        }
    };
    GLOD_inicioRapido.prototype.Recargar = function () {
        var _this = this;
        this.DesactiveTabs();
        this.Events.bind("OnGetPartial", function () {
            _this.ActiveTabs();
        });
        this.ReOrganizeTab();
        this.getPartialAsync("#TabContent", "/ASP/LibroObras/LibroIndexRecargar", { IdEmpresa: $("#IdEmpresa").val(), TipoVista: $("#TipoVista").val() });
    };
    GLOD_inicioRapido.prototype.getLibBit = function (_id) {
        var _this = this;
        this.DesactiveTabs();
        this.Events.bind("OnGetPartial", function () {
            _this.ActiveTabs();
        });
        this.ReOrganizeTab();
        $("#IdEmpresa").val(_id);
        this.getPartialAsync("#TabContent", "/ASP/LibroObras/LibroIndexRecargar", { IdEmpresa: _id, TipoVista: $("#TipoVista").val() });
    };
    GLOD_inicioRapido.prototype.ChangeTipoVista = function () {
        var _this = this;
        this.DesactiveTabs();
        this.Events.bind("OnGetPartial", function () {
            _this.ActiveTabs();
        });
        this.ReOrganizeTab();
        var TipoVista;
        if ($("#TipoVista").val() == "IndexUsr") {
            $("#Icon").addClass('fa-th-large');
            $("#Icon").removeClass('fa-align-justify');
            TipoVista = "IndexUsrRows";
        }
        else {
            $("#Icon").removeClass('fa-th-large');
            $("#Icon").addClass('fa-align-justify');
            TipoVista = "IndexUsr";
        }
        $("#TipoVista").val(TipoVista);
        this.getPartialAsync("#TabContent", "/ASP/LibroObras/LibroIndexRecargar", { IdEmpresa: $("#IdEmpresa").val(), TipoVista: $("#TipoVista").val() });
    };
    GLOD_inicioRapido.prototype.ReOrganizeTab = function () {
        $("#tab1").addClass("active");
        $("#tab2").removeClass("active");
    };
    GLOD_inicioRapido.prototype.DesactiveTabs = function () {
        $(".LiTab").css("pointer-events", "none");
        $("a.Tab-Admin").css("cursor", "not-allowed");
    };
    GLOD_inicioRapido.prototype.ActiveTabs = function () {
        $(".LiTab").css("pointer-events", "all");
        $("a.Tab-Admin").css("cursor", "default");
    };
    return GLOD_inicioRapido;
}(GenericController));
