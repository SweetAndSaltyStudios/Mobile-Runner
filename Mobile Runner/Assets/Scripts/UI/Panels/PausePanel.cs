using UnityEngine;

public class PausePanel : UIPanel
{
    public override void Open()
    {
        base.Open();

        Time.timeScale = 0f;
    }

    public override void Close()
    {
        base.Close();

        Time.timeScale = 1f;
    }
}
