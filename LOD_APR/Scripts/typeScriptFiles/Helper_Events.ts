class EventsHelper{
    public events: any;
    public bind:Function;
    public call:Function;
    public unbind:Function;
    public on:Function;
    constructor(){
        this.events = [];
        this.bind = (name:string,fn:any)=>{
            this.events[name] = [];
            var id = Math.random().toString(36).replace(/[^a-z0-9]+/g,'').substring(0,8);
            this.events[name][id] = {
                id: id,
                callback: fn
            }
            return id;
        };

        this.call = (name:string,param:any=null)=>{
            if(this.events[name] != null){
                for(var x in this.events[name]){
                    console.debug(name,param);
                    this.events[name][x].callback(param);
                }
            }
        };

        this.on = (name:string)=>{
            if(this.events[name] != null){
                return true;
            }else{
                return false;
            };
        };

        this.unbind = (name:string, id:string)=>{
            if(this.events[name] != null){
               if(this.events[name][id] != null){
                   delete(this.events[name][id]);
               }
            }
        };
        
    }
}