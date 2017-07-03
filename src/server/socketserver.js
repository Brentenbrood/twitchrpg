const net = require('net');
var server = { };
module.exports = server;

// var socket;
// server.getsocket = function(){
//     return this.socket;
// };
server = net.createServer(function (socket) {
    console.log('client connected');
    server.socket = socket;
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