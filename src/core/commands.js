

var commands = {};
module.exports = commands;

//variable declaration
commands.list = {};

//function implementations
commands.add = function(command, callback, allow_overwrite = false){
	if(commands.isRegistered(command) && !allow_overwrite)
		throw "command: '" + command + "' is trying to overwrite an already defined function";

	commands.list[command] = callback;
}

commands.remove = function(command){
	if(!commands.isRegistered(command))
		throw "command: '" + command + "' is not registered but tries to be deleted";

	delete commands.list[command];
}

commands.isRegistered = function(command){
	return commands.hasOwnProperty(command);
}

commands.execute = function(command, args, userstate){
	if(isRegistered(command)){
		var callback = commands.list[command];
		callback(args);
	}else{
		console.log("%s tried to execute the command: \"%s\", but it is not valid", userstate.username, command);
	}
}