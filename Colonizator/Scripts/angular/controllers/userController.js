var myAppModule = angular.module('colonizatory', []);

var userController = function ($scope) {
    var injector = angular.injector(['colonizatory', 'ng']);
    $scope.userName = "test";
    $scope.token = "test";
    $scope.playerId = 0;
    $scope.currentUser = null;
    $scope.testClick = function () {
        injector.get('gameUserStateApiService').GetUserState($scope.token, $scope.playerId, function (data) {
            $scope.currentUser = data.CurrentPlayer;
            console.log("gameStateApiService.GetUserState", $scope.currentUser);
        });
    };
}