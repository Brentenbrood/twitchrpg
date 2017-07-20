var jsonrequest = function(type, data = {}, request = true){
	if (!(this instanceof jsonrequest)) return new jsonrequest();

	if(typeof type === 'string'){
		this.type = type;
		this.request = request;
		this.data = data;
	}
}

module.exports = jsonrequest;

jsonrequest.prototype.getJSON = function(){
	return JSON.stringify(this);
}