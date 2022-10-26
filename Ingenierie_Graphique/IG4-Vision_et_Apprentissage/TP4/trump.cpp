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
  // namedWindow( win_name, WINDOW_AUTOSIZE );
  // imshow( win_name, panneau );
  // setMouseCallback("panneau", mouse_callback, NULL);
  vector<Point2f> psrc, pdst;

  /* On préférera la fonction findHomography() car nous avons 4 points à projeter, getAffineTransform() étant limité à 3 paires de points pour le calcul */

  // A compléter
  psrc.push_back(Point2f(0, 0));
  psrc.push_back(Point2f(trump.cols, 0));
  psrc.push_back(Point2f(trump.cols, trump.rows));
  psrc.push_back(Point2f(0, trump.rows));

  pdst.push_back(Point2f(498, 96));
  pdst.push_back(Point2f(727, 75));
  pdst.push_back(Point2f(763, 464));
  pdst.push_back(Point2f(466, 486));

  Mat M = findHomography(psrc, pdst);
  Mat resultat;
  warpPerspective(trump, resultat, M, Size(panneau.cols, panneau.rows));

  for(int j = 0; j < resultat.rows; j++){
    for(int i = 0; i < resultat.cols; i++){
      if(resultat.at<Vec3b>(j,i) == Vec3b(0,0,0)){
        resultat.at<Vec3b>(j,i) = panneau.at<Vec3b>(j,i);
      }
    }
  }

  namedWindow("resultat", WINDOW_AUTOSIZE );
  imshow("resultat", resultat );

  imwrite("resultat.jpg", resultat);
  // Attendre une touche 
  waitKey(0);

  
  return(0);
}


