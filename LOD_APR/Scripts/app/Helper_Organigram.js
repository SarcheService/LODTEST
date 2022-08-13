var Helper_Organigram = (function () {
    function Helper_Organigram(_Organigram) {
        this.OrganigramName = _Organigram;
        this.Events = new EventsHelper();
    }
    Helper_Organigram.prototype.getColorNodeTitle = function (estado) {
        if (!estado) {
            return 'style="background-color:#9c9c9c"';
        }
    };
    Helper_Organigram.prototype.getColorNodeContent = function (estado) {
        if (!estado) {
            return 'style="border-color:#9c9c9c"';
        }
    };
    Helper_Organigram.prototype.initOrganigram = function () {
        var _this = this;
        var nodeTemplate = function (data) {
            var space = (data.espacio) ? 0 : 90;
            return "<div class=\"nodeclick\" title=\"Unidad " + data.name + ":" + data.title + "\">\n            <div class=\"title\" " + _this.getColorNodeTitle(data.estado) + (">" + data.name + "</div>\n            " + ((data.title != null) ? '<div class="content-orgchart" ' + _this.getColorNodeContent(data.estado) + '><b>' + data.title + '</b></div>' : '') + "\n            </div>");
        };
        this.Organigram = $(this.OrganigramName).orgchart({
            'nodeContent': 'title',
            'nodeTemplate': nodeTemplate,
            'parentNodeSymbol': false,
            'pan': true,
            'zoom': true,
            'zoominLimit': 1.2,
            'zoomoutLimit': 0.6,
            'createNode': function ($node, data) {
                if (data.childrenSoporte != null) {
                    var orientacion = "-", StyleOrientacion = "";
                    var h = 12.5;
                    var i = 0, j = data.childrenSoporte.length;
                    var _loop_1 = function (index) {
                        if (i == 2 || i == 3) {
                            if (i == 2) {
                                h = h + 87, 5;
                            }
                            if (i == 2) {
                                i = 0;
                            }
                        }
                        if (orientacion == "-") {
                            orientacion = "";
                        }
                        else {
                            orientacion = "-";
                        }
                        var element = data.childrenSoporte[index];
                        assistantNode = '<div id="nodeSop' + element.id + '" style="padding: ' + h + 'px 0px 0px 3px;z-index:' + j + '" title="' + element.name + ':' + element.title + '" id="nodeSop' + element.id + '" class="assistant-node' + orientacion + '"><div ' + StyleOrientacion + ' style="height:' + h + 'px" class="connector' + orientacion + '"/><div class="title" ' + _this.getColorNodeTitle(element.estado) + '>' + element.name + '<i class="fa fa-user-circle-o symbol"></i></div><div class="content-orgchart" ' + _this.getColorNodeContent(element.estado) + '><b>' + element.title + '</b></div><i class="edge verticalEdge bottomEdge fa"></i></div>';
                        $node.append(assistantNode);
                        $node.on('dblclick', '#nodeSop' + element.id, function () {
                            _this.Events.call("NodeDblclick", element);
                        });
                        $node.on('click', '#nodeSop' + element.id, function () {
                            _this.Events.call("NodeClick", element);
                        });
                        i++;
                        j--;
                    };
                    var assistantNode;
                    for (var index = 0; index < data.childrenSoporte.length; index++) {
                        _loop_1(index);
                    }
                }
                $node.on('dblclick', '.nodeclick', function () {
                    _this.Events.call("NodeDblclick", data);
                });
                $node.on('click', '.nodeclick', function () {
                    _this.Events.call("NodeClick", data);
                });
            },
            'initCompleted': function ($chart) {
                var $container = $('#chart-container');
                $container.scrollLeft(($container[0].scrollWidth - $container.width()) / 2);
                $('.orgchart').addClass('noncollapsable');
                $('#chart-container').css("overflow", "auto");
                _this.Events.call("initCompletedOrg");
            }
        });
        this.Events.call("OnInitOrgData");
    };
    Helper_Organigram.prototype.imprimirOrganigrama = function (elemento) {
        elemento = elemento.replace("#", "");
        var a = document.getElementById(elemento);
        var contenido = document.getElementById(elemento).innerHTML;
        var contenidoOriginal = document.body.innerHTML;
        document.body.innerHTML = contenido;
        window.print();
        document.body.innerHTML = contenidoOriginal;
        return true;
    };
    Helper_Organigram.prototype.updateOrganigramData = function (_urlGetTree, _params) {
        var _this = this;
        if (_params === void 0) { _params = {}; }
        var result_;
        $.ajax({
            url: _urlGetTree,
            async: true,
            type: 'GET',
            data: _params,
            beforeSend: function () {
                $(".panel-load").addClass("sk-loading");
            },
            success: function (result) {
                var result_ = JSON.parse(result);
                _this.Organigram.init({ 'data': result_ });
                _this.Events.call("OnGetOrgData");
                $(".panel-load").removeClass("sk-loading");
            }
        });
    };
    return Helper_Organigram;
}());
