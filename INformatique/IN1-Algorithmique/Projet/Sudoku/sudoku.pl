sudoku(PreRemplies, S) :-
	/* Preremplies est la liste des cases initialement remplies, 
	S est une grille remplie (résultat de la démonstration du but.) */
	grille(G0), /* G0 est une grille. Pour chaque case, les 9 valeurs possibles.*/
	enleverPreRemplies(PreRemplies, G0, G1),
	reduireParPreRemplies(PreRemplies, G1, G),
	resoudre(G, PreRemplies, S).

enleverPreRemplies(PreRemplies, G0, G1) :-
	/* G1 est l'ensemble des cases de G0 desquelles les pré-remplies ont été retirées */
	true.

reduireParPreRemplies(PreRemplies, G1, G) :-
	/* réduction des domaines des cases de G1 en fonction des valeurs des cases pré-remplies */
	true.

resoudre(G, PreRemplies, S) :-
	/* S est une grille dont toutes les cases ont été remplies (une valeur par case) */
	true.

grille(Grille) :- setof(V, between(1,9,V), Dij), /* Dij=[1,2,...,9] */
  setof(case(9,I,J,Dij), (between(1,9,I), between(1,9,J)), Grille). 
  /* Grille=[ case(9,1,1,[1,2,...,9]), ..., case(9,9,9,[1,2,...,9]) ] */

afficherGrille(G) :- /* afficher les lignes de la grille, ligne par ligne. */
	for( 	between(1,9,I), /* pour chaque ligne I, I entre 1 et 9... */
			(nl, afficherLigne(I,G)) /* afficher la ligne I de la grille G */
		).

afficherLigne(I,G) :- /* afficher la ligne I de la grille G. */
	for(	between(1,9,J),  /* pour chaque colonne J, J entre 1 et 9 */
			(member(case(_,I,J,Dij), G), /* pour chaque case (I,J) de la ligne I */
				afficherDomaine(Dij), write(' ')) ). /* afficher son domaine Dij */

afficherDomaine(Dij) :- /* si le domaine ne contient qu'une valeur : l'afficher */
	Dij = [V], write(V). 
afficherDomaine(Dij) :- /* si plus d'une valeur, afficher les 2 premières */
	Dij = [V1,V2|_], write("["), write(V1), write(","), write(V2), write(",...]"). 

/* Définition d'un ``for'' en Prolog : for(B,Faire). pour toute démonstration du but B, 
   demander la démonstration du but Faire.
   Remarque : la démonstration du but ``for'' réussit toujours. */
for(B,Faire) :- B, Faire, fail. 
for(_,_).  

