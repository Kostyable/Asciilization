namespace Asciilization;

public static class Game
{
    public static Random random;
    public static Map map;
    public static Civilization[] civilizations;
    public static Civilization playerCiv;

    public static void Init()
    {
        random = new Random();
        map = new Map(100, 50);
        civilizations = new Civilization[Enum.GetNames(typeof(CivNames)).Length];
        for (int i = 0; i < civilizations.Length; i++)
        {
            civilizations[i] = new Civilization((CivNames)i);
        }
        playerCiv = civilizations[random.Next(Enum.GetNames(typeof(CivNames)).Length)];
    }
    
    public static void Main(string[] args)
    {
        Console.Title = "ASCIILIZATION";
        Init();
        Output.Init(16, 8, 0, 0, 3);
        Launch(map, civilizations);
        Control.Map(map);
    }
    
    public static void Launch(Map map, Civilization[] civilizations)
    {
        Generation.Map(map);
        Generation.Rivers(map, 7, 5);
        Generation.Settlers(map, civilizations);
        map.Select(playerCiv.units[0].currentHex);
        Control.CursorInCenter(map);
        Output.Map(map);
    }
}