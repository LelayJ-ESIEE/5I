#include "opencv2/highgui/highgui.hpp"
#include "opencv2/imgproc/imgproc.hpp"
#include "opencv2/calib3d/calib3d.hpp"
#include <iostream>
#include <stdio.h>
#include <stdlib.h>

using namespace cv;
using namespace std;


void mouse_callback(int event, int x, int y, int flags, void* unused)
{
if (event == EVENT_LBUTTONDOWN)
	cout << "Vous avez cliqué sur le point (" << x << ", " << y << ")\n";
}



int main( int, char** argv )
{
  const char *win_name = "Mon image";
  
  // Charger les images
  Mat panneau = imread("panneau.jpg", 1 );
  Mat trump = imread("trump.jpg", 1);

  
  // Les afficher
  //namedWindow( win_name, CV_WINDOW_AUTOSIZE );
  //imshow( win_name, panneau );
  vector<Point2f> psrc, pdst;

  // A compléter


  namedWindow("resultat", WINDOW_AUTOSIZE );
  imshow("resultat", panneau );

  imwrite("resultat.jpg", panneau);
  // Attendre une touche 
  waitKey(0);

  
  return(0);
}


