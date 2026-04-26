using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;

namespace Server;

public class PacketHandler
{
    public static void C_SignUp(PacketSession session, IMessage packet)
    {
        C_SignUp signUpPacket = (C_SignUp)packet;
        ClientSession clientSession = (ClientSession)session;
        clientSession.HandleSignUp(signUpPacket);
    }

    public static void C_SignIn(PacketSession session, IMessage packet)
    {
        C_SignIn signInPacket = (C_SignIn)packet;
        ClientSession clientSession = (ClientSession)session;
        clientSession.HandleSignIn(signInPacket);
    }
    
    public static void C_EnterGameHandler(PacketSession session, IMessage packet)
    {
        C_EnterGame enterGamePacket = (C_EnterGame)packet;
        ClientSession clientSession = (ClientSession)session;
        clientSession.HandleEnterGame(enterGamePacket);
    }
}