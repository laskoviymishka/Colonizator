angular.module('colonizatory', []).factory('gameUserStateApiService', function ($http) {
    return new stateRepo($http);
});


function stateRepo($http) {
    var self = this;
    self.GetUserState = function (token, playerId, callback) {
        var uri = '/api/GameApi/GetState?tokenId=' + token + '&playerId=' + playerId + '';
        $http({ method: 'GET', url: uri }).success(function (data, status, headers, config) {
            console.log("success", data, status, headers, config);
            callback(data);
        }).error(function (data, status, headers, config) {
            console.log("error", data, status, headers, config);
            callback(data);
        });
    };
}
