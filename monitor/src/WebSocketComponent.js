import React, {useEffect, useRef} from 'react';

const WebSocketComponent = () => {
  const canvasLeftRef = useRef(null);
  const canvasRightRef = useRef(null);
  useEffect(() => {
    const socket = new WebSocket('ws://localhost:8080'); // change the address to yours

    socket.onopen = () => {
      console.log('WebSocket connected');
    };

    socket.onmessage = (event) => {
      const eventData = JSON.parse(event.data);
      // assume event data = {cameraId:int, frameData:binary, height:int, width:int}
      // cameraId = 1 for left camera, 2 for right camera
      // frameData = jpg binary data
      if (eventData.cameraId === 1)
        renderImage(eventData.frameData, eventData.width, eventData.height, canvasLeftRef);//call renderImage function
      else if (eventData.cameraId === 2)
        renderImage(eventData.frameData, eventData.width, eventData.height, canvasRightRef);
    };

    socket.onclose = () => {
      console.log('WebSocket disconnected');
    };

    return () => {
      socket.close(); // close the WebSocket connection when the component is unmounted
    };
  }, []);
  const renderImage = (data, width, height, canvasRef) => {
    const canvas = canvasRef.current;
    const context = canvas.getContext('2d');
    // transfer the data to image
    const imageData = data;
    var binaryImageData = atob(imageData);
    var blob = new Blob([new Uint8Array(binaryImageData.length).map((_, i) => binaryImageData.charCodeAt(i))], { type: 'image/jpeg' });
    const imageBlob = new Blob([blob], { type: 'image/jpeg' });
    const imageObjectURL = URL.createObjectURL(imageBlob);

    const img = new Image();
    img.src = imageObjectURL;
    img.onload = () => {
      console.log('image loaded');
      canvas.width = width;
      canvas.height = height;
      context.clearRect(0, 0, canvas.width, canvas.height);
      context.drawImage(img, 0, 0, canvas.width, canvas.height);
      URL.revokeObjectURL(imageObjectURL);
    };
  };
  return (
    <div>
      WebSocket图像
      <div><canvas ref={canvasLeftRef} width={'100%'} height={'100%'} /></div>
      <div><canvas ref={canvasRightRef} width={'100%'} height={'100%'} /></div>
    </div>
  );
};

export default WebSocketComponent;
