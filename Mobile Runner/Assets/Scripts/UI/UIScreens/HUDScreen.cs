using SweetAndSaltyStudios;

public class HUDScreen : BaseUIScreen
{
    protected override void Awake()
    {
        base.Awake();

        GameManager.Instance.ChangeGameState(GAME_STATE.START);
    }

    protected override void OnEnable()
    {
        base.OnEnable();   
    }

    public override void OpenScreen()
    {
        base.OpenScreen();
    }
}
