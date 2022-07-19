using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.UnityUtils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rect = OpenCVForUnity.CoreModule.Rect;



namespace OpenCVForUnityExample
{
    public class ImageFaceRec : MonoBehaviour
    {
        float scoreThreshold = 0.9f;

        /// <summary>
        /// Suppress bounding boxes of iou >= nms_threshold
        /// </summary>
        float nmsThreshold = 0.3f;

        /// <summary>
        /// Keep top_k bounding boxes before NMS.
        /// </summary>
        int topK = 5000;

        /// <summary>
        /// FD_MODEL_FILENAME
        /// </summary>
        protected static readonly string FD_MODEL_FILENAME = "objdetect/face_detection_yunet_2021dec.onnx";

        /// <summary>
        /// The fd model filepath.
        /// </summary>
        string fd_model_filepath;

        /// <summary>
        /// The cosine similar thresh.
        /// </summary>
        double cosine_similar_thresh = 0.363;

        /// <summary>
        /// The l2norm similar thresh.
        /// </summary>
        double l2norm_similar_thresh = 1.128;

        /// <summary>
        /// SF_MODEL_FILENAME
        /// </summary>
        protected static readonly string SF_MODEL_FILENAME = "objdetect/face_recognition_sface_2021dec.onnx";

        /// <summary>
        /// The sf model filepath.
        /// </summary>
        string sf_model_filepath;

        /// <summary>
        /// IMAGE_0_FILENAME
        /// </summary>
        protected string IMAGE_0_FILENAME;

        /// <summary>
        /// The image 0 filepath.
        /// </summary>
        string image_0_filepath;

        /// <summary>
        /// IMAGE_1_FILENAME
        /// </summary>
        protected string IMAGE_1_FILENAME;

        /// <summary>
        /// The image 1 filepath.
        /// </summary>
        string image_1_filepath;

        /// <summary>
        /// SAMPLE_IMAGE_FILENAME
        /// </summary>
        protected string SAMPLE_IMAGE_FILENAME;

        /// <summary>
        /// The sample image filepath.
        /// </summary>
        string sample_image_filepath;

        Mat testSampleMat;

        Mat image0Mat;

        Mat image1Mat;

        public ImageFaceRec(Mat img0, string img1, string sampleImg)
        {
            //IMAGE_0_FILENAME = img0;
            IMAGE_1_FILENAME = img1;
            SAMPLE_IMAGE_FILENAME = sampleImg;
            fd_model_filepath = Utils.getFilePath(FD_MODEL_FILENAME);
            sf_model_filepath = Utils.getFilePath(SF_MODEL_FILENAME);
            //image_0_filepath = Utils.getFilePath(IMAGE_0_FILENAME);
            image_1_filepath = Utils.getFilePath(IMAGE_1_FILENAME);
            sample_image_filepath = Utils.getFilePath(SAMPLE_IMAGE_FILENAME);

            testSampleMat = Imgcodecs.imread(sample_image_filepath, Imgcodecs.IMREAD_COLOR);
            //image0Mat = new Mat(img0.height, img0.width, CvType.CV_8UC4);
            //Utils.texture2DToMat(img0, image0Mat);
            image0Mat = img0;
            image1Mat = Imgcodecs.imread(image_1_filepath, Imgcodecs.IMREAD_COLOR);
            image0Mat = Imgcodecs.imread(image_0_filepath, Imgcodecs.IMREAD_COLOR);
        }

