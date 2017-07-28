var user = require("./accounts/user.js");
var commands = require("./commands/command.js");

var message_parser = {};
module.exports = function(tmi_client){
	if(typeof tmi_client !== 'undefined' && tmi_client !== null){
		message_parser.init(tmi_client);
	}

	return message_parser;
};

//variable declarations
message_parser.prefix = "!";

//function implementations
message_parser.init = function(tmi_client){
	tmi_client.on('chat', message_parser.onChat);
}

/**
 * Callback for a chat message event
 * @param channel {string} the channel the message came from
 * @param userstate {object} the userstate object https://docs.tmijs.org/v1.2.1/Events.html#chat
 * @param message {string} the message
 * @param fromSelf {boolean} is the command send out by this bot?
 */
message_parser.onChat = function(channel, userstate, message, fromSelf){
	if(fromSelf)
		return;

	var id = userstate['user-id'];
	if(user.check(userstate)){
		user.add(userstate);
	}

	if(message.startsWith(message_parser.prefix)){
		var cmd = message_parser.prepareCommand(message);
		commands.execute(cmd.command, cmd.args, userstate);
	}
};

/**
 * Splits up the chat message into usable pieces for the command system
 * @param message {string} 
 * @return {object} with the properties: 'commmand' and 'args'
 */
message_parser.prepareCommand = function(message){
	//TODO: Take a look at the nodejs module 'minimist' for implementing arguments and optional arguments
	var full_command = message.substring(message_parser.prefix.length);
	var args = full_command.split(' ');
	if(args.length > 0){
		var command = args[0];
		args.shift();

		return {'command': command, 'args': args};
	}
};