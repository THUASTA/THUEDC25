import './App.css'

import { Button, Popover } from 'antd';
import React, { useEffect, useState } from 'react';

import PlayerInfo from './PlayerInfo';
import VideoStreamPlayer from './VideoStreamPlayer';
import SettingsItem from './SettingsItem';
import GridCanvas from './GridCanvas';
var server = 'ws://localhost:8080'
var calibrated1 = false;
var calibrated2 = false;

var coord1 = [];
var coord2 = [];
var gameState = "STANDBY";
const STAGES = ["READY", "RUNNING", "BATTLING", "FINISHED", "ENDED"];
const App = () => {

  const [camera1, setCamera1] = useState(null);
  const [camera2, setCamera2] = useState(null);
  const [player1, setPlayer1] = useState(null);
  const [player2, setPlayer2] = useState(null);
  const [mines, setMines] = useState([]);
  const [chunks, setChunks] = useState([]);
  const [ipAddress, setIPAddress] = useState(localStorage.getItem("ipAddress") || "127.0.0.1:8080");
  const [isOpen, setIsOpen] = useState(true);
  const [stage, setStage] = useState(STAGES[0]);
  const [ticks, setTicks] = useState(0);


  var player1Camera = -1;
  var player2Camera = -1;
  const [currentConfigs, setCurrentConfigs] = useState({});
  //新建一个函数用于将设置的参数存入localstorage
  const saveSettingsToLocalStorage = (title) => {
    console.log(title,currentConfigs[title]);
    if(currentConfigs[title]!==undefined){
      localStorage.setItem('config_'+title, JSON.stringify(currentConfigs[title]));
    }
  };

  function nullOrDefault(inputValue, defaultValue) {
    if (inputValue === null || inputValue === undefined || isNaN(inputValue)) {
      return defaultValue;
    }
    else return inputValue;
  }
  function data1() {
    try {
      const inputs = document.getElementsByClassName("color-value");
      console.log(parseInt(inputs[0].value));
      console.log(nullOrDefault(parseInt(inputs[0].value), 0));
      const hueCenter1 = nullOrDefault(parseInt(inputs[0].value), 0);
      const hueRange1 = nullOrDefault(parseInt(inputs[1].value), 0);
      const saturationCenter1 = nullOrDefault(parseInt(inputs[2].value), 0);
      const saturationRange1 = nullOrDefault(parseInt(inputs[3].value), 0);
      const valueCenter1 = nullOrDefault(parseInt(inputs[4].value), 0);
      const valueRange1 = nullOrDefault(parseInt(inputs[5].value), 0);
      const hueCenter2 = nullOrDefault(parseInt(inputs[6].value), 0);
      const hueRange2 = nullOrDefault(parseInt(inputs[7].value), 0);
      const saturationCenter2 = nullOrDefault(parseInt(inputs[8].value), 0);
      const saturationRange2 = nullOrDefault(parseInt(inputs[9].value), 0);
      const valueCenter2 = nullOrDefault(parseInt(inputs[10].value), 0);
      const valueRange2 = nullOrDefault(parseInt(inputs[11].value), 0);
      const areas = document.getElementsByClassName("area");
      const area1 = nullOrDefault(parseInt(areas[0].value));
      const area2 = nullOrDefault(parseInt(areas[1].value));
      const cameras = document.getElementsByClassName("camera-select");
      const camera1 = cameras[0].value === "" ? null : Number(cameras[0].value);
      const camera2 = cameras[1].value === "" ? null : Number(cameras[1].value);
      const ports = document.getElementsByClassName("port-select");
      const port1 = ports[0].value;
      const port2 = ports[1].value;
      // console.log(port1);
      const masks = document.getElementsByClassName("show-mask");
      const mask1 = masks[0].checked;
      const mask2 = masks[1].checked;
      const baud = document.getElementsByClassName("baudRate");
      const baudrate1 = Number(baud[0].value);
      const baudrate2 = Number(baud[1].value);
      const cameraSettings = document.getElementsByClassName("camera-setting");
      const cameraBrightness1 = nullOrDefault(parseFloat(cameraSettings[0].value), 0);
      const cameraContrast1 = nullOrDefault(parseFloat(cameraSettings[1].value), 0);
      const cameraSaturation1 = nullOrDefault(parseFloat(cameraSettings[2].value), 0);
      const cameraExposure1 = nullOrDefault(parseFloat(cameraSettings[3].value), 0);
      const cameraAutoExplosure1 = nullOrDefault(parseFloat(cameraSettings[4].value), 0);
      const cameraBrightness2 = nullOrDefault(parseFloat(cameraSettings[5].value), 0);
      const cameraContrast2 = nullOrDefault(parseFloat(cameraSettings[6].value), 0);
      const cameraSaturation2 = nullOrDefault(parseFloat(cameraSettings[7].value), 0);
      const cameraExposure2 = nullOrDefault(parseFloat(cameraSettings[8].value), 0);
      const cameraAutoExplosure2 = nullOrDefault(parseFloat(cameraSettings[9].value), 0);
      const data1 = {
        messageType: "HOST_CONFIGURATION_FROM_CLIENT",
        token: "",
        configuration: {
          cameras: [
            camera1 === null ? undefined : {
              cameraId: camera1,
              properties: {
                brightness: cameraBrightness1,
                contrast: cameraContrast1,
                saturation: cameraSaturation1,
                exposure: cameraExposure1,
                autoExplosure: cameraAutoExplosure1
              },
              recognition: {
                hueCenter: hueCenter1,
                hueRange: hueRange1,
                saturationCenter: saturationCenter1,
                saturationRange: saturationRange1,
                valueCenter: valueCenter1,
                valueRange: valueRange1,
                minArea: area1,
                showMask: mask1
              }
            },
            camera2 === null ? undefined : {
              cameraId: camera2,
              properties: {
                brightness: cameraBrightness2,
                contrast: cameraContrast2,
                saturation: cameraSaturation2,
                exposure: cameraExposure2,
                autoExplosure: cameraAutoExplosure2
              },
              recognition: {
                hueCenter: hueCenter2,
                hueRange: hueRange2,
                saturationCenter: saturationCenter2,
                saturationRange: saturationRange2,
                valueCenter: valueCenter2,
                valueRange: valueRange2,
                minArea: area2,
                showMask: mask2
              }
            }
          ].filter(x => x !== undefined),
          players: [
            {
              playerId: 0,
              camera: camera1 === null ? undefined : camera1,
              serialPort: port1 === "" ? undefined : port1
            },
            {
              playerId: 1,
              camera: camera2 === null ? undefined : camera2,
              serialPort: port2 === "" ? undefined : port2
            }
          ],
          serialPorts: [
            port1 === "" ? undefined : {
              portName: port1,
              baudRate: baudrate1
            },
            port2 === "" ? undefined : {
              portName: port2,
              baudRate: baudrate2
            }
          ].filter(x => x !== undefined)
        }
      }
      player1Camera = camera1;
      player2Camera = camera2;
      data1.configuration.cameras = data1.configuration.cameras.filter(camera => camera !== undefined);
      data1.configuration.serialPorts = data1.configuration.serialPorts.filter(serialPort => serialPort !== undefined);

      console.log(data1);
      return data1;
    }
    catch {
      return null;
    }
  }
  function data2() {
    try {
      const inputs = document.getElementsByClassName("color-value");
      const hueCenter1 = nullOrDefault(parseInt(inputs[0].value), 0);
      const hueRange1 = nullOrDefault(parseInt(inputs[1].value), 0);
      const saturationCenter1 = nullOrDefault(parseInt(inputs[2].value), 0);
      const saturationRange1 = nullOrDefault(parseInt(inputs[3].value), 0);
      const valueCenter1 = nullOrDefault(parseInt(inputs[4].value), 0);
      const valueRange1 = nullOrDefault(parseInt(inputs[5].value), 0);
      const hueCenter2 = nullOrDefault(parseInt(inputs[6].value), 0);
      const hueRange2 = nullOrDefault(parseInt(inputs[7].value), 0);
      const saturationCenter2 = nullOrDefault(parseInt(inputs[8].value), 0);
      const saturationRange2 = nullOrDefault(parseInt(inputs[9].value), 0);
      const valueCenter2 = nullOrDefault(parseInt(inputs[10].value), 0);
      const valueRange2 = nullOrDefault(parseInt(inputs[11].value), 0);
      const areas = document.getElementsByClassName("area");
      const area1 = nullOrDefault(parseFloat(areas[0].value), 0);
      const area2 = nullOrDefault(parseFloat(areas[1].value), 0);
      const cameras = document.getElementsByClassName("camera-select");
      const camera1 = cameras[0].value === "" ? undefined : Number(cameras[0].value);
      const camera2 = cameras[1].value === "" ? undefined : Number(cameras[1].value);
      const ports = document.getElementsByClassName("port-select");
      const port1 = ports[0].value;
      const port2 = ports[1].value;
      const masks = document.getElementsByClassName("show-mask");
      const mask1 = masks[0].checked;
      const mask2 = masks[1].checked;
      const baud = document.getElementsByClassName("baudRate");
      const baudrate1 = Number(baud[0].value);
      const baudrate2 = Number(baud[1].value);
      const cameraSettings = document.getElementsByClassName("camera-setting");
      const cameraBrightness1 = nullOrDefault(parseFloat(cameraSettings[0].value), 0);
      const cameraContrast1 = nullOrDefault(parseFloat(cameraSettings[1].value), 0);
      const cameraSaturation1 = nullOrDefault(parseFloat(cameraSettings[2].value), 0);
      const cameraExposure1 = nullOrDefault(parseFloat(cameraSettings[3].value), 0);
      const cameraAutoExplosure1 = nullOrDefault(parseFloat(cameraSettings[4].value), 0);
      const cameraBrightness2 = nullOrDefault(parseFloat(cameraSettings[5].value), 0);
      const cameraContrast2 = nullOrDefault(parseFloat(cameraSettings[6].value), 0);
      const cameraSaturation2 = nullOrDefault(parseFloat(cameraSettings[7].value), 0);
      const cameraExposure2 = nullOrDefault(parseFloat(cameraSettings[8].value), 0);
      const cameraAutoExplosure2 = nullOrDefault(parseFloat(cameraSettings[9].value), 0);
      const corner1 = coord1;
      const corner2 = coord2;
      const data2 = {
        messageType: "HOST_CONFIGURATION_FROM_CLIENT",
        token: "",
        configuration: {
          cameras: [
            camera1 === null ? undefined : {
              cameraId: camera1,
              properties: {
                brightness: cameraBrightness1,
                contrast: cameraContrast1,
                saturation: cameraSaturation1,
                exposure: cameraExposure1,
                autoExplosure: cameraAutoExplosure1
              },
              recognition: {
                hueCenter: hueCenter1,
                hueRange: hueRange1,
                saturationCenter: saturationCenter1,
                saturationRange: saturationRange1,
                valueCenter: valueCenter1,
                valueRange: valueRange1,
                minArea: area1,
                showMask: mask1
              },
              calibration: {
                topLeft: {
                  x: corner1[0][0],
                  y: corner1[0][1]
                },
                topRight: {
                  x: corner1[1][0],
                  y: corner1[1][1]
                },
                bottomLeft: {
                  x: corner1[3][0],
                  y: corner1[3][1]
                },
                bottomRight: {
                  x: corner1[2][0],
                  y: corner1[2][1]
                }
              }
            },
            camera2 === null ? undefined : {
              cameraId: camera2,
              properties: {
                brightness: cameraBrightness2,
                contrast: cameraContrast2,
                saturation: cameraSaturation2,
                exposure: cameraExposure2,
                autoExplosure: cameraAutoExplosure2
              },
              recognition: {
                hueCenter: hueCenter2,
                hueRange: hueRange2,
                saturationCenter: saturationCenter2,
                saturationRange: saturationRange2,
                valueCenter: valueCenter2,
                valueRange: valueRange2,
                minArea: area2,
                showMask: mask2
              },
              calibration: {
                topLeft: {
                  x: corner2[0][0],
                  y: corner2[0][1]
                },
                topRight: {
                  x: corner2[1][0],
                  y: corner2[1][1]
                },
                bottomLeft: {
                  x: corner2[3][0],
                  y: corner2[3][1]
                },
                bottomRight: {
                  x: corner2[2][0],
                  y: corner2[2][1]
                }
              },
            }
          ].filter(x => x !== undefined),
          players: [
            {
              playerId: 0,
              camera: camera1 === null ? undefined : camera1,
              serialPort: port1 === "" ? undefined : port1
            },
            {
              playerId: 1,
              camera: camera2 === null ? undefined : camera2,
              serialPort: port2 === "" ? undefined : port2
            }
          ],
          serialPorts: [
            port1 === "" ? undefined : {
              portName: port1,
              baudRate: baudrate1
            },
            port2 === "" ? undefined : {
              portName: port2,
              baudRate: baudrate2
            }
          ].filter(x => x !== undefined)
        }
      }

      player1Camera = camera1;
      player2Camera = camera2;

      console.log('send data2');
      console.log(data2);
      return data2;
    }
    catch (e) {

      console.log(e);
      return null;
    }
  }

  useEffect(
    () => {
      if (!isOpen) {
        server = "ws://" + ipAddress
        const ws = new WebSocket(server);
        console.log('enter');
        ws.onopen = () => {
          console.log('WebSocket connected to ' + server);
        };

        ws.onmessage = (event) => {
          const data = JSON.parse(event.data);
          if (data.messageType === 'ERROR') {
            console.log(data);
          }
          // about the data format, see https://thuasta.github.io/EDCHost/api/viewer/
          if (data.messageType === 'COMPETITION_UPDATE') {
            setCamera1(data.cameras.find((value) => value.cameraId === player1Camera));
            setCamera2(data.cameras.find((value) => value.cameraId === player2Camera));
            setStage(STAGES[Number(data.info.stage)]);
            setTicks(data.info.elapsedTicks);
            if (data.info.stage === 0) {
              gameState = "STANDBY";
            }
            else if (data.info.stage === 4) {
              gameState = "ENDED";
            }

            let p1 = data.players.find((value) => value.playerId === 0);
            let p2 = data.players.find((value) => value.playerId === 1);

            setPlayer1(p1);
            setPlayer2(p2);
            // console.log("Generate" , position1, position2);
            setMines(data.mines);
            setChunks(data.chunks);
          }

          if (data.messageType === 'HOST_CONFIGURATION_FROM_SERVER') {
            console.log(data);
            const ports = document.getElementsByClassName("port-select");
            const cameras = document.getElementsByClassName("camera-select");
            const bauds = document.getElementsByClassName("baudRate");
            // 假设data.availableCameras是一个包含摄像头信息的数组
            const newCameras = data.availableCameras;
            const newPorts = data.availableSerialPorts;
            // 遍历每个 "port-select" 元素
            Array.from(ports).forEach((portElement, index) => {
              // 清空当前选择框的选项
              portElement.innerHTML = "";
              // 创建port
              const option = document.createElement("option");
              option.value = ""; // 假设摄像头对象有一个id属性
              option.text = ""; // 假设摄像头对象有一个name属性
              portElement.add(option);
              // 为选择框添加新的选项
              newPorts.forEach((port) => {
                const option = document.createElement("option");
                option.value = port; // 假设摄像头对象有一个id属性
                option.text = port; // 假设摄像头对象有一个name属性
                portElement.add(option);
              });

              // 如果需要，你还可以设置默认值
            });

            // 遍历每个 "port-select" 元素
            Array.from(cameras).forEach((cameraElement, index) => {
              // 清空当前选择框的选项
              cameraElement.innerHTML = "";
              // 创建空相机
              const option = document.createElement("option");
              option.value = ""; // 假设摄像头对象有一个id属性
              option.text = ""; // 假设摄像头对象有一个name属性
              cameraElement.add(option);
              // 为选择框添加新的选项
              newCameras.forEach((camera) => {
                const option = document.createElement("option");
                option.value = camera; // 假设摄像头对象有一个id属性
                option.text = camera; // 假设摄像头对象有一个name属性
                cameraElement.add(option);
              });
              // 如果需要，你还可以设置默认值
              // port.value = newPorts[0].id; // 假设选择第一个摄像头作为默认值
            });
            let hueCenter = [undefined, undefined], hueRange = [undefined, undefined], saturationCenter = [undefined, undefined], saturationRange = [undefined, undefined], valueCenter = [undefined, undefined], valueRange = [undefined, undefined], area = [undefined, undefined], showMask = [false, false];
            let cameraContrast = [undefined, undefined], cameraSaturation = [undefined, undefined], cameraAutoExplosure = [undefined, undefined], cameraExposure = [undefined, undefined], cameraBrightness = [undefined, undefined];
            for (var i = 0; i <= 1; i++) {
              const cameraIndex = data.configuration.players[i].camera;
              if (data.configuration.cameras[cameraIndex] !== undefined) {
                hueCenter[i] = data.configuration.cameras[cameraIndex].recognition.hueCenter;
                hueRange[i] = data.configuration.cameras[cameraIndex].recognition.hueRange;
                saturationCenter[i] = data.configuration.cameras[cameraIndex].recognition.saturationCenter;
                saturationRange[i] = data.configuration.cameras[cameraIndex].recognition.saturationRange;
                valueCenter[i] = data.configuration.cameras[cameraIndex].recognition.valueCenter;
                valueRange[i] = data.configuration.cameras[cameraIndex].recognition.valueRange;
                area[i] = data.configuration.cameras[cameraIndex].recognition.minArea;
                showMask[i] = data.configuration.cameras[cameraIndex].recognition.showMask;
                cameraBrightness[i] = data.configuration.cameras[cameraIndex].properties.brightness;
                cameraContrast[i] = data.configuration.cameras[cameraIndex].properties.contrast;
                cameraSaturation[i] = data.configuration.cameras[cameraIndex].properties.saturation;
                cameraExposure[i] = data.configuration.cameras[cameraIndex].properties.exposure;
                cameraAutoExplosure[i] = data.configuration.cameras[cameraIndex].properties.autoExplosure;

              }
              const serialPortIndex = data.configuration.players[i].serialPort;
              if (data.configuration.serialPorts[serialPortIndex] !== undefined) {
                ports[i].value = data.configuration.serialPorts[serialPortIndex].portName;
                bauds[i].value = data.configuration.serialPorts[serialPortIndex].baudRate;
              }
            }
            const inputs = document.getElementsByClassName("color-value");
            inputs[0].value = nullOrDefault(hueCenter[0], "");
            inputs[1].value = nullOrDefault(hueRange[0], "");
            inputs[2].value = nullOrDefault(saturationCenter[0], "");
            inputs[3].value = nullOrDefault(saturationRange[0], "");
            inputs[4].value = nullOrDefault(valueCenter[0], "");
            inputs[5].value = nullOrDefault(valueRange[0], "");
            inputs[6].value = nullOrDefault(hueCenter[1], "");
            inputs[7].value = nullOrDefault(hueRange[1], "");
            inputs[8].value = nullOrDefault(saturationCenter[1], "");
            inputs[9].value = nullOrDefault(saturationRange[1], "");
            inputs[10].value = nullOrDefault(valueCenter[1], "");
            inputs[11].value = nullOrDefault(valueRange[1], "");
            const Areas = document.getElementsByClassName("area");
            Areas[0].value = nullOrDefault(area[0], "");
            Areas[1].value = nullOrDefault(area[1], "");
            const masks = document.getElementsByClassName("show-mask");
            masks[0].checked = nullOrDefault(showMask[0], false);
            masks[1].checked = nullOrDefault(showMask[1], false);
            // 如果需要，你还可以设置默认值
            // port.value = newPorts[0].id; // 假设选择第一个摄像头作为默认值

            const cameraSettings = document.getElementsByClassName("camera-setting");
            cameraSettings[0].value = nullOrDefault(cameraBrightness[0], "");
            cameraSettings[1].value = nullOrDefault(cameraContrast[0], "");
            cameraSettings[2].value = nullOrDefault(cameraSaturation[0], "");
            cameraSettings[3].value = nullOrDefault(cameraExposure[0], "");
            cameraSettings[4].value = nullOrDefault(cameraAutoExplosure[0], "");
            cameraSettings[5].value = nullOrDefault(cameraBrightness[1], "");
            cameraSettings[6].value = nullOrDefault(cameraContrast[1], "");
            cameraSettings[7].value = nullOrDefault(cameraSaturation[1], "");
            cameraSettings[8].value = nullOrDefault(cameraExposure[1], "");
            cameraSettings[9].value = nullOrDefault(cameraAutoExplosure[1], "");
            // 更新player1Camera和player2Camera
          };
        };

        ws.onclose = () => {
          console.log('WebSocket disconnected');
          gameState = "STANDBY";
        };
        const start = document.getElementById("startbutton");
        const end = document.getElementById("endbutton");
        const setting = document.getElementById("setting");
        const canvas = document.getElementById("canvas");
        setting.onclick = async () => {
          ws.send(JSON.stringify({ messageType: "COMPETITION_CONTROL_COMMAND", command: "GET_HOST_CONFIGURATION", token: "" }));
          setTimeout(() => {
            const confirm = document.getElementById("confirmbutton");
            confirm.onclick = () => {
              saveSettingsToLocalStorage('Player 1');
              saveSettingsToLocalStorage('Player 2');
              console.log('saved to localStorage');
              if (calibrated1 && calibrated2) {
                ws.send(JSON.stringify(data2()));
                console.log('send data2');
              }
              else {
                ws.send(JSON.stringify(data1()));
                console.log('send data1');
              }
            }
          }, 1000)

        }

        start.onclick = () => {
          if (gameState === "STANDBY") {
            ws.send(JSON.stringify({ messageType: "COMPETITION_CONTROL_COMMAND", command: "START", token: "" }));
            gameState = "RUNNING";
          }
          else if (gameState === "ENDED") {
            ws.send(JSON.stringify({ messageType: "COMPETITION_CONTROL_COMMAND", command: "RESET", token: "" }));
            gameState = "STANDBY";
          }
        }
        end.onclick = () => {
          ws.send(JSON.stringify({ messageType: "COMPETITION_CONTROL_COMMAND", command: "END", token: "" }));
          gameState = "ENDED";
        }
        canvas.onclick = () => {
          if (calibrated1 && calibrated2) {
            ws.send(JSON.stringify(data2()));
            // message = false;
            console.log('send data2');
          }
        }
        return () => {
          ws.close(); // close the WebSocket connection when the component is unmounted
        };
      }
    },
    [isOpen]
  ,[currentConfigs]);
  const setCurrentConfig = (config,name) => {
    var tmp_config=currentConfigs
    tmp_config[name]=config
    setCurrentConfigs(tmp_config)
    console.log('Saving to currentConfig',name,config)
  }
  const [calibrating, setCalibrating] = useState(false);
  const finishCalibrate1 = (corner) => {
    setCalibrating(false);
    console.log('camera1 calibrated');
    for (let i = 0; i < 4; i++) {
      coord1[i] = corner[i];
    }
    calibrated1 = true;
    console.log(calibrated1)
    // send messages
  }

  const finishCalibrate2 = (corner) => {
    setCalibrating(false);
    console.log('camera2 calibrated');
    for (let i = 0; i < 4; i++) {
      coord2[i] = corner[i];
    }
    calibrated2 = true;
    // send messages
  }

  const handleConfirm = () => {
    console.log('保存的IP地址为:', ipAddress);
    setIsOpen(false);
    localStorage.setItem('ipAddress',ipAddress);
  };

  return (
    <div class='app-container'>
      <>
        {isOpen && (
          <div className="dialog-overlay">
            <div className="dialog">
              <h2>请输入您的IP地址</h2>
              <input
                type="text"
                value={ipAddress}
                onChange={(e) => setIPAddress(e.target.value)}
                placeholder="IP:Port"
              />
              <div className="button-container">
                <button onClick={handleConfirm}>确认</button>
              </div>
            </div>
          </div>
        )}
      </>
      <div class='game-info-container'>
        <div>Stage: {stage}</div>
        <div>Ticks: {ticks}</div>
      </div>
      <div class='top'>
        <div class='control-buttons-container'>
          <div class='control-buttons-row'>
            <Button type='primary' size='large' id="startbutton" block>Start</Button>
            <Button size="large" block
              onClick={() => { setCalibrating(true); calibrated1 = false; calibrated2 = false }}
            >Calibrate</Button>
          </div>
          <div class='control-buttons-row'>
            <Button size="large" id="endbutton" block>End</Button>
            <Popover
              id="popover"
              placement="bottomLeft"
              arrow={false}
              trigger="click"
              content={(
                <div>
                  <div class='flex'>
                    <SettingsItem title='Player 1' getDataFunc={setCurrentConfig}/>
                    <SettingsItem title='Player 2' getDataFunc={setCurrentConfig}/>
                  </div>
                  <Button id="confirmbutton">Confirm</Button>
                </div>
              )}
            >
              <Button size='large' block id="setting">Settings</Button>
            </Popover>
          </div>
        </div>
        <div class='player-list-container'>
          <PlayerInfo
            playerId={1}
            health={player1 ? player1.health : undefined}
            maxHealth={player1 ? player1.attributes.maxHealth : undefined}
            agility={player1 ? player1.attributes.agility : undefined}
            strength={player1 ? player1.attributes.strength : undefined}
            emerald={player1 ? player1.inventory.emerald : undefined}
            wool={player1 ? player1.inventory.wool : undefined}
          />
          <PlayerInfo
            playerId={2}
            health={player2 ? player2.health : undefined}
            maxHealth={player2 ? player2.attributes.maxHealth : undefined}
            agility={player2 ? player2.attributes.agility : undefined}
            strength={player2 ? player2.attributes.strength : undefined}
            emerald={player2 ? player2.inventory.emerald : undefined}
            wool={player2 ? player2.inventory.wool : undefined}
          />
        </div>
      </div>
      <div class='flex' id="canvas">
        <div class='video-canvas-container'>
          <div class='video-stream-player'>
            {camera1 ?
              <VideoStreamPlayer
                data={camera1.frameData}
                width={camera1.width}
                height={camera1.height}
              />
              : <></>}
          </div>
          <div class='grid-canvas-container'>
            <GridCanvas
              calibrating={calibrating}
              finishCalibrateCallback={finishCalibrate1}
              mines={mines}
              chunks={chunks}
              homePosition1={player1 ? player1.homePosition : null}
              playerPosition1={player1 ? player1.position : null}
              homePosition2={player2 ? player2.homePosition : null}
              playerPosition2={player2 ? player2.position : null}
            />
          </div>
        </div>
        <div class='video-canvas-container'>
          <div class='video-stream-player'>
            {camera2 ?
              <VideoStreamPlayer
                data={camera2.frameData}
                width={camera2.width}
                height={camera2.height}
              />
              : <></>}
          </div>
          <div class='grid-canvas-container'>
            <GridCanvas
              calibrating={calibrating}
              finishCalibrateCallback={finishCalibrate2}
              mines={mines}
              chunks={chunks}
              homePosition1={player1 ? player1.homePosition : null}
              playerPosition1={player1 ? player1.position : null}
              homePosition2={player2 ? player2.homePosition : null}
              playerPosition2={player2 ? player2.position : null}
            />
          </div>
        </div>
      </div>
    </div>
  )
}

export default App;