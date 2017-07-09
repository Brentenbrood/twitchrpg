var net = require('net');
var user = require('./accounts/user.js');
var socketserver = {};
module.exports = socketserver;

var clients = [];

var server = net.createServer(function (socket) {
    console.log('client connected');

    socket.id = socket.remoteAddress + ":" + socket.remotePort;
    clients.push(socket);

    socket.on('data', function(data){
    	console.log("received data: '" + data + "'");
		data = data.toString('utf8');
    	switch(data){
			case "getallplayers":
				var players = user.getAll();
				socketserver.broadcast(players);
				break;
			default:
				console.log(data);
				break;
		}

    });

    socket.on("end", function(){
    	var index = clients.indexOf(socket);
    	if(index == -1){
        	console.log("unknown client disconnected: " + socket.id);
    	}else{
        	console.log("client disconnected: " + socket.id);
        	clients.splice(index, 1);
    	}

    });

    socket.write('{"type":"connection_start"}');
});

server.on('error', (err) => {
    throw err;
});

server.listen(8124, () => {
    console.log('server bound');
});

socketserver.server = server;

socketserver.broadcast = function(msg){
	clients.forEach(function(client){
		client.write(msg);
	});

	console.log("broadcasted: '" + msg + "'");
};