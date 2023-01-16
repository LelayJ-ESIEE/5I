"""
Edited by Jules LELAY
5I-IN7
January 2023

Second script of the unit, converting an RGB image to a grayscale one with OpenCV cvtColor
"""
import cv2

def convertToGrey(img):
    dest = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
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
        cv2.imshow("dest",convertToGrey(img))
        if cv2.waitKey(1) == ord('q'):
            break
    cv2. destroyWindow ('src')
    cv2. destroyWindow ('dest')

def affCompareCam(cameraCapture):
    success, frame = cameraCapture.read()
    while success and cv2.waitKey(1) == -1:
        cv2.imshow('src', frame)
        cv2.imshow('dest', convertToGrey(frame))
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
        affCompareImg(img, convertToGrey(img))
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
