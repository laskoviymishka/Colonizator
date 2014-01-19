var COS_30 = 0.86602540378443864676372317075294; // sqrt(3)/2
var a = 70;
var tileWidth = a * COS_30 * 2;
var tileHeight = a * 2;
var tester;
var hexMap = {};
var roadsContainer = [];
var townsContainer = [];
var townSize = { w: 50, h: 50 };
var villageSize = { w: 45, h: 36 };
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

			if (field[i][j].FaceNumber > 0) {
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

function ClearNodes(container) {
}

var cityRoad;

function GetCitiesAdnRoads() {
	$('.town').remove()
	$('.road').remove()
	$('.possible-town').remove()
	$.getJSON('/Game/CitiesAndRoads?token=' + token, function (data) {
		cityRoad = data;
		DrawTowns(data.Cities);
		DrawRoads(data.Roads);
		console.log('/Game/CitiesAndRoads?token=' + token);
	});
	$.getJSON('/Game/AvailableMap?playerId=' + userId + '&token=' + token, function (data2) {
		global_data = data2;
		console.log('/Game/AvailableMap?playerId=' + userId + '&token=' + token);
		DrawTowns(data2.Cities, true);
		DrawRoads(data2.Roads, true);
	});
}

function DrawTowns(towns, isPossible) {
	if (!isPossible) ClearNodes(townsContainer);
	var canvas = document.getElementById('canvas');
	var townElement = document.createElement('div');
	townElement.style.position = 'absolute';
	townElement.style.zIndex = '3';
	if (isPossible) {
		townElement.setAttribute('class', 'possible-town');
	} else {
		townElement.setAttribute('class', 'town');
	}
	townElement.innerHTML = '&nbsp;';
	for (var i = 0; i < towns.length; i++) {
		var town = towns[i];
		var delta = GetTownRelativePosition(town.Position);
		var newTown = townElement.cloneNode(true);
		var size = town.CitySize == 'v' ? villageSize : townSize;
		newTown.style.width = size.w + 'px';
		newTown.style.height = size.h + 'px';
		newTown.style.left = hexMap[town.HexagonIndex].offsetLeft + delta.dx - size.w / 2 + 'px';
		newTown.style.top = hexMap[town.HexagonIndex].offsetTop + delta.dy - size.h * 2 / 3 + 'px';
		newTown.style.background = 'url(/Sprites/' + town.CitySize + town.PlayerId + '.png)';
		newTown.style.backgroundSize = '100% auto';
		newTown.id = i;
		if (isPossible) {
			$(newTown).on('click', function () {
				var t1 = towns[this.id];
				$.post(
                    "/Game/BuildCity",
                    { token: token, playerId: userId, hexA: t1.HexA, hexB: t1.HexB, hexC: t1.HexC, hexIndex: t1.HexagonIndex });
				console.log('clicked town', t1.HexagonIndex, t1.Position, t1.HexA, t1.HexB, t1.HexC);
			});
		}

		canvas.appendChild(newTown);
		townsContainer.push(newTown);
	}
}

function DrawRoads(roads, isPossible) {
	if (!isPossible) ClearNodes(roadsContainer);
	var canvas = document.getElementById('canvas');
	var townRoad = document.createElement('div');
	townRoad.style.position = 'absolute';
	townRoad.innerHTML = '&nbsp;';
	if (isPossible) {
		townRoad.setAttribute('class', 'possible-town');
	} else {
		townRoad.setAttribute('class', 'road');
	}

	for (var i = 0; i < roads.length; i++) {
		var r = roads[i];
		var angle = GetRoadAngle(r.Position);
		var startPoint = GetTownRelativePosition(r.Position);
		var newRoad = townRoad.cloneNode(true);
		newRoad.style.width = roadSize.w + 'px';
		newRoad.style.height = roadSize.h + 'px';
		newRoad.style.left = hexMap[r.HexagonIndex].offsetLeft + startPoint.dx + 'px';
		newRoad.style.top = hexMap[r.HexagonIndex].offsetTop + startPoint.dy - roadSize.h / 2 + 'px';
		newRoad.style.background = 'url(/Sprites/r' + r.PlayerId + '.png)';
		newRoad.style.backgroundSize = '100% auto';
		newRoad.style.transform = 'rotate(' + angle + 'deg)';
		newRoad.style['WebkitTransform'] = 'rotate(' + angle + 'deg)';
		newRoad.style.transformOrigin = 'left';
		newRoad.style['WebkitTransformOrigin'] = 'left';
		newRoad.id = i;
		if (isPossible) {
			$(newRoad).on('click', function () {
				var r1 = roads[this.id];
				$.post("/Game/BuildRoad",
                    {
                    	token: token, playerId: userId, haxagonIndex: r1.HexagonIndex, hexA: r1.HexA, hexB: r1.HexB
                    }
				);
				console.log('clicked road', r1.HexagonIndex, r1.HexA, r1.HexB);
			});
		}

		canvas.appendChild(newRoad);
		roadsContainer.push(newRoad);
	}
}

function ApplyAllowedUserMoves() {

}