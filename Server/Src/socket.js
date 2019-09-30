var customProto = require('./CustomProto/netProto_pb.js');
var net = require('net');
var protobuf = require('protobufjs');
var dgram = require('dgram');
var udpSocket = dgram.createSocket('udp4');
console.log("program start");

var server = net.createServer(function (socket) {

    console.log(' server accept a new connection ');
    var rsp = new customProto.PlayerConnectRsp();
    rsp.setPlayid(1001);
    rsp.setUdpport(3003);
    var b = rsp.serializeBinary();
    var headBuffer = Buffer.alloc(8);
    headBuffer.writeInt16LE(b.length + 6);
    var rspId = 2;
    headBuffer.writeUInt16LE(rspId, 2);
    headBuffer.writeInt32LE(0, 4);
    var sendBuffer = Buffer.concat([headBuffer, b], headBuffer.length + b.length);
    console.log('rsp length:' + b.length);
    //send to client
    socket.write(sendBuffer);
    console.log('tcp send connection rsp');

    socket.on('connection', function (data) {
        console.log('connection event');
       
    });

    socket.on('data', function (data) {
        var dataBuffer = Buffer.from(data);
        console.log("a data coming ");
        console.log("total data length:" + data.length);
        var i = 0;
        var length = data.readInt16LE(i);
        i++;
        i++;
        //message type 
        var t = dataBuffer.readUInt16LE(i);
        console.log('a data [Length]' + length + "[type]" + t);

    });

    socket.on('error', function (data) {
        console.log('a error ....');
    });

    socket.on('close', function (data) {
        console.log('a close ....');
    });

});

server.listen(3001);
/* udp */
udpSocket.on('connect', function (data) {
    console.log('udpSocket a new connect ');
});

udpSocket.on('close', () => {
    console.log('udpSocketÒÑ¹Ø±Õ');
});

udpSocket.on('error', (err) => {
    console.log('udpSocket' + err);
});

udpSocket.on('listening', () => {
    console.log('udpSocket  listening...');
});

udpSocket.on('message', (data, rinfo) => {
    console.log(`receive message from ${rinfo.address}:${rinfo.port}`);
    var dataBuffer = Buffer.from(data);
    console.log("a data coming ");
    console.log("total data length:" + data.length);
    var i = 0;
    var length = data.readInt16LE(i);
    i++;
    i++;
    //message type 
    var t = dataBuffer.readUInt16LE(i);
    console.log('a data [Length]' + length + "[type]" + t);
    if (t == 2) {
        var encodeBuffer = data.slice(8, length - 6 + 8);
        console.log("deserialize length :" + encodeBuffer.length + typeof (encodeBuffer));
        var u8array = new Uint8Array(encodeBuffer);
        var ack = customProto.PlayerConnectRsp.deserializeBinary(u8array);
        console.log("the Playid :" + ack.getPlayid());
        console.log("udp port :" + ack.getUdpport());
    }
});

udpSocket.bind('3003');