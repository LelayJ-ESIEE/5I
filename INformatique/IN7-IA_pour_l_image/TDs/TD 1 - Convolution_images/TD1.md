#IN/IN7 
# TD1
Date limite de rendu : 15/01/2023, 23:59

## 2. Opérations de base
Programme *colors* disponible dans le dossier *src*
### 1. Création et manipulation d’images avec Python et OpenCV
```Python
import cv2
import numpy as np

img = np.zeros((128,128,3),np.uint8)
print(img) # pour afficher dans la console

cv2.imshow("Image",img)
cv2.waitKey(0)
cv2. destroyWindow ("Image")
```
### 2. Rouge, Vert et Bleu
```Python
import cv2
import numpy as np

colors = ["Blue", "Green", "Red"]
for c in range(-1,-4,-1):
    img = np.zeros((512,512,3),np.uint8)
    for i in range(512):
        for j in range(512):
            img[i][j][c] = 255
    print(img) # pour afficher dans la console
    cv2.imshow(colors[c],img)
    cv2.waitKey(0)
    cv2.destroyWindow(colors[c])
```
### 3. Lignes, rectangles, cercles et texte
```Python
"""
First script of the unit, showing 3 512*512 images for the 3 primary colors with a black frame, a
centered white circle and the color name underlined.
"""

import cv2
import numpy as np

BLACK = (0,0,0)
WHITE = (255,255,255)
COLORS = ["Blue", "Green", "Red"]

for c in range(-1,-4,-1):
    img = np.zeros((512,512,3),np.uint8)
	
    for i in range(512):
        for j in range(512):
            img[i][j][c] = 224
	
    cv2.line(img, (256-10*len(COLORS[c]),258), (256+10*len(COLORS[c]),258), WHITE, 2, cv2.LINE_4)
    cv2.rectangle(img, (1,1), (510,510), BLACK, 3, cv2.LINE_4)
    cv2.circle(img, (255,255), 255, WHITE,3)
    cv2.putText(img, COLORS[c], (256-10*len(COLORS[c]),255), cv2.FONT_HERSHEY_TRIPLEX, 1, WHITE)
	
    print(img) # pour afficher dans la console
	
    cv2.imshow(COLORS[c],img)
    cv2.waitKey(0)
    cv2.destroyWindow(COLORS[c])

```
Intensité de couleur réduite à 224 par soucis de lisibilité et de confort de visualisation
### 4. Commentaires
#### Code commenté
```Python
"""
First script of the unit, showing 3 512*512 images for the 3 primary colors with a black frame, a
centered white circle and the color name underlined.
"""

import cv2
import numpy as np

BLACK = (0,0,0)
WHITE = (255,255,255)
COLORS = ["Blue", "Green", "Red"]

for c in range(-1,-4,-1): # Parcours inverse pour respecter l'ordre RGB
    img = np.zeros((512,512,3),np.uint8) # Création d'une image noire

    # Parcours de l'image et colorisation par mise à 224 de la valeur de couleur appropriée
    for i in range(512):
        for j in range(512):
            img[i][j][c] = 224

    # Soulignement du texte. img est l'image en cours d'écriture, les positions de départ et
    # d'arrivée à 256 ± (1/2 caractère) * longueur du texte, la couleur blanche définie plus haut
    # une épaisseur de 2 et de type 4-connectée
    cv2.line(img, (256-10*len(COLORS[c]),258), (256+10*len(COLORS[c]),258), WHITE, 2, cv2.LINE_4)

    # Encadrement de l'image. img est l'image en cours d'écriture, les positions de départ et
    # d'arrivée dans les coins supérieur gauche et inférieur droit, de couleur noire définie plus
    # haut et des lignes de largeur 3 et de type 4-connectée
    cv2.rectangle(img, (1,1), (510,510), BLACK, 3, cv2.LINE_4)

    # Encerclement du texte. img est l'image en cours d'écriture, le cercle est centré, de rayon
    # 1/2 côté de l'image, de la couleur blanche définie plus haut et d'épaisseur 3
    cv2.circle(img, (255,255), 255, WHITE,3)

    # Écriture du texte. img est l'image en cours d'écriture, COLORS[c] le nom de la couleur en
    # arrière-plan, le départ du texte placé à 256 ± (1/2 caractère) * longueur du texte, d'une
    # police d'écriture avec serif complexe, de taille normale et  de la couleur blanche définie
    # plus haut
    cv2.putText(img, COLORS[c], (256-10*len(COLORS[c]),255), cv2.FONT_HERSHEY_TRIPLEX, 1, WHITE)

    print(img) # pour afficher dans la console

    # Affichage de l'image écrite, attente d'une entrée clavier et destruction de la fenêtre
    cv2.imshow(COLORS[c],img)
    cv2.waitKey(0)
    cv2.destroyWindow(COLORS[c])

```
#### Exemple d'exécution
![[Red.png]]
![[Green.png]]
![[Blue.png]]
## 3. Traitement d'images
### Mise en œuvre du TD
#### 1. Téléchargement de l'archive
#### 2. Décompression de l'archive
#### 3. Test et modification
#### 4. Commentaires
##### RK_Read_Image.py
###### Code commenté
```Python
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
cv2.imshow("Lena Soderberg",img)
cv2.waitKey(0)

cv2. destroyWindow ('Lena Soderberg')

```
###### Exemple d'exécution
![[TD_Image.png]]
##### RK_Read_Video.py
###### Code commenté
```Python
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

```
###### Exemple d'exécution
![[TD_Video.png]]
##### RK_Read_Webcam.py
###### Code commenté
```Python
"""
Edited by Rostom Kachouri
M1-IRV_ST2IAI _ Mars 2021
"""

#Read Webcam

import cv2

# 0 caméra back (principale), 1 caméra front et 2 webcame externe si elle existe
# Take capture path in input
cam_id = int(input("Input camera id (0 back cam (principal), 1 front cam and 2 external webcam): "))
cameraCapture = cv2.VideoCapture (cam_id)
cv2 .namedWindow ('MyWindow' )
print ('Showing camera feed. Press any key to stop.')
success, frame = cameraCapture.read()
while success and cv2.waitKey(1) == -1:
    cv2.imshow('MyWindow', frame)
    success, frame = cameraCapture.read()

cv2. destroyWindow ('MyWindow')

```
###### Exemple d'exécution
![[TD_Webcam.png]]
### Conversion couleur
#### 1. Conversion manuelle
#### 2. Commentaires
##### Code commenté
```Python
"""
Edited by Jules LELAY
5I-IN7
January 2023

Second script of the unit, converting an RGB image to a grayscale one manually
"""
import cv2
import numpy as np

def convertToGrey(img):
    dest = np.zeros((len(img),len(img[0]),1),np.uint8) # Création d'une image noire
    # Copy grey converted pixels
    for i in range(len(dest)):
        for j in range(len(dest[0])):
            dest[i][j] = img[i][j][0] * 0.2989 + img[i][j][1] * 0.5870 + img[i][j][2] * 0.1140
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

```
##### Exemple d'exécution
![[Gray_manual.png]]
#### 3. Conversion par OpenCV
#### 4. Commentaires
##### Code commenté
```Python
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

```
##### Exemple d'exécution
![[Gray_auto.png]]
Les rendus sont similaires, mais la méthode manuelle est plus sombre.
### Convolution d'images
#### 5. Convolution manuelle
#### 6. Commentaires
##### Code commenté
```Python
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

```
##### Exemple d'exécution
![[Convolution_manual.png]]
#### 7. Convolution par OpenCV
#### 8. Commentaires
##### Code commenté
```Python
"""
Edited by Jules LELAY
5I-IN7
January 2023

Second script of the unit, converting an RGB image to a grayscale one manually
"""
import cv2

def convolute(img):
    dest = cv2.blur(img, (5,5))
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

```
##### Exemple d'exécution
![[Convolution_auto.png]]
##### Remarques
Que la méthode soit manuelle ou à l'aide des librairies OpenCV, le résultat semble en tout point similaire.