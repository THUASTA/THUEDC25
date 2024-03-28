import './SettingsItem.css'

import React, { useState,useEffect } from 'react';

const SettingsItem = ({ title , getDataFunc}) => {
    const defaultConfig = JSON.parse(localStorage.getItem('config_'+title)) || {
        hue: { center: 10, range: 20 },
        saturation: { center: 200, range: 100 },
        value: { center: 200, range: 100 },
        minArea: 0,
        camera: '',
        port: '',
        showMask: false,
        baudRate: '115200',
        cameraContrast: undefined,
        cameraBrightness: undefined,
        cameraExposure: undefined,
        cameraAutoExposure: undefined,
        cameraSaturation: undefined
    };

    const [hue, setHue] = useState(defaultConfig.hue);
    const [saturation, setSaturation] = useState(defaultConfig.saturation);
    const [value, setValue] = useState(defaultConfig.value);
    const [minArea, setMinArea] = useState(defaultConfig.minArea);
    const [camera, setCamera] = useState(defaultConfig.camera);
    const [port, setPort] = useState(defaultConfig.port);
    const [showMask, setShowMask] = useState(defaultConfig.showMask);
    const [baudRate, setBaudRate] = useState(defaultConfig.baudRate);
    const [cameraContrast, setCameraContrast] = useState(defaultConfig.cameraContrast);
    const [cameraBrightness, setCameraBrightness] = useState(defaultConfig.cameraBrightness);
    const [cameraExposure, setCameraExposure] = useState(defaultConfig.cameraExposure);
    const [cameraAutoExposure, setCameraAutoExposure] = useState(defaultConfig.cameraAutoExposure);
    const [cameraSaturation, setCameraSaturation] = useState(defaultConfig.cameraSaturation);

    useEffect(() => {
        console.log('useEffect in settingsItem.js');
        const config = {
            hue,
            saturation,
            value,
            minArea,
            camera,
            port,
            showMask,
            baudRate,
            cameraContrast,
            cameraBrightness,
            cameraExposure,
            cameraAutoExposure,
            cameraSaturation
        };
        console.log(title,config)
        getDataFunc(config, title);
    }, [hue, saturation, value, minArea, camera, port, showMask, baudRate, cameraContrast, cameraBrightness, cameraExposure, cameraAutoExposure, cameraSaturation]);
    
    return (

        <div class='settings-container'>
            <span class='settings-title'>{title}</span>
            <div class='settings-items-container'>
                <div class='settings-item'>
                    <span class='settings-item-label'>Hue</span>
                    <div class='settings-item-value-container'>
                        <input type='number' class='color-value' placeholder='center'
                            value={hue.center}
                            onChange={(e) => setHue({ center: e.target.value, range: hue.range })}
                        />
                        <input type='number' class='color-value' placeholder='range'
                            value={hue.range}
                            onChange={(e) => setHue({ center: hue.center, range: e.target.value })}
                        />
                    </div>
                </div>
                <div class='settings-item'>
                    <span class='settings-item-label'>Saturation</span>
                    <div class='settings-item-value-container'>
                        <input type='number' class='color-value' placeholder='center'
                            value={saturation.center}
                            onChange={(e) => setSaturation({ center: e.target.value, range: saturation.range })}
                        />
                        <input type='number' class='color-value' placeholder='range'
                            value={saturation.range}
                            onChange={(e) => setSaturation({ center: saturation.center, range: e.target.value })}
                        />
                    </div>
                </div>
                <div class='settings-item'>
                    <span class='settings-item-label'>Value</span>
                    <div class='settings-item-value-container'>
                        <input type='number' class='color-value' placeholder='center'
                            value={value.center}
                            onChange={(e) => setValue({ center: e.target.value, range: value.range })}
                        />
                        <input type='number' class='color-value' placeholder='range'
                            value={value.range}
                            onChange={(e) => setValue({ center: value.center, range: e.target.value })}
                        />
                    </div>
                </div>
                <div class='settings-item'>
                    <span class='settings-item-label'>Min-Area</span>
                    <div class='settings-item-value-container'>
                        <input type='number' class='area' placeholder='min-area'
                            value={minArea}
                            onChange={(e) => setMinArea(e.target.value)}
                        />
                    </div>
                </div>
                <div class='settings-item'>
                    <span class='settings-item-label'>Camera</span>
                    <div class='settings-item-value-container'>
                        <select class='camera-select'
                            value={camera}
                            onChange={(e) => setCamera(e.target.value)}
                        >
                            <option>0</option>
                            <option>1</option>
                        </select>
                    </div>
                </div>
                <div class='settings-item'>
                    <span class='settings-item-label'>Port</span>
                    <div class='settings-item-value-container'>
                        <select class='port-select'
                            value={port}
                            onChange={(e) => setPort(e.target.value)}
                        >
                            <option>COM9</option>
                        </select>
                    </div>
                </div>
                <div class='settings-item'>
                    <span class='settings-item-label'>Show Mask</span>
                    <div class='settings-item-value-container'>
                        <input type='checkbox' class='show-mask'
                            value={showMask}
                            onChange={(e) => setShowMask(e.target.value)}
                        />
                    </div>
                </div>
                <div class='settings-item'>
                    <span class='settings-item-label'>BaudRate</span>
                    <div class='settings-item-value-container'>
                        <select class='baudRate'
                            value={baudRate}
                            onChange={(e) => setBaudRate(e.target.value)}
                        >
                            <option>2400</option>
                            <option>4800</option>
                            <option>9600</option>
                            <option>19200</option>
                            <option>38400</option>
                            <option>57600</option>
                            <option>115200</option>
                        </select>
                    </div>
                </div>
                <div class='settings-item'>
                <span class='settings-item-label'>Camera Paras</span>
                </div>

                <div class='settings-item'>
                    <span class='settings-item-label'>Brightness</span>
                    <div class='settings-item-value-container'>
                        <input type='number' class='camera-setting' placeholder='brightness'
                            value={cameraBrightness}
                            onChange={(e) => setCameraBrightness(e.target.value)}
                        />
                    </div>
                </div>
                <div class='settings-item'>
                    <span class='settings-item-label'>Contrast</span>
                    <div class='settings-item-value-container'>
                        <input type='number' class='camera-setting' placeholder='contrast'
                            value={cameraContrast}
                            onChange={(e) => setCameraContrast(e.target.value)}
                        />
                    </div>
                </div>
                <div class='settings-item'>
                    <span class='settings-item-label'>Saturation</span>
                    <div class='settings-item-value-container'>
                        <input type='number' class='camera-setting' placeholder='saturation'
                            value={cameraSaturation}
                            onChange={(e) => setCameraSaturation(e.target.value)}
                        />
                    </div>
                </div>
                <div class='settings-item'>
                    <span class='settings-item-label'>Exposure</span>
                    <div class='settings-item-value-container'>
                        <input type='number' class='camera-setting' placeholder='exposure'
                            value={cameraExposure}
                            onChange={(e) => setCameraExposure(e.target.value)}
                        />
                    </div>
                </div>
                <div class='settings-item'>
                    <span class='settings-item-label'>Auto Exposure</span>
                    <div class='settings-item-value-container'>
                        <input type='checkbox' class='camera-setting' placeholder='auto-exposure'
                            value={cameraAutoExposure}
                            onChange={(e) => setCameraAutoExposure(e.target.value)}
                        />
                    </div>
                </div>

            </div>
        </div>
    )
}

export default SettingsItem;