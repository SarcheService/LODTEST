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
var GenericSelectFaena = (function (_super) {
    __extends(GenericSelectFaena, _super);
    function GenericSelectFaena(_InputName, _LabelName) {
        var _this = _super.call(this) || this;
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalSelectTree";
        _this.treeName = "#treeSelectFaena";
        _this.urlGetTree = "/Admin/Faenas/getTree";
        _this.urlGetModal = "/Admin/Faenas/getSelFaena";
        _this.InputName = _InputName;
        _this.LabelName = _LabelName;
        _this.modal = new ModalHelper(_this.modalName);
        return _this;
    }
    GenericSelectFaena.prototype.seleccionarFaena = function (_input, _label) {
        this.getModal({}, { InputName: this.InputName, LabelName: this.LabelName });
    };
    GenericSelectFaena.prototype.deleteFaena = function () {
        $(this.InputName).val(null);
        $(this.LabelName).val('-');
    };
    GenericSelectFaena.prototype.initSearchTree = function (_params) {
        var _this = this;
        this.searchTree = new SearchTreeHelper(this.treeName, "#btnExpSearch_Faena", "#btnColSearch_Faena", "#txtSearch_Faena");
        this.searchTree.Events.bind("OnSelectNode", function (node) {
            var tipo = node.data.type;
            var parents = node.parents;
            var rootNodeId = node.data.db_id;
            if (tipo == "ua" || tipo == 'ro') {
                $("#btnSelectFaena").prop("disabled", true);
                _this.nodeSearchID = null;
                _this.nodeSearchName = '-';
            }
            else {
                $("#btnSelectFaena").prop("disabled", false);
                _this.nodeSearchID = node.data.db_id;
                _this.nodeSearchName = node.text;
            }
            $(_this.InputName).val(_this.nodeSearchID);
            $(_this.LabelName).val(_this.nodeSearchName);
        });
        this.searchTree.initTree();
        this.searchTree.updateTreeData(this.urlGetTree, _params);
    };
    ;
    GenericSelectFaena.prototype.getModal = function (_paramsTree, _paramsModal) {
        var _this = this;
        this.modal.Events.bind("OnGetModal", function () {
            _this.modal.initModal();
            _this.initSearchTree(_paramsTree);
            _this.modal.open();
        });
        this.modal.getModal(this.modalCanvas, this.urlGetModal, _paramsModal);
    };
    return GenericSelectFaena;
}(GenericController));
var GenericSelectUnidadJerarquica = (function (_super) {
    __extends(GenericSelectUnidadJerarquica, _super);
    function GenericSelectUnidadJerarquica(_InputName, _LabelName) {
        var _this = _super.call(this) || this;
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalSelectTree";
        _this.treeName = "#treeSelectJerarquia";
        _this.urlGetTree = "/PER/Jerarquia/getSelectTree";
        _this.urlGetModal = "/PER/Jerarquia/getSelJerarquia";
        _this.modal = new ModalHelper(_this.modalName);
        _this.InputName = _InputName;
        _this.LabelName = _LabelName;
        return _this;
    }
    GenericSelectUnidadJerarquica.prototype.seleccionarUnidad = function (_input, _label) {
        this.getModal({}, { InputName: this.InputName, LabelName: this.LabelName });
    };
    GenericSelectUnidadJerarquica.prototype.deleteUnidad = function () {
        $(this.InputName).val(null);
        $(this.LabelName).val('-');
    };
    GenericSelectUnidadJerarquica.prototype.initSearchTree = function (_params) {
        var _this = this;
        this.searchTree = new SearchTreeHelper(this.treeName, "#btnExpSearch_Jer", "#btnColSearch_Jer", "#txtSearch_Jer");
        this.searchTree.Events.bind("OnSelectNode", function (node) {
            var tipo = node.data.type;
            var parents = node.parents;
            var rootNodeId = node.data.db_id;
            if (tipo == "ua" || tipo == 'ro') {
                $("#btnSelectFaena").prop("disabled", true);
                _this.nodeSearchID = null;
            }
            else {
                $("#btnSelectFaena").prop("disabled", false);
                _this.nodeSearchID = node.data.db_id;
                _this.nodeSearchName = node.text;
                for (var i = 0; i < parents.length; i++) {
                    if (parents[i] != "#") {
                        var n = $(_this.treeName).jstree(true).get_node(parents[i]);
                        if (n["data"].type == "fa") {
                            _this.nodeSearchName = n.text + "/" + node.text;
                            break;
                        }
                        ;
                    }
                }
            }
            $(_this.InputName).val(_this.nodeSearchID);
            $(_this.LabelName).val(_this.nodeSearchName);
        });
        this.searchTree.initTree();
        this.searchTree.updateTreeData(this.urlGetTree, _params);
    };
    ;
    GenericSelectUnidadJerarquica.prototype.getModal = function (_paramsTree, _paramsModal) {
        var _this = this;
        this.modal.Events.bind("OnGetModal", function () {
            _this.modal.initModal();
            _this.initSearchTree(_paramsTree);
            _this.modal.open();
        });
        this.modal.getModal(this.modalCanvas, this.urlGetModal, _paramsModal);
    };
    return GenericSelectUnidadJerarquica;
}(GenericController));
var GenericSelectJerarquia = (function (_super) {
    __extends(GenericSelectJerarquia, _super);
    function GenericSelectJerarquia() {
        var _this = _super.call(this) || this;
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalSelectTree";
        _this.treeName = "#treeSelectJerarquia";
        _this.urlGetTree = "/PER/Jerarquia/getSelectTree";
        _this.urlGetModal = "/PER/Jerarquia/getSelJerarquia";
        _this.modal = new ModalHelper(_this.modalName);
        return _this;
    }
    GenericSelectJerarquia.prototype.initSearchTree = function (_params) {
        var _this = this;
        this.searchTree = new SearchTreeHelper(this.treeName, "#btnExpSearch_Jer", "#btnColSearch_Jer", "#txtSearch_Jer");
        this.searchTree.Events.bind("OnSelectNode", function (node) {
            var tipo = node.data.type;
            var parents = node.parents;
            var rootNodeId = node.data.db_id;
            if (tipo == "ua" || tipo == 'ro') {
                $("#btnSelectFaena").prop("disabled", true);
                _this.nodeSearchID = null;
            }
            else {
                $("#btnSelectFaena").prop("disabled", false);
                _this.nodeSearchID = node.data.db_id;
                _this.nodeSearchName = node.text;
                for (var i = 0; i < parents.length; i++) {
                    if (parents[i] != "#") {
                        var n = $(_this.treeName).jstree(true).get_node(parents[i]);
                        if (n["data"].type == "fa") {
                            _this.nodeSearchName = n.text + "/" + node.text;
                            break;
                        }
                        ;
                    }
                }
            }
        });
        this.searchTree.initTree();
        if (_params != "0") {
            this.searchTree.Events.bind("OnGetTreeData", function () {
                var NodoSelecionado = "t_" + _params;
                setTimeout(function () { $('#treeSelectJerarquia').jstree(true).select_node(NodoSelecionado); }, 300);
            });
        }
        this.searchTree.updateTreeData(this.urlGetTree, _params);
    };
    ;
    GenericSelectJerarquia.prototype.getModal = function (_paramsTree, _paramsModal) {
        var _this = this;
        this.modal.Events.bind("OnGetModal", function () {
            _this.modal.initModal();
            _this.initSearchTree(_paramsTree);
            _this.modal.open();
        });
        this.modal.getModal(this.modalCanvas, this.urlGetModal, _paramsModal);
    };
    return GenericSelectJerarquia;
}(GenericController));
var GenericSelectMarcaModelo = (function (_super) {
    __extends(GenericSelectMarcaModelo, _super);
    function GenericSelectMarcaModelo(_InputName, _LabelName) {
        var _this = _super.call(this) || this;
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalSelectTree";
        _this.treeName = "#treeSelectMarcaModelo";
        _this.urlGetTree = "/Admin/MarcaModelo/getSelectTree";
        _this.urlGetModal = "/Admin/MarcaModelo/getSelMarcaModelo";
        _this.InputName = _InputName;
        _this.LabelName = _LabelName;
        _this.OriginalInputVal = $(_this.InputName).val();
        _this.OriginalLabelVal = $(_this.LabelName).val();
        _this.modal = new ModalHelper(_this.modalName);
        return _this;
    }
    GenericSelectMarcaModelo.prototype.seleccionarModelo = function (_input, _label) {
        this.getModal({}, { InputName: this.InputName, LabelName: this.LabelName });
    };
    GenericSelectMarcaModelo.prototype.deleteModelo = function () {
        $(this.InputName).val(this.OriginalInputVal);
        $(this.LabelName).val(this.OriginalLabelVal);
    };
    GenericSelectMarcaModelo.prototype.selectModel = function () {
        $(this.InputName).val(this.TemporalInputVal);
        $(this.LabelName).val(this.TemporalLabelVal);
    };
    GenericSelectMarcaModelo.prototype.initSearchTree = function (_params) {
        var _this = this;
        this.searchTree = new SearchTreeHelper(this.treeName, "#btnExpSearch_Model", "#btnColSearch_Model", "#txtSearch_Model");
        this.searchTree.Events.bind("OnSelectNode", function (node) {
            var tipo = node.data.type;
            var parents = node.parents;
            var rootNodeId = node.data.db_id;
            if (tipo == "ua" || tipo == 'ro' || tipo == 'ma') {
                $("#btnSelectMarcaModelo").prop("disabled", true);
                _this.nodeSearchID = null;
            }
            else {
                $("#btnSelectMarcaModelo").prop("disabled", false);
                _this.nodeSearchID = node.data.db_id;
                _this.nodeSearchName = node.text;
                for (var i = 0; i < parents.length; i++) {
                    if (parents[i] != "#") {
                        var n = $(_this.treeName).jstree(true).get_node(parents[i]);
                        if (n["data"].type == "ma") {
                            _this.nodeSearchName = n.text + "/" + node.text;
                            break;
                        }
                        ;
                    }
                }
            }
            _this.TemporalInputVal = _this.nodeSearchID;
            _this.TemporalLabelVal = _this.nodeSearchName;
        });
        this.searchTree.initTree();
        this.searchTree.updateTreeData(this.urlGetTree, _params);
    };
    ;
    GenericSelectMarcaModelo.prototype.getModal = function (_paramsTree, _paramsModal) {
        var _this = this;
        this.modal.Events.bind("OnGetModal", function () {
            _this.modal.initModal();
            _this.initSearchTree(_paramsTree);
            _this.modal.open();
        });
        this.modal.getModal(this.modalCanvas, this.urlGetModal, _paramsModal);
    };
    return GenericSelectMarcaModelo;
}(GenericController));
var GenericSelectClaseActivo = (function (_super) {
    __extends(GenericSelectClaseActivo, _super);
    function GenericSelectClaseActivo(_InputName, _LabelName) {
        var _this = _super.call(this) || this;
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalSelectTree";
        _this.treeName = "#treeSelectClase";
        _this.urlGetTree = "/GAE/Clase/getTree";
        _this.urlGetModal = "/GAE/Clase/getSelClase";
        _this.InputName = _InputName;
        _this.LabelName = _LabelName;
        _this.OriginalInputVal = $(_this.InputName).val();
        _this.OriginalLabelVal = $(_this.LabelName).val();
        _this.modal = new ModalHelper(_this.modalName);
        return _this;
    }
    GenericSelectClaseActivo.prototype.seleccionarClase = function (_input, _label) {
        this.getModal({}, { InputName: this.InputName, LabelName: this.LabelName });
    };
    GenericSelectClaseActivo.prototype.deleteClase = function () {
        $(this.InputName).val(this.OriginalInputVal);
        $(this.LabelName).val(this.OriginalLabelVal);
    };
    GenericSelectClaseActivo.prototype.selectClase = function () {
        $(this.InputName).val(this.TemporalInputVal);
        $(this.LabelName).val(this.TemporalLabelVal);
    };
    GenericSelectClaseActivo.prototype.initSearchTree = function (_params) {
        var _this = this;
        this.searchTree = new SearchTreeHelper(this.treeName, "#btnExpSearch_Clase", "#btnColSearch_Clase", "#txtSearch_Clase");
        this.searchTree.Events.bind("OnSelectNode", function (node) {
            var tipo = node.data.type;
            var parents = node.parents;
            var rootNodeId = node.data.db_id;
            if (tipo == "ua" || tipo == 'ro') {
                $("#btnSelectClase").prop("disabled", true);
                _this.nodeSearchID = null;
            }
            else {
                $("#btnSelectClase").prop("disabled", false);
                _this.nodeSearchID = node.data.db_id;
                _this.nodeSearchName = node.text;
                for (var i = 0; i < parents.length; i++) {
                    if (parents[i] != "#") {
                        var n = $(_this.treeName).jstree(true).get_node(parents[i]);
                        if (n["data"].type == "clase") {
                            _this.nodeSearchName = n.text + "/" + node.text;
                            break;
                        }
                        ;
                    }
                }
            }
            _this.TemporalInputVal = _this.nodeSearchID;
            _this.TemporalLabelVal = _this.nodeSearchName;
        });
        this.searchTree.initTree();
        this.searchTree.updateTreeData(this.urlGetTree, _params);
    };
    ;
    GenericSelectClaseActivo.prototype.getModal = function (_paramsTree, _paramsModal) {
        var _this = this;
        this.modal.Events.bind("OnGetModal", function () {
            _this.modal.initModal();
            _this.initSearchTree(_paramsTree);
            _this.modal.open();
        });
        this.modal.getModal(this.modalCanvas, this.urlGetModal, { InputName: this.InputName, LabelName: this.LabelName });
    };
    return GenericSelectClaseActivo;
}(GenericController));
var GenericSelectIcono = (function (_super) {
    __extends(GenericSelectIcono, _super);
    function GenericSelectIcono() {
        var _this = _super.call(this) || this;
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalSelectIcono";
        _this.urlGetModal = "/Intranet/Configuraciones/getSelIcono";
        _this.modal = new ModalHelper(_this.modalName);
        return _this;
    }
    GenericSelectIcono.prototype.getModal = function (_paramsModal) {
        var _this = this;
        this.modal.Events.bind("OnGetModal", function () {
            _this.modal.initModal();
            _this.modal.open();
        });
        this.modal.getModal(this.modalCanvas, this.urlGetModal, _paramsModal);
    };
    GenericSelectIcono.prototype.icono = function (e) {
        $(".icono-div").removeClass("icono-div-active");
        e.parent("div").addClass("icono-div-active");
        var clase = e.children("i").attr("class");
        $("#btnSelectIcono").prop("disabled", false).data('clase', clase);
    };
    GenericSelectIcono.prototype.seleccionar = function (e) {
        var indice = $("#indiceInput").val();
        var clase = e.data('clase');
        $("input[name='BotonIcono'][data-indice-input='" + indice + "']").val(clase).trigger("change");
        ;
        this.modal.close();
    };
    return GenericSelectIcono;
}(GenericController));
var GenericSelectCarpetasDoc = (function (_super) {
    __extends(GenericSelectCarpetasDoc, _super);
    function GenericSelectCarpetasDoc(_InputName, _LabelName, _TipoObject) {
        var _this = _super.call(this) || this;
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalSelectTree";
        _this.treeName = "#treeSelectCarpetasDoc";
        _this.urlGetTree = "/Admin/Paths/getTreeCarpetasDoc";
        _this.urlGetModal = "/Admin/Paths/getSelCarpetasDoc";
        _this.InputName = _InputName;
        _this.LabelName = _LabelName;
        _this.OriginalInputVal = $(_this.InputName).val();
        _this.OriginalLabelVal = $(_this.LabelName).val();
        _this.TipoObject = _TipoObject;
        _this.modal = new ModalHelper(_this.modalName);
        return _this;
    }
    GenericSelectCarpetasDoc.prototype.seleccionarCarpetasDoc = function (_input, _label) {
        this.getModal({}, { InputName: this.InputName, LabelName: this.LabelName });
    };
    GenericSelectCarpetasDoc.prototype.deleteCarpetasDoc = function () {
        $(this.InputName).val(this.OriginalInputVal);
        $(this.LabelName).val(this.OriginalLabelVal);
    };
    GenericSelectCarpetasDoc.prototype.selectCarpetasDoc = function () {
        $(this.InputName).val(this.TemporalInputVal);
        $(this.LabelName).val(this.TemporalLabelVal);
    };
    GenericSelectCarpetasDoc.prototype.initSearchTree = function (_params) {
        var _this = this;
        this.searchTree = new SearchTreeHelper(this.treeName, "#btnExpSearch_CarpetasDoc", "#btnConSearch_CarpetasDoc", "#treeSelectCarpetasDoc");
        this.searchTree.Events.bind("OnSelectNode", function (node) {
            var tipo = node.data.type;
            var parents = node.parents;
            var rootNodeId = node.data.db_id;
            if (tipo == 'ro' || tipo == 'ti') {
                $("#btnSelectCarpetasDoc").prop("disabled", true);
                _this.nodeSearchID = null;
                _this.nodeSearchName = '-';
            }
            else {
                $("#btnSelectCarpetasDoc").prop("disabled", false);
                _this.nodeSearchID = node.data.db_id;
                _this.nodeSearchName = node.text;
                for (var i = 0; i < parents.length; i++) {
                    if (parents[i] != "#") {
                        var n = $(_this.treeName).jstree(true).get_node(parents[i]);
                        if (n["data"].type == "fa") {
                            _this.nodeSearchName = n.text + "/" + node.text;
                            break;
                        }
                        ;
                    }
                }
            }
            _this.TemporalInputVal = _this.nodeSearchID;
            _this.TemporalLabelVal = _this.nodeSearchName;
        });
        this.searchTree.initTree();
        this.searchTree.updateTreeData(this.urlGetTree, _params);
        console.log("entro");
    };
    ;
    GenericSelectCarpetasDoc.prototype.getModal = function (_paramsTree, _paramsModal) {
        var _this = this;
        _paramsTree = { TipoObject: this.TipoObject };
        this.modal.Events.bind("OnGetModal", function () {
            _this.modal.initModal();
            _this.initSearchTree(_paramsTree);
            _this.modal.open();
        });
        this.modal.getModal(this.modalCanvas, this.urlGetModal, _paramsModal);
    };
    return GenericSelectCarpetasDoc;
}(GenericController));
