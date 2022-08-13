class TableHelper{
    private tableName:string;
    private Table:any;
    

    constructor(_tableName:string,_customSearch?:boolean, _Cache:boolean = true, _Order:boolean = true){
        this.tableName=_tableName;
        

        if(_customSearch!=null && _customSearch==true){
            
            
            
            this.Table= $(_tableName).DataTable(
                {
                    "sDom": 'ltipr', 
                    stateSave: _Cache, 
                    "bSort": _Order,
                    language: this.language()
                    

                }
            );

            $("#searchInput").keyup(()=>{
                this.Table.search($("#searchInput").val()).draw() ;
            });
            $("#btnTableSearch").click(function() {
                $("#searchInput").keyup();
            });

        }else{
            this.Table= $(_tableName).DataTable(
                { 
                    stateSave: _Cache, 
                    "bSort": _Order,
                    language: this.language()
                }
            );
        }

    }


    public language(){
        return  {
            "sProcessing":     "Procesando...",
            "sLengthMenu":     "Mostrar _MENU_ registros",
            "sZeroRecords":    "No se encontraron resultados",
            "sEmptyTable":     "No se encontraron datos para mostrar",
            "sInfo":           "Mostrando _START_ al _END_ de  _TOTAL_ registros",
            "sInfoEmpty":      "Mostrando 0 al 0 de 0 registros",
            "sInfoFiltered":   "(filtrado de un total de _MAX_ registros)",
            "sInfoPostFix":    "",
            "sSearch":         "Buscar:",
            "sUrl":            "",
            "sInfoThousands":  ",",
            "sLoadingRecords": "Cargando...",
            "oPaginate": {
                "sFirst":    "Primero",
                "sLast":     "Último",
                "sNext":     "Siguiente",
                "sPrevious": "Anterior"
            }
        };
    }

}


