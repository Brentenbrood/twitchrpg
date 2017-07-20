var game_level = require('./../game_level.js');
var util = require('util');

var overworld_level = function(){
	if (!(this instanceof overworld_level)) return new overworld_level();

	this.levelName = "overworld";

	var self = this;
	var commands = [require('./vote.js')(this)];
	commands.forEach(function(cmd){
		self.local_commands[cmd.command_name] = cmd.execute;
	});
};
module.exports = overworld_level;


util.inherits(overworld_level, game_level);



var voting = false;

overworld_level.prototype.isVoting = function() {
	return this.voting;
};
overworld_level.prototype.setVoting = function(newVoting){
	this.voting = newVoting;
}

