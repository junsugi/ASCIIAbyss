namespace DummyClient.View;

public class ASCIIView
{
    private static readonly ASCIIView _instance = new ASCIIView();
    public static ASCIIView Instance => _instance;
    
    public string Logo()
    {
        return """
                 ___   _____ _____ _____ _____    ___  ________   _______ _____ 
                / _ \ /  ___/  __ \_   _|_   _|  / _ \ | ___ \ \ / /  ___/  ___|
               / /_\ \\ `--.| /  \/ | |   | |   / /_\ \| |_/ /\ V /\ `--.\ `--. 
               |  _  | `--. \ |     | |   | |   |  _  || ___ \ \ /  `--. \`--. \
               | | | |/\__/ / \__/\_| |_ _| |_  | | | || |_/ / | | /\__/ /\__/ /
               \_| |_/\____/ \____/\___/ \___/  \_| |_/\____/  \_/ \____/\____/ )

               """;
    }
}