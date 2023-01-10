import cv2
import numpy as np

colors = ["Red", "Green", "Blue"]
for c in range(3):
    img = np.zeros((512,512,3),np.uint8)
    for i in range(512):
        for j in range(512):
            img[i][j][c] = 255
    cv2.imshow(colors[c],img)
    cv2.waitKey(0)

for i in range(3):
    cv2.destroyWindow(colors[i])
