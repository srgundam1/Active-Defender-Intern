using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity.UnityUtils.Helper;
using UnityEngine.UI;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;

namespace OpenCVForUnityExample
{
    public class Screenshot : MonoBehaviour
    {

        Dnn_Object_Detection obj;
        FaceDetection_WebCam web;
        [SerializeField] GameObject quad;
        CascadeClassifier cascade;
        protected static readonly string HAAR_CASCADE_FILENAME = "objdetect/haarcascade_frontalface_alt.xml";



        void Awake()
        {
            obj = quad.GetComponent<Dnn_Object_Detection>();
            web = quad.GetComponent<FaceDetection_WebCam>();
        }
        
        void Start()
        {
          

                Button button = gameObject.GetComponent<Button>();

                button.onClick.AddListener(() =>
                {
                    
                    WebCamTextureToMatHelper cam = obj.webCamTextureToMatHelper;
                    Texture2D snap = new Texture2D(cam.GetWidth(), cam.GetHeight());
                    WebCamTexture txt = cam.GetWebCamTexture();
                    snap.SetPixels(txt.GetPixels());
                    snap.Apply();
                    //string path = SaveTexture(snap);
                    //Bitmap myBitmap = new Bitmap(path);
                    //Texture2D temp = Resources.Load("R_82301") as Texture2D;
                    Mat imgMat = new Mat(snap.height, snap.width, CvType.CV_8UC4);
                    Utils.texture2DToMat(snap, imgMat);
                    //myBitmap.Save(outPath, System.Drawing.Imaging.ImageFormat.Bmp);
                    ImageFaceRec imageFaceRec = new ImageFaceRec(imgMat, "face/facerec_1.bmp" , "face/facerec_sample.bmp");


                    //GameObject.Find("Canvas").transform.localScale = new Vector3(0, 0, 0);

                    //GameObject quad = GameObject.Find("Quad");
                    //quad.GetComponent<Renderer>().material.mainTexture = snap;
                    //gameObject.GetComponent<Renderer>().material.mainTexture = snap;

                    Texture2D detectedFaces = imageFaceRec.recognizeFaces();
                    quad.GetComponent<Renderer>().material.mainTexture = detectedFaces;

                });

            

          

        }

        private Texture2D detectFaces(Texture2D imgTexture)
        {
            string cascade_filepath = Utils.getFilePath(HAAR_CASCADE_FILENAME);

            cascade = new CascadeClassifier(cascade_filepath);

            Mat imgMat = new Mat(imgTexture.height, imgTexture.width, CvType.CV_8UC4);

            Utils.texture2DToMat(imgTexture, imgMat);
            Debug.Log("imgMat.ToString() " + imgMat.ToString());

            if (cascade == null)
            {
                Imgproc.putText(imgMat, "model file is not loaded.", new Point(5, imgMat.rows() - 30), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
                Imgproc.putText(imgMat, "Please read console message.", new Point(5, imgMat.rows() - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);

                Texture2D _texture = new Texture2D(imgMat.cols(), imgMat.rows(), TextureFormat.RGBA32, false);
                Utils.matToTexture2D(imgMat, _texture);
                gameObject.GetComponent<Renderer>().material.mainTexture = _texture;
                return null;
            }


            Mat grayMat = new Mat();
            Imgproc.cvtColor(imgMat, grayMat, Imgproc.COLOR_RGBA2GRAY);
            Imgproc.equalizeHist(grayMat, grayMat);


            MatOfRect faces = new MatOfRect();

            if (cascade != null)
                cascade.detectMultiScale(grayMat, faces, 1.1, 2, 2,
                    new Size(20, 20), new Size());

            OpenCVForUnity.CoreModule.Rect[] rects = faces.toArray();
            for (int i = 0; i < rects.Length; i++)
            {
                Debug.Log("detect faces " + rects[i]);

                Imgproc.rectangle(imgMat, new Point(rects[i].x, rects[i].y), new Point(rects[i].x + rects[i].width, rects[i].y + rects[i].height), new Scalar(255, 0, 0, 255), 2);
            }


            Texture2D texture = new Texture2D(imgMat.cols(), imgMat.rows(), TextureFormat.RGBA32, false);

            Utils.matToTexture2D(imgMat, texture);

            return texture;
        }

        private string SaveTexture(Texture2D texture)
        {
            byte[] bytes = texture.EncodeToPNG();
            var dirPath = Application.dataPath + "/OpenCVForUnity/Examples/Resources";
            if (!System.IO.Directory.Exists(dirPath))
            {
                System.IO.Directory.CreateDirectory(dirPath);
            }
            int randomRange = Random.Range(0, 100000);
            System.IO.File.WriteAllBytes(dirPath + "/R_" + randomRange.ToString() + ".png", bytes);
            Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + dirPath);
#if UNITY_EDITOR
         UnityEditor.AssetDatabase.Refresh();
#endif
            return "R_" + randomRange.ToString();
        }
    }
}