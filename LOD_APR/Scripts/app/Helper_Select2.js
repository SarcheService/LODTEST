var SelectHelper = (function () {
    function SelectHelper(_selectName, _url, _place, _clear, _multiple, _tag) {
        this.selectName = _selectName;
        this.url = _url;
        this.allowClear = _clear;
        this.placeholder = _place;
        this.allowMultiple = _multiple;
        this.allowTag = _tag;
        this.Events = new EventsHelper();
    }
    SelectHelper.prototype.initSelect = function () {
        $(this.selectName).select2({
            tags: this.allowTag,
            multiple: this.allowMultiple,
            allowClear: this.allowClear,
            placeholder: this.placeholder,
            theme: "bootstrap",
            tokenSeparators: [',', ' '],
        });
        this.Events.call("OnInitSelect");
    };
    ;
    SelectHelper.prototype.initSelectAjax = function () {
        $(this.selectName).select2({
            placeholder: this.placeholder,
            theme: "bootstrap",
            tokenSeparators: [',', ' '],
            minimumInputLength: 3,
            minimumResultsForSearch: 10,
            ajax: {
                url: this.url,
                dataType: "json",
                type: "GET",
                data: function (params) {
                    var queryParameters = {
                        q: params.term
                    };
                    return queryParameters;
                },
                processResults: function (data) {
                    return {
                        results: $.map(data, function (item) {
                            return {
                                text: item.name,
                                id: item.id
                            };
                        })
                    };
                }
            }
        });
        this.Events.call("OnInitSelect");
    };
    ;
    SelectHelper.prototype.initSelectAjaxWhitParameter = function (_id) {
        $(this.selectName).select2({
            placeholder: this.placeholder,
            theme: "bootstrap",
            tokenSeparators: [',', ' '],
            minimumInputLength: 3,
            minimumResultsForSearch: 10,
            ajax: {
                url: this.url,
                dataType: "json",
                type: "GET",
                data: function (params) {
                    var queryParameters = {
                        q: params.term,
                        id: _id
                    };
                    return queryParameters;
                },
                processResults: function (data) {
                    return {
                        results: $.map(data, function (item) {
                            return {
                                text: item.name,
                                id: item.id
                            };
                        })
                    };
                }
            }
        });
        this.Events.call("OnInitSelect");
    };
    ;
    SelectHelper.prototype.initSelectAjaxWhitParameterModal = function (_id, _modal) {
        $(this.selectName).select2({
            dropdownParent: $(_modal),
            placeholder: this.placeholder,
            theme: "bootstrap",
            tokenSeparators: [',', ' '],
            minimumInputLength: 3,
            minimumResultsForSearch: 10,
            ajax: {
                url: this.url,
                dataType: "json",
                type: "GET",
                data: function (params) {
                    var queryParameters = {
                        q: params.term,
                        id: _id
                    };
                    return queryParameters;
                },
                processResults: function (data) {
                    return {
                        results: $.map(data, function (item) {
                            return {
                                text: item.name,
                                id: item.id
                            };
                        })
                    };
                }
            }
        });
        this.Events.call("OnInitSelect");
    };
    ;
    SelectHelper.prototype.getSelected = function () {
        return $(this.selectName).val();
    };
    SelectHelper.prototype.clearSelection = function () {
        $(this.selectName).val('').trigger('change');
    };
    return SelectHelper;
}());
