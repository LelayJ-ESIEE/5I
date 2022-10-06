#include "opencv2/highgui.hpp"
#include "opencv2/imgproc.hpp"
#include "opencv2/features2d.hpp"
#include "opencv2/xfeatures2d.hpp"

#include <iostream>

using namespace cv;
using namespace std;

Mat src[2];


int main( int argc, char** argv )
{
  char *noms[] = { "source", "cible" };
  
  for (int i= 0; i <= 1; i++)
    {
      src[i]= imread(argv[1+i]);
      namedWindow(noms[i]);
      imshow(noms[i], src[i]);
    }
  
    waitKey();
    return 0;
}

