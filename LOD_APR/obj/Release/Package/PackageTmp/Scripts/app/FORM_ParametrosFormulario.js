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
var FORM_ParametrosFormulario = (function (_super) {
    __extends(FORM_ParametrosFormulario, _super);
    function FORM_ParametrosFormulario() {
        var _this = _super.call(this) || this;
        _this.alert = new AlertHelper();
        _this.modalCanvas = "#modalCanvas";
        _this.modalName = "#modalparametrosEvaluacion";
        _this.formName = "#formparametrosEvaluacion";
        _this.modalForm = new ModalHelper(_this.modalName);
        _this.UpdateSorteable();
        return _this;
    }
    FORM_ParametrosFormulario.prototype.initModal = function (data, status, xhr) {
        $.validator.unobtrusive.parse($(this.formName));
        this.modalForm.initModal();
        $(document).on("click", "[type='checkbox']", function (e) {
            if (this.id == "TextoLargo_1" && this.checked) {
                $("#Obligatoria").attr("value", "True");
            }
            else if (this.id == "TextoLargo_1" && !this.checked) {
                $("#Obligatoria").attr("value", "False");
            }
        });
        this.modalForm.open();
    };
    FORM_ParametrosFormulario.prototype.saveResult = function (data, status, xhr) {
        var data1 = data.split(";");
        if (data1[0] == 'true') {
            this.UpdatePanelParams(data1[1]);
            return;
        }
        else {
            this.modalForm.close();
            this.alert.toastErrorData(data);
        }
    };
    FORM_ParametrosFormulario.prototype.UpdatePanelParams = function (_idPanel) {
        var _this = this;
        var loadUrl = "/Admin/Formularios/ListParams";
        this.Events.bind("OnGetPartial", function () {
            _this.UpdateSorteable();
        });
        var id = { id: _idPanel };
        this.getPartial("#Panel_" + _idPanel, loadUrl, id);
        this.alert.toastOk();
        this.modalForm.close();
        return;
    };
    FORM_ParametrosFormulario.prototype.UpdateSorteable = function () {
        var _this = this;
        $(".dragable-List").sortable({
            items: ".list-group-item",
            stop: function (event, ui) {
                var IdItem = ui.item[0].dataset.iditem;
                var indexReal = 0;
                console.log(ui);
                console.log("Cambio de Index UI: " + IdItem);
                $('.dragable-List').find('.list-group-item').each(function (index, item) {
                    console.log("Cambio de Index: " + index + ' ' + item.dataset.id);
                    var itemIdItem = item.dataset.iditem;
                    var itemId = item.dataset.id;
                    if (itemIdItem == IdItem) {
                        indexReal++;
                        _this.UpdateParamIndex(itemId, index);
                        $("#ParamIndic_" + itemId).text(indexReal + ")");
                    }
                });
            }
        });
    };
    FORM_ParametrosFormulario.prototype.UpdateParamIndex = function (_Id, _Index) {
        this.Events.bind("OnPostCustomData", function (_Indice) { });
        this.postCustomData({ id: _Id, index: _Index }, "/Admin/FormPreguntas/UpdateIndex");
    };
    return FORM_ParametrosFormulario;
}(GenericController));
