// var attack = {};
// module.exports = attack;
// var users = require('../accounts/users.json');
//
// attack.command_name = "attack";
//
// attack.execute = function(args, userstate){
//     var enemy  = args;
//     var damage = users.players[userstate["user-id"]].attack;
//     client.say(config.channel, userstate["display-name"] + " did " + damage + " damage to "+ enemy +"!");
//     var data = {
//         'username': userstate["display-name"],
//         'enemy': enemy,
//         'damage': damage,
//     };
//     data = JSON.stringify(data);
//     data = data.toString('utf8');
//     console.log(data);
//     socketserver.broadcast(data);
//     //socketserver.broadcast(userstate["display-name"] + " did " + damage + " damage to "+ enemy +"!");
// };

var jsonrequest = require('./../jsonrequest.js');
var users = require('../accounts/users.json');

var attack = {};
module.exports = function(battle_level){
    attack.battle_level = battle_level;

    return attack;
};

attack.command_name = "attack";

attack.execute = function(args, userdata){
    var damage = users.players[userdata["user-id"]].attack;
    var data = {
        'username': userdata["display-name"],
        'enemy': "monster",
        'damage': damage,
    };
    data = JSON.stringify(data);
    data = data.toString('utf8');

    if(args.length > 0){
        var dirtext = args[0];
        switch(dirtext){
            case "monster":
                client.say(config.channel, "Attacking monster!");
                var request = new jsonrequest("Attack", data, true);
                socketserver.broadcast(request.getJSON());
                break;
        }
    }
}