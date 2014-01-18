﻿var COS_30 = 0.86602540378443864676372317075294; // sqrt(3)/2
var a = 70;
var tileWidth = a * COS_30 * 2;
var tileHeight = a * 2;

var hexMap = {};
var townSize = { w: 70, h: 70 };
var villageSize = { w: 50, h: 40 };
var roadSize = { w: 70, h: 14 };

function DrawField(field) {
    var maxFieldsInRow = field.length;
    var canvas = document.getElementById('canvas');
    canvas.innerHTML = '';

    var tile = document.createElement('div');
    tile.setAttribute("class", 'tile');
    tile.style.width = tileWidth + 'px';
    tile.style.height = tileHeight + 'px';
    tile.innerHTML = '&nbsp;';

    var numberElement = document.createElement('div');
    numberElement.setAttribute("class", 'tile_number');
    numberElement.style.margin = tileHeight / 2 - 30 + 'px auto';

    var seaTileNumber = 0;
    var groundTileNumber = 0;

    for (var i = 0; i < field.length; i++) {
        var offsetPx = (maxFieldsInRow - field[i].length) / 2 * tileWidth;
        var offsetPy = i * 3 / 2 * a + 'px';
        for (var j = 0; j < field[i].length; j++) {
            var newTile = tile.cloneNode(true);
            newTile.style.background = 'url(/Sprites/f' + field[i][j].ResourceType + '.png)';
            newTile.style.left = offsetPx + 'px';
            newTile.style.top = offsetPy;
            canvas.appendChild(newTile);

            if (field[i][j].FaceNumber >= 0) {
                var newNumber = numberElement.cloneNode(true);
                newNumber.innerHTML = field[i][j].FaceNumber;
                newTile.appendChild(newNumber);
            }
            newTile.style.backgroundSize = '100% auto';
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
}

function GetTownRelativePosition(num) {
    var dx = -1, dy = -1;
    if (num == 4 || num == 5) dx = 0;
    else if (num == 1 || num == 2) dx = tileWidth;
    else if (num == 0 || num == 3) dx = tileWidth / 2;
    if (num == 0) dy = 0;
    else if (num == 1 || num == 5) dy = a / 2;
    else if (num == 2 || num == 4) dy = a + a / 2;
    else if (num == 3) dy = tileHeight;
    return { dx: dx, dy: dy };
}

function GetRoadAngle(num) {
    return num * 60 + 30;
}

function DrawTowns(towns) {
    var canvas = document.getElementById('canvas');
    var townElement = document.createElement('div');
    townElement.style.position = 'absolute';
    townElement.style.zIndex = '3';
    townElement.innerHTML = '&nbsp;';
    townElement.setAttribute('object-type', 'town');
    for (var i = 0; i < towns.length; i++) {
        var delta = GetTownRelativePosition(towns[i].Position);
        var newTown = townElement.cloneNode(true);
        var size = towns[i].CitySize == 'v' ? villageSize : townSize;
        newTown.style.width = size.w + 'px';
        newTown.style.height = size.h + 'px';
        newTown.style.left = hexMap[towns[i].HexagonIndex].offsetLeft + delta.dx - size.w / 2 + 'px';
        newTown.style.top = hexMap[towns[i].HexagonIndex].offsetTop + delta.dy - size.h * 2 / 3 + 'px';
        newTown.style.background = 'url(/Sprites/' + towns[i].CitySize + towns[i].PlayerId + '.png)';
        newTown.style.backgroundSize = '100% auto';

        canvas.appendChild(newTown);
    }
}

function DrawRoads(roads) {
    var canvas = document.getElementById('canvas');
    var townRoad = document.createElement('div');
    townRoad.style.position = 'absolute';
    townRoad.innerHTML = '&nbsp;';
    townRoad.setAttribute('object-type', 'road');

    for (var i = 0; i < roads.length; i++) {
        var angle = GetRoadAngle(roads[i].Position);
        var startPoint = GetTownRelativePosition(roads[i].Position);
        var newRoad = townRoad.cloneNode(true);
        newRoad.style.width = roadSize.w + 'px';
        newRoad.style.height = roadSize.h + 'px';
        newRoad.style.left = hexMap[roads[i].HexagonIndex].offsetLeft + startPoint.dx + 'px';
        newRoad.style.top = hexMap[roads[i].HexagonIndex].offsetTop + startPoint.dy - roadSize.h / 2 + 'px';
        newRoad.style.background = 'url(/Sprites/r' + roads[i].PlayerId + '.png)';
        newRoad.style.backgroundSize = '100% auto';
        newRoad.style.transform = 'rotate(' + angle + 'deg)';
        newRoad.style.transformOrigin = 'left';

        canvas.appendChild(newRoad);
    }
}