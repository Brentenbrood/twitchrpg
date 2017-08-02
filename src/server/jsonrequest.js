var jsonrequest = function(type, data = {}, request = true){
	if (!(this instanceof jsonrequest)) return new jsonrequest();

	if(typeof type === 'string'){
		this.type = type;
		this.data = data;
        this.request = request;
	}
}

module.exports = jsonrequest;

jsonrequest.prototype.getJSON = function(){
	return JSON.stringify(this);
}