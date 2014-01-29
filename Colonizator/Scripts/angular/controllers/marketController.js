var marketController = function ($scope) {
    var injector = angular.injector(['colonizatory', 'ng']);
    var gameEventHub = injector.get('gameEventHub');

    $scope.userName = "test";
    $scope.token = "test";
    $scope.playerId = 0;
    $scope.currentUser = null;
    $scope.orders = [];
    $scope.loadMarket = function () {
        injector.get('gameApiService').GetUserState($scope.token, $scope.playerId, function (data) {
            $scope.currentUser = data.CurrentPlayer;
            console.log("gameApiService.GetUserState current user", $scope.currentUser);
            $scope.orders = [];
            for (var i = 0; i < data.Orders.length; i++) {
                $scope.orders.push(data.Orders[i]);
            }
            console.log("gameApiService.GetUserState orders", data.Orders);
            $scope.$apply();
        });
    };

    gameEventHub.updateState = function (data) {
        console.log("gameEventHub.updateState from marketController", data);
    };
}