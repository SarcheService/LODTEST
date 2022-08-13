Vue.component('select-dropdown', {
    name: "select-dropdown",
    props: {
        id: String,
        tipo: String,
        titulo: {
            type: String,
            default: null
        },
        placeholder: {
            type: String,
            default: 'Seleccione una Opción'
        },
        nodata: {
            type: String,
            default: 'No se encontraron coincidencias...'
        },
        idselected: {
            type: Number,
            default: null
        },
        selected: {
            type: String,
            default: null
        },
        readonly: {
            type: Boolean,
            default: false
        },
        em: {
            type: Array,
            default: function () {
                return []
            }
        },
        on: {
            type: Array,
            default: function () {
                return []
            }
        },
    },
    data() {
        return {
            etiqueta: this.selected,
            primarikey: this.idselected,
            dataset: [],
            isSearch: false,
            temp: this.selected
        }
    },
    computed: {
        label: function () {
            if (this.titulo == null) {
                return "Selecionar " + this.tipo;
            } else {
                return this.titulo;
            }
        }
    },
    watch: {
        etiqueta: function () {
            this.debouncedGetAnswer()
        }
    },
    created: function () {
        this.debouncedGetAnswer = _.debounce(this.onChange, 300)
    },
    methods: {
        onClick: function (v,t) {
            this.isSearch = false;
            this.primarikey = v;
            this.etiqueta = t;
            this.temp = t;
            this.em.forEach(element => this.$root.$emit(element, this.primarikey));
        },
        reset: function (event) {
            this.isSearch = false;
            this.primarikey = this.idselected;
            this.etiqueta = this.selected;
            this.temp = this.selected;
            this.em.forEach(element => this.$root.$emit(element, 0));
        },
        search: function (event) {
            this.dataset = [];
            this.isSearch = true;
            this.$refs.searchbar.focus();
        },
        onChange: function (event) {
            if (this.isSearch == true) {
                axios.get("/Helpers/SelectDrop/Get" + this.tipo + "?param=" + this.etiqueta)
                    .then((response) => {
                        this.dataset = response.data;
                    })
                    .catch(function (error) {
                        console.error(error);
                    });
            }
        },
        onBlur: function (event) {

            if (this.primarikey == null) {
                this.primarikey = this.idselected;
                this.etiqueta = this.selected;
            } else {
                this.etiqueta = this.temp;
            }
               
            this.isSearch = false;
            
        },
    },
    template: `<div class="row">
                   <div class="col-sm-12 m-b">
                       <label class="control-label">{{label}}</label>
                       <input v-bind:id="id" v-bind:name="id" v-bind:value="primarikey" type="hidden">
                       <div class="input-group col-md-12 col-sm-12">
                           <span class="input-group-btn">
                               <button v-if="readonly" type="button" class="btn btn-warning btn-sm" disabled><i class="fa fa-search" aria-hidden="true"></i></button>
                               <button v-if="!readonly" @click="search" type="button" class="btn btn-warning btn-sm"><i class="fa fa-search" aria-hidden="true"></i></button>
                               <button v-if="!readonly" @click="reset" type="button" class="btn btn-default btn-sm"><i class="fa fa-times" aria-hidden="true"></i></button>
                           </span> 
                        <input @blur="onBlur" :placeholder="placeholder" v-model:value="etiqueta" :readonly="readonly || !isSearch" class="form-control input-sm text-box single-line" ref="searchbar" type="text">
                       </div>
                <transition mode="out-in" appear appear-active-class="animated flipInX" enter-active-class="animated flipInX" leave-active-class="animated flipOutX">
                       <div v-if="isSearch" class="div-tree-item dropdown-menu open">
                            <center><p class="m-t-sm" v-if="dataset.length<=0">{{this.nodata}}</p></center>
                           <ul class="ul-dropdown-item">
                               <transition-group name="tranitem" tag="li" mode="out-in" appear appear-active-class="animated fadeIn faster" enter-active-class="animated fadeIn faster">
                                <li v-for="item in dataset" class="m-b-xs li-dropdown-item" :key="item.value" @click="onClick(item.value,item.text)">
                                    <div class="media">
                                        <div class="media-left media-top">
                                           <h5 v-if="item.image==null && item.shortext!=null" :data-letters="item.shortext" :class="item.dataletters" href="#" class="no-margins" style="vertical-align: middle;padding-top:3px;"></h5>
                                           <img v-if="item.image!=null" :src="item.image" alt="logo" style="width:36px;max-height:36px;margin-top:4px;">
                                        </div>
                                          <div class="media-body">
                                            <h5 class="m-b-none">{{item.text}}</h5>
                                            {{item.description}}
                                          </div>
                                    </div>
                                </li>
                            </transition-group >
                           </ul>
                       </div>
             </transition>
                   </div>
               </div>`
});
