using Newtonsoft.Json;
using System.Collections.Generic;

namespace EDCViewer.Messages
{
    public record CompetitionUpdate : Message
    {
        [JsonConverter(typeof(CommandEnumConverter))]
        [JsonProperty("messageType")]
        public override IMessage.MessageType Type { get; } = IMessage.MessageType.CompetitionUpdate;

        [JsonProperty("cameras")]
        public List<Camera> cameras { get; init; } = new();

        [JsonProperty("chunks")]
        public List<Chunk> chunks { get; init; } = new();

        [JsonProperty("events")]
        public List<Event> events { get; init; } = new();

        [JsonProperty("info")]
        public Info info { get; init; } = new();

        [JsonProperty("mines")]
        public List<Mine> mines { get; init; } = new();

        [JsonProperty("players")]
        public List<Player> players { get; init; } = new();

        public record Camera
        {
            [JsonProperty("cameraId")]
            public int cameraId { get; init; }

            [JsonProperty("frameData")]
            public string frameData { get; init; }

            [JsonProperty("height")]
            public int height { get; init; }

            [JsonProperty("width")]
            public int width { get; init; }

        }

        public record Chunk
        {
            [JsonProperty("chunkId")]
            public int chunkId { get; init; }

            [JsonProperty("height")]
            public int height { get; init; }

            [JsonProperty("position")]
            public Position? position { get; init; }

            public record Position
            {
                [JsonProperty("x")]
                public int x { get; init; }

                [JsonProperty("y")]
                public int y { get; init; }

            }
        }

        public record Event
        {
            [JsonProperty("PlayerAttackEvent")]
            public PlayerAttackEvent? playerAttackEvent { get; set; }

            [JsonProperty("PlayerDigEvent")]
            public PlayerDigEvent? playerDigEvent { get; set; }

            [JsonProperty("PlayerPickUpEvent")]
            public PlayerPickUpEvent? playerPickUpEvent { get; set; }

            [JsonProperty("PlayerPlaceBlockEvent")]
            public PlayerPlaceBlockEvent? playerPlaceBlockEvent { get; set; }

            [JsonProperty("PlayerTryAttackEvent")]
            public PlayerTryAttackEvent? playerTryAttackEvent { get; set; }

            [JsonProperty("PlayerTryUseEvent")]
            public PlayerTryUseEvent? playerTryUseEvent { get; set; }

            public record PlayerAttackEvent
            {
                public enum PlayerAttack
                {
                    PlayerAttack
                }

                [JsonProperty("eventType")]
                public PlayerAttack Type => PlayerAttack.PlayerAttack;

                [JsonProperty("playerId")]
                public int playerId { get; init; }

                [JsonProperty("targetPlayerId")]
                public int targetPlayerId { get; init; }
            }

            public record PlayerDigEvent
            {
                public enum PlayerDig
                {
                    PlayerDig
                }

                [JsonProperty("eventType")]
                public PlayerDig Type => PlayerDig.PlayerDig;

                [JsonProperty("playerId")]
                public int playerId { get; init; }

                [JsonProperty("targetChunk")]
                public int targetChunk { get; init; }

            }

            public record PlayerPickUpEvent
            {
                public enum PlayerPickUp
                {
                    PlayerPickUp
                }

                [JsonProperty("eventType")]
                public PlayerPickUp Type => PlayerPickUp.PlayerPickUp;

                [JsonProperty("playerId")]
                public int playerId { get; init; }

                [JsonProperty("mineId")]
                public string mineId { get; init; }

                [JsonProperty("itemType")]
                public ItemType itemType { get; init; }

                [JsonProperty("itemCount")]
                public int itemCount { get; init; }

                public enum ItemType
                {
                    IronIngot,

                    GoldIngot,

                    Diamond

                }
            }

            public record PlayerPlaceBlockEvent
            {
                public enum PlayerPlaceBlock
                {
                    PlayerPlaceBlock
                }

                [JsonProperty("eventType")]
                public PlayerPlaceBlock Type => PlayerPlaceBlock.PlayerPlaceBlock;

                [JsonProperty("playerId")]
                public int playerId { get; init; }

                [JsonProperty("blockType")]
                public BlockType blockType => BlockType.Wool;

                public enum BlockType
                {
                    Wool
                }
            }

            public record PlayerTryAttackEvent
            {
                public enum PlayerTryAttack
                {
                    PlayerTryAttack
                }

                [JsonProperty("eventType")]
                public PlayerTryAttack Type => PlayerTryAttack.PlayerTryAttack;

                [JsonProperty("playerId")]
                public int playerId { get; init; }

                [JsonProperty("targetChunk")]
                public int targetChunk { get; init; }

            }

            public record PlayerTryUseEvent
            {
                public enum PlayerTryUse
                {
                    PlayerTryUse
                }

                [JsonProperty("eventType")]
                public PlayerTryUse Type => PlayerTryUse.PlayerTryUse;

                [JsonProperty("playerId")]
                public int playerId { get; init; }

                [JsonProperty("targetChunk")]
                public int targetChunk { get; init; }

            }

        }

        public record Info
        {
            [JsonProperty("elapsedTicks")]
            public int elapsedTicks { get; init; }

            [JsonProperty("stage")]
            public Stage stage { get; init; }

            public enum Stage
            {
                Ready,

                Running,

                Battling,

                Finished,

                Ended,
            }
        }

        public record Mine
        {
            [JsonProperty("mineId")]
            public string mineId { get; init; }

            [JsonProperty("accumulatedOreCount")]
            public int accumulatedOreCount { get; init; }

            [JsonProperty("oreType")]
            public OreType oreType { get; init; }

            public enum OreType
            {
                IronOre,

                GoldOre,

                DiamondOre

            }

            [JsonProperty("position")]
            public Position? position { get; init; }

            public record Position
            {
                [JsonProperty("x")]
                public float x { get; init; }

                [JsonProperty("y")]
                public float y { get; init; }

            }
        }

        public record Player
        {
            [JsonProperty("playerId")]
            public int playerId { get; init; }

            [JsonProperty("cameraId")]
            public int cameraId { get; init; }

            [JsonProperty("attributes")]
            public Attributes? attributes { get; init; }

            public record Attributes
            {
                [JsonProperty("agility")]
                public int agility { get; init; }

                [JsonProperty("maxHealth")]
                public int maxHealth { get; init; }

                [JsonProperty("strength")]
                public int strength { get; init; }

            }

            [JsonProperty("health")]
            public int health { get; init; }

            [JsonProperty("homePosition")]
            public HomePosition? homePosition { get; init; }

            public record HomePosition
            {
                [JsonProperty("x")]
                public float x { get; init; }

                [JsonProperty("y")]
                public float y { get; init; }

            }

            [JsonProperty("inventory")]
            public Inventory? inventory { get; init; }

            public record Inventory
            {
                [JsonProperty("emerald")]
                public int emerald { get; init; }

                [JsonProperty("wool")]
                public int wool { get; init; }

            }

            [JsonProperty("position")]
            public Position? position { get; init; }

            public record Position
            {
                [JsonProperty("x")]
                public float x { get; init; }

                [JsonProperty("y")]
                public float y { get; init; }

            }
        }
    }
    }
