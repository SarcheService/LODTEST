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
var GLOD_Administracion = (function (_super) {
    __extends(GLOD_Administracion, _super);
    function GLOD_Administracion() {
        var _this = _super.call(this) || this;
        _this.modalCanvas = "#modalCanvas";
        _this.modalEdit = "#modalForm";
        _this.modalDelete = "#modalDelete";
        _this.formEdit = "#formEdit";
        _this.urlGetTree = "/GLOD/Home/getTree";
        _this.urlAdd = "/GLOD/Home/Create";
        _this.urlEdit = "/GLOD/Home/Edit";
        _this.urlDelete = "/GLOD/Home/Delete";
        _this.nodeParent = 0;
        _this.modalForm = new ModalHelper("#modalAdministracion");
        $(".panel-load").removeClass("sk-loading");
        var tableHelper = new TableHelper('#tablaContratos', false);
        return _this;
    }
    GLOD_Administracion.prototype.initTreeAdministracion = function (_treeName, _id, _tipo) {
        var _this = this;
        this.treeView = new TreeHelperAdmin(_treeName);
        $('#treeView').on("paste.jstree", function (parent, node) {
            var idOrigen = node.node[0];
            var idDestino = _this.treeView.SelectedNode;
            idOrigen.parents = [];
            idOrigen.children = [];
            idOrigen.children_d = [];
            idDestino.parents = [];
            idDestino.children = [];
            idDestino.children_d = [];
            console.log(idOrigen);
            console.log(idDestino);
            _this.Events.bind("OnGetPartial", function () {
                _this.initModal(null, null, null);
            });
            _this.getPartial("#modalCanvas", "/GLOD/Home/MoveNode", { origin: JSON.stringify(idOrigen), destination: JSON.stringify(idDestino) });
        });
        $('#treeView').on("show_contextmenu.jstree", function (e, $node) {
            $(_treeName).jstree({
                "contextmenu": {
                    "items": function ($node) {
                        return {
                            "Cut": {
                                "separator_before": false,
                                "separator_after": false,
                                "label": "Cortar",
                                "_disabled": _this.treeView.DisableCutContext(),
                                "action": function (obj) {
                                    _this.treeView.CutNode = $node;
                                    $(_treeName).jstree().cut($node);
                                }
                            },
                            "Paste": {
                                "separator_before": false,
                                "separator_after": false,
                                "label": _this.treeView.GetPasteContextName(),
                                "_disabled": _this.treeView.DisablePasteContext(),
                                "action": function (obj) {
                                    $(_treeName).jstree().paste($node);
                                }
                            }
                        };
                    }
                }
            });
        });
        this.treeView.Events.bind("OnSelectNode", function (node) {
            _this.treeView.SelectedNode = node;
            _this.nodeParent = node.data.db_id;
            _this.nodeID = node.data.db_id;
            var hijos = node.children.length;
            var tipo = node.data.type;
            var parents = node.parents;
            $('#treeView').on('select_node.jstree', function (e, data) {
                var countSelected = data.selected.length;
                if (countSelected > 1) {
                    data.instance.deselect_node([data.selected[0]]);
                }
            });
            _this.Events.bind("OnGetPartial", function () {
                _this.AddParamHrefArray(["#CreateCarpetas", "#CreateProyectos", "#CreateObras", "#CreateContratos", "#CreateBitacoras"], "IdEmpresa=" + $("#IdEmpresa").val());
                _this.ActiveTabs();
                if (hijos > 0) {
                    $("#BtnEliminarCarpeta").addClass("hide");
                    $("#BtnEliminarObra").addClass("hide");
                    $("#BtnEliminarProyecto").addClass("hide");
                }
            });
            if (tipo == "ro") {
                var loadUrl = "/GLOD/HOME/Details";
                _this.getPartial("#DivDetails", loadUrl, {});
                return;
            }
            if (tipo == "carp") {
                _this.Events.bind("OnGetPartial", function () {
                    var tableHelper = new TableHelper('#tablaLogs', false);
                    if (hijos > 0) {
                        $("#BtnEliminarCarpeta").addClass("hide");
                    }
                });
                var loadUrl = "/GLOD/Carpetas/Details";
                var data = { id: _this.nodeID };
                _this.getPartial("#DivDetails", loadUrl, data);
                return;
            }
            if (tipo == "con") {
                _this.Events.bind("OnGetPartial", function () {
                });
                var loadUrl = "/GLOD/Contratos/Details";
                var data = { id: _this.nodeID };
                _this.getPartial("#DivDetails", loadUrl, data);
                return;
            }
            if (tipo == "lib") {
                _this.Events.bind("OnGetPartial", function () {
                });
                var loadUrl = "/GLOD/LibroObras/Details";
                var data = { id: _this.nodeID };
                _this.getPartial("#DivDetails", loadUrl, data);
                return;
            }
            return;
        });
        this.treeView.initTree();
        this.treeView.Events.bind("OnGetTreeData", function () {
            setTimeout(function () {
                $('#treeView').jstree(true).select_node(_tipo + _id);
            }, 1000);
        });
        this.treeView.updateTreeData(this.urlGetTree);
    };
    GLOD_Administracion.prototype.getTree = function (id) {
        this.DesactiveTabs();
        $("#IdEmpresa").val(id);
        this.treeView.Events.bind("OnGetTreeData", function () {
            setTimeout(function () {
                $('#treeView').jstree(true).select_node("t_0_anchor");
            }, 1000);
        });
        this.treeView.updateTreeData(this.urlGetTree);
    };
    GLOD_Administracion.prototype.DesactiveTabs = function () {
        $(".LiTab").css("pointer-events", "none");
        $("a.Tab-Admin").css("cursor", "not-allowed");
    };
    GLOD_Administracion.prototype.ActiveTabs = function () {
        $(".LiTab").css("pointer-events", "all");
        $("a.Tab-Admin").css("cursor", "default");
    };
    GLOD_Administracion.prototype.initModal = function (data, status, xhr) {
        this.modalForm = new ModalHelper("#modalJerarquia");
        $.validator.unobtrusive.parse($("#formJerarquia"));
        this.modalForm.initModal();
        this.modalForm.open();
    };
    GLOD_Administracion.prototype.saveResult = function (data, status, xhr) {
        if (data == 'true') {
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
        this.ActiveTabs();
    };
    GLOD_Administracion.prototype.sleep = function (milliseconds) {
        var start = new Date().getTime();
        for (var i = 0; i < 1e7; i++) {
            if ((new Date().getTime() - start) > milliseconds) {
                break;
            }
        }
    };
    return GLOD_Administracion;
}(GenericController));
