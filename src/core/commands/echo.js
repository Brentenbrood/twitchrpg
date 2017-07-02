var echo = {};
module.exports = echo;

echo.execute = function(args, userdata){
	var total = "";
	for(var i = 0; i < args.length; i++){
		total += args[i] + " ";
	}

	client.say("BRENTOLINNI", total);
}