var net = require('net');
var user = require('./accounts/user.js');
var jsonrequest = require('./jsonrequest.js');

var socketserver = {};
module.exports = socketserver;

socketserver.clients = [];

var onConnection = function (socket) {
    console.log('client connected');

    socket.id = socket.remoteAddress + ":" + socket.remotePort;
    socketserver.clients.push(socket);

    socket.on('data', function(data){
    	console.log("received data: '" + data + "'")
		data = data.toString('utf8');

		try{
			var json = JSON.parse(data);
			var request = new jsonrequest(json["type"], json["data"], json["request"]);

			//TODO: Add a similar system that the C# SocketConnection has with a dictionary of responder objects
	    	switch(data){
				case "getallplayers":
					var players = user.getAll();
					socket.write(players);
					break;
				default:
					var response = new jsonrequest(request.type, {"default": "unrecognised command"}, false);
					socketserver.broadcast(response.getJSON());
					break;
			}
		}catch(err){
			//okay its not a valid json request, so we scream in the console!
			console.error("The folowing request is not a valid jsonrequest! please fix fast schnell: " + data);
		}
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

    //socket.write("Hello sir!");
    var request = new jsonrequest("test", {"sometest": 123}, false);
    socket.write(request.getJSON());
};

var server = net.createServer(onConnection);

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
};