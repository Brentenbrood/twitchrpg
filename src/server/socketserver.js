var net = require('net');
var socketserver = {};
module.exports = socketserver;

socketserver.clients = [];

var server = net.createServer(function (socket) {
    console.log('client connected');

    socket.id = socket.remoteAddress + ":" + socket.remotePort;
    socketserver.clients.push(socket);

    socket.on('data', function(data){
    	console.log("received data: '" + data + "'");
    });

    socket.on("end", function(){
    	var index = socketserver.clients.indexOf(socket);
    	if(index == -1){
        	console.log("unknown client disconnected: " + socket.id);
    	}else{
        	console.log("client disconnected: " + socket.id);
        	socketserver.clients.splice(index, 1);
    	}

    });

    socket.write("Hello sir!");

    socket.pipe(socket);
});

server.on('error', (err) => {
    throw err;
});

server.listen(8124, () => {
    console.log('server bound');
});

socketserver.server = server;

socketserver.broadcast = function(msg){
	socketserver.clients.forEach(function(client){
		client.write(msg);
	});

	console.log("broadcasted: '" + msg + "'");
}