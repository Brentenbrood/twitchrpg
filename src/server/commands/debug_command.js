var debug_command = {};
module.exports = debug_command;

debug_command.command_name = "debug";

debug_command.execute = function(args, userdata){
	if(args.length > 0){
		switch(args[0]){
			case "currentlevel":
				console.log(gamestate.current_level.levelName + " : " + gamestate.current_level);
				break;
			case "levelcommands":
				Object.keys(gamestate.current_level.local_commands).forEach(function(key){
					console.log("%s: %s", key, gamestate.current_level.local_commands[key].command_name);
				})
				break;
		}
	}
}