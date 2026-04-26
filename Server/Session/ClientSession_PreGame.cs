using Google.Protobuf.Protocol;
using Server.Game;
using ServerCore;

namespace Server;

public partial class ClientSession : PacketSession
{
    public void HandleSignUp(C_SignUp packet)
    {
        // 회원가입 시작
        string email = packet.AccountInfo.Email;
        string password = packet.AccountInfo.Password;
        bool isSuccess = AccountService.Instance.SignUp(email, password);
        if (isSuccess)
            Send(new S_SignUp());
    }

    public void HandleSignIn(C_SignIn packet)
    {
        string email = packet.AccountInfo.Email;
        string password = packet.AccountInfo.Password;
        
        SignInResult result = AccountService.Instance.SignIn(email, password);
        if (result != null)
        {
            S_SignIn signInPacket = new S_SignIn();
            signInPacket.Player = result.Player.Mapper();
            Send(signInPacket);
        }
    }
    
    public void HandleEnterGame(C_EnterGame enterGamePacket)
    {
        // 게임 룸에 입장
        GameRoom room = GameRoomManager.Instance.Find(1);
    }
}