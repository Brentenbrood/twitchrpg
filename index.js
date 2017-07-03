//Initial requires.
var tmi = require('tmi.js');
var config = require('./config.js');
var user = require('./src/server/accounts/user.js');
var message_parser = require('./src/server/message_parser.js');
const net = require('net');
server = net.createServer(function (socket) {
    console.log('client connected');
    socket.on("end", function(){
       console.log("client disconnected");
    });
    socket.pipe(socket);
});
server.on('error', (err) => {
    throw err;
});
server.listen(8124, () => {
    console.log('server bound');
});

//This is creating our client connection with settings.
client = new tmi.client(config.options);

//This connects to the twitch.
client.connect();

message_parser.init(client);

//This function is executed as soon as the bot has connected to the channel.
client.on("connected", function(address, port){
  client.action("channel name or use the channels array if you want it on all channels.", "You have summoned me.");
});

//This function is executed everytime someone sends a message in the chat.
client.on("chat", function(channel, userstate, message){
  //Using this if statement you can check the contents of a message and create commands.
  //This checks the contents of the message to see if they match the given message, this means that the message HAS to be that it cannot just contain that message.
  //This allows you to set commands or even words without worrying if someone was to use the word in a sentence or so on.
});