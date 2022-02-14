import matplotlib.pyplot as plt

xB, xI, xD, yB, yI, yD = [], [], [], [], [], []

with open("boundaryPoints.txt") as file:
    for line in file:
        xC, yC = line.split()
        xB.append(float(xC))
        yB.append(float(yC))

with open("internalPoints.txt") as file:
    for line in file:
        xC, yC = line.split()
        xI.append(float(xC))
        yI.append(float(yC))

with open("dummyPoints.txt") as file:
    for line in file:
        xC, yC = line.split()
        xD.append(float(xC))
        yD.append(float(yC))

plt.plot(xB, yB, 'o', color='r')
plt.plot(xI, yI, 'o', color='y')
plt.plot(xD, yD, 'o', color='b')
plt.show()
