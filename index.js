//Initial requires.
var tmi = require('tmi.js');
var user = require('./src/server/accounts/user.js');
var message_parser = require('./src/server/message_parser.js');
var socketserver = require('./src/server/socketserver.js');

config = require('./config.js');

// const server = require('http').createServer();
// const io = require('socket.io')(server, {
//     path: '/data',
//     serveClient: false,
//     // below are engine.IO options
//     pingInterval: 10000,
//     pingTimeout: 5000,
//     cookie: false
// });
// server.listen(8124);
// server.on('connection', function(socket) {
//     console.log('client connected');
//     socket.emit('message', 'Hello World!');
// });

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