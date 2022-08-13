var TableHelper = (function () {
    function TableHelper(_tableName, _customSearch, _Cache, _Order) {
        var _this = this;
        if (_Cache === void 0) { _Cache = true; }
        if (_Order === void 0) { _Order = true; }
        this.tableName = _tableName;
        if (_customSearch != null && _customSearch == true) {
            this.Table = $(_tableName).DataTable({
                "sDom": 'ltipr',
                stateSave: _Cache,
                "bSort": _Order,
                language: this.language()
            });
            $("#searchInput").keyup(function () {
                _this.Table.search($("#searchInput").val()).draw();
            });
            $("#btnTableSearch").click(function () {
                $("#searchInput").keyup();
            });
        }
        else {
            this.Table = $(_tableName).DataTable({
                stateSave: _Cache,
                "bSort": _Order,
                language: this.language()
            });
        }
    }
    TableHelper.prototype.language = function () {
        return {
            "sProcessing": "Procesando...",
            "sLengthMenu": "Mostrar _MENU_ registros",
            "sZeroRecords": "No se encontraron resultados",
            "sEmptyTable": "No se encontraron datos para mostrar",
            "sInfo": "Mostrando _START_ al _END_ de  _TOTAL_ registros",
            "sInfoEmpty": "Mostrando 0 al 0 de 0 registros",
            "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
            "sInfoPostFix": "",
            "sSearch": "Buscar:",
            "sUrl": "",
            "sInfoThousands": ",",
            "sLoadingRecords": "Cargando...",
            "oPaginate": {
                "sFirst": "Primero",
                "sLast": "Último",
                "sNext": "Siguiente",
                "sPrevious": "Anterior"
            }
        };
    };
    return TableHelper;
}());