        public Texture2D recognizeFaces()
        {
            //if true, The error log of the Native side OpenCV will be displayed on the Unity Editor Console.
            Utils.setDebugMode(true);


            if (string.IsNullOrEmpty(image_0_filepath) || string.IsNullOrEmpty(image_1_filepath) || string.IsNullOrEmpty(sample_image_filepath))
            {
                Debug.LogError(IMAGE_0_FILENAME + " or " + IMAGE_1_FILENAME + " or " + SAMPLE_IMAGE_FILENAME + " is not loaded. Please move from “OpenCVForUnity/StreamingAssets/” to “Assets/StreamingAssets/” folder.");
            }

            //Mat testSampleMat = Imgcodecs.imread(sample_image_filepath, Imgcodecs.IMREAD_COLOR);
            //Mat image0Mat = Imgcodecs.imread(image_0_filepath, Imgcodecs.IMREAD_COLOR);
            //Mat image1Mat = Imgcodecs.imread(image_1_filepath, Imgcodecs.IMREAD_COLOR);

            int imageSizeW = 112;
            int imageSizeH = 112;
            Mat resultMat = new Mat(imageSizeH * 2, imageSizeW * 2, CvType.CV_8UC3, new Scalar(127, 127, 127, 255));


            if (string.IsNullOrEmpty(fd_model_filepath) || string.IsNullOrEmpty(sf_model_filepath))
            {
                Debug.LogError(FD_MODEL_FILENAME + " or " + SF_MODEL_FILENAME + " is not loaded. Please read “StreamingAssets/objdetect/setup_objdetect_module.pdf” to make the necessary setup.");

                Imgproc.putText(resultMat, "model file is not loaded.", new Point(5, resultMat.rows() - 30), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255), 2, Imgproc.LINE_AA, false);
                Imgproc.putText(resultMat, "Please read console message.", new Point(5, resultMat.rows() - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255), 2, Imgproc.LINE_AA, false);
            }
            else
            {

                FaceDetectorYN faceDetector = FaceDetectorYN.create(fd_model_filepath, "", new Size(imageSizeW, imageSizeH), scoreThreshold, nmsThreshold, topK);
                FaceRecognizerSF faceRecognizer = FaceRecognizerSF.create(sf_model_filepath, "");


                // Detect faces.
                faceDetector.setInputSize(testSampleMat.size());
                Mat faces_sample = new Mat();
                faceDetector.detect(testSampleMat, faces_sample);

                if (faces_sample.rows() < 1)
                {
                    Debug.Log("Cannot find a face in " + SAMPLE_IMAGE_FILENAME + ".");
                }

                faceDetector.setInputSize(image0Mat.size());
                Mat faces_0 = new Mat();
                faceDetector.detect(image0Mat, faces_0);

                if (faces_0.rows() < 1)
                {
                    Debug.Log("Cannot find a face in " + IMAGE_0_FILENAME + ".");
                }

                faceDetector.setInputSize(image1Mat.size());
                Mat faces_1 = new Mat();
                faceDetector.detect(image1Mat, faces_1);

                if (faces_1.rows() < 1)
                {
                    Debug.Log("Cannot find a face in " + IMAGE_1_FILENAME + ".");
                }


                /*
                // Draw results on the input image.
                DrawDetection(faces_sample.row(0),testSampleMat);
                DrawDetection(faces_0.row(0), image0Mat);
                DrawDetection(faces_1.row(0), image1Mat);
                */


                // Aligning and cropping facial image through the first face of faces detected.
                
                Mat aligned_face_sample = new Mat();
                faceRecognizer.alignCrop(testSampleMat, faces_sample.row(0), aligned_face_sample);
                Mat aligned_face_0 = new Mat();
                faceRecognizer.alignCrop(testSampleMat, faces_0.row(0), aligned_face_0);  //Problem line
//                faceRecognizer.alignCrop(image0Mat, faces_0.row(0), aligned_face_0);  //Problem line

                Mat aligned_face_1 = new Mat();
                faceRecognizer.alignCrop(image1Mat, faces_1.row(0), aligned_face_1);
                

                // Run feature extraction with given aligned_face.
                Mat feature_sample = new Mat();
                faceRecognizer.feature(aligned_face_sample, feature_sample);
                feature_sample = feature_sample.clone();
                Mat feature_0 = new Mat();
                faceRecognizer.feature(aligned_face_0, feature_0);
                feature_0 = feature_0.clone();
                Mat feature_1 = new Mat();
                faceRecognizer.feature(aligned_face_1, feature_1);
                feature_1 = feature_1.clone();


                // Match the face features.
                bool sample_0_match = Match(faceRecognizer, feature_sample, feature_0);
                bool sample_1_match = Match(faceRecognizer, feature_sample, feature_1);

                Debug.Log("Match(" + SAMPLE_IMAGE_FILENAME + ", " + IMAGE_0_FILENAME + "): " + sample_0_match);
                Debug.Log("Match(" + SAMPLE_IMAGE_FILENAME + ", " + IMAGE_1_FILENAME + "): " + sample_1_match);


                // Draw the recognition result.
                aligned_face_sample.copyTo(resultMat.submat(new Rect(new Point(imageSizeW / 2, 0), aligned_face_sample.size())));
                aligned_face_0.copyTo(resultMat.submat(new Rect(new Point(0, imageSizeH), aligned_face_0.size())));
                aligned_face_1.copyTo(resultMat.submat(new Rect(new Point(imageSizeW, imageSizeH), aligned_face_1.size())));

                Imgproc.putText(resultMat, "TestSample", new Point(imageSizeW / 2 + 5, 15), Imgproc.FONT_HERSHEY_SIMPLEX, 0.4, new Scalar(255, 255, 255, 255), 1, Imgproc.LINE_AA, false);
                Imgproc.putText(resultMat, "Image0", new Point(5, imageSizeH + 15), Imgproc.FONT_HERSHEY_SIMPLEX, 0.4, new Scalar(255, 255, 255, 255), 1, Imgproc.LINE_AA, false);
                Imgproc.putText(resultMat, "Image1", new Point(imageSizeW + 5, imageSizeH + 15), Imgproc.FONT_HERSHEY_SIMPLEX, 0.4, new Scalar(255, 255, 255, 255), 1, Imgproc.LINE_AA, false);

                if (sample_0_match)
                    Imgproc.rectangle(resultMat, new Rect(0, imageSizeH, imageSizeW, imageSizeH), new Scalar(255, 0, 0, 255), 2);
                if (sample_1_match)
                    Imgproc.rectangle(resultMat, new Rect(imageSizeW, imageSizeH, imageSizeW, imageSizeH), new Scalar(255, 0, 0, 255), 2);
            }

