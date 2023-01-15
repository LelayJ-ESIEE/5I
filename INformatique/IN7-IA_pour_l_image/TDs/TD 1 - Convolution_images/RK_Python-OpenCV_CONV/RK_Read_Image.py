"""
Edited by Rostom Kachouri
M1-IRV_ST2IAI _ Mars 2021
"""


#Read Image

import cv2
# LOAD AN IMAGE USING 'IMREAD'
img = cv2.imread("RK_Data/lena.jpg")
# DISPLAY
cv2.imshow("Lena Soderberg",img)
cv2.waitKey(0)

cv2. destroyWindow ('Lena Soderberg')
