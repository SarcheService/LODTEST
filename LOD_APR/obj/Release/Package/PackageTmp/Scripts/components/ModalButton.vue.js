Vue.component('modal-button', {
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
        FormButtonName: {
            type: String,
            default: 'Guardar'
        },
        FormButtonIcon: {
            type: String,
            default: 'fa fa-upload'
        },
        FormButtonClass: {
            type: String,
            default: 'btn btn-sm btn-success'
        },
        FormButtonResetName: {
            type: String,
            default: 'Cancelar'
        },
        FormButtonResetIcon: {
            type: String,
            default: 'fa fa-ban'
        },
        FormButtonResetClass: {
            type: String,
            default: 'btn btn-sm btn-default'
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
        },
        PostUrl: {
            type: String,
            default: ''
        },
        ResultOk: {
            type: String,
            default: 'true'
        },
        After: {
            type: String,
            default: 'redirect' // redirect | custom | customroot | emitroot
        },
        GetAfterUrl: {
            type: String,
            default: ''
        },
        CustomAfter: {
            type: String,
            default: ''
        },
        ShowProgress: {
            type: Boolean,
            default: false
        },
        ShowSubmit: {
            type: Boolean,
            default: true
        }
    },
    data() {
        return {
            ModalOpen: false,
            FormValidate: {},
            ProgressBarValue: 0,
            IsLoading: false,
            IsSending: false
        }
    },
    mounted() {
        //this.refresh();
        //this.$on("test", () => {
        //    this.getModal();
        //});

        if (this.After == 'emitroot') {
            this.$on(this.CustomAfter, (event) => {
                this.$root.$on(event);
            });
        }
    },
    computed: {
        FormId: function () {
            return "side_bar_form_" + this.Id;
        },
        JQFormName: function () {
            return "#side_bar_form_" + this.Id;
        },
        FormPostUrl: function () {
            return (this.PostUrl != '') ? this.PostUrl : this.GetUrl;
        },
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
            this.ProgressBarValue = 0;

            axios.get(this.GetUrl)
                .then((response) => {
                    var MyComponent = Vue.extend({
                        template: response.data
                    })
                    var compiled = new MyComponent().$mount()
                    //console.log(this.JQDivFormName);
                    $(this.JQDivFormName).append(compiled.$el)

                    $('.summernote').summernote({
                        height: 150,
                        toolbar: [
                            // [groupName, [list of button]]
                            ['style', ['bold', 'italic', 'underline', 'clear']],
                            ['font', ['strikethrough', 'superscript', 'subscript']],
                            ['fontsize', ['fontsize']],
                            ['color', ['color']],
                            ['para', ['ul', 'ol', 'paragraph']],
                            ['height', ['height']]
                        ]
                    })

                    this.ModalOpen = true;
                    this.IsLoading = false;
                    //this.getTipos();
                    //this.FormBody = compiled.$el;
                    //this.modalOpen();
                })
                .catch(function (error) {
                    console.error(error);
                    this.IsLoading = false;
                });
            //this.ModalOpen = true;
            //$.validator.unobtrusive.parse(myForm);
        },
        submitForm: function () {

            jQuery('.summernote').text(jQuery('.summernote').code());
            //$("#Cuerpo").val(jQuery('#Cuerpo').code());
            let myForm = document.getElementById(this.FormId);
            //console.log(jQuery('#Cuerpo').code());
            var form = $(this.JQFormName);
            //console.log(myForm);
            $.validator.unobtrusive.parse(form);
            form.validate();

            if (form.valid() === false) {
                console.log('No se validó el form');
                return false;
            }
          
            let formData = new FormData(myForm);
            formData.append("Cuerpo", $('#Cuerpo').text());

            this.IsSending = true;
            //console.log(formData);
            //return false;

            axios.post(myForm.action, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                },
                onUploadProgress: (progressEvent) => {
                    const totalLength = progressEvent.lengthComputable ? progressEvent.total : progressEvent.target.getResponseHeader('content-length') || progressEvent.target.getResponseHeader('x-decompressed-content-length');
                    //console.log("onUploadProgress", totalLength);
                    if (totalLength !== null) {
                        this.ProgressBarValue = Math.round((progressEvent.loaded * 100) / totalLength);
                    }
                }
            }).then((response) => {
                this.IsSending = false;
                this.FormValidate = response.data;
                let href = response.data.Parametros;
                this.ProgressBarValue = 0;
                if (this.FormValidate.Status) {
                    this.resetForm();
                    if (this.After == 'redirect') {
                        //console.log(this.FormValidate.Parametros);
                        window.location.href = href;
                    } else if (this.After == 'table_update') {

                    }else if (this.After == 'custom') {
                        console.log('custom:' + this.CustomAfter);
                        this.$parent.$emit(this.CustomAfter);
                    } else if (this.After == 'customroot') {
                        console.log('custom:' + this.CustomAfter);
                        this.$root.$emit(this.CustomAfter);
                    }
                };
            })
            .catch(function (error) {
                console.error(error);
                this.ProgressBarValue = 0;
                this.IsSending = false;
            });
        },
        resetForm: function () {
            this.ModalOpen = false;
            this.FormValidate = {};
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
                                    <h3 :class="[FormClass,'m-t-none']">{{FormName}}</h3>
                                     <ul>
                                      <li v-for="error in FormValidate.ErrorMessage" class="text-danger">{{error}}</li>
                                    </ul>
                                </div>
                            </div>
                            <form :id="FormId" :action="FormPostUrl" method="post" enctype="multipart/form-data" v-on:submit.prevent="submitForm">
                                <div class="row">
                                    <div class="col-sm-12 col-md-12 m-t-md">
                                        <div :id="DivFormName"></div>
                                    </div>
                                </div>
                              <div v-if="ShowProgress" class="row">
                                    <div class="col-md-6">
                                        <div class="progress full progress-striped active">
                                            <div :style="{'width':ProgressBarValue + '%'}" role="progressbar" class="progress-bar progress-bar-success">{{ProgressBarValue}}%</div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row m-t-md">
                                    <div class="col-md-12">
                                        <div class="btn-group pull-right">
                                            <button :disabled="IsSending"  v-if="ShowSubmit" type="submit" :class="FormButtonClass"><i v-if="IsSending" class="fa fa-refresh fa-spin fa-fw"></i><i v-if="!IsSending" :class="FormButtonIcon"></i> {{FormButtonName}}</button>
                                            <button type="button" :class="FormButtonResetClass" @click="resetForm"><i :class="FormButtonResetIcon"></i> {{FormButtonResetName}} </button>
                                        </div>
                                    </div>
                                </div>
                            </form>
                        </div>
                    </div>
               </transition>
            </div>`
});