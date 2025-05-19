using UnityEngine;

/// <summary>
/// Unity�� JsonUtility�� �迭 �Ǵ� ����Ʈ ��ü�� ���� ������ȭ�� �� ���� ������
/// ���� Ŭ������ ���� �迭�� ���μ� ó���ϴ� ���� Ŭ����.
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// JSON ���ڿ��� �迭�� ������ȭ��.
    /// ��� ��: JsonHelper.FromJson&lt;MyType&gt;(jsonString);
    /// </summary>
    /// <typeparam name="T">������ȭ�� ��ü Ÿ��</typeparam>
    /// <param name="json">JSON ���ڿ� (�迭 ����)</param>
    /// <returns>T Ÿ���� �迭</returns>
    public static T[] FromJson<T>(string json)
    {
        // �迭�� ���δ� ���ο� JSON �������� ��ȯ
        string newJson = "{ \"array\": " + json + "}";

        // ���� JSON�� Wrapper ��ü�� ������ȭ
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);

        // �迭 ��ȯ
        return wrapper.array;
    }

    /// <summary>
    /// �迭�� ���α� ���� ���� Ŭ����
    /// </summary>
    /// <typeparam name="T">���δ� �迭�� Ÿ��</typeparam>
    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}
