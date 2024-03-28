# EDCHost

The official host program for Electronic Design Competition

## Install

Download the latest release from [here](https://github.com/THUASTA/EDCHost/releases) and unzip it to an empty folder.

## Usage

Run `EdcHost.exe` to launch the program.

See API references at [API documentation](https://thuasta.github.io/EDCHost/api).

### Configuration

When first running the app, a `config.json` file will be created under current work directory. You can edit the configurations inside it.

Here is an example:

```json
{
    "loggingLevel": "Information",
    "serverPort": 8080,
    "game": {
        "diamondMines": [
            {
                "Item1": 1,
                "Item2": 3
            },
            {
                "Item1": 4,
                "Item2": 4
            }
        ],
        "goldMines": [
            {
                "Item1": 2,
                "Item2": 1
            },
            {
                "Item1": 4,
                "Item2": 7
            }
        ],
        "ironMines": [
            {
                "Item1": 0,
                "Item2": 1
            },
            {
                "Item1": 7,
                "Item2": 6
            }
        ]
    }
}
```

## Contributing

If you have any suggestions or improvements, please submit [an issue](https://github.com/THUASTA/EDCHost/issues/new) or a pull request.

## License

[GPL-3.0-only](LICENSE) © THUASTA
