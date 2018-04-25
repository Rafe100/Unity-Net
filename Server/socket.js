var net = require('net');
var protobuf = require('protobufjs');
var customProto = require("./CustomProto/CustomProtocol_pb.js");
var loginReq = customProto.MsgClientTestLoginReq;


var server = net.createServer(
function(socket)
{
	console.log("a connection...." + socket.remoteAddress + ':' + socket.port);
	
	socket.setEncoding("base64");
	socket.on('data',function(data){
		//var i = 0;
		//console.log("total length:" + data.length);
		//var length = data.readInt16LE(i);
		//console.log('a data ..L..' + length);
		//i++;
		//i++;
		//var t = data.readInt16LE(2);
	    //console.log('a data ..L..' + length + "[t]" + t);
	    var t = 0;
		if(t === 9)
		{
			
			var message = heartBeat.create(); // or use .fromObject if conversion is necessary
            var encodeBuffer = heartBeat.encode(message).finish();
			var lengthBuffer = Buffer.alloc(2);
			lengthBuffer.writeInt16LE(encodeBuffer.length + 6);
			var typeBuffer = Buffer.alloc(2);
			typeBuffer.writeInt16LE(0);
			var externBuffer = Buffer.alloc(4);
			var sendBuffer = Buffer.concat([lengthBuffer,typeBuffer,externBuffer,encodeBuffer]);
			socket.write(sendBuffer);
		}
		else if(t === 1)
		{
			
			
			var length = data.readInt16LE();
			console.log("length" + length);
			var encodeBuffer = Buffer.alloc(length).fill(data, 8, length - 6,"binary");
			var ui8array = new Uint8Array(encodeBuffer);
			var ack = loginReq.deserializeBinary(encodeBuffer);
			console.log("ack" + ack.gate_ip);
			console.log("ack" + ack.gateIp);

			socket.write(sendBuffer);
		}
		var ack = loginReq.deserializeBinary(data);
		console.log("ack a" + ack.getAccount());
		console.log("ack p" + ack.getPasswd());
	});

	
	socket.on('error',function(data){
	
	console.log('a error ....');
	});
	
	socket.on('close',function(data){
	
	console.log('a close ....');
	});
	
}

);
server.listen(3001);


server.on('listening',function(){
  console.log("server listening:" + server.address().port);
});


server.on("error",function(exception){
  console.log("server error:" + exception);
});


var req = new loginReq();
req.setAccount("zzz");
req.setPasswd("123");
var b = req.serializeBinary();
var dreq = loginReq.deserializeBinary(b);
console.log("deserialize :" + dreq.getAccount());
console.log("deserialize :" + dreq.getPasswd());