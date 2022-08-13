class ModalHelper implements IModalHelper{
        modalName:string;
        public modalBodyName:string;
        //EVENTOS
        public Events: EventsHelper;
        //OnInitModal
        //OnGetModal
        //OnOpenModal
        //OnCloseModal

        constructor(_modalName:string){
            this.modalName=_modalName;
            this.Events = new EventsHelper();
        }

        public initModal(): void{

            $(this.modalName).on('hidden.bs.modal', ()=> {
                $(this.modalName).remove();
                //$(this.modalName).data('bs.modal', null);
            });
        
            $(this.modalName).modal({
                backdrop: 'static',
                show: false,
                keyboard: false
            });

            this.Events.call("OnInitModal");

        };

        public getModal(_modalCanvas:string,_url:string,_data:any):void{

            $.ajax({
                url: _url,
                type: 'GET',
                data: _data,
                beforeSend: ()=> {
                    $(".panel-load").addClass("sk-loading");
                },
                success: (result)=> {
                     $(".panel-load").removeClass("sk-loading");
                    $(_modalCanvas).append(result);
                    this.Events.call("OnGetModal");
                }
            });

        };

        public open():void{
            $(this.modalName).modal("show");
            this.Events.call("OnOpenModal");
        };
        public close():void{
            $(this.modalName).modal("hide");
            this.Events.call("OnCloseModal");
        };

}