# TODO : À Corriger

def calculerM(G):
	n = len(G[0])
	S = len(G)
	m = [[0]*S for i in range(n)]

	for s in range(S):
		m[0][s] = G[s][0]

	for k in range(1, n):
		for s in range(S):
			# print([m[k-1][S-i-1] + G[i][k] for i in range(s+1)])
			m[k][s] = max([m[k-1][S-i-1] + G[i][k] for i in range(s+1)])
	
	return m

def main():
	# G = [
	# 		[0,0,0,0],
	# 		[5,2,4,8],
	# 		[10,4,6,12],
	# 		[10,6,10,14],
	# 		[10,8,12,15]
	# 	]
	G = [
			[0,0,0,0,0],
			[1,5, 4,2,3],
			[2,10,6,4,6],
			[3,15,8,6,9],
			[4,15,9,8,9],
			[5,15,9,8,9],
			[6,15,9,8,9],
			[7,15,9,8,9]
		]
	
	M = calculerM(G)

	for i in range(len(M)):
		print(M[i])

if __name__ == "__main__":
	main()