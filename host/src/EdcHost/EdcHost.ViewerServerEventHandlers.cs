using System.Diagnostics;
using Emgu.CV.CvEnum;

namespace EdcHost;

partial class EdcHost : IEdcHost
{
    void HandleAfterMessageReceiveEvent(object? sender, ViewerServers.AfterMessageReceiveEventArgs e)
    {
        switch (e.Message)
        {
            case ViewerServers.CompetitionControlCommandMessage message:
                switch (message.Command)
                {
                    case "START":
                        HandleStartGame();
                        break;

                    case "END":
                        HandleEndGame();
                        break;

                    case "RESET":
                        HandleResetGame();
                        break;

                    case "GET_HOST_CONFIGURATION":
                        HandleGetHostConfiguration();
                        break;
                    default:
                        _logger.Error($"Invalid command: {message.Command}.");
                        break;
                }
                break;

            case ViewerServers.HostConfigurationFromClientMessage message:
                HandleUpdateConfiguration(message);
                break;

            default:
                _logger.Error($"Invalid message type: {e.Message.MessageType}.");
                break;
        }
    }

    void HandleStartGame()
    {
        try
        {
            _gameRunner.Start();
        }
        catch (Exception e)
        {
            _logger.Error($"Failed to start game: {e}");
            _viewerServer.Publish(new ViewerServers.ErrorMessage()
            {
                Message = e.Message
            });
        }
    }

    void HandleEndGame()
    {
        try
        {
            _gameRunner.End();
        }
        catch (Exception e)
        {
            _logger.Error($"Failed to end game: {e}");
            _viewerServer.Publish(new ViewerServers.ErrorMessage()
            {
                Message = e.Message
            });
        }
    }

    void HandleResetGame()
    {
        try
        {
            _logger.Information("Reloading config...");
            Config newConfig = ReLoadConfig();

            _logger.Information("Resetting Game...");
            if (_gameRunner.IsRunning)
            {
                _logger.Information("Game is running, Stopping.");
                _gameRunner.End();
            }

            _game = Games.IGame.Create(
                diamondMines: newConfig.Game.DiamondMines,
                goldMines: newConfig.Game.GoldMines,
                ironMines: newConfig.Game.IronMines
            );
            _gameRunner = Games.IGameRunner.Create(_game);

            _game.AfterJudgementEvent += HandleAfterJudgementEvent;

            for (int i = 0; i < _game.Players.Count; i++)
            {
                _game.Players[i].OnAttack += HandlePlayerAttackEvent;
                _game.Players[i].OnPlace += HandlePlayerPlaceEvent;
                _game.Players[i].OnDig += HandlePlayerDigEvent;
            }
            _logger.Information("Done.");
        }
        catch (Exception e)
        {
            _logger.Error($"Failed to reset game: {e}");
            _viewerServer.Publish(new ViewerServers.ErrorMessage()
            {
                Message = e.Message
            });
        }
    }

