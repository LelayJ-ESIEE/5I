longueur([], 0).
longueur([_X|Reste],N) :-
	longueur(Reste,M),
	N is M+1.

membre(X,[X|_Reste]).
membre(X,[_Y|Reste]) :- membre(X,Reste).

concat([],L2,L2).
concat([Head|Tail],L2,[Head|L3]) :-
concat(Tail,L2,L3).

initial(a).
terminal(a). terminal(b).
delta(a,0,a).
delta(a,1,b).
delta(b,0,c).
delta(b,1,b).
delta(c,0,c).
delta(c,1,c).
accepte(C) :-
	initial(Q0),
	deltaStar(Q0,C,Qt),
	terminal(Qt).
deltaStar(Q,[],Q).
deltaStar(Q1,[X|Reste],Q2) :-
	delta(Q1,X,Q),
	deltaStar(Q,Reste,Q2).