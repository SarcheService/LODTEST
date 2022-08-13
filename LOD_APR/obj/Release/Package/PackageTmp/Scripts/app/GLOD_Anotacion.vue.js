
Vue.component('glod-anotacion', {
    name: "glod-anotacion",
    props: {
        idlibro: Number,
        tabla: String
    },
    data() {
        return {
            filtrorapido: {},
            rango: null,
            isfiltrorapido: false,
            showSpinner: false,
            IdBandeja: 1,
            TituloBandeja: ['Bandeja Principal', 'Mis Publicaciones', 'Mis Borradores', 'Mis Destacadas', 'Nombrado en', 'Mis Firmas Pendientes', 'Mis Respuestas Pendientes']
        }
    },
    computed: {
        Titulo: function () {
            return this.TituloBandeja[this.IdBandeja-1];
        }
    },
    mounted: function () {
        this.getStats(this.idlibro);
    },
    methods: {
        onFiltroRapido: function (id, idlibro) {
            this.showSpinner = true;
            axios.get('/GLOD/Anotaciones/GetFiltroRapido/' + id + '?idLibro=' + idlibro)
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
                    };
                    this.IdBandeja = id;
                    this.showSpinner = false;
                    //console.log('onFiltroRapido');
                    this.$root.$emit(this.tabla + '_refresh', tableRows);
                })
                .catch(function (error) {
                    console.error(error);
                });
        },
        getStats: function (IdLibro) {
            axios.get('/GLOD/Anotaciones/GetUserStats?idLibro=' + IdLibro)
                .then((response) => {
                    this.filtrorapido = response.data
                })
                .catch(function (error) {
                    console.error(error);
                });
        }
    }
});

Vue.component('select-tipo-anotacion', {
    name: 'select-tipo-anotacion',
    props: {
        IdLibro: Number
    },
    data() {
        return {
            TipoSelected: '',
            LstTipos: [],
            LstSubtipos: []
        }
    },
    mounted() {
        this.getTipos(this.IdLibro);
    },
    methods: {
        getTipos: function (IdLibro) {
            //console.log(IdLibro);
            axios.get('/GLOD/Anotaciones/GetTipos?id=' + IdLibro).then((response) => {
                this.LstTipos = response.data;
            })
                .catch(function (error) {
                    console.error(error);
                });
        },
        getSubTipos: function (event) {
            console.log(event.target.value);
            axios.get('/GLOD/Anotaciones/GetSubTipos?id=' + event.target.value).then((response) => {
                this.LstSubtipos = response.data;
            })
                .catch(function (error) {
                    console.error(error);
                });
        },
    },
    template: `<div class="row">
        <div class="col-md-12">
            <label class="control-label" for="IdTipos">Tipo Anotación</label>
            <select @change="getSubTipos($event)" v-model="TipoSelected" name="IdTipos" class="form-control" data-live-search="true">
                <option v-for="item in LstTipos" :value="item.Value" :key="item.Value">
                    {{ item.Text }}
                </option>
            </select>
        </div>
        <div class="col-md-12">
            <label class="control-label" for="IdTipoSub">Subtipo Anotación</label>
            <select name="IdTipoSub" class="form-control" data-live-search="true">
                <option v-for="item in LstSubtipos" :value="item.Value" :key="item.Value">
                    {{ item.Text }}
                </option>
            </select>
        </div>
    </div>`
});

