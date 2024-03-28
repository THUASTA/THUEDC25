using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using EDCViewer.Messages;

public class CameraDisplay : MonoBehaviour
{
    public GameObject CameraListObj;
    public List<RawImage> RawImageList=new();
    private void Start()
    {
        CameraListObj = GameObject.Find("Canvas/CameraList");
        foreach(RawImage rawImage in CameraListObj.GetComponentsInChildren<RawImage>())
        {
            RawImageList.Add(rawImage);
        }
        //CameraListObj.SetActive(false);
         
    }

    // This method is called to display the camera image
    public void DisplayCameraImage(CompetitionUpdate competitionUpdate)
    {
        //UnityMainThreadDispatcher.Enqueue(() =>
        //{
            for (int i = 0; i < competitionUpdate.cameras.Count; i++)
            {
                //int cameraId = competitionUpdate.cameras[i].cameraId;

                if (i >= RawImageList.Count)
                {
                    GameObject newCameraObj = new();
                    RawImageList.Add(newCameraObj.AddComponent<RawImage>());

                    newCameraObj.transform.parent = CameraListObj.transform;
                }


                // Get the camera frame data (base64 encoded JPEG image)
                string frameData = competitionUpdate.cameras[i].frameData;

                // Decode the frame data to a texture
                Texture2D texture = DecodeBase64Jpeg(frameData);

                // Destroy old texture
                if (RawImageList[i].texture != null)
                {
                    Destroy(RawImageList[i].texture);
                }
                // Set the texture to the RawImage component
                RawImageList[i].texture = texture; 
            }
        //});
    }

    public Texture2D DecodeBase64Jpeg(string base64Data)
    {
        byte[] imageData = System.Convert.FromBase64String(base64Data);
        Texture2D texture = new(1, 1);
        texture.LoadImage(imageData);
        return texture;
    }


}
