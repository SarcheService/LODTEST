//INTERFACE PARA LOS MODALES
interface IMapsHelper{
    mapName:string;
    Latitud:number;
    Longitud:number;
    Edit:boolean;
}
//INTERFACE PARA LOS MODALES
interface IModalHelper{
    modalName:string;
    modalBodyName:string;
}
//INTERFACE PARA TAGS-ADMIN
interface ITagsParameters{
    AddTagLink:string;
    TagsTreeDataParams:any;
}
//INTERFACES PARA LOS TREEVIEWS
interface ITreeHelper{
    treeName: string;
    expandBtnName:string;
    collapseBtnName:string;
    searchInputName:string;
}
//INTERFACE PARA CREAR ALERTAS TIPO TOAST
interface IModal{
    modalName:string;
    modalBodyName:string;
}
//INTERFACE PARA TRABAJO CON ARCHIVOS
interface IFileModal{
    modalForm:string;
    modalBodyName:string;
    formEdit:string;
    inputFileName:string;
    locallyFiles:boolean;
    urlAddModel:string;
}
interface IFile{
    File:any;
    FileName?:string;
    Parameters?:string[];
}
//CLASE EXCEPTIONHANDLER
//ESTRUCTURA BASICA DEL ERROR (MENSAJE Y ID)
interface IException{
    error: string;
    id?: number;
}
//ESTRUCTURA DE UN ARRAY DE ERRORES
interface IExceptionArrayItem{
    [index:number]:IException;
}
//DEFINICION DE LA ESTRUCTURA DE LA CLASE CON UN ARRAY DE EXCEPCIONES Y UN METODO DE GUARDADO DE LOGS 
interface IErrorHandler{
    exceptions: IExceptionArrayItem[];
    logException(error:string,id?:number):void;
}
//INTERFACE PARA CONFIGURAR EL LOG DE ERRORES
interface IErrorHandlerSettings{
    logAllExceptions: boolean;
}