var attack = {};
module.exports = attack;
var users = require('../accounts/users.json');

attack.command_name = "attack";

attack.execute = function(args, userstate){
    var enemy  = args;
    var damage = users[userstate["user-id"]].attack;
    client.say(config.channel, userstate["display-name"] + " did " + damage + " damage to "+ enemy +"!");
    var data = {
        'username': userstate["display-name"],
        'enemy': enemy,
        'damage': damage,
    };
    var socket = socketserver.broadcast(userstate["display-name"] + " did " + damage + " damage to "+ enemy +"!");
};