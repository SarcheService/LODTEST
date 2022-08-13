var TreeHelper = (function () {
    function TreeHelper(_treeName) {
        this.treeName = _treeName;
        this.Events = new EventsHelper();
        this.expandBtnName = '#btnExpand';
        this.collapseBtnName = '#btnCollapse';
        this.searchInputName = '#treesearch';
    }
    TreeHelper.prototype.initTree = function () {
        var _this = this;
        $(this.treeName).jstree({
            'core': {
                "check_callback": function (op, node, par, pos, more) {
                    return true;
                }, 'data': []
            },
            'search': {
                'case_insensitive': false,
                'show_only_matches': true
            },
            'plugins': ["search", "sort", "dnd", "state"],
        });
        if (this.Events.on("OnSelectNode")) {
            $(this.treeName).on("select_node.jstree", function (evt, data) {
                _this.nodeData = data.node;
                _this.Events.call("OnSelectNode", data.node);
            });
        }
        var to = false;
        $(this.searchInputName).keyup(function () {
            if (to) {
                clearTimeout(to);
            }
            to = setTimeout(function () {
                var v = $(_this.searchInputName).val();
                $(_this.treeName).jstree(true).search(v);
            }, 500);
        });
        $(this.collapseBtnName).click(function () {
            $(_this.treeName).jstree('close_all');
        });
        $(this.expandBtnName).click(function () {
            $(_this.treeName).jstree('open_all');
        });
        this.Events.call("OnInitTree");
    };
    TreeHelper.prototype.updateTreeData = function (_urlGetTree, _params) {
        var _this = this;
        if (_params === void 0) { _params = {}; }
        $.ajax({
            url: _urlGetTree,
            data: _params,
            type: 'GET',
            success: function (result) {
                var lstNodes = JSON.parse(result);
                $(_this.treeName).jstree(true).settings.core.data = lstNodes;
                $(_this.treeName).jstree(true).refresh();
                _this.Events.call("OnGetTreeData");
            },
            error: function (xhr, status, error) {
                var alert = new AlertHelper();
                alert.toastErrorData(xhr.responseText);
            }
        });
    };
    return TreeHelper;
}());
var TreeHelperAdmin = (function () {
    function TreeHelperAdmin(_treeName) {
        this.treeName = _treeName;
        this.Events = new EventsHelper();
        this.expandBtnName = '#btnExpand';
        this.collapseBtnName = '#btnCollapse';
        this.searchInputName = '#treesearch';
    }
    TreeHelperAdmin.prototype.initTree = function () {
        var _this = this;
        $(this.treeName).jstree({
            'core': {
                "check_callback": function (op, node, par, pos, more) {
                    return true;
                }, 'data': []
            },
            'search': {
                'case_insensitive': false,
                'show_only_matches': true
            },
            'plugins': ["search", "sort", "state", "contextmenu", "wholerow"],
            "contextmenu": {
                "items": function ($node) {
                    return {
                        "Cut": {
                            "separator_before": false,
                            "separator_after": false,
                            "label": "Cortar",
                            "_disabled": _this.DisableCutContext(),
                            "action": function (obj) {
                                _this.CutNode = $node;
                                $(_this.treeName).jstree().cut($node);
                            }
                        },
                        "Paste": {
                            "separator_before": false,
                            "separator_after": false,
                            "label": _this.GetPasteContextName(),
                            "_disabled": _this.DisablePasteContext(),
                            "action": function (obj) {
                                $(_this.treeName).jstree().paste($node);
                            }
                        }
                    };
                }
            }
        });
        if (this.Events.on("OnSelectNode")) {
            $(this.treeName).on("select_node.jstree", function (evt, data) {
                _this.nodeData = data.node;
                _this.Events.call("OnSelectNode", data.node);
            });
        }
        var to = false;
        $(this.searchInputName).keyup(function () {
            if (to) {
                clearTimeout(to);
            }
            to = setTimeout(function () {
                var v = $(_this.searchInputName).val();
                $(_this.treeName).jstree(true).search(v);
            }, 500);
        });
        $(this.collapseBtnName).click(function () {
            $(_this.treeName).jstree('close_all');
        });
        $(this.expandBtnName).click(function () {
            $(_this.treeName).jstree('open_all');
        });
        this.Events.call("OnInitTree");
    };
    TreeHelperAdmin.prototype.GetCutNode = function () {
        return this.CutNode;
    };
    TreeHelperAdmin.prototype.DisableCutContext = function () {
        var tipoDest = this.SelectedNode.data.type;
        if (tipoDest == "ro") {
            return true;
        }
        else {
            return false;
        }
    };
    TreeHelperAdmin.prototype.DisablePasteContext = function () {
        if (!$(this.treeName).jstree().can_paste()) {
            return true;
        }
        var tipo = this.CutNode.data.type;
        var tipoDest = this.SelectedNode.data.type;
        var result = false;
        if (tipo == "proy") {
            if (tipoDest == "proy" || tipoDest == "obra" || tipoDest == "con" || tipoDest == "bit") {
                result = true;
                console.log("Un " + tipo + " No se puede pegar en: " + tipoDest);
            }
        }
        else if (tipo == "obra") {
            if (tipoDest == "obra" || tipoDest == "con" || tipoDest == "bit") {
                result = true;
                console.log("Un " + tipo + " No se puede pegar en: " + tipoDest);
            }
        }
        else if (tipo == "carp") {
            if (tipoDest == "con" || tipoDest == "bit") {
                result = true;
                console.log("Un " + tipo + " No se puede pegar en: " + tipoDest);
            }
        }
        else if (tipo == "con") {
            if (tipoDest == "con" || tipoDest == "bit") {
                result = true;
                console.log("Un " + tipo + " No se puede pegar en: " + tipoDest);
            }
        }
        else if (tipo == "bit") {
            if (tipoDest == "con" || tipoDest == "bit") {
                result = true;
                console.log("Un " + tipo + " No se puede pegar en: " + tipoDest);
            }
        }
        return result;
    };
    TreeHelperAdmin.prototype.GetPasteContextName = function () {
        if (!$(this.treeName).jstree().can_paste()) {
            return "Pegar Aquí";
        }
        var tipo = this.CutNode.data.type;
        var tipoDest = this.SelectedNode.data.type;
        var result = "Pegar " + this.CutNode.text + " Aquí";
        if (tipo == "proy") {
            if (tipoDest == "proy" || tipoDest == "obra" || tipoDest == "con" || tipoDest == "bit") {
                result = "Este Proyecto No se puede pegar aquí";
            }
        }
        else if (tipo == "obra") {
            if (tipoDest == "obra" || tipoDest == "con" || tipoDest == "bit") {
                result = "La Obra " + this.CutNode.text + " No se puede pegar aquí";
            }
        }
        else if (tipo == "carp") {
            if (tipoDest == "con" || tipoDest == "bit") {
                result = "La Carpeta " + this.CutNode.text + " No se puede pegar aquí";
            }
        }
        else if (tipo == "con") {
            if (tipoDest == "con" || tipoDest == "bit") {
                result = "El Contrato " + this.CutNode.text + " No se puede pegar aquí";
            }
        }
        else if (tipo == "bit") {
            if (tipoDest == "con" || tipoDest == "bit") {
                result = "La Bitácora " + this.CutNode.text + " No se puede pegar aquí";
            }
        }
        return result;
    };
    TreeHelperAdmin.prototype.updateTreeData = function (_urlGetTree, _params) {
        var _this = this;
        if (_params === void 0) { _params = {}; }
        $.ajax({
            url: _urlGetTree,
            data: _params,
            type: 'GET',
            success: function (result) {
                var lstNodes = JSON.parse(result);
                $(_this.treeName).jstree(true).settings.core.data = lstNodes;
                $(_this.treeName).jstree(true).refresh();
                _this.Events.call("OnGetTreeData");
            },
            error: function (xhr, status, error) {
                var alert = new AlertHelper();
                alert.toastErrorData(xhr.responseText);
            }
        });
    };
    return TreeHelperAdmin;
}());
var SearchTreeHelper = (function () {
    function SearchTreeHelper(_treeName, _expandBtnName, _collapseBtnName, _searchInputName) {
        this.Events = new EventsHelper();
        this.treeName = _treeName;
        this.expandBtnName = _expandBtnName;
        this.collapseBtnName = _collapseBtnName;
        this.searchInputName = _searchInputName;
    }
    SearchTreeHelper.prototype.initTree = function () {
        var _this = this;
        $(this.treeName).jstree({
            'search': {
                'case_insensitive': false,
                'show_only_matches': true
            },
            'plugins': ["search", "sort", "state"],
        });
        if (this.Events.on("OnSelectNode")) {
            $(this.treeName).on("select_node.jstree", function (evt, data) {
                _this.nodeData = data.node;
                _this.Events.call("OnSelectNode", data.node);
            });
        }
        var to = false;
        $(this.searchInputName).keyup(function () {
            if (to) {
                clearTimeout(to);
            }
            to = setTimeout(function () {
                var v = $(_this.searchInputName).val();
                $(_this.treeName).jstree(true).search(v);
            }, 500);
        });
        $(this.collapseBtnName).click(function () {
            $(_this.treeName).jstree('close_all');
        });
        $(this.expandBtnName).click(function () {
            $(_this.treeName).jstree('open_all');
        });
    };
    SearchTreeHelper.prototype.updateTreeData = function (_urlGetTree, _data) {
        var _this = this;
        $.ajax({
            url: _urlGetTree,
            data: _data,
            type: 'GET',
            success: function (result) {
                var lstNodes = JSON.parse(result);
                $(_this.treeName).jstree(true).settings.core.data = lstNodes;
                $(_this.treeName).jstree(true).refresh();
                _this.Events.call("OnGetTreeData");
            },
            error: function (xhr, status, error) {
                var alert = new AlertHelper();
                alert.toastErrorData(xhr.responseText);
            }
        });
    };
    return SearchTreeHelper;
}());
