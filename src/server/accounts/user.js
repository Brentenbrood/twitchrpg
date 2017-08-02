var fs = require('fs');
var users = require('./users.json');
var user = { };
module.exports = user;

user.check = function(userstate){
    for (var user_id in users.players) {
        if(userstate["user-id"] == user_id) {
            //console.log(userstate["user-id"] + " already exists");
            return false;
        }
    }
    return true;
};
user.add = function(userstate){
    users.players[userstate["user-id"]] = {
        "name": userstate["display-name"],
        "level": 1,
        "attack": 20,
        "xp": 0
    };
    console.log(users);
        fs.writeFile('./src/server/accounts/users.json', JSON.stringify(users), function (err) {
            if(err == !null){
                console.log(err);
            } else {
                console.log('database updated');
            }
        });
};
user.update = function(values){
    users.players[userstate["user-id"]] = {
        "level": values.xp,
        "xp": values.xp
    };
};
user.getAll = function(){
    var data = users;
    return data;
};