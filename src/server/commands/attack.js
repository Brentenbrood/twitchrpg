var attack = {};
module.exports = attack;
var users = require('../accounts/users.json');

attack.command_name = "attack";

attack.execute = function(args, userstate){
    var enemy  = args;
    var damage = users[userstate["user-id"]].attack;
    client.say("BRENTOLINNI", userstate["display-name"] + " did " + damage + " damage to "+ enemy +"!");
};