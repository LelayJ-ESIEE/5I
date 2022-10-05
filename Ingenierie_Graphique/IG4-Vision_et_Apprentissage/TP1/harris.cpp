#include "opencv2/highgui.hpp"
#include "opencv2/imgproc.hpp"
#include <iostream>

using namespace cv;
using namespace std;

Mat src, gradX, gradY;

double harris(Mat& Gx, Mat& Gy, int x, int y, int w, Mat& H)
{
  // A vous de l'écrire
  return 0;  // doit en fait retourner det(H)-0.15 tr^2(H)
}


void mouse_callback(int event, int x, int y, int flags, void* unused)
{
if (event == EVENT_LBUTTONDOWN)
  {
	cout << "Vous avez cliqué sur le point (" << x << ", " << y << ")\n";

	// A vous de compléter

  }
}


void getDoGX(Mat& K, int w, double sigma)
{
  K= Mat(2*w+1,2*w+1, CV_64FC1);

  double alpha= 1/(2*M_PI*pow(sigma,4));
  double beta= -1/(2*sigma*sigma);
  
  for (int i=-w; i <= w; i++)
    for (int j=-w; j<= w; j++)
      K.at<double>(j+w,i+w)= i*alpha*exp(beta*(i*i+j*j));
}

string type2str(int type) {
  string r;

  uchar depth = type & CV_MAT_DEPTH_MASK;
  uchar chans = 1 + (type >> CV_CN_SHIFT);

  switch ( depth ) {
    case CV_8U:  r = "8U"; break;
    case CV_8S:  r = "8S"; break;
    case CV_16U: r = "16U"; break;
    case CV_16S: r = "16S"; break;
    case CV_32S: r = "32S"; break;
    case CV_32F: r = "32F"; break;
    case CV_64F: r = "64F"; break;
    default:     r = "User"; break;
  }

  r += "C";
  r += (chans+'0');

  return r;
}

int main( int argc, char** argv )
{
  Mat tmp = imread(argv[1]);
  Mat kx, gradX, ky, gradY;

  if (tmp.channels() > 1)
    {
      cvtColor( tmp, src, COLOR_BGR2GRAY );
      tmp= src;
    }
  tmp.convertTo(src, CV_8UC1 );
  // attention, autre conversion en CV_32FC1 probablement nécessaire...

  namedWindow("src");
  imshow("src", src);

  // Routine de traitement du clic souris1
  setMouseCallback("src", mouse_callback, NULL);

  // A vous de compléter le code...
  // Gradient X
  getDoGX(kx, 1, 1 );
  cv::filter2D(tmp, gradX, -1, kx);
  gradX.convertTo(src, CV_32FC1 );
  namedWindow("GradX");
  imshow("GradX", src);
  setMouseCallback("GradX", mouse_callback, NULL);

  // Gradient Y
  cv::transpose(kx, ky);
  cv::filter2D(tmp, gradY, -1, ky);
  gradY.convertTo(src, CV_32FC1 );
  namedWindow("GradY");
  imshow("GradY", src);
  setMouseCallback("GradY", mouse_callback, NULL);
  
  waitKey();
  return 0;
}


