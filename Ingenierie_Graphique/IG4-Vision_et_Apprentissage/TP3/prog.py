## 1.3 Base d’images
# ???

### 2.1.1 Fonction vocabulaire
def vocabulaire(N,chemins,fichier,methode):
    """
    Etant donne un entier N, une liste des chemins d’acces a des repertoires, et eventuellement un parametre pour la methode a utiliser :
    — Charge chaque image de ces repertoires, et en extrait des SIFT. Aide : glob.glob, s=cv2.xfeatures2d.SIFT_create(), s.detectAndCompute()
    — Clusterise l’ensemble des SIFT trouves en N clusters, en tenant compte de methode ou non. Aide : sklearn.cluster.*
    — Sauvegarde sur disque les centres de clusters SIFT trouves sous forme d’une matrice, que nous appellerons matrice vocabulaire, si fichier est precise. Aide : savetxt().
    — Retourne l’inertie moyenne, et la plus grande erreur au sens de la norme L2 induites par cette clusterisation.
    """
    pass

### 2.1.2 Recherche de N
def coude():
    """
    Etant donné une liste de chemins d’accès :
    — Appelle la fonction vocabulaire() précédente sur vos quatre répertoires d’apprentissage, en faisant varier N
    — Représente graphiquement l’inertie moyenne et la plus grande erreur qui résulte de la clusterisation en fonction de N.
    — Sauvegarde ces graphiques sur disque
    """
    pass

# 1. ???
# 2. ???
# 3. ???
# 4. ???

# 3 Vectorisation

## 3.1 D’une seule image
def vectoriser(im, voca):
    """
    étant donné un fichier image fourni en paramètre et la matrice vocabulaire :
    — Extrait des SIFT de cette image
    — Vectorise le résultat, en produisant un vecteur dont chaque composante i représente le nombre de fois où le i-ème mot du vocabulaire s’est trouvé plus proche d’un descripteur extrait de l’image que n’importe quel autre mot, comme expliqué en cours
    — Renvoie le vecteur obtenu
    """
    pass

## 3.2 De toute la base d’apprentissage
def test_vecto():
    """
    appelle la fonction vectoriser() sur l’ensemble des images de vos répertoires d’apprentissage. Vous obtiendrez en résultat une matrice de vecteurs, et une liste (ou un vecteur, au choix) de noms de fichiers, que vous sauvegarderez sur disque sous le nom base.pickle. Aide : module pickle de Python
    """
    pass

# 4 Apprentissage et tests

## 4.1 Avec 2 classes
# Lignes utilisées : ???

### 4.1.1 Effet de la Kernel-LDA
# ???

### 4.1.2 Séparation par SVM
# ???