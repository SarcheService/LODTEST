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
var FORM_ItemsFormulario = (function (_super) {
    __extends(FORM_ItemsFormulario, _super);
    function FORM_ItemsFormulario() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalItemsFormEval";
        _this.formName = "#formItemsFormEval";
        _this.modalForm = new ModalHelper(_this.modalName);
        $("#DivDetails").sortable({
            items: ".dragable-panel",
            stop: function (event, ui) {
                $('.dragable-panel').each(function (index, element) {
                    _this.UpdateItemIndex(element.dataset.id, index, element.dataset.idform);
                });
            }
        });
        $('.panel-body').on('shown.bs.collapse', function (item) {
            $("#Icon_" + item.target.id).removeClass("fa-chevron-down").addClass("fa-chevron-up");
        });
        $('.panel-body').on('hidden.bs.collapse', function (item) {
            $("#Icon_" + item.target.id).removeClass("fa-chevron-up").addClass("fa-chevron-down");
        });
        return _this;
    }
    FORM_ItemsFormulario.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        $(document).on("click", "[type='checkbox']", function (e) {
            if (this.id == "Activo_1" && this.checked) {
                $("#Activo").attr("value", "True");
            }
            else if (this.id == "Activo_1" && !this.checked) {
                $("#Activo").attr("value", "False");
            }
        });
        this.modalForm.open();
    };
    FORM_ItemsFormulario.prototype.saveResult = function (data, status, xhr) {
        var data1 = data.split(";");
        if (data1[0] == 'true') {
            var loadUrl = "/Admin/Formularios/Details";
            this.Events.bind("OnGetPartial", function () { });
            var id = { id: data1[1] };
            this.getPartial("#DivDetails", loadUrl, id);
            this.alert.toastOk();
            this.modalForm.close();
            return;
        }
        else {
            this.modalForm.close();
            this.alert.toastErrorData(data);
        }
    };
    FORM_ItemsFormulario.prototype.UpdateItemIndex = function (_Id, _Index, _IdForm) {
        this.Events.bind("OnPostCustomData", function (_Indice) { });
        this.postCustomData({ id: _Id, index: _Index }, "/Admin/FormItems/UpdateIndex");
    };
    return FORM_ItemsFormulario;
}(GenericController));
