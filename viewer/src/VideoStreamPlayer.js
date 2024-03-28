import React, { useEffect, useRef } from 'react';

const VideoStreamPlayer = ({ data, width, height }) => {
    const canvasRef = useRef(null);
    const imgRef = useRef(new Image());

    useEffect(
        () => {
            const canvas = canvasRef.current;
            const context = canvas.getContext('2d');
            // transfer the data to image
            const imageData = data;
            var binaryImageData = atob(imageData);
            var blob = new Blob([new Uint8Array(binaryImageData.length).map((_, i) => binaryImageData.charCodeAt(i))], { type: 'image/jpeg' });
            const imageBlob = new Blob([blob], { type: 'image/jpeg' });
            const imageObjectURL = URL.createObjectURL(imageBlob);

            let img = imgRef.current;
            img.src = imageObjectURL;
            img.onload = () => {
                canvas.width = width;
                canvas.height = height;
                context.clearRect(0, 0, canvas.width, canvas.height);
                context.drawImage(img, 0, 0, canvas.width, canvas.height);
                URL.revokeObjectURL(imageObjectURL);
            };
        }
        , [data, width, height]
    );

    return (
        <div>
            <canvas ref={canvasRef} width={'100%'} height={'100%'} className='video-stream-player'/>
        </div>
    );
}

export default VideoStreamPlayer;