Vue.component('glod-detalle-anotacion', {
    name: "glod-detalle-anotacion",
    props: {
        IdAnotacion: Number
    },
    data() {
        return {
            anotacion: {
                FirmaDescript: {},
                Remitente: {},
                FirmaDescript: {},
                UsuarioActual: {},
                EstadoAnotacion: {},
                Receptores: [],
                EstadoFirma: { IsFirmada: true},
                FechaResp: {},
                Referencias: []
            },
            EditMode: false,
            showSpinner: false
        }
    },
    computed: {
        vistosBuenos: function () {
            let recVB = this.anotacion.Receptores.filter(user => user.VistoBueno == true);
            return recVB;
        }
    },
    mounted: function () {

        this.$on("GetReceptores", () => {
            this.getReceptores();
        });

        this.$on("GetAnotacionData", () => {
            this.GetAnotacionData();
        });

        this.GetAnotacionData();
    },
    watch: {
        EditMode: function () {
            this.debouncedGetAnswer()
        }
    },
    created: function () {
        this.debouncedGetAnswer = _.debounce(this.refreshSummerNote, 200)
    },
    methods: {
        GetAnotacionData: function (id) {
            this.showSpinner = true;
            axios.get('/GLOD/Anotaciones/GetAnotacionData/' + this.IdAnotacion)
                .then((response) => {
                    this.anotacion = response.data;
                    this.showSpinner = false;
                    //console.log(this.anotacion);
                })
                .catch(function (error) {
                    console.error(error);
                });
        },
        editAnotacion: function () {
            this.EditMode = true;
            //this.anotacion.SolicitudRest = false;
            //console.log('Editando...');
        },
        refreshSummerNote: function () {
            $('#CuerpoAnotacion').summernote({
                height: 150,
                toolbar: [
                    ['style', ['bold', 'italic', 'underline', 'clear']],
                    ['font', ['strikethrough', 'superscript', 'subscript']],
                    ['fontsize', ['fontsize']],
                    ['color', ['color']],
                    ['para', ['ul', 'ol', 'paragraph']],
                    ['height', ['height']]
                ]
            })
        },
        saveAnotacion: function () {
            this.EditMode = false;
            this.showSpinner = true;
            this.anotacion.Cuerpo = jQuery('#CuerpoAnotacion').code()

            var x = document.getElementsByClassName("note-editor");
            while (x.length > 0) {
                x[0].parentNode.removeChild(x[0]);
            }

            axios.post('/GLOD/Anotaciones/Update/', this.anotacion)
                .then((response) => {
                    this.showSpinner = false;
                })
                .catch(function (error) {
                    console.error(error);
                });
        },
        cancelUpdate: function () {
            this.GetAnotacionData();
            this.EditMode = false;
            var x = document.getElementsByClassName("note-editor");
            while (x.length > 0) {
                x[0].parentNode.removeChild(x[0]);
            }
        },
        setDestacar: function () {
            this.anotacion.UsuarioActual.EsDestacada = !this.anotacion.UsuarioActual.EsDestacada
            this.showSpinner = true;
            axios.post('/GLOD/Anotaciones/SetDestacarAnotacion/', { id: this.IdAnotacion, estado: this.anotacion.UsuarioActual.EsDestacada })
                .then((response) => {
                    this.showSpinner = false;
                })
                .catch(function (error) {
                    console.error(error);
                });
        },
        notBeforeToday: function (date) {
            const today = new Date();
            today.setHours(0, 0, 0, 0);

            return date < today;// || date > new Date(today.getTime() + 7 * 24 * 3600 * 1000);
        },
        getReceptores: function () {
            console.log('Entró a getReceptores!');
            this.showSpinner = true;
            axios.get('/GLOD/Anotaciones/GetReceptoresData/' + this.IdAnotacion)
                .then((response) => {
                    this.anotacion.Receptores = response.data;
                    this.showSpinner = false;
                    console.log('Se recopilaron los datos de los receptores!');
                })
                .catch(function (error) {
                    console.error(error);
                });
        },
        getLog: function () {
            this.$root.$emit("dtLogsAnotacion_GetData");
        }
    }
});

