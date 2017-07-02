var user = require("./accounts/user.js");
var commands = require("./commands.js");

var message_parser = {};

//aliases
module.exports = message_parser;
var mp = message_parser;

//variable declarations
message_parser.prefix = "!";

//function implementations
message_parser.init = function(client){
	client.on('chat', mp.onChat);
}

message_parser.onChat = function(channel, userstate, message, self){
	if(self)
		return;

	var id = userstate['user-id'];
	if(!user.check(userstate)){
		user.add(userstate);
	}

	if(message.startsWith(mp.prefix)){
		var cmd = mp.prepareCommand(message);
		commands.execute(cmd.command, cmd.args, userstate);
	}
};

message_parser.prepareCommand = function(message){
	var full_command = message.substring(mp.prefix.length);
	var args = full_command.split(' ');
	if(args.length > 0){
		var command = args[0];
		args.shift();

		return {'command': command, 'args': args};
	}
}