/* Exemple d'exécution :
% swipl  <-- lancement de Prolog dans une fenête Unix
Welcome to SWI-Prolog (threaded, 64 bits, version 8.4.0)
SWI-Prolog comes with ABSOLUTELY NO WARRANTY. This is free software.
Please run ?- license. for legal details.

For online help and background, visit https://www.swi-prolog.org
For built-in help, use ?- help(Topic). or ?- apropos(Word).

?- [complement].
true.

?- grille(G), afficherGrille(G).

[1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] 
[1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] 
[1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] 
[1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] 
[1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] 
[1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] 
[1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] 
[1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] 
[1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] 
G = [case(9, 1, 1, [1, 2, 3, 4, 5, 6, 7|...]), case(9, 1, 2, [1, 2, 3, 4, 5, 6|...]), case(9, 1, 3, [1, 2, 3, 4, 5|...]), case(9, 1, 4, [1, 2, 3, 4|...]), case(9, 1, 5, [1, 2, 3|...]), case(9, 1, 6, [1, 2|...]), case(9, 1, 7, [1|...]), case(9, 1, 8, [...|...]), case(..., ..., ..., ...)|...].

?- G = [case(1, 1, 1, [1]), case(9, 1, 2, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 1, 3, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 1, 4, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 1, 5, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 1, 6, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 1, 7, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 1, 8, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 1, 9, [6]), case(9, 2, 1, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 2, 2, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 2, 3, [6]), case(9, 2, 4, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 2, 5, [2]), case(9, 2, 6, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 2, 7, [7]), case(9, 2, 8, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 2, 9, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 3, 1, [7]), case(1, 3, 2, [8]), case(1, 3, 3, [9]), case(1, 3, 4, [4]), case(1, 3, 5, [5]), case(9, 3, 6, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 3, 7, [1]), case(9, 3, 8, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 3, 9, [3]), case(9, 4, 1, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 4, 2, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 4, 3, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 4, 4, [8]), case(9, 4, 5, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 4, 6, [7]), case(9, 4, 7, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 4, 8, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 4, 9, [4]), case(9, 5, 1, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 5, 2, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 5, 3, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 5, 4, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 5, 5, [3]), case(9, 5, 6, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 5, 7, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 5, 8, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 5, 9, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 6, 1, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 6, 2, [9]), case(9, 6, 3, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 6, 4, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 6, 5, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 6, 6, [4]), case(1, 6, 7, [2]), case(9, 6, 8, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 6, 9, [1]), case(1, 7, 1, [3]), case(1, 7, 2, [1]), case(1, 7, 3, [2]), case(1, 7, 4, [9]), case(1, 7, 5, [7]), case(9, 7, 6, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 7, 7, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 7, 8, [4]), case(9, 7, 9, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 8, 1, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 8, 2, [4]), case(9, 8, 3, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 8, 4, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 8, 5, [1]), case(1, 8, 6, [2]), case(9, 8, 7, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 8, 8, [7]), case(1, 8, 9, [8]), case(1, 9, 1, [9]), case(9, 9, 2, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(1, 9, 3, [8]), case(9, 9, 4, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 9, 5, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 9, 6, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 9, 7, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 9, 8, [1, 2, 3, 4, 5, 6, 7, 8, 9]), case(9, 9, 9, [1, 2, 3, 4, 5, 6, 7, 8, 9])], afficherGrille(G).

1 [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] 6 
[1,2,...] [1,2,...] 6 [1,2,...] 2 [1,2,...] 7 [1,2,...] [1,2,...] 
7 8 9 4 5 [1,2,...] 1 [1,2,...] 3 
[1,2,...] [1,2,...] [1,2,...] 8 [1,2,...] 7 [1,2,...] [1,2,...] 4 
[1,2,...] [1,2,...] [1,2,...] [1,2,...] 3 [1,2,...] [1,2,...] [1,2,...] [1,2,...] 
[1,2,...] 9 [1,2,...] [1,2,...] [1,2,...] 4 2 [1,2,...] 1 
3 1 2 9 7 [1,2,...] [1,2,...] 4 [1,2,...] 
[1,2,...] 4 [1,2,...] [1,2,...] 1 2 [1,2,...] 7 8 
9 [1,2,...] 8 [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] [1,2,...] 
G = [case(1, 1, 1, [1]), case(9, 1, 2, [1, 2, 3, 4, 5, 6|...]), case(9, 1, 3, [1, 2, 3, 4, 5|...]), case(9, 1, 4, [1, 2, 3, 4|...]), case(9, 1, 5, [1, 2, 3|...]), case(9, 1, 6, [1, 2|...]), case(9, 1, 7, [1|...]), case(9, 1, 8, [...|...]), case(..., ..., ..., ...)|...].

?- G = [case(1, 1, 1, [1]), case(1, 1, 9, [6]), case(1, 2, 3, [6]), case(1, 2, 5, [2]), case(1, 2, 7, [7]), case(1, 3, 1, [7]), case(1, 3, 2, [8]), case(1, 3, 3, [9]), case(1, 3, 4, [4]), case(1, 3, 5, [5]), case(1, 3, 7, [1]), case(1, 3, 9, [3]), case(1, 4, 4, [8]), case(1, 4, 6, [7]), case(1, 4, 9, [4]), case(1, 5, 5, [3]), case(1, 6, 2, [9]), case(1, 6, 6, [4]), case(1, 6, 7, [2]), case(1, 6, 9, [1]), case(1, 7, 1, [3]), case(1, 7, 2, [1]), case(1, 7, 3, [2]), case(1, 7, 4, [9]), case(1, 7, 5, [7]), case(1, 7, 8, [4]), case(1, 8, 2, [4]), case(1, 8, 5, [1]), case(1, 8, 6, [2]), case(1, 8, 8, [7]), case(1, 8, 9, [8]), case(1, 9, 1, [9]), case(1, 9, 3, [8])],
grille(S), sudoku(G, S), afficherGrille(S).
*/