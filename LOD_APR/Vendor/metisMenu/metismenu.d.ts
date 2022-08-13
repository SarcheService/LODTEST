// Type definitions for metisMenu 2.0.3
// Project: http://github.com/onokumus/metisMenu
// Definitions by: onokums <https://github.com/onokumus/>
// Definitions: https://github.com/borisyankov/DefinitelyTyped

//SE COMENTO LA SIGUIENTE LINEA POR NO ENCONTRARSE EL ARCHIVO
//REFERENCIA A MODULO JQUERY SE INSTALO LOCALMENTE EN EL EQUIPO DE DESARROLLO
//COMENTARIO: Enrique Angel 09-03-2017
///// <reference path="../jquery/jquery.d.ts"/>

interface MetisMenuOptions {
    toggle?: boolean;
    doubleTapToGo?: boolean;
    preventDefault?: boolean;
    activeClass?: string;
    collapseClass?: string;
    collapseInClass?: string;
    collapsingClass?: string;
}

interface JQuery {
    metisMenu(options?: MetisMenuOptions): JQuery;
}
