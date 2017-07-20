var ping = {};
module.exports = ping;

ping.command_name = "ping";

ping.execute = function(args, userdata){
	gamestate.current_level.pingTwitchAsync().then((latency)=>{
		client.say(config.channel, 'pong! after: ' + latency);
		console.log(latency);
	});
	console.log("starting to ping...");
};