Vue.component('select-tipo-firma', {
    name: 'select-tipo-firma',
    props: {
        FirmasPermitidas: Array,
        IdAnotacion: Number,
        EsTomaConocimiento: Boolean
    },
    data() {
        return {
            selected: 1,
            code: null,
            esperar: false,
            FormValidate: { ErrorMessage:[]}
        }
    },
    computed: {
        UrlVerifySignState: function () {
            if (this.EsTomaConocimiento) {
                return '/GLOD/Anotaciones/GetVBState?id=' + this.IdAnotacion + '&code=' + this.code;
            } else {
                return '/GLOD/Anotaciones/GetSignState/' + this.IdAnotacion;
            }
        },
        UrlForm: function () {
            if (!this.EsTomaConocimiento) {
                return '/GLOD/Anotaciones/FirmarAnotacion'
            } else {
                return '/GLOD/Anotaciones/VBAnotacion'
            }
        }
    },
    methods: {
        onChange: function (e) {
            this.selected = e;
        },
        inArray: function (valor) {  //función para corroborar que tiene permiso de firma
            console.log(valor);
            console.log(this.FirmasPermitidas);
            console.log(this.FirmasPermitidas.length);
            var length = this.FirmasPermitidas.length;
            for (var i = 0; i < length; i++) {
                if (this.FirmasPermitidas[i] == valor) return true;
            }
            return false;
        },
         GenerarCodigo: function () {
            this.esperar = true;
        },
        submitForm: function () {
            let myForm = document.getElementById('form_firma_simple');

            let formData = new FormData(myForm);
            this.esperar = true;

            axios.post(myForm.action, formData).then((response) => {
                this.esperar = false;
                this.FormValidate = response.data;
                let href = response.data.Parametros;

                if (this.FormValidate.Status) {
                    if (this.selected == 1) {
                        this.code = href;
                        this.waitForSign();
                    } else {
                        window.location.href = href;
                    }
                };
            })
                .catch(function (error) {
                    console.error(error);
                    this.ProgressBarValue = 0;
                    this.IsSending = false;
                });
        },
        waitForSign: function () {

            setInterval(() => {

                axios.get(this.UrlVerifySignState).then((response) => {
                    let formValidate = response.data;
                    if (formValidate.Status) {
                        window.location.href = '/GLOD/Anotaciones/Edit/' + this.IdAnotacion;
                    }
                })
                    .catch(function (error) {
                        console.error(error);
                        this.ProgressBarValue = 0;
                        this.IsSending = false;
                    });

            }, 5000);

        }
    }, //se cambio este tempalte agregando el v-if="inArray()"
    template: `<div class="row">
                    <div class="col-md-12"> 
                        <center>
                            <div data-toggle="buttons" class="btn-group">
                                    <label v-if="inArray(1)" class="btn btn-default active" @click="onChange(1)">
                                        <i class="fa fa-certificate"></i>
                                        <br />
                                        <input @click="onChange($event)" type="radio" id="option1" name="tipo" value="1" v-model="selected"> Firma <br />Electrónica Avanzada
                                    </label>
                                
                                    <label v-if="inArray(2)" class="btn btn-default" @click="onChange(2)">
                                        <i class="fa fa-building"></i>
                                        <br />
                                        <input type="radio" id="option2" name="tipo" value="2" v-model="selected"> Firma <br />MINSEGPRE
                                    </label>
                                
                                    <label  v-if="inArray(3)" class="btn btn-default" @click="onChange(3)">
                                        <i class="fa fa-user"></i>
                                        <br />
                                        <input type="radio" id="option3" name="tipo" value="3" v-model="selected"> Firma <br /> Electrónica Simple
                                    </label>
                            </div>
                        </center>
                    </div>
        <div v-if="FormValidate.ErrorMessage.length>0" class="col-sm-12 m-t-lg">
            <center>
                <ul>
                    <li v-for="error in FormValidate.ErrorMessage" class="text-danger">{{error}}</li>
                </ul>
            </center>
        </div>
        <div v-if="inArray(2) && selected=='2'" class="col-md-8 col-md-offset-2 m-t-md">
            <center>
                <label class="control-label text-info m-b-sm">Segundo Factor de Validación OTP</label>
                <form id="form_firma_simple" :action="UrlForm" method="post" enctype="multipart/form-data" v-on:submit.prevent="submitForm">
                    <input type="hidden" name="tipo" value="2" >
                    <input type="hidden" name="IdAnotacion" v-model="IdAnotacion" >
                    <div class="input-group m-t-sm">
                        <input type="text" name="Password" class="form-control input-sm" required />
                        <span class="input-group-btn">
                            <button type="submit" class="btn btn-sm btn-success"><i v-if="esperar" class="fa fa-refresh fa-spin fa-fw"></i><i v-if="!esperar"  class="fa fa-check"></i> Firmar Anotación</button>
                        </span>
                    </div>
                </form>
            </center>
        </div>
                    <div v-if="inArray(3) && selected=='3'" class="col-md-8 col-md-offset-2 m-t-md">
                        <center>
                            <label class="control-label text-info m-b-sm">Este tipo de Firma requiere su Password</label>
                <form id="form_firma_simple" :action="UrlForm" method="post" enctype="multipart/form-data" v-on:submit.prevent="submitForm">
                    <input type="hidden" name="tipo" value="3" >
                    <input type="hidden" name="IdAnotacion" v-model="IdAnotacion" >
                    <div class="input-group m-t-sm">
                        <input type="password" name="Password" class="form-control input-sm" required />
                        <span class="input-group-btn">
                            <button type="submit" class="btn btn-sm btn-success"><i v-if="esperar" class="fa fa-refresh fa-spin fa-fw"></i><i v-if="!esperar"  class="fa fa-check"></i> Firmar Anotación</button>
                        </span>
                    </div>
                </form>
            </center>
        </div>
        <div v-if="inArray(1) && selected=='1'" class="col-md-8 col-md-offset-2 m-t-md">
            <center>
                <label class="control-label text-info m-b-sm">Código FEA</label><br />
                <form id="form_firma_simple" :action="UrlForm" method="post" enctype="multipart/form-data" v-on:submit.prevent="submitForm">
                    <input type="hidden" name="tipo" value="1" >
                    <input type="hidden" name="IdAnotacion" v-model="IdAnotacion" >
                    <button v-if="!code" :disabled="esperar" @click="GenerarCodigo" type="submit" role="button" class="btn btn-info btn-sm m-t-sm" title="Generar Código y Firmar"><i v-if="esperar" class="fa fa-refresh fa-spin fa-fw"></i><i v-if="!esperar"  class="fa fa-check"></i> Generar Código y Firmar</button>
                    <h1 v-if="code">{{code}}</h1>
                </form>
                <hr />
                <h2><i v-if="code" class="fa fa-refresh fa-spin fa-fw"></i></h2>
                <h3 v-if="code">Esperando Firma...</h3>
                        </center>
                    </div>

                </div>`
});

