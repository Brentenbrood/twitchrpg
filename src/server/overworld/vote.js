var alias = require('./../commands/alias.js');
var jsonrequest = require('./../jsonrequest.js');

var vote = {};

vote.command_name = "vote";

vote.execute = function(args, userdata){
	if(args.length > 0){
		var dirtext = args[0];
		switch(dirtext){
			case "forward":
				client.say(config.channel, "Sending forward...");
				var request = new jsonrequest("AddVote", {"direction": "forward"}, true);
				socketserver.broadcast(request.getJSON());
				break;
			case "backward":
				client.say(config.channel, "Sending backwards...");
				var request = new jsonrequest("AddVote", {"direction": "backward"}, true);
				socketserver.broadcast(request.getJSON());
				break;
			case "left":
				client.say(config.channel, "Sending left...");
				var request = new jsonrequest("AddVote", {"direction": "left"}, true);
				socketserver.broadcast(request.getJSON());
				break;
			case "right":
				client.say(config.channel, "Sending right...");
				var request = new jsonrequest("AddVote", {"direction": "right"}, true);
				socketserver.broadcast(request.getJSON());
				break;
		}
	}
}

module.exports = function(overworld_level){
	vote.overworld_level = overworld_level;

	alias.add("forward", vote, ["forward"]);
	alias.add("backward", vote, ["backward"]);
	alias.add("left", vote, ["left"]);
	alias.add("right", vote, ["right"]);

	return vote;
};

