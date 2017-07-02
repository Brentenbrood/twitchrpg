var echo = {};
module.exports = echo;

echo.execute = function(args, userdata){
	var total = "";
	console.log(args.length);
	for(var i = 0; i < args.length; i++){
		total += args[i] + " ";
	}

	console.log(typeof(total) == "undefined");
	console.log(total);
	client.say("BRENTOLINNI", total);
}