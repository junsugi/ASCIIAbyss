using System.Net;
using DummyClient.View;
using Google.Protobuf.Protocol;
using Spectre.Console;

namespace DummyClient;

class Program
{
    #region Spectre.Console

    static async Task Main(string[] args)
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 5555);
        Connector connector = new Connector();
        ServerSession session = SessionManager.Instance.Generate();
        connector.Connect(endPoint, session);
        
        AnsiConsole.MarkupLine(ASCIIView.Instance.Logo());
        AnsiConsole.Status().Start("Loading...", ctx =>
        {
            
            Thread.Sleep(2000);
        });

        Player player = null;
        
        if (session.IsConnected)
        {
            while (true)
            {
                List<string> menuChoices = new List<string>();
                if (session.UIState == PlayerClientUIState.UiLogin)
                {
                    menuChoices.Add("회원가입");
                    menuChoices.Add("로그인");
                }
                else if (session.UIState == PlayerClientUIState.UiLobby)
                {
                    menuChoices.Add("방만들기");
                    menuChoices.Add("로비");
                }
                
                var menuInput = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold]메뉴[/]를 선택해주세요.")
                        .MoreChoicesText("[grey](더 보려면 위아래로 움직이세요)[/]")
                        .AddChoices(menuChoices));

                AnsiConsole.MarkupLine($"{menuInput}을 선택하였습니다.\n");

                string email = "";
                string password = "";

                switch (menuInput)
                {
                    case "회원가입" when session.UIState == PlayerClientUIState.UiLogin:
                        email = AnsiConsole.Ask<string>("원하시는 [green]이메일[/]을 입력해주세요. : ");
                        password = AnsiConsole.Prompt(
                            new TextPrompt<string>("원하시는 [bold green]비밀번호[/]를 입력하세요:")
                                .PromptStyle("red")
                                .Secret('*'));
                        
                        session.SignUp(email, password);
                        
                        AnsiConsole.MarkupLine("[bold blue]회원가입[/]이 완료되었습니다! \n");
                        break;
                    case "로그인" when session.UIState == PlayerClientUIState.UiLogin:
                        email = AnsiConsole.Ask<string>("[green]이메일[/]을 입력해주세요. : ");
                        password = AnsiConsole.Prompt(
                            new TextPrompt<string>("[bold green]비밀번호[/]를 입력하세요:")
                                .PromptStyle("red")
                                .Secret('*'));
                        // account가 Null인지 아닌지에 따라 다음 스텝
                        try
                        {
                             player = await session.SignInAsync(email, password);
                            if (session.UIState == PlayerClientUIState.UiLobby)
                            {
                                AnsiConsole.MarkupLine($"[bold blue]{player.DisplayName}[/]이 로그인 하셨습니다! \n");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case "방만들기" when session.UIState == PlayerClientUIState.UiLobby:
                        string displayName = AnsiConsole.Ask<string>("[green]방 이름[/]을 입력해주세요. : ");
                        GameRoom gameRoom = await session.CreateRoomAsync(displayName);
                        AnsiConsole.MarkupLine($"{gameRoom.Id} / {gameRoom.displayName}");
                        
                        // 방 입장 뷰
                        gameRoom = await session.EnterGame(gameRoom.Id, player);
                        if (gameRoom != null)
                        {
                            ASCIIView.Instance.CreateGameRoomView(gameRoom, player);
                        }
                        break;
                    case "로비" when session.UIState == PlayerClientUIState.UiLobby:
                        Dictionary<int, GameRoom> gameRooms = await session.LobbyAsync();
                        if (gameRooms.Count == 0)
                        {
                            AnsiConsole.MarkupLine("생성된 방이 없습니다.");
                        }
                        else
                        {
                            var lobbyInput = AnsiConsole.Prompt<GameRoom>(
                                new SelectionPrompt<GameRoom>()
                                    .PageSize(5)
                                    .Title("[bold]메뉴[/]를 선택해주세요.")
                                    .MoreChoicesText("[grey](더 보려면 위아래로 움직이세요)[/]")
                                    .AddChoices(gameRooms.Values)
                                    .UseConverter(room => $"{room.displayName} - {room.playerCount} / 4"));

                            // 방 입장 뷰
                            gameRoom = await session.EnterGame(lobbyInput.Id, player);
                            
                        }
                        break;
                    case "종료":
                        AnsiConsole.MarkupLine($"게임을 [bold red]종료[/]합니다.");
                        break;
                    default:
                        break;
                }
            }
        }
    }

    #endregion
}