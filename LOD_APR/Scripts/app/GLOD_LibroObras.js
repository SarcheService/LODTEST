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
var GLOD_LibroObras = (function (_super) {
    __extends(GLOD_LibroObras, _super);
    function GLOD_LibroObras() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalLibros";
        _this.formName = "#formLibros";
        _this.urlGetTree = "/GLOD/Home/getTree";
        _this.modalForm = new ModalHelper(_this.modalName);
        $('[data-toggle="tooltip"]').tooltip();
        $('.clickable').click(function () {
            $('#home').html('<embed src="./files/' + this.id + '_4b/' + this.id + '_ejemplo.pdf" type="application/pdf" width="100%" height="400px" class="margenfilemodal" />');
        });
        return _this;
    }
    GLOD_LibroObras.prototype.initModalDelete = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        this.modalForm.open();
    };
    GLOD_LibroObras.prototype.initModal = function (data, status, xhr) {
        var _this = this;
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        $("#IdTipoLod").select2({
            allowClear: true,
            placeholder: '< Seleccione un Tipo Libro >',
            theme: "bootstrap"
        });
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
        this.modalForm.open();
    };
    GLOD_LibroObras.prototype.saveResult = function (data, status, xhr) {
        var data1 = data.split(";");
        if (data1[0] == 'delete') {
            glod_administracion.treeView.Events.bind("OnGetTreeData", function () {
                setTimeout(function () { $('#treeView').jstree(true).select_node("c_" + data1[1]); }, 1000);
            });
            glod_administracion.treeView.updateTreeData(this.urlGetTree);
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else if (data1[0] == 'true') {
            glod_administracion.treeView.Events.bind("OnGetTreeData", function () {
                setTimeout(function () { $('#treeView').jstree(true).select_node("l_" + data1[1]); }, 1000);
            });
            glod_administracion.treeView.updateTreeData(this.urlGetTree);
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    GLOD_LibroObras.prototype.Filtro = function () {
        this.btnSubmitLadda = $('#btnFilter').ladda();
        this.btnSubmitLadda.ladda('start');
        var filtro = {
            IdEstado: $('#IdEstado').val(),
            FDesde: $('#FDesdeFilter').val(),
            FHasta: $('#FHastaFilter').val(),
            UserId: $('#UserId').val(),
            IdBitacora: $('#IdBitacora').val(),
            searchCuerpo: $('#searchCuerpo').val()
        };
        this.Events.bind("OnGetPartial", function () {
            var tableHelper = new TableHelper('#datatableAnot1', true);
            var btns = $(".ladda-button").ladda();
            btns.ladda('stop');
        });
        this.getPartialContentTypeJson("#PanelAnotaciones", "/ASP/Bitacoras/Filtro", filtro, true);
    };
    GLOD_LibroObras.prototype.CleanFiltro = function () {
        this.btnSubmitLadda = $('#btnRemoveFiltro').ladda();
        this.btnSubmitLadda.ladda('start');
        $('#IdEstado').val('').trigger('change');
        $('#FDesdeFilter').val("");
        $('#FHastaFilter').val("");
        $('#UserId').val('').trigger('change');
        $('#searchCuerpo').val("");
        this.Events.bind("OnGetPartial", function () {
            var tableHelper = new TableHelper('#datatableAnot1', true);
            var btns = $(".ladda-button").ladda();
            btns.ladda('stop');
        });
        this.getPartialContentTypeJson("#PanelAnotaciones", "/ASP/Bitacoras/RemoveFiltro", { IdBitacora: $('#IdBitacora').val() }, true);
    };
    GLOD_LibroObras.prototype.getPartialContentTypeJson = function (_panelName, _url, _data, FireEvent) {
        var _this = this;
        if (FireEvent === void 0) { FireEvent = true; }
        $.ajax({
            url: _url,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(_data),
            async: false,
            beforeSend: function () {
                $(".panel-load").addClass("sk-loading");
            },
            success: function (result) {
                $(".panel-load").removeClass("sk-loading");
                $(_panelName).html(result);
                if (FireEvent) {
                    _this.Events.call("OnGetPartial");
                }
            }
        });
    };
    GLOD_LibroObras.prototype.GetFiltroRapido = function (cs, IdBit) {
        var _this = this;
        this.Events.bind("OnGetPartial", function () {
            _this.InitBit();
        });
        this.getPartial("#PanelAnotaciones", "/ASP/Bitacoras/GetFiltroRapido", { cs: cs, IdBit: IdBit });
    };
    GLOD_LibroObras.prototype.GetUserBit = function (IdBit) {
        var _this = this;
        this.Events.bind("OnPostCustomData", function (response) {
            if (response == 'true') {
                _this.alert.toastOk();
            }
            else {
                _this.alert.toastError();
            }
            $(".panel-load").removeClass("sk-loading");
        });
        $(".panel-load").addClass("sk-loading");
        var LstOptionsSelected = $("#formBitUser_Selected option").map(function () {
            return $(this).val();
        }).get();
        var LstOptionsNotSelected = $("#formBitUser_NonSelected option").map(function () {
            return $(this).val();
        }).get();
        this.postCustomDataAsync({ LstOptionsSelected: LstOptionsSelected, LstOptionsNotSelected: LstOptionsNotSelected, IdBit: IdBit }, "/ASP/Bitacoras/SetUserBits");
    };
    GLOD_LibroObras.prototype.RestaurarUserBit = function (IdBit) {
        this.Events.bind("OnGetPartial", function () {
            $('#formBitUser').bootstrapDualListbox({
                selectorMinimalHeight: 160,
                filterTextClear: 'Elementos No Seleccionados N°',
                filterPlaceHolder: 'Filtro',
                moveSelectedLabel: 'Mover Seleccionado',
                moveAllLabel: 'Mover Todo',
                removeSelectedLabel: 'Remover Seleccionado',
                removeAllLabel: 'Remover Todo',
                infoText: 'Elementos en Lista N° {0}',
                infoTextFiltered: '<span class="label label-warning">Filtrado</span> {0} from {1}',
                infoTextEmpty: 'Lista Vacia',
                SelectedListId: 'formBitUser_Selected',
                nonSelectedListId: 'formBitUser_NonSelected'
            });
        });
        this.getPartialAsync("#divDualList", "/ASP/Bitacoras/Restaurar/", { IdBit: IdBit });
    };
    GLOD_LibroObras.prototype.InsertContactoABit = function (IdBit) {
        var _this = this;
        var IdContacto = $('#IdContacto').val();
        this.Events.bind("OnPostCustomData", function (response) {
            _this.getPartial("#divContactosBit", "/ASP/Bitacoras/GetContactosBit/", { IdBit: IdBit });
        });
        this.postCustomDataAsync({ IdBit: IdBit, IdContacto: IdContacto }, "/ASP/Bitacoras/SetContactoBit/");
    };
    GLOD_LibroObras.prototype.GetLobrasBit = function (IdBit) {
        var _this = this;
        this.Events.bind("OnPostCustomData", function (result) {
            if ("true") {
                _this.Events.bind("OnGetPartial", function (result) { });
                _this.alert.toastOk();
                _this.getPartial("#divTableHistorialBit", "/ASP/Bitacoras/GetBitLobrasAsoc", { IdBit: IdBit });
                var tableHelper = new TableHelper('#tablaBitLobrasAsoc', false);
            }
        });
        var LstOptionsSelected = $("#formBitLobras_Selected option").map(function () {
            return $(this).val();
        }).get();
        var LstOptionsNotSelected = $("#formBitLobras_NonSelected option").map(function () {
            return $(this).val();
        }).get();
        this.postCustomDataAsync({ LstOptionsSelected: LstOptionsSelected, LstOptionsNotSelected: LstOptionsNotSelected, IdBit: IdBit }, "/ASP/Bitacoras/SetLobrasBit", "2");
    };
    GLOD_LibroObras.prototype.RestaurarLobrasBit = function (IdBit) {
        this.Events.bind("OnGetPartial", function (result) {
            $('#formBitLobras').bootstrapDualListbox({
                selectorMinimalHeight: 160,
                filterTextClear: 'Elementos No Seleccionados N°',
                filterPlaceHolder: 'Filtro',
                moveSelectedLabel: 'Mover Seleccionado',
                moveAllLabel: 'Mover Todo',
                removeSelectedLabel: 'Remover Seleccionado',
                removeAllLabel: 'Remover Todo',
                infoText: 'Elementos en Lista N° {0}',
                infoTextFiltered: '<span class="label label-warning">Filtrado</span> {0} from {1}',
                infoTextEmpty: 'Lista Vacia',
                SelectedListId: 'formBitLobras_Selected',
                nonSelectedListId: 'formBitLobras_NonSelected'
            });
        });
        this.getPartial("#divDualList2", "/ASP/Bitacoras/RestaurarLobrasBit/", { IdBit: IdBit });
    };
    GLOD_LibroObras.prototype.addFav = function (id, idlib) {
        var _this = this;
        $("#" + id).toggleClass('text-warning');
        $("#" + id).toggleClass('fa-star');
        $("#" + id).toggleClass('fa-star-o');
        this.Events.bind("OnPostCustomData", function (result) {
            if (result == "destacada") {
                _this.alert.toastOk();
                _this.GetPanelOpc(idlib);
            }
            else if (result == "nodestacada") {
                _this.alert.toastOk();
                _this.GetPanelOpc(idlib);
            }
            else if (result == "error") {
                _this.alert.toastErrorData("Ha ocurrido un error durante la modificación. Intentelo nuevamente");
            }
        });
        this.postCustomData({ id: id }, "/AnotacionesBit/addDestacada/");
    };
    GLOD_LibroObras.prototype.addFavAsoc = function (id, idbit) {
        var _this = this;
        $("#" + id).toggleClass('text-warning');
        $("#" + id).toggleClass('fa-star');
        $("#" + id).toggleClass('fa-star-o');
        this.Events.bind("OnPostCustomData", function (result) {
            if (result == "destacada") {
                _this.alert.toastOk();
                _this.GetPanelOpc(idbit);
            }
            else if (result == "nodestacada") {
                _this.alert.toastOk();
                _this.GetPanelOpc(idbit);
            }
            else if (result == "error") {
                _this.alert.toastErrorData("Ha ocurrido un error durante la modificación. Intentelo nuevamente");
            }
        });
        this.postCustomData({ id: id, idbit: idbit }, "/AnotacionesBit/addDestacadaAsoc/");
    };
    GLOD_LibroObras.prototype.MarcarDest = function (cs, IdBit) {
        var _this = this;
        $(".panel-load").addClass("sk-loading");
        var array = [];
        var count = 0;
        $('input:checkbox[id*="chkInd"]').each(function () {
            if ($(this).is(':checked')) {
                array[count] = $(this).val();
                count++;
            }
        });
        if (array.length > 0) {
            this.Events.bind("OnPostCustomData", function (result) {
                if (result == "true") {
                    var Case = $("#Cs").val();
                    _this.alert.toastOk();
                    _this.GetFiltroRapido(Case, IdBit);
                    _this.GetPanelOpc(IdBit);
                }
                else {
                    _this.alert.toastErrorData("Ha ocurrido un problema. Por favor, notifique al administrador;" + result);
                }
            });
            this.postCustomData({ cs: cs, sel: array, IdBit: IdBit }, '/ASP/AnotacionesBit/MarcarDest/');
        }
    };
    GLOD_LibroObras.prototype.MarcarLeidaSel = function (cs, IdBit) {
        var _this = this;
        $(".panel-load").addClass("sk-loading");
        var array = [];
        var count = 0;
        $('input:checkbox[id*="chkInd"]').each(function () {
            if ($(this).is(':checked')) {
                array[count] = $(this).val();
                count++;
            }
        });
        if (array.length > 0) {
            this.Events.bind("OnPostCustomData", function (result) {
                if (result == "true") {
                    _this.alert.toastOk();
                    var Case = $("#Cs").val();
                    _this.GetFiltroRapido(Case, IdBit);
                }
                else {
                    _this.alert.toastErrorData(result);
                }
            });
            this.postCustomData({ cs: cs, sel: array, IdBit: IdBit }, "/AnotacionesBit/MarcarLeidaSel/");
        }
    };
    GLOD_LibroObras.prototype.QuitarContactoBit = function (IdBit, IdContacto) {
        var _this = this;
        this.Events.bind("OnPostCustomData", function (result) {
            if (result == "true") {
                _this.getPartial("#divContactosBit", "/ASP/Bitacoras/GetContactosBit/", { IdBit: IdBit });
            }
            else {
                _this.alert.toastError();
            }
        });
        this.postCustomData({ IdBit: IdBit, IdContacto: IdContacto }, "/ASP/Bitacoras/QuitarContactoBit/");
    };
    GLOD_LibroObras.prototype.GetPanelOpc = function (IdBit) {
        this.getPartial("#PanelOpc", "/ASP/Bitacoras/GetOpcionesFiltro/", { id: IdBit });
    };
    return GLOD_LibroObras;
}(GenericController));
