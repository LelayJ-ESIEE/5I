def calculerBP(C):
	P = sum(C)
	BP = [[False]*(P+1) for k in range(len(C)+1)]
	BP[0][0] = True

	for k in range(0, len(C)):
		for e in range(0,P+1):
			#print("here")
			if(BP[k][e]):
				# Optimiser !
				BP[k+1][e+C[k]] = True
				BP[k+1][abs(e-C[k])] = True
	return BP

def TEM(C, BP):
	if not C:
		return ([],[])

	e = BP[-1].index(True)
	print("Tas d'écart minimum e = " + str(e) + " : ")

	tas1 = ["Cerise_"+str(len(C)-1)]
	tas2 = []
	tem = (tas1, tas2)
	added_to = 1

	for k in range(-2, -(len(C)+1), -1):

		# if BP[k][e+C[k+1]]:
		# 	e = e+C[k+1]
		# 	added_to = 3 - added_to
		# 	tem[added_to-1].append("Cerise_"+str(len(C)+k))
		# elif BP[k][abs(e-C[k+1])]:
		# 	e = abs(e-C[k+1])
		# 	tem[added_to-1].append("Cerise_"+str(len(C)+k))
		# else : print("Oh Oh...")
		break

	return tem

def main():
	# C = [0,1,2,3,4,0,5,6,7,8]
	# C = [2,2,3,1]
	# C = [1,1,1,2,3]
	C = list(range(500))
	
	BP = calculerBP(C)

# 	for l in BP:
# 		print(l)

	# print(TEM(C, BP))

if __name__ == "__main__":
	main()