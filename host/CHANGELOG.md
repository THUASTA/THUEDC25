# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.4.0] - 2023-12-16

### Added

- Dynamic map reloading when resetting the game.

### Fixed

- Failed to close serial ports.

## [0.3.5] - 2023-12-15

### Fixed

- The x,y of the map is incorrect 

## [0.3.4] - 2023-12-15

### Fixed

- Do not enqueue player event if game is not running

## [0.3.3] - 2023-12-08

### Fixed

- Wrong position when a player is dead

## [0.3.2] - 2023-12-08

### Fixed

- Wrong position when a player is dead

## [0.3.1] - 2023-12-08

### Fixed

- Abnormal behavior when trading.
- Wrong ore accumulation mechanism.
- Uncaught exceptions.

## [0.3.0] - 2023-12-06

### Changed

- Adapt to the new API.

## [0.2.0] - 2023-11-23

### Changed

- Change mine positions and player spawn points to integer tuples.
- Performance improvements.

### Fixed

- Crash on errors in message publishing.
- Mistakes in README.
- Masks cannot be shown in the GUI.
- Failed to open a camera that has been opened before.
- Failed to open a serial port that has been opened before.
- Default configuration values that should not exist.
- Reversed camera calibration parameters.

## [0.1.0] - 2023-11-21

Initial release.

[unreleased]: https://github.com/THUASTA/EDCHost/compare/v0.4.0...HEAD
[0.4.0]: https://github.com/THUASTA/EDCHost/compare/v0.3.5...v0.4.0
[0.3.5]: https://github.com/THUASTA/EDCHost/compare/v0.3.4...v0.3.5
[0.3.4]: https://github.com/THUASTA/EDCHost/compare/v0.3.3...v0.3.4
[0.3.3]: https://github.com/THUASTA/EDCHost/compare/v0.3.2...v0.3.3
[0.3.2]: https://github.com/THUASTA/EDCHost/compare/v0.3.1...v0.3.2
[0.3.1]: https://github.com/THUASTA/EDCHost/compare/v0.3.0...v0.3.1
[0.3.0]: https://github.com/THUASTA/EDCHost/compare/v0.2.0...v0.3.0
[0.2.0]: https://github.com/THUASTA/EDCHost/compare/v0.1.0...v0.2.0
[0.1.0]: https://github.com/THUASTA/EDCHost/releases/tag/v0.1.0
