using Newtonsoft.Json;
using Org.Json;

namespace SocketIOClientJavaSample
{
    public class Data
    {
        public string username;
        public string message;
        public int numUsers;

        public static Data FromData(Java.Lang.Object[] data)
        {
            if (data != null && data.Length == 1)
            {
                var json = data[0] as JSONObject;
                return JsonConvert.DeserializeObject<Data>(json.ToString());
            }
            return null;
        }
    }
}
