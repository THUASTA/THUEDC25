// server.js
// provide fake data for debugging
const WebSocket = require('ws');
const http = require('http');
const fs = require('fs');
const {TIMEOUT} = require('dns');

const server = http.createServer(
    (req, res) => {
        // handle http request here
    });

const wss = new WebSocket.Server({server});

wss.on('connection', (ws) => {
  // read the picture (change the path to yours)
  fs.readFile('pic.jpg', (err, imageData) => {
    if (err) {
      console.error(err);
      return;
    }
    const cameraId = 2;
    const width = 640;
    const height = 480;
    const imageDataObject = {
      cameraId: cameraId,
      frameData: Buffer.from(imageData).toString('base64'),
      width: width,
      height: height
    }  // send to client
                            ws.send(JSON.stringify(imageDataObject));
  });
});

server.listen(8080, () => {
  console.log('WebSocket server is running on port 8080');
});
