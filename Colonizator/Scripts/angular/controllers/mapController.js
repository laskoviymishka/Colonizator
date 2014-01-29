var mapController = function ($scope, $http) {
    /*
        private fields
    */
    var cos30 = 0.86602540378443864676372317075294; // sqrt(3)/2
    var a = 70;
    var tileWidth = a * cos30 * 2;
    var tileHeight = a * 2;
    var townSize = { w: 50, h: 50 };
    var villageSize = { w: 45, h: 36 };
    var roadSize = { w: 70, h: 14 };
    var hexMap = [];
    var movePlayer = {};
    var currentUser = {};
    /*
        public fields for scope. need for rendering and comunications.
    */

    $scope.tiles = [];
    $scope.towns = [];
    $scope.roads = [];
    $scope.playerId = 0;
    $scope.token = "test";
    $scope.confirmationBuy = {
        action: 'постройка города',
        show: false,
        declineClick: function (parameters) {
            $scope.confirmationBuy.show = false;
        },
        resourcesCost: []
    };

    $scope.skip = {
        show: false,
        click: function (parameters) {
            $http.post(
               "/Game/PassMove",
               {
                   token: $scope.token,
                   playerId: $scope.playerId
               });
            this.show = false;
        }
    };

    $scope.freeResource = {
        show: false,
        action: '',
        needCount : 2,
        acceptShow: false,
        chosableResource:[],
        updateAcceptShow: updateAccept(),
        accept: function() {
            var contract = {
                Token: $scope.token,
                playerId: $scope.playerId,
                first: -1,
                second: -1
            };
            for (var i = 0; i < $scope.freeResource.chosableResource.length; i++) {
                if ($scope.freeResource.chosableResource[i].IsChoosen) {
                    if (contract.first == -1) {
                        contract.first = $scope.freeResource.chosableResource[i].Type;
                    } else {
                        if (contract.second == -1) {
                            contract.second = $scope.freeResource.chosableResource[i].Type;
                        }
                    }
                }
            }
            $.post("/Game/ChooseFreeResource", contract);
            this.show = false;
            this.chosableResource = chooseFreeResouce();
            this.acceptShow = false;
        },
        decline: function() {
            this.chosableResource =
            [
                {
                    Type: 0,
                    IsChoosen: false,
                    Choose: function() {
                        if (acceptShow) return;
                        this.IsChoosen = !this.IsChoosen;
                        updateAcceptShow();
                    }
                },
                {
                    Type: 1,
                    IsChoosen: false,
                    Choose: function() {
                        if (acceptShow) return;
                        this.IsChoosen = !this.IsChoosen;
                        updateAcceptShow();
                    }
                },
                {
                    Type: 2,
                    IsChoosen: false,
                    Choose: function() {
                        if (acceptShow) return;
                        this.IsChoosen = !this.IsChoosen;
                        updateAcceptShow();
                    }
                },
                {
                    Type: 3,
                    IsChoosen: false,
                    Choose: function() {
                        if (acceptShow) return;
                        this.IsChoosen = !this.IsChoosen;
                        updateAcceptShow();
                    }
                },
                {
                    Type: 4,
                    IsChoosen: false,
                    Choose: function() {
                        if (acceptShow) return;
                        this.IsChoosen = !this.IsChoosen;
                        updateAcceptShow();
                    }
                }
            ];
            this.show = false;
        }
    };

    function generateResourceChooser() {
        return [
            {
                Type: 0,
                IsChoosen: false,
                Choose: function() {
                    if ($scope.freeResource.acceptShow) return;
                    this.IsChoosen = !this.IsChoosen;
                    updateAccept();
                }
            },
            {
                Type: 1,
                IsChoosen: false,
                Choose: function() {
                    if ($scope.freeResource.acceptShow) return;
                    this.IsChoosen = !this.IsChoosen;
                    updateAccept();
                }
            },
            {
                Type: 2,
                IsChoosen: false,
                Choose: function() {
                    if ($scope.freeResource.acceptShow) return;
                    this.IsChoosen = !this.IsChoosen;
                    updateAcceptShow();
                }
            },
            {
                Type: 3,
                IsChoosen: false,
                Choose: function() {
                    if ($scope.freeResource.acceptShow) return;
                    this.IsChoosen = !this.IsChoosen;
                    updateAccept();
                }
            },
            {
                Type: 4,
                IsChoosen: false,
                Choose: function() {
                    if ($scope.freeResource.acceptShow) return;
                    this.IsChoosen = !this.IsChoosen;
                    updateAccept();
                }
            }
        ];
    }

    function updateAccept() {
        var choosedCount = 0;
        if ($scope.freeResource == undefined) return;
        for (var i = 0; i < $scope.freeResource.chosableResource.length; i++) {
            if ($scope.freeResource.chosableResource[i].IsChoosen) {
                choosedCount++;
            }
        }
        if (choosedCount < 2) {
            $scope.freeResource.acceptShow = false;
        }
        if (choosedCount == 2) {
            $scope.freeResource.acceptShow = true;
        }
        if (choosedCount > 2) {
            for (var i = 0; i < $scope.freeResource.chosableResource.length; i++) {
                $scope.freeResource.chosableResource[i].IsChoosen = false;
            }
            updateAccept();
        }
    }

    $scope.dice = {
        show: false,
        throwShow: false,
        declineShow: false,
        throwAction: function (data) {
            $.get('/Game/ThrowDice?playerId=' + $scope.playerId + '&token=' + $scope.token, function (res) {
                console.log('/Game/ThrowDice?playerId=' + $scope.playerId + '&token=' + $scope.token);
            });
        },
        declineAction: function (data) {
            this.show = false;
            this.throwShow = false;
            this.declineShow = false;
            this.result = '';
        },
        result: ''
    };

    /*
        public scope events handlers.
    */

    $scope.townClick = function (town) {
        console.log("newTown.townClick", town);
        buildCity(town);
    };

    $scope.tileClick = function (tile) {
        console.log("newTile.tileClick", tile);
    };

    $scope.roadClick = function (road) {
        console.log("newRoad.roadClick", road);
        buildRoad(road);
    };

    /*
        event hadlers for server push notifications
    */

    angular.injector(['colonizatory', 'ng']).get('gameEventHub').subscribeUpdateState(function (data) {
        console.log("gameEventHub.updateState from mapController", data);
        $scope.UpdateMapViewState();
        movePlayer = data.movePlayer;
        switch (data.Args.Action) {
            case 0://"NextMove":
                console.log("updateState switch (data.Args.Action) NextMove", data);
                if (!data.isStartUp) {
                    needThrowDice();
                }
                passMoveAvaible();
                break;
            case 1:// "DiceThrowen":
                console.log("updateState switch (data.Args.Action) DiceThrowen", data);
                throwenDice(data);
                break;
            case 2://"MoveRobber":
                moveRobber(data);
                console.log("updateState switch (data.Args.Action) MoveRobber", data);
                break;
            case 3://"Monopoly":
                console.log("updateState switch (data.Args.Action) Monopoly", data);
                monopolyResouce(data);
                break;
            case 4://"FreeResource":
                freeResouce(data);
                console.log("updateState switch (data.Args.Action) FreeResource", data);
                break;
            case 5:// "CardUpdate":
                console.log("updateState switch (data.Args.Action) CardUpdate", data);
                break;
            case 6://"RegularUpdate":
                console.log("updateState switch (data.Args.Action) RegularUpdate", data);
                passMoveAvaible();
                moveRobber(data);
                break;
            default:
                console.log("updateState switch (data.Args.Action) default", data);
                break;
        }
    });

    angular.injector(['colonizatory', 'ng']).get('gameEventHub').subscribeThrowenDice(function (data) {
        console.log("gameEventHub.throwenDice from mapController", data);
        throwenDice(data);
    });


    /*
        public scope methods.
    */

    $scope.GetMap = function () {
        var uri = '/api/GameApi/Map?tokenId=' + $scope.token + '';
        angular.injector(['colonizatory', 'ng']).get('gameEventHub').joinGame($scope.token);
        $http({ method: 'GET', url: uri }).success(function (data, status, headers, config) {
            console.log("success", data, status, headers, config);
            var tiles = drawField(data);
            $scope.tiles = [];
            for (var i = -18; i < tiles.length; i++) {
                if (i == 0) continue;
                $scope.tiles.push(tiles[i]);
            }
            console.log("tiles", $scope.tiles);
        }).error(function (data, status, headers, config) {
            console.log("error", data, status, headers, config);
        });
        uri = '/api/GameApi/GetState?tokenId=' + $scope.token + '&playerId=' + $scope.playerId + '';
        $http({ method: 'GET', url: uri }).success(function (data) {
            currentUser = data.CurrentPlayer;
        });
    };

    $scope.UpdateMapViewState = function () {
        var uri = '/api/GameApi/MapViewState?token=' + $scope.token + '&playerId=' + $scope.playerId + '';
        $http({ method: 'GET', url: uri }).success(function (data, status, headers, config) {
            console.log("success", data, status, headers, config);

            $scope.towns = [];
            var towns = drawTowns(data.Cities, false);
            if (towns != undefined) {
                for (var i = 0; i < towns.length; i++) {
                    $scope.towns.push(towns[i]);
                }
            }

            var possibleTowns = drawTowns(data.PossibleCities, true);
            if (possibleTowns != undefined) {
                for (var i = 0; i < possibleTowns.length; i++) {
                    $scope.towns.push(possibleTowns[i]);
                }
            }


            $scope.roads = [];
            var roads = drawRoads(data.Roads, false);
            if (roads != undefined) {
                for (var i = 0; i < roads.length; i++) {
                    $scope.roads.push(roads[i]);
                }
            }
            var possibleRoads = drawRoads(data.PossibleRoads, true);
            if (possibleRoads != undefined) {
                for (var i = 0; i < possibleRoads.length; i++) {
                    $scope.roads.push(possibleRoads[i]);
                }
            }

            console.log("towns", $scope.towns);
            console.log("roads", $scope.roads);
        }).error(function (data, status, headers, config) {
            console.log("error", data, status, headers, config);
        });
    };

    /*
        private methods helpers that help to build map.
    */

    function needThrowDice() {
        console.log('NeedThrowDice');
        togleCube(0, 1);
        togleCube(1, 1);
        if (movePlayer == currentUser.PlayerName) {
            $scope.dice.show = true;
            $scope.dice.throwShow = true;
            $scope.dice.declineShow = false;
            $scope.dice.result = 'Ваш ход, бросьте кубики';
        } else {
            $scope.dice.show = true;
            $scope.dice.throwShow = false;
            $scope.dice.declineShow = false;
            $scope.dice.result = 'Ход ' + movePlayer + ' , ждите пока он бросит кубики';
        }
    };

    function throwenDice(data) {
        console.log('throwenDice', data);
        if (currentUser.PlayerName == movePlayer) {
            $scope.dice.result = 'Вам выпало ' + data.Args.First + ' и ' + data.Args.Second;
        } else {
            $scope.dice.result = movePlayer + ' выпало ' + data.Args.First + ' и ' + data.Args.Second;
        }
        $scope.dice.show = true;
        $scope.dice.declineShow = true;
        $scope.dice.throwShow = false;
        togleCube(0, data.Args.First);
        togleCube(1, data.Args.Second);
    };

    function passMoveAvaible() {
        $scope.skip.show = movePlayer == currentUser.PlayerName;
    };

    function freeResouce(data) {
        $scope.freeResource.show = true;
        $scope.freeResource.chosableResource = generateResourceChooser();
        $scope.freeResource.acceptShow = false;
    };

    function monopolyResouce(data) {

    };

    function buildRoad(road) {
        var postObject = {
            token: $scope.token,
            playerId: $scope.playerId,
            hexA: road.hexA,
            hexB: road.hexB,
            haxagonIndex: road.hexagonIngex
        };
        $scope.confirmationBuy.action = 'постройка дороги';
        $scope.confirmationBuy.acceptClick = function (parameters) {
            $http.post(
                '/Game/BuildRoad',
                postObject);
            $scope.confirmationBuy.show = false;
        };
        $scope.confirmationBuy.resourcesCost = [];
        $scope.confirmationBuy.resourcesCost.push({ Type: 0, Qty: 1 });
        $scope.confirmationBuy.resourcesCost.push({ Type: 3, Qty: 1 });

        $scope.confirmationBuy.show = true;
    };

    function buildCity(town) {
        var postObject = {
            token: $scope.token,
            playerId: $scope.playerId,
            hexA: town.hexA,
            hexB: town.hexB,
            hexC: town.hexC,
            hexIndex: town.hexagonIngex
        };

        $scope.confirmationBuy.action = 'постройка города';
        $scope.confirmationBuy.acceptClick = function (parameters) {
            $http.post(
                '/Game/BuildCity',
                postObject);
            $scope.confirmationBuy.show = false;
        };

        $scope.confirmationBuy.resourcesCost = [];
        $scope.confirmationBuy.resourcesCost.push({ Type: 0, Qty: 1 });
        $scope.confirmationBuy.resourcesCost.push({ Type: 1, Qty: 1 });
        $scope.confirmationBuy.resourcesCost.push({ Type: 2, Qty: 1 });
        $scope.confirmationBuy.resourcesCost.push({ Type: 3, Qty: 1 });


        $scope.confirmationBuy.show = true;
        console.log($scope.confirmationBuy);
    };

    function drawField(field) {
        hexMap = [];
        var maxFieldsInRow = field.length;
        var seaTileNumber = 0;
        var groundTileNumber = 0;

        for (var i = 0; i < field.length; i++) {
            var offsetPx = (maxFieldsInRow - field[i].length) / 2 * tileWidth;
            var offsetPy = i * 3 / 2 * a;
            for (var j = 0; j < field[i].length; j++) {
                var newTile = {};
                newTile.resourceType = field[i][j].ResourceType;
                newTile.left = offsetPx;
                newTile.width = tileWidth;
                newTile.height = tileHeight;
                newTile.top = offsetPy;
                newTile.id = i + '-' + j;
                if (field[i][j].FaceNumber != 0) {
                    newTile.faceNumber = field[i][j].FaceNumber;
                    newTile.isFaceNumberStyle = 'margin: 40px auto;';
                } else {
                    newTile.faceNumber = '';
                    newTile.isFaceNumberStyle = 'display: none; margin: 40px auto;';
                }
                newTile.tileStyle = "width: " + newTile.width + "px; height: " + newTile.height + "px;";
                newTile.tileStyle += " background-image: url(http://localhost:26346/Sprites/f" + newTile.resourceType + ".png); background-size: 100%;";
                newTile.tileStyle += " left: " + newTile.left.toString() + "px; top: " + newTile.top.toString() + "px; background-position: initial; background-repeat: initial;";
                offsetPx += tileWidth;
                var tileNum;
                if (i == 0 || i == field.length - 1 || j == 0 || j == field[i].length - 1) {
                    tileNum = --seaTileNumber;
                } else {
                    tileNum = ++groundTileNumber;
                }
                hexMap[tileNum] = newTile;
            }
        }
        return hexMap;
    };

    function getTownRelativePosition(num) {
        var dx = -1, dy = -1;
        if (num == 4 || num == 5) dx = 0;
        else if (num == 1 || num == 2) dx = tileWidth;
        else if (num == 0 || num == 3) dx = tileWidth / 2;
        if (num == 0) dy = 0;
        else if (num == 1 || num == 5) dy = a / 2;
        else if (num == 2 || num == 4) dy = a + a / 2;
        else if (num == 3) dy = tileHeight;
        return { dx: dx, dy: dy };
    };

    function getRoadAngle(num) {
        return num * 60 + 30;
    };

    function drawTowns(towns, isPossible) {
        var townsResult = [];
        if (towns == undefined || towns == null) return townsResult;

        for (var i = 0; i < towns.length; i++) {
            var newTown = {};

            if (!isPossible) {
                newTown.class = 'town';
            } else {
                newTown.class = 'possible-town';
            }

            var town = towns[i];
            var delta = getTownRelativePosition(town.Position);
            var size = town.CitySize == 'v' ? villageSize : townSize;
            newTown.width = size.w;
            newTown.height = size.h;
            newTown.owner = town.PlayerId;
            newTown.size = town.CitySize;
            newTown.hexagonIngex = town.HexagonIndex;
            newTown.position = town.Position;
            newTown.hexA = town.HexA;
            newTown.hexB = town.HexB;
            newTown.hexC = town.HexC;

            newTown.left = hexMap[town.HexagonIndex].left + delta.dx - size.w / 2;
            newTown.top = hexMap[town.HexagonIndex].top + delta.dy - size.h * 2 / 3;
            newTown.image = 'url(/Sprites/' + town.CitySize + town.PlayerId + '.png)';
            newTown.id = i;
            newTown.style = 'position: absolute; z-index: 3;';
            newTown.style += 'width: ' + newTown.width + 'px; height: ' + newTown.height + 'px; left: ' + newTown.left + 'px; top: ' + newTown.top + 'px;';
            newTown.style += 'background-image: ' + newTown.image + ';';
            newTown.style += 'background-size: 100%;';
            newTown.style += 'background-position:  initial; background-repeat: initial;';

            townsResult[i] = newTown;
        }
        return townsResult;
    };

    function drawRoads(roads, isPossible) {
        var roadsResult = [];
        if (roads == undefined || roads == null) return roadsResult;

        for (var i = 0; i < roads.length; i++) {
            var newRoad = {};

            if (!isPossible) {
                newRoad.class = 'town';
            } else {
                newRoad.class = 'possible-town';
            }
            var road = roads[i];
            var angle = getRoadAngle(road.Position);
            var startPoint = getTownRelativePosition(road.Position);
            newRoad.width = roadSize.w;
            newRoad.hexA = road.HexA;
            newRoad.hexB = road.HexB;
            newRoad.height = roadSize.h;
            newRoad.owner = road.PlayerId;
            newRoad.hexagonIngex = road.HexagonIndex;
            newRoad.left = hexMap[road.HexagonIndex].left + startPoint.dx;
            newRoad.top = hexMap[road.HexagonIndex].top + startPoint.dy - roadSize.h / 2;
            newRoad.image = 'url(/Sprites/r' + road.PlayerId + '.png);';
            newRoad.transform = 'rotate(' + angle + 'deg);';
            newRoad.webkitTransform = 'rotate(' + angle + 'deg);';
            newRoad.transformOrigin = 'left';
            newRoad.webkitTransformOrigin = 'left';
            newRoad.id = i;
            newRoad.style = 'position: absolute; width: 70px; height: 14px;';
            newRoad.style += 'left: ' + newRoad.left + 'px; top: ' + newRoad.top + 'px;';
            newRoad.style += 'background-image:' + newRoad.image + '';
            newRoad.style += '-webkit-transform: ' + newRoad.transform + '; -webkit-transform-origin: 0% 50%;';
            newRoad.style += 'transform: ' + newRoad.transform + '; transform-origin: 0% 50%;';
            newRoad.style += 'background-size: 100%;';
            newRoad.style += 'background-position: initial initial; background-repeat: initial initial;';

            roadsResult[i] = newRoad;
        }

        return roadsResult;
    };
};
