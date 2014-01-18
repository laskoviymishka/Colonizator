var COS_30 = 0.86602540378443864676372317075294; // sqrt(3)/2
var a = 70;
var tileWidth = a * COS_30 * 2;
var tileHeight = a * 2;

var hexMap = {};


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
            newTile.style.background = 'url(/Sprites/f' + field[i][j].type + '.png)';
            newTile.style.left = offsetPx + 'px';
            newTile.style.top = offsetPy;
            canvas.appendChild(newTile);

            if (field[i][j].number > 0) {
                var newNumber = numberElement.cloneNode(true);
                newNumber.innerHTML = field[i][j].number;
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

function DrawTowns(towns) {
    var townElement = document.createElement('div');
    townElement.style.position = 'absolute';
    townElement.innerHTML = '&nbsp;';

    for (var i = 0; i < towns.length; i++) {
        var delta = GetTownRelativePosition(towns[i].vertex);
        var spriteUrl = 'url(/Sprites/' + towns[i].type + towns[i].user + '.png)';
        var newTown = townElement.cloneNode(true);
        newTown.style.left = hexMap[towns[i].hex].offsetWidth + delta.dx - (towns[i].type == 'v' ? 100 : 150) / 2 + 'px';
        newTown.style.top = hexMap[towns[i].hex].offsetHeight + delta.dy - 50 + 'px';
        newTown.style.background = spriteUrl;

        var canvas = document.getElementById('canvas');
        canvas.appendChild(newTown);
    }
}