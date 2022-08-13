class SelectHelper{
    private selectName:string;
    private url:string;
    private allowClear:boolean;
    private placeholder:string;
    private allowMultiple:boolean;
    private allowTag:boolean;
    //EVENTOS
    public Events: EventsHelper;
    //OnInitSelect

    constructor(_selectName:string,_url:string,_place:string,_clear:boolean,_multiple:boolean,_tag:boolean){
        this.selectName=_selectName;
        this.url = _url;
        this.allowClear = _clear;
        this.placeholder = _place;
        this.allowMultiple = _multiple;
        this.allowTag = _tag;
        this.Events = new EventsHelper();
    }

    public initSelect(): void{
       
        $(this.selectName).select2({
            tags: this.allowTag,
            multiple: this.allowMultiple,
            allowClear: this.allowClear,
            placeholder:  this.placeholder, //'',
            theme: "bootstrap",
            tokenSeparators: [',', ' '],
        });

        this.Events.call("OnInitSelect");

    };

    public initSelectAjax(): void{
        
         $(this.selectName).select2({
            // tags: this.allowTag,
            // multiple: this.allowMultiple,
            // allowClear: this.allowClear,
             placeholder:  this.placeholder, //'',
             theme: "bootstrap",
             tokenSeparators: [',', ' '],
             minimumInputLength: 3,
             minimumResultsForSearch: 10,
             ajax: {
                 url:  this.url,
                 dataType: "json",
                 type: "GET",
                 data: (params)=> {        
                    var queryParameters = {
                            q: params.term
                        }
                        return queryParameters;
                 },
                 processResults: function (data) {
                     return {
                         results: $.map(data, function (item) {
                             return {
                                 text: item.name,
                                 id: item.id
                             }
                         })
                     };
                 }
             }
         });

         this.Events.call("OnInitSelect");

    };

    public initSelectAjaxWhitParameter(_id:number): void{
        
        $(this.selectName).select2({
           // tags: this.allowTag,
           // multiple: this.allowMultiple,
           // allowClear: this.allowClear,
            placeholder:  this.placeholder, //'',
            theme: "bootstrap",
            tokenSeparators: [',', ' '],
            minimumInputLength: 3,
            minimumResultsForSearch: 10,
            ajax: {
                url:  this.url,
                dataType: "json",
                type: "GET",
                data: (params)=> {        
                   var queryParameters = {
                           q: params.term,
                           id:_id
                       }
                       return queryParameters;
                },
                processResults: function (data) {
                    return {
                        results: $.map(data, function (item) {
                            return {
                                text: item.name,
                                id: item.id
                            }
                        })
                    };
                }
            }
        });

        this.Events.call("OnInitSelect");

   };

   public initSelectAjaxWhitParameterModal(_id:number,_modal:string): void{
        
    $(this.selectName).select2({
        dropdownParent: $(_modal),
        placeholder:  this.placeholder, //'',
        theme: "bootstrap",
        tokenSeparators: [',', ' '],
        minimumInputLength: 3,
        minimumResultsForSearch: 10,
        ajax: {
            url:  this.url,
            dataType: "json",
            type: "GET",
            data: (params)=> {        
               var queryParameters = {
                       q: params.term,
                       id:_id
                   }
                   return queryParameters;
            },
            processResults: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            text: item.name,
                            id: item.id
                        }
                    })
                };
            }
        }
    });

    this.Events.call("OnInitSelect");

};



    public getSelected():any{
        return $(this.selectName).val();
    }

    public clearSelection():any{
        $(this.selectName).val('').trigger('change')
    }
    
}
