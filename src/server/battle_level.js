var game_level = require('./game_level.js');
var util = require('util');

var battle_level = function(){
	if (!(this instanceof battle_level)) return new battle_level();

	this.levelName = "battle";
};
module.exports = battle_level;

util.inherits(battle_level, game_level);

battle_level.prototype.someMethod = function(first_argument) {
	// body...
};

