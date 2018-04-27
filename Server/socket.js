var net = require('net');
var protobuf = require('protobufjs');
var customProto = require("./CustomProto/CustomProtocol_pb.js");
var loginReq = customProto.MsgClientTestLoginReq;
var loginRsp = customProto.MsgClientLoginRsp;

var server = net.createServer(
function(socket)
{
	console.log("a connection...." + socket.remoteAddress + ':' + socket.port);
	
	//socket.setEncoding("base64");
	socket.on('data',function(data){
		var dataBuffer = Buffer.from(data);
		var i = 0;
		console.log("data type:" + typeof(data));
		console.log("total data length:" + data.length);
		console.log("total dataBuffer length:" + dataBuffer.length);
		//message length 2+4+message.length
		var length = data.readInt16LE(i);
		i++;
		i++;
		//message type 
		var t = dataBuffer.readUInt16LE(i);
	    console.log('a data [Length]' + length + "[type]" + t);
		if(t === 1)
		{
			//use protobufjs to deserialize
			var encodeBuffer = data.slice(8,length - 6 + 8);
			console.log("deserialize length :" + encodeBuffer.length + typeof(encodeBuffer));
			var u8array = new Uint8Array(encodeBuffer);
			var ack = loginReq.deserializeBinary(u8array);
			console.log("ack account :" + ack.getAccount());
			console.log("ack passwd :" + ack.getPasswd());
			
			//use protobufjs to serialize and send message
			var rsp = new loginRsp();
			rsp.setUserId(936523016);
			rsp.setToken("tttttt");
			rsp.setGateIp("127.0.0.1");
			rsp.setGatePort(3002);
			var b = rsp.serializeBinary();
			var headBuffer = Buffer.alloc(8);
			headBuffer.writeInt16LE(b.length + 6);
			headBuffer.writeUInt16LE(11,2);
			headBuffer.writeInt32LE(0,4);
			sendBuffer = Buffer.concat([headBuffer,b],headBuffer.length + b.length);
			//send to client
			socket.write(sendBuffer);
			
		}
		
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
req.setAccount("myAccount");
req.setPasswd("123");
var b = req.serializeBinary();
var dreq = loginReq.deserializeBinary(b);
console.log("deserialize :" + dreq.getAccount());
console.log("deserialize :" + dreq.getPasswd());