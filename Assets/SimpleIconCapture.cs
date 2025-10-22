using UnityEngine;

public class SimpleIconCapture : MonoBehaviour
{
    public GameObject[] modelsToCapture;
    public Transform capturePoint;

    private void Start()
    {
        GenerateAllIcons();
    }

    public void GenerateAllIcons()
    {
        foreach (GameObject model in modelsToCapture)
        {
            if (model != null)
            {
                CreateIconForModel(model);
            }
        }
    }

    private void CreateIconForModel(GameObject model)
    {
        GameObject modelCopy = Instantiate(model, capturePoint.position, Quaternion.identity);

        modelCopy.transform.rotation = Quaternion.Euler(0, 45, 0);

        StartCoroutine(TakeScreenshotAfterFrame(model.name, modelCopy));
    }

    private System.Collections.IEnumerator TakeScreenshotAfterFrame(string modelName, GameObject modelCopy)
    {
        yield return new WaitForEndOfFrame();

        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        byte[] bytes = screenshot.EncodeToPNG();
        string filename = $"Icon_{modelName}_{System.DateTime.Now:yyyyMMddHHmmss}.png";
        System.IO.File.WriteAllBytes(Application.dataPath + "/" + filename, bytes);

        Debug.Log($"Screenshot saved: {filename}");

        DestroyImmediate(modelCopy);
    }
}