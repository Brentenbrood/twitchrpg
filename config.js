var config = {};

config.options = {
  options: {
  	clientId: null,
    debug: true
  },

  connection: {
  	//server: "", //Connect to this server (Overrides cluster and connect to this server instead)
    cluster: "aws",
  	port: 80,
    reconnect: true,
    maxReconnectAttempts: Infinity,
    maxReconnectInterval: 30000,
    reconnectDecay: 1.5,
    reconnectInterval: 1000,
    //secure: false, //Use secure connection (SSL / HTTPS) (Overrides port to 443)
    timeout: 9999
  },

  identity: {
    username: "FILL IN ",
    password: "oauth:m8kph7twbl290gl9s4h9it1uir4p0c"
  },

/* "logger: Object - Custom logger with the methods info, warn, and error"... What signature do the functions have?
  logger:{
    info: function(){},
    warn: function(){},
    error: function(){}
  },
*/

  channels: ["BRENTOLINNI"]
}