#IN/IN7 
# TD1
Date limite de rendu : 15/01/2023, 23:59

## 2. Opérations de base

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