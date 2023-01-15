"""
Edited by Rostom Kachouri
M1-IRV_ST2IAI _ Mars 2021
"""


#Read Webcam

import cv2
  
# 0 caméra back (principale), 1 caméra front et 2 webcame externe si elle existe 
cameraCapture = cv2.VideoCapture (0)
cv2 .namedWindow ('MyWindow' )
print ('Showing camera feed. Press any key to stop.')
success, frame = cameraCapture.read()
while success and cv2.waitKey(1) == -1:
    cv2.imshow('MyWindow', frame)
    success, frame = cameraCapture.read()
    
cv2. destroyWindow ('MyWindow')

