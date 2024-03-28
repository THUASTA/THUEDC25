// testserver.js
// provide fake data for debugging

const WebSocket = require('ws');
const http = require('http');
const fs = require('fs');
const { TIMEOUT } = require('dns');

const server = http.createServer(
  (req, res) => {
    // handle http request here
  });

const wss = new WebSocket.Server({ server });

wss.on('connection', (ws) => {
  // read the picture (change the path to yours)
  fs.readFile('../public/assets/pic.jpg', (err, imageData) => {
    if (err) {
      console.error(err);
      return;
    }
    const width = 640;
    const height = 480;
    // data format: see https://thuasta.github.io/EDCHost/api/viewer/
    const data = {
      messageType: "COMPETITION_UPDATE",
      cameras: [
        {
          cameraId: 1,
          frameData: Buffer.from(imageData).toString('base64'),
          width: width,
          height: height
        },
        {
          cameraId: 2,
          frameData: Buffer.from(imageData).toString('base64'),
          width: width,
          height: height
        }
      ],
      players: [
      ]
    }

    ws.send(JSON.stringify(data));
  });
  ws.onmessage = (event) => {
    const data = JSON.parse(event.data);
    console.log(data);
  };
});

server.listen(8080, () => {
  console.log('WebSocket server is running on port 8080');
});
