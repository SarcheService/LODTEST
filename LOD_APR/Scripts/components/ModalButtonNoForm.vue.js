Vue.component('modal-button-no-form', {
    props: {
        Id: {
            type: String,
            default: ''
        },
        ButtonName: {
            type: String,
            default: 'Nuevo Ítem'
        },
        ButtonIcon: {
            type: String,
            default: 'fa fa-plus'
        },
        ButtonClass: {
            type: String,
            default: 'btn btn-sm btn-success pull-right'
        },
        ButtonTitle: {
            type: String,
            default: ''
        },
        FormButtonCloseName: {
            type: String,
            default: 'Cancelar'
        },
        FormButtonCloseIcon: {
            type: String,
            default: 'fa fa-times'
        },
        FormButtonCloseClass: {
            type: String,
            default: 'btn btn-sm btn-default pull-right'
        },
        FormName: {
            type: String,
            default: 'Nuevo Ítem'
        },
        FormClass: {
            type: String,
            default: 'text-success'
        },
        GetUrl: {
            type: String,
            default: ''
        }
    },
    data() {
        return {
            ModalOpen: false,
            IsLoading: false,
            IsSending: false
        }
    },
    mounted() {

    },
    computed: {
        //FormId: function () {
        //    return "side_bar_form_" + this.Id;
        //},
        //JQFormName: function () {
        //    return "#side_bar_form_" + this.Id;
        //},
        //FormPostUrl: function () {
        //    return (this.PostUrl != '') ? this.PostUrl : this.GetUrl;
        //},
        JQDivFormName: function () {
            return "#side_bar_form_div_" + this.Id;
        },
        DivFormName: function () {
            return "side_bar_form_div_" + this.Id;
        },
    },
    methods: {
        getModal: function () {

            this.IsLoading = true;
         
            axios.get(this.GetUrl)
                .then((response) => {
                    var MyComponent = Vue.extend({
                        template: response.data
                    })
                    var compiled = new MyComponent().$mount()
                    $(this.JQDivFormName).append(compiled.$el)

                    this.ModalOpen = true;
                    this.IsLoading = false;

                })
                .catch(function (error) {
                    console.error(error);
                    this.IsLoading = false;
                });

        },
        resetForm: function () {
            this.ModalOpen = false;
            document.getElementById(this.DivFormName).innerHTML = "";
        }
    },
    template: `<div style="display:inline">
                <button :disabled="ModalOpen" @click="getModal" type="button" role="button" :class="ButtonClass" :title="ButtonTitle"><i v-if="IsLoading" class="fa fa-refresh fa-spin fa-fw"></i><i v-if="!IsLoading"  :class="ButtonIcon"></i> {{ButtonName}}</button>
                   <transition mode="out-in" appear appear-active-class="animated fadeInRight" enter-active-class="animated fadeInRight" leave-active-class="animated fadeOutRight">
                       <div v-show="ModalOpen" class="right-sidebar sidebar-open">
                        <div class="right-sidebar-body p-m">
                            <div class="row border-bottom">
                                <div class="col-md-12">
                                    <h3 :class="[FormClass,'m-t-none']">
                                        <b>{{FormName}}</b>
                                        <button type="button" :class="FormButtonCloseClass" @click="resetForm"><i :class="FormButtonCloseIcon"></i></button>
                                    </h3>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 col-md-12 m-t-md">
                                    <div :id="DivFormName"></div>
                                </div>
                            </div>
                        </div>
                    </div>
               </transition>
            </div>`
});