using UnityEngine;

/// <summary>
/// Unity의 JsonUtility는 배열 또는 리스트 자체를 직접 역직렬화할 수 없기 때문에
/// 래퍼 클래스를 통해 배열을 감싸서 처리하는 헬퍼 클래스.
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// JSON 문자열을 배열로 역직렬화함.
    /// 사용 예: JsonHelper.FromJson&lt;MyType&gt;(jsonString);
    /// </summary>
    /// <typeparam name="T">역직렬화할 객체 타입</typeparam>
    /// <param name="json">JSON 문자열 (배열 형태)</param>
    /// <returns>T 타입의 배열</returns>
    public static T[] FromJson<T>(string json)
    {
        // 배열을 감싸는 새로운 JSON 포맷으로 변환
        string newJson = "{ \"array\": " + json + "}";

        // 감싼 JSON을 Wrapper 객체로 역직렬화
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);

        // 배열 반환
        return wrapper.array;
    }

    /// <summary>
    /// 배열을 감싸기 위한 래퍼 클래스
    /// </summary>
    /// <typeparam name="T">감싸는 배열의 타입</typeparam>
    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}
