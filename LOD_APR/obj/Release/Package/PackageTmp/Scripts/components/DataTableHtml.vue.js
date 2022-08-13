Vue.component('data-table', {
    name: "data-table",
    props: {
        tableid: String,
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

        //events from others components
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
            //this.refreshData(1);
            //console.log('emmit refreshData');


        });

        let table = document.getElementById(this.tableid);
        let i = 0;
        for (let row of table.rows) {

            if (i > 0)
                this.tableRows.push(row);

            for (let cel of row.cells) {
                if (cel.localName == 'th')
                    this.columns.push(cel.innerText);
            }

            i++;
        }

        this.refreshData(this.selected);
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
    },
});

Vue.component('data-table-paginador', {
    name: "data-table-paginador",
    template: `<div class="row">
                    <div class="col-md-12">
                        <div class="row">
                            <div class="col-md-12">
                                <p class="text-muted m-b-none">Mostrando página {{PaginaActual}} de {{TotalPaginas}} páginas. <label class="label label-info"> {{TotalRegistros}} registros totales</label></p>
                            </div>
                        </div>
                        <ul class="pagination m-t-xs pull-right">
                            <li :class="[{disabled : PaginaActual==1} ,'paginate_button next']">
                                <a role="button" @click="$emit('click',1)">Primera</a>
                            </li>
                            <li :class="[{disabled : PaginaActual==1} ,'paginate_button next']">
                                <a role="button" @click="$emit('click',PaginaActual-1)">Anterior</a>
                            </li>
                            <li v-for="index in ListPaginas" :class="[{ active: PaginaActual==index }, 'paginate_button']">
                                <a role="button" @click="$emit('click',index)">{{index}}</a>
                            </li>
                            <li :class="[{disabled : PaginaActual==TotalPaginas} ,'paginate_button next']">
                                <a role="button" @click="$emit('click',PaginaActual+1)">Siguiente</a>
                            </li>
                            <li :class="[{disabled : PaginaActual==TotalPaginas} ,'paginate_button next']">
                                <a role="button" @click="$emit('click',TotalPaginas)">Última</a>
                            </li>
                        </ul>
                    </div>
                </div>`,
    props: {
        PaginaActual: Number,
        PaginaFinal: Number,
        TotalPaginas: Number,
        TotalRegistros: Number,
        ListPaginas: {
            type: Array,
            default: []
        },
    },
})

Vue.component('data-table-top', {
    name: "data-table-top",
    template: `<div class="row">
                    <div class="col-sm-5">
                        <label v-if="showPaginate" style="display:inline-block;font-weight: normal;">
                            Mostrar
                            <select v-model="selected" @change="$emit('change', selected)" class="form-control input-sm" style="display:inline-block;width:75px">
                                <option v-for="reg in regbypage" :key="reg" :value="reg">{{reg}}</option>
                            </select> Registros
                        </label>
                    </div>
                    <div class="col-sm-1"></div>
                    <div class="col-sm-6">
                        <div v-if="showSearch" class="input-group m-b col-md-12 col-sm-12">
                           <span class="input-group-btn">
                               <button @click="onReset" type="button" class="btn btn-default"><i class="fa fa-times" aria-hidden="true"></i></button>
                           </span> 
                        <input v-model="search" type="text" class="form-control" placeholder="Buscar en la Tabla">
                       </div>
                    </div>
                </div>`,
    data() {
        return {
            selected: 10,
            search: ''
        }
    },
    props: {
        regbypage: {
            type: Array,
            default: function () {
                return [10, 25, 50, 100]
            }
        }, 
        showSearch: {
            type: Boolean,
            default: true
        },
        showPaginate: {
            type: Boolean,
            default: true
        }
    },
    watch: {
        search: function () {
            this.debouncedGetAnswer()
        }
    },
    created: function () {
        this.debouncedGetAnswer = _.debounce(this.onSearch,300)
    },
    methods: {
        onSearch: function () {
            //console.log("search");
            this.$emit('search', this.search);
        },
        onReset: function () {
            this.search = '';
        }
    }
})

Vue.component('data-table-search', {
    name: "data-table-search",
    template: `<div class="row">
                    <div class="col-sm-12">
                        <div class="input-group">
                           <span class="input-group-btn">
                               <button @click="onReset" type="button" class="btn btn-default"><i class="fa fa-times" aria-hidden="true"></i></button>
                           </span> 
                           <input v-model="search" type="text" class="form-control" placeholder="Buscar en la Tabla">
                       </div>
                    </div>
                </div>`,
    data() {
        return {
            search: ''
        }
    },
    props: {
        tableid: String
    },
    watch: {
        search: function () {
            this.debouncedGetAnswer()
        }
    },
    created: function () {
        this.debouncedGetAnswer = _.debounce(this.onSearch, 300)
    },
    methods: {
        onSearch: function () {
            this.$root.$emit(this.tableid + '_search', this.search);
        },
        onReset: function () {
            this.search = '';
        }
    }
})

Vue.component('data-table-numreg', {
    name: "data-table-numreg",
    template: `<div class="row">
                    <div class="col-sm-12">
                        <label style="display:inline-block;font-weight: normal;">
                            Mostrar
                            <select v-model="selected" @change="change" class="form-control input-sm" style="display:inline-block;width:75px">
                                <option v-for="reg in regbypage" :key="reg" :value="reg">{{reg}}</option>
                            </select> Registros
                        </label>
                    </div>
                </div>`,
    data() {
        return {
            selected: 10
        }
    },
    props: {
        tableid: String,
        regbypage: {
            type: Array,
            default: function () {
                return [10, 25, 50, 100]
            }
        }
    },
    methods: {
        change: function () {
            this.$root.$emit(this.tableid + '_paginate', this.selected);
        }
    }
})

Vue.component('button-submit-filter', {
    name: "button-submit-filter",
    props: {
        tableId: String
    },
    computed: {
        formid: function () {
            return 'form_' + this.tableId;
        }
    },
    methods: {
        onSubmit: function () {
          
            let myForm = document.getElementById(this.formid);
            let formData = new FormData(myForm);
            //const data = {};
          
            //var file = document.querySelector('#archivos');
            //formData.append("archivo", file.files[0]);
            //console.log(formData)

            //for (let [key, val] of formData.entries()) {
            //    Object.assign(data, { [key]: val })
            //}
            //console.log(data)

            axios.post(myForm.action, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            })
                .then((response) => {
                    let doc = document.createElement('html');
                    doc.innerHTML = response.data;
                    let table = doc.querySelector('table');
                    let tableRows = [];
                    let i = 0;
                    for (let row of table.rows) {
                        if (i > 0)
                            tableRows.push(row);
                        i++;
                    }
                    
                    this.$root.$emit(this.tableId + '_refresh', tableRows);
                })
                .catch(function (error) {
                    console.error(error);
                });
        },
    },
    template: `<button class="btn btn-sm btn-info btn-group-justified" data-style="zoom-in" type="submit" @click='onSubmit'><i class="fa fa-filter"></i> Filtrar</button>`
})