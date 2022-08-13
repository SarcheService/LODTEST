Vue.component('ficha-personal', {
    props: {
        titulo: {
            type: String,
            default: "Ficha del Personal"
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
        this.$root.$on("get-personal-sel", (id) => {
            this.id = id;
            this.refresh();
        });
    },
    methods: {
        refresh: function () {
            axios.get('/Helpers/Fichas/FichaPersonal?id=' + this.id)
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
                                   <img v-if="dataset.foto!==null" v-bind:src="'/Images/Personal/' + dataset.foto" alt="logo" class="img-circle img-profile-galena" >
                                   <h2 v-if="dataset.foto==null" v-bind:data-letters="dataset.dataletters" :class="dataset.dlclass" href="#" class="no-margins" style="vertical-align: middle;"></h2>
                                   <div class="m-t-sm">
                                       <h3 class="m-b-xs">{{dataset.nombre}} {{dataset.apellidos}}</h3>
                                       <div class="text-muted font-bold m-b-xs">{{dataset.cargo}}</div>
                                   </div>
                                   <div class="text-center">
                                       <div class="btn-group">
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
                                                        <strong>Área</strong>
                                                    </td>
                                                    <td>
                                                        {{dataset.unidad}}
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <strong>Faena</strong>
                                                    </td>
                                                    <td>
                                                       {{dataset.faena}}
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <strong>E-mail</strong>
                                                    </td>
                                                    <td>
                                                      {{dataset.email}}
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
                                                        <strong>Anexo</strong>
                                                    </td>
                                                    <td>
                                                           {{dataset.anexo}}
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>`
});