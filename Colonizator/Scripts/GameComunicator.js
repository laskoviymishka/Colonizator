
var gameHub;
var mapHub;
var markethub;
var userId = -1;
var userMove;
var userName;
var isInQueue = false;
var token;
var global_data;
var currentUser;
$(function () {
    mapHub = $.connection.mapHub;

    mapHub.client.gameEnd = function (data) {
        toastr.success("Поздравляем победителя. Все остальные лузеры.", data);
    };

    mapHub.client.updateToast = function (data) {
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
    };
    mapHub.client.updateState = function (data) {
        console.log("update state", data);
        userMove = data.movePlayer;
        switch (data.Args.Action) {
            case 0://"NextMove":
                console.log("updateState switch (data.Args.Action) NextMove", data);
                GetCitiesAdnRoads();
                GetPartials();
                if (!data.isStartUp) {
                    NeedThrowDice();
                }
                PassMoveAvaible();
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
                break;
            case 4://"FreeResource":
                chooseFreeResouce(data);
                console.log("updateState switch (data.Args.Action) FreeResource", data);
                break;
            case 5:// "CardUpdate":
                console.log("updateState switch (data.Args.Action) CardUpdate", data);
                GetPartials();
                break;
            case 6://"RegularUpdate":
                console.log("updateState switch (data.Args.Action) RegularUpdate", data);
                GetCitiesAdnRoads();
                GetPartials();
                PassMoveAvaible();
                moveRobber(data);
                break;
            default:
                console.log("updateState switch (data.Args.Action) default", data);
                break;
        }
    };

    mapHub.client.throwenDice = throwenDice;

    mapHub.client.updateQueue = function (eventArgs) {
        console.log("update queue");
        if (!isInQueue) {
            userId = eventArgs.playerCount - 1;
            isInQueue = true;
            currentUser = eventArgs.player;
        }
        $('#userQueue').html("Начат поиск игры вы " + (userId + 1) + "-й в очереди");
    };

    mapHub.client.gameStart = function (eventArgs) {
        console.log("start game");
        $('#preGameState').hide();

        if (!isInQueue) {
            userId = eventArgs.playerCount - 1;
            currentUser = eventArgs.player;
            isInQueue = true;
        }
        token = eventArgs.token;
        $('#UserName').html(userName);
        $('#UserColor').html(userId);
        $('#tokenGame').html(token);

        $.getJSON('/Game/Map?token=' + token, function (data) {
            DrawField(data);
            GetCitiesAdnRoads();
        });
        GetPartials();
    };

    $('#displayname').focus();
    // Start the connection.
    $.connection.hub.start().done(function () {
        $('#search').click(function () {
            userName = $('#displayname').val();
            mapHub.server.searchGame($('#displayname').val());
            $('#creatingGame').hide();
            $('#userQueue').html("Начат поиск игры");
        });
    });
    $('#throwDiceBtn').click(function () {
        $.get('/Game/ThrowDice?playerId=' + userId + '&token=' + token, function (data) {
            console.log('/Game/ThrowDice?playerId=' + userId + '&token=' + token);
            $('#throwDiceResult').html(data);
            $('#throwDiceCloseBtn').show();
        });
    });
    $('#passMove').hide();
    $('#passMove').click(PassMove);
});
// This optional function html-encodes messages for display in the page.
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}

function toQueue() {
}

function NeedThrowDice() {
    console.log('NeedThrowDice');
    if (currentUser == userMove) {
        $('#throwDiceResult').html('Ваш ход, бросьте кубики');
        $('#throwDiceModal').modal('show');
        $('#throwDiceCloseBtn').hide();
        $('#throwDiceBtn').show();
    } else {
        $('#throwDiceResult').html('Ход ' + userMove + ' , ждите пока он бросит кубики')
        $('#throwDiceModal').modal('show');
        $('#throwDiceCloseBtn').hide();
        $('#throwDiceBtn').hide();
    }
    togleCube(0, 1);
    togleCube(1, 1);
}

function PassMoveAvaible() {
    console.log('PassMoveAvaible');
    if (currentUser == userMove) {
        $('#passMove').show();
    } else {
        $('#passMove').hide();
    }
}

function PassMove() {
    console.log('PassMove');
    if (currentUser == userMove) {
        $('#passMove').show();
        $.post(
                "/Game/PassMove",
                {
                    token: token,
                    playerId: userId
                });
    }
}

function throwenDice(data) {
    console.log('throwenDice', data);
    if (currentUser == userMove) {
        $('#throwDiceResult').html('Вам выпало ' + data.Args.First + ' и ' + data.Args.Second);
    } else {
        $('#throwDiceResult').html(userMove + ' выпало ' + data.Args.First + ' и ' + data.Args.Second);
    }
    $('#throwDiceCloseBtn').show();
    $('#throwDiceBtn').hide();
    togleCube(0, data.Args.First);
    togleCube(1, data.Args.Second);
    GetPartials();
}