    void HandleGetHostConfiguration()
    {
        try
        {
            ViewerServers.HostConfigurationFromServerMessage configMessage = new()
            {
                AvailableCameras = _cameraServer.AvailableCameraIndexes,
                AvailableSerialPorts = _slaveServer.AvailablePortNames,
                Configuration = new()
                {
                    Cameras = _cameraServer.OpenCameraIndexes.Select((cameraIndex) =>
                        {
                            CameraServers.ICamera? cameraOrNull = _cameraServer.GetCamera(cameraIndex);
                            Debug.Assert(cameraOrNull != null);

                            CameraServers.ICamera camera = cameraOrNull!;

                            return new ViewerServers.HostConfiguration.CameraType()
                            {
                                CameraId = cameraIndex,

                                Calibration = camera.Locator.Options.Calibrate == false ? null : new()
                                {
                                    TopLeft = new ViewerServers.HostConfiguration.CameraType.CalibrationType.Point()
                                    {
                                        X = camera.Locator.Options.TopLeftX,
                                        Y = camera.Locator.Options.TopLeftY
                                    },
                                    TopRight = new ViewerServers.HostConfiguration.CameraType.CalibrationType.Point()
                                    {
                                        X = camera.Locator.Options.TopRightX,
                                        Y = camera.Locator.Options.TopRightY
                                    },
                                    BottomLeft = new ViewerServers.HostConfiguration.CameraType.CalibrationType.Point()
                                    {
                                        X = camera.Locator.Options.BottomLeftX,
                                        Y = camera.Locator.Options.BottomLeftY
                                    },
                                    BottomRight = new ViewerServers.HostConfiguration.CameraType.CalibrationType.Point()
                                    {
                                        X = camera.Locator.Options.BottomRightX,
                                        Y = camera.Locator.Options.BottomRightY
                                    }
                                },

                                Properties = new ViewerServers.HostConfiguration.CameraType.PropertiesType()
                                {
                                    FrameWidth = camera.GetProperty(CapProp.FrameWidth),
                                    FrameHeight = camera.GetProperty(CapProp.FrameHeight),
                                    Fps = camera.GetProperty(CapProp.Fps),
                                    Brightness = camera.GetProperty(CapProp.Brightness),
                                    Contrast = camera.GetProperty(CapProp.Contrast),
                                    Saturation = camera.GetProperty(CapProp.Saturation),
                                    Hue = camera.GetProperty(CapProp.Hue),
                                    Gain = camera.GetProperty(CapProp.Gain),
                                    Exposure = camera.GetProperty(CapProp.Exposure),
                                    Monochrome = camera.GetProperty(CapProp.Monochrome),
                                    Sharpness = camera.GetProperty(CapProp.Sharpness),
                                    AutoExposure = camera.GetProperty(CapProp.AutoExposure),
                                    Gamma = camera.GetProperty(CapProp.Gamma),
                                    Temperature = camera.GetProperty(CapProp.Temperature),
                                    WhiteBalanceRedV = camera.GetProperty(CapProp.WhiteBalanceRedV),
                                    Zoom = camera.GetProperty(CapProp.Zoom),
                                    Focus = camera.GetProperty(CapProp.Focus),
                                    IsoSpeed = camera.GetProperty(CapProp.IsoSpeed),
                                    Iris = camera.GetProperty(CapProp.Iris),
                                    Autofocus = camera.GetProperty(CapProp.Autofocus),
                                    AutoWb = camera.GetProperty(CapProp.AutoWb),
                                    WbTemperature = camera.GetProperty(CapProp.WbTemperature),
                                },

                                Recognition = new ViewerServers.HostConfiguration.CameraType.RecognitionType()
                                {
                                    HueCenter = camera.Locator.Options.HueCenter,
                                    HueRange = camera.Locator.Options.HueRange,
                                    SaturationCenter = camera.Locator.Options.SaturationCenter,
                                    SaturationRange = camera.Locator.Options.SaturationRange,
                                    ValueCenter = camera.Locator.Options.ValueCenter,
                                    ValueRange = camera.Locator.Options.ValueRange,
                                    MinArea = camera.Locator.Options.MinArea,
                                    ShowMask = camera.Locator.Options.ShowMask
                                },
                            };
                        }).ToList(),

                    Players = _playerHardwareInfo.Select((kv) =>
                        {
                            int playerIndex = kv.Key;
                            PlayerHardwareInfo playerHardwareInfo = kv.Value;

                            return new ViewerServers.HostConfiguration.PlayerType()
                            {
                                PlayerId = playerIndex,
                                Camera = playerHardwareInfo.CameraIndex,
                                SerialPort = playerHardwareInfo.PortName
                            };
                        }).ToList(),

                    SerialPorts = _slaveServer.OpenPortNames.Select((portName) =>
                        {
                            SlaveServers.ISlaveServer.PortInfo? portInfoOrNull = _slaveServer.GetPortInfo(portName);
                            Debug.Assert(portInfoOrNull != null);
                            SlaveServers.ISlaveServer.PortInfo portInfo = portInfoOrNull!;

                            return new ViewerServers.HostConfiguration.SerialPortType()
                            {
                                PortName = portName,
                                BaudRate = portInfo.BaudRate
                            };
                        }).ToList(),
                },
            };

            _viewerServer.Publish(configMessage);
        }
        catch (Exception e)
        {
            _logger.Error($"Failed to get host configuration: {e}");
            _viewerServer.Publish(new ViewerServers.ErrorMessage()
            {
                Message = e.Message
            });
        }
    }

