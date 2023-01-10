import cv2
import numpy as np
img = np.zeros((128,128,3),np.uint8)
print(img) # pour afficher dans la console
cv2.imshow("Image",img)
cv2.waitKey(0)
cv2. destroyWindow ("Image")
