Vue.component('ficha-sujeto', {
    props: {
        titulo: {
            type: String,
            default: "Ficha de Empresa"
        },
        id: {
            type: Number,
            default:0
        }
    },
    data() {
        return {
            dataset: {}
        }
    },
    mounted() {
        
        this.refresh();
        this.$root.$on("get-sujeto-sel", (id) => {
            this.id = id;
            this.refresh();
        });
    },
    methods: {
        refresh: function () {
            axios.get('/Fichas/FichaSujeto?id=' + this.id)
                .then(response => (this.dataset = response.data))
                .catch(function (error) {
                    console.error(error);
                });
        }
    },
    template: `<div class="hpanel">
                               <div class="panel-heading hbuilt">
                                   <div class="row">
                                       <div class="col-xs-8">
                                           <span>{{titulo}}</span>
                                       </div>
                                       <div class="col-xs-4 pull-right">
                                           <label v-if="dataset.estado" class="label label-success pull-right">Activo</label>
                                           <label v-if="!dataset.estado" class="label label-danger pull-right">Inactivo</label>
                                        </div>
                                   </div>
                               </div>
                               <div class="panel-body text-center panel-load">
                                <center>
                                   <img v-if="dataset.foto!==null" v-bind:src="'/Images/Sujetos/' + dataset.foto" alt="logo" class="img-responsive" style="max-width:240px" >
                                   <h2 v-if="dataset.foto==null" v-bind:data-letters="dataset.dataletters" :class="dataset.dlclass" href="#" class="no-margins" style="vertical-align: middle;"></h2>
                                   <div class="m-t-sm">
                                       <h3 class="m-b-xs">{{dataset.razon}}</h3>
                                   </div>
                                </center>
                                   <div class="text-center">
                                       <div class="btn-group">
                                           <a v-if="dataset.web!=null" v-bind:href="dataset.web" class="btn btn-info btn-xs" target="_blank"><i class="fa fa-globe"></i></a>
                                           <a v-if="dataset.email!=null" v-bind:href="'mailto:' + dataset.email" class="btn btn-default btn-xs" target="_blank"><i class="fa fa-envelope-o"></i></a>
                                           <a v-if="dataset.isfacebook" v-bind:href="dataset.urlfacebook" class="btn btn-info btn-xs" target="_blank"><i class="fa fa-facebook"></i></a>
                                           <a v-if="dataset.istwitter" v-bind:href="dataset.urltwitter" class="btn btn-default btn-xs" target="_blank"><i class="fa fa-twitter"></i></a>
                                           <a v-if="dataset.islinkedin" v-bind:href="dataset.urllinkedin" class="btn btn-primary btn-xs" target="_blank"><i class="fa fa-linkedin"></i></a>
                                       </div>
                                   </div>
                                    <div class="text-left m-t-md">
                                        <table class="table table-hover small m-b-xs">
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <strong>Rut</strong>
                                                    </td>
                                                    <td>
                                                       {{dataset.rut}}
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <strong>N. Fantasía</strong>
                                                    </td>
                                                    <td>
                                                        {{dataset.fantasia}}
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <strong>Giro</strong>
                                                    </td>
                                                    <td>
                                                      {{dataset.giro}}
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <strong>Dirección</strong>
                                                    </td>
                                                    <td>
                                                        {{dataset.direccion}}
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <strong>Ciudad</strong>
                                                    </td>
                                                    <td>
                                                         {{dataset.ciudad}}
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <strong>Tel.Contacto</strong>
                                                    </td>
                                                    <td>
                                                      {{dataset.telefono}}
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <strong>Es Cliente</strong>
                                                    </td>
                                                    <td v-if="dataset.escliente">Sí</td>
                                                    <td v-if="!dataset.escliente">No</td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <strong>Es Proveedor</strong>
                                                    </td>
                                                    <td v-if="dataset.esproveedor">Sí</td>
                                                    <td v-if="!dataset.esproveedor">No</td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <strong>Es Contratista</strong>
                                                    </td>
                                                    <td v-if="dataset.escontratista">Sí</td>
                                                    <td v-if="!dataset.escontratista">No</td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <strong>Es Organización</strong>
                                                    </td>
                                                    <td v-if="dataset.esorganizacion">Sí</td>
                                                    <td v-if="!dataset.esorganizacion">No</td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <strong>Es Emp. Relacionada</strong>
                                                    </td>
                                                    <td v-if="dataset.esrelacionada">Sí</td>
                                                    <td v-if="!dataset.esrelacionada">No</td>
                                                </tr>
                                               
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>`
});