Vue.component('select-referencia-anotacion', {
    name: 'select-referencia-anotacion',
    props: {
        IdContrato: Number
    },
    data() {
        return {
            TipoSelected: '',
            LstTipos: [],
            LstSubtipos: []
        }
    },
    mounted() {
        this.getTipos(this.IdContrato);
    },
    methods: {
        getTipos: function (IdContrato) {
            console.log(IdContrato);
            axios.get('/GLOD/LibroObras/GetLibrosContrato/' + IdContrato).then((response) => {
                this.LstTipos = response.data;
            })
                .catch(function (error) {
                    console.error(error);
                });
        },
        getSubTipos: function (event) {
            console.log(event.target.value);
            axios.get('/GLOD/LibroObras/GetAnotaciones/' + event.target.value).then((response) => {
                this.LstSubtipos = response.data;
            })
                .catch(function (error) {
                    console.error(error);
                });
        },
    },
    template: `<div class="row">
        <div class="col-md-12">
            <label class="control-label" for="IdLibro">Seleccione un Libro</label>
            <select @change="getSubTipos($event)" v-model="TipoSelected" name="IdLibro" class="form-control" data-live-search="true">
                <option v-for="item in LstTipos" :value="item.Value" :key="item.Value">
                    {{ item.Text }}
                </option>
            </select>
        </div>
        <div class="col-md-12">
            <label class="control-label" for="IdAnotacionRef">Seleccione un Folio</label>
            <select name="IdAnotacionRef" class="form-control" data-live-search="true">
                <option v-for="item in LstSubtipos" :value="item.Value" :key="item.Value">
                    {{ item.Text }}
                </option>
            </select>
        </div>
    </div>`
});

Vue.component('select-tipo-doc', {
    name: 'select-tipo-doc',
    props: {
        //IdContrato: Number
    },
    data() {
        return {
            //TipoSelected: '',
            LstTipos: [],
           //LstSubtipos: []
        }
    },
    mounted() {
        this.$root.$on('select_GetTipoDoc', (idTwo) => {
            this.getTipos(idTwo);
        })  
    },
    methods: {
        getTipos: function (IdTwo) {
            console.log(IdContrato);
            axios.get('/GLOD/AnotDocs/GetTipoDocumento/' + IdTwo).then((response) => {
                this.LstTipos = response.data;
            })
                .catch(function (error) {
                    console.error(error);
                });
        },
    },
    template: `<div class="row">
        <div class="col-md-12">
            <label class="control-label" for="IdLibro">Seleccione un Tipo de Documento</label>
            <select name="IdTipoDoc" class="form-control" data-live-search="true">
                <option v-for="item in LstTipos" :value="item.Value" :key="item.Value">
                    {{ item.Text }}
                </option>
            </select>
        </div>
    </div>`
});

