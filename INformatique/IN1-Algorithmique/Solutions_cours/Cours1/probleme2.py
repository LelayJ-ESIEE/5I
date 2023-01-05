def calculerBP(C, k):
	"""
	"""
	P = sum(C)
	BP = [[False]*(P+1) for k in range(len(C)+1)]
	BP[0][0] = True

	for k in range(0, len(C)):
		for e in range(0,P+1):
			if(BP[k][e]):
				# Optimiser !
				BP[k+1][e+C[k]] = True
				BP[k+1][abs(e-C[k])] = True
	return BP

def TEM(C, e):
	pass

def main():
	C = [1,2,3,4,5,6,7,8]
	BP = calculerBP(C, len(C))

	e = BP[-1].index(True)
	print(TEM(C,e))

if __name__ == "__main__":
	main()