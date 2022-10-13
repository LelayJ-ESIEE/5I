## 4.2 Apprentissage des 4 classes

"""
Construit et entraîne 3 classificateurs binaires A,B,C de la manière suivante :
— le classificateur A décide si une image test appartient dans l’union des classes 1 et 2, ou celle des classes 3 et 4
— le classificateur B décide si une image test appartient à la classe 1 ou à la classe 2, sachant qu’elles ne peuvent pas appartenir à autre chose
— le classificateur C décide si une image test appartient à la classe 3 ou à la classe 4, sachant qu’elles ne peuvent pas appartenir à autre chose
Vous utiliserez C-SVC, avec le noyau et les paramètres de votre choix. En enchaînant les décisions A-B ou A-C pour aboutir à une classe finale, reportez les classifications obtenues pour chaque images de test non encore vue après les avoir vectorisées.
"""
pass