Vue.component('select-responder-anotacion', {
    name: 'select-responder-anotacion',
    props: {
        IdContrato: Number
    },
    data() {
        return {
            IdLibro: 0,
            IdTipo: 0,
            IdSubtipo: 0,
            LstLibros: [],
            LstTipos: [],
            LstSubtipos: []
        }
    },
    computed: {
        //IdLibro: function () {
        //    return parseInt(this.TipoSelected);
        //}
    },
    mounted() {
        this.getLibros(this.IdContrato);
    },
    methods: {
        getLibros: function (IdContrato) {
            console.log(IdContrato);
            axios.get('/GLOD/LibroObras/GetLibrosContrato/' + IdContrato).then((response) => {
                this.LstLibros = response.data;
            })
                .catch(function (error) {
                    console.error(error);
                });
        },
        getTipoAnot: function (event) {
            axios.get('/GLOD/Anotaciones/GetTipos/' + event.target.value).then((response) => {
                this.LstTipos = response.data;
            })
                .catch(function (error) {
                    console.error(error);
                });
        },
        getSubTipoAnot: function (event) {
            console.log(event.target.value);
            axios.get('/GLOD/Anotaciones/GetSubTipos/' + event.target.value).then((response) => {
                this.LstSubtipos = response.data;
            })
                .catch(function (error) {
                    console.error(error);
                });
        },
    },
    template: `<div class="row">
        <div class="col-md-12">
            <label class="control-label" for="IdLibro">Seleccione un Libro</label>
            <select @change="getTipoAnot($event)" v-model="IdLibro" name="IdLibro" class="form-control" data-live-search="true">
                <option v-for="item in LstLibros" :value="item.Value" :key="item.Value">
                    {{ item.Text }}
                </option>
            </select>
        </div>
        <div class="col-md-12">
            <label class="control-label" for="IdTipos">Tipo Anotación</label>
            <select @change="getSubTipoAnot($event)" v-model="IdTipo" name="IdTipos" class="form-control" data-live-search="true">
                <option v-for="item in LstTipos" :value="item.Value" :key="item.Value">
                    {{ item.Text }}
                </option>
            </select>
        </div>
        <div class="col-md-12">
            <label class="control-label" for="IdTipoSub">Subtipo Anotación</label>
            <select name="IdTipoSub" class="form-control" v-model="IdSubtipo" data-live-search="true">
                <option v-for="item in LstSubtipos" :value="item.Value" :key="item.Value">
                    {{ item.Text }}
                </option>
            </select>
        </div>
    </div>`
});

Vue.component('select-item-informe', {
    name: 'select-item-informe',
    props: {
        IdInforme: Number
    },
    data() {
        return {
            TipoSelected: '',
            LstTipos: [],
            LstSubtipos: []
        }
    },
    mounted() {
        this.getTipos(this.IdLibro);
    },
    methods: {
        getTipos: function (IdLibro) {
            console.log(IdLibro);
            axios.get('/GLOD/FormInformes/GetTipoInforme').then((response) => {
                this.LstTipos = response.data;
            })
                .catch(function (error) {
                    console.error(error);
                });
        },
        getSubTipos: function (event) {
            console.log(event.target.value);
            axios.get('/GLOD/FormInformes/GetItemInforme?IdTipo=' + event.target.value + '&&IdInforme=' + this.IdInforme).then((response) => {
                this.LstSubtipos = response.data;
            })
                .catch(function (error) {
                    console.error(error);
                });
        },
    },
    template: `<div class="row">
        <div class="col-md-12">
            <label class="control-label" for="IdTipos">Tipo Informe</label>
            <select @change="getSubTipos($event)" v-model="TipoSelected" name="IdTipos" class="form-control" data-live-search="true">
                <option v-for="item in LstTipos" :value="item.Value" :key="item.Value">
                    {{ item.Text }}
                </option>
            </select>
        </div>
        <div class="col-md-12">
            <label class="control-label" for="IdTipoSub">Informe</label>
            <select name="IdItem" class="form-control" data-live-search="true">
                <option v-for="item in LstSubtipos" :value="item.Value" :key="item.Value">
                    {{ item.Text }}
                </option>
            </select>
        </div>
    </div>`
});