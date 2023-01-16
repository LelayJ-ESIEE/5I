"""
Edited by Jules LELAY
5I-IN7
January 2023

Second script of the unit, converting an RGB image to a grayscale one manually
"""
import cv2
import numpy as np

def convolute(img):
    dest = np.zeros((len(img),len(img[0]),3),np.uint8) # Création d'une image noire
    for i in range(len(dest)):
        for j in range(len(dest[0])):
            for c in range(3): # for each pixel, for each channel, apply average filter
                moy = 0
                n = 0
                for x in range(-2,3):
                    for y in range(-2,3):
                       if i+x in range(len(img)) and j+y in range(len(img[0])):
                           moy += img[i+x][j+y][c]
                           n += 1
                moy /= n
                dest[i][j][c] = moy
    return dest

def affCompareImg(src, dest):
    cv2.imshow("src",src)
    cv2.imshow("dest",dest)
    cv2.waitKey(0)
    cv2.destroyWindow("src")
    cv2.destroyWindow("dest")

def affCompareCap(src):
    while True:
        success, img = src.read()
        cv2.imshow("src", img)
        cv2.imshow("dest",convolute(img))
        if cv2.waitKey(1) == ord('q'):
            break
    cv2. destroyWindow ('src')
    cv2. destroyWindow ('dest')

def affCompareCam(cameraCapture):
    success, frame = cameraCapture.read()
    while success and cv2.waitKey(1) == -1:
        cv2.imshow('src', frame)
        cv2.imshow('dest', convolute(frame))
        success, frame = cameraCapture.read()
    cv2.destroyAllWindows()

def main():
    """
    Open files as in previous steps, then call the method to convert image or frames to grey scale
    """
    mode = input("Input desired mode among (i)mage, (v)ideo or web(c)am: ")
    if mode in ("image","i"):
        img_path = input("Input image path: ")
        img = cv2.imread(img_path)
        affCompareImg(img, convolute(img))
    if mode in ("video","v"):
        cap_path = input("Input capture path: ")
        cap = cv2.VideoCapture(cap_path)
        affCompareCap(cap)
    if mode in ("webcam","c"):
        cam_id = int(input("Input camera id (0 back cam (principal), 1 front cam and 2 external webcam): "))
        cameraCapture = cv2.VideoCapture (cam_id)
        affCompareCam(cameraCapture)

if __name__ == "__main__":
    main()
