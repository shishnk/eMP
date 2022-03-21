import matplotlib.pyplot as plt

xB, xI, xD, yB, yI, yD = [], [], [], [], [], []

with open("points/boundaryPoints.txt") as file:
    for line in file:
        xC, yC = line.split()
        xB.append(float(xC))
        yB.append(float(yC))

with open("points/internalPoints.txt") as file:
    for line in file:
        xC, yC = line.split()
        xI.append(float(xC))
        yI.append(float(yC))

with open("points/dummyPoints.txt") as file:
    for line in file:
        xC, yC = line.split()
        xD.append(float(xC))
        yD.append(float(yC))

plt.grid()

plt.plot(xB, yB, 'o', color='r')
plt.plot(xI, yI, 'o', color='y')
plt.plot(xD, yD, 'o', color='b')
plt.show()