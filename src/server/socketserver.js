const net = require('net');
let server = {};

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

module.exports = server;