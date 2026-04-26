using System.Text;
using Newtonsoft.Json;

namespace Server;

public class Map
{
    private Dictionary<int, MapData> mapDict = new Dictionary<int, MapData>();

    public void LoadMap(string prefix = "../../../../Common/MapData/")
    {
        // JSON 데이터 파싱해서 id별로 Dictionary에 저장
        string fileName = "MapData.json";
        string filePath = prefix + fileName;

        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath, Encoding.UTF8);
            MapDataRes results = JsonConvert.DeserializeObject<MapDataRes>(jsonString);
            if (results != null && results.maps != null)
            {
                mapDict = results.maps.ToDictionary(x => x.id, x => x);
            }
        }
    }

    public MapData Find(int mapId)
    {
        MapData mapdata = null;
        if (mapDict.TryGetValue(mapId, out mapdata))
            return mapdata;

        return null;
    }
}

#region JSON 파싱용 Class
public class MapDataRes
{
    public List<MapData> maps { get; set; }
}

public class MapData
{
    public int id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public List<Exit> exits = new List<Exit>();
    public Event events { get; set; }
    public List<MonsterSpawn> monsterSpawns = new List<MonsterSpawn>();
}

public class Exit
{
    public int nextMapId { get; set; }
    public string direction { get; set; }
}

public class Event
{
    public EventType type { get; set; }

    public string question { get; set; }

    // 나중에 Options 클래스 생성해야될듯
    public List<string> options { get; set; }
    public int timer { get; set; }

    public enum EventType
    {
        NONE, // 기본값
        VOTE
    }
}

public class MonsterSpawn
{
    public int monsterId { get; set; }
    public int chance { get; set; }
}

#endregion