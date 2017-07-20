var game_level = require('./game_level.js')();

var gamestate = {};
module.exports = gamestate;

gamestate.current_level = game_level;
gamestate.levels = {};

gamestate.switchLevel = function(levelName){
	var level = gamestate.getLevel(levelName);
	if(level == null)
		throw "Trying to switch to level: '" + levelName + "' but it is not registered as a level!";

	gamestate.current_level = level;
}

gamestate.addLevel = function(level){
	if(gamestate.getLevel(level.levelName) != null){
		console.warn("WARNING: the key" + level.levelName + "is already defined and is now overwriting the level!");
	}
	gamestate.levels[level.levelName] = level;
}

gamestate.getLevel = function(key){
	if(gamestate.levels.hasOwnProperty(key))
		return gamestate.levels[key];

	return null; 
}