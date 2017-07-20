//Initial requires.
var tmi = require('tmi.js');
var user = require('./src/server/accounts/user.js');
socketserver = require('./src/server/socketserver.js');

config = require('./config.js');

//This is creating our client connection with settings.
client = new tmi.client(config.options);
var message_parser = require('./src/server/message_parser.js')(client);

gamestate = require('./src/server/gamestate.js');
var overworld_level = require('./src/server/overworld/overworld_level.js')();
var battle_level = require('./src/server/battle_level.js')();
gamestate.addLevel(overworld_level);
gamestate.addLevel(battle_level);
gamestate.switchLevel(overworld_level.levelName);

//This connects to the twitch.
client.connect();

//This function is executed as soon as the bot has connected to the channel.
client.on("connected", function(address, port){
  config.options.channels.forEach(function(channel){
      client.action(channel, "You have summoned me.");
  });
});

//This function is executed everytime someone sends a message in the chat.
client.on("chat", function(channel, userstate, message){
  //Using this if statement you can check the contents of a message and create commands.
  //This checks the contents of the message to see if they match the given message, this means that the message HAS to be that it cannot just contain that message.
  //This allows you to set commands or even words without worrying if someone was to use the word in a sentence or so on.
});