using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public record CompetitionUpdateMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "COMPETITION_UPDATE";

    [JsonPropertyName("cameras")]
    public List<Camera> cameras { get; init; } = new();

    [JsonPropertyName("chunks")]
    public List<Chunk> chunks { get; init; } = new();

    [JsonPropertyName("events")]
    public List<Event> events { get; init; } = new();

    [JsonPropertyName("info")]
    public Info info { get; init; } = new();

    [JsonPropertyName("mines")]
    public List<Mine> mines { get; init; } = new();

    [JsonPropertyName("players")]
    public List<Player> players { get; init; } = new();

    public record Camera
    {
        [JsonPropertyName("cameraId")]
        public int cameraId { get; init; }

        [JsonPropertyName("frameData")]
        public string? frameData { get; init; }

        [JsonPropertyName("height")]
        public int height { get; init; }

        [JsonPropertyName("width")]
        public int width { get; init; }

    }

    public record Chunk
    {
        [JsonPropertyName("chunkId")]
        public int chunkId { get; init; }

        [JsonPropertyName("height")]
        public int height { get; init; }

        [JsonPropertyName("position")]
        public Position? position { get; init; }

        public record Position
        {
            [JsonPropertyName("x")]
            public int x { get; init; }

            [JsonPropertyName("y")]
            public int y { get; init; }

        }
    }

    public record Event
    {
        [JsonPropertyName("PlayerAttackEvent")]
        public PlayerAttackEvent? playerAttackEvent { get; init; }

        [JsonPropertyName("PlayerDigEvent")]
        public PlayerDigEvent? playerDigEvent { get; init; }

        [JsonPropertyName("PlayerPickUpEvent")]
        public PlayerPickUpEvent? playerPickUpEvent { get; init; }

        [JsonPropertyName("PlayerPlaceBlockEvent")]
        public PlayerPlaceBlockEvent? playerPlaceBlockEvent { get; init; }

        [JsonPropertyName("PlayerTryAttackEvent")]
        public PlayerTryAttackEvent? playerTryAttackEvent { get; init; }

        [JsonPropertyName("PlayerTryUseEvent")]
        public PlayerTryUseEvent? playerTryUseEvent { get; init; }

        public record PlayerAttackEvent
        {
            public enum PlayerAttack
            {
                PlayerAttack
            }

            [JsonPropertyName("eventType")]
            public PlayerAttack Type => PlayerAttack.PlayerAttack;

            [JsonPropertyName("playerId")]
            public int playerId { get; init; }

            [JsonPropertyName("targetPlayerId")]
            public int targetPlayerId { get; init; }
        }

        public record PlayerDigEvent
        {
            public enum PlayerDig
            {
                PlayerDig
            }

            [JsonPropertyName("eventType")]
            public PlayerDig Type => PlayerDig.PlayerDig;

            [JsonPropertyName("playerId")]
            public int playerId { get; init; }

            [JsonPropertyName("targetChunk")]
            public int targetChunk { get; init; }

        }

        public record PlayerPickUpEvent
        {
            public enum PlayerPickUp
            {
                PlayerPickUp
            }

            [JsonPropertyName("eventType")]
            public PlayerPickUp Type => PlayerPickUp.PlayerPickUp;

            [JsonPropertyName("playerId")]
            public int playerId { get; init; }

            [JsonPropertyName("itemType")]
            public ItemType itemType { get; init; }

            [JsonPropertyName("itemCount")]
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

            [JsonPropertyName("eventType")]
            public PlayerPlaceBlock Type => PlayerPlaceBlock.PlayerPlaceBlock;

            [JsonPropertyName("playerId")]
            public int playerId { get; init; }

            [JsonPropertyName("blockType")]
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

            [JsonPropertyName("eventType")]
            public PlayerTryAttack Type => PlayerTryAttack.PlayerTryAttack;

            [JsonPropertyName("playerId")]
            public int playerId { get; init; }

            [JsonPropertyName("targetChunk")]
            public int targetChunk { get; init; }

        }

        public record PlayerTryUseEvent
        {
            public enum PlayerTryUse
            {
                PlayerTryUse
            }

            [JsonPropertyName("eventType")]
            public PlayerTryUse Type => PlayerTryUse.PlayerTryUse;

            [JsonPropertyName("playerId")]
            public int playerId { get; init; }

            [JsonPropertyName("targetChunk")]
            public int targetChunk { get; init; }

        }

    }

    public record Info
    {
        [JsonPropertyName("elapsedTicks")]
        public int elapsedTicks { get; init; }

        [JsonPropertyName("stage")]
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
        [JsonPropertyName("mineId")]
        public string? mineId { get; init; }

        [JsonPropertyName("accumulatedOreCount")]
        public int accumulatedOreCount { get; init; }

        [JsonPropertyName("oreType")]
        public OreType oreType { get; init; }

        public enum OreType
        {
            IronOre,

            GoldOre,

            DiamondOre

        }

        [JsonPropertyName("position")]
        public Position? position { get; init; }

        public record Position
        {
            [JsonPropertyName("x")]
            public float x { get; init; }

            [JsonPropertyName("y")]
            public float y { get; init; }

        }
    }

    public record Player
    {
        [JsonPropertyName("playerId")]
        public int playerId { get; init; }

        [JsonPropertyName("cameraId")]
        public int cameraId { get; init; }

        [JsonPropertyName("attributes")]
        public Attributes? attributes { get; init; }

        public record Attributes
        {
            [JsonPropertyName("agility")]
            public int agility { get; init; }

            [JsonPropertyName("maxHealth")]
            public int maxHealth { get; init; }

            [JsonPropertyName("strength")]
            public int strength { get; init; }

        }

        [JsonPropertyName("health")]
        public int health { get; init; }

        [JsonPropertyName("homePosition")]
        public HomePosition? homePosition { get; init; }

        public record HomePosition
        {
            [JsonPropertyName("x")]
            public float x { get; init; }

            [JsonPropertyName("y")]
            public float y { get; init; }

        }

        [JsonPropertyName("inventory")]
        public Inventory? inventory { get; init; }

        public record Inventory
        {
            [JsonPropertyName("emerald")]
            public int emerald { get; init; }

            [JsonPropertyName("wool")]
            public int wool { get; init; }

        }

        [JsonPropertyName("position")]
        public Position? position { get; init; }

        public record Position
        {
            [JsonPropertyName("x")]
            public float x { get; init; }

            [JsonPropertyName("y")]
            public float y { get; init; }

        }
    }
}
