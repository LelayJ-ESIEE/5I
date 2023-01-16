from probleme2 import TEM

def evaluerBP(BP, C, k, e): # TODO : List(List) -> List(Set())
	print("here")
	if k >= len(C) : return
	# for next_e in [e+C[k], abs(e-C[k])] :
	if BP[k+1][e+C[k]] != True:
		BP[k+1][e+C[k]] = True
		evaluerBP(BP, C, k+1, e+C[k])
	if BP[k+1][abs(e-C[k])] != True:
		BP[k+1][abs(e-C[k])] = True
		evaluerBP(BP, C, k+1, abs(e-C[k]))

def calculerBP(C):
	P = sum(C)
	BP = [[False]*(P+1) for k in range(len(C)+1)]
	BP[0][0] = True

	evaluerBP(BP, C, 0, 0)
	
	return BP

def main():
	# C = [0,1,2,3,4,0,5,6,7,8]
	# C = [2,2,3,1]
	# C = [1,1,1,2,3]
	C = [i for i in range(100)]
	
	BP = calculerBP(C)

# 	for l in BP:
# 		print(l)

	# print(TEM(C, BP))

if __name__ == "__main__":
	main()