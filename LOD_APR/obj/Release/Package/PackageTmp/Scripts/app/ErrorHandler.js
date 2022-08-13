var ErrorHandler = (function () {
    function ErrorHandler(settings) {
        this.logAllExceptionS = settings.logAllExceptions;
    }
    ErrorHandler.prototype.logException = function (error, id) {
        var err = { error: error, id: id };
        this.exceptions.push([err]);
    };
    return ErrorHandler;
}());
