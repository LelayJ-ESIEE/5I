#IN/IN1
# Cours 1
04/01/2023, 13:00–17:00

## Introduction
René NATOWICZ, rene.natowicz@esiee.fr, Bureau 5452
Mails en "E5FI : ..."
Possibilité de faire du Prolog

## Notations
Intervalles Python : \[i,j\] = {i, i+1, ... j-1}
___
## Programmation dynamique
"Supposons le problème résolu. Quelle fut la dernière étape ?"
>Toute sous-solution d'une solution optimale d'un problème d'optimisation est elle-même optimale

## Le premier problème
### Énoncé
Problème du sac à dos.
Sac de contenance C. Objets \[0,n] de taille t<sub>i</sub> et de valeur v<sub>i</sub>. Maximiser la valeur du contenu du sac.

Soit : Calculer m(n,C) sous ensemble de \[0:n] de valeur max

### Solution 1 : algorithme glouton
Test de toutes les possibilités : complexité mémoire de 2<sup>n</sup>

### Solution 2
"Supposons le problème résolu. Quelle fut la dernière étape ?"

2 possibilités :
- l'objet n-1 n'est pas dans le sac : m(n,C) = m(n-1,C)
- l'objet n-1 est dans le sac : m(n,C) = m(n-1,C-t<sub>n-1</sub>)+v<sub>n-1</sub>

D'où m(n,C) = max(m(n-1, C), m(n-1, C-t<sub>n-1</sub>)+v<sub>n-1</sub>)

#### Généralisation
m(k, c) = valeur max d'un sac c contenant un sous ensemble de [0:k] ; où 0≤k≤n et 0≤c≤C

m(k, c) = (m(k-1, c), m(k-1, c-t<sub>k-1</sub>)+v<sub>k-1</sub>)

Cas de base : m(0, c) = 0 pour tout c.

#### Résolution
![[probleme1.py]]

## Le second problème
### Énoncé
N cerises, ayant chacune un poids - \[0, n\] avec p<sub>i</sub> poids de la cerise i
Objectif : Réalisation de 2 tas d'écart de poids minimum.

On définira ici l'écart comme la valeur absolue de la différence de poids des deux tas.

On note bp(k, e) la vérité de la proposition "Il existe une bi-partition des k-premières cerises, d'écart e".
L'écart minimum est la plus petite valeur e telle que bp(n, e) soit vrai.

Soit : calculer les valeurs bp(k, e) pour k dans \[0 : n+1] et e dans \[0 : sum(p<sub>i</sub>)]

### Réflection
bp(0,0) vrai, bp(0,e) faux sinon

bp(k,e) = bp(k-1, |e±p<sub>i</sub>|)

### Résolution
![[probleme2.py]]