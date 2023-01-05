def CalculerM(V,T,C):
	"""
	Solution du problème du sac à dos

	Args:
		V liste des valeurs des objets
		T liste des tailles des objets

	Return:
		M[0:n+1][0:C+1] de terme général M[k][c] = m(k,c)
	"""
	M = [[0]*(C+1) for k in range(len(V)+1)]

	for c in range(C+1):
		M[0][c] = 0

	for k in range(1, len(V)+1):
		for c in range(0, C+1):
			if c >= T[k-1]:
				M[k][c] = max((M[k-1][c], M[k-1][c-T[k-1]] + V[k-1]))
			else:
				M[k][c] = M[k-1][c]
	
	return(M)

def SVM(M, V, T, k, c):
	if not k:
		return ""
	
	if M[k][c] > M[k-1][c]:
		S = SVM(M, V, T, k-1, c-T[k-1])
		res = "objet_" + str(k) + "(" + str(V[k-1]) + ", " + str(T[k-1]) + ")"
		if S :
			res += ", " + S
		return res
	
	return SVM(M, V, T, k-1, c)

def main():
	V = [1,2,3,7,30,10]
	T = [2,5,7,12,0,9]
	C = 15
	
	M = CalculerM(V,T,C)

	S = SVM(M, V, T, len(V), C)
	

	print("SVM de valeur " + str(M[-1][-1]) + " rempli à " + str(sum([int(i.replace(")","")) for i in (S.replace(" ", "").split(','))[1::2]])) + "/" + str(C) + " : " + S)

if __name__ == "__main__":
	main()