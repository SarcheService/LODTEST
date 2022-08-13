class ErrorHandler implements IErrorHandler{
    exceptions: IExceptionArrayItem[];
    //variable local no es originaria de la interfaz IErrorHandler
    logAllExceptionS:boolean;

    constructor(settings: IErrorHandlerSettings){
        this.logAllExceptionS = settings.logAllExceptions;
    }

    logException(error:string,id?:number){
        var err:IException = {error,id};
        this.exceptions.push([err]);
    }

}