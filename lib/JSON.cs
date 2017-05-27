public static class JSON
{
    public static string Serialize(object value) => 
        Newtonsoft.Json.JsonConvert.SerializeObject(value);
        
    public static T Deserialize<T>(string json) => 
        Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
}