
#include "gameengine.h"
#include "introstate.h"

int main ( int argc, char *argv[] ) {
	CGameEngine game;
	game.Init( "Engine Test v1.0" );
	game.ChangeState( CIntroState::Instance() );
	game.Run();
	game.Cleanup();
	return 0;
}
