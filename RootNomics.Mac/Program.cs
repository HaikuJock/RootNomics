
using RootNomics.Mac;
using RootNomicsGame;

using var game = new RootNomicsGame.RootNomics(
    new MousePressEventProvider(),
    new BrowserOpener(),
    new TextClipboard(),
    new NullAudioPlayer());
game.Run();
