# thuedc-25

Platform for the 25th Tsinghua University Electronic Design Competition

## Install

This platform does not require installation. Just download and extract the host from releases.

## Usage

1. Run the host program.

1. Visit <https://thuasta.github.io/thuedc-25/viewer>.

1. Connect to the host program.

### Configuration

When first running the host, a `config.json` file will be created under current work directory. You can edit the configurations inside it.

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

Ask questions by creating an issue.

PRs accepted.

## License

GPL-3.0-only © Student Association of Science and Technology, Department of Automation, Tsinghua University
