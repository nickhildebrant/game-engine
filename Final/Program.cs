bool ShouldRestart = false;

do
{
    using var game = new Final.Final();
    game.Run();
}
while(ShouldRestart);