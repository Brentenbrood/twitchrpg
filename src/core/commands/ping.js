var ping = {};
module.exports = ping;

ping.command_name = "ping";

ping.execute = function(args, userdata){
	client.say('brentolinni', 'pong!');
}