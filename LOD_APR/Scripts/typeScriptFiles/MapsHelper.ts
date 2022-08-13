declare var google: any;
class MapsHelper implements IMapsHelper{
    mapName:string;
    Latitud:number;
    Longitud:number;
    Edit:boolean;
    Zoom:number;
    Map:any;
    Marker:any;
    
    constructor(_Edit?:boolean){

        if(_Edit!=null){
            this.Edit=_Edit;
        }else{
            this.Edit=false;
        }

    }

    public refreshMap(_mapName:string,_Latitud:number,_Longitud:number,_Zoom:number): void{
        this.mapName=_mapName;
        this.Latitud=_Latitud;
        this.Longitud=_Longitud;
        this.Zoom=_Zoom;

        this.Map = new google.maps.Map(document.getElementById(_mapName), {
        zoom: _Zoom,
        center: new google.maps.LatLng(_Latitud,_Longitud),
        mapTypeControl: true,
        disableDefaultUI: true,
        zoomControl: true,
        mapTypeControlOptions: {
            style: google.maps.MapTypeControlStyle.DROPDOWN_MENU,
            mapTypeIds: [
                google.maps.MapTypeId.ROADMAP,
                 google.maps.MapTypeId.TERRAIN
                ]
            }
        });
    }

    public getStaticUrl(_Latitud:number,_Longitud:number,_Zoom:number){
        this.Latitud=_Latitud;
        this.Longitud=_Longitud;
        this.Zoom=_Zoom;

        return `https://maps.googleapis.com/maps/api/staticmap?center=${_Latitud},${_Longitud}&zoom=${_Zoom}&size=400x400&maptype=hybrid&markers=color:blue%7Clabel:S%7C${_Latitud},${_Longitud}&key=AIzaSyAnOIlwB52_tymzbyG1NInO8nhF-SjnZDo`;

    }

    public insertMarker(_Titulo) {

        this.Marker = new google.maps.Marker({
                position: this.Map.getCenter(),
                draggable: true,
                map: this.Map,
                title: _Titulo,
                icon: "http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=F|FF0000|000000"
        })

    }
    

}