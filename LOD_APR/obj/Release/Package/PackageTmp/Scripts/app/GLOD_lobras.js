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
var GLOD_lobras = (function (_super) {
    __extends(GLOD_lobras, _super);
    function GLOD_lobras() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalLibroObras";
        _this.formName = "#formLibroObras";
        _this.InitLobras();
        _this.modalForm = new ModalHelper(_this.modalName);
        $('[data-toggle="tooltip"]').tooltip();
        $('.input-daterange').datepicker({
            language: 'es',
            format: "dd-mm-yyyy",
            today: true,
            todayHighlight: true,
            autoclose: true
        });
        $("#IdEstado").select2({
            allowClear: true,
            placeholder: 'Seleccione un estado',
            theme: "bootstrap"
        });
        $("#UserId").select2({
            allowClear: true,
            placeholder: 'Remitentes',
            theme: "bootstrap"
        });
        return _this;
    }
    GLOD_lobras.prototype.initModal = function (data, status, xhr) {
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
                    _this.getPartial("#DivImgLod", "/Admin/Documentos/getImagenPreview/", data);
                    return;
                }
                else {
                    _this.alert.toastErrorData(response);
                }
            });
            _this.postFileToDB(data, "/Admin/Documentos/AddImagenPreview/");
        });
        this.modalForm.open();
    };
    GLOD_lobras.prototype.GetFiltroRapido = function (cs, idLib) {
        this.getPartial("#PanelAnotaciones", "/ASP/LibroObras/GetFiltroRapido", { cs: cs, idLib: idLib });
        this.InitLobras();
    };
    GLOD_lobras.prototype.saveResult = function (data, status, xhr) {
        var data1 = data.split(";");
        if (data1[0] == 'true') {
            this.getPartial("#divTableDatos", "/ASP/LibroObras/GetLibrosObras/", { id: data1[1] });
            this.Events.bind("OnGetPartial", function () {
                var tableHelper = new TableHelper('#datatableAnot1', true);
            });
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(data1[1]);
        }
    };
    GLOD_lobras.prototype.SaveArchivarLDOResult = function (data, status, xhr) {
        var data1 = data.split(";");
        if (data1[0] == 'true') {
            this.Events.bind("OnGetPartial", function () {
            });
            this.alert.toastOk();
            this.modalForm.close();
            window.location.href = "/ASP/LibroObras/Index/" + data1[1];
            return;
        }
        else {
            this.alert.toastErrorData(data1[0]);
        }
    };
    GLOD_lobras.prototype.InitModalArchivar = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm = new ModalHelper("#modaLibroObras");
        this.modalForm.initModal();
        this.modalForm.open();
    };
    GLOD_lobras.prototype.Filtro = function () {
        this.btnSubmitLadda = $('#btnFilter').ladda();
        this.btnSubmitLadda.ladda('start');
        var filtro = {
            IdEstado: $('#IdEstado').val(),
            FDesde: $('#FDesdeFilter').val(),
            FHasta: $('#FHastaFilter').val(),
            UserId: $('#UserId').val(),
            IdLibro: $('#IdLibroObra').val(),
            searchCuerpo: $('#searchCuerpo').val()
        };
        this.Events.bind("OnGetPartial", function () {
            var tableHelper = new TableHelper('#datatableAnot1', true);
            var btns = $(".ladda-button").ladda();
            btns.ladda('stop');
        });
        this.getPartialContentTypeJson("#PanelAnotaciones", "/ASP/LibroObras/Filtro", filtro, true);
    };
    GLOD_lobras.prototype.CleanFiltro = function () {
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
        this.getPartialContentTypeJson("#PanelAnotaciones", "/ASP/LibroObras/RemoveFiltro", { IdLibro: $('#IdLibroObra').val() }, true);
    };
    GLOD_lobras.prototype.getPartialContentTypeJson = function (_panelName, _url, _data, FireEvent) {
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
    GLOD_lobras.prototype.InitLobras = function () {
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
        });
        $(function () {
            var checkAll = $('input.chkTodos');
            var checkboxes = $('input.chkInd');
            checkAll.on('ifChecked ifUnchecked', function (event) {
                if (event.type == 'ifChecked') {
                    checkboxes.iCheck('check');
                }
                else {
                    checkboxes.iCheck('uncheck');
                }
            });
            checkboxes.on('ifChanged', function (event) {
                if (checkboxes.filter(':checked').length == checkboxes.length) {
                    checkAll.prop('checked', 'checked');
                }
                else {
                    checkAll.removeProp('checked');
                }
                checkAll.iCheck('update');
            });
        });
        var tableHelper = new TableHelper('#datatableAnot1', true);
    };
    GLOD_lobras.prototype.MarcarDest = function (cs, idLib) {
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
            this.postCustomData({ cs: cs, sel: array }, '/ASP/Anotaciones/MarcarDest/');
            this.Events.bind("OnPostCustomData", function (result) {
                if (result == "true") {
                    var Case = $("#Cs").val();
                    _this.alert.toastOk();
                    _this.GetFiltroRapido(Case, idLib);
                    _this.GetPanelOpc(idLib);
                }
                else {
                    _this.alert.toastErrorData("Ha ocurrido un problema. Por favor, notifique al administrador;" + result);
                }
            });
        }
    };
    GLOD_lobras.prototype.GetPanelOpc = function (idLib) {
        this.getPartial("#PanelOpc", "/ASP/LibroObras/GetOpcionesFiltro/", { id: idLib });
    };
    GLOD_lobras.prototype.addFav = function (id, idlib) {
        var _this = this;
        $("#" + id).toggleClass('text-warning');
        $("#" + id).toggleClass('fa-star');
        $("#" + id).toggleClass('fa-star-o');
        this.postCustomData({ id: id }, "/Anotaciones/addDestacada/");
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
    };
    GLOD_lobras.prototype.MarcarLeidaSel = function (cs, idlib) {
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
            this.postCustomData({ cs: cs, sel: array }, "/Anotaciones/MarcarLeidaSel/");
            this.Events.bind("OnPostCustomData", function (result) {
                if (result == "true") {
                    _this.alert.toastOk();
                    var Case = $("#Cs").val();
                    _this.GetFiltroRapido(Case, idlib);
                }
                else {
                    _this.alert.toastErrorData(result);
                }
            });
        }
    };
    GLOD_lobras.prototype.SetUserLib = function (Case, IdLib) {
        var _this = this;
        $(".modal-load").addClass("sk-loading");
        if (Case == 0) {
            var LstOptionsSelected = $("#formPersSelected_Selected option").map(function () {
                return $(this).val();
            }).get();
            var LstOptionsNotSelected = $("#formPersSelected_NotSelected option").map(function () {
                return $(this).val();
            }).get();
            this.postCustomDataAsync({ LstOptionsSelected: LstOptionsSelected,
                LstOptionsNotSelected: LstOptionsNotSelected, IdLib: IdLib }, "/ASP/LibroObras/SetPersonalLib");
            this.Events.bind("OnPostCustomData", function (response) {
                $(".modal-load").removeClass("sk-loading");
                if (response == 'true') {
                    _this.alert.toastOk();
                }
                else {
                    _this.alert.toastError();
                }
            });
        }
        else {
            var LstOptionsSelected = $("#formContSelected_Selected option").map(function () {
                return $(this).val();
            }).get();
            var LstOptionsNotSelected = $("#formContSelected_NotSelected option").map(function () {
                return $(this).val();
            }).get();
            this.postCustomData({ LstOptionsSelected: LstOptionsSelected,
                LstOptionsNotSelected: LstOptionsNotSelected, IdLib: IdLib }, "/ASP/LibroObras/SetContactosLib");
            this.Events.bind("OnPostCustomData", function (response) {
                $(".modal-load").removeClass("sk-loading");
                if (response == 'true') {
                    _this.alert.toastOk();
                }
                else {
                    _this.alert.toastError();
                }
            });
        }
    };
    GLOD_lobras.prototype.RestaurardualListBox = function (Case, IdLib) {
        $(".modal-load").addClass("sk-loading");
        if (Case == 0) {
            this.getPartialAsync("#divDualListPersonal", "/ASP/LibroObras/RestoredualListPers/", { IdLib: IdLib, IdEmpresa: $("#IdEmpresa").val() }, false);
            this.Events.bind("OnGetPartial", function () {
                $('#formLibUserPer').bootstrapDualListbox({
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
                    SelectedListId: 'formPersSelected_Selected',
                    nonSelectedListId: 'formPersSelected_NotSelected'
                });
                $(".modal-load").removeClass("sk-loading");
            });
        }
        else {
            this.getPartialAsync("#divDualListContacto", "/ASP/LibroObras/RestoredualListCont/", { IdLib: IdLib }, false);
            this.Events.bind("OnGetPartial", function () {
                $('#formLibUserCon').bootstrapDualListbox({
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
                    SelectedListId: 'formContSelected_Selected',
                    nonSelectedListId: 'formContSelected_NotSelected'
                });
                $(".modal-load").removeClass("sk-loading");
            });
        }
    };
    return GLOD_lobras;
}(GenericController));
