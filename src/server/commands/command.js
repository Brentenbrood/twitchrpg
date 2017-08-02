//all the other commands

var all_commands = [
	require('./echo.js'),
	require('./ping.js'),
    require('../battle/attack.js'),
    require('./debug_command.js')
];

var command = {};
module.exports = command;

//variable declaration
command.list = {};

//function implementations
command.add = function(command_name, callback, allow_overwrite = false){
	if(command.isRegistered(command_name) && !allow_overwrite)
		throw "command_name: '" + command_name + "' is trying to overwrite an already defined function";

	command.list[command_name] = callback;
};

command.remove = function(command_name){
	if(!command.isRegistered(command_name))
		throw "command_name: '" + command_name + "' is not registered but tries to be deleted";

	delete command.list[command_name];
};

command.get = function(command_name){
	if(command.isRegistered(command_name))
		return command.list[command_name];

	console.warn("trying to get a command with name: '" + command_name + "' but it is not registered");
	return null;
};

command.isRegistered = function(command_name){
	return command.list.hasOwnProperty(command_name);
};

//global command methods
command.isCurrentlyRegistered = function(command_name){
	return gamestate.current_level.local_commands.hasOwnProperty(command_name) ||
		command.isRegistered(command_name);
}

command.getAvailableCommand = function(command_name){
	//TODO: Make this not have a double check
	if(command.isCurrentlyRegistered(command_name))
		return gamestate.current_level.local_commands.hasOwnProperty(command_name) ? 
			gamestate.current_level.local_commands[command_name] : command.get(command_name);

	console.warn("trying to get a command with name: '" + command_name + "' but it is not registered");
	return null;
}

command.execute = function(command_name, args, userstate){
	if(command.isCurrentlyRegistered(command_name)){
		var callback = command.getAvailableCommand(command_name);
		callback(args, userstate);
	}else{
		console.log("%s tried to execute the command_name: \"%s\", but it is not valid", userstate.username, command_name);
	}
};

//register the commands
for (var i = 0; i < all_commands.length; i++) {
	var cmd = all_commands[i];
	command.add(cmd.command_name, cmd.execute);
}
