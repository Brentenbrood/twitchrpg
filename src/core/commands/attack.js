var attack = { };
var users = require('../accounts/users.json');
var command = require('./command.js');
module.exports = attack;

var command_name = "attack";

function Attack(args,userstate){
    var damage = users[userstate["user-id"]];
    console.log(userstate["display-name"] + " did " + damage + " damage!");
}
command.add(command_name, Attack);