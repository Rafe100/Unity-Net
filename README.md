# Net
client use Unity2017
server use Nodejs
use protobufjs to serialize

tools:
libprotoc 3.5.1

protoc command:
protoc --js_out=import_style=commonjs,binary:. CustomProtocol.proto