#include "opencv2/highgui.hpp"
#include "opencv2/imgproc.hpp"
#include "opencv2/features2d.hpp"
#include "opencv2/xfeatures2d.hpp"

#include <iostream>

using namespace cv;
using namespace std;

#define N 20

Mat src[2];
std::vector<KeyPoint> kpts[2];
Mat descriptor[2];
Mat dst[2];
std::vector<DMatch> matches;
Mat img_matches;

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
    ptr->detectAndCompute(src[i], Mat(), kpts[i], descriptor[i]);

    // Draw Keypoints
    drawKeypoints( src[i], kpts[i], dst[i], Scalar(0,192,0), DrawMatchesFlags::DRAW_RICH_KEYPOINTS );

    // namedWindow(noms[i]);
    // imshow(noms[i], src[i]);
    // imshow(noms[i], dst[i]);
  }

  // Compute SIFTs correspondances
  Ptr<BFMatcher> BFMatcher = cv::BFMatcher::create();  
  BFMatcher->DescriptorMatcher::match(descriptor[0], descriptor[1], matches);
  // Sort pairs and print them
  sort(matches.begin(), matches.end());
  float avg = 0.0f;
  for(int i = 0; i < matches.size(); i++)
  {
    cout << "KP " << matches[i].queryIdx << " <-> " << matches[i].trainIdx << ", distance=" << matches[i].distance << "\n";
    if(i<N)
      avg += matches[i].distance;
  }
  avg /= N;
  // Show the two images with the N best correspondances
  vector<char> mask(matches.size(), 0);
  for(int i; i < N; i++)
    mask[i] = 1;
  namedWindow("figure");
  drawMatches(src[0], kpts[0], src[1], kpts[1], matches, img_matches, Scalar(0,192,0), Scalar(0,192,0), mask, DrawMatchesFlags::DRAW_RICH_KEYPOINTS);
  imshow("figure", img_matches);
  // Print statistics
  cout << "STATISTIQUES: moyenne=" << avg << ", min=" << matches[0].distance << ", max=" << matches[N-1].distance << "\n";

  /*
  Pour at2 et at3 :
    STATISTIQUES: moyenne=58.4352, min=50.2096, max=65.7647
    Les paires sont toutes géométriquement cohérentes, mais on se retrouve avec un problèmes en cas de similarité géométrique des points-clés

  Avec at2 et at2_mini : bonne résistance au changement d'échelle mais disparition de certaines paires suite à la disparition de certains détails

  SIFT n'étant pas invariant à la rotation, on perd certains points entre at2 et Rotated/at2

  Sur changement de point de vue, les scores baissent de façon perceptible.
  */

  waitKey();
  return 0;
}

