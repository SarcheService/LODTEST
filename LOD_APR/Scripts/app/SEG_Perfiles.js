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
var SEG_Perfiles = (function (_super) {
    __extends(SEG_Perfiles, _super);
    function SEG_Perfiles(_Area, _Controller) {
        var _this = _super.call(this) || this;
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalDelete";
        _this.formName = "#formDatos";
        _this.urlGetTable = "/" + _Area + "/" + _Controller + "/getPerfiles";
        var tablaDatos = new TableHelper('#tablaDatos', false);
        _this.modalForm = new ModalHelper(_this.modalName);
        $('.iCheck-helper').click(function (e) {
            if (this.previousSibling.value == 1) {
                $('#TipoPerfil').val(1);
            }
            else {
                $('#TipoPerfil').val(2);
            }
        });
        return _this;
    }
    SEG_Perfiles.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        this.modalForm.open();
    };
    SEG_Perfiles.prototype.saveResult = function (data, status, xhr) {
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
    SEG_Perfiles.prototype.GetPerfiles = function (_id) {
        this.setLink("#BtnCreate", "/Admin/Perfil/Create?id=" + _id);
        $("#IdEmpresa").val(_id);
        this.getPartial("#divTableDatos", this.urlGetTable, { id: _id });
        var tablaDatos = new TableHelper('#tablaDatos', false);
    };
    SEG_Perfiles.prototype.getTable = function () {
        this.getPartial("#divTableDatos", this.urlGetTable, { id: $("#IdEmpresa").val() });
        var tablaDatos = new TableHelper('#tablaDatos', false);
    };
    return SEG_Perfiles;
}(GenericController));
var OpcionesPerfiles = (function (_super) {
    __extends(OpcionesPerfiles, _super);
    function OpcionesPerfiles(_idPerfil, _Area, _Controller) {
        var _this = _super.call(this) || this;
        _this.urlGetTable = "/" + _Area + "/" + _Controller + "/getOpciones";
        _this.urlGetTableTipo = "/" + _Area + "/" + _Controller + "/getTipo";
        _this.btnSubmit = $('#btnSubmit').ladda();
        $("#IdSistema").select2({
            allowClear: false,
            theme: "bootstrap"
        }).on('change', function (e) {
            var _IdSistema = $("#IdSistema").val();
            var _idPerfil = $("#IdPerfil").val();
            if (String(_IdSistema) != "") {
                _this.urlGetTable = "/" + _Controller + "/getOpciones";
                _this.getOptionChanage(_idPerfil, _IdSistema);
            }
        });
        var _IdSistema = $("#IdSistema").val();
        _this.getOpciones(_idPerfil, _Area, _Controller);
        return _this;
    }
    OpcionesPerfiles.prototype.getOpciones = function (_idPerfil, _Area, _Controller) {
        var _this = this;
        this.Events.bind("OnGetPartial", function () {
            $('#tabPermisos').on('change', 'input[type=checkbox]', function (e) {
                var permiso = $("#" + e.currentTarget.id).data("opcion");
                var perfil = $("#" + e.currentTarget.id).data("perfil");
                var isChecked = e.currentTarget.checked;
                _this.savePermiso(perfil, permiso, isChecked, _Area, _Controller);
            });
        });
        this.getPartial("#tabPermisos", this.urlGetTable, { idPerfil: _idPerfil });
    };
    OpcionesPerfiles.prototype.getTipo = function (_idPerfil, _Area, _Controller) {
        var _this = this;
        this.Events.bind("OnGetPartial", function () {
            $('#tabTipo').on('change', 'input[type=checkbox]', function (e) {
                var tipo = $("#" + e.currentTarget.id).data("tipo");
                var perfil = $("#" + e.currentTarget.id).data("perfil");
                var isChecked = e.currentTarget.checked;
                _this.savePermisoTipo(perfil, tipo, isChecked, _Area, _Controller);
            });
        });
        this.getPartial("#tabTipo", this.urlGetTableTipo, { idPerfil: _idPerfil });
    };
    OpcionesPerfiles.prototype.getPerfilEmpresa = function (id) {
        console.log("id");
        if (this.Estandar) {
            this.Estandar = false;
            this.getSelectListItem("#IdSistema", "/Admin/Perfil/GetOptionsSelect", { IdPerfil: $("#IdPerfil").val(), IdEmpresa: id });
            this.Events.bind("OnGetLatestSelect", function () {
            });
        }
        if (id == -1) {
            this.Estandar = true;
            this.getSelectListItem("#IdSistema", "/Admin/Perfil/GetOptionsSelect", { IdPerfil: $("#IdPerfil").val(), IdEmpresa: id });
            this.Events.bind("OnGetLatestSelect", function () {
            });
        }
        $("#IdEmpresa").val(id);
        var idSis = $("#IdSistema").val();
        $("#IdSistema").val(idSis).trigger('change');
    };
    OpcionesPerfiles.prototype.savePermiso = function (idPerfil, idPerm, checked, _Area, _Controller) {
        var _this = this;
        this.Events.bind("OnPostCustomData", function (data) {
            if (data == 'true') {
                _this.alert.toastOk();
            }
            else {
                _this.alert.toastErrorData(data);
            }
        });
        this.postCustomData({ idPerfil: idPerfil, idPermiso: idPerm, isChecked: checked, IdEmpresa: $("#IdEmpresa").val() }, '/' + _Area + '/' + _Controller + '/savePermiso/');
    };
    OpcionesPerfiles.prototype.savePermisoTipo = function (idPerfil, Tipo, checked, _Area, _Controller) {
        var _this = this;
        this.Events.bind("OnPostCustomData", function (data) {
            if (data == 'true') {
                _this.alert.toastOk();
            }
            else {
                _this.alert.toastErrorData(data);
            }
        });
        this.postCustomData({ idPerfil: idPerfil, tipo: Tipo, isChecked: checked }, '/' + _Area + '/' + _Controller + '/savePermisoTipo/');
    };
    OpcionesPerfiles.prototype.getOptionChanage = function (_IdPerfil, _IdSistema) {
        var _this = this;
        this.DesactiveTabs();
        this.Events.bind("OnGetPartial", function () {
            _this.ActiveTabs();
        });
        this.getPartial("#tabPermisos", this.urlGetTable, { IdSistema: _IdSistema, idPerfil: _IdPerfil, IdEmpresa: $("#IdEmpresa").val() });
    };
    OpcionesPerfiles.prototype.submit = function () {
        this.btnSubmit.ladda('start');
    };
    OpcionesPerfiles.prototype.saveResult = function (data, status, xhr) {
        if (data == 'true') {
            this.alert.toastOk();
            this.btnSubmit.ladda('stop');
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    OpcionesPerfiles.prototype.DesactiveTabs = function () {
        $(".LiTab").css("pointer-events", "none");
        $("a.Tab-Admin").css("cursor", "not-allowed");
    };
    OpcionesPerfiles.prototype.ActiveTabs = function () {
        $(".LiTab").css("pointer-events", "all");
        $("a.Tab-Admin").css("cursor", "default");
    };
    return OpcionesPerfiles;
}(GenericController));
