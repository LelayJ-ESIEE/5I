# from probleme2 import TEM

def evaluerBP(BP, C, k, e):
    if k >= len(C) : return
    if not e+C[k] in BP[k+1] :
        BP[k+1].add(e+C[k])
        evaluerBP(BP, C, k+1, e+C[k])
    if not abs(e-C[k]) in BP[k+1]:
        BP[k+1].add(abs(e-C[k]))
        evaluerBP(BP, C, k+1, abs(e-C[k]))

def calculerBP(C):
    BP = [set() for k in range(len(C)+1)]
    BP[0].add(0)
    print(BP)

    evaluerBP(BP, C, 0, 0)
    print(BP)

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
