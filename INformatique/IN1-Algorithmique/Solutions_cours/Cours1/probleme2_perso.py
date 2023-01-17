# from probleme2 import TEM

def calculerBP(C):
    BP = [set() for k in range(len(C)+1)]
    BP[0].add(0)
    for k in range(len(C)):
        for e in BP[k]:
            next_k = k+1
            for next_e in [e+C[k], abs(e-C[k])]:
                if not next_e in BP[next_k]:
                    BP[next_k].add(next_e)
    return BP

def main():
    # C = [0,1,2,3,4,0,5,6,7,8]
    # C = [2,2,3,1]
    # C = [1,1,1,2,3]
    # C = list(range(986))
    C = list(range(500))

    BP = calculerBP(C)

#     for l in BP:
#         print(l)

    # print(TEM(C, BP))

if __name__ == "__main__":
    main()
