"""
Edited by Rostom Kachouri
M1-IRV_ST2IAI _ Mars 2021
"""

#Read Image

import cv2
# Take image path in input
img_path = input("Input image path: ")
# LOAD AN IMAGE USING 'IMREAD'
img = cv2.imread(img_path)
# DISPLAY
cv2.imshow("Image",img)
cv2.waitKey(0)

cv2. destroyWindow ('Lena Soderberg')
