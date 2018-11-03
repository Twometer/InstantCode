using InstantCode.Protocol.Packets;

namespace InstantCode.Protocol.Handler
{
    public interface INetHandler
    {
        void HandleP00Login(P00Login p00Login);

        void HandleP01State(P01State p01State);

        void HandleP02NewSession(P02NewSession p02NewSession);

        void HandleP03CloseSession(P03CloseSession p03CloseSession);

        void HandleP04OpenStream(P04OpenStream p04OpenStream);

        void HandleP05StreamData(P05StreamData p05StreamData);

        void HandleP06CloseStream(P06CloseStream p06CloseStream);

        void HandleP07CodeChange(P07CodeChange p07CodeChange);

        void HandleP08CursorPosition(P08CursorPosition p08CursorPosition);

        void HandleP09Save(P09Save p09Save);

        void HandleP0AUserList(P0AUserList p0AUserList);
    }
}
