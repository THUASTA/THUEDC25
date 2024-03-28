# Serial Port Protocols

## Table of Contents

In a packet, the first 5 bytes are the header and the later bytes are the data.

| Byte     | Description                                          |
| -------- | ---------------------------------------------------- |
| 0        | Always 0x55                                          |
| 1        | Always 0xAA                                          |
| 2 to 3   | Short. The number of bytes of the data (N)           |
| 4        | Byte. The checksum of the data                       |
| 5 to N+4 | The data of the packet. N is the length of the data. |

* [Operations](#operations)
  * [PUB main](#pub-main-operation)
  * [SUB main](#sub-main-operation)

## Operations

### PUB `main` Operation

#### Message `<anonymous-message-1>`

##### Payload

| Name       | Type    | Description                                                            | Value                   | Constraints     | Notes                                     |
| ---------- | ------- | ---------------------------------------------------------------------- | ----------------------- | --------------- | ----------------------------------------- |
| (root)     | object  | -                                                                      | -                       | -               | **additional properties are NOT allowed** |
| actionType | integer | The action type. 0: ATTACK 1: PLACE_BLOCK 2: TRADE                     | allowed (`0`, `1`, `2`) | format (`int8`) | -                                         |
| param      | integer | The parameter. Chunk id for ATTACK and PLACE_BLOCK. Item id for TRADE. | -                       | format (`int8`) | -                                         |

The ID of the top-left chunk is 0 and the ID of the bottom-right chunk is 63. The chunk ID increases first from left to right and then from top to down. For example, chunk (0, 0) has ID 0, and chunk (1, 0) has ID 1.

The item IDs are defined as belows:

| Item ID | Item |
|---|---|
| 0 | AgilityBoost |
| 1 | HealthBoost |
| 2 | StrengthBoost |
| 3 | Wool |
| 4 | PotionOfHealing |

### SUB `main` Operation

#### Message `<anonymous-message-2>`

##### Payload

| Name                         | Type           | Description                                                | Value | Constraints      | Notes                                     |
| ---------------------------- | -------------- | ---------------------------------------------------------- | ----- | ---------------- | ----------------------------------------- |
| (root)                       | object         | -                                                          | -     | -                | **additional properties are NOT allowed** |
| gameStage                    | integer        | The game stage 0: READY 1: RUNNING 2: BATTLING 3: FINISHED | -     | format (`int8`)  | -                                         |
| elapsedTicks                 | integer        | The elapsed ticks.                                         | -     | format (`int32`) | -                                         |
| informationOfChunks               | array<integer> | -                                                          | -     | 64 items         | -                                         |
| informationOfChunks (single item) | integer        | The information of chunks.                                      | -     | format (`int8`)  | -                                         |
| hasBed                       | boolean        | Whether the player has bed.                                | -     | -                | -                                         |
| hasBedOpponent               | boolean        | Whether the opponent has bed.                              | -     | -                | -                                         |
| positionX                    | number         | The x coordinate.                                          | -     | format (`float`) | -                                         |
| positionY                    | number         | The y coordinate.                                          | -     | format (`float`) | -                                         |
| positionOpponentX            | number         | The x coordinate of the opponent.                          | -     | format (`float`) | -                                         |
| positionOpponentY            | number         | The y coordinate of the opponent.                          | -     | format (`float`) | -                                         |
| agility                      | integer        | The agility point.                                         | -     | format (`int8`)  | -                                         |
| health                       | integer        | The health point.                                          | -     | format (`int8`)  | -                                         |
| maxHealth                    | integer        | The max health point.                                      | -     | format (`int8`)  | -                                         |
| strength                     | integer        | The strength point.                                        | -     | format (`int8`)  | -                                         |
| emeraldCount                 | integer        | The emerald count.                                         | -     | format (`int8`)  | -                                         |
| woolCount                    | integer        | The wool count.                                            | -     | format (`int8`)  | - |

InfomationOfChunk is defined as below:

```c
informationOfChunk = heightOfChunk + (oreKindOfChunk << 4)
```

The ore kinds are defined as belows:

| ID | OreKind |
|---|---|
| 0 | IronIngot |
| 1 | GoldIngot |
| 2 | Diamond |
| 3 | None |
