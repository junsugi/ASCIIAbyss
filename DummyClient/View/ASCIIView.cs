using Spectre.Console;

namespace DummyClient.View;

public class ASCIIView
{
    private static readonly ASCIIView _instance = new ASCIIView();
    public static ASCIIView Instance => _instance;
    
    private Table _chatTable;
    private Layout _layout;
    
    
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
    
    public void ChatClient()
    {
        // 1. 레이아웃 초기 설정
        _chatTable = new Table().Border(TableBorder.None).Expand().AddColumn("Chat Log");
        _layout = new Layout("Root")
            .SplitRows(
                new Layout("Chat"),
                new Layout("Input").Size(3)
            );
    }

    public async void CreateGameRoomView(GameRoom gameRoom, Player player)
    {
        ChatClient();
        
        // 하단 가이드 출력
        _layout["Input"].Update(new Panel(new Text("메시지를 입력하고 Enter를 누르세요.")).Border(BoxBorder.None));

        // 2. Live Display 시작 (화면 전체 제어권 획득)
        await AnsiConsole.Live(_layout).StartAsync(async ctx =>
        {
            // [핵심] 입력을 담당하는 태스크를 별도로 실행
            var inputTask = Task.Run(() =>
            {
                while (true)
                {
                    // 유저로부터 입력을 받음 (이 순간 콘솔은 입력 대기 상태가 됨)
                    var msg = AnsiConsole.Ask<string>("[bold yellow]>[/] ");

                    // 큐에 메시지 삽입 (내가 보낸 메시지)
                    gameRoom.AddMessage(msg);
                    // ※ 실제로는 여기서 서버로 패킷 전송 로직이 들어감 (예: _session.Send(chatPacket))
                }
            });

            // 3. 메인 렌더링 루프 (출력 전용)
            while (true)
            {
                // 큐에 쌓인 메시지가 있다면 테이블에 추가
                while (gameRoom.TryDequeueMessage(out var message))
                {
                    _chatTable.AddRow(new Markup(message));
                    
                    // 스크롤 효과: 메시지가 너무 많으면 위에서부터 삭제 (예: 15줄 유지)
                    if (_chatTable.Rows.Count > 15)
                    {
                        _chatTable.Rows.RemoveAt(0);
                    }
                }

                // 채팅 레이아웃 업데이트
                _layout["Chat"].Update(
                    new Panel(_chatTable)
                        .Header("[bold green] Real-time Chat Room [/]")
                        .Expand()
                        .Border(BoxBorder.Rounded)
                );

                // 화면 갱신
                ctx.Refresh();

                // UI 갱신 부하 조절 (너무 빠르면 CPU 점유율 상승)
                await Task.Delay(100);
            }
        });
    }
}