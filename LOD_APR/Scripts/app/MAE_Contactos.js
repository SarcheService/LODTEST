var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var MAE_Contactos = (function (_super) {
    __extends(MAE_Contactos, _super);
    function MAE_Contactos() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalContacto";
        _this.formName = "#formContacto";
        _this.inputFileName = "#fileImage";
        _this.urlGetTable = "/Admin/Contactos/getContactos";
        _this.modalForm = new ModalHelper(_this.modalName);
        return _this;
    }
    ;
    MAE_Contactos.prototype.initModal = function (data, status, xhr) {
        $(".panel-load").removeClass("sk-loading");
        $("#IdSucursal").select2({
            allowClear: true,
            placeholder: 'Seleccione una sucursal',
            theme: "bootstrap"
        });
        this.initForm();
        this.modalForm.initModal();
        this.modalForm.open();
    };
    MAE_Contactos.prototype.initDeleteModal = function (data, status, xhr) {
        $(".panel-load").removeClass("sk-loading");
        this.initForm();
        this.modalForm = new ModalHelper("#modalDelete");
        this.modalForm.initModal();
        this.modalForm.open();
    };
    MAE_Contactos.prototype.saveResult = function (data, status, xhr) {
        $(".panel-load").removeClass("sk-loading");
        if (data == 'true') {
            this.Events.bind("OnGetPartial", function () {
            });
            this.getPartial("#divTableDatos", this.urlGetTable, {});
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    MAE_Contactos.prototype.saveDeleteResult = function (data, status, xhr) {
        $(".panel-load").removeClass("sk-loading");
        var datos = JSON.parse(data);
        if (!datos.error) {
            this.getPartial("#divContactos", this.urlGetTable, { id: datos.idSujeto });
            this.alert.toastOk();
            this.modalForm.close();
            this.modalForm = new ModalHelper("#modalDelete");
            this.modalForm.close();
            return;
        }
        else {
            this.alert.toastErrorData(datos.mensaje);
        }
    };
    MAE_Contactos.prototype.initForm = function () {
        var _this = this;
        this.formValidator = {
            rules: {
                Nombre: { required: true },
                CargoContacto: { required: true },
                Email: { email: true },
            },
            messages: {
                Nombre: "Dato obligatorio",
                CargoContacto: "Dato obligatorio",
                Email: { email: "E-mail inválido" }
            },
            errorClass: "text-danger",
            errorElement: "h5",
            submitHandler: function (form) {
                _this.modalForm.close();
            }
        };
        $(this.formName).validate(this.formValidator);
        this.Events.call("OnInitForm");
    };
    MAE_Contactos.prototype.SaveFileToDB = function (_id, _url) {
        var _this = this;
        var formToValidate = $(this.formName);
        formToValidate.validate();
        if (formToValidate.valid() == false) {
            return;
        }
        if ($("#Nombre").val() == '') {
            this.alert.toastErrorData("El nombre es obligatorio");
            return;
        }
        if ($("#CargoContacto").val() == '') {
            this.alert.toastErrorData("El cargo es obligatorio");
            return;
        }
        var form = document.getElementById(this.formName.replace("#", ""));
        var fileUpload = $(this.inputFileName).get(0);
        var files = fileUpload.files;
        var data = new FormData(form);
        for (var i = 0; i < files.length; i++) {
            data.append(files[i].name, files[i]);
        }
        this.Events.bind("OnSuccessSaveFile", function (response) {
            if (response == 'true') {
                _this.getPartial("#divContactos", _this.urlGetTable, { id: _id });
                _this.modalForm.close();
                _this.alert.toastOk();
                return;
            }
            else {
                _this.alert.toastErrorData(response);
            }
        });
        this.postFileToDB(data, _url);
    };
    MAE_Contactos.prototype.clearImage = function (_image) {
        $("#" + _image).replaceWith('<h2 href="#" data-letters="Foto" class="data-letters-galena" style="vertical-align: middle;"></h2>');
        $(".ruta_imagen").val('');
    };
    return MAE_Contactos;
}(GenericController));