function GetPartials() {
    $.get('/Game/GameStatePartial?playerId=' + userId + '&token=' + token, function (data) {
        console.log('/Game/GameStatePartial?playerId=' + userId + '&token=' + token);
        $('#gameState').html(data);
    });
    $.get('/Game/MarketPartial?playerId=' + userId + '&token=' + token, function (data) {
        console.log('/Game/MarketPartial?playerId=' + userId + '&token=' + token);
        $('#gameMarket').html(data);
    });
    $.get('/Game/DeckPartial?playerId=' + userId + '&token=' + token, function (data) {
        console.log('/Game/DeckPartial?playerId=' + userId + '&token=' + token);
        $('#deckPartial').html(data);
    });
}

var moveRobber = function (args) {
    var robberElement = document.createElement('div');
    robberElement.setAttribute("class", 'tile_number');
    robberElement.style.margin = tileHeight / 2 - 30 + 'px auto';
    robberElement.id = "robber";
    if (currentUser == userMove) {
        tileClick = function (parameters) {
            $('.tile_number').removeAttr("hidden");
            console.log("MoveRobber", parameters, args);
            var contract = { token: token, playerId: userId, hexagonIngex: parameters.HexagonIndex };
            $.post("/Game/MoveRobber", $.toDictionary(contract));
            console.log("/Game/MoveRobber", contract);
            $('#robber').remove();
            $('#2-3>div').attr('hidden', 'true');
            var tile = document.getElementById(parameters.id);
            robberElement.innerHTML = "R";
            tile.appendChild(robberElement);
        };
    }
};

function resumeGame(token) {
    mapHub.server.resumeGame(token, $('#resumeName').val());
};

var chosenResourcs;
var neededResourceCount = 7;
var neededResourceQty = -1;
var chooseMonopolyResource = function (args) {
    chosenResourcs = [
        { Type: 0, Qty: 0 },
        { Type: 1, Qty: 0 },
        { Type: 2, Qty: 0 },
        { Type: 3, Qty: 0 },
        { Type: 4, Qty: 0 }
    ];
    neededResourceCount = 1;
    neededResourceQty = 1;
    if (currentUser == userMove) {
        $('#freeResourceChoose').modal('show');
        $('#resourceCheckLabe').html('Играя эту карточку, игрок выбирает один тип ресурса. Остальные игроки должны отдать ему все карточки ресурсов этого типа');
        $('#freeResourceChooseSendBtn').hide();
        $('#freeResourceChooseSendBtn').on('click', function (parameters) {
            console.log('chooseMonopolyResource', parameters);
            var contract = {
                Token: token,
                PlayerId: userId,
                resource: 1,
            };
            $.post("/Game/ChooseMonopolyResource", contract);
            console.log("/Game/ChooseMonopolyResource", contract);
        });
    }
};

var chooseFreeResouce = function (args) {
    chosenResourcs = [
        { Type: 0, Qty: 0 },
        { Type: 1, Qty: 0 },
        { Type: 2, Qty: 0 },
        { Type: 3, Qty: 0 },
        { Type: 4, Qty: 0 }
    ];
    neededResourceCount = 2;
    neededResourceQty = 1;
    if (currentUser == userMove) {
        $('#freeResourceChoose').modal('show');
        $('#resourceCheckLabe').html('Выберете 2 ресурса, которые получите бесплатно в результате розыгрыша карты "изобилие"');
        $('#freeResourceChooseSendBtn').hide();
        $('#freeResourceChooseSendBtn').on('click', function (parameters) {
            console.log('chooseFreeResouce', parameters);
            var contract = {
                Token: token,
                playerId: userId,
                first: 1,
                second: 2
            };
            $.post("/Game/ChooseFreeResource", contract);
            console.log("/Game/ChooseFreeResource", contract);
        });
    }
};

var updateChosenResource = function (resource) {
    console.log('updateChosenResource', chosenResourcs);
    var selector = '#chooseResource' + resource;
    var card = $(selector);
    var selectedValueCount = 0;
    if (card.hasClass('unchooseResourceTemplate')) {
        card.addClass('chooseResourceTemplate');
        card.removeClass('unchooseResourceTemplate');
        chosenResourcs[resource].Qty = 1;
    } else {
        card.addClass('unchooseResourceTemplate');
        card.removeClass('chooseResourceTemplate');
        chosenResourcs[resource].Qty = 0;
    }

    for (var i = 0; i < chosenResourcs.length; i++) {
        if (chosenResourcs[i].Qty == neededResourceQty) {
            selectedValueCount++;
        }
    }

    if (selectedValueCount == neededResourceCount) {
        $('#freeResourceChooseSendBtn').show();
    } else {
        $('#freeResourceChooseSendBtn').hide();
    }
    console.log('updateChosenResource', chosenResourcs);
};