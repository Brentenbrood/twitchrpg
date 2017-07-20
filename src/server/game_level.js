var socketserver = require('./socketserver.js');
var user = require('./accounts/user.js');

var game_level = function(){
	if (!(this instanceof game_level)) return new game_level();

	//Do extra stuffs
};
module.exports = game_level;

/**
 * A list of commands that can only be executed when the level is active.
 * If a command is in local_commands and in the global list, the local command will get executed
 * @type {Object} a dictionairy of the local commands
 */
game_level.prototype.local_commands = {};

/**
 * A list of api handlers, indexed by JsonRequest.type
 * @type {Object} a dictionairy indexed by jsonrequest which should handle(and return) api requests
 */
game_level.prototype.local_apis = {};

game_level.prototype.levelName = "Base level class";

/**
 * Gets a stringified representation of the jsom of all the saved users in a async way
 * @return {Promise} A promise which returns the stringyfied json of all saved users
 */
game_level.prototype.getAllPlayersAsync = function(){
	return new Promise((resolve, reject) => {
		var users = game_level.getAllPlayers();
		resolve(users);
	});
}

/**
 * Gets the stringified representation of the jsom of all the saved users
 * @return {string} the stringified representation of the jsom of all the saved users
 */
game_level.prototype.getAllPlayers = function(){
	var users = user.getAll();
	return users;
}

/**
 * A wrapper which fires when the event 'pong' has been received
 * This is a test function, also client.ping is already a promise
 * @return {Promise} returns a promise which returns the latency to the twitch server
 */
game_level.prototype.pingTwitchAsync = function(){
	return new Promise((resolve, reject) => {
		client.once("pong", (latency) => {
			resolve(latency);
		});
		client.ping();
	});
}