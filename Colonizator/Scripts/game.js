var COS_30 = 0.86602540378443864676372317075294; // sqrt(3)/2
var a = 70;
var tileWidth = a * COS_30 * 2;
var tileHeight = a * 2;

function DrawField(field) {
    var maxFieldsInRow = 7;
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

    for (var i = 0; i < field.length; i++) {
        var offsetPx = (maxFieldsInRow - field[i].length) / 2 * tileWidth;
        for (var j = 0; j < field[i].length; j++) {
            var newTile = tile.cloneNode(true);
            newTile.style.background = 'url(/Sprites/f' + field[i][j].type + '.png)';
            newTile.style.left = offsetPx + 'px';
            newTile.style.top = i * 3 / 2 * a + 'px';
            canvas.appendChild(newTile);

            if (field[i][j].number > 0) {
                var newNumber = numberElement.cloneNode(true);
                newNumber.innerHTML = field[i][j].number;
                newTile.appendChild(newNumber);
            }
            newTile.style.backgroundSize = '100% auto';
            offsetPx += tileWidth;
        }
    }
}
