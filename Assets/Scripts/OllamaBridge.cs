using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class OllamaBridge : MonoBehaviour
{
    // Cổng API mặc định của Ollama chạy local
    private string ollamaUrl = "http://localhost:11434/api/generate";

    void Start()
    {
        // Vừa Play game là chào hỏi luôn
        StartCoroutine(SendRequestToOllama("Xin chào, bạn có phải là Mita không?"));
    }

    IEnumerator SendRequestToOllama(string promptText)
    {
        // 1. Đóng gói dữ liệu gửi đi (Lưu ý: model phải ghi chuẩn xác tên bạn vừa bật, stream: false để nhận 1 cục text thay vì từng chữ)
        string jsonData = "{\"model\": \"llama3.1:8b\", \"prompt\": \"" + promptText + "\", \"stream\": false}";

        // 2. Thiết lập đường truyền POST
        UnityWebRequest request = new UnityWebRequest(ollamaUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Đang gửi tín hiệu đến Llama 3.1:8b... Vui lòng chờ...");

        // 3. Chờ phản hồi từ AI
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Lỗi kết nối: " + request.error);
        }
        else
        {
            // 4. Bóc tách JSON trả về để lấy nguyên phần text
            OllamaResponse aiResponse = JsonUtility.FromJson<OllamaResponse>(request.downloadHandler.text);
            Debug.Log("Mita trả lời: " + aiResponse.response);
        }
    }

    // Class phụ để Unity hiểu cấu trúc JSON của Ollama
    [System.Serializable]
    public class OllamaResponse
    {
        public string response;
    }
}