using System;

namespace Murasaki {
    public class Program {
        public static void Main() {
            CGameEngine game = new CGameEngine();
            game.Init("Engine Test");
            //game.ChangeState(CIntroState.Instance());
            game.ChangeState(CPlayState.Instance());
            game.Run();
        }
    }
}
