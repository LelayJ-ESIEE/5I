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

double moyenne(char** filepaths )
{
  Mat tmp;
  
  for (int i= 0; i <= 1; i++)
  {
    src[i]= imread(filepaths[i]);

    // Grey scale conversion
    if (src[i].channels() > 1)
    {
      cvtColor( src[i], tmp, COLOR_BGR2GRAY );
      src[i]= tmp;
    } 
    
    // SIFT detection
    Ptr<Feature2D> ptr = cv::SIFT::create();
    ptr->detectAndCompute(src[i], Mat(), kpts[i], descriptor[i]);
  }

  // Compute SIFTs correspondances
  Ptr<BFMatcher> BFMatcher = cv::BFMatcher::create();  
  BFMatcher->DescriptorMatcher::match(descriptor[0], descriptor[1], matches);

  // Sort pairs and print them
  sort(matches.begin(), matches.end());
  float avg = 0.0f;

  // Compute average distance
  for(int i = 0; i < matches.size() && i<N; i++)
  {
    if(i<N)
      avg += matches[i].distance;
  }
  avg /= N;
  return avg;
}

int main( int argc, char** argv )
{
  Mat dist = Mat(argc-1, argc-1, CV_64F);
  for(int i = 0; i < argc-1; i++)
  {
    dist.at<double>(i,i) = 0;
    for(int j = i+1; j < argc-1; j++)
    {
      char* imgs[] = {argv[1+i], argv[1+j]};
      dist.at<double>(j,i) = dist.at<double>(i,j) = moyenne(imgs);
    }
  }
  cout << "Matrice des distances:" << endl << "--------------------------" << endl << dist << endl;
}

// On remarque de mauvaises classifications entre les classes at et kt : certaines classifications rapprochent plus des chats de l'Arc de Tromphe que d'autres chats.
// La distance moyenne ne semble donc pas être un critère de classification utilisable efficacement.