var ModalHelper = (function () {
    function ModalHelper(_modalName) {
        this.modalName = _modalName;
        this.Events = new EventsHelper();
    }
    ModalHelper.prototype.initModal = function () {
        var _this = this;
        $(this.modalName).on('hidden.bs.modal', function () {
            $(_this.modalName).remove();
        });
        $(this.modalName).modal({
            backdrop: 'static',
            show: false,
            keyboard: false
        });
        this.Events.call("OnInitModal");
    };
    ;
    ModalHelper.prototype.getModal = function (_modalCanvas, _url, _data) {
        var _this = this;
        $.ajax({
            url: _url,
            type: 'GET',
            data: _data,
            beforeSend: function () {
                $(".panel-load").addClass("sk-loading");
            },
            success: function (result) {
                $(".panel-load").removeClass("sk-loading");
                $(_modalCanvas).append(result);
                _this.Events.call("OnGetModal");
            }
        });
    };
    ;
    ModalHelper.prototype.open = function () {
        $(this.modalName).modal("show");
        this.Events.call("OnOpenModal");
    };
    ;
    ModalHelper.prototype.close = function () {
        $(this.modalName).modal("hide");
        this.Events.call("OnCloseModal");
    };
    ;
    return ModalHelper;
}());
