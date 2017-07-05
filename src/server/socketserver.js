const net = require('net');
var socketserver = { };
module.exports = socketserver;

socketserver.sockets = []

// var socket;
// server.getsocket = function(){
//     return this.socket;
// };
socketserver.server = net.createServer(function (socket) {
    console.log('client connected');
    socketserver.sockets.push(socket);
    socket.on("end", function(){
        console.log("client disconnected");
    });
    socket.pipe(socket);
});
socketserver.server.on('error', (err) => {
    throw err;
});
socketserver.server.listen(8124, () => {
    console.log('server bound');
});

socketserver.write = function(data){
	for (var i = 0; i < socketserver.sockets.length; i++) {
		socketserver.sockets[i].write(data);
	}
}