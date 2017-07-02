var user = require("./accounts/user.js");
var commands = require("./commands/command.js");

var message_parser = {};
module.exports = message_parser;

//aliases
var mp = message_parser;

//variable declarations
message_parser.prefix = "!";

//function implementations
message_parser.init = function(client){
	client.on('chat', mp.onChat);
}

/**
 * @param channel {string}
 * @param userstate {object} the userstate object https://docs.tmijs.org/v1.2.1/Events.html#chat
 * @param message {string} 
 * @param self {boolean} is the command send out by this bot?
 */
message_parser.onChat = function(channel, userstate, message, self){
	if(self)
		return;

	var id = userstate['user-id'];
	if(user.check(userstate)){
		user.add(userstate);
	}

	if(message.startsWith(mp.prefix)){
		var cmd = mp.prepareCommand(message);
		commands.execute(cmd.command, cmd.args, userstate);
	}
};

/**
 * plits up the chat message into usable pieces for the command system
 * @param message {string} 
 * @return {object} with the properties: 'commmand' and 'args'
 */
message_parser.prepareCommand = function(message){
	var full_command = message.substring(mp.prefix.length);
	var args = full_command.split(' ');
	if(args.length > 0){
		var command = args[0];
		args.shift();

		return {'command': command, 'args': args};
	}
}