var command_list = require('./command.js');

var alias = {};
module.exports = alias;

alias.command_name = "alias";

alias.list = [];

alias.execute = function(args, userdata){

	if(userdata["mod"]){
		if(args.length > 0){
			if(args[0] == "add"){
				if(args.length > 2){
					//TODO: Make the mods be able to add aliases
				}else{
					client.say(config.channel, "@" + userdata['display-name'] + " usage: !alias add newname commandname [optional args]");
				}
			}
		}else{
			client.say(config.channel, "@" + userdata['display-name'] + " usages: !alias add newname commandname [optional args] , !alias remove newname");
		}
	}
}

var alias_command = function(newname, command, base_args = []){
	if (!(this instanceof alias_command)) return new alias_command();
	
	this.command_name = newname;
	this.reference_command = command;
	this.base_args = base_args;

	this.execute = function(args, userdata){
		var both_args = base_args.concat(args);
		command.execute(both_args, userdata);
	}
}

alias.add = function(newname, command, args = []){
	var newalias = new alias_command(newname, command, args);

	alias.list.push(newalias);

	command_list.add(newalias.command_name, newalias.execute);
}