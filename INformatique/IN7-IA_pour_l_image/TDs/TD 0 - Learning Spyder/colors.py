import cv2
import numpy as np

colors = ["Blue", "Green", "Red"]
for c in range(-1,-3,-1):
    img = np.zeros((512,512,3),np.uint8)
    for i in range(512):
        for j in range(512):
            img[i][j][c] = 255
    print(img) # pour afficher dans la console
    cv2.imshow(colors[c],img)
    cv2.waitKey(0)
    cv2.destroyWindow(colors[c])
