var game_level = require('./game_level.js');
var util = require('util');

var battle_level = function(){
	if (!(this instanceof battle_level)) return new battle_level();

	this.levelName = "battle";
    var self = this;
    var commands = [require('./battle/attack.js')(this)];
    commands.forEach(function(cmd){
        self.local_commands[cmd.command_name] = cmd.execute;
    });
};
module.exports = battle_level;

util.inherits(battle_level, game_level);

battle_level.prototype.someMethod = function(first_argument) {
	// body...
};

