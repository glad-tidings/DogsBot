using System.Text.Json.Serialization;

namespace Dogs
{
    public class DogsQuery
    {
        public int Index { get; set; }
        public string Name { get; set; } = string.Empty;
        public string API_ID { get; set; } = string.Empty;
        public string API_HASH { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Auth { get; set; } = string.Empty;
        public bool Active { get; set; }
        public bool DailyReward { get; set; }
        public int[] DaySleep { get; set; } = [];
        public int[] NightSleep { get; set; } = [];
    }

    public class DogsJoinResponse
    {
        [JsonPropertyName("telegram_id")]
        public long TelegramId { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
        [JsonPropertyName("age")]
        public int Age { get; set; }
        [JsonPropertyName("is_premium")]
        public bool IsPremium { get; set; }
        [JsonPropertyName("balance")]
        public int Balance { get; set; }
        [JsonPropertyName("reference")]
        public string Reference { get; set; } = string.Empty;
    }

    public class DogsRewardsResponse
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("claimable")]
        public int Claimable { get; set; }
        [JsonPropertyName("age")]
        public int Age { get; set; }
        [JsonPropertyName("premium")]
        public int Premium { get; set; }
        [JsonPropertyName("frens")]
        public int Frens { get; set; }
        [JsonPropertyName("boost")]
        public int Boost { get; set; }
        [JsonPropertyName("connect")]
        public int Connect { get; set; }
        [JsonPropertyName("daily")]
        public int Daily { get; set; }
        [JsonPropertyName("streak")]
        public int Streak { get; set; }
        [JsonPropertyName("tasks")]
        public int Tasks { get; set; }
    }

    public class DogsCalendarItem
    {
        [JsonPropertyName("ID")]
        public int ID { get; set; }
        [JsonPropertyName("IsKey")]
        public bool IsKey { get; set; }
        [JsonPropertyName("IsChecked")]
        public bool IsChecked { get; set; }
        [JsonPropertyName("IsCurrent")]
        public bool IsCurrent { get; set; }
        [JsonPropertyName("IsAvailable")]
        public bool IsAvailable { get; set; }
        [JsonPropertyName("IsPayed")]
        public bool IsPayed { get; set; }
    }

    public class ProxyType
    {
        [JsonPropertyName("Index")]
        public int Index { get; set; }
        [JsonPropertyName("Proxy")]
        public string Proxy { get; set; } = string.Empty;
    }

    public class Httpbin
    {
        [JsonPropertyName("origin")]
        public string Origin { get; set; } = string.Empty;
    }
}
