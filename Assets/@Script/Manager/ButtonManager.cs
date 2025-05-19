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
        Debug.Log("F4 ��ư ����");
        // ��: SceneManager.LoadScene("MainScene");
    }

    void OnClickC()
    {
        Debug.Log("C ��ư ����");
        // ���ϴ� ���
    }

    void OnClickESC()
    {
        Debug.Log("ESC ��ư ����");
        Application.Quit(); // ���� ����
    }

    void OnClickV()
    {
        Debug.Log("V ��ư ����");
        // ���ϴ� ���
    }

    void OnClickE()
    {
        Debug.Log("E ��ư ����");
        // ���ϴ� ���
    }
}