            Texture2D texture = new Texture2D(resultMat.cols(), resultMat.rows(), TextureFormat.RGB24, false);

            Utils.matToTexture2D(resultMat, texture);
            
            return texture; 
        }

        private void DrawDetection(Mat d, Mat frame)
        {
            float[] buf = new float[15];
            d.get(0, 0, buf);

            Scalar color = new Scalar(255, 255, 0, 255);

            Imgproc.rectangle(frame, new Point(buf[0], buf[1]), new Point(buf[0] + buf[2], buf[1] + buf[3]), color, 2);
            Imgproc.circle(frame, new Point(buf[4], buf[5]), 2, color, 2);
            Imgproc.circle(frame, new Point(buf[6], buf[7]), 2, color, 2);
            Imgproc.circle(frame, new Point(buf[8], buf[9]), 2, color, 2);
            Imgproc.circle(frame, new Point(buf[10], buf[11]), 2, color, 2);
            Imgproc.circle(frame, new Point(buf[12], buf[13]), 2, color, 2);
        }

        private bool Match(FaceRecognizerSF faceRecognizer, Mat feature1, Mat feature2)
        {
            double cos_score = faceRecognizer.match(feature1, feature2, FaceRecognizerSF.FR_COSINE);
            bool cos_match = (cos_score >= cosine_similar_thresh);

            Debug.Log((cos_match ? "They have the same identity;" : "They have different identities;") + "\n"
                + " Cosine Similarity: " + cos_score + ", threshold: " + cosine_similar_thresh + ". (higher value means higher similarity, max 1.0)");


            double L2_score = faceRecognizer.match(feature1, feature2, FaceRecognizerSF.FR_NORM_L2);
            bool L2_match = (L2_score <= l2norm_similar_thresh);

            Debug.Log((L2_match ? "They have the same identity;" : "They have different identities;") + "\n"
                + " NormL2 Distance: " + L2_score + ", threshold: " + l2norm_similar_thresh + ". (lower value means higher similarity, min 0.0)");

            return cos_match && L2_match;
        }
    }
}