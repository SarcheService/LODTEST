Vue.component('session-timer', {
    name: "session-timer",
    props: {
        sessionTime: {
            type: Number,
            default:20
        },
        minutesToWarn: {
            type: Number,
            default: 2
        },
        showTimer: {
            type: Boolean,
            default: true
        },
    },
    data() {
        return {
            minutesForWarning: this.minutesToWarn,
            sessionTimeout : parseInt(this.sessionTime),
            timeToWarn : null,
            timeToEnd : null,
            time: 0,
            timer: null,
            showDialog: false,
            showWarning: true,
            showRedirect: true,
        }
    },
    computed: {
        prettyTime() {
            let time = this.time / 60
            let minutes = parseInt(time)
            let secondes = Math.round((time - minutes) * 60)
            return minutes + ":" + secondes
        }
    },
    mounted() {
        this.timeToWarn = this.setTimeToWarn();
        this.timeToEnd = this.setTimeToEnd();
        this.time = this.sessionTime * 60;
        this.start();
        setInterval(this.checkTime, 10000);
    },
    filters: {
        prettify: function (value) {
            let data = value.split(':');
            let minutes = data[0];
            let secondes = data[1];
            if (minutes < 10)
                minutes = "0" + minutes;
            
            if (secondes < 10)
                secondes = "0" + secondes;
            
            return minutes + ":" + secondes;
        }
    },
    methods: {
        setTimeToWarn: function () {
            return this.addMinutes(new Date(), this.sessionTimeout - this.minutesForWarning);
        },
        setTimeToEnd: function () {
            return this.addMinutes(new Date(), this.sessionTimeout);
        },
        addMinutes: function (date, minutes) {
            var newDate = new Date(date.getTime() + minutes * 60 * 1000);
            return newDate;
        },
        remainingMinutes: function (date) {
            var remaining = Math.round((date - (new Date()).getTime()) / 60 / 1000);
            return remaining;
        },
        rennewSession: function () {
            
            axios.post("/Account/SessionRenew", {})
                .then((response) => {
                    this.timeToWarn = this.setTimeToWarn(); 
                    this.timeToEnd = this.setTimeToEnd();
                    this.showDialog = false;
                    this.time = this.sessionTime * 60;
                })
                .catch(function (error) {
                    console.error(error);
                });

        },
        cancelRenew: function () {
            this.showDialog = false;
            this.showWarning = false;
        },
        checkTime: function () {

            if (this.showWarning && new Date() > this.timeToWarn && new Date() < this.timeToEnd)
                this.showDialog = true;
            
            if (new Date() > this.timeToEnd)
                document.getElementById('logoutForm').submit();
            
        },
        start: function () {
            if(!this.timer) {
                this.timer = setInterval(() => {
                    if (this.time > 0) {
                        this.time--
                    } else {
                       document.getElementById('logoutForm').submit();
                    }
                }, 1000)
            }
        }
    },
    template: `<div>
                <div class="timer" v-if="showTimer"><small>Cierre Sesión {{prettyTime | prettify}}</small></div>
                <div v-if="showDialog" class="text-left">
                    <div class="modal fade hmodal-danger in" id="modalFaena" tabindex="-1" role="dialog" aria-hidden="true" style="display: block; padding-right: 19px;">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="color-line"></div>
                                <div class="modal-header">
                                    <h4 class="modal-title m-b-none">Cierre de Sesión</h4>
                                    <p class="font-bold">Su sesión en el sistema está a punto de caducar.</p>
                                </div>
                                <div class="modal-body">
                                    <h3>¿Desea renovar su sesión de usuario por un tiempo más?</h3>
                                </div>
                                <div class="modal-footer">
                                    <button type="button" @click="rennewSession" class="btn btn-success"><i class="fa fa-check"></i> Renovar Sesión </button>
                                    <button type="button" @click="cancelRenew" class="btn btn-default"><i class="fa fa-ban"></i> Cancelar </button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-backdrop fade in"></div>
            </div>
            </div>`
});