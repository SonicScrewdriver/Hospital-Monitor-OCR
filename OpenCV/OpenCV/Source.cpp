#include "opencv2/highgui.hpp"
#include "opencv2/imgproc.hpp"
#include <iostream>
#include <stdio.h>
#include <opencv2/opencv.hpp>

using namespace cv;

struct Color32
{
	uchar red;
	uchar green;
	uchar blue;
	uchar alpha;
};

Mat _currentFrame;

extern "C" __declspec(dllexport) void ProcessImage(unsigned char* data, int width, int height)
	{
		using namespace cv;
		//Resize Mat to match the array passed to it from C#
		Mat resizedMat(height, width, _currentFrame.type());
		resize(_currentFrame, resizedMat, resizedMat.size(), INTER_CUBIC);

		//Convert from RGB to ARGB 
		Mat argb_img;
		cvtColor(resizedMat, argb_img, COLOR_RGB2BGRA);



		std::vector<Mat> bgra;
		split(argb_img, bgra);
		std::swap(bgra[0], bgra[3]);
		std::swap(bgra[1], bgra[2]);
		std::memcpy(data, argb_img.data, argb_img.total() * argb_img.elemSize());

		// create an opencv object sharing the same data space
			/// OLD STUFF BELOW
		/*
		Mat image(height, width, CV_8UC4, *rawImage);


		/// OLD STUFF BELOW

		// start with flip (in both directions) if your image looks inverted
		// flip(image, image, -1);

		// start processing the image
		// ************************************************

		Mat edges;
		Canny(image, edges, 50, 200);
		dilate(edges, edges, (5, 5));
		cvtColor(edges, edges, COLOR_GRAY2RGBA);
		normalize(edges, edges, 0, 1, NORM_MINMAX);
		multiply(image, edges, image);

		// end processing the image
		// ************************************************

		// flip again (just vertically) to get the right orientation
		flip(image, image, 0); */

	}


