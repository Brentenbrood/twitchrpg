//Initial requires.
var tmi = require('tmi.js');
var config = require('./config.js');
var user = require('./src/core/accounts/user.js');

//These are the settings for the client to use.
var options = {
  options: {
    debug: true
  },

  connection: {
    cluster: "aws",
    reconnect: true
  },

  identity: {
    username: "brentolinni",
    password: "oauth:3mr7wbq778ezrw2u6excq0ntv7fnkr"
  },
  channels: ["BRENTOLINNI"]
};

//This is creating our client connection with settings.
var client = new tmi.client(config.options);

//This connects to the twitch.
client.connect();

//This function is executed as soon as the bot has connected to the channel.
client.on("connected", function(address, port){
  client.action("channel name or use the channels array if you want it on all channels.", "You have summoned me.");
});

//This function is executed everytime someone sends a message in the chat.
client.on("chat", function(channel, userstate, message){
  //Using this if statement you can check the contents of a message and create commands.
  //This checks the contents of the message to see if they match the given message, this means that the message HAS to be that it cannot just contain that message.
  //This allows you to set commands or even words without worrying if someone was to use the word in a sentence or so on.
  if(message === "!add"){
    if(user.check(userstate)){
      user.add(usersstate);
    };
  }

});

client.on("ban", function (channel, username, reason) {
    //client.say("channel", "User: " + user[display-name] + " - has been banned.")
});