var hub = (function () {
    function Singleton() {

        var updateStateHandlers = [];
        var throwenDiceHandlers = [];
        var gameStartHandlers = [];
        var updateQueueHandlers = [];
        var updateToastHandlers = [];

        this.subscribeUpdateState = function (callback) {
            updateStateHandlers[updateStateHandlers.length] = callback;
        };

        this.subscribeThrowenDice = function (callback) {
            throwenDiceHandlers[throwenDiceHandlers.length] = callback;
        };

        this.subscribeGameStart = function (callback) {
            gameStartHandlers[gameStartHandlers.length] = callback;
        };

        this.subscribeUpdateQueue = function (callback) {
            updateQueueHandlers[updateQueueHandlers.length] = callback;
        };

        this.subscribeUpdateToast = function (callback) {
            updateToastHandlers[updateToastHandlers.length] = callback;
        };

        this.subscribeUpdateToast(function (data) {
            switch (data.Type) {
                case 0:
                    toastr.info(data.Body, data.Title);
                    break;
                case 1:
                    toastr.warning(data.Body, data.Title);
                    break;
                case 3:
                    toastr.success(data.Body, data.Title);
                    break;
                case 2:
                    toastr.error(data.Body, data.Title);
                    break;
                default:
                    toastr.error('Что-то определенно пошло не так', 'Можно глянуть в консольку!!');
                    break;
            }
        });

        this.subscribeUpdateState(function (data) {
            console.log("gameEventHub.updateState from hub", data);
        });
        this.subscribeThrowenDice(function (data) {
            console.log("gameEventHub.throwenDice from hub", data);
        });
        this.subscribeGameStart(function (data) {
            console.log("gameEventHub.gameStart from hub", data);
        });
        this.subscribeUpdateQueue(function (data) {
            console.log("gameEventHub.updateQueue from hub", data);
        });

        this.joinGame = function (token) {
            mapHub.server.joinGame(token);
        };

        var mapHub = $.connection.mapHub;

        mapHub.client.updateToast = function (data) {
            for (var i = 0; i < updateToastHandlers.length; i++) {
                updateToastHandlers[i](data);
            }
        };
        mapHub.client.updateState = function (data) {
            for (var i = 0; i < updateStateHandlers.length; i++) {
                updateStateHandlers[i](data);
            }
        };
        mapHub.client.throwenDice = function (data) {
            for (var i = 0; i < throwenDiceHandlers.length; i++) {
                throwenDiceHandlers[i](data);
            }
        };
        mapHub.client.updateQueue = function (data) {
            for (var i = 0; i < updateQueueHandlers.length; i++) {
                updateQueueHandlers[i](data);
            }
        };
        mapHub.client.gameStart = function (data) {
            for (var i = 0; i < gameStartHandlers.length; i++) {
                gameStartHandlers[i](data);
            }
        };

        $.connection.hub.start().done(function (data) {
            console.log("connection.hub.start()", data);
        });
    }
    var instance;
    var _static = {
        name: 'SingletonTester',
        // This is a method for getting an instance
        // It returns a singleton instance of a singleton object
        getInstance: function () {
            if (instance === undefined) {
                instance = new Singleton();
            }
            return instance;
        }
    };
    return _static;
})();

angular.module('colonizatory', []).factory('gameEventHub', function ($http) {
    return hub.getInstance();
});
