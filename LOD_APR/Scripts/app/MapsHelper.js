var MapsHelper = (function () {
    function MapsHelper(_Edit) {
        if (_Edit != null) {
            this.Edit = _Edit;
        }
        else {
            this.Edit = false;
        }
    }
    MapsHelper.prototype.refreshMap = function (_mapName, _Latitud, _Longitud, _Zoom) {
        this.mapName = _mapName;
        this.Latitud = _Latitud;
        this.Longitud = _Longitud;
        this.Zoom = _Zoom;
        this.Map = new google.maps.Map(document.getElementById(_mapName), {
            zoom: _Zoom,
            center: new google.maps.LatLng(_Latitud, _Longitud),
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
    };
    MapsHelper.prototype.getStaticUrl = function (_Latitud, _Longitud, _Zoom) {
        this.Latitud = _Latitud;
        this.Longitud = _Longitud;
        this.Zoom = _Zoom;
        return "https://maps.googleapis.com/maps/api/staticmap?center=" + _Latitud + "," + _Longitud + "&zoom=" + _Zoom + "&size=400x400&maptype=hybrid&markers=color:blue%7Clabel:S%7C" + _Latitud + "," + _Longitud + "&key=AIzaSyAnOIlwB52_tymzbyG1NInO8nhF-SjnZDo";
    };
    MapsHelper.prototype.insertMarker = function (_Titulo) {
        this.Marker = new google.maps.Marker({
            position: this.Map.getCenter(),
            draggable: true,
            map: this.Map,
            title: _Titulo,
            icon: "http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=F|FF0000|000000"
        });
    };
    return MapsHelper;
}());
