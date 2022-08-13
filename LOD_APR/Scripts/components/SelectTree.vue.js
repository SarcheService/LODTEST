Vue.component('select-tree', {
    name:"select-tree",
    props: {
        id: String,
        tipo: Number, //0- Jerarquia; 1-Faena; 2-Marca/Modelo; 3-Clase Activo; 4-Paths; 5-Contratos
        titulo: {
            type: String,
            default: function () {
                var t = 'Seleccionar ';
                var a = ['Jerarquía', 'Faena', 'Modelo', 'Clase de Activo', 'Carpeta', 'Contrato']
                return t + a[this.tipo];
            } 
        },
        idselected: {
            type: Number,
            default: null
        },
        selected: {
            type: String,
            default: function () {
                var t = 'Seleccione ';
                var a = ['una Unidad Jerárquica', 'una Faena', 'un Modelo', 'una Clase de Activo', 'una Carpeta', 'un Contrato']
                return t + a[this.tipo];
            } 
        },
        readonly: {
            type: Boolean,
            default: false
        },
        tipopath: {
            type: String,
            required: false
            }
    },
    data() {
        return {
            etiqueta: this.selected,
            primarikey: this.idselected,
            treeData: {},
            isOpen: true,
            isSearch: false
        }
    },
    computed: {
        isFolder: function () {
            return this.treeData.children &&
                this.treeData.children.length
        },
        isApply: function () {
            return (this.isSearch && this.primarikey!=null)
        }
    },
    template: `<div class="row">
                   <div class="col-sm-12 m-b">
                       <label class="control-label">{{titulo}}</label>
                       <input v-bind:id="id" v-bind:name="id" v-bind:value="primarikey" type="hidden">
                       <div class="input-group col-md-12 col-sm-12">
                           <span class="input-group-btn">
                               <button v-if="!readonly" @click="search" type="button" class="btn btn-warning btn-sm"><i class="fa fa-search" aria-hidden="true"></i></button>
                               <button v-if="!readonly" @click="reset" type="button" class="btn btn-default btn-sm"><i class="fa fa-times" aria-hidden="true"></i></button>
                               <button v-if="isApply" @click="apply" type="button" class="btn btn-success btn-sm"><i class="fa fa-check" aria-hidden="true"></i> Aplicar</button>
                           </span> 
                        <input class="form-control input-sm text-box single-line" v-bind:value="etiqueta" type="text" readonly>
                       </div>
                <transition mode="out-in" appear appear-active-class="animated flipInX" enter-active-class="animated flipInX" leave-active-class="animated flipOutX">
                       <div v-if="isSearch" class="div-tree-item dropdown-menu open">
                           <ul class="ul-tree-item">
                               <li>
                                   <div :class="{'txt-bold': isFolder}" class="tree-item" @click="toggle">
                                        <span v-if="isFolder">[{{ isOpen ? '-' : '+' }}]</span>                                      
                                        <i :class="treeData.icon"></i> {{ treeData.text }}
                                    </div>
                                    <ul v-show="isOpen" v-if="isFolder">
                                      <select-tree-item
                                        class="item ul-tree-item"
                                        v-for="(child, index) in treeData.children"
                                        :key="index"
                                        :item="child"
                                        :tipo="tipo"
                                        :primarikey="primarikey"
                                        @clicked="onClickChild"
                                      ></select-tree-item>
                                    </ul>
                               </li>
                           </ul>
                       </div>
             </transition>
                   </div>
               </div>`,
    methods: {
        toggle: function () {
            if (this.isFolder) {
                this.isOpen = !this.isOpen
            }
        },
        onClickChild: function (event) {
            this.primarikey = event[0];
            this.etiqueta = event[1];
            this.$root.$emit('select_GetTipoDoc', event[0])
        },
        reset: function (event) {
            this.isSearch = false;
            this.primarikey = this.idselected;
            this.etiqueta = this.selected;
        },
        apply: function (event) {
            this.isSearch = false;
        },
        search: function (event) {
            var a = ['GetTreeJerarquia', 'GetTreeFaenas', 'GetTreeMarcas', 'GetTreeClases', 'GetTreePaths', 'GetTreeContratos']
            axios.get('/SelectTree/' + a[this.tipo])
                .then((response) =>{
                    this.treeData = response.data;
                })
                .catch(function (error) {
                    console.error(error);
                });
            this.isSearch = true;
        }
    }
});

Vue.component('select-tree-item', {
    name:"select-tree-item",
    template: `<li>
                   <div :class= "{bold: isFolder, 'selected-tree-item': isSelected}" class="tree-item" @click="toggle">
                       <span v-if="isFolder">[{{ isOpen? '-' : '+' }}]</span>                       
                       <i :class="item.icon"></i> {{ item.text }} <i v-if="isSelected" class="fa fa-check text-success"></i>
                   </div >
                   <ul v-show="isOpen" v-if="isFolder">
                        <select-tree-item
                            class="item ul-tree-item"
                            v-for="(child, index) in item.children"
                            :key="index"
                            :item="child"
                            :tipo="tipo"
                            :primarikey="primarikey"
                            @clicked="$emit('clicked', $event)">
                         </select-tree-item>
                   </ul>
               </li>`,
    props: {
        item: Object,
        primarikey: Number,
        tipo: Number
    },
    data: function () {
        return {
            isOpen: true
        }
    },
    computed: {
        isFolder: function () {
            return this.item.children &&
                this.item.children.length
        },
        isSelected: function () {
            return (this.primarikey == this.item.data.db_id);
        }
    },
    methods: {
        toggle: function (element) {
            if (this.isFolder) 
                this.isOpen = !this.isOpen

            var emit = false;

            //0- Jerarquia
            if (this.tipo == 0 && (this.item.data.type != "ro"))
                emit = true;
            //1-Faena
            if (this.tipo == 1 && (this.item.data.type != "ro" && this.item.data.type != "ua"))
                emit = true;
            //2-Marca/modelo
            if (this.tipo == 2 && this.item.data.type == "mo")
                emit = true;
            //3-Clase Activo
            if (this.tipo == 3 && this.item.data.type == "clase")
                emit = true;
            //4-Paths
            if (this.tipo == 4 && this.item.data.type == "two")
                emit = true;
            //5-Contratos
            if (this.tipo == 5 && this.item.data.type == "con")
                emit = true;
            
            if (emit)
                this.$emit('clicked', [this.item.data.db_id, this.item.text])
        }
    }
})
