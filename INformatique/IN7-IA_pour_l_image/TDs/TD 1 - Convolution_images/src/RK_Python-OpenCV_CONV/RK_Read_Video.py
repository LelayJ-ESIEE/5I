"""
Edited by Rostom Kachouri
M1-IRV_ST2IAI _ Mars 2021
"""

#Read Video

import cv2
#frameWidth = 640
#frameHeight = 480
# Take capture path in input
cap_path = input("Input capture path: ")
cap = cv2.VideoCapture(cap_path)
while True:
    success, img = cap.read()
    #img = cv2.resize(img, (frameWidth, frameHeight))
    cv2.imshow("Video", img)
    #cv2.waitKey(1)
    if cv2.waitKey(1) == ord('q'):
        break

cv2. destroyWindow ('Video')
