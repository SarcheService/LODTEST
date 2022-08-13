Vue.component('data-table-json', {
    name: "data-table-json",
    props: {
        tableid: String,
        getUrl: String,
        nodata: {
            type: String,
            default: 'No se encontraron registros para mostrar.'
        },
    },
    data() {
        return {
            search: '',
            selected: 10,
            tableRows: [],
            columns: [],
            PaginaActual: 1,
            TotalPaginas: 1,
            PaginaInicial: 0,
            PaginaFinal: 0,
            ListPaginas: []
        }
    },
    mounted() {

        this.$root.$on(this.tableid + "_search", (search) => {
            this.onSearch(search);
        });

        this.$root.$on(this.tableid + "_paginate", (cant) => {
            this.refreshData(cant);
        });
        
        this.$root.$on(this.tableid + "_refresh", (rows) => {
            this.tableRows = rows;
            this.clearRows();
            this.refreshPagination(rows.length);
            this.changePage(1);
            this.ifNoData();
        });

        this.$root.$on(this.tableid + "_GetData", () => {
            this.onGetData();
        });

        this.$on(this.tableid + "_GetData", () => {
            this.onGetData();
        });

        this.onGetData();
       
    },
    methods: {
        onSearch: function (search) {
            //console.log('onSearch');
            let searchResult = Array.from(this.tableRows)
                .filter(chapter => chapter.innerText.toLowerCase().includes(search.toLowerCase()))

            this.PaginaActual = 1;
            this.clearRows();
            this.refreshPagination(searchResult.length);

            let skip = 0;
            var newRowsArray = searchResult.slice(skip, searchResult.length);

            var tableRef = document.getElementById(this.tableid).getElementsByTagName('tbody')[0];
            let maxRows = (this.selected <= newRowsArray.length) ? this.selected : newRowsArray.length;
            for (let index = 0; index < maxRows; index++) {
                tableRef.insertRow().innerHTML = newRowsArray[index].outerHTML;
            }

            this.ifNoData();

        },
        refreshData: function (selected) {

            this.selected = selected;
            this.PaginaActual = 1;
            this.clearRows();
            this.refreshPagination(this.tableRows.length);
                       
            var tableRef = document.getElementById(this.tableid).getElementsByTagName('tbody')[0];
            let maxRows = (this.selected <= this.tableRows.length) ? this.selected : this.tableRows.length;

            for (let index = 0; index < maxRows; index++) {
                tableRef.insertRow().innerHTML = this.tableRows[index].outerHTML;
            }

            this.ifNoData();
          
        },
        refreshPagination: function (totalRows) {

            let totalPaginas = parseInt(totalRows);
            let cantidadPaginas = Math.ceil(totalPaginas / this.selected);
            this.TotalPaginas = cantidadPaginas;
            let Inicial = 1;
            let radio = 3;
            let cantidadMaximaDePaginas = radio * 2 + 1;
            let Final = (cantidadPaginas > cantidadMaximaDePaginas) ? cantidadMaximaDePaginas : cantidadPaginas;
            if (this.PaginaActual > radio + 1) {
                Inicial = this.PaginaActual - radio;
                if (cantidadPaginas > this.PaginaActual + radio) {
                    Final = this.PaginaActual + radio;
                }
                else {
                    Final = cantidadPaginas;
                }
            }
            //console.log(Inicial);
            this.PaginaInicial = Inicial;
            this.PaginaFinal = Final;
            this.ListPaginas = [];
            for (var i = Inicial; i <= Final; i++) {
                this.ListPaginas.push(i);
            }
            //console.log(this.ListPaginas);

        },
        changePage: function (index) {

            if (index > this.TotalPaginas || index<1)
                return false;

            this.PaginaActual = index;
            this.clearRows();

            let skip = ((index - 1) * this.selected);
            var newRowsArray = this.tableRows.slice(skip, this.tableRows.length);

            var tableRef = document.getElementById(this.tableid).getElementsByTagName('tbody')[0];
            let maxRows = (this.selected <= newRowsArray.length) ? this.selected : newRowsArray.length;
            for (let index = 0; index < maxRows; index++) {
                tableRef.insertRow().innerHTML = newRowsArray[index].outerHTML;
            }
            this.refreshPagination(this.tableRows.length);
            this.ifNoData();
            
        },
        clearRows: function () {
            let table = document.getElementById(this.tableid);
            let rowCount = table.rows.length;
            for (var i = 1; i < rowCount; i++) {
                table.deleteRow(1);
            }
        },
        ifNoData: function () {
            var tableRef = document.getElementById(this.tableid).getElementsByTagName('tbody')[0];
            if (tableRef.rows.length <= 0) {
                tableRef.insertRow().innerHTML = '<tr class="text-center"><td colspan="' + this.columns.length + '"><center>' + this.nodata + '</center></td></tr>';
            }
        },
        onGetData: function () {
            axios.get(this.getUrl)
                .then((response) => {
                    this.tableRows = response.data;
                })
                .catch(function (error) {
                    console.error(error);
                });
        }
    },
});