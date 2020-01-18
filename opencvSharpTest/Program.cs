using System;
using OpenCvSharp;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;


namespace opencvSharpTest
{

    class Program
    {
        static void Main(string[] args)
        {
            //CapatureCamera();
            CalibrateCamera();
        }

        static void CapatureCamera()
        {

            Mat src = new Mat();
            Mat show = new Mat();
            FrameSource frame = Cv2.CreateFrameSource_Camera(0);

            int width = 0;
            int height = 0;
            Window window = new Window("Camera");
            //int index = 0;
            //string path = System.IO.Path.GetDirectoryName(Environment.CurrentDirectory);
            //Console.WriteLine(path);

            //Mat chessBoardImage = new Mat(path + "\\calib-checkerboard.png");
            //window.ShowImage(chessBoardImage);
            //window.SetProperty(WindowProperty.Fullscreen,1);

            while (true)
            {
                frame.NextFrame(src);
                if (src.Empty()) continue;

                //if (index < 20)
                //{
                //    string savePath = path + "\\tex\\texture_" + index.ToString() + ".jpg";
                //    src.SaveImage(savePath);
                //    Console.WriteLine("image saved! " + savePath);
                //    index++;
                //}
                //window.ShowImage(src);
                Cv2.WaitKey(30);
            }

        }

        //public void FishEyeCalibrate()
        //{
        //    var patternSize = new Size(10, 7);

        //    using (var image = Image("calibration/00.jpg"))
        //    using (var corners = new Mat<Point2f>())
        //    {
        //        Cv2.FindChessboardCorners(image, patternSize, corners);

        //        var objectPointsArray = Create3DChessboardCorners(patternSize, 1.0f).ToArray();
        //        var imagePointsArray = corners.ToArray();

        //        using (var objectPoints = Mat<Point3f>.FromArray(objectPointsArray))
        //        using (var imagePoints = Mat<Point2f>.FromArray(imagePointsArray))
        //        using (var cameraMatrix = new Mat<double>(Mat.Eye(3, 3, MatType.CV_64FC1)))
        //        using (var distCoeffs = new Mat<double>())
        //        {
        //            var rms = Cv2.FishEye.Calibrate(new[] { objectPoints }, new[] { imagePoints }, image.Size(), cameraMatrix,
        //                distCoeffs, out var rotationVectors, out var translationVectors);

        //            var distCoeffValues = distCoeffs.ToArray();
        //            Assert.Equal(55.15, rms, 2);
        //            Assert.Contains(distCoeffValues, d => Math.Abs(d) > 1e-20);
        //            Assert.NotEmpty(rotationVectors);
        //            Assert.NotEmpty(translationVectors);
        //        }
        //    }
        //}

        private static IEnumerable<Point3f> Create3DChessboardCorners(Size boardSize, float squareSize)
        {
            for (int y = 0; y < boardSize.Height; y++)
            {
                for (int x = 0; x < boardSize.Width; x++)
                {
                    yield return new Point3f(x * squareSize, y * squareSize, 0);
                }
            }
        }

        static void CalibrateCamera()
        {
            Mat src = new Mat();
            string path = System.IO.Path.GetDirectoryName(Environment.CurrentDirectory);
            src.Add(new Mat(path + "\\tex\\texture_2.jpg", ImreadModes.Grayscale));
            Size imageSize = src.Size();
            Size patternSize = new Size(9, 6);
            //Size regionSize = new Size(4, 4);
            Size rectangleSize = new Size(30f,30f);

            Mat imagePoints = new  Mat();
            Mat objectPoints = new Mat();

            var objectPoint = new List<Point3f>();
            for (int i = 0; i < patternSize.Height; i++)
            {
                for (int j = 0; j < patternSize.Width; j++)
                {
                    objectPoint.Add(new Point3f(j * rectangleSize.Width,
                        i * rectangleSize.Height, 0.0F));
                }
            }

            Mat corners = new Mat();
            bool isFoundChessboard = Cv2.FindChessboardCorners(src, patternSize, corners);
              
            //Cv2.DrawChessboardCorners(src, patternSize, corners, isFoundChessboard);
            //Window.ShowImages(src);

            //bool isCornerSubpix = Cv2.Find4QuadCornerSubpix(currentMat, corners, regionSize);
            //if (!isCornerSubpix)
            //{
            //    continue;
            //}

            Mat cameraMatrix =  Mat.Eye(3,3, MatType.CV_64FC1); // output from CalibrateCamera
            using var distCoeffs = new Mat(4, 1, MatType.CV_64FC1);
            distCoeffs.Set<double>(0, 0);
            distCoeffs.Set<double>(1, 0);
            distCoeffs.Set<double>(2, 0);
            distCoeffs.Set<double>(3, 0);// output from CalibrateCamera

            imagePoints.Add(corners);
            //var objectPointMat = Create3DChessboardCorners(patternSize, 1.0f);
            MatOfPoint3f objectPointMat = new MatOfPoint3f(patternSize.Width * patternSize.Height, 1,
                objectPoint.ToArray());
            objectPoints.Add(objectPointMat);

            var result = Cv2.CalibrateCamera(new[] { objectPointMat }, new[] { corners }, src.Size(), cameraMatrix,
                distCoeffs, out var rotationVectors, out var translationVectors);
            //var rms = Cv2.FishEye.Calibrate(new[] { objectPointMat }, new[] { corners }, src.Size(), cameraMatrix,
            //    distCoeffs, out var rotationVectors, out var translationVectors);


            //var distCoeffValues = distCoeffs.ToArray();


            //Cv2.FishEye.Calibrate(objectPoints, imagePoints, imageSize, cameraMatrix, distCoeffs, out var rvecs, out var tvecs, 0);

            Mat rMatrix = new Mat();
            Mat newCameraMatrix = new Mat();
            Mat out1 = new Mat();
            Mat out2 = new Mat();
            Mat undis = new Mat();
            Console.WriteLine(distCoeffs.Total());
            Cv2.FishEye.UndistortImage(src, undis, cameraMatrix, distCoeffs);
            //Cv2.FishEye.InitUndistortRectifyMap(cameraMatrix, distCoeffs, rMatrix, newCameraMatrix, imageSize, MatType.CV_32F, out1, out2);
            //Cv2.Remap(src, undis, out1, out2);
            undis.SaveImage(path + "\\undisTex\\image1.jpg");
            Console.WriteLine("Hello World!");
        }
    }
}