    void HandleUpdateConfiguration(ViewerServers.HostConfigurationFromClientMessage message)
    {
        try
        {
            foreach (ViewerServers.HostConfiguration.CameraType camera in message.Configuration.Cameras)
            {
                if (!_cameraServer.OpenCameraIndexes.Contains(camera.CameraId))
                {
                    _logger.Information($"Opening camera {camera.CameraId}...");
                    _cameraServer.OpenCamera(camera.CameraId, new CameraServers.Locator());
                }

                CameraServers.ICamera? cameraOrNull = _cameraServer.GetCamera(camera.CameraId);
                Debug.Assert(cameraOrNull != null);
                CameraServers.ICamera cameraInstance = cameraOrNull!;

                CameraServers.RecognitionOptions recognitionOptions = cameraInstance.Locator.Options;

                if (camera.Recognition is not null)
                {
                    recognitionOptions.HueCenter = camera.Recognition.HueCenter;
                    recognitionOptions.HueRange = camera.Recognition.HueRange;
                    recognitionOptions.SaturationCenter = camera.Recognition.SaturationCenter;
                    recognitionOptions.SaturationRange = camera.Recognition.SaturationRange;
                    recognitionOptions.ValueCenter = camera.Recognition.ValueCenter;
                    recognitionOptions.ValueRange = camera.Recognition.ValueRange;
                    recognitionOptions.MinArea = camera.Recognition.MinArea;
                    recognitionOptions.ShowMask = camera.Recognition.ShowMask;
                }

                if (camera.Calibration is not null)
                {
                    recognitionOptions.Calibrate = true;

                    recognitionOptions.TopLeftX = camera.Calibration.TopLeft.X;
                    recognitionOptions.TopLeftY = camera.Calibration.TopLeft.Y;
                    recognitionOptions.TopRightX = camera.Calibration.TopRight.X;
                    recognitionOptions.TopRightY = camera.Calibration.TopRight.Y;
                    recognitionOptions.BottomLeftX = camera.Calibration.BottomLeft.X;
                    recognitionOptions.BottomLeftY = camera.Calibration.BottomLeft.Y;
                    recognitionOptions.BottomRightX = camera.Calibration.BottomRight.X;
                    recognitionOptions.BottomRightY = camera.Calibration.BottomRight.Y;
                }

                if (camera.Properties is not null)
                {
                    if (camera.Properties.FrameWidth is not null)
                    {
                        cameraInstance.SetProperty(CapProp.FrameWidth, camera.Properties.FrameWidth.Value);
                    }
                    if (camera.Properties.FrameHeight is not null)
                    {
                        cameraInstance.SetProperty(CapProp.FrameHeight, camera.Properties.FrameHeight.Value);
                    }
                    if (camera.Properties.Fps is not null)
                    {
                        cameraInstance.SetProperty(CapProp.Fps, camera.Properties.Fps.Value);
                    }
                    if (camera.Properties.Brightness is not null)
                    {
                        cameraInstance.SetProperty(CapProp.Brightness, camera.Properties.Brightness.Value);
                    }
                    if (camera.Properties.Contrast is not null)
                    {
                        cameraInstance.SetProperty(CapProp.Contrast, camera.Properties.Contrast.Value);
                    }
                    if (camera.Properties.Saturation is not null)
                    {
                        cameraInstance.SetProperty(CapProp.Saturation, camera.Properties.Saturation.Value);
                    }
                    if (camera.Properties.Hue is not null)
                    {
                        cameraInstance.SetProperty(CapProp.Hue, camera.Properties.Hue.Value);
                    }
                    if (camera.Properties.Gain is not null)
                    {
                        cameraInstance.SetProperty(CapProp.Gain, camera.Properties.Gain.Value);
                    }
                    if (camera.Properties.Exposure is not null)
                    {
                        cameraInstance.SetProperty(CapProp.Exposure, camera.Properties.Exposure.Value);
                    }
                    if (camera.Properties.Monochrome is not null)
                    {
                        cameraInstance.SetProperty(CapProp.Monochrome, camera.Properties.Monochrome.Value);
                    }
                    if (camera.Properties.Sharpness is not null)
                    {
                        cameraInstance.SetProperty(CapProp.Sharpness, camera.Properties.Sharpness.Value);
                    }
                    if (camera.Properties.AutoExposure is not null)
                    {
                        cameraInstance.SetProperty(CapProp.AutoExposure, camera.Properties.AutoExposure.Value);
                    }
                    if (camera.Properties.Gamma is not null)
                    {
                        cameraInstance.SetProperty(CapProp.Gamma, camera.Properties.Gamma.Value);
                    }
                    if (camera.Properties.Temperature is not null)
                    {
                        cameraInstance.SetProperty(CapProp.Temperature, camera.Properties.Temperature.Value);
                    }
                    if (camera.Properties.WhiteBalanceRedV is not null)
                    {
                        cameraInstance.SetProperty(CapProp.WhiteBalanceRedV, camera.Properties.WhiteBalanceRedV.Value);
                    }
                    if (camera.Properties.Zoom is not null)
                    {
                        cameraInstance.SetProperty(CapProp.Zoom, camera.Properties.Zoom.Value);
                    }
                    if (camera.Properties.Focus is not null)
                    {
                        cameraInstance.SetProperty(CapProp.Focus, camera.Properties.Focus.Value);
                    }
                    if (camera.Properties.IsoSpeed is not null)
                    {
                        cameraInstance.SetProperty(CapProp.IsoSpeed, camera.Properties.IsoSpeed.Value);
                    }
                    if (camera.Properties.Iris is not null)
                    {
                        cameraInstance.SetProperty(CapProp.Iris, camera.Properties.Iris.Value);
                    }
                    if (camera.Properties.Autofocus is not null)
                    {
                        cameraInstance.SetProperty(CapProp.Autofocus, camera.Properties.Autofocus.Value);
                    }
                    if (camera.Properties.AutoWb is not null)
                    {
                        cameraInstance.SetProperty(CapProp.AutoWb, camera.Properties.AutoWb.Value);
                    }
                    if (camera.Properties.WbTemperature is not null)
                    {
                        cameraInstance.SetProperty(CapProp.WbTemperature, camera.Properties.WbTemperature.Value);
                    }
                }
            }

            foreach (KeyValuePair<int, PlayerHardwareInfo> kvp in _playerHardwareInfo)
            {
                _playerHardwareInfo.AddOrUpdate(
                    kvp.Key,
                    new PlayerHardwareInfo()
                    {
                        CameraIndex = kvp.Value.CameraIndex,
                        PortName = null
                    },
                    (_, _) => new PlayerHardwareInfo()
                    {
                        CameraIndex = kvp.Value.CameraIndex,
                        PortName = null
                    }
                );
            }

            foreach (string port in _slaveServer.OpenPortNames)
            {
                try
                {
                    _slaveServer.ClosePort(port);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to close port {port}: {ex}");
                }
            }

            foreach (ViewerServers.HostConfiguration.SerialPortType serialPort in message.Configuration.SerialPorts)
            {
                /// <remark>
                /// When the hardware disconnets, serial port is closed but not removed from OpenSerialPorts.
                /// Therefore, we are not going to check whether a serial port is in OpenSerialPorts.
                /// Instead, we will try to open a serial port which throws exception if failed.
                /// </remark>
                _slaveServer.OpenPort(serialPort.PortName, serialPort.BaudRate);
            }

            foreach (ViewerServers.HostConfiguration.PlayerType player in message.Configuration.Players)
            {
                // Do not need to check if a player exists because we do not care.

                PlayerHardwareInfo playerHardwareInfo = new()
                {
                    CameraIndex = player.Camera,
                    PortName = player.SerialPort
                };

                _logger.Information($"Setting playerHardwareInfo of player {player.PlayerId}:");
                _logger.Information($"Camera: {playerHardwareInfo.CameraIndex}");
                _logger.Information($"Serial port: {playerHardwareInfo.PortName}");

                _playerHardwareInfo.AddOrUpdate(player.PlayerId, playerHardwareInfo, (_, _) => playerHardwareInfo);
            }
        }
        catch (Exception e)
        {
            _logger.Error("Error updating configuration: {0}", e.Message);
            _viewerServer.Publish(new ViewerServers.ErrorMessage());
        }
    }

}
