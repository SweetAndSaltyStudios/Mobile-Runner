using SweetAndSaltyStudios;

public class PauseScreen : BaseUIScreen
{
    public override void OpenScreen()
    {
        base.OpenScreen();

        GameManager.Instance.ChangeGameState(GAME_STATE.PAUSE);
    }

    public override void CloseScreen()
    {
        base.CloseScreen();

        GameManager.Instance.ChangePreviousGameState();
    }
}
