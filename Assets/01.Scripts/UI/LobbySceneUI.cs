using UnityEngine;

public class LobbySceneUI : MonoBehaviour
{
    public void PopUp(GameObject popUp)
    {
        popUp.SetActive(!popUp.activeSelf);
    }
}
