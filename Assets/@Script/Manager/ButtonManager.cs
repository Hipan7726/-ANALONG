using UnityEngine;
using UnityEngine.UI;

public class StartGameButtonManager : MonoBehaviour
{
    public Button ButtonF4;
    public Button ButtonC;
    public Button ButtonESC;
    public Button ButtonV;
    public Button ButtonE;

    private void Start()
    {
        ButtonF4.onClick.AddListener(OnClickF4);
        ButtonC.onClick.AddListener(OnClickC);
        ButtonESC.onClick.AddListener(OnClickESC);
        ButtonV.onClick.AddListener(OnClickV);
        ButtonE.onClick.AddListener(OnClickE);
    }

    void OnClickF4()
    {
        Debug.Log("F4 버튼 눌림");
        // 예: SceneManager.LoadScene("MainScene");
    }

    void OnClickC()
    {
        Debug.Log("C 버튼 눌림");
        // 원하는 기능
    }

    void OnClickESC()
    {
        Debug.Log("ESC 버튼 눌림");
        Application.Quit(); // 게임 종료
    }

    void OnClickV()
    {
        Debug.Log("V 버튼 눌림");
        // 원하는 기능
    }

    void OnClickE()
    {
        Debug.Log("E 버튼 눌림");
        // 원하는 기능
    }
}
