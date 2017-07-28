//TODO: Create 4 direct commands: !left !right !forward !back

var jsonrequest = require('./../jsonrequest.js');

var vote = {};
module.exports = function(overworld_level){
	vote.overworld_level = overworld_level;

	return vote;
};

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