#include "opencv2/highgui.hpp"
#include "opencv2/imgproc.hpp"
#include "opencv2/features2d.hpp"
#include "opencv2/xfeatures2d.hpp"

#include <iostream>

using namespace cv;
using namespace std;

Mat src[2];
std::vector<KeyPoint> kp[2];
Mat descriptor[2];
Mat dst[2];

int main( int argc, char** argv )
{
  Mat tmp;
  char *noms[] = { "source", "cible" };
  
  for (int i= 0; i <= 1; i++)
    {
      src[i]= imread(argv[1+i]);

      // Grey scale conversion
      if (src[i].channels() > 1)
      {
        cvtColor( src[i], tmp, COLOR_BGR2GRAY );
        src[i]= tmp;
      }
      
      // SIFT detection
      Ptr<Feature2D> ptr = cv::SIFT::create();
      ptr->detectAndCompute(src[i], Mat(), kp[i], descriptor[i]);

      // Draw Keypoints
      cv::drawKeypoints	( src[i], kp[i], dst[i], Scalar(0,170,0), DrawMatchesFlags::DRAW_RICH_KEYPOINTS );

      namedWindow(noms[i]);
      imshow(noms[i], dst[i]);
    }
  
    waitKey();
    return 